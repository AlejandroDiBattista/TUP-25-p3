namespace Tienda.Modelos;

public class ItemCarrito
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string ImagenUrl { get; set; } = "";
}

public class Carrito
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<ItemCarrito> Items { get; set; } = new();
}