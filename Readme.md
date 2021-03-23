# POC - OpenTelemetry - DotNet

Proof of concept about how to 

## Requirements

- [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0)

## Optional

- Docker (to run [zipkin](https://hub.docker.com/r/openzipkin/zipkin))
- An instance of Azure Application Insights

## Discovering traces

Traces can be discovered in Console, Zipkin and ApplicationInsights.

To use Zipkin you can to run an instance with docker:

`docker run -d -p 9411:9411 openzipkin/zipkin-slim`

To use ApplicationInsights the InstrumenationKey should be specified in the appsettings (or in user secrets)

```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "Set in user secrets"
  }
}
```

## Interesting stuff

- [W3C TraceContext](https://www.w3.org/TR/trace-context/)

- [Deep Dive into Open Telemetry for .NET](https://rehansaeed.com/deep-dive-into-open-telemetry-for-net/)

-  [ActivitySource and OpenTelemetry 1.0](https://jimmybogard.com/building-end-to-end-diagnostics-activitysource-and-open/)

- [ASP.NET Core OpenTelemetry Logging](https://carlos.mendible.com/2021/01/08/asp.net-core-opentelemetry-logging/)

- [Netcoreconf - Introduction to OpenTelemetry](https://www.youtube.com/watch?v=eVOl6W7d8HU) (Spanish)

- [Azure.Monitor.OpenTelemetry.Exporter](https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/monitor/Azure.Monitor.OpenTelemetry.Exporter)