namespace cliente.Models;

public class ItemCarritoResponse
{
  public int ProductoId { get; set; }
  public string Nombre { get; set; } = string.Empty;
  public decimal Precio { get; set; }
  public int Cantidad { get; set; }
    public int StockDisponible { get; set; }
  public decimal Subtotal => Precio * Cantidad;
}

public class ProductoResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
}