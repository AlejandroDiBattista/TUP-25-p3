using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar el HttpClient para apuntar al servidor API
// IMPORTANTE: La URL debe terminar con "/" para que las rutas relativas funcionen
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184/") });

// Registrar los servicios
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<CarritoService>();

await builder.Build().RunAsync();