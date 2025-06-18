// DTOs/DatosClienteDTO.cs
namespace cliente.DTOs
{
    // Data Transfer Object para enviar los datos del cliente al backend al confirmar la compra.
    public class DatosClienteDTO
    {
        public string NombreSolicitante { get; set; } = string.Empty; // Nombre del cliente
        public string ApellidoSolicitante { get; set; } = string.Empty; // Apellido del cliente
        public string CorreoElectronicoContacto { get; set; } = string.Empty; // Email del cliente
    }
}
