using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cliente.Services; // Asegúrate que el namespace coincida con el de CartService

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registro del servicio del carrito
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();