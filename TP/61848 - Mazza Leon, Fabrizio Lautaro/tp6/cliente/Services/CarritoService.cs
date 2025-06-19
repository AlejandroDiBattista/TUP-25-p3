using System.Net.Http;
using System.Threading.Tasks;
using cliente.Models;

namespace cliente.Services
{
    public class CarritoService
    {
        public int CarritoId { get; set; } = 0;
        public List<ItemCarrito> items = new();

        public IReadOnlyList<ItemCarrito> Items => items;
    
        public int CantidadTotal => Items.Sum(i => i.Cantidad);

        private readonly HttpClient _httpClient;

        public CarritoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task InicializarCarritoAsync(ApiService apiService)
        {
            if (CarritoId == 0)
            {
                CarritoId = await apiService.CrearCarritoAsync();
            }
        }

        public void CambiarCantidad(int productoId, int nuevaCantidad, List<Producto> productosCatalogo = null)
        {
            var item = Items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null)
            {
                int cantidadAnterior = item.Cantidad;
                if (nuevaCantidad == 0)
                {
                    items.Remove(item);
                }
                else if (nuevaCantidad > 0 && nuevaCantidad <= item.Producto.Stock + cantidadAnterior)
                {
                    item.Cantidad = nuevaCantidad;
                }
                // Actualizar stock en catálogo si se pasa la lista
                if (productosCatalogo != null)
                {
                    var productoCatalogo = productosCatalogo.FirstOrDefault(p => p.Id == productoId);
                    if (productoCatalogo != null)
                    {
                        productoCatalogo.Stock += (cantidadAnterior - nuevaCantidad);
                        if (productoCatalogo.Stock < 0) productoCatalogo.Stock = 0;
                    }
                }
            }
        }
        

        public void AgregarProducto(Producto producto)
        {
            var item = items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                item.Cantidad++;
            }
            else
            {
                items.Add(new ItemCarrito { Producto = producto, Cantidad = 1 });
            }
        }

         public async Task AgregarProductoAsync(int productoId)
        {
            if (CarritoId == 0)
                throw new InvalidOperationException("Carrito no inicializado.");

            var response = await _httpClient.PutAsync($"/carritos/{CarritoId}/{productoId}", null);
            response.EnsureSuccessStatusCode();
        }

        public void Vaciar()
        {
            items.Clear();
        }

        public decimal CalcularTotal()
        {
            return items.Sum(i => i.Producto.Precio * i.Cantidad);
        }
    }
}