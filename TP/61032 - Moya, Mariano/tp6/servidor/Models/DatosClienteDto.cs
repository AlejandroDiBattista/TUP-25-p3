namespace Servidor.Models;

/// <summary>
/// DTO para recibir los datos del cliente al confirmar la compra.
/// </summary>
public record DatosClienteDto(string Nombre, string Apellido, string Email);
