namespace servidor.Models;

public class Carrito
{
  public string Id {get;set;}
  public List<ProductoCarrito> Productos {get;set;} = new List<ProductoCarrito>();

  public class ProductoCarrito
  {
    public int Id {get;set;}
    public int Cantidad {get;set;}
  }
}