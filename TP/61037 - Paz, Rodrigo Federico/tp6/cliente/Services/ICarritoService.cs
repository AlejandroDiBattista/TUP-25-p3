using cliente.Models;

public interface ICarritoService

{
    Task<List<ItemCarritoResponse>> ObtenerItemsCarritoAsync();
    Task AgregarAlCarritoAsync(int productoId);

    Task ActualizarCantidadAsync(int productoId, int nuevaCantidad);
}