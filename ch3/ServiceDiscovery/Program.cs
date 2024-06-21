using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul; //add package

namespace ServiceDiscovery;


class Program
{
    static void Main(string[] args)
    {
        new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config
                .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsentings.json", true, true)
                .AddJsonFile($"appsentings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddJsonFile("ocelot.json")
                .AddEnvironmentVariables();
        })
        .ConfigureServices(s => {
            s.AddOcelot()
            AddConsul(); // add consul config
        })
        .ConfigureLogging((hostingContext, logging) => {
            // add your logging
        })
        .UseIISIntegration()
        .Configure(app => {
            app.UseOcleto().Wait();
        })
        .Build()
        .Run();
    }
}
