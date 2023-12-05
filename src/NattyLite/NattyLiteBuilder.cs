using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Core;
using NattyLite.Internal;

namespace NattyLite;

public class NattyLiteBuilder(IServiceCollection services)
{
    public NattyLiteBuilder AddSubscriber<THandler, TMessage>(
        string subject, 
        string? queueGroup = default, 
        INatsDeserialize<TMessage>? serializer = default, 
        NatsSubOpts? opts = default)
        where THandler : class, IHandle<TMessage> 
        where TMessage : class
    {
        services.AddScoped<THandler>();
        services.AddScoped<IHandle<TMessage>>(x=>x.GetRequiredService<THandler>());
        services.AddScoped<IMessageHandler<NatsMsg<TMessage>>, NatsMessageHandler<TMessage, THandler>>();
        services.AddSingleton<INatsSubscriber>(
            sp => new NatsSubscriber<TMessage>(sp, subject, queueGroup, serializer, opts));
        services.AddSingleton<NatsPublisher>();
        return this;
    }
}
public static class NattyLiteBuilderExtensions
{
    public static NattyLiteBuilder AddSubscriber<THandler>(
        this NattyLiteBuilder builder, 
        string subject, 
        string? queueGroup = default, 
        NatsSubOpts? opts = default) where THandler : class, IHandle
    {
        var handlerType = typeof(THandler);
        var handleInterface = handlerType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandle<>));
        var messageType = handleInterface.GetGenericArguments()[0];
        var method = typeof(NattyLiteBuilder).GetMethod(nameof(NattyLiteBuilder.AddSubscriber));
        var genericMethod = method.MakeGenericMethod(new[] { handlerType, messageType });
        genericMethod.Invoke(builder, new object[] { subject, queueGroup, null, opts });

        return builder;
    }
}