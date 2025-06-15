using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente.Servicios;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184") });
builder.Services.AddScoped<ServicioApi>();
builder.Services.AddSingleton<EstadoCarrito>();
await builder.Build().RunAsync();