using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar el HttpClient para apuntar al servidor API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184") });

// Registrar el servicio API
builder.Services.AddScoped<ApiService>(sp =>
{
    var http = sp.GetRequiredService<HttpClient>();
    var js = sp.GetRequiredService<IJSRuntime>();
    return new ApiService(http, js);
});

await builder.Build().RunAsync();
