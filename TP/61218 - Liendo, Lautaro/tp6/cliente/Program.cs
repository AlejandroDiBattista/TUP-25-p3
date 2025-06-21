// Program.cs (Cliente)
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using cliente; // Asegúrate de que este sea el namespace de tu proyecto cliente.
using cliente.Services; // Importa tu namespace de servicios.

namespace cliente
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // Configura HttpClient para usar la URL base de tu backend.
            // Asegúrate de que esta URL coincida con la URL de tu servidor backend.
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5184/")
            });

            // Registra tu servicio de API como Scoped.
            builder.Services.AddScoped<ApiService>();

            // Registra el servicio de estado del carrito como Singleton para que su estado sea compartido globalmente.
            builder.Services.AddSingleton<CartStateService>();

            await builder.Build().RunAsync();
        }
    }
}
