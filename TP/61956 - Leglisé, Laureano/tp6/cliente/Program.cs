using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// La URL tiene que tener "/" al final para que funcione correctamente
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184/") });

// Registrar los servicios
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<CarritoService>();

await builder.Build().RunAsync();