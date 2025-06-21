namespace TiendaOnline.Frontend.Services;

public class CartState
{
    public Guid? CarritoId { get; private set; }
    public int   TotalItems { get; private set; }
    public event Action? Changed;

    public void Init(Guid id, int total = 0)
    {
        CarritoId  = id;
        TotalItems = total;
        Changed?.Invoke();
    }

    public void SetTotal(int t) { TotalItems = t; Changed?.Invoke(); }

    public void Reset()
    {
        CarritoId = null;
        TotalItems = 0;
        Changed?.Invoke();
    }
}