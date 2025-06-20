using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient para llamar al servidor
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184/") });

// Registrar servicios
builder.Services.AddScoped<ApiService>();
builder.Services.AddSingleton<CarritoService>(); 

await builder.Build().RunAsync();
