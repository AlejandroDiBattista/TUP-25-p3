using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184") });

builder.Services.AddScoped<ApiService>();

// --- CAMBIO CLAVE AQUÍ ---
// Cambiamos de Singleton a Scoped para que coincida con el ciclo de vida de HttpClient.
builder.Services.AddScoped<CarritoService>();

await builder.Build().RunAsync();