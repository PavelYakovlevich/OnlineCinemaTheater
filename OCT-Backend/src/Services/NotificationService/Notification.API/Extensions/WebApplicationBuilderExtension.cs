using AutoMapper;
using Configurations.MessageBroker;
using MassTransit;
using NLog;
using NLog.Web;
using Notification.API.Consumers;
using Notification.Contract;
using Notification.Core;
using Notification.Core.Configuration;
using Notification.Core.Extensions;
using Notification.Core.Services;
using Notification.Email.MailKit;
using Notification.Email.SendGrid;
using System.Reflection;

namespace Notification.API.Extensions;

internal static class WebApplicationBuilderExtension
{
    public static void SetupHealthCheck(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddDiskStorageHealthCheck(options =>
            {
                options.CheckAllDrives = true;
            });

    public static IServiceCollection SetupAutoMapper(this WebApplicationBuilder builder) =>
        builder.Services.AddScoped(fact => new MapperConfiguration(config =>
        {
            config.AddMessagesToModelsMapping();
        })
        .CreateMapper());

    public static IServiceCollection SetupMassTransit(this WebApplicationBuilder builder)
    {
        var rabbitMqConfiguration = new RabbitMqConfiguration();
        builder.Configuration.GetSection(nameof(RabbitMqConfiguration)).Bind(rabbitMqConfiguration);

        builder.Services.AddSingleton(rabbitMqConfiguration);

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

    public static IServiceCollection SetupLogger(this WebApplicationBuilder builder) =>
        builder.Services.AddSingleton(fact => LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger());

    public static IServiceCollection SetupNotificationService(this WebApplicationBuilder builder)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));

        var emailConfig = GetEmailConfiguration(builder);

        builder.Services.AddTransient<INotificationService<EmailDetails>, EmailNotificationService>();

        return emailConfig.SenderSource switch
        {
            nameof(emailConfig.MailKit) => AddMailKitSender(builder, emailConfig),
            nameof(emailConfig.SendGrid) => AddSendGridSender(builder, emailConfig),
            _ => throw new InvalidOperationException($"Unknown email sender name: {emailConfig.SenderSource}")
        };

        static EmailConfiguration GetEmailConfiguration(WebApplicationBuilder builder)
        {
            var emailConfiguration = new EmailConfiguration();

            builder.Configuration.GetSection(nameof(EmailConfiguration))
                .Bind(emailConfiguration);

            builder.Services.AddSingleton(emailConfiguration);

            return emailConfiguration;
        }
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

            config.ReceiveEndpoint(MessageBrokerQueues.ConfirmEmailQueue, conf =>
            {
                conf.ConfigureConsumer<SendConfirmationEmailMessageConsumer>(context);
            });

            config.ReceiveEndpoint(MessageBrokerQueues.ChangePasswordEmailQueue, conf =>
            {
                conf.ConfigureConsumer<SendPasswordChangeEmailMessageConsumer>(context);
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

    private static IServiceCollection AddSendGridSender(WebApplicationBuilder builder, EmailConfiguration configuration) =>
        builder.Services.AddTransient<IEmailSender, SendGridEmailSender>(factory => new SendGridEmailSender(configuration.SendGrid.APIKey));

    private static IServiceCollection AddMailKitSender(WebApplicationBuilder builder, EmailConfiguration configuration) =>
        builder.Services.AddTransient<IEmailSender, MailKitEmailSender>(factory => 
            new MailKitEmailSender(configuration.MailKit.Login, configuration.MailKit.Password, configuration.MailKit.SenderName));
}
