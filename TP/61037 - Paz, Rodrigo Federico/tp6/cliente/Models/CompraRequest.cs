namespace cliente.Models
{
    public class CompraRequest
    {
        public string Nombre { get; set; } = ""; 
        public string Apellido { get; set; } = "";
        public string Email { get; set; } = "";
        public List<ItemCompraRequest> Items { get; set; } = new();
    }

    public class ItemCompraRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
