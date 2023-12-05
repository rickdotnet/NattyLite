namespace NattyLite.Internal;

internal interface IMessageHandler<in T>
{
    Task HandleAsync(T message, CancellationToken cancellationToken = default);
}
internal interface IMessageHandler<in T, TResponse>
{
    Task<TResponse> HandleAsync(T message, CancellationToken cancellationToken = default);
}