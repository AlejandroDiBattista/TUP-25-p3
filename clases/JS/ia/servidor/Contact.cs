using System.ComponentModel.DataAnnotations;

public class Contact {
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public string Apellido { get; set; }
    [Required]
    public string Telefono { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
