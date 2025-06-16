using System.Net.Http.Json;
using cliente.Dtos;
using Microsoft.JSInterop;

namespace cliente.Services;

public class CarritoService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    private Guid? _carritoId;

    public CarritoService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<List<ItemCompraDto>> ObtenerCarrito()
    {
        await AsegurarCarritoId();

        var response = await _http.GetAsync($"carritos/{_carritoId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            await CrearCarritoDeNuevo();
            return new List<ItemCompraDto>();
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ItemCompraDto>>() ?? new();
    }

    public async Task AgregarProducto(int productoId)
    {
        await AsegurarCarritoId();

        var response = await _http.PutAsync($"carritos/{_carritoId}/{productoId}", null);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            await CrearCarritoDeNuevo();
            response = await _http.PutAsync($"carritos/{_carritoId}/{productoId}", null);
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task QuitarProducto(int productoId)
    {
        await AsegurarCarritoId();

        var response = await _http.DeleteAsync($"carritos/{_carritoId}/{productoId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            await CrearCarritoDeNuevo();
            return;
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task VaciarCarrito()
    {
        await AsegurarCarritoId();

        var response = await _http.DeleteAsync($"carritos/{_carritoId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            await CrearCarritoDeNuevo();
            return;
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task ConfirmarCompra(dtos.ClienteDto cliente)
    {
        await AsegurarCarritoId();

        var response = await _http.PutAsJsonAsync($"carritos/{_carritoId}/confirmar", cliente);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            await CrearCarritoDeNuevo();
            throw new InvalidOperationException("El carrito se perdió. Por favor agregá productos nuevamente.");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            throw new InvalidOperationException("No se pudo confirmar la compra. Verificá que el carrito no esté vacío.");
        }

        response.EnsureSuccessStatusCode();

        await _js.InvokeVoidAsync("localStorage.removeItem", "carritoId");
        _carritoId = null;
    }

    private async Task AsegurarCarritoId()
    {
        if (_carritoId != null)
            return;

        var carritoIdStr = await _js.InvokeAsync<string>("localStorage.getItem", "carritoId");

        if (!string.IsNullOrWhiteSpace(carritoIdStr) && Guid.TryParse(carritoIdStr, out var id))
        {
            _carritoId = id;
            return;
        }

        await CrearCarritoDeNuevo();
    }

    private async Task CrearCarritoDeNuevo()
    {
        var response = await _http.PostAsJsonAsync("carritos", new { });
        _carritoId = await response.Content.ReadFromJsonAsync<Guid>();
        await _js.InvokeVoidAsync("localStorage.setItem", "carritoId", _carritoId.ToString());
    }
}
