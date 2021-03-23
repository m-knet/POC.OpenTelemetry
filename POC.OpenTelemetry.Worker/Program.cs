using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace POC.OpenTelemetry.Worker
{
    public static class Program
    {
        public static readonly string ApplicationName = typeof(Program).Assembly.GetName().Name;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOpenTelemetryTracing(c =>
                    {
                        c.SetResourceBuilder(ResourceBuilder.CreateEmpty().AddService(ApplicationName))
                            .AddSource(ApplicationName)
                            .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                            .AddHttpClientInstrumentation()
                            .AddConsoleExporter()
                            .AddZipkinExporter();

                        var instrumentationKey = hostContext.Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");

                        if (!string.IsNullOrEmpty(instrumentationKey))
                        {
                            c.AddAzureMonitorTraceExporter(az =>
                                az.ConnectionString = $"InstrumentationKey={instrumentationKey}");
                        }
                    });

                    services.AddHostedService<EventConsumer>();
                });
    }
}
