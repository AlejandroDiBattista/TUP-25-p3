using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Text.Json;
using cliente.Models;

public class CarritoService : ICarritoService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public CarritoService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    private class CrearCarritoResponse
    {
        public Guid CarritoId { get; set; }
    }

    /// 
    public event Action? CarritoActualizado;
    private void NotificarCambio() => CarritoActualizado?.Invoke();
/// 
     

    private async Task<Guid> ObtenerOCrearCarritoIdAsync()
    {
        var carritoId = await _localStorage.GetItemAsync<Guid?>("carritoId");

        if (carritoId.HasValue && carritoId.Value != Guid.Empty)
        {
            return carritoId.Value;
        }

        var response = await _httpClient.PostAsync("carritos", null);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var crearCarritoResponse = JsonSerializer.Deserialize<CrearCarritoResponse>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (crearCarritoResponse == null || crearCarritoResponse.CarritoId == Guid.Empty)
        {
            throw new Exception("Error al crear el carrito. El servidor no devolvió un ID válido.");
        }

        await _localStorage.SetItemAsync("carritoId", crearCarritoResponse.CarritoId);

        return crearCarritoResponse.CarritoId;
    }


    public async Task<List<ItemCarritoResponse>> ObtenerItemsCarritoAsync()
    {
        var carritoId = await ObtenerOCrearCarritoIdAsync();

        var response = await _httpClient.GetAsync($"/carritos/{carritoId}");
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<ItemCarritoResponse>>();
        return items ?? new List<ItemCarritoResponse>();
    }

    public async Task AgregarAlCarritoAsync(int productoId)
{
    var carritoId = await ObtenerOCrearCarritoIdAsync();

    // Obtener el carrito actual
    var items = await ObtenerItemsCarritoAsync();
    var itemExistente = items.FirstOrDefault(i => i.ProductoId == productoId);
    var nuevaCantidad = (itemExistente?.Cantidad ?? 0) + 1;

    var response = await _httpClient.PutAsJsonAsync(
        $"/carritos/{carritoId}/{productoId}",
        new { Cantidad = nuevaCantidad }
    );

    response.EnsureSuccessStatusCode();
    NotificarCambio(); // 🔔
}

public async Task ActualizarCantidadAsync(int productoId, int nuevaCantidad)
{
    var carritoId = await ObtenerOCrearCarritoIdAsync();
    var response = await _httpClient.PutAsJsonAsync(
        $"/carritos/{carritoId}/{productoId}",
        new { Cantidad = nuevaCantidad }
    );
    response.EnsureSuccessStatusCode();
    NotificarCambio(); // 🔔
}

public async Task EliminarProductoAsync(int productoId)
{
    var carritoId = await ObtenerOCrearCarritoIdAsync();
    var response = await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}");
    response.EnsureSuccessStatusCode();
    NotificarCambio(); // 🔔
}

    public async Task VaciarCarritoAsync()
    {
        var carritoId = await ObtenerOCrearCarritoIdAsync();
        var response = await _httpClient.DeleteAsync($"/carritos/{carritoId}");
        response.EnsureSuccessStatusCode();
        NotificarCambio(); // 🔔
    }
 }

