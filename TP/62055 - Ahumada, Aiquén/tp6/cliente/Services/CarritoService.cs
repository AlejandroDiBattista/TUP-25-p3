using cliente.Models;

namespace cliente.Services;

public class CarritoService
{
    public List<Producto> Items { get; private set; } = new();

    public event Action OnChange;

    public void Agregar(Producto producto)
    {
        Items.Add(producto);
        OnChange?.Invoke();
    }

    public void Quitar(Producto producto)
    {
        Items.Remove(producto);
        OnChange?.Invoke();
    }

    public void Vaciar()
    {
        Items.Clear();
        OnChange?.Invoke();
    }
}