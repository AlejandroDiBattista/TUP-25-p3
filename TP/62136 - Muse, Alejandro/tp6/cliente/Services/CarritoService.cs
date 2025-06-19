using cliente.Models;

namespace cliente.Services;

public class CarritoItem
{
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }
}

public class CarritoService
{
    public List<CarritoItem> Items { get; } = new();

    public event Action? OnChange;

    public void Agregar(Producto producto)
    {
        var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
        if (item != null)
            item.Cantidad++;
        else
            Items.Add(new CarritoItem { Producto = producto, Cantidad = 1 });

        OnChange?.Invoke();
    }

    public void Quitar(Producto producto)
    {
        var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
        if (item != null)
        {
            item.Cantidad--;
            if (item.Cantidad <= 0)
                Items.Remove(item);

            OnChange?.Invoke();
        }
    }

    public void Eliminar(Producto producto)
    {
        var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
        if (item != null)
        {
            Items.Remove(item);
            OnChange?.Invoke();
        }
    }

    public void Vaciar()
    {
        Items.Clear();
        OnChange?.Invoke();
    }

    public int Cantidad => Items.Sum(i => i.Cantidad);

    public decimal Total => Items.Sum(i => i.Producto.Precio * i.Cantidad);
}