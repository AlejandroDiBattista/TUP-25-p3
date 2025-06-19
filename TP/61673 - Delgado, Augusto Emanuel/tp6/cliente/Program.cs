using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184/") });
builder.Services.AddSingleton<CarritoService>(); 
builder.Services.AddScoped<ApiService>(); 
builder.Services.AddBlazoredLocalStorageAsSingleton();

await builder.Build().RunAsync();