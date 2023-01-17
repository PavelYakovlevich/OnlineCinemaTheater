namespace Configurations.MessageBroker;

public class MessageBrokerConfiguration
{
    public string UsedBrokerConfigurationName { get; set; }

    public RabbitMqConfiguration RabbitMqConfiguration { get; set; }

    public AzureServiceBusConfiguration AzureServiceBusConfiguration { get; set; }
}
