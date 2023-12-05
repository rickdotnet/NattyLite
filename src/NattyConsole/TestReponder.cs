using NattyLite;

namespace NattyConsole;

public class TestResponder : IHandle<TestMessage, string>
{
    public async Task<string> HandleAsync(TestMessage message, CancellationToken cancellationToken = default)
    {
        return message.Data;
    }
}