using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace servidor.Services
{
    public class ProductoService
    {
        private readonly AppDbContext _context;

        public ProductoService(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todos los productos desde la base de datos
        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            return await _context.Productos.ToListAsync();
        }

        // Obtener un producto por ID
        public async Task<Producto> ObtenerPorIdAsync(int id)
        {
            return await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
        }

        // Agregar un nuevo producto
        public async Task AgregarAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
        }

        // Agregar productos iniciales si la tabla está vacía
       public async Task AgregarProductosInicialesAsync()
{
    if (!_context.Productos.Any()) // Solo si la tabla está vacía
    {
        var productos = new List<Producto>
        {
            new Producto { Nombre = "Laptop Dell XPS 13", Descripcion = "Ultrabook con Intel i7.", Precio = 1299.99m, Stock = 15 },
            new Producto { Nombre = "iPhone 14 Pro", Descripcion = "Smartphone con pantalla OLED.", Precio = 999.00m, Stock = 10 },
            new Producto { Nombre = "Audífonos Sony WH-1000XM5", Descripcion = "Cancelación de ruido.", Precio = 349.99m, Stock = 25 },
            new Producto { Nombre = "Smartwatch Samsung Galaxy Watch 6", Descripcion = "GPS integrado.", Precio = 279.00m, Stock = 18 },
            new Producto { Nombre = "Mouse Logitech MX Master 3", Descripcion = "Ergonómico e inalámbrico.", Precio = 99.99m, Stock = 30 },
            new Producto { Nombre = "Monitor LG UltraWide 34”", Descripcion = "Resolución QHD, ideal para diseño.", Precio = 449.00m, Stock = 12 },
            new Producto { Nombre = "Teclado Mecánico Keychron K8", Descripcion = "Bluetooth y cable USB-C.", Precio = 89.00m, Stock = 20 },
            new Producto { Nombre = "Cámara Canon EOS R10", Descripcion = "Mirrorless de 24MP.", Precio = 1099.00m, Stock = 8 },
            new Producto { Nombre = "Impresora HP Smart Tank 7301", Descripcion = "Multifuncional con Wi-Fi.", Precio = 249.00m, Stock = 10 },
            new Producto { Nombre = "Bocina JBL Charge 5", Descripcion = "Portátil y resistente al agua.", Precio = 149.00m, Stock = 22 }
        };

        _context.Productos.AddRange(productos);
        await _context.SaveChangesAsync();

        Console.WriteLine("Productos agregados exitosamente!");
    }
}


        // Actualizar un producto existente
        public async Task ActualizarAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

    }    
}       // Eliminar un producto