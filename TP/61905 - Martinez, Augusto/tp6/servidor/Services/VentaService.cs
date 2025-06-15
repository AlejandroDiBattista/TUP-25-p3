using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace servidor.Services
{
    public class VentaService
    {
        private readonly AppDbContext _context;

        public VentaService(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Método corregido para obtener las ventas
        public async Task<List<Venta>> ObtenerVentasAsync()
        {
            return await _context.Ventas.Include(v => v.Items).ToListAsync();
        }

        // 🔹 Método para registrar una venta
        public async Task RegistrarVentaAsync(Venta venta)
        {
            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();
        }
    }
}
