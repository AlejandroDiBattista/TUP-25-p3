using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using d8;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registrar el servicio ContadoresServicio como singleton
builder.Services.AddSingleton<ContadoresServicio>();

var server = builder.Build();
await server.RunAsync();
