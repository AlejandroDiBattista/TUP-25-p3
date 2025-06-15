namespace cliente.Services;

public class ArticuloCarrito
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Cantidad { get; set; }
    public string ImagenUrl { get; set; }
}