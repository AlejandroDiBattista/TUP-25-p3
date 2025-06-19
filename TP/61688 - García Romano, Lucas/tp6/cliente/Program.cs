using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);// Crea el host de la aplicación Blazor

builder.RootComponents.Add<App>("#app");// Agrega el componente raíz de la aplicación Blazor

builder.RootComponents.Add<HeadOutlet>("head::after");// Agrega el componente HeadOutlet para manejar el head del HTML

// Configurar el HttpClient para apuntar al servidor API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184") });

// Registrar el servicio API
builder.Services.AddScoped<ApiService>();// Registrar el servicio de API

builder.Services.AddScoped<ProductoService>();// Registrar el servicio de producto

builder.Services.AddScoped<CarritoService>();//registrar el servicio de carrito


await builder.Build().RunAsync();// Ejecuta la aplicación Blazor
