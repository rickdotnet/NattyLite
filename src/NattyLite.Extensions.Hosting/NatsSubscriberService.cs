using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NattyLite.Extensions.Hosting;

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