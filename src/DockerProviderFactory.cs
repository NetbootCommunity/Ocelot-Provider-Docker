using Microsoft.Extensions.DependencyInjection;
using Ocelot.Logging;
using Ocelot.ServiceDiscovery;

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
                   : dockerServiceDiscoveryProvider;
           };
    }
}