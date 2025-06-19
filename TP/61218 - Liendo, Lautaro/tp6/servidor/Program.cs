using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using servidor.Data;
using servidor.Models;
using servidor.DTOs;
using servidor.Helpers;
using Microsoft.AspNetCore.Http;

var constructorAppWeb = WebApplication.CreateBuilder(args);

constructorAppWeb.Services.AddDbContext<TiendaContext>(opcionesContexto =>
    opcionesContexto.UseSqlite("Data Source=tienda_ropa.db"));

constructorAppWeb.Services.AddCors(configuracionCors =>
{
    configuracionCors.AddPolicy("AccesoAppCliente", politicaCORS =>
    {
        politicaCORS.WithOrigins("http://localhost:5177", "https://localhost:7221")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
    });
});

constructorAppWeb.Services.AddControllers();

var aplicacion = constructorAppWeb.Build();

aplicacion.UseCors("AccesoAppCliente");

using (var alcanceServicio = aplicacion.Services.CreateScope())
{
    var contextoDBInicial = alcanceServicio.ServiceProvider.GetRequiredService<TiendaContext>();
    contextoDBInicial.Database.EnsureCreated();

    if (!contextoDBInicial.InventarioArticulos.Any())
    {
        contextoDBInicial.InventarioArticulos.AddRange(
            new ArticuloInventario { Denominacion = "Remera Algodón Básica", Caracteristicas = "Tejido suave, corte regular, disponible en S, M, L, XL.", ValorUnitario = 22.50, CantidadDisponible = 60, UrlImagenArticulo = "https://placehold.co/400x400/FF5733/FFFFFF?text=Remera" },
            new ArticuloInventario { Denominacion = "Jean Clásico Azul", Caracteristicas = "Modelo recto, mezclilla duradera, talles 28-38.", ValorUnitario = 55.00, CantidadDisponible = 45, UrlImagenArticulo = "https://placehold.co/400x400/3366FF/FFFFFF?text=Jean" },
            new ArticuloInventario { Denominacion = "Camisa Lino Blanca", Caracteristicas = "Ideal para verano, ligera y transpirable, talles S-XL.", ValorUnitario = 48.75, CantidadDisponible = 35, UrlImagenArticulo = "https://placehold.co/400x400/33FF57/FFFFFF?text=Camisa+Lino" },
            new ArticuloInventario { Denominacion = "Vestido Casual Estampado", Caracteristicas = "Corte holgado, patrón floral, talla única.", ValorUnitario = 65.00, CantidadDisponible = 20, UrlImagenArticulo = "https://placehold.co/400x400/8A2BE2/FFFFFF?text=Vestido" },
            new ArticuloInventario { Denominacion = "Short Deportivo Running", Caracteristicas = "Material sintético, secado rápido, con bolsillos.", ValorUnitario = 30.00, CantidadDisponible = 50, UrlImagenArticulo = "https://placehold.co/400x400/FF33CC/FFFFFF?text=Short+Deportivo" },
            new ArticuloInventario { Denominacion = "Zapatillas Blancas Urbanas", Caracteristicas = "Suela de goma, estilo minimalista, talles 36-44.", ValorUnitario = 80.00, CantidadDisponible = 30, UrlImagenArticulo = "https://placehold.co/400x400/00FFFF/FFFFFF?text=Zapatillas" },
            new ArticuloInventario { Denominacion = "Cinturón Cuero Genuino", Caracteristicas = "Hebilla metálica, ancho estándar, varios largos.", ValorUnitario = 28.00, CantidadDisponible = 40, UrlImagenArticulo = "https://placehold.co/400x400/A52A2A/FFFFFF?text=Cinturon" },
            new ArticuloInventario { Denominacion = "Gorra Baseball Algodón", Caracteristicas = "Ajustable, visera curva, ideal para el sol.", ValorUnitario = 15.00, CantidadDisponible = 70, UrlImagenArticulo = "https://placehold.co/400x400/808080/FFFFFF?text=Gorra" },
            new ArticuloInventario { Denominacion = "Buzo Friza Oversize", Caracteristicas = "Felpa suave, estilo holgado, muy cómodo.", ValorUnitario = 60.00, CantidadDisponible = 25, UrlImagenArticulo = "https://placehold.co/400x400/4B0082/FFFFFF?text=Buzo" },
            new ArticuloInventario { Denominacion = "Calcetines Deportivos Pack x3", Caracteristicas = "Algodón transpirable, soporte para arco, unisex.", ValorUnitario = 10.50, CantidadDisponible = 80, UrlImagenArticulo = "https://placehold.co/400x400/DAA520/FFFFFF?text=Calcetines" }
        );
        contextoDBInicial.SaveChanges();
    }
}

var sesionesDeComprarArticulos = new ConcurrentDictionary<string, List<DetalleCarritoMemoria>>();

aplicacion.MapGet("/", () => Results.Ok(new { MensajeServicio = "API de Tienda de Ropa Activa", HoraDelSistema = DateTime.Now }));

aplicacion.MapGet("/productos", async (TiendaContext gestorDB) =>
    await gestorDB.InventarioArticulos.ToListAsync()
);

aplicacion.MapGet("/productos/buscar/{textoConsulta}", async (TiendaContext gestorDB, string textoConsulta) =>
{
    var consultaArticulos = gestorDB.InventarioArticulos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(textoConsulta))
    {
        var textoNormalizado = textoConsulta.ToLowerInvariant();
        consultaArticulos = consultaArticulos.Where(item =>
            (item.Denominacion != null && item.Denominacion.ToLowerInvariant().Contains(textoNormalizado)) ||
            (item.Caracteristicas != null && item.Caracteristicas.ToLowerInvariant().Contains(textoNormalizado))
        );
    }
    return await consultaArticulos.ToListAsync();
});

aplicacion.MapPost("/carritos", () =>
{
    var idGeneradoParaCarrito = Guid.NewGuid().ToString();
    sesionesDeComprarArticulos[idGeneradoParaCarrito] = new List<DetalleCarritoMemoria>();
    return Results.Ok(new { identificadorNuevoCarrito = idGeneradoParaCarrito });
});

aplicacion.MapGet("/carritos/{identificadorCarrito}", (string identificadorCarrito) =>
{
    if (!sesionesDeComprarArticulos.TryGetValue(identificadorCarrito, out var elementosEnCarrito))
        return Results.NotFound("Identificador de carrito no válido o inexistente.");

    return Results.Ok(elementosEnCarrito);
});

aplicacion.MapDelete("/carritos/{idCarritoAVaciar}", async (string idCarritoAVaciar, TiendaContext gestorDB) =>
{
    if (!sesionesDeComprarArticulos.TryGetValue(idCarritoAVaciar, out var carrito))
        return Results.NotFound("Carrito no encontrado para vaciar.");

    foreach (var item in carrito)
    {
        var articulo = await gestorDB.InventarioArticulos.FindAsync(item.ArticuloId);
        if (articulo != null)
        {
            articulo.CantidadDisponible += item.Unidades;
        }
    }
    await gestorDB.SaveChangesAsync();

    sesionesDeComprarArticulos[idCarritoAVaciar] = new List<DetalleCarritoMemoria>();
    return Results.Ok();
});

aplicacion.MapPut("/carritos/{idCarrito}/anadir/{idArticulo}", async (string idCarrito, int idArticulo, int cantidadSolicitada, TiendaContext gestorDB) =>
{
    if (cantidadSolicitada <= 0)
        return Results.BadRequest("La cantidad de artículos debe ser un número positivo.");

    var articuloDelInventario = await gestorDB.InventarioArticulos.FindAsync(idArticulo);
    if (articuloDelInventario == null)
        return Results.NotFound("Artículo no hallado en el inventario.");

    var itemsDelCarrito = sesionesDeComprarArticulos.GetOrAdd(idCarrito, _ => new List<DetalleCarritoMemoria>());
    var itemExistenteEnCarrito = itemsDelCarrito.FirstOrDefault(i => i.ArticuloId == idArticulo);

    int cantidadReservadaAntes = itemExistenteEnCarrito?.Unidades ?? 0;

    int stockRealDisponible = articuloDelInventario.CantidadDisponible + cantidadReservadaAntes;

    if (cantidadSolicitada > stockRealDisponible)
        return Results.BadRequest($"Stock insuficiente para '{articuloDelInventario.Denominacion}'. Solo hay {stockRealDisponible} unidades disponibles.");

    articuloDelInventario.CantidadDisponible = stockRealDisponible - cantidadSolicitada;
    await gestorDB.SaveChangesAsync();

    if (itemExistenteEnCarrito == null)
    {
        itemsDelCarrito.Add(new DetalleCarritoMemoria
        {
            ArticuloId = idArticulo,
            Unidades = cantidadSolicitada,
            ValorPorUnidad = articuloDelInventario.ValorUnitario
        });
    }
    else
    {
        itemExistenteEnCarrito.Unidades = cantidadSolicitada;
    }

    return Results.Ok(itemsDelCarrito);
});

aplicacion.MapDelete("/carritos/{idCarrito}/remover/{idArticulo}", async (string idCarrito, int idArticulo, int cantidadAReducir, TiendaContext gestorDB) =>
{
    if (!sesionesDeComprarArticulos.TryGetValue(idCarrito, out var itemsDelCarrito))
        return Results.NotFound("Identificador de carrito no válido.");

    var itemParaGestionar = itemsDelCarrito.FirstOrDefault(i => i.ArticuloId == idArticulo);
    if (itemParaGestionar == null)
        return Results.NotFound("Artículo no encontrado en este carrito.");

    int cantidadAReducirReal = cantidadAReducir <= 0 ? itemParaGestionar.Unidades : cantidadAReducir;
    if (cantidadAReducirReal > itemParaGestionar.Unidades)
        cantidadAReducirReal = itemParaGestionar.Unidades;

    var articulo = await gestorDB.InventarioArticulos.FindAsync(idArticulo);
    if (articulo == null)
        return Results.NotFound("Artículo no encontrado en inventario.");

    articulo.CantidadDisponible += cantidadAReducirReal;
    await gestorDB.SaveChangesAsync();

    if (cantidadAReducirReal == itemParaGestionar.Unidades)
    {
        itemsDelCarrito.Remove(itemParaGestionar);
    }
    else
    {
        itemParaGestionar.Unidades -= cantidadAReducirReal;
    }

    return Results.Ok(itemsDelCarrito);
});

aplicacion.MapPut("/carritos/{idCarrito}/confirmar", async (string idCarrito, DatosClienteDTO infoComprador, TiendaContext gestorDB) =>
{
    if (!ValidacionUtil.EsDatosClienteValidos(infoComprador))
    {
        return Results.BadRequest(new { error = "Datos del cliente incompletos. Se requieren Nombre, Apellido y Correo Electrónico." });
    }

    if (!sesionesDeComprarArticulos.TryGetValue(idCarrito, out var elementosParaConfirmar))
        return Results.BadRequest(new { error = "El identificador del carrito proporcionado no existe o es inválido." });

    if (elementosParaConfirmar.Count == 0)
        return Results.BadRequest(new { error = "El carrito está vacío, no se puede finalizar la compra." });

    foreach (var detalleActual in elementosParaConfirmar)
    {
        var articuloEnStock = await gestorDB.InventarioArticulos.FindAsync(detalleActual.ArticuloId);
        if (articuloEnStock == null)
            return Results.BadRequest(new { error = $"Artículo con ID {detalleActual.ArticuloId} no encontrado en el inventario." });
    }

    double totalTransaccion = 0;
    var nuevoRegistroCompra = new RegistroCompra
    {
        FechaRealizacion = DateTime.Now,
        NombreCliente = infoComprador.NombreSolicitante,
        ApellidoCliente = infoComprador.ApellidoSolicitante,
        EmailCliente = infoComprador.CorreoElectronicoContacto,
        Detalles = new List<DetalleCompra>()
    };

    foreach (var itemAProcesar in elementosParaConfirmar)
    {
        var articuloParaRegistro = await gestorDB.InventarioArticulos.FindAsync(itemAProcesar.ArticuloId);

        nuevoRegistroCompra.Detalles.Add(new DetalleCompra
        {
            ArticuloInventarioId = articuloParaRegistro.Id,
            CantidadAdquirida = itemAProcesar.Unidades,
            PrecioUnitarioAlMomento = articuloParaRegistro.ValorUnitario
        });

        totalTransaccion += articuloParaRegistro.ValorUnitario * itemAProcesar.Unidades;
    }
    nuevoRegistroCompra.MontoTotal = totalTransaccion;
    gestorDB.HistorialCompras.Add(nuevoRegistroCompra);
    await gestorDB.SaveChangesAsync();

    sesionesDeComprarArticulos[idCarrito] = new List<DetalleCarritoMemoria>();

    return Results.Ok(new { ID_ConfirmacionCompra = nuevoRegistroCompra.Id, ValorFinalCompra = nuevoRegistroCompra.MontoTotal });
});

aplicacion.Run();

public class DetalleCarritoMemoria
{
    public int ArticuloId { get; set; }
    public int Unidades { get; set; }
    public double ValorPorUnidad { get; set; }
}
