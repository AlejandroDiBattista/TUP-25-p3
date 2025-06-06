using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Servidor.Models;
using MYContext;
using Servidor.Dto;

namespace Services;

public interface IPruductServices
{
    Task<List<Producto>> GetPorducts(string? query);
    Task<List<ItemCompraGtDto>> GetPorductsCarrito(int id);
    Task CarritoInit(CompraDto dto);
    Task ActualizarCarrito(int id, ItemCompraDto dto);

    Task ElimnarCarrito(int id);
    Task ElimnarPorudctoCarrito(int idCompra, int Id_iten);

}

public class ProductService : IPruductServices
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Producto>> GetPorducts(string? busqueda)
    {

        var query = _context.Productos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            query = query.Where(p =>
                p.Nombre.Contains(busqueda)
               );
        }

        return await query.ToListAsync();
    }

    public async Task<List<ItemCompraGtDto>> GetPorductsCarrito(int id)
    {
        var res = await _context.ItemsCompras
                .Join(
                    _context.Productos,
                    item => item.ProductoId,
                    producto => producto.Id_producto,
                    (item, producto) => new ItemCompraGtDto
                    {
                        Id_iten = item.Id_iten,
                        Cantidad = item.Cantidad,
                        ProductoId = producto.Id_producto,
                        NombreProducto = producto.Nombre,
                        PrecioProducto = item.PrecioUnitario,
                        CompraId = item.CompraId
                    }
                )
                .Where(x => x.CompraId == id)
                .ToListAsync();

        return res;
    }

    public async Task CarritoInit(CompraDto dto)
    {
        var data = new Compra { Fecha = dto.Fecha };
        _context.Compras.Add(data);
        await _context.SaveChangesAsync();

    }
    public async Task ActualizarCarrito(int id, ItemCompraDto dto)
    {
        var prod = await _context.Productos.FindAsync(dto.ProductoId);

        if (prod != null && prod.Stock >= dto.Cantidad)
        {
            var buscar = await _context.ItemsCompras
                    .FirstOrDefaultAsync(x => x.ProductoId == dto.ProductoId && x.CompraId == id);

            if (buscar != null)
            {
                buscar.Cantidad = dto.Cantidad;
            }
            else
            {
                var nuevoItem = new ItemCompra
                {
                    ProductoId = dto.ProductoId,
                    CompraId = id,
                    Cantidad = dto.Cantidad,
                    PrecioUnitario = dto.PrecioUnitario
                };

                await _context.ItemsCompras.AddAsync(nuevoItem);
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task ElimnarCarrito(int id)
    {
        var res = await _context.Compras.FindAsync(id);
        if (res != null)
        {
            _context.Compras.Remove(res);
            await _context.SaveChangesAsync();
        }
    }
    public async Task ElimnarPorudctoCarrito(int idCompra, int Id_iten)
    {
        var res = await _context.ItemsCompras
        .Where(x => x.CompraId == idCompra && x.Id_iten == Id_iten)
        .FirstOrDefaultAsync();
        if (res != null)
        {
            _context.ItemsCompras.Remove(res);
            await _context.SaveChangesAsync();
        }
    }


}