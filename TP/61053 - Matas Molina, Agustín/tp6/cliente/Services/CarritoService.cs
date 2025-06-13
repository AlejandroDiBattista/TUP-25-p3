public class CarritoService
{
    public event Action? OnChange;
    private List<ProductoCarrito> productos = new();

    public IReadOnlyList<ProductoCarrito> Productos => productos;

    public int CantidadTotal => productos.Sum(p => p.Cantidad);

    public void AgregarProducto(ProductoCarrito producto)
    {
        var existente = productos.FirstOrDefault(p => p.Id == producto.Id);
        if (existente != null)
            existente.Cantidad++;
        else
            productos.Add(new ProductoCarrito
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                ImagenUrl = producto.ImagenUrl,
                Precio = producto.Precio,
                Cantidad = 1
            });
        OnChange?.Invoke();
    }

    public void SumarCantidad(int productoId)
    {
        var prod = productos.FirstOrDefault(p => p.Id == productoId);
        if (prod != null)
        {
            prod.Cantidad++;
            OnChange?.Invoke();
        }
    }

    public void RestarCantidad(int productoId)
    {
        var prod = productos.FirstOrDefault(p => p.Id == productoId);
        if (prod != null)
        {
            prod.Cantidad--;
            if (prod.Cantidad <= 0)
                productos.Remove(prod);
            OnChange?.Invoke();
        }
    }

    public void EliminarProducto(int productoId)
    {
        var prod = productos.FirstOrDefault(p => p.Id == productoId);
        if (prod != null)
        {
            productos.Remove(prod);
            OnChange?.Invoke();
        }
    }

    public void Vaciar()
    {
        productos.Clear();
        OnChange?.Invoke();
    }
}