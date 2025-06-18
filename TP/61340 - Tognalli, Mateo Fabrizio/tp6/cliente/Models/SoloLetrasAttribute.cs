using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Cliente.Models;

/// <summary>
/// Atributo de validación personalizado que verifica que el texto solo contenga letras, espacios y caracteres acentuados
/// </summary>
public class SoloLetrasAttribute : ValidationAttribute
{
    private readonly Regex _regex = new Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$", RegexOptions.Compiled);

    public SoloLetrasAttribute()
    {
        ErrorMessage = "Solo se permiten letras, espacios y acentos";
    }    public override bool IsValid(object value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true; // La validación de Required se encarga de los valores nulos/vacíos
        }

        return _regex.IsMatch(value.ToString());
    }
}
