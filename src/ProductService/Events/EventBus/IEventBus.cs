using ProductService.Events.Handlers;
using System;

namespace ProductService.Infrastructure.EventBus
{
    public interface IEventBus
    {
        void Publish<T>(T @event) where T : class;
        void Subscribe<T, TH>(string topic)
            where T : class
            where TH : IEventHandler<T>, new();
        void Unsubscribe<T, TH>()
            where T : class
            where TH : IEventHandler<T>;
        void ConsumeEvents<T, TH>(CancellationToken cancellationToken) where T : class where TH : IEventHandler<T>, new();
    }
}
