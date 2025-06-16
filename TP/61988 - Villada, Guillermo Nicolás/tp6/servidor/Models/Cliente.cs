using System.ComponentModel.DataAnnotations;

namespace servidor.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
    }
}