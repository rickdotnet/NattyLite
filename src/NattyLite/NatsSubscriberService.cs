using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NattyLite.Internal;

namespace NattyLite;

public class NatsSubscriberService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscribers = serviceProvider.GetServices<INatsSubscriber>();

        // start all subscribers
        var tasks = subscribers.Select(subscriber => subscriber.SubscribeAsync());
        await Task.WhenAll(tasks);
    }
}