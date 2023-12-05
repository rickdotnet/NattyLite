namespace NattyLite.Internal;

internal interface IMessageHandler<in T>
{
    Task HandleAsync(T message, CancellationToken cancellationToken = default);
}