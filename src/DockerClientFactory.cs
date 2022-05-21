using Docker.DotNet;
using Docker.DotNet.Models;

namespace Ocelot.Provider.Docker
{
    /// <summary>
    /// The docker client factory.
    /// </summary>
    /// <seealso cref="IDockerClientFactory" />
    public class DockerClientFactory : IDockerClientFactory
    {
        /// <summary>
        /// Creates the docker client.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        private DockerClient CreateClient(DockerRegistryConfiguration config)
        {
            DockerClientConfiguration clientConfiguration;
            if (config.Port == 0)
                clientConfiguration = new DockerClientConfiguration(new Uri($"{config.Scheme}://{config.Host}"));
            else
                clientConfiguration = new DockerClientConfiguration(new Uri($"{config.Scheme}://{config.Host}:{config.Port}"));

            return clientConfiguration.CreateClient();
        }

        /// <summary>
        /// Lists the docker containers asynchronous.
        /// </summary>
        /// <param name="config">The docker configuration.</param>
        /// <returns></returns>
        public async Task<IList<ContainerListResponse>> ListContainersAsync(DockerRegistryConfiguration config)
        {
            var client = CreateClient(config);
            return (await client.Containers.ListContainersAsync(new ContainersListParameters()
            {
                All = true,
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    {
                        "status", new Dictionary<string, bool>
                        {
                            {"running", true}
                        }
                    }
                }
            })).Where(x => x.Labels.Any(x => x.Key == "ocelot.service" && x.Value == config.Service)).ToList();
        }
    }
}