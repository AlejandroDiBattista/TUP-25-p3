// DTOs/DatosClienteDTO.cs
namespace servidor.DTOs
{
    public class DatosClienteDTO
    {
        public string NombreSolicitante { get; set; } = string.Empty; // Antes NombreComprador
        public string ApellidoSolicitante { get; set; } = string.Empty; // Antes ApellidoComprador
        public string CorreoElectronicoContacto { get; set; } = string.Empty; // Antes EmailContacto
    }
}