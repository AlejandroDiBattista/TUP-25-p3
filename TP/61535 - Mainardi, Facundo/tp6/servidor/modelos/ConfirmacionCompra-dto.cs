using Servidor.Modelos;
namespace Servidor.DTOs;

public class ConfirmacionCompraDto
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public List<ItemCarrito> Items { get; set; }
}