using cliente;
using cliente.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 👇 Acá tenés que poner la URL real de tu backend, incluyendo el puerto correcto
builder.Services.AddScoped(sp => new HttpClient {
    BaseAddress = new Uri("http://localhost:5184") // Reemplazá con tu puerto real
});

builder.Services.AddScoped<ApiService>(); // Registrar tu ApiService

await builder.Build().RunAsync();
