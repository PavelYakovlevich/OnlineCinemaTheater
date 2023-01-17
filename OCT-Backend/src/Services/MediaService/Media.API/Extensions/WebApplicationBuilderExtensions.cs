using Azure.Storage.Blobs;
using Azure.Storage;
using Configurations.Logging;
using Configurations.MessageBroker;
using MassTransit;
using Media.API.Consumers;
using Media.Contract.Services;
using Media.Core;
using Media.Data.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using Configurations.BlobStorage;
using BlobStorage.Abstractions;
using BlobStorage.Implementations;
using Media.Contract.Repositories;
using Media.Data.Repositories;
using Media.Data.Extensions;
using Configurations.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Media.API.Extensions;

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
            opt.AddPolicy("ModeratorOnly", policy => policy.RequireRole("Moderator"));
        });
    }

    public static IServiceCollection SetupServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ITrailerService, TrailerService>();
        builder.Services.AddTransient<IMediaContentService, MediaContentService>();
        return builder.Services.AddTransient<IMediaService, MediaService>();
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
                    UseRabbitMq(configure, messageBrokerConfiguration.RabbitMqConfiguration);
                    break;
                case nameof(AzureServiceBusConfiguration):
                    UseAzureServiceBus(configure, messageBrokerConfiguration.AzureServiceBusConfiguration);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown message broker configuration {messageBrokerConfiguration.UsedBrokerConfigurationName}");
            }

            configure.AddHealthChecks();
        });

        static void UseRabbitMq(IBusRegistrationConfigurator configurator, RabbitMqConfiguration rabbitMqConfiguration)
        {
            configurator.AddConsumers(Assembly.GetExecutingAssembly());

            configurator.UsingRabbitMq((context, config) =>
            {
                config.Host(rabbitMqConfiguration.Host, hostConfig =>
                {
                    hostConfig.Username(rabbitMqConfiguration.Username);
                    hostConfig.Password(rabbitMqConfiguration.Password);
                });

                config.ReceiveEndpoint(MessageBrokerQueues.MediaInfoCreatedQueue, conf =>
                {
                    conf.ConfigureConsumer<MediaInfoCreatedMesageConsumer>(context);
                });

                config.ReceiveEndpoint(MessageBrokerQueues.MediaInfoUpdatedQueue, conf =>
                {
                    conf.ConfigureConsumer<MediaInfoUpdatedMessageConsumer>(context);
                });

                config.ReceiveEndpoint(MessageBrokerQueues.MediaInfoDeletedQueue, conf =>
                {
                    conf.ConfigureConsumer<MediaInfoDeletedMessageConsumer>(context);
                });
            });
        }

        static void UseAzureServiceBus(IBusRegistrationConfigurator configurator, AzureServiceBusConfiguration azureServiceBusConfiguration)
        {
            configurator.AddConsumers(Assembly.GetExecutingAssembly());

            configurator.UsingAzureServiceBus((context, config) =>
            {
                config.Host(azureServiceBusConfiguration.ConnectionString);

                config.ReceiveEndpoint(MessageBrokerQueues.MediaInfoCreatedQueue, conf =>
                {
                    conf.ConfigureConsumer<MediaInfoCreatedMesageConsumer>(context);
                });

                config.ReceiveEndpoint(MessageBrokerQueues.MediaInfoUpdatedQueue, conf =>
                {
                    conf.ConfigureConsumer<MediaInfoUpdatedMessageConsumer>(context);
                });

                config.ReceiveEndpoint(MessageBrokerQueues.MediaInfoDeletedQueue, conf =>
                {
                    conf.ConfigureConsumer<MediaInfoDeletedMessageConsumer>(context);
                });
            });
        }
    }

    public static void SetupHealthCheck(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("SqlServer");
        var azuriteConnectionString = builder.Configuration.GetConnectionString("AzureStorage");

        builder.Services.AddHealthChecks()
            .AddSqlServer(connectionString)
            .AddAzureBlobStorage(azuriteConnectionString);

        // TODO: add consul health check.
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

    public static IServiceCollection SetupDb(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("SqlServer");

        builder.Services.AddTransient<IMediaInfoRepository, MediaInfoRepository>();
        builder.Services.AddTransient<IMediaRepository, MediaRepository>();

        return builder.Services.AddDbContext<MediaServiceDbContext>(options =>
        {
            options.UseSqlServer(connectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(MediaServiceDbContext).GetTypeInfo().Assembly.GetName().Name);
                builder.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), null);
            });
        });
    }

    public static IServiceCollection SetupBlobStorage(this WebApplicationBuilder builder)
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

    public static IServiceCollection SetupAutoMapper(this WebApplicationBuilder builder)
    {
        return builder.Services.AddAutoMapper(c =>
        {
            c.AddApiToModelsMapping();
            c.AddModelsToEntitesMapping();
        });
    }

    public static async Task IntializeAzureStorage(this WebApplication app)
    {
        var azureConfiguration = app.Services.GetRequiredService<AzureStorageConfiguration>();

        await EnsureContainerCreated(azureConfiguration.Address,
            azureConfiguration.Account,
            BlobContainersNames.Trailers,
            azureConfiguration.AccountKey);

        await EnsureContainerCreated(azureConfiguration.Address,
            azureConfiguration.Account,
            BlobContainersNames.MediasContent,
            azureConfiguration.AccountKey);
    }

    private static async Task EnsureContainerCreated(string address, string account, string container, string key)
    {
        var containerAddress = $"{address}/{account}/{container}";

        var client = new BlobContainerClient(
            new Uri(containerAddress),
            new StorageSharedKeyCredential(account, key));

        await client.CreateIfNotExistsAsync();

        await client.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
    }
}
