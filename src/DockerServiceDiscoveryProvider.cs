using Docker.DotNet.Models;
using Ocelot.Logging;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;

namespace Ocelot.Provider.Docker
{
    public class DockerServiceDiscoveryProvider : IServiceDiscoveryProvider
    {
        private readonly DockerRegistryConfiguration _dockerConfiguration;
        private readonly IOcelotLogger _logger;
        private readonly IDockerClientFactory _dockerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DockerServiceDiscoveryProvider"/> class.
        /// </summary>
        /// <param name="dockerRegistryConfiguration">The docker registry configuration.</param>
        /// <param name="factory">The ocelot logger factory.</param>
        /// <param name="dockerFactory">The docker client factory.</param>
        public DockerServiceDiscoveryProvider(DockerRegistryConfiguration dockerRegistryConfiguration, IOcelotLoggerFactory factory, IDockerClientFactory dockerFactory)
        {
            _dockerConfiguration = dockerRegistryConfiguration;
            _logger = factory.CreateLogger<DockerServiceDiscoveryProvider>();
            _dockerClient = dockerFactory;
        }

        /// <summary>
        /// Get service configurations for ocelot.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public async Task<List<Service>> Get()
        {
            var services = new List<Service>();
            var containers = await _dockerClient.ListContainersAsync(_dockerConfiguration);
            foreach (var container in containers)
            {
                try { services.Add(ContainerToService(container)); }
                catch (Exception ex) { _logger.LogWarning(ex.Message); }
            }
            return services;
        }

        /// <summary>
        /// Converts the container configuration to ocelot service.
        /// </summary>
        /// <param name="container">The container configuration.</param>
        private static Service ContainerToService(ContainerListResponse container)
        {
            // Service
            var service = container.Labels.FirstOrDefault(x => x.Key == "ocelot.service").Value;
            if (service == null)
                throw new ArgumentException(nameof(service));

            // Network Address
            var networkAddress = string.Empty;
            var network = container.Labels.FirstOrDefault(x => x.Key == "ocelot.network").Value;
            if (string.IsNullOrEmpty(network))
            {
                var networkConfig = container.NetworkSettings.Networks.FirstOrDefault();
                if (networkConfig.Equals(default(KeyValuePair<string, EndpointSettings>)))
                    throw new ArgumentException(nameof(networkConfig));
                networkAddress = networkConfig.Value.IPAddress;
            }
            else
            {
                var networkConfig = container.NetworkSettings.Networks.FirstOrDefault(x => x.Key == network);
                if (networkConfig.Equals(default(KeyValuePair<string, EndpointSettings>)))
                    throw new ArgumentException(nameof(networkConfig));
                networkAddress = networkConfig.Value.IPAddress;
            }

            // Network Port
            int networkPort;
            var rawPort = container.Labels.FirstOrDefault(x => x.Key == "ocelot.port").Value;
            if (rawPort == null) networkPort = 80;
            else networkPort = Convert.ToInt32(rawPort);

            // Return service configuration
            return new Service(
                name: container.Names.First(), hostAndPort: new ServiceHostAndPort(networkAddress, networkPort),
                id: container.ID, version: String.Empty, tags: Enumerable.Empty<string>());
        }
    }
}