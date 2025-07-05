using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// apuntando API
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("http://localhost:5184/") });

// Servicios
builder.Services.AddScoped<TiendaService>();
builder.Services.AddScoped<CarritoService>(); 

await builder.Build().RunAsync();
