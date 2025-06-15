using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar el HttpClient para apuntar al servidor API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184") });

// Registrar el servicio API
builder.Services.AddScoped<ApiService>();
builder.Services.AddSingleton<CarritoService>(); // Singleton porque debe mantenerse en memoria
builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped(sp => new CarritoService(sp.GetRequiredService<HttpClient>()));

builder.Services.AddScoped<Buscador>();

await builder.Build().RunAsync();
