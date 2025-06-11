using cliente.Models;
using System.Net.Http.Json;

namespace cliente.Services;

public class CarritoService
{
    private readonly HttpClient http;
    private Guid carritoId = Guid.Empty;

    public event Action? OnChange;

    public CarritoService(HttpClient http)
    {
        this.http = http;
    }

    private async Task AsegurarCarritoInicializado()
    {
        if (carritoId == Guid.Empty)
        {
            var response = await http.PostAsync("carritos", null);
            carritoId = await response.Content.ReadFromJsonAsync<Guid>();
        }
    }

        public async Task AgregarProducto(Producto producto)
    {
        if (producto.Stock <= 0)
    {
        Console.WriteLine("No se puede agregar producto sin stock.");
        return;
    }

        await AsegurarCarritoInicializado();
        await http.PutAsync($"carritos/{carritoId}/{producto.Id}", null);
        OnChange?.Invoke();
    }
public async Task IncrementarCantidad(int productoId)
{
    await AsegurarCarritoInicializado();
    await http.PutAsync($"carritos/{carritoId}/{productoId}", null);
    OnChange?.Invoke();
}


    public async Task QuitarProducto(Producto producto)
    {
        if (carritoId == Guid.Empty) return;

        await http.DeleteAsync($"carritos/{carritoId}/{producto.Id}");
        OnChange?.Invoke();
    }

    public async Task<List<ItemCarritoDto>> ObtenerCarrito()
    {
        if (carritoId == Guid.Empty)
            return new List<ItemCarritoDto>();

        return await http.GetFromJsonAsync<List<ItemCarritoDto>>($"carritos/{carritoId}") ?? new();
    }

    public async Task ConfirmarCompra()
    {
        if (carritoId == Guid.Empty) return;

        await http.PutAsync($"carritos/{carritoId}/confirmar", null);
        carritoId = Guid.Empty;
        OnChange?.Invoke();
    }

    public async Task ConfirmarCompra(DatosClienteDto datos)
    {
        if (carritoId == Guid.Empty) return;

        await http.PutAsJsonAsync($"carritos/{carritoId}/confirmar", datos);
        carritoId = Guid.Empty;
        OnChange?.Invoke();
    }

    public async Task VaciarCarrito()
    {
        if (carritoId == Guid.Empty) return;

        await http.DeleteAsync($"carritos/{carritoId}");
        OnChange?.Invoke();
    }
}
