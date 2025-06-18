using System.Net.Http.Json;
using cliente.Modelos;

namespace cliente.Services
{
    public class ProductoService
    {
        private readonly HttpClient _http;

        public List<Producto> Productos { get; private set; } = new();

        public ProductoService(HttpClient http)
        {
            _http = http;
        }

        public async Task ObtenerProductosAsync(string? searchTerm = null)
    {
        string url = "/productos";
        if (!string.IsNullOrEmpty(searchTerm))
        {
            // Codifica el término de búsqueda para la URL
            url += $"?q={Uri.EscapeDataString(searchTerm)}";
        }

        try
        {
            Productos = (await _http.GetFromJsonAsync<List<Producto>>(url)) ?? new List<Producto>();
            
            Console.WriteLine($"DEBUG: Productos cargados. Cantidad: {Productos?.Count ?? 0}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            Productos = new List<Producto>(); // Asegurarse de que no sea null
        }
    }
        
    }

    
}

