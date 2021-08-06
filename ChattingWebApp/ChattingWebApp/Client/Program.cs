using Fluxor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChattingWebApp.Client
{
    // If you are going to make a release erase the line #24
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddFluxor(config =>
            {
                config
                      .ScanAssemblies(typeof(Program).Assembly)
                      .UseReduxDevTools();
            });

            await builder.Build().RunAsync();
        }
    }
}
