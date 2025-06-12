    // servidor/Models/CompraConfirmationDto.cs
    using System.ComponentModel.DataAnnotations;

    namespace servidor.Models // Asegúrate de que el namespace sea 'servidor.Models'
    {
        public class CompraConfirmationDto
        {
            [Required]
            [MaxLength(100)]
            public string NombreCliente { get; set; } = string.Empty;
            [Required]
            [MaxLength(100)]
            public string ApellidoCliente { get; set; } = string.Empty;
            [Required]
            [MaxLength(100)]
            [EmailAddress]
            public string EmailCliente { get; set; } = string.Empty;
        }
    }
    