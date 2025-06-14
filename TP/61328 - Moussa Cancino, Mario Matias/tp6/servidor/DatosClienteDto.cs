namespace servidor
{
    public class DatosClienteDto
    {
        
        [System.ComponentModel.DataAnnotations.Required]
        public string NombreCliente { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string ApellidoCliente { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string EmailCliente { get; set; }
    }
}