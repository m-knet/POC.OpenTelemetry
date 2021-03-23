using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace POC.OpenTelemetry.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOpenTelemetryTracing(c =>
            {
                c.SetResourceBuilder(ResourceBuilder.CreateEmpty().AddService(Program.ApplicationName))
                    .AddSource(Program.ApplicationName)
                    .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter()
                    .AddZipkinExporter();

                var instrumentationKey = _configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");

                if (!string.IsNullOrEmpty(instrumentationKey))
                {
                    c.AddAzureMonitorTraceExporter(az =>
                        az.ConnectionString = $"InstrumentationKey={instrumentationKey}");
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    using var httpClient = new HttpClient();

                    await Greeter.SayHelloAsync();

                    await httpClient.GetAsync("https://google.com");

                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapGet("/exception", async context =>
                {
                    await Greeter.ThrowException();

                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }

    public static class Greeter
    {
        private static readonly ActivitySource ActivitySource = new(Program.ApplicationName);

        public static async Task SayHelloAsync()
        {
            using var activity = ActivitySource.StartActivity(nameof(SayHelloAsync));

            activity?.SetTag("greeter", "hello");

            await Task.CompletedTask;

            activity.SetStatus(Status.Ok);
        }

        public static async Task ThrowException()
        {
            using var activity = ActivitySource.StartActivity(nameof(ThrowException));
            activity?.SetTag("greeter", "exception");

            try
            {
                await Task.CompletedTask;
                throw new InvalidOperationException("Greeter exception");
            }
            catch (Exception ex)
            {
                activity.RecordException(ex);
                activity.SetStatus(Status.Error);
                throw;
            }
        }
    }
}
