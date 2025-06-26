namespace shared.DTOs;

public class CarritoResponse
{
    public string CarritoId { get; set; } = string.Empty;
}

public class CompraResponse
{
    public string CompraId { get; set; } = string.Empty;
    public decimal Total { get; set; }
}