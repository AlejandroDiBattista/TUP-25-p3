// Program.cs
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using servidor.Data;
using servidor.Models;
using servidor.DTOs;
using servidor.Helpers;
using Microsoft.AspNetCore.Http; // Necesario para StatusCodes

// ---
// 1. Configuración del constructor de la aplicación web
// ---
var constructorAppWeb = WebApplication.CreateBuilder(args);

// Configuración de la base de datos SQLite con el nuevo nombre del DbSet
constructorAppWeb.Services.AddDbContext<TiendaContext>(opcionesContexto =>
    opcionesContexto.UseSqlite("Data Source=tienda_ropa.db")); // Nombre de la DB también cambiado

// Configuración de la política CORS
constructorAppWeb.Services.AddCors(configuracionCors =>
{
    configuracionCors.AddPolicy("AccesoAppCliente", politicaCORS => // Nombre de la política cambiado
    {
        politicaCORS.WithOrigins("http://localhost:5177", "https://localhost:7221")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
    });
});

constructorAppWeb.Services.AddControllers();

// ---
// 2. Construcción de la aplicación y configuración de middleware
// ---
var aplicacion = constructorAppWeb.Build(); // Nombre de la instancia de la aplicación

aplicacion.UseCors("AccesoAppCliente"); // Uso del nuevo nombre de la política CORS

// ---
// 3. Inicialización de la base de datos y carga de datos de ejemplo (¡Ahora con ropa!)
// ---
using (var alcanceServicio = aplicacion.Services.CreateScope()) // Nombre de la variable de scope
{
    var contextoDBInicial = alcanceServicio.ServiceProvider.GetRequiredService<TiendaContext>(); // Nombre del contexto
    contextoDBInicial.Database.EnsureCreated();
    
    if (!contextoDBInicial.InventarioArticulos.Any()) // Usando el nuevo DbSet 'InventarioArticulos'
    {
        contextoDBInicial.InventarioArticulos.AddRange(
            // Lista de productos de ropa con URLs de imagen de ejemplo
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

// ---
// 4. Almacenamiento en memoria para las sesiones de carrito
// ---
// Mantiene los estados de los carritos de los usuarios mientras la aplicación está en ejecución.
var sesionesDeComprarArticulos = new ConcurrentDictionary<string, List<DetalleCarritoMemoria>>(); // Nuevo tipo interno

// ---
// 5. Definición de Endpoints de la API (Manteniendo las rutas exactas del TP)
// ---

// Endpoint raíz de la API
aplicacion.MapGet("/", () => Results.Ok(new { MensajeServicio = "API de Tienda de Ropa Activa", HoraDelSistema = DateTime.Now }));

// Endpoints relacionados con el inventario de artículos
// Obtener todos los artículos del inventario
aplicacion.MapGet("/productos", async (TiendaContext gestorDB) => {
    return await gestorDB.InventarioArticulos.ToListAsync(); // Usando el nuevo DbSet
});

// Buscar artículos por texto en su denominación o características
aplicacion.MapGet("/productos/buscar/{textoConsulta}", async (TiendaContext gestorDB, string textoConsulta) => {
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

// Endpoints relacionados con la gestión de carritos
// Inicializar un nuevo carrito de compra
aplicacion.MapPost("/carritos", () => {
    var idGeneradoParaCarrito = Guid.NewGuid().ToString();
    sesionesDeComprarArticulos[idGeneradoParaCarrito] = new List<DetalleCarritoMemoria>();
    return Results.Ok(new { identificadorNuevoCarrito = idGeneradoParaCarrito });
});

// Obtener los detalles de un carrito específico
aplicacion.MapGet("/carritos/{identificadorCarrito}", async (string identificadorCarrito, TiendaContext gestorDB) => {
    if (!sesionesDeComprarArticulos.TryGetValue(identificadorCarrito, out var elementosEnCarrito))
        return Results.NotFound("Identificador de carrito no válido o inexistente.");

    var detallesExpandidos = new List<object>();
    foreach (var itemDeCarrito in elementosEnCarrito)
    {
        var articuloAsociado = await gestorDB.InventarioArticulos.FindAsync(itemDeCarrito.ArticuloId);
        detallesExpandidos.Add(new
        {
            ID_Articulo = itemDeCarrito.ArticuloId,
            NombreDelArticulo = articuloAsociado?.Denominacion ?? "Artículo Desconocido",
            CantidadSeleccionada = itemDeCarrito.Unidades,
            PrecioPorUnidad = itemDeCarrito.ValorPorUnidad
        });
    }
    return Results.Ok(detallesExpandidos);
});

// Vaciar un carrito de compra
aplicacion.MapDelete("/carritos/{idCarritoAVaciar}", (string idCarritoAVaciar) => {
    if (!sesionesDeComprarArticulos.ContainsKey(idCarritoAVaciar))
        return Results.NotFound("Carrito no encontrado para vaciar.");
    
    sesionesDeComprarArticulos[idCarritoAVaciar] = new List<DetalleCarritoMemoria>(); 
    return Results.Ok();
});

// Añadir o actualizar un artículo en el carrito
aplicacion.MapPut("/carritos/{idCarrito}/anadir/{idArticulo}", async (string idCarrito, int idArticulo, int cantidadSolicitada, TiendaContext gestorDB) => {
    if (cantidadSolicitada <= 0)
        return Results.BadRequest("La cantidad de artículos debe ser un número positivo.");

    var articuloDelInventario = await gestorDB.InventarioArticulos.FindAsync(idArticulo);
    if (articuloDelInventario == null)
        return Results.NotFound("Artículo no hallado en el inventario.");

    if (articuloDelInventario.CantidadDisponible < cantidadSolicitada)
        return Results.BadRequest($"Stock insuficiente para '{articuloDelInventario.Denominacion}'. Solo hay {articuloDelInventario.CantidadDisponible} unidades disponibles.");

    var itemsDelCarrito = sesionesDeComprarArticulos.GetOrAdd(idCarrito, _ => new List<DetalleCarritoMemoria>());
    var itemExistenteEnCarrito = itemsDelCarrito.FirstOrDefault(i => i.ArticuloId == idArticulo);

    if (itemExistenteEnCarrito == null)
    {
        itemsDelCarrito.Add(new DetalleCarritoMemoria { 
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

// Eliminar o reducir la cantidad de un artículo en el carrito
aplicacion.MapDelete("/carritos/{idCarrito}/remover/{idArticulo}", (string idCarrito, int idArticulo, int cantidadAReducir = 0) => {
    if (!sesionesDeComprarArticulos.TryGetValue(idCarrito, out var itemsDelCarrito))
        return Results.NotFound("Identificador de carrito no válido.");

    var itemParaGestionar = itemsDelCarrito.FirstOrDefault(i => i.ArticuloId == idArticulo);
    if (itemParaGestionar == null)
        return Results.NotFound("Artículo no encontrado en este carrito.");

    if (cantidadAReducir <= 0 || cantidadAReducir >= itemParaGestionar.Unidades)
        itemsDelCarrito.Remove(itemParaGestionar); // Elimina el artículo completo del carrito
    else
        itemParaGestionar.Unidades -= cantidadAReducir; // Reduce la cantidad especificada

    return Results.Ok(itemsDelCarrito);
});

// Endpoint para confirmar una compra y persistirla
aplicacion.MapPut("/carritos/{idCarrito}/confirmar", async (
    HttpContext contextoHttp,
    string idCarrito,
    TiendaContext gestorDB
) => {
    try
    {
        using var lectorDeContenido = new StreamReader(contextoHttp.Request.Body);
        var cuerpoSolicitud = await lectorDeContenido.ReadToEndAsync();
        
        DatosClienteDTO infoComprador; // Uso del DTO renombrado
        try {
            infoComprador = System.Text.Json.JsonSerializer.Deserialize<DatosClienteDTO>(cuerpoSolicitud);
            
            // Usando el método auxiliar de la clase Helpers/ValidacionUtil
            if (!ValidacionUtil.EsDatosClienteValidos(infoComprador))
            {
                return Results.BadRequest(new { error = "Datos del cliente incompletos. Se requieren Nombre, Apellido y Correo Electrónico." });
            }
        }
        catch (System.Text.Json.JsonException jsonEx) // Se captura la excepción y se usa para eliminar la advertencia
        {
            // Opcional: Console.WriteLine($"Error de deserialización JSON: {jsonEx.Message}");
            return Results.BadRequest(new { error = "Formato de los datos del cliente inválido." });
        }
        
        if (!sesionesDeComprarArticulos.TryGetValue(idCarrito, out var elementosParaConfirmar))
            return Results.BadRequest(new { error = "El identificador del carrito proporcionado no existe o es inválido." });
            
        if (elementosParaConfirmar.Count == 0)
            return Results.BadRequest(new { error = "El carrito está vacío, no se puede finalizar la compra." });

        // Primera verificación: asegurar que hay suficiente stock para todos los artículos
        foreach (var detalleActual in elementosParaConfirmar)
        {
            var articuloEnStock = await gestorDB.InventarioArticulos.FindAsync(detalleActual.ArticuloId);
            if (articuloEnStock == null)
                return Results.BadRequest(new { error = $"Artículo con ID {detalleActual.ArticuloId} no encontrado en el inventario." });
                
            if (articuloEnStock.CantidadDisponible < detalleActual.Unidades)
                return Results.BadRequest(new { error = $"Stock insuficiente para '{articuloEnStock.Denominacion}'. Cantidad disponible: {articuloEnStock.CantidadDisponible}." });
        }

        // Segunda fase: Procesar la compra, ajustar stock y calcular el total
        double totalTransaccion = 0;
        var nuevoRegistroCompra = new RegistroCompra
        {
            FechaRealizacion = DateTime.Now,
            NombreCliente = infoComprador.NombreSolicitante,
            ApellidoCliente = infoComprador.ApellidoSolicitante,
            EmailCliente = infoComprador.CorreoElectronicoContacto,
            Detalles = new List<DetalleCompra>() // Inicializar la lista de detalles
        };

        foreach (var itemAProcesar in elementosParaConfirmar)
        {
            var articuloParaActualizar = await gestorDB.InventarioArticulos.FindAsync(itemAProcesar.ArticuloId);
            articuloParaActualizar.CantidadDisponible -= itemAProcesar.Unidades; // Reducir stock
            totalTransaccion += articuloParaActualizar.ValorUnitario * itemAProcesar.Unidades; // Sumar al total
            
            nuevoRegistroCompra.Detalles.Add(new DetalleCompra
            {
                ArticuloInventarioId = articuloParaActualizar.Id,
                CantidadAdquirida = itemAProcesar.Unidades,
                PrecioUnitarioAlMomento = articuloParaActualizar.ValorUnitario
            });
        }
        nuevoRegistroCompra.MontoTotal = totalTransaccion;
        gestorDB.HistorialCompras.Add(nuevoRegistroCompra); // Añadir el nuevo registro de compra
        await gestorDB.SaveChangesAsync(); // Guardar todos los cambios en la base de datos

        // Vaciar el carrito en memoria después de una confirmación exitosa
        sesionesDeComprarArticulos[idCarrito] = new List<DetalleCarritoMemoria>(); 
        return Results.Ok(new { ID_ConfirmacionCompra = nuevoRegistroCompra.Id, ValorFinalCompra = nuevoRegistroCompra.MontoTotal });
    } 
    catch (Exception ex) // Captura cualquier excepción genérica
    {
        // La variable 'ex' se usa aquí, eliminando la advertencia CS0168.
        // Para debugging, puedes descomentar la siguiente línea (no recomendado en producción por seguridad).
        // Console.WriteLine($"Error inesperado en confirmación de compra: {ex.Message} - {ex.StackTrace}");

        // CORRECCIÓN: Uso de Results.Problem() para devolver un error HTTP 500 estándar.
        return Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Error interno del servidor",
            detail: "Ha ocurrido un error inesperado al procesar su compra. Por favor, inténtelo de nuevo."
        );
    }
});

// ---
// 6. Ejecución de la aplicación
// ---
aplicacion.Run();

// ---
// 7. Clases para manejo interno del carrito en memoria (No para EF Core)
//    Esta es una clase simple para los ítems que viven en la ConcurrentDictionary
// ---
public class DetalleCarritoMemoria // Antes ItemCarrito, pero este es solo para la memoria
{
    public int ArticuloId { get; set; } // Antes ProductoId
    public int Unidades { get; set; } // Antes Cantidad
    public double ValorPorUnidad { get; set; } // Antes PrecioUnitario
}