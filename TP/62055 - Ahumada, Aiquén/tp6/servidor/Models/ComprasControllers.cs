
using Microsoft.AspNetCore.Mvc;
using servidor.Data;
using servidor.Models;

namespace servidor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly TiendaDbContext _context;

        public ComprasController(TiendaDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> RegistrarCompra([FromBody] CompraDTO compraDto)
        {
            // Crear la compra
            var compra = new Compra
            {
                Nombre = compraDto.Nombre,
                Apellido = compraDto.Apellido,
                Email = compraDto.Email,
                Fecha = DateTime.Now,
                Items = new List<ItemCompra>()
            };

            foreach (var itemDto in compraDto.Items)
            {
                // Disminuir el stock del producto
                var producto = await _context.Productos.FindAsync(itemDto.ProductoId);
                if (producto != null)
                {
                    producto.Stock -= itemDto.Cantidad;
                    if (producto.Stock < 0)
                        producto.Stock = 0;
                }

                compra.Items.Add(new ItemCompra
                {
                    ProductoId = itemDto.ProductoId,
                    NombreProducto = itemDto.NombreProducto,
                    PrecioUnitario = itemDto.PrecioUnitario,
                    Cantidad = itemDto.Cantidad
                });
            }

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    // DTOs para recibir la compra
    public class CompraDTO
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public List<ItemCompraDTO> Items { get; set; }
    }

    public class ItemCompraDTO
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
    }
}