using System.Net.Mail;
using servidor.DTOs;  

namespace servidor.Helpers
{
    public static class ValidacionUtil
    {
        public static bool EsDatosClienteValidos(DatosClienteDTO datosCliente)
        {
            if (datosCliente == null)
                return false;

            if (string.IsNullOrWhiteSpace(datosCliente.NombreSolicitante))
                return false;

            if (string.IsNullOrWhiteSpace(datosCliente.ApellidoSolicitante))
                return false;

            if (string.IsNullOrWhiteSpace(datosCliente.CorreoElectronicoContacto))
                return false;

            try
            {
                var mail = new MailAddress(datosCliente.CorreoElectronicoContacto);
                return mail.Address == datosCliente.CorreoElectronicoContacto;
            }
            catch
            {
                return false;
            }
        }
    }
}
