namespace NattyLite;

public interface IHandle
{
    
}
public interface IHandle<in T> : IHandle
{
    Task HandleAsync(T message, CancellationToken cancellationToken = default);
}

public interface IHandle<in TMessage, TResponse> : IHandle
{
    Task<TResponse> HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}