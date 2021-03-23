using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace POC.OpenTelemetry.API
{
    public static class Program
    {
        public static string ApplicationName => typeof(Program).Assembly.GetName().Name;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
