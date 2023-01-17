using Configurations.Consul;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Consul.Extensions;

public static class WebApplicationBuilderExtesions
{
    public static void RegisterInConsul(this IHostBuilder builder)
    {
        builder.ConfigureServices((config, services) =>
        {
            var configuration = new ServiceConfiguration();
            config.Configuration.GetSection(nameof(ServiceConfiguration)).Bind(configuration);

            services.AddSingleton(configuration);
            services.AddSingleton<IHostedService, ServiceDiscoveryHostedService>();
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(config =>
            {
                config.Address = configuration.ServiceDiscoveryAddress;
            }));
        });
    }
}
