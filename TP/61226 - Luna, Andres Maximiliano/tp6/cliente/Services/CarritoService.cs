using cliente.Models;

namespace cliente.Services;

public class CarritoService
{
    private List<ItemCarrito> _items = new();

    public List<ItemCarrito> ObtenerCarrito() => _items;

    public void AgregarAlCarrito(Producto producto)
    {
        var item = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);

        if (item != null)
        {
            if (item.Cantidad < producto.Stock)
                item.Cantidad++;
        }
        else
        {
            _items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
        }
    }

    public void AumentarCantidad(int productoId)
    {
        var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item != null && item.Cantidad < item.Producto.Stock)
            item.Cantidad++;
    }

    public void DisminuirCantidad(int productoId)
    {
        var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item != null)
        {
            item.Cantidad--;
            if (item.Cantidad <= 0)
                _items.Remove(item);
        }
    }

    public void EliminarDelCarrito(int productoId)
    {
        var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item != null)
            _items.Remove(item);
    }

    public void VaciarCarrito() => _items.Clear();

    public decimal CalcularTotal() =>
        _items.Sum(i => i.Cantidad * i.Producto.Precio);

    public int ObtenerCantidadTotalEnCarrito()
    {
        return _items.Sum(p => p.Cantidad);
    } 
}
