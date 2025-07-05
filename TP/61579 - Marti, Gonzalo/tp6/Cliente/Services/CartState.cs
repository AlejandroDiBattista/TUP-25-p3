namespace TiendaOnline.Cliente.Services;


public class CartState
{
    public int CartId { get; set; } = 0;
    public event Action? OnChange;
    private int _items;
    public int Items
    {
        get => _items;
        set
        {
            _items = value;
            OnChange?.Invoke();
        }
    }
}
