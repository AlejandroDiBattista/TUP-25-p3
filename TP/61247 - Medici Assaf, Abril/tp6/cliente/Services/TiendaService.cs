using System.Net.Http.Json;
using Cliente.Models;

public class TiendaService
{
    private readonly HttpClient _http;
    private string _carritoId = Guid.NewGuid().ToString();
    public List<productosCarritos> Carrito = new();

    public TiendaService(HttpClient http)
    {
        _http = http;
    }

    public event Action OnCarritoChanged;
    private void NotificarCambio() => OnCarritoChanged?.Invoke();

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        var productos = await _http.GetFromJsonAsync<List<Producto>>("productos");
        return productos ?? new List<Producto>();
    }

    public async Task CargarCarrito()
    {
        Carrito = await _http.GetFromJsonAsync<List<productosCarritos>>($"carritos/{_carritoId}") ?? new();
    }

    public async Task<bool> AgregarAlCarrito(int productoId)
    {
        var response = await _http.PostAsync($"carritos/{_carritoId}/agregar/{productoId}", null);
        if (response.IsSuccessStatusCode)
        {
            await CargarCarrito();
            NotificarCambio();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task QuitarDelCarrito(int productoId)
    {
        await _http.DeleteAsync($"carritos/{_carritoId}/quitar/{productoId}");
        await CargarCarrito();
        NotificarCambio();
    }

    public async Task VaciarCarrito()
    {
        await _http.DeleteAsync($"carritos/{_carritoId}");
        Carrito.Clear();
        NotificarCambio();
    }

    public async Task ConfirmarCompra(DatosCliente cliente)
    {
        await _http.PutAsJsonAsync($"carritos/{_carritoId}/confirmar", cliente);
        Carrito.Clear();
        _carritoId = Guid.NewGuid().ToString();
    }

    public decimal CalcularTotal() =>
        Carrito.Sum(item => item.producto.Valor * item.Cantidad);

    public int CantidadTotal() =>
        Carrito.Sum(item => item.Cantidad);
}
