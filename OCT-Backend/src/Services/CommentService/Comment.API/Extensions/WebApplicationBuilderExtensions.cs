using System.Text;
using MassTransit;
using Serilog;
using Configurations.Logging;
using Configurations.MessageBroker;
using Configurations.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Comment.API.Extensions;
using Comment.Contract.Services;
using Comment.Core;
using Comment.Contract.Repositories;
using Comment.Data.Repositories;
using System.Reflection;
using Comment.API.Consumers;
using Comment.Data.Extensions;
using AutoMapper;
using Comment.API.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace MediaInfo.API.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static void SetupAuthentication(this WebApplicationBuilder builder)
    {
        var jwtConfiguration = new JWTConfiguration();
        builder.Configuration.GetSection(nameof(JWTConfiguration)).Bind(jwtConfiguration);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfiguration.Issuer,

                    ValidateLifetime = true,

                    ValidateAudience = false,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(jwtConfiguration.Key))
                };
            });
    }

    public static void SetupAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(opt =>
        {
            opt.AddPolicy("UserOnly", p =>
            {
                p.RequireRole("User");
            });
        });
    }

    public static void SetupHeathCheck(this WebApplicationBuilder builder)
    {
        var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB");

        builder.Services.AddHealthChecks()
            .AddMongoDb(mongoConnectionString);
    }

    public static IServiceCollection SetupFluentValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(CreateCommentRequestValidator));
            
        return builder.Services.AddFluentValidation();
    }

    public static IServiceCollection SetupServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ICommentService, CommentService>();
        return builder.Services.AddTransient<IUserService, UserService>();
    }

    public static IServiceCollection SetupRepositories(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("MongoDB");

        builder.Services.AddTransient<ICommentRepository, CommentRepository>(f =>
            new CommentRepository(connectionString, f.GetRequiredService<IMapper>()));
        return builder.Services.AddTransient<IUserRepository, UserRepository>(f => 
            new UserRepository(connectionString, f.GetRequiredService<IMapper>()));
    }

    public static IServiceCollection SetupMassTransit(this WebApplicationBuilder builder)
    {
        var messageBrokerConfiguration = new MessageBrokerConfiguration();
        builder.Configuration.GetSection(nameof(MessageBrokerConfiguration)).Bind(messageBrokerConfiguration);

        builder.Services.AddSingleton(messageBrokerConfiguration);

        return builder.Services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetExecutingAssembly());

            switch (messageBrokerConfiguration.UsedBrokerConfigurationName)
            {
                case nameof(RabbitMqConfiguration):
                    UseRabbitMq(configure, messageBrokerConfiguration.RabbitMqConfiguration);
                    break;
                case nameof(AzureServiceBusConfiguration):
                    UseAzureServiceBus(configure, messageBrokerConfiguration.AzureServiceBusConfiguration);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown message broker configuration {messageBrokerConfiguration.UsedBrokerConfigurationName}");
            }
        });

        static void UseRabbitMq(IBusRegistrationConfigurator configurator, RabbitMqConfiguration rabbitMqConfiguration)
        {
            configurator.UsingRabbitMq((context, config) =>
            {
                config.Host(rabbitMqConfiguration.Host, hostConfig =>
                {
                    hostConfig.Username(rabbitMqConfiguration.Username);
                    hostConfig.Password(rabbitMqConfiguration.Password);
                });

                config.ReceiveEndpoint(MessageBrokerQueues.UserCreatedQueue, conf =>
                {
                    conf.ConfigureConsumer<UserCreatedMessageConsumer>(context);
                });

                config.ReceiveEndpoint(MessageBrokerQueues.UserUpdatedQueue, conf =>
                {
                    conf.ConfigureConsumer<UserUpdatedMessageConsumer>(context);
                });
            });
        }

        static void UseAzureServiceBus(IBusRegistrationConfigurator configurator, AzureServiceBusConfiguration azureServiceBusConfiguration)
        {
            configurator.UsingAzureServiceBus((context, config) =>
            {
                config.Host(azureServiceBusConfiguration.ConnectionString);

                config.ReceiveEndpoint(MessageBrokerQueues.UserCreatedQueue, conf =>
                {
                    conf.ConfigureConsumer<UserCreatedMessageConsumer>(context);
                });

                config.ReceiveEndpoint(MessageBrokerQueues.UserUpdatedQueue, conf =>
                {
                    conf.ConfigureConsumer<UserUpdatedMessageConsumer>(context);
                });
            });
        }
    }

    public static IServiceCollection SetupAutoMapper(this WebApplicationBuilder builder)
    {
        return builder.Services.AddAutoMapper(c =>
        {
            c.AddApiToModelsMapping();
            c.AddMessagesToModelsMapping();
            c.AddModelsToEntitesMapping();
        });
    }

    public static IServiceCollection SetupSerilog(this WebApplicationBuilder builder)
    {
        var serilogConfiguration = new SerilogConfiguration();

        builder.Configuration.GetSection(nameof(SerilogConfiguration)).Bind(serilogConfiguration);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
               Path.Combine("/", serilogConfiguration.LogsPath, serilogConfiguration.LogFileName),
               rollingInterval: serilogConfiguration.RollingInterval,
               fileSizeLimitBytes: serilogConfiguration.FileSizeLimitBytes,
               retainedFileCountLimit: serilogConfiguration.RetainedFileCountLimit,
               rollOnFileSizeLimit: serilogConfiguration.RollOnFileSizeLimit,
               shared: serilogConfiguration.Shared)
            .CreateLogger();

        return builder.Services.AddSingleton(serilogConfiguration);
    }
}
