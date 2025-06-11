namespace cliente.Models;

using cliente.Models;

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public List<ItemCompra> Items { get; set; } = new List<ItemCompra>();
}

public class ItemCompra
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;
}
