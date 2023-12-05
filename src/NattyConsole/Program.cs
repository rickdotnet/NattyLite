using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NattyConsole;
using NattyLite;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddNattyLite(new NattyConfig("nats://nats.rhinostack.com:4222"))
    .AddSubscriber<TestHandler>("foo")
    .AddResponder<TestResponder>("foo.responder");

// demo
builder.Services.AddHostedService<PublishingBackgroundService>();

var host = builder.Build();
await host.RunAsync();