namespace cliente.Models;

using cliente.Models;

public class Carrito
{
    public int Id { get; set; }
    public List<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();
}

public class ItemCarrito
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; }
    public int CarritoId { get; set; }
    public Carrito Carrito { get; set; }
}
