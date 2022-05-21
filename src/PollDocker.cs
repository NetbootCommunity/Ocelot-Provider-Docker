using Ocelot.Logging;
using Ocelot.ServiceDiscovery.Providers;
using Ocelot.Values;

namespace Ocelot.Provider.Docker
{
    public class PollDocker : IServiceDiscoveryProvider, IDisposable
    {
        private readonly IOcelotLogger _logger;
        private readonly IServiceDiscoveryProvider _dockerServiceDiscoveryProvider;
        private readonly Timer _timer;
        private bool _polling;
        private List<Service> _services;

        public PollDocker(int pollingInterval, IOcelotLoggerFactory factory, IServiceDiscoveryProvider dockerServiceDiscoveryProvider)
        {
            _logger = factory.CreateLogger<PollDocker>();
            _dockerServiceDiscoveryProvider = dockerServiceDiscoveryProvider;
            _services = new List<Service>();

            _timer = new Timer(async x =>
            {
                if (_polling)
                {
                    return;
                }

                _polling = true;
                await Poll();
                _polling = false;
            }, null, pollingInterval, pollingInterval);
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _logger.LogDebug("PollDocker disposed.");
        }

        public Task<List<Service>> Get()
        {
            return Task.FromResult(_services);
        }

        private async Task Poll()
        {
            _logger.LogDebug("Services updated.");
            _services = await _dockerServiceDiscoveryProvider.Get();
        }
    }
}