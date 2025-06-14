using Servidor.Modelos;

public class CompraConfirmacionDTO
{
    public List<CarritoItemDTO> Items { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
}