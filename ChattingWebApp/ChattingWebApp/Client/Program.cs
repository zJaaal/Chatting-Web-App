using Blazored.LocalStorage;
using ChattingWebApp.Client.Handlers;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ChattingWebApp.Client.Store;

namespace ChattingWebApp.Client
{
    // If you are going to make a release erase the line #24
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();


            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            //Use this client for authorize your users to realize request to the API
            //Inject IHttpClientFactory and then do something like this
            //var client = http.CreateClient("AuthorizationClient");

            builder.Services.AddHttpClient("AuthorizationClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                            .AddHttpMessageHandler<CustomAuthorizationHandler>();

            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddTransient<CustomAuthorizationHandler>();

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
