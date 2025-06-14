using System.Text.Json.Serialization; // Necesario para evitar ciclos en JSON


public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int CompraId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    public Producto Producto { get; set; }

    [JsonIgnore] // Evita que se serialice la Compra y cree un ciclo infinito
    public Compra Compra { get; set; }
}