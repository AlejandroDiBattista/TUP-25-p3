using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace servidor.Services
{
    public class CarritoItemService // 🔥 Se corrigió el nombre de la clase
    {
        private readonly AppDbContext _context;

        public CarritoItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CarritoItem>> ObtenerCarritoItemsAsync()
        {
            return await _context.CarritoItems.ToListAsync();
        }
    }
}
