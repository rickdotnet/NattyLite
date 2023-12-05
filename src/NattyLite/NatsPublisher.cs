using NATS.Client.Core;

namespace NattyLite;

public class NatsPublisher
{
    private readonly INatsConnection connection;

    public NatsPublisher(INatsConnection connection)
    {
        this.connection = connection;
    }

    public ValueTask PublishAsync<T>(string subject, T message, CancellationToken cancellationToken = default)
        => connection.PublishAsync(subject, message, cancellationToken: cancellationToken);

    // TODO: this supports one message responses, but, we could support multiple responses
    public async ValueTask<TResponse> RequestAsync<TRequest, TResponse>(
        string subject,
        TRequest request,
        int timeout = 60, // seconds
        CancellationToken cancellationToken = default)
    {
        var replyOpts = new NatsSubOpts
        {
            Timeout = TimeSpan.FromSeconds(timeout),
        };
        
        var response =
            await connection.RequestAsync<TRequest, TResponse>(
                subject,
                request,
                replyOpts: replyOpts,
                cancellationToken: cancellationToken
            );
        
        return response.Data!;
    }
}