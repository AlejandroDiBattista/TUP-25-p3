namespace servidor.Models;

public class Producto
//Son los detalles de los diferentes productos//
{
    public int Id { get; set; }//identificador unico del producto
    public string Nombre { get; set; } = "";//nombre del producto
    public string Descripcion { get; set; } = "";//descripcion del producto
    public decimal Precio { get; set; }//precio del producto

    
    public int Stock { get; set; }//la cantidad de Productos

    
    public string ImagenUrl { get; set; } = "";//evita que un producto no sea nulo si no tiene imagen

    public string Marca { get; set; } = "";//marca del producto

}