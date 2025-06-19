// Modelo de producto en la base de datos
public class Producto
{
    public int Id { get; set; } // Identificador único
    public string Nombre { get; set; } = null!; // Nombre del producto
    public string Descripcion { get; set; } = null!; // Descripción
    public decimal Precio { get; set; } // Precio unitario
    public int Stock { get; set; } // Stock disponible
    public string ImagenUrl { get; set; } = null!; // URL de la imagen
}
