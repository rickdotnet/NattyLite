using Microsoft.Extensions.DependencyInjection;
using NATS.Client.Hosting;
using NATS.Client.Serializers.Json;

namespace NattyLite.Extensions.Hosting;

public static class ServiceCollectionExtensions
{
    public static NattyLiteBuilder AddNattyLite(this IServiceCollection services, NattyConfig nattyConfig)
    {
        // Configure NattyLite based on nattyConfig
        services.AddNats(configureOpts: opt => opt with
        {
            Url = nattyConfig.Url, 
            Name = "MyClient",
            SerializerRegistry = NatsJsonSerializerRegistry.Default
        });
        
        services.AddHostedService<NatsSubscriberService>();
        
        return new NattyLiteBuilder(services);
    }
}