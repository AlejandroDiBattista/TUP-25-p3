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
        if (ObtenerStock(producto.Id) > 0)
        {
            item.Cantidad++;
            RestarStock(producto.Id, 1);
        }
    }
    else
    {
        if (ObtenerStock(producto.Id) > 0)
        {
            _items.Add(new ItemCarrito
            {
                Producto = producto,
                Cantidad = 1
            });
            RestarStock(producto.Id, 1);
        }
    }   
    }

    public void AumentarCantidadCarrito(int productoId)
    {
        var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item != null && item.Producto.StockDisponible > 0)
        {
            item.Cantidad++;
            item.Producto.StockDisponible--;
            RestarStock(productoId, 1);
        }
    }

    public void DisminuirCantidadCarrito(int productoId)
    {
        var item = _items.FirstOrDefault(p => p.Producto.Id == productoId);
        if (item != null && item.Cantidad > 0)
        {
            item.Cantidad--;
            SumarStock(productoId, 1);
            if (item.Cantidad == 0)
            {
                _items.Remove(item);
            }
        }
    }

    public void EliminarDelCarrito(int productoId)
    {
        var item = _items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item != null)
            _items.Remove(item);
    }

    public void VaciarCarrito()
    {
        foreach (var item in _items)
        {
            SumarStock(item.Producto.Id, item.Cantidad);
        }

        _items.Clear();
    }

    public decimal CalcularTotal() =>
        _items.Sum(i => i.Cantidad * i.Producto.Precio);

    public int ObtenerCantidadTotalEnCarrito()
    {
        return _items.Sum(p => p.Cantidad);
    }

    private Dictionary<int, int> stockActual = new();

    public void EstablecerStock(int productoId, int cantidad)
    {
        stockActual[productoId] = cantidad;
    }

    public int ObtenerStock(int productoId)
    {
        return stockActual.ContainsKey(productoId) ? stockActual[productoId] : 0;
    }

    public void RestarStock(int productoId, int cantidad)
    {
        if (stockActual.ContainsKey(productoId))
            stockActual[productoId] = Math.Max(0, stockActual[productoId] - cantidad);
    }

    public void SumarStock(int productoId, int cantidad)
    {
        if (stockActual.ContainsKey(productoId))
            stockActual[productoId] += cantidad;
    }

    public bool StockInicializado(int productoId)
    {
        return stockActual.ContainsKey(productoId);
    }

}
