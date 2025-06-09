namespace servidor.Models;

public class ItemCarrito
{
    public int ProductoId { get; set; } 
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class Carrito
{
    public Guid Id { get; set; } = Guid.NewGuid();
    //Fecha de creacion
    public List<ItemCarrito> Items { get; set; } = new();
    //
}