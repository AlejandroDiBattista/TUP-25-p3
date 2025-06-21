namespace shared.Models;

public class ItemCarrito
{
    public string Id { get; set; } = string.Empty;
    public string CarritoId { get; set; } = string.Empty;
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
    public int Stock { get; set; }
    public decimal Importe => Cantidad * Precio;
}