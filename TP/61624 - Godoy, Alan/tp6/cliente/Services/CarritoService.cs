using cliente.Models;

namespace cliente.Services;

public class CarritoService
{
    public List<ItemCompra> Items { get; private set; } = new();

    public event Action? OnChange;

    public void AgregarAlCarrito(Producto producto)
    {
        if (producto == null) return;

        var item = Items.FirstOrDefault(i => i.Producto?.Id == producto.Id);
        if (item != null)
        {
            item.Cantidad++;
        }
        else
        {
            Items.Add(new ItemCompra { Producto = producto, Cantidad = 1 });
        }
        OnChange?.Invoke();
    }

    public void IncrementarCantidad(ItemCompra item)
    {
        if (item == null) return;
        item.Cantidad++;
        OnChange?.Invoke();
    }

    public void DecrementarCantidad(ItemCompra item)
    {
        if (item == null) return;

        if (item.Cantidad > 1)
        {
            item.Cantidad--;
        }
        else
        {
            EliminarItem(item);
        }
        OnChange?.Invoke();
    }

    public void EliminarItem(ItemCompra item)
    {
        if (item == null) return;
        Items.Remove(item);
        OnChange?.Invoke();
    }

    public void LimpiarCarrito()
    {
        Items.Clear();
        OnChange?.Invoke();
    }

public int ObtenerCantidadProducto(int idProducto)
{
    return Items.FirstOrDefault(i => i.Producto.Id == idProducto)?.Cantidad ?? 0;
}

    public decimal ObtenerTotal()
    {
        return Items.Sum(i => (i.Producto?.Precio ?? 0) * i.Cantidad);
    }

    public void AgregarProducto(Producto producto)
    {
        AgregarAlCarrito(producto);
    }

    public void EliminarProducto(int idProducto)
    {
        var item = Items.FirstOrDefault(i => i.Producto?.Id == idProducto);
        if (item != null)
        {
            EliminarItem(item);
        }
    }
public bool PuedeAgregarMas(int idProducto, int stockTotal)
{
    return ObtenerCantidadProducto(idProducto) < stockTotal;
}


    public void CambiarCantidad(int idProducto, int cantidad)
    {
        var item = Items.FirstOrDefault(i => i.Producto?.Id == idProducto);
        if (item != null && cantidad > 0)
        {
            item.Cantidad = cantidad;
            OnChange?.Invoke();
        }
        else if (item != null && cantidad <= 0)
        {
            EliminarItem(item);
        }
    }

    public void VaciarCarrito()
    {
        LimpiarCarrito();
    }

    public int ObtenerCantidadTotal()
    {
        return Items.Sum(i => i.Cantidad);
    }

    // Para mostrar luego de vaciar el carrito
public List<ItemCompra> ItemsFactura { get; private set; } = new();

public void GuardarFactura()
{
    ItemsFactura = Items.Select(i => new ItemCompra
    {
        Producto = i.Producto,
        Cantidad = i.Cantidad
    }).ToList();
}

public decimal ObtenerTotalFactura()
{
    return ItemsFactura.Sum(i => i.Producto.Precio * i.Cantidad);
}

}
