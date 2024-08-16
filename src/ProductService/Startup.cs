using Confluent.Kafka;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductService.API.Services;
using ProductService.Context;
using ProductService.Events.Handlers;
using ProductService.Infrastructure.EventBus;
using System.Reflection;

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
        // services.AddSingleton<IEventHandler<ProductPurchasedEvent>, ProductPurchasedEventHandler>();
        // Add PostgreSQL database context
        services.AddDbContext<ProductContext>(options =>
            options.UseNpgsql(_configuration.GetConnectionString("ProductDatabase")));
        // Dependency Decleration
        services.AddScoped<IProductServiceAPI, ProductServiceAPI>();

        // Other services and configurations...
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddControllers();

        // Add Authentication Services
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = $"https://{_configuration["Auth0:Domain"]}/";
            options.Audience = _configuration["Auth0:Audience"];
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };

            // Enable logging
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                    return Task.CompletedTask;
                }
            };
        });

        // Add authorization
        services.AddAuthorization(options =>
        {
            options.AddPolicy("create:products", policy => policy.RequireClaim("permissions", "create:products"));
            options.AddPolicy("delete:products", policy => policy.RequireClaim("permissions", "delete:products"));
            options.AddPolicy("edit:products", policy => policy.RequireClaim("permissions", "edit:products"));
            options.AddPolicy("view:products", policy => policy.RequireClaim("permissions", "view:products"));

            options.AddPolicy("create:categories", policy => policy.RequireClaim("permissions", "create:categories"));
            options.AddPolicy("delete:categories", policy => policy.RequireClaim("permissions", "delete:categories"));
            options.AddPolicy("edit:categories", policy => policy.RequireClaim("permissions", "edit:categories"));
            options.AddPolicy("view:categories", policy => policy.RequireClaim("permissions", "view:categories"));
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

            // Add JWT Authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
        });
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
    }
}
