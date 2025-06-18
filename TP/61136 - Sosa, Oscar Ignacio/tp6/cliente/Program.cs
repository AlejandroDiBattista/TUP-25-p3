using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ✅ Servicio del carrito debe ser Singleton en Blazor WebAssembly
builder.Services.AddSingleton<CartService>();

// Otros servicios pueden seguir siendo Scoped si usan HttpClient, etc.
builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();
// Nota: Asegúrate de que el archivo Program.cs esté en la raíz del proyecto Cliente y que las rutas de los componentes sean correctas.
// También asegúrate de que los servicios ApiService y CartService estén correctamente implementados y registrados.