namespace cliente.Modelos;

// Esta es la clase que faltaba para representar una compra finalizada
public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public required string NombreCliente { get; set; }
    public required string ApellidoCliente { get; set; }
    public required string EmailCliente { get; set; }
    public List<ItemCompra> Items { get; set; } = new();
}

// Esta clase define cada línea de producto dentro de una Compra.
// También la necesitamos en el cliente para que entienda la estructura completa.
public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}