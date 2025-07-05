namespace servidor.ModelosServidor
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string EmailCliente { get; set; }

        // Relación con items de compra
        public List<ItemCompra> Items { get; set; } = new();
    }
}