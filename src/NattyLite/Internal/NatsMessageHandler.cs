using NATS.Client.Core;

namespace NattyLite.Internal;

internal class NatsMessageHandler<TMessage, THandler>(THandler handler) 
    : IMessageHandler<NatsMsg<TMessage>> where THandler : IHandle<TMessage>
{
    private readonly THandler handler = handler;

    public async Task HandleAsync(NatsMsg<TMessage> message, CancellationToken cancellationToken = default)
    {
        if (message.ReplyTo != null)
        {
            Console.WriteLine($"ReplyTo: {message.ReplyTo}");
            await message.ReplyAsync(DateTime.Now.Ticks.ToString(), cancellationToken: cancellationToken);
        }
        
        await handler.HandleAsync(message.Data, cancellationToken);
    }
}