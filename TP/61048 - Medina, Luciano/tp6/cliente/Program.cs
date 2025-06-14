using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar el HttpClient para apuntar al servidor API (esto ya estaba bien)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184") });

// Registrar el servicio API (lo dejamos por si se usa en el futuro)
builder.Services.AddScoped<ApiService>();

// Registramos CarritoService como Singleton para que mantenga su estado
// (el ID del carrito) a través de todas las páginas de la aplicación.
builder.Services.AddSingleton<CarritoService>();

await builder.Build().RunAsync();