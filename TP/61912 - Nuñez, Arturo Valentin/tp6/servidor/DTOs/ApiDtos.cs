
namespace Servidor.DTOs
{
    // Parámetros para los PUT
    public record QuantityUpdate(int Cantidad);
    public record ClienteData(string Nombre, string Apellido, string Email);
}
