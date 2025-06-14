using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar el HttpClient para apuntar al servidor API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184/") });

// Registrar el servicio API
builder.Services.AddBlazoredLocalStorage(config => { config.JsonSerializerOptions.PropertyNamingPolicy = null; });
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<CarritoStateService>();

await builder.Build().RunAsync();
