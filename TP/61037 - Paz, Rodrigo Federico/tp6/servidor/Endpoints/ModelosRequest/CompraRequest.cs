namespace servidor.Endpoints.ModelosRequest;

public class CompraRequest
{
    public string NombreCliente { get; set; } = string.Empty;
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
    public List<ItemCompraRequest> Items { get; set; } = new();
}

public class ItemCompraRequest
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}