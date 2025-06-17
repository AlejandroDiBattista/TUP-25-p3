using System.Collections.Generic;
namespace Cliente.Services
{
    public class CartService
    {
        public List<Product> Items { get; } = new();

        public event Action? OnChange;

        public void AddToCart(Product product)
        {
            Items.Add(product);
            OnChange?.Invoke();
        }

        public void RemoveFromCart(Product product)
        {
            Items.Remove(product);
            OnChange?.Invoke();
        }
 public void ClearCart()
        {
            Items.Clear();
            OnChange?.Invoke();
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}