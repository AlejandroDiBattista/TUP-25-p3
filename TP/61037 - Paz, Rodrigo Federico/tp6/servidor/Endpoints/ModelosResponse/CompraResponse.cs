namespace servidor.Endpoints.ModelosResponse;

public class CompraResponse
{
    public int Id { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public List<ItemCompraResponse> Items { get; set; }

    public class ItemCompraResponse
    {
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}