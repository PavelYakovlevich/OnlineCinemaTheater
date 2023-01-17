namespace Configurations.Consul;

public class ServiceConfiguration
{
    public string ServiceName { get; set; }

    public string ServiceId { get; set; }

    public Uri ServiceAddress { get; set; }

    public Uri ServiceDiscoveryAddress { get; set; }
}
