using cliente.Modelos;
#nullable enable

namespace cliente.Services;

    public class CarritoService
    {
        public List<ItemCarrito> Items { get; private set; } = new();

        public string CarritoId { get; private set; } = Guid.NewGuid().ToString();

        public decimal Total => Items.Sum(i => i.Importe);
        public int TotalItems => Items.Sum(i => i.Cantidad);
        private readonly HttpClient _http;

    public CarritoService(HttpClient http)
    {
        _http = http;
    }

    public async Task Vaciar()
    {
        await _http.DeleteAsync($"api/carritos/{CarritoId}");
        Items.Clear();
        OnChange?.Invoke();
    }
        
    public event Action? OnChange;

      public async Task AgregarProductoAsync(Producto producto)
{
    if (string.IsNullOrEmpty(CarritoId))
        await Inicializar();

    var response = await _http.PutAsync($"/carritos/{CarritoId}/{producto.Id}", null);
    if (response.IsSuccessStatusCode)
    {
        var item = Items.FirstOrDefault(i => i.Producto.Id == producto.Id);
        if (item != null)
        {
            item.Cantidad++;
        }
        else
        {
            Items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
        }

        OnChange?.Invoke();
    }
    else
    {
        throw new Exception("No se pudo agregar el producto al carrito en el servidor.");
    }
}

        public void QuitarProducto(int productoId)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                item.Cantidad--;
                if (item.Cantidad <= 0)
                    Items.Remove(item);
            }

            OnChange?.Invoke();
        }


    public void EliminarProducto(int productoId)
    {
        var item = Items.FirstOrDefault(i => i.Producto.Id == productoId);
        if (item != null)
            Items.Remove(item);

        OnChange?.Invoke();
    }
    public async Task Inicializar()
{
    if (string.IsNullOrEmpty(CarritoId))
    {
        var response = await _http.PostAsync("/carritos", null);
        if (response.IsSuccessStatusCode)
        {
            CarritoId = await response.Content.ReadAsStringAsync();
            CarritoId = CarritoId.Trim('"');
        }
        else
        {
            throw new Exception("No se pudo crear el carrito en el servidor.");
        }
    }
}

          private void NotifyStateChanged() => OnChange?.Invoke();
    }

