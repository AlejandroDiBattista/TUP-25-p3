using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cliente;
using Cliente.Services;
using Cliente.Modelos;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar el HttpClient para apuntar al backend API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5184") // Cambialo si usás otro puerto
});

builder.Services.AddScoped<ApiService>();
builder.Services.AddSingleton<CarritoFinal>();

await builder.Build().RunAsync();