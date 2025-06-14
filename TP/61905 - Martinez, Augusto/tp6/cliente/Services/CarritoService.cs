using Cliente.Models;
using System.Collections.Generic;
using System.Linq;

public class CarritoService
{
    private List<ItemCarrito> items = new();

    public IReadOnlyList<ItemCarrito> Items => items;

    public void AgregarProducto(Producto producto)
    {
        if (producto.Stock <= 0)
            return; // No hay stock disponible

        var item = items.FirstOrDefault(i => i.Producto.Id == producto.Id);
        if (item != null)
        {
            if (producto.Stock > 0)
            {
                item.Cantidad++;
                producto.Stock--;
            }
        }
        else
        {
            items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
            producto.Stock--;
        }
    }

    public void EliminarProducto(Producto producto)
    {
        var item = items.FirstOrDefault(i => i.Producto.Id == producto.Id);
        if (item != null)
        {
            producto.Stock += item.Cantidad; // Devolvemos el stock
            items.Remove(item);
        }
    }

    public decimal CalcularTotal()
    {
        return items.Sum(i => i.Producto.Precio * i.Cantidad);
    }

    public int ContadorProductos()
    {
        return items.Sum(i => i.Cantidad);
    }
}

public class ItemCarrito
{
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }
}
