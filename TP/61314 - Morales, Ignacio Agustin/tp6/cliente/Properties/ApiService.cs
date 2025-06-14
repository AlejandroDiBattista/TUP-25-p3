using System.Net.Http.Json;
using cliente.Models;
using cliente.shared;

namespace cliente.Models;

public class ApiService
{
    private readonly HttpClient _http;
    public Guid CarritoId { get; set; } = Guid.NewGuid();

    public ApiService(HttpClient http)
    {
        _http = http;

        if (_http.BaseAddress == null)
        {
            _http.BaseAddress = new Uri("https://localhost:5184/"); 
        }
    }

    public async Task<List<Producto>> ObtenerProductos()
    {
        return await _http.GetFromJsonAsync<List<Producto>>("productos");
    }

    public async Task CrearCarrito(Producto productos)
    {
        var response = await _http.PostAsJsonAsync("carrito", productos);
        var carrito = await response.Content.ReadFromJsonAsync<CarritoResponse>();
        CarritoId = carrito.Id;
    }

    public async Task<CarritoResponse> ObtenerCarrito()
    {
        return await _http.GetFromJsonAsync<CarritoResponse>($"carrito/{CarritoId}");
    }

    public async Task AgregarProducto(int productoId)
    {
        await _http.PutAsync($"carrito/{CarritoId}/{productoId}", null);
    }

    public async Task EliminarProducto(int productoId)
    {
        await _http.DeleteAsync($"carrito/{CarritoId}/{productoId}");
    }

    public async Task ConfirmarCompra(Compra compras)
    {
        await _http.PutAsJsonAsync($"carrito/{CarritoId}/confirmar", compras);
    }

    public async Task<bool> RestarStockProducto(int productoId)
    {
        var response = await _http.PutAsync($"productos/{productoId}/restar-stock", null);
        return response.IsSuccessStatusCode;
    }
    

    public class CarritoResponse
    {
        public Guid Id { get; set; }
        public List<ItemCompra> Items { get; set; }
    }

    public async Task<DatosRespuesta> ObtenerDatosAsync()
    {
        return await _http.GetFromJsonAsync<DatosRespuesta>("datos");
    }
}