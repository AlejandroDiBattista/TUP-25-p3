using servidor.Models;

namespace servidor.Cart;

public class CartService
{
    private readonly Dictionary<string, Dictionary<int, int>> _carts = new();

    public string CreateCart()
    {
        var id = Guid.NewGuid().ToString();
        _carts[id] = new();
        return id;
    }

    public Dictionary<int, int> GetCart(string cartId)
    {
        return _carts.TryGetValue(cartId, out var cart) ? cart : new();
    }

    public void ClearCart(string cartId)
    {
        if (_carts.ContainsKey(cartId))
            _carts[cartId].Clear();
    }

    public void SetItem(string cartId, int productId, int quantity)
    {
        if (!_carts.ContainsKey(cartId)) _carts[cartId] = new();
        if (quantity <= 0)
            _carts[cartId].Remove(productId);
        else
            _carts[cartId][productId] = quantity;
    }

    public void RemoveItem(string cartId, int productId)
    {
        if (_carts.TryGetValue(cartId, out var cart))
            cart.Remove(productId);
    }
}
