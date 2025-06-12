using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using cliente.Models;

namespace cliente.Services;

public class CarritoService
{
    private readonly HttpClient _httpClient;
    private Carrito _carrito;

    private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public CarritoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task InicializarCarritoAsync()
    {
        if (_carrito != null)
            return;

        try
        {
            var response = await _httpClient.PostAsJsonAsync<Carrito>(
                "/carritos",
                null,
                _jsonOptions
            );
            response.EnsureSuccessStatusCode();
            _carrito = await response.Content.ReadFromJsonAsync<Carrito>(_jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al inicializar el carrito: {ex.Message}");
        }
    }

    public int GetCantidadItems()
    {
        if (_carrito == null)
            return 0;

        return _carrito.Items?.Count ?? 0;
    }

    public async Task ObtenerCarritoAsync()
    {
        try
        {
            _carrito = await _httpClient.GetFromJsonAsync<Carrito>($"/carritos/{_carrito.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener el carrito: {ex.Message}");
        }
    }

    public async Task AgregarItemACarritoAsync(int productoId)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync<Carrito>(
                $"/carritos/{_carrito.Id}/{productoId}",
                null
            );
            response.EnsureSuccessStatusCode();
            _carrito = await response.Content.ReadFromJsonAsync<Carrito>(_jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al agregar el item al carrito: {ex.Message}");
        }
    }

    public async Task EliminarItemDeCarritoAsync(int productoId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/carritos/{_carrito.Id}/{productoId}");
            response.EnsureSuccessStatusCode();
            _carrito = await response.Content.ReadFromJsonAsync<Carrito>(_jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar el item del carrito: {ex.Message}");
        }
    }

    public async Task ConfirmarCompraAsync(CompraDto compraDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"/carritos/{_carrito.Id}/confirmar",
                compraDto,
                _jsonOptions
            );
            response.EnsureSuccessStatusCode();
            _carrito = null; // Limpiar el carrito después de confirmar la compra
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al confirmar la compra: {ex.Message}");
        }
    }

    public async Task VaciarCarritoAsync()
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/carritos/{_carrito.Id}");
            response.EnsureSuccessStatusCode();
            _carrito.Items = new List<ItemCarrito>(); // Limpiar los items del carrito
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al vaciar el carrito: {ex.Message}");
        }
    }
}
