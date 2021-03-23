using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace POC.OpenTelemetry.Worker
{
    public class EventConsumer : BackgroundService
    {
        private static readonly ActivitySource ActivitySource = new(Program.ApplicationName);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var activity = ActivitySource.StartActivity("CONSUME MESSAGE", ActivityKind.Consumer);

                var @event = await ConsumeEvent();

                activity?.SetParentId(@event.ParentId);
                @activity?.SetTag("eventName", @event.Name);

                // Do something
                await Task.Delay(10000, stoppingToken);
            }
        }

        private static async Task<Event> ConsumeEvent()
        {
            // Read from an event bus
            await Task.Delay(100);

            return new Event
            {
                ParentId = "00-da681fe82d6ff94d87eba58dc50fb268-f9b3703d381d914d-01"
            };
        }

        private class Event
        {
            public string ParentId { get; init; }

            public string Name { get; } = Guid.NewGuid().ToString();
        }
    }
}
