using Shared.Models;

public class CarritoService
{
    public List<CarritoItemDto> Items { get; set; } = new();

    public void Agregar(ProductoDto producto)
    {
        var item = Items.FirstOrDefault(x => x.ProductoId == producto.Id);
        if (item == null)
        {
            Items.Add(new CarritoItemDto
            {
                ProductoId = producto.Id,
                Nombre = producto.Nombre,
                Cantidad = 1,
                PrecioUnitario = producto.Precio,
                ImagenUrl = producto.ImagenUrl
            });
        }
        else
        {
            item.Cantidad++;
        }
    }

    public void Quitar(CarritoItemDto item)
    {
        Items.Remove(item);
    }

    public void CambiarCantidad(CarritoItemDto item, int delta)
    {
        item.Cantidad += delta;
        if (item.Cantidad < 1) item.Cantidad = 1;
    }

    public void Vaciar() => Items.Clear();

    public decimal Total() => Items.Sum(x => x.PrecioUnitario * x.Cantidad);
}