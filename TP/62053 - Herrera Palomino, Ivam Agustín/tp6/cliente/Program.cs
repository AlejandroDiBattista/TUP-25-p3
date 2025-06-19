using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TiendaOnline.Frontend.Services;          // servicio de API
using TiendaOnline.Frontend;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5007")      // URL del backend
});

// Servicios propios
builder.Services.AddScoped<ApiClient>();
builder.Services.AddSingleton<CartState>();

await builder.Build().RunAsync();