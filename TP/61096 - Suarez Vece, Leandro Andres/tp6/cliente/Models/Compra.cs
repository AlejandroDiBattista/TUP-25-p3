namespace cliente.Models;

public class CompraPendienteDto
{
    public int Id_compra { get; set; }
    public DateTime Fecha { get; set; }
    public bool Entregado { get; set; }
}
public class CompraGetDto
{
    public int Id_compra { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public DateTime Fecha { get; set; }
    public List<ItemCompraGtDto> Items { get; set; }

    public decimal Total => Items?.Sum(i => i.subTotal) ?? 0m;
}
public class ConfirmarCompraDto
{
    public string? NombreCliente { get; set; }


    public string? ApellidoCliente { get; set; }


    public string? EmailCliente { get; set; }
}

public class CompraDto
{
    public DateTime Fecha { get; set; } = DateTime.Now;
}