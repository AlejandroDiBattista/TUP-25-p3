namespace servidor.Modelos;

public class Producto
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public required string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public required string ImagenUrl { get; set; }
}