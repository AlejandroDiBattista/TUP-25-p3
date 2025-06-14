using cliente;
using Cliente.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 👉 HttpClient con base address del backend
builder.Services.AddScoped(sp => new HttpClient {
    BaseAddress = new Uri("http://localhost:5184") // Asegurate que coincida con tu backend
});

// 👉 Registro de servicios
builder.Services.AddScoped<ApiService>();
builder.Services.AddSingleton<CarritoService>();

await builder.Build().RunAsync();
