using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;

namespace Ocelot.Provider.Docker
{
    public static class OcelotBuilderExtensions
    {
        /// <summary>
        /// Adds docker provider in ocelot solution.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IOcelotBuilder AddDocker(this IOcelotBuilder builder)
        {
            builder.Services.AddSingleton(DockerProviderFactory.Get);
            builder.Services.AddSingleton<IDockerClientFactory, DockerClientFactory>();
            return builder;
        }
    }
}