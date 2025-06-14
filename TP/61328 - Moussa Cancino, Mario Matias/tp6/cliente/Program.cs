using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente;
using cliente.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// --- INICIA CORRECCIÓN ---
// HttpClient y ApiService ahora se registran como Singleton 
// para que el CartService (que también es Singleton) pueda usarlos.
builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5184") });
builder.Services.AddSingleton<ApiService>();
// --- FIN CORRECCIÓN ---

// CartService se mantiene como Singleton, lo cual es correcto.
builder.Services.AddSingleton<CartService>();

await builder.Build().RunAsync();