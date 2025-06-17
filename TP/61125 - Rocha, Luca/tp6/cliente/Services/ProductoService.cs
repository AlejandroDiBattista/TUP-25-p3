using cliente.Modelos;
using System.Net.Http.Json;
using System.Net.Http;
using System;

namespace cliente.Services;

public class ProductoService
{
    private readonly HttpClient _http;

    public ProductoService(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        try
        {
            // 🔥 Verificamos que la API responde antes de intentar obtener datos
            var productos = await _http.GetFromJsonAsync<List<Producto>>("http://localhost:5184/api/productos");

            if (productos == null || productos.Count == 0)
            {
                Console.WriteLine("⚠️ No se encontraron productos en la API.");
                return new List<Producto>(); // Retornamos lista vacía para evitar errores en Blazor
            }

            Console.WriteLine($"✅ Productos cargados correctamente: {productos.Count}");
            return productos;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Error de conexión con la API: {ex.Message}");
            return new List<Producto>(); // Evita que Blazor se rompa
        }
        catch (Exception ex)
        {
            Console.WriteLine($"🚨 Error inesperado al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
    }
}