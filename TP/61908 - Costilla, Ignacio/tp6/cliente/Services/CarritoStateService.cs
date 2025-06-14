namespace cliente.Services;

public class CarritoStateService
{
    public int? CarritoId { get; private set; }
    public int NumeroDeItems { get; private set; }

    public event Action? OnChange;

    public void SetCarritoId(int id)
    {
        CarritoId = id;
        NotifyStateChanged();
    }

    public void SetNumeroDeItems(int count)
    {
        NumeroDeItems = count;
        NotifyStateChanged();
    }

    public void LimpiarCarrito()
    {
        CarritoId = null;
        NumeroDeItems = 0;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}