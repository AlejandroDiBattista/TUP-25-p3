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
    Task<CompraPendienteDto> GetCarritoPendiente();
    Task<List<CompraGetDto>> GetHistorial(Page page);
    Task CarritoInit(CompraDto dto);
    Task ActualizarCarrito(int id, ItemCompraDto dto);
    Task ConfirmarCompra(int id, ConfirmarCompraDto dto);
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
    public async Task<CompraPendienteDto> GetCarritoPendiente()
    {
        var res = await _context.Compras.Where(x => x.Entregado == false)
                        .Select(x => new CompraPendienteDto
                        {
                            Id_compra = x.Id_compra,
                            Fecha = x.Fecha,
                            Entregado = x.Entregado
                        })
                         .FirstOrDefaultAsync();
        return res;
    }
    public async Task<List<CompraGetDto>> GetHistorial(Page page)
    {
        var res = await _context.Compras
            .Where(x => x.Entregado == true)
            .OrderBy(c => c.Fecha)
            .Skip(page.pageIndex * page.pageSize)
            .Take(page.pageSize)
            .Select(x => new CompraGetDto
            {
                Id_compra = x.Id_compra,
                NombreCliente = x.NombreCliente,
                ApellidoCliente = x.ApellidoCliente,
                EmailCliente = x.EmailCliente,
                Fecha = x.Fecha,
                Items = new List<ItemCompraGtDto>()
            })
            .ToListAsync();

        //problema de n+1 pero funca y se ve fachero
        if (res != null)
        {
            foreach (var compra in res)
            {
                compra.Items = await GetPorductsCarrito(compra.Id_compra);
            }
        }
        return res;
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
        var data = new Compra { Fecha = dto.Fecha, Entregado = false };
        _context.Compras.Add(data);
        await _context.SaveChangesAsync();

    }
    public async Task ConfirmarCompra(int id, ConfirmarCompraDto dto)
    {
        var res = await _context.Compras.FindAsync(id);
        if (res != null)
        {
            res.NombreCliente = dto.NombreCliente;
            res.ApellidoCliente = dto.ApellidoCliente;
            res.EmailCliente = dto.EmailCliente;
            res.Entregado = true;
            await _context.SaveChangesAsync();
        }
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