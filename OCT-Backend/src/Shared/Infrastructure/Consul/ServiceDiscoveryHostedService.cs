using Configurations.Consul;
using Consul;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Consul;

public class ServiceDiscoveryHostedService : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly ServiceConfiguration _serviceConfiguration;
    private string _registrationId;

    public ServiceDiscoveryHostedService(IConsulClient consulClient, ServiceConfiguration serviceConfiguration)
    {
        _consulClient = consulClient;
        _serviceConfiguration = serviceConfiguration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _registrationId = $"{_serviceConfiguration.ServiceId}-{_serviceConfiguration.ServiceName}";

        var registration = new AgentServiceRegistration
        {
            ID = _registrationId,
            Name = _serviceConfiguration.ServiceName,
            Address = _serviceConfiguration.ServiceAddress.Host,
            Port = _serviceConfiguration.ServiceAddress.Port,
        };

        await _consulClient.Agent.ServiceDeregister(_registrationId, cancellationToken);

        await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken) =>
        await _consulClient.Agent.ServiceDeregister(_registrationId, cancellationToken);
}
