namespace cliente.Modelos;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; }

    // Propiedades para el carrito
    public int Cantidad { get; set; } = 1;
    public decimal PrecioUnitario => Precio;
}