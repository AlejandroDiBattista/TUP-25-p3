using cliente.Models;

namespace cliente.Services;

public class CartService
{
    public List<ItemCarrito> Items { get; private set; } = new();

    public event Action? OnChange;

   public void NotificarCambio() => OnChange?.Invoke();

    public void AgregarProducto(Producto producto)
    {
        var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
        if (item is not null)
        {
            item.Cantidad++;
        }
        else
        {
            Items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
        }

        NotificarCambio();
    }

    public void EliminarProducto(int productoId)
    {
        var item = Items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item is not null)
        {
            Items.Remove(item);
            NotificarCambio(); 
        }
    }

    public void CambiarCantidad(int productoId, int nuevaCantidad)
    {
        var item = Items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item is not null)
        {
            if (nuevaCantidad <= 0)
                Items.Remove(item);
            else
                item.Cantidad = nuevaCantidad;

            NotificarCambio(); 
        }
    }

    public void VaciarCarrito()
    {
        Items.Clear();
        NotificarCambio();
    }

    public int TotalItems() => Items.Sum(i => i.Cantidad);
}