using System.Collections.Generic;
using Shared;

public class CarritoService
{
    private List<Producto> _items = new();

    public IReadOnlyList<Producto> ObtenerCarrito() => _items;

    public void AgregarAlCarrito(Producto producto)
    {
        _items.Add(producto);
    }

    public void EliminarDelCarrito(Producto producto)
    {
        _items.Remove(producto);
    }

    public void VaciarCarrito()
    {
        _items.Clear();
    }
}