namespace cliente.Services;



public class CarritoService
{
    public List<ItemCarrito> Items { get; set; } = new();

    public void AgregarProducto(Producto producto)
    {
        var existente = Items.FirstOrDefault(p => p.Producto.Id == producto.Id);
        if (existente != null)
        {
            existente.Cantidad++;
        }
        else
        {
            Items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
        }
    }

    public void EliminarProducto(int productoId)
    {
        var item = Items.FirstOrDefault(p => p.Producto.Id == productoId);
        if (item != null)
        {
            Items.Remove(item);
        }
    }

    public void VaciarCarrito()
    {
        Items.Clear();
    }

    public decimal CalcularTotal()
    {
        return Items.Sum(i => i.Cantidad * i.Producto.Precio);
    }
}

public class ItemCarrito
{
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }
}




