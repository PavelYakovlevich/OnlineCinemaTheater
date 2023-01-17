using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Serilog;
using FluentValidation.AspNetCore;
using Configurations.MessageBroker;
using Authentication.Core.Configuration;
using Authentication.Core.Generators;
using Authentication.Core.Services;
using Authentication.Contract.Generators;
using Authentication.Contract.Repositories;
using Authentication.Data.Repositories;
using Authentication.Contract.Services;
using Authentication.Data.Extensions;
using Authentication.Data.Context;
using Configurations.Logging;
using Infrastructure.MessageBroker.Implementations;
using Infrastructure.MessageBroker.Abstractions;

namespace Authentication.API.Extensions;

internal static class WebApplicationBuilderExtension
{
    public static void SetupHealthCheck(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("SqlServer");

        builder.Services.AddHealthChecks()
            .AddSqlServer(connectionString)
            .AddDiskStorageHealthCheck(options =>
            {
                options.CheckAllDrives = true;
            });
    }


    public static IServiceCollection SetupFluentValidation(this WebApplicationBuilder builder) =>
        builder.Services.AddFluentValidation(configure =>
        {
            configure.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        });

    public static IServiceCollection SetupApplicationConfiguration(this WebApplicationBuilder builder)
    {
        var appConfiguration = new ApplicationConfiguration();
        builder.Configuration.GetSection(nameof(ApplicationConfiguration)).Bind(appConfiguration);

        return builder.Services.AddSingleton(appConfiguration)
            .AddSingleton(appConfiguration.JwtConfiguration);
    }

    public static IServiceCollection SetupAuthService(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ITokenGenerator<Guid>, BCryptTokenGenerator>()
            .AddTransient<IAccountRepository, AccountRepository>()
            .AddTransient<IEmailConfirmationTokenRepository, EmailConfirmationTokenRepository>()
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddTransient<IAccountService, AccountService>()
            .AddTransient<IAuthenticationPublishService, RabbitMqAuthenticationPublishService>();

        return builder.Services.AddTransient<IAuthenticationService, AuthenticationService>()
            .AddTransient<IRefreshTokenRepository, RefreshTokenRepository>()
            .AddTransient<JWTTokenGenerator>();
    }

    public static IServiceCollection SetupLogger(this WebApplicationBuilder builder)
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

    public static IServiceCollection SetupAutoMapper(this WebApplicationBuilder builder) =>
        builder.Services.AddAutoMapper(config =>
        {
            config.ConfigApiToServicesModelsMapping();
            config.ConfigModelsToEntitesMapping();
        });

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

    public static IServiceCollection SetupDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("SqlServer");

        return builder.Services.AddDbContext<AuthenticationServiceDbContext>(options =>
        {
            options.UseSqlServer(connectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(AuthenticationServiceDbContext).GetTypeInfo().Assembly.GetName().Name);
                builder.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), null);
            });
        });
    }

    private static void UseRabbitMq(this IBusRegistrationConfigurator configurator, RabbitMqConfiguration rabbitMqConfiguration)
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

    private static void UseAzureServiceBus(this IBusRegistrationConfigurator configurator, AzureServiceBusConfiguration azureServiceBusConfiguration)
    {
        configurator.UsingAzureServiceBus((context, config) =>
        {
            config.Host(azureServiceBusConfiguration.ConnectionString);
        });
    }
}