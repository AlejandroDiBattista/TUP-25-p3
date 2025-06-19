using System.Collections.ObjectModel;
using cliente.Services;

namespace cliente.Services;

public class CarritoService
{
    public int CarritoId { get; set; } = 0;
    public List<ArticuloCarrito> Articulos { get; } = new();
    public string Mensaje { get; set; } = "";

    public event Action? OnChange;

    private readonly ApiService _apiService;

    public CarritoService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task AgregarAsync(Producto producto, int cantidad = 1)
    {
        if (CarritoId == 0)
        {
            CarritoId = await _apiService.CrearCarritoAsync();
        }

        var existente = Articulos.FirstOrDefault(a => a.ProductoId == producto.Id);
        int nuevaCantidad = cantidad;
        if (existente != null)
        {
            nuevaCantidad = existente.Cantidad + cantidad;
        }

        var ok = await _apiService.AgregarOActualizarProductoEnCarritoAsync(CarritoId, producto.Id, nuevaCantidad);

        if (!ok)
        {
            CarritoId = await _apiService.CrearCarritoAsync();
            Articulos.Clear();
            Mensaje = "El carrito se perdió. Se creó uno nuevo. Agregue los productos nuevamente.";
            OnChange?.Invoke();
            return;
        }

        if (existente != null)
        {
            existente.Cantidad = nuevaCantidad;
        }
        else
        {
            Articulos.Add(new ArticuloCarrito
            {
                ProductoId = producto.Id,
                Nombre = producto.Nombre,
                PrecioUnitario = producto.Precio,
                Cantidad = cantidad,
                ImagenUrl = producto.ImagenUrl
            });
        }
        Mensaje = "";
        OnChange?.Invoke();
    }

    public async Task CambiarCantidadAsync(int productoId, int nuevaCantidad)
    {
        if (CarritoId != 0)
        {
            var ok = await _apiService.AgregarOActualizarProductoEnCarritoAsync(CarritoId, productoId, nuevaCantidad);
            if (!ok)
            {
                CarritoId = 0;
                Articulos.Clear();
                Mensaje = "El carrito se perdió o hubo un error. Agregue los productos nuevamente.";
                OnChange?.Invoke();
                return;
            }
        }
        var index = Articulos.FindIndex(a => a.ProductoId == productoId);
        if (index != -1 && nuevaCantidad > 0)
        {
            var articulo = Articulos[index];
            Articulos[index] = new ArticuloCarrito
            {
                ProductoId = articulo.ProductoId,
                Nombre = articulo.Nombre,
                PrecioUnitario = articulo.PrecioUnitario,
                Cantidad = nuevaCantidad,
                ImagenUrl = articulo.ImagenUrl
            };
        }
        Mensaje = "";
        OnChange?.Invoke();
    }

    public async Task QuitarAsync(int productoId)
    {
        if (CarritoId != 0)
        {
            var ok = await _apiService.QuitarProductoDelCarritoAsync(CarritoId, productoId);
            if (!ok)
            {
                CarritoId = 0;
                Articulos.Clear();
                Mensaje = "El carrito se perdió o hubo un error. Agregue los productos nuevamente.";
                OnChange?.Invoke();
                return;
            }
        }
        var articulo = Articulos.FirstOrDefault(a => a.ProductoId == productoId);
        if (articulo != null)
        {
            Articulos.Remove(articulo);
        }
        Mensaje = "";
        OnChange?.Invoke();
    }

    public void Vaciar()
    {
        Articulos.Clear();
        CarritoId = 0;
        Mensaje = "";
        OnChange?.Invoke();
    }

    public async Task VaciarAsync()
    {
        if (CarritoId != 0)
        {
            var ok = await _apiService.VaciarCarritoAsync(CarritoId);
            if (!ok)
            {
                CarritoId = 0;
                Articulos.Clear();
                Mensaje = "El carrito se perdió o hubo un error. Agregue los productos nuevamente.";
                OnChange?.Invoke();
                return;
            }
        }
        Articulos.Clear();
        CarritoId = 0;
        Mensaje = "";
        OnChange?.Invoke();
    }

    public int TotalArticulos() => Articulos.Sum(a => a.Cantidad);
    public decimal TotalImporte() => Articulos.Sum(a => a.Cantidad * a.PrecioUnitario);
}