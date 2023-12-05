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

internal class NatsMessageHandler<TMessage,TResponse, THandler>(THandler handler) 
    : IMessageHandler<NatsMsg<TMessage>,TResponse> where THandler : IHandle<TMessage,TResponse>
{
    private readonly THandler handler = handler;

    public async Task<TResponse> HandleAsync(NatsMsg<TMessage> message, CancellationToken cancellationToken = default)
    {
        if (message.ReplyTo != null)
        {
            Console.WriteLine($"ReplyTo: {message.ReplyTo}");
            var response = await handler.HandleAsync(message.Data, cancellationToken);
            await message.ReplyAsync(response, cancellationToken: cancellationToken);
        }
        
        // nothing to reply to
        throw new Exception("Nothing to respond to");
        return default;
    }
}