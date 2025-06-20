public class Productos
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public int Precio { get; set; }
    public int Stock { get; set; }
    public string? ImagenUrl { get; set; }
    
}