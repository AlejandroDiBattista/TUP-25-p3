using System.Net.Http.Json;

public class ProductoService
{
    private readonly HttpClient _http;

    public ProductoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Producto>> ObtenerProductos(string? busqueda = null)
    {
        var url = "/productos";
        if (!string.IsNullOrWhiteSpace(busqueda))
            url += $"?q={Uri.EscapeDataString(busqueda)}";
        return await _http.GetFromJsonAsync<List<Producto>>(url) ?? new();
    }

    public async Task<bool> ActualizarProducto(int id, Producto producto)
    {
        var response = await _http.PutAsJsonAsync($"/productos/{id}", producto);
        return response.IsSuccessStatusCode;
    }

    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string ImagenUrl { get; set; }
    }
}