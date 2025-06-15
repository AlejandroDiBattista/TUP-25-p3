namespace servidor.Endpoints.ModelosRequest;

public class CompraRequest
{
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public List<ItemCompraRequest> Items { get; set; }

    public class ItemCompraRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
    
}