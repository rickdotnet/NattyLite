using NATS.Client.Core;
using NattyLite.Internal;
using NSubstitute;

namespace NattyLite.Tests;

public class NatsMessageHandlerTests
{
    private readonly IHandle<string> handle;
    private readonly NatsMessageHandler<string, IHandle<string>> messageHandler;

    public NatsMessageHandlerTests()
    {
        handle = Substitute.For<IHandle<string>>();
        messageHandler = new NatsMessageHandler<string, IHandle<string>>(handle);
    }

    [Fact]
    public async Task HandleAsync_ShouldForwardMessageToHandle()
    {
        // Arrange
        var message = new NatsMsg<string> { Data = "Test Message" };

        // Act
        await messageHandler.HandleAsync(message);

        // Assert
        await handle.Received().HandleAsync(message.Data, Arg.Any<CancellationToken>());
    }
}