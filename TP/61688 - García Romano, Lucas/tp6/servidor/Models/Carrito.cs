namespace servidor.Models; 

public class ItemCarrito //representa un item en el carrito de compras
{
    public int ProductoId { get; set; } //identifica el producto
    public string Nombre { get; set; } = string.Empty;//nombre del producto
    public int Cantidad { get; set; }//cantidad del producto
    public decimal PrecioUnitario { get; set; }//precio del producto
}

public class Carrito
{
    public Guid Id { get; set; } = Guid.NewGuid(); //fecha de creacion
   
    public List<ItemCarrito> Items { get; set; } = new();//lista de items en el carritp
}