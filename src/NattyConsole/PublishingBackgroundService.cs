using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NATS.Client.Core;

namespace NattyConsole;

public class PublishingBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var conn = serviceProvider.GetRequiredService<INatsConnection>();

        while(!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(2000, stoppingToken);
            Console.WriteLine("Publishing message");
            
            await conn.PublishAsync("foo", new TestMessage("Hey!"), cancellationToken: stoppingToken);
            
            var test = await conn.RequestAsync<TestMessage,string>("foo.responder", new TestMessage("Hey!"), cancellationToken: stoppingToken);
            Console.WriteLine($"Got response: {test.Data}");
        }
    }
}