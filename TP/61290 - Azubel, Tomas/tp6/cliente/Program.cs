using System;
using System.Net.Http;
using cliente;
using cliente.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:5184")
});

builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<CartStorage>();

await builder.Build().RunAsync();