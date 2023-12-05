using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using NattyLite.Internal;
using NSubstitute;

namespace NattyLite.Tests;

public class NattyLiteBuilderTests
{
    private readonly IServiceCollection services;
    private readonly NattyLiteBuilder builder;

    public NattyLiteBuilderTests()
    {
        services = new ServiceCollection();
        var mockConnection = Substitute.For<INatsConnection>();
        services.AddSingleton(mockConnection);
        builder = new NattyLiteBuilder(services);
    }

    [Fact]
    public void AddNatsSubscriber_ShouldAddDependenciesCorrectly()
    {
        // Arrange
        const string subject = "TestSubject";

        // Act
        builder.AddSubscriber<TestHandler>(subject);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var handler = serviceProvider.GetRequiredService<IHandle<TestMessage>>();
        Assert.IsType<TestHandler>(handler);

        var messageHandler = serviceProvider.GetRequiredService<IMessageHandler<NatsMsg<TestMessage>>>();
        Assert.IsType<NatsMessageHandler<TestMessage, TestHandler>>(messageHandler);

        var subscriber = serviceProvider.GetRequiredService<INatsSubscriber>();
        Assert.IsType<NatsSubscriber<TestMessage>>(subscriber);
    }

    private class TestHandler : IHandle<TestMessage>
    {
        public Task HandleAsync(TestMessage message, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private class TestMessage { }
}