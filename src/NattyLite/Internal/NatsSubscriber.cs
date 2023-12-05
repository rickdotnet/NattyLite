using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;

namespace NattyLite.Internal;

internal class NatsSubscriber<T> : INatsSubscriber
{
    private readonly INatsConnection connection;
    private readonly string subject;
    private readonly string? queueGroup;
    private readonly IServiceProvider serviceProvider;
    //private readonly IMessageHandler<NatsMsg<T>> handler;
    private readonly INatsDeserialize<T>? serializer;
    private readonly NatsSubOpts? opts;
    private readonly CancellationToken cancellationToken;
    

    internal NatsSubscriber(
        IServiceProvider serviceProvider,
        string subject,
        string? queueGroup,
        INatsDeserialize<T>? serializer,
        NatsSubOpts? opts = default,
        CancellationToken cancellationToken = default)
    {
        this.serviceProvider = serviceProvider;
        this.connection = serviceProvider.GetRequiredService<INatsConnection>();
        this.subject = subject;
        this.queueGroup = queueGroup;
        
        this.serializer = serializer ?? serviceProvider.GetService<INatsDeserialize<T>>();
        this.opts = opts;
        this.cancellationToken = cancellationToken;
    }

    public async Task SubscribeAsync()
    {
        // TODO: figure out serializer
        await foreach (var msg in connection.SubscribeAsync(subject, queueGroup, serializer, opts, cancellationToken))
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<NatsMsg<T>>>();
            await handler.HandleAsync(msg, cancellationToken);
        }
    }
}