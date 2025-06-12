using System.Net.Http.Json;

namespace cliente.Services;

public class CarritoService {
    private readonly HttpClient _httpClient;

    private string carritoId;

    public CarritoService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<string> ObtenerOCrearCarritoAsync() {
        if (!string.IsNullOrEmpty(carritoId))
            return carritoId;

        var response = await _httpClient.PostAsync("/api/carritos", null);
        if (response.IsSuccessStatusCode) {
            carritoId = await response.Content.ReadAsStringAsync();
        } else {
            Console.WriteLine("Error al crear el carrito");
        }

        return carritoId;
    }

    public async Task<bool> AgregarProductoAsync(string productoId, int cantidad) {
        var id = await ObtenerOCrearCarritoAsync();

        var dto = new {
            ProductoId = productoId,
            Cantidad = cantidad
        };

        var response = await _httpClient.PutAsJsonAsync($"/api/carritos/{id}/{productoId}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<ItemCarritoDto>> ObtenerItemsDelCarritoAsync()
    {
        var id = await ObtenerOCrearCarritoAsync();
        var items = await _httpClient.GetFromJsonAsync<List<ItemCarritoDto>>($"/carritos/{id}");
        return items ?? new List<ItemCarritoDto>();
    }

    public async Task ModificarCantidadAsync(string productoId, int nuevaCantidad)
    {
        var id = await ObtenerOCrearCarritoAsync();
        var dto = new { Cantidad = nuevaCantidad };
        await _httpClient.PutAsJsonAsync($"/carritos/{id}/{productoId}", dto);
    }

    public async Task VaciarCarritoAsync()
    {
        var id = await ObtenerOCrearCarritoAsync();
        await _httpClient.GetAsync($"/carritos/vaciar/{id}");
    }

    public string ObtenerCarritoId() => carritoId;
}

public class ItemCarritoDto
{
    public string ProductoId { get; set; }
    public string Nombre { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
