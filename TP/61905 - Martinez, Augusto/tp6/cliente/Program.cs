using cliente;
using Cliente.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 👉 HttpClient con base address del backend (Debe coincidir con la API)
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri("http://localhost:5184") 
});

// 👉 Registro de servicios (CarritoService como Scoped)
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<CarritoService>(); // 🔥 Cambiar a Scoped

await builder.Build().RunAsync();
