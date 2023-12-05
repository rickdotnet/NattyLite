using NattyLite;

namespace NattyConsole;

public class TestHandler : IHandle<TestMessage>
{
    public Task HandleAsync(TestMessage message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Received: {(message?.Data ?? "null message")}");
        return Task.CompletedTask;
    }
}

public class TestMessage(string? data)
{
    public string Data { get; set; } = data ?? "default";

}