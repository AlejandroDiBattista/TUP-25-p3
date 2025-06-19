namespace cliente.Modelos;

public class CarritoItem // ¡Este es el nuevo modelo para el cliente!
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty; // Coincide con backend
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; } // Coincide con backend
}


//correcion para subir a github