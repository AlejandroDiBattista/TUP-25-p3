using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient para apuntar al servidor API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5184/")
});

// Solo registra servicios, NO endpoints
builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<ProductoService>();
var app = builder.Build();
await app.RunAsync();