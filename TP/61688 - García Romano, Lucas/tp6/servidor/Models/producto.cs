namespace servidor.Models;

public class Producto
//Son los detalles de los diferentes productos//
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal Precio { get; set; }

    //La cantidad de Productos
    public int Stock { get; set; }

    //Evita que un producto no sea nulo si no tiene imagen
    public string ImagenUrl { get; set; } = "";

    public string Marca { get; set; } = "";

}