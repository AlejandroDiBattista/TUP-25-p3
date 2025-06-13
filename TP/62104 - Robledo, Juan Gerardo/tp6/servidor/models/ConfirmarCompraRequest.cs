namespace servidor.models
{
    public class ConfirmarCompraRequest
    {
        [Required]
        public string NombreCliente { get; set; } = string.Empty;
        [Required]
        public string ApellidoCliente { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string EmailCliente { get; set; } = string.Empty;
    }
}