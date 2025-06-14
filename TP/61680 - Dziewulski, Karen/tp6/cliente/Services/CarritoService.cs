using System.Net.Http.Json;
using cliente.Modelos;
#nullable enable

namespace cliente.Services;

public class CarritoService
{
    private readonly HttpClient _http;
    public Guid CarritoId { get; private set; }

    public List<ItemCarrito> Items { get; private set; } = new();
    public event Action? OnChange;

    public decimal Total => Items.Sum(i => i.Importe);
    public int TotalItems => Items.Sum(i => i.Cantidad);

    public CarritoService(HttpClient http)
    {
        _http = http;
    }

    public async Task InicializarAsync()
    {
        if (CarritoId == Guid.Empty)
        {
            // Solicitar nuevo carrito al backend
            CarritoId = await _http.PostAsync("/carritos", null)
                                   .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<Guid>())
                                   .Unwrap();
        }

        await CargarCarrito();
    }

    public async Task CargarCarrito()
    {
        var response = await _http.GetFromJsonAsync<List<ItemBackendDto>>($"/carritos/{CarritoId}");
        if (response != null)
        {
            Items = response.Select(r => new ItemCarrito
            {
                Producto = new Producto
                {
                    Id = r.ProductoId,
                    Nombre = r.Nombre ?? "",
                    Precio = r.PrecioUnitario
                },
                Cantidad = r.Cantidad
            }).ToList();

            OnChange?.Invoke();
        }
    }

    public async Task AgregarProducto(Producto producto)
    {
        var response = await _http.PutAsync($"/carritos/{CarritoId}/{producto.Id}", null);
        if (response.IsSuccessStatusCode)
        {
            await CargarCarrito();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al agregar: {error}");
        }
    }

    public async Task QuitarProducto(int productoId)
    {
        var response = await _http.DeleteAsync($"/carritos/{CarritoId}/{productoId}");
        if (response.IsSuccessStatusCode)
        {
            await CargarCarrito();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al quitar: {error}");
        }
    }

    public async Task EliminarProducto(int productoId)
    {
        var item = Items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item != null)
        {
            for (int i = 0; i < item.Cantidad; i++)
            {
                await _http.DeleteAsync($"/carritos/{CarritoId}/{productoId}");
            }
            await CargarCarrito();
        }
    }

    public async Task Vaciar()
    {
        var response = await _http.DeleteAsync($"/carritos/{CarritoId}");
        if (response.IsSuccessStatusCode)
        {
            Items.Clear();
            OnChange?.Invoke();
        }
    }

    public async Task ConfirmarCompra(string nombre, string apellido, string email)
    {
        var cliente = new { Nombre = nombre, Apellido = apellido, Email = email };
        var response = await _http.PutAsJsonAsync($"/carritos/{CarritoId}/confirmar", cliente);
        if (response.IsSuccessStatusCode)
        {
            Items.Clear();
            CarritoId = Guid.Empty;

            // Pedimos un nuevo carrito (igual que en InicializarAsync)
            CarritoId = await _http.PostAsync("/carritos", null)
                                .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<Guid>())
                                .Unwrap();

            await CargarCarrito(); // Refrescamos
            OnChange?.Invoke();

        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al confirmar: {error}");
        }
    }

    private class ItemBackendDto
    {
        public int ProductoId { get; set; }
        public string? Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
    }
}

