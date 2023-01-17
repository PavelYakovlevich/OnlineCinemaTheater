using AutoMapper;
using Configurations.Logging;
using Configurations.MessageBroker;
using MassTransit;
using Serilog;
using System.Reflection;
using User.API.Consumers;
using User.Contract.Repositories;
using User.Contract.Services;
using User.Core.Services;
using User.Data.Repositories;
using User.Data.Extensions;
using FluentMigrator.Runner;
using User.Data;
using FluentValidation.AspNetCore;
using FluentValidation;
using User.API.Validators;
using Configurations.BlobStorage;
using BlobStorage.Abstractions;
using BlobStorage.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Configurations.Authentication;
using System.Security.Claims;
using Infrastructure.MessageBroker.Abstractions;
using Infrastructure.MessageBroker.Implementations;

namespace User.API.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static void SetupAuthentication(this WebApplicationBuilder builder)
    {
        var jwtConfiguration = new JWTConfiguration();
        builder.Configuration.GetSection(nameof(JWTConfiguration)).Bind(jwtConfiguration);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = ValidateId,
                };

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

    public static void SetupHealthCheck(this WebApplicationBuilder builder)
    {
        var sqlServerConnectionString = builder.Configuration.GetConnectionString("SqlServer");
        var azureStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorage");

        builder.Services.AddHealthChecks()
            .AddSqlServer(sqlServerConnectionString)
            .AddAzureBlobStorage(azureStorageConnectionString);
    }

    public static IServiceCollection SetupFluentValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(ChangeUserRequestValidator));

        return builder.Services.AddFluentValidation();
    }
        
    public static IServiceCollection SetupImageService(this WebApplicationBuilder builder)
    {
        var imageServiceConfiguration = new BlobServiceConfiguration();
        builder.Configuration.GetSection(nameof(BlobServiceConfiguration)).Bind(imageServiceConfiguration);
        builder.Services.AddSingleton(imageServiceConfiguration);

        var azureStorageConfiguration = new AzureStorageConfiguration();
        builder.Configuration.GetSection(nameof(AzureStorageConfiguration)).Bind(azureStorageConfiguration);
        builder.Services.AddSingleton(azureStorageConfiguration);

        return builder.Services.AddTransient<BlobsServiceBase<IFormFile>, AzureBlobsService>(f => 
            new AzureBlobsService(imageServiceConfiguration, azureStorageConfiguration));
    }

    public static IServiceCollection SetupUserService(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IUserPublishService, RabbitMqUserPublishService>();
        return builder.Services.AddTransient<IUserService, UserService>();
    }

    public static IServiceCollection SetupAutoMapper(this WebApplicationBuilder builder)
    {
        return builder.Services.AddAutoMapper(c =>
        {
            c.AddModelsToEntitiesMapping();
            c.AddApiToModelsMapping();
        });
    }

    public static IServiceCollection SetupDb(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("SqlServer");
        var masterConnectionString = builder.Configuration.GetConnectionString("SqlServerMaster");

        builder.Services.AddLogging(c => c.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.GetAssembly(typeof(Database))).For.All());

        builder.Services.AddTransient(f => new Database(masterConnectionString));

        return builder.Services.AddTransient<IUserRepository, UserRepository>(fact =>
            new UserRepository(connectionString, fact.GetRequiredService<IMapper>()));
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

    public static IServiceCollection SetupMassTransit(this WebApplicationBuilder builder)
    {
        var messageBrokerConfiguration = new MessageBrokerConfiguration();
        builder.Configuration.GetSection(nameof(MessageBrokerConfiguration)).Bind(messageBrokerConfiguration);

        builder.Services.AddSingleton(messageBrokerConfiguration);

        return builder.Services.AddMassTransit(configure =>
        {
            switch (messageBrokerConfiguration.UsedBrokerConfigurationName)
            {
                case nameof(RabbitMqConfiguration):
                    configure.UseRabbitMq(messageBrokerConfiguration.RabbitMqConfiguration);
                    break;
                case nameof(AzureServiceBusConfiguration):
                    configure.UseAzureServiceBus(messageBrokerConfiguration.AzureServiceBusConfiguration);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown message broker configuration {messageBrokerConfiguration.UsedBrokerConfigurationName}");
            }
        });
    }

    private static void UseRabbitMq(this IBusRegistrationConfigurator configurator, RabbitMqConfiguration rabbitMqConfiguration)
    {
        configurator.AddConsumers(Assembly.GetExecutingAssembly());

        configurator.UsingRabbitMq((context, config) =>
        {
            config.Host(rabbitMqConfiguration.Host, hostConfig =>
            {
                hostConfig.Username(rabbitMqConfiguration.Username);
                hostConfig.Password(rabbitMqConfiguration.Password);
            });

            config.ReceiveEndpoint(MessageBrokerQueues.AccountCreatedQueue, conf =>
            {
                conf.ConfigureConsumer<CreateUserMessageConsumer>(context);
            });
        });
    }

    private static void UseAzureServiceBus(this IBusRegistrationConfigurator configurator, AzureServiceBusConfiguration azureServiceBusConfiguration)
    {
        configurator.UsingAzureServiceBus((context, config) =>
        {
            config.Host(azureServiceBusConfiguration.ConnectionString);
        });
    }

    private static Task ValidateId(TokenValidatedContext context)
    {
        if (!context.HttpContext.Request.RouteValues.TryGetValue("id", out var value))
        {
            return Task.CompletedTask;
        }

        var id = (string)value;

        var identity = context.Principal?.Identities?.FirstOrDefault();
        var idClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim is null)
        {
            context.Fail("User id claim was not found");
            return Task.CompletedTask;
        }

        if (id != idClaim.Value)
        {
            context.Fail($"User id from claim {idClaim.Value} is not equal to route id {id}");
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}
