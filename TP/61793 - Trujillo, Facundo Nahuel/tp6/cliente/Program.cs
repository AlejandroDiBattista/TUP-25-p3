using Cliente;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cliente.Services;
using Cliente.Modelo;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar el HttpClient para apuntar al servidor API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184") });
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<CartService>();
// Registrar el servicio API si lo usas
// builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();
