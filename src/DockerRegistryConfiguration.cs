namespace Ocelot.Provider.Docker
{
    public class DockerRegistryConfiguration
    {
        /// <summary>
        /// The scheme.
        /// </summary>
        public string Scheme { get; }

        /// <summary>
        /// The host.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// The port.
        /// </summary>
        public int Port { get; }

        public string Service { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DockerRegistryConfiguration" /> class.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public DockerRegistryConfiguration(string scheme, string host, string service, int port)
        {
            Host = string.IsNullOrEmpty(host) ? "localhost" : host;
            Port = port;
            Scheme = string.IsNullOrEmpty(scheme) ? "tcp" : scheme;
            Service = service;
        }
    }
}