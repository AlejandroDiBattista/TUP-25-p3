namespace cliente.Models;

public class ItemCarrito
{
    public int Id {get; set;}
    public Producto Producto { get; set; }
    public string Nombre {get; set;} = string.Empty;
    public decimal Precio {get; set;}
    public int Cantidad { get; set; }
}
