using cliente.Models;

namespace cliente.Services;

public class CarritoService
{
    public event Action? OnChange;

    private readonly List<(Producto producto, int cantidad)> _items = new();

    public IReadOnlyList<(Producto producto, int cantidad)> Items => _items;

    public void AgregarProducto(Producto producto)
    {
        var item = _items.FirstOrDefault(i => i.producto.Id == producto.Id);

        if (item.producto != null)
        {
            var index = _items.IndexOf(item);
            _items[index] = (item.producto, item.cantidad + 1);
        }
        else
        {
            _items.Add((producto, 1));
        }

        OnChange?.Invoke();
    }

    public void QuitarProducto(Producto producto)
    {
        var item = _items.FirstOrDefault(i => i.producto.Id == producto.Id);
        if (item.producto != null)
        {
            var index = _items.IndexOf(item);
            if (item.cantidad > 1)
                _items[index] = (item.producto, item.cantidad - 1);
            else
                _items.Remove(item);
        }

        OnChange?.Invoke();
    }

    public void Vaciar()
    {
        _items.Clear();
        OnChange?.Invoke();
    }

    public int TotalItems() => _items.Sum(i => i.cantidad);
}
