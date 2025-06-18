using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Registramos HttpClient para que esté disponible en toda la app.
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5184") // Asegúrate que el puerto sea el de tu servidor
});

// 2. Registramos CarritoService como "Scoped". 
//    Esto es suficiente para que no pierda su estado al navegar.
builder.Services.AddScoped<CarritoService>();

// (El registro de ApiService puede permanecer o quitarse)
builder.Services.AddScoped<ApiService>();


await builder.Build().RunAsync();
