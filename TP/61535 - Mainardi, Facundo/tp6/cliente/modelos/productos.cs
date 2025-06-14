namespace Cliente.Modelos;
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; }

    public ICollection<ItemCompra> ItemsCompra { get; set; }
}

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }

    public ICollection<ItemCompra> ItemsCompra { get; set; }
}

public class ItemCompra
{
    public int Id { get; set; }

    public int ProductoId { get; set; }
    public Producto Producto { get; set; }

    public int CompraId { get; set; }
    public Compra Compra { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}