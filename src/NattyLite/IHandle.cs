namespace NattyLite;

public interface IHandle
{
    
}
public interface IHandle<in T> : IHandle
{
    Task HandleAsync(T message, CancellationToken cancellationToken = default);
}