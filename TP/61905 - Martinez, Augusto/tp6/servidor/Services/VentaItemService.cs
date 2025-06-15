using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace servidor.Services
{
    public class VentaItemService // 🔥 Corrige si aparece "CarritoService" aquí
    {
        private readonly AppDbContext _context;

        public VentaItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VentaItem>> ObtenerVentaItemsAsync()
        {
            return await _context.VentaItems.ToListAsync();
        }
    }
}

