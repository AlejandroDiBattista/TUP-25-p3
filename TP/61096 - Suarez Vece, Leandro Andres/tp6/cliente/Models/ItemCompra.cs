namespace cliente.Models;

public class ItemCompraDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class ItemCompraGtDto : ICardItem
{
    public int? Id_iten { get; set; }
    public int Cantidad { get; set; }
    public int ProductoId { get; set; }
    public int CompraId { get; set; }
    public int Stock { get; set; }
    public string NombreProducto { get; set; }
    public decimal PrecioProducto { get; set; }

    public decimal subTotal => Cantidad * PrecioProducto;
    public string Nombre => NombreProducto;
    public decimal Precio => PrecioProducto;
    public string? ImagenUrl => null; // No tiene imagen
    public string? Descripcion => null;
    public int Id => ProductoId;

}