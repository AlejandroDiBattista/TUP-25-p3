namespace servidor.Models
{
    public class Compra 
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string ApellidoCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public List<ItemCompra> Item { get; set; } = new ();
    }
}