using AutoMapper;
using System.Reflection;
using FluentMigrator.Runner;
using FluentValidation;
using FluentValidation.AspNetCore;
using BlobStorage.Abstractions;
using BlobStorage.Implementations;
using MediaInfo.Data.Extensions;
using MediaInfo.Contract.Services;
using MediaInfo.Core.Services;
using MediaInfo.Data;
using MediaInfo.Contract.Repositories;
using MediaInfo.Data.Repositories;
using Configurations.Logging;
using Serilog;
using Azure.Storage.Blobs;
using Azure.Storage;
using Configurations.BlobStorage;
using Configurations.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MediaInfo.API.Validators.Participant;
using Configurations.MessageBroker;
using MassTransit;
using Infrastructure.MessageBroker.Abstractions;
using Infrastructure.MessageBroker.Implementations;
using MediaInfo.Domain.Extensions;

namespace MediaInfo.API.Extensions;

internal static class WebApplicationBuilderExtensions
{
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
            });
        }

        static void UseAzureServiceBus(IBusRegistrationConfigurator configurator, AzureServiceBusConfiguration azureServiceBusConfiguration)
        {
            configurator.UsingAzureServiceBus((context, config) =>
            {
                config.Host(azureServiceBusConfiguration.ConnectionString);
            });
        }
    }

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

    public static void SetupHealthCheck(this WebApplicationBuilder builder)
    {
        var sqlServerConnectionString = builder.Configuration.GetConnectionString("Postgres");
        var azureStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorage");

        builder.Services.AddHealthChecks()
            .AddNpgSql(sqlServerConnectionString)
            .AddAzureBlobStorage(azureStorageConnectionString);
    }

    public static IServiceCollection SetupFluentValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(ParticipantRequestValidator));

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

    public static IServiceCollection SetupParticipantService(this WebApplicationBuilder builder) =>
        builder.Services.AddTransient<IParticipantService, ParticipantService>();

    public static IServiceCollection SetupMediaInfoService(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMediaInfoPublishService, RabbitMqMediaInfoPublishService>();
        return builder.Services.AddTransient<IMediaInfoService, MediaInfoService>();
    }

    public static IServiceCollection SetupGenreService(this WebApplicationBuilder builder) =>
        builder.Services.AddTransient<IGenreService, GenreService>();

    public static IServiceCollection SetupAutoMapper(this WebApplicationBuilder builder)
    {
        return builder.Services.AddAutoMapper(c =>
        {
            c.AddApiToModelsMapping();
            c.AddModelsToEntitiesMapping();
            c.AddCoreModelsMapping();
        });
    }

    public static IServiceCollection SetupDb(this WebApplicationBuilder builder)
    {
        var mediaInfoDBConnectionString = builder.Configuration.GetConnectionString("Postgres");
        var masterDBConnectionString = builder.Configuration.GetConnectionString("PostgresMaster");

        builder.Services.AddLogging(c => c.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c
                .AddPostgres()
                .WithGlobalConnectionString(mediaInfoDBConnectionString)
                .ScanIn(Assembly.GetAssembly(typeof(Database))).For.All());

        builder.Services.AddTransient(f => new Database(masterDBConnectionString));

        builder.Services.AddTransient<IGenreRepository, GenreRepository>(fact =>
            new GenreRepository(mediaInfoDBConnectionString, fact.GetRequiredService<IMapper>()));
        builder.Services.AddTransient<IMediaInfoRepository, MediaInfoRepository>(fact =>
            new MediaInfoRepository(mediaInfoDBConnectionString, fact.GetRequiredService<IMapper>()));
        return builder.Services.AddTransient<IParticipantRepository, ParticipantRepository>(fact =>
            new ParticipantRepository(mediaInfoDBConnectionString, fact.GetRequiredService<IMapper>()));
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

    public static async Task IntializeAzureStorage(this WebApplication app)
    {
        var azureConfiguration = app.Services.GetRequiredService<AzureStorageConfiguration>();

        await EnsureContainerCreated(azureConfiguration.Address,
            azureConfiguration.Account,
            BlobContainersNames.ParticipantsProfilePictures,
            azureConfiguration.AccountKey);

        await EnsureContainerCreated(azureConfiguration.Address,
            azureConfiguration.Account,
            BlobContainersNames.MediaInfoPictures,
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
