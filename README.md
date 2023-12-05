# NattyLite

Welcome to NattyLite. It's like your favorite lightweight beer, but for NATS.

## What is NattyLite?

NattyLite is a lightweight, no-nonsense, NATS-focused library. It's not trying to be the next agnostic service bus library. Nope, it's just here to make NATS even more enjoyable while still keeping it as lightweight and flexible as possible.

## Why NattyLite?

Why not? Here's what we have going on:

1. **NATS-focused:** All NATS, all the time. No fluff, no distractions.
    
2. **Lightweight:** NattyLite goes down easy, just like a refreshing light beer. It's light on resources, but packs a punch where it counts.
    
3. **Simplicity:** We're not about making things complicated. We're about making NATS as easy and fun to use as possible; just like cracking open a cold one with some friends.

## Status

NattyLite is currently in development. It's not ready for any use, let alone production, but I'm working on it. If you'd like to help out, please feel free to submit a PR.

## Usage

```cs
var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddNattyLite(new NattyConfig("nats://localhost:4222"))
    .AddNatsSubscriber<TestHandler>("foo");

// demo
builder.Services.AddHostedService<PublishingBackgroundService>();

var host = builder.Build();
await host.RunAsync();

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
```
