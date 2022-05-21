using Docker.DotNet.Models;

namespace Ocelot.Provider.Docker
{
    public interface IDockerClientFactory
    {
        Task<IList<ContainerListResponse>> ListContainersAsync(DockerRegistryConfiguration config);
    }
}