using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using NattyLite.Internal;
using NSubstitute;

namespace NattyLite.Tests;

public class NatsSubscriberTests
{
    private INatsConnection connection;
    private IMessageHandler<NatsMsg<string>> handler;
    private NatsSubscriber<string> natsSubscriber;
    
    private void SetupNatsSubscriber()
    {
        connection = Substitute.For<INatsConnection>();
        handler = Substitute.For<IMessageHandler<NatsMsg<string>>>();
        
        var serviceProvider = Substitute.For<IServiceProvider>();
        var serviceScope = Substitute.For<IServiceScope>();
        var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
        
        serviceScope.ServiceProvider.Returns(serviceProvider);
        serviceScopeFactory.CreateScope().Returns(serviceScope);
        serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(serviceScopeFactory);
        serviceProvider.GetService(typeof(INatsConnection)).Returns(connection);
        serviceProvider.GetService(typeof(IMessageHandler<NatsMsg<string>>)).Returns(handler);
        
        natsSubscriber = new NatsSubscriber<string>(serviceProvider, "subject", null, null);
    }

    [Fact]
    public async Task SubscribeAsync_ShouldHandleIncomingMessages()
    {
        // Arrange
        SetupNatsSubscriber();
        var message = new NatsMsg<string> { Data = "Test Message" };
        var messages = new List<NatsMsg<string>> { message };
        connection.SubscribeAsync<string>("subject").Returns(messages.ToAsyncEnumerable());

        // Act
        await natsSubscriber.SubscribeAsync();

        // Assert
        await handler.Received().HandleAsync(message, Arg.Any<CancellationToken>());
    }

    private (NatsSubscriber<string> subscriber, IMessageHandler<NatsMsg<string>> handler) SetupNatsSubscriberWithConnection(INatsConnection connection)
    {
        var handler = Substitute.For<IMessageHandler<NatsMsg<string>>>();
        var serviceProvider = Substitute.For<IServiceProvider>();
        var serviceScope = Substitute.For<IServiceScope>();
        var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();

        serviceScope.ServiceProvider.Returns(serviceProvider);
        serviceScopeFactory.CreateScope().Returns(serviceScope);
        serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(serviceScopeFactory);
        serviceProvider.GetService(typeof(INatsConnection)).Returns(connection);
        serviceProvider.GetService(typeof(IMessageHandler<NatsMsg<string>>)).Returns(handler);

        var subscriber = new NatsSubscriber<string>(serviceProvider, "subject", null, null);

        return (subscriber, handler);
    }

    [Fact]
    public async Task MultipleSubscribers_ShouldHandleIncomingMessagesConcurrently()
    {
        // Arrange
        var message1 = new NatsMsg<string> { Data = "Test Message 1" };
        var message2 = new NatsMsg<string> { Data = "Test Message 2" };

        var messages1 = new List<NatsMsg<string>> { message1 };
        var messages2 = new List<NatsMsg<string>> { message2 };

        var connection1 = Substitute.For<INatsConnection>();
        connection1.SubscribeAsync<string>("subject").Returns(messages1.ToAsyncEnumerable());

        var connection2 = Substitute.For<INatsConnection>();
        connection2.SubscribeAsync<string>("subject").Returns(messages2.ToAsyncEnumerable());

        var (natsSubscriber1, handler1) = SetupNatsSubscriberWithConnection(connection1);
        var (natsSubscriber2, handler2) = SetupNatsSubscriberWithConnection(connection2);

        // Act
        var task1 = natsSubscriber1.SubscribeAsync();
        var task2 = natsSubscriber2.SubscribeAsync();

        await Task.WhenAll(task1, task2);

        // Assert
        await handler1.Received().HandleAsync(message1, Arg.Any<CancellationToken>());
        await handler2.Received().HandleAsync(message2, Arg.Any<CancellationToken>());
    }
}