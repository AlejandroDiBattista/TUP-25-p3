using Microsoft.EntityFrameworkCore;
using servidor.ModelosServidor; 

var builder = WebApplication.CreateBuilder(args);

// Conexión a SQLite con EF Core
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TiendaSqlite") 
                    ?? "Data Source=tienda.db"));

// CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins( "http://localhost:5177",
                            "https://localhost:7221",
                            "http://localhost:5184" )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Controladores 
builder.Services.AddControllers();

var app = builder.Build();

// Aplicar migraciones al inicio
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.Migrate();
}

// Pipeline
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseCors("AllowClientApp");

app.MapGet("/", () => "Servidor API está en funcionamiento");
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

// ====== endpoints =======

// 1) Listar Productos (opcion para buscar por nombre)
app.MapGet("/productos", async (TiendaContext db, string? q) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q));
    return await query.ToListAsync();
});


// 2) Crear un carrito nuevo (inicia con compra vacia)
app.MapPost("/carritos", async (TiendaContext db) =>
{
    var compra = new Compra
    {
        Fecha = DateTime.UtcNow,
        Total = 0m,
        NombreCliente = "",
        ApellidoCliente = "",
        EmailCliente = "",
        Items = new List<ItemCompra>()
    };
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{compra.Id}", compra.Id);
});

// 3) Obtener items de un carrito
app.MapGet("/carritos/{id:int}", async (int id, TiendaContext db) =>
{
    var items = await db.ItemsCompra
        .Include(ic => ic.Producto)
        .Where(ic => ic.CompraId == id)
        .ToListAsync();
    return Results.Ok(items);
});


// 4) Vaciar un carrito (eliminar los items)
app.MapDelete("/carritos/{id:int}", async (int id, TiendaContext db) =>
{
    var items = db.ItemsCompra.Where(ic => ic.CompraId == id);
    db.ItemsCompra.RemoveRange(items);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// 5) Confirmar compra (registrar cliente y calcular el total)
app.MapPut("/carritos/{id:int}/confirmar", async (int id, CheckoutDto datos, TiendaContext db) =>
{
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
    if (compra == null)
        return Results.NotFound($"Carrito {id} no existe.");

    // Copiar datos del cuerpo en la entidad
    compra.NombreCliente = datos.NombreCliente;
    compra.ApellidoCliente = datos.ApellidoCliente;
    compra.EmailCliente = datos.EmailCliente;

    // Recalcular total
    compra.Total = compra.Items.Sum(i => i.Cantidad * i.PrecioUnitario);

    await db.SaveChangesAsync();
    return Results.Ok(compra);
});

// 6) Agregar o actualizar cantidad de un producto en el carrito
app.MapPut("/carritos/{id:int}/{prodId:int}", async (int id, int prodId, int cantidad, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(prodId);
    if (producto == null)
        return Results.BadRequest($"Producto {prodId} no existe.");

    if (producto.Stock < cantidad)
        return Results.BadRequest("Stock insuficiente.");

    // Buscar item existente
    var item = await db.ItemsCompra
        .FirstOrDefaultAsync(ic => ic.CompraId == id && ic.ProductoId == prodId);

    if (item == null)
    {
        // Crear nuevo ítem
        item = new ItemCompra
        {
            CompraId = id,
            ProductoId = prodId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        };
        producto.Stock -= cantidad;
        db.ItemsCompra.Add(item);
    }
    else
    {
        // Ajustar cantidad
        int delta = cantidad - item.Cantidad;
        if (producto.Stock < delta)
            return Results.BadRequest("Stock insuficiente para aumentar la cantidad.");

        item.Cantidad = cantidad;
        producto.Stock -= delta;
        db.ItemsCompra.Update(item);
    }

    await db.SaveChangesAsync();
    return Results.Ok(item);
});

// 7) Eliminar o reducir cantidad de un producto en el carrito
app.MapDelete("/carritos/{id:int}/{prodId:int}", async (int id, int prodId, TiendaContext db) =>
{
    var item = await db.ItemsCompra
        .FirstOrDefaultAsync(ic => ic.CompraId == id && ic.ProductoId == prodId);
    if (item == null)
        return Results.NotFound($"Producto {prodId} no está en el carrito {id}.");

    // Devolver stock
    var producto = await db.Productos.FindAsync(prodId);
    if (producto != null)
        producto.Stock += item.Cantidad;

    db.ItemsCompra.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.Run();
