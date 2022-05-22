using Microsoft.Extensions.DependencyInjection;
using Ocelot.Logging;
using Ocelot.ServiceDiscovery;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;

namespace Ocelot.Provider.Docker
{
    public static class DockerProviderFactory
    {
        public static readonly ServiceDiscoveryFinderDelegate Get = (provider, config, route) =>
        {
            var factory = provider.GetService<IOcelotLoggerFactory>();
            var dockerFactory = provider.GetService<IDockerClientFactory>();
            var dockerRegistryConfiguration = new DockerRegistryConfiguration(config.Scheme, config.Host, route.ServiceName, config.Port);

            var dockerServiceDiscoveryProvider = new DockerServiceDiscoveryProvider(dockerRegistryConfiguration, factory!, dockerFactory!);
            return config.Type?.ToLower() == "polldocker"
                ? new PollDocker(config.PollingInterval, factory!, dockerServiceDiscoveryProvider)
                : new Docker(dockerServiceDiscoveryProvider);
        };

        private sealed class Docker : IServiceDiscoveryProvider
        {
            private readonly IServiceDiscoveryProvider serviceDiscoveryProvider;

            public Docker(IServiceDiscoveryProvider serviceDiscoveryProvider)
            {
                this.serviceDiscoveryProvider = serviceDiscoveryProvider;
            }

            public Task<List<Service>> Get()
            {
                return this.serviceDiscoveryProvider.Get();
            }
        }
    }
}