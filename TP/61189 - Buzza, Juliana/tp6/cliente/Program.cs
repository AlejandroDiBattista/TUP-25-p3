using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services; 

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7295/") });

builder.Services.AddSingleton<ApiService.CartStateService>();

builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();
