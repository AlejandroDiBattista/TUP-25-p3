using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services; 

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { 
    // Revisa que este sea el puerto de tu backend
    BaseAddress = new Uri("http://localhost:5184") 
});

builder.Services.AddScoped<ApiService>();
builder.Services.AddSingleton<EstadoCarrito>();

await builder.Build().RunAsync();