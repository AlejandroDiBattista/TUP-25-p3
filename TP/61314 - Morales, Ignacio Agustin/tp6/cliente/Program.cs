using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Models;
// Add the following using if CarritoService is in another namespace, e.g. cliente.Services
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar el HttpClient para apuntar al servidor API (solo una vez, con barra al final)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5184/")
});

// Registrar el servicio API
builder.Services.AddScoped<ApiService>();
builder.Services.AddSingleton<CarritoService>();

await builder.Build().RunAsync();

