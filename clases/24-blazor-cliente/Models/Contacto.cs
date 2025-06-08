public class Contacto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
    public string Iniciales => (!string.IsNullOrEmpty(Nombre) ? Nombre[0].ToString().ToUpper() : "") + (!string.IsNullOrEmpty(Apellido) ? Apellido[0].ToString().ToUpper() : "");
}