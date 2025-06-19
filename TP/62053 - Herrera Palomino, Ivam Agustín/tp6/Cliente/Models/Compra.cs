namespace TiendaOnline.Frontend.Models
{
    /// <summary>
    /// Representa una compra realizada por un cliente.
    /// </summary>
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string ApellidoCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;

        public List<ItemCompra> Items { get; set; } = new();
    }

}
