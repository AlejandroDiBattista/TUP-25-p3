using System.Collections.Generic;
using System.Linq;
using Cliente.Models;


public class CarritoService
{
    private List<ItemCarrito> items = new();

    public IReadOnlyList<ItemCarrito> Items => items;

    public void AgregarProducto(Producto producto)
    {
        var item = items.FirstOrDefault(i => i.Producto.Id == producto.Id);
        if (item != null)
        {
            item.Cantidad++;
        }
        else
        {
            items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
        }
    }

    public void EliminarProducto(Producto producto)
    {
        var item = items.FirstOrDefault(i => i.Producto.Id == producto.Id);
        if (item != null)
        {
            items.Remove(item);
        }
    }

    public decimal CalcularTotal()
    {
        return items.Sum(i => i.Producto.Precio * i.Cantidad);
    }
}

public class ItemCarrito
{
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }
}
