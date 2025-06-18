// Helpers/ValidacionUtil.cs
using servidor.DTOs; // Importa el DTO

namespace servidor.Helpers
{
    public static class ValidacionUtil
    {
        public static bool EsDatosClienteValidos(DatosClienteDTO dtoCliente)
        {
            return dtoCliente != null &&
                   !string.IsNullOrWhiteSpace(dtoCliente.NombreSolicitante) &&
                   !string.IsNullOrWhiteSpace(dtoCliente.ApellidoSolicitante) &&
                   !string.IsNullOrWhiteSpace(dtoCliente.CorreoElectronicoContacto);
        }
    }
}