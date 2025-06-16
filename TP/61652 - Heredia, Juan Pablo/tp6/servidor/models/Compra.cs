
#nullable enable
namespace servidor.models
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string? NombreCliente { get; set; }
        public string? ApellidoCliente { get; set; }
        public string? EmailCliente { get; set; }
        public List<Compras> Items { get; set; } = new();
    }
}