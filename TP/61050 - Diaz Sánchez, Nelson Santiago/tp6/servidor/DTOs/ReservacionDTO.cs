namespace servidor.DTOs;

public class ItemReservaDTO
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
}

public class ReservacionDTO
{
    public string NombreCliente { get; set; } = "Pendiente";
    public string ApellidoCliente { get; set; } = "Pendiente";
    public string EmailCliente { get; set; } = "pendiente@email.com";
    public List<ItemReservaDTO> Items { get; set; }
}
