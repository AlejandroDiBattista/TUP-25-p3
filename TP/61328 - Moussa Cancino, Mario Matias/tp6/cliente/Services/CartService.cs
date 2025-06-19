using cliente.Modelos;

namespace cliente.Services;

public class CartService
{
    public List<CarritoItem> Items { get; private set; } = new();
    public Guid? CartId { get; private set; }
    
    // Nueva propiedad para calcular el total de ítems
    public int ItemCount => Items.Sum(item => item.Cantidad);
    
    // Diccionario para trackear el stock reservado por producto
    private Dictionary<int, int> _reservedStock = new();
    
    public event Action? OnChange;

    private readonly ApiService _apiService;

    public CartService(ApiService apiService)
    {
        _apiService = apiService;
    }

    private async Task EnsureCartExistsAsync()
    {
        if (!CartId.HasValue)
        {
            CartId = await _apiService.CreateCartAsync();
        }
    }

    public async Task AddToCart(Producto producto)
    {
        await EnsureCartExistsAsync();
        if (CartId is null) return;
        
        var updatedCart = await _apiService.AddProductToCartAsync(CartId.Value, producto.Id);
        if (updatedCart != null)
        {
            Items = updatedCart;
            
            // Actualizar stock reservado
            UpdateReservedStock();
            
            NotifyStateChanged();
        }
    }

    public async Task RemoveFromCart(int productoId)
    {
        if (!CartId.HasValue) return;
        
        var updatedCart = await _apiService.RemoveProductFromCartAsync(CartId.Value, productoId);
        if (updatedCart != null)
        {
            Items = updatedCart;
            
            // Actualizar stock reservado
            UpdateReservedStock();
            
            NotifyStateChanged();
        }
    }
    
    public async Task EmptyCartAsync()
    {
        if(!CartId.HasValue) return;
        
        await _apiService.EmptyCartAsync(CartId.Value);
        Items.Clear();
        CartId = null;
        
        // Limpiar stock reservado
        _reservedStock.Clear();
        
        NotifyStateChanged();
    }

    // Método para obtener el stock disponible considerando lo que está en el carrito
    public int GetAvailableStock(Producto producto)
    {
        var reservedForThisProduct = _reservedStock.GetValueOrDefault(producto.Id, 0);
        return producto.Stock - reservedForThisProduct;
    }

    // Método para verificar si se puede agregar más cantidad de un producto
    public bool CanAddToCart(Producto producto)
    {
        return GetAvailableStock(producto) > 0;
    }

    // Método privado para actualizar el stock reservado basado en el carrito actual
    private void UpdateReservedStock()
    {
        _reservedStock.Clear();
        
        foreach (var item in Items)
        {
            _reservedStock[item.ProductoId] = item.Cantidad;
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}