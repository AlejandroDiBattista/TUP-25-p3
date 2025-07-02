using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services;

public class CarritoService
{
    private readonly HttpClient _http;

    public int CarritoId { get; private set; }
    public List<ItemCarrito> Items { get; private set; } = new();

    public CarritoService(HttpClient http)
    {
        _http = http;
    }

    /*────────────  API pública ────────────*/

    public async Task Agregar(int productoId, int cantidad = 1)
    {
        await EnsureCarritoAsync();
        var resp = await _http.PutAsync(
            $"carritos/{CarritoId}/{productoId}?cantidad={cantidad}", null);

        if (resp.IsSuccessStatusCode)
        {
            await RefrescarAsync();
            NotificarCambio();
        }
    }

    public async Task Quitar(int productoId)
    {
        await _http.DeleteAsync($"carritos/{CarritoId}/{productoId}");
        await RefrescarAsync();
    }

    public async Task Vaciar()
    {
        if (CarritoId == 0) return;

        var resp = await _http.DeleteAsync($"carritos/{CarritoId}");
        if (resp.IsSuccessStatusCode)
        {
            Items.Clear();
            CarritoId = 0;
            NotificarCambio();
        }
    }

    public async Task ActualizarItems() => await RefrescarAsync();

    public decimal TotalImporte() => Items.Sum(i => i.Precio * i.Cantidad);
    public int     TotalItems()
    {
        return Items.Sum(i => i.Cantidad);
    }

    public async Task ActualizarCantidad(int productoId, int cantidad)
    {
        if (CarritoId == 0) return;
        var resp = await _http.PutAsync($"carritos/{CarritoId}/{productoId}?cantidad={cantidad}", null);
        if (resp.IsSuccessStatusCode)
        {
            await RefrescarAsync();
            OnChange?.Invoke();
        }
    }

    /*────────────  privados ────────────*/

    async Task EnsureCarritoAsync()
    {
        if (CarritoId != 0) return;

        var resp = await _http.PostAsync("carritos", null);
        resp.EnsureSuccessStatusCode();

        var dto = await resp.Content.ReadFromJsonAsync<IdDto>();
        CarritoId = dto.id;
    }

    async Task RefrescarAsync()
    {
        if (CarritoId == 0) return;

        var carrito = await _http.GetFromJsonAsync<Carrito>($"carritos/{CarritoId}");
        if (carrito != null)
            Items = carrito.Items;
    }

    public event Action? OnChange;
    public void NotificarCambio() => OnChange?.Invoke();

    private record IdDto(int id);   // <──  minúscula!
}
