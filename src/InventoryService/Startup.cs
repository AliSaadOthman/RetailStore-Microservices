using InventoryService.Events.Handlers;
using ProductService.Events;
using ProductService.Events.Handlers;
using ProductService.Infrastructure.EventBus;
using System.Reflection;
using System.Text.RegularExpressions;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        // Get Kafka configuration from appsettings.json
        var kafkaConfig = _configuration.GetSection("Kafka");

        var bootstrapServers = kafkaConfig["BootstrapServers"];
        var groupId = kafkaConfig["GroupId"];
        var publishTopic = kafkaConfig["PublishTopic"];

        // Configure EventBus with Kafka settings
        if (bootstrapServers != null && groupId != null && publishTopic != null)
        {
            services.AddSingleton<IEventBus>(sp =>
                    new EventBusKafka(bootstrapServers, groupId, publishTopic));
        }
        // Event Handlers
        services.AddSingleton<IEventHandler<ProductDeletedEvent>, ProductDeletedEventHandler>();

        // Other services and configurations...
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        eventBus.Subscribe<ProductDeletedEvent, ProductDeletedEventHandler>("product_events");
    }
}
