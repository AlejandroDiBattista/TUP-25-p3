using cliente.Models;

public interface ICarritoService

{

    event Action? CarritoActualizado;
    Task<List<ItemCarritoResponse>> ObtenerItemsCarritoAsync();
    Task AgregarAlCarritoAsync(int productoId);

    Task ActualizarCantidadAsync(int productoId, int nuevaCantidad);

     Task EliminarProductoAsync(int productoId);     // 👈 Agregado
    Task VaciarCarritoAsync();                      // 👈 Agregado

}