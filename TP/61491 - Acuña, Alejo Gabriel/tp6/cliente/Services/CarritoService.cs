using cliente.Models;
using System.Net.Http.Json;

namespace cliente.Services;

public class CarritoService
{
    private readonly HttpClient http;
    private Guid carritoId = Guid.Empty;

    public event Action? OnChange;

    public int TotalItems { get; private set; } = 0;

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
        await ActualizarCantidad();
    }

    public async Task IncrementarCantidad(int productoId)
    {
        await AsegurarCarritoInicializado();
        await http.PutAsync($"carritos/{carritoId}/{productoId}", null);
        await ActualizarCantidad();
    }

    public async Task QuitarProducto(Producto producto)
    {
        if (carritoId == Guid.Empty) return;

        await http.DeleteAsync($"carritos/{carritoId}/{producto.Id}");
        await ActualizarCantidad();
    }

    public async Task QuitarProductoTotal(int productoId)
    {
        if (carritoId == Guid.Empty) return;

        await http.DeleteAsync($"carritos/{carritoId}/{productoId}/todo");
        await ActualizarCantidad();
    }

    public async Task<List<ItemCarritoDto>> ObtenerCarrito()
    {
        if (carritoId == Guid.Empty)
            return new List<ItemCarritoDto>();

        return await http.GetFromJsonAsync<List<ItemCarritoDto>>($"carritos/{carritoId}") ?? new();
    }

    public async Task ConfirmarCompra(DatosClienteDto datos)
    {
        if (carritoId == Guid.Empty) return;

        await http.PutAsJsonAsync($"carritos/{carritoId}/confirmar", datos);
        carritoId = Guid.Empty;
        await ActualizarCantidad();
    }

    public async Task VaciarCarrito()
    {
        if (carritoId == Guid.Empty) return;

        await http.DeleteAsync($"carritos/{carritoId}");
        await ActualizarCantidad();
    }

    public async Task ActualizarCantidad()
    {
        if (carritoId == Guid.Empty)
        {
            TotalItems = 0;
        }
        else
        {
            var carrito = await ObtenerCarrito();
            TotalItems = carrito.Sum(i => i.Cantidad);
        }
        OnChange?.Invoke();
    }
}
