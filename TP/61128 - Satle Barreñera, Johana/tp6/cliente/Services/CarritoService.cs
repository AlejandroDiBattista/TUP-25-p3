using cliente.Modelos;

namespace cliente.Services;

public class CarritoService
{
    public List<Producto> ProductosEnCarrito { get; private set; } = new();

    public event Action? OnChange;

    public void AgregarProducto(Producto producto)
    {
        var existente = ProductosEnCarrito.FirstOrDefault(p => p.Id == producto.Id);

        if (existente != null)
        {
            existente.Cantidad += 1;
        }
        else
        {
            // Clonamos el producto con cantidad inicial 1
            ProductosEnCarrito.Add(new Producto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                ImagenUrl = producto.ImagenUrl,
                Cantidad = 1
            });
        }

        OnChange?.Invoke();
    }

    public void EliminarProducto(int id)
    {
        var producto = ProductosEnCarrito.FirstOrDefault(p => p.Id == id);
        if (producto != null)
        {
            ProductosEnCarrito.Remove(producto);
            OnChange?.Invoke();
        }
    }

    public void LimpiarCarrito()
    {
        ProductosEnCarrito.Clear();
        OnChange?.Invoke();
    }

    public decimal CalcularTotal() =>
        ProductosEnCarrito.Sum(p => p.Precio * p.Cantidad);



        public void AumentarCantidad(int id)
    {
        var producto = ProductosEnCarrito.FirstOrDefault(p => p.Id == id);
        if (producto != null && producto.Cantidad < producto.Stock)
        {
            producto.Cantidad++;
            OnChange?.Invoke();
        }
    }

    public void DisminuirCantidad(int id)
    {
        var producto = ProductosEnCarrito.FirstOrDefault(p => p.Id == id);
        if (producto != null)
        {
            producto.Cantidad--;
            if (producto.Cantidad <= 0)
            {
                ProductosEnCarrito.Remove(producto);
            }
            OnChange?.Invoke();
        }
    }
}