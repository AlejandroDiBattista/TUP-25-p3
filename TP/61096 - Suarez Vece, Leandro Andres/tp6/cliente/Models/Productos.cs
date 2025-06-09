namespace cliente.Models;

public class Producto : ICardItem
{
    public int Id_producto { get; set; }
    public string ImagenUrl { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public int Stock { get; set; }
    public decimal Precio { get; set; }
    public int Id => Id_producto;

}

