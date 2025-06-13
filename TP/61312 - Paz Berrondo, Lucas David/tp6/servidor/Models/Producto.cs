namespace servidor.Models;

// Modelo que representa un producto en la tienda gaming
public class Producto
{
    public int Id { get; set; }  // Clave primaria
    public string Nombre { get; set; } = string.Empty;  // Nombre del producto
    public string Descripcion { get; set; } = string.Empty;  // Descripción detallada
    public decimal Precio { get; set; }  // Precio en ARS (decimal para precisión monetaria)
    public int Stock { get; set; }  // Cantidad disponible en inventario
    public string ImagenUrl { get; set; } = string.Empty;  // URL de la imagen del producto
}
