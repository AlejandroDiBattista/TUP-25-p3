namespace servidor.Models;

public class CompraDTO
{
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public List<ItemCompraDTO> Items { get; set; } = new();
}

public class ItemCompraDTO
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}
