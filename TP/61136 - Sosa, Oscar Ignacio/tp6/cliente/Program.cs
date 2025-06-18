using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cliente;
using Cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app"); // <- Aquí usa el componente App
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<CartService>();
builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();
