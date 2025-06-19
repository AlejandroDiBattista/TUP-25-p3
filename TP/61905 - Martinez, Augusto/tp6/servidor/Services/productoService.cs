using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using servidor.Models;
using servidor.Data;

namespace servidor.Services
{
    public class ProductoService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductoService> _logger;

        public ProductoService(AppDbContext context, ILogger<ProductoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ Obtener todos los productos con validación mejorada
        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            try
            {
                var productos = await _context.Productos.AsNoTracking().ToListAsync();

                if (productos == null || productos.Count == 0)
                {
                    _logger.LogWarning("No se encontraron productos en la base de datos.");
                }

                return productos ?? new List<Producto>();
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al obtener productos: {ex.Message}");
                return new List<Producto>(); // Retorna lista vacía en caso de error
            }
        }

        // ✅ Obtener un producto específico por ID con validaciones
        public async Task<Producto> ObtenerProductoPorIdAsync(int id)
        {
            try
            {
                var producto = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                if (producto == null)
                {
                    _logger.LogWarning($"Producto con ID {id} no encontrado.");
                }
                return producto;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al obtener producto por ID {id}: {ex.Message}");
                return null;
            }
        }

        // ✅ Agregar productos iniciales solo si la tabla está vacía
        public async Task AgregarProductosInicialesAsync()
        {
            try
            {
                if (await _context.Productos.AnyAsync()) return; // Evita duplicados

                var productos = new List<Producto>
                {
                    new Producto { Nombre = "Laptop Dell XPS 13", Descripcion = "Ultrabook con Intel i7.", Precio = 1299.99m, Stock = 15, ImagenUrl = "/images/laptop.jpg" },
                    new Producto { Nombre = "iPhone 14 Pro", Descripcion = "Smartphone con pantalla OLED.", Precio = 999.00m, Stock = 10, ImagenUrl = "/images/iphone.jpg" },
                    new Producto { Nombre = "Audífonos Sony WH-1000XM5", Descripcion = "Cancelación de ruido.", Precio = 349.99m, Stock = 25, ImagenUrl = "/images/sony-headphones.jpg" },
                    new Producto { Nombre = "Smartwatch Samsung Galaxy Watch 6", Descripcion = "GPS integrado.", Precio = 279.00m, Stock = 18, ImagenUrl = "/images/samsung-watch.jpg" },
                    new Producto { Nombre = "Mouse Logitech MX Master 3", Descripcion = "Ergonómico e inalámbrico.", Precio = 99.99m, Stock = 30, ImagenUrl = "/images/logitech-mouse.jpg" },
                    new Producto { Nombre = "Monitor LG UltraWide 34”", Descripcion = "Resolución QHD, ideal para diseño.", Precio = 449.00m, Stock = 12, ImagenUrl = "/images/lg-monitor.jpg" },
                    new Producto { Nombre = "Teclado Mecánico Keychron K8", Descripcion = "Bluetooth y cable USB-C.", Precio = 89.00m, Stock = 20, ImagenUrl = "/images/keychron.jpg" },
                    new Producto { Nombre = "Cámara Canon EOS R10", Descripcion = "Mirrorless de 24MP.", Precio = 1099.00m, Stock = 8, ImagenUrl = "/images/canon-r10.jpg" },
                    new Producto { Nombre = "Impresora HP Smart Tank 7301", Descripcion = "Multifuncional con Wi-Fi.", Precio = 249.00m, Stock = 10, ImagenUrl = "/images/hp-printer.jpg" },
                    new Producto { Nombre = "Bocina JBL Charge 5", Descripcion = "Portátil y resistente al agua.", Precio = 149.00m, Stock = 22, ImagenUrl = "/images/jbl.jpg" }
                };

                await _context.Productos.AddRangeAsync(productos);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Productos iniciales agregados correctamente.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al agregar productos iniciales: {ex.Message}");
            }
        }
    }
}
