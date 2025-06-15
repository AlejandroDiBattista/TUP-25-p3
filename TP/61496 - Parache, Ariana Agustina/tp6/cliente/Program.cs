using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;
using cliente.Models;
using System.Net.Http;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");


builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
});

builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<ProductoService>();

await builder.Build().RunAsync();
