using System;

namespace Cliente.Models2
{
    /// <summary>
    /// Representa una respuesta simple del servidor con mensaje y fecha.
    /// </summary>
    public class DatosRespuesta
    {
        /// <summary>
        /// Mensaje enviado por el servidor.
        /// </summary>
        public string Mensaje { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora de la respuesta.
        /// </summary>
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
// Este código define una clase que representa la respuesta del servidor con un mensaje y una fecha.
// Es útil para manejar respuestas simples desde el backend, como confirmaciones o mensajes de estado.