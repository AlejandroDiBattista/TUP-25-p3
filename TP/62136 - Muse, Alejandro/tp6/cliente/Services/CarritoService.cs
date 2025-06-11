using cliente.Models;

namespace cliente.Services;

public class CarritoService
{
    public List<Producto> ProductosEnCarrito { get; } = new();

    public event Action? OnChange;

    public void Agregar(Producto producto)
    {
        ProductosEnCarrito.Add(producto);
        OnChange?.Invoke();
    }

    public int Cantidad => ProductosEnCarrito.Count;
}