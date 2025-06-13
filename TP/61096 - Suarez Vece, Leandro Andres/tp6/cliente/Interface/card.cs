namespace cliente.Models;

public interface ICardItem
{
    string? ImagenUrl { get; }
    string Nombre { get; }
    string? Descripcion { get; }
    decimal Precio { get; }
    int Id { get; }
    int Stock { get; set; }

}