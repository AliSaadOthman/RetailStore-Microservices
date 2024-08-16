using Confluent.Kafka;
using Newtonsoft.Json;
using ProductService.Events.Handlers;
using System;
using static Confluent.Kafka.ConfigPropertyNames;
using System.Threading;

namespace ProductService.Infrastructure.EventBus
{
    public class EventBusKafka : IEventBus
    {
        private readonly string _publishTopic;
        private readonly string _groupId;
        private readonly string _bootstrapServers;
        private IConsumer<Ignore, string>? _consumer;
        private CancellationTokenSource? _cancellationTokenSource;

        public EventBusKafka(string bootstrapServers, string groupID, string publishTopic)
        {
            _bootstrapServers = bootstrapServers;
            _groupId = groupID;
            _publishTopic = publishTopic;
        }

        public void Publish<T>(T @event) where T : class
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var message = new Message<Null, string>
                {
                    Value = JsonConvert.SerializeObject(@event)
                };

                producer.Produce(_publishTopic, message);
            }
        }

        public void Subscribe<T, TH>(string topic)
                    where T : class
                    where TH : IEventHandler<T>, new()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _consumer.Subscribe(topic);
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => ConsumeEvents<T, TH>(_cancellationTokenSource.Token)); 
        }

        public void Unsubscribe<T, TH>()
            where T : class
            where TH : IEventHandler<T>
        {
            _cancellationTokenSource?.Cancel();
            _consumer?.Close();
            _consumer?.Dispose();
        }

        public void ConsumeEvents<T, TH>(CancellationToken cancellationToken)
                    where T : class
                    where TH : IEventHandler<T>, new()
        {
            var eventHandler = new TH();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer?.Consume(cancellationToken);
                        if (consumeResult != null && consumeResult.Message != null)
                        {
                            var @event = JsonConvert.DeserializeObject<T>(consumeResult.Message.Value);
                            if (@event != null)
                            {
                                // Log the message consumption
                                Console.WriteLine($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'");

                                eventHandler.Handle(@event);

                                // Optionally commit the offset manually
                                _consumer?.Commit(consumeResult);
                            }
                            else
                            {
                                // Log or handle the case where deserialization resulted in a null object
                                Console.WriteLine("Failed to deserialize message");
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Consume error: {e.Error.Reason}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _consumer?.Close();
            }
        }

    }
}
