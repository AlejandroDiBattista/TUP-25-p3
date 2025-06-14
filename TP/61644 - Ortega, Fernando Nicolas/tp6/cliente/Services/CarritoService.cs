using System.Collections.ObjectModel;

namespace cliente.Services;

public class CarritoService
{
    public ObservableCollection<ArticuloCarrito> Articulos { get; } = new();

    public event Action? OnChange;

    public void Agregar(Producto producto, int cantidad = 1)
    {
        var existente = Articulos.FirstOrDefault(a => a.ProductoId == producto.Id);
        if (existente != null)
        {
            existente.Cantidad += cantidad;
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
        OnChange?.Invoke();
    }

    public void Quitar(int productoId)
    {
        var articulo = Articulos.FirstOrDefault(a => a.ProductoId == productoId);
        if (articulo != null)
        {
            Articulos.Remove(articulo);
            OnChange?.Invoke();
        }
    }

    public void CambiarCantidad(int productoId, int nuevaCantidad)
    {
        var articulo = Articulos.FirstOrDefault(a => a.ProductoId == productoId);
        if (articulo != null && nuevaCantidad > 0)
        {
            articulo.Cantidad = nuevaCantidad;
            OnChange?.Invoke();
        }
    }

    public void Vaciar()
    {
        Articulos.Clear();
        OnChange?.Invoke();
    }

    public int TotalArticulos() => Articulos.Sum(a => a.Cantidad);
    public decimal TotalImporte() => Articulos.Sum(a => a.Cantidad * a.PrecioUnitario);
}