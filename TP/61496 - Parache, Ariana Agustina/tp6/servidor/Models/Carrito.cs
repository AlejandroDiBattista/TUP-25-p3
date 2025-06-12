namespace servidor.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        public List<ItemCarrito> Items { get; set; } = new();
        public bool Confirmado { get; set; } = false;

        // Datos cliente (se llenan al confirmar)
        public string? NombreCliente { get; set; }
        public string? ApellidoCliente { get; set; }
        public string? EmailCliente { get; set; }
    }
}
