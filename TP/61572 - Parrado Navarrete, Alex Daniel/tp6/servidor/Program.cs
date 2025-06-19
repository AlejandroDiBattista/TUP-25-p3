// === Program.cs (Servidor) ===
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Servidor.Data;
using TiendaOnline.Servidor.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) Forzar URLs: HTTP 5000 y HTTPS 5001
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

// 2) CORS (antes de Build)
builder.Services.AddCors(o => o.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
));

// 3) DbContext
var dataFolder = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataFolder);
var conn = $"Data Source={Path.Combine(dataFolder, "tienda.db")}";
builder.Services.AddDbContext<AppDbContext>(opts => opts.UseSqlite(conn));

// 4) Swagger & Dev exception
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// 5) Seed inicial
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    ctx.Database.EnsureCreated();
    DbInitializer.Initialize(ctx);
}

// 6) Middleware
app.UseCors();  // **Muy importante** antes de Map*
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tienda API V1"));
}
app.UseHttpsRedirection();

// 7) Única definición de GET /productos con filtro por ?q=
app.MapGet("/productos", async (string? q, AppDbContext db) =>
{
    var query = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        var term = q.ToLower();
        query = query.Where(p =>
            p.Nombre.ToLower().Contains(term) ||
            p.Descripcion.ToLower().Contains(term));
    }
    var lista = await query.ToListAsync();
    return Results.Ok(lista);
});

// 8) A partir de aquí van tus endpoints de carrito (solo una vez cada uno):
app.MapPost("/carritos", async (AppDbContext db) =>
{
    var c = new Compra {
        Fecha           = DateTime.UtcNow,
        Total           = 0,
        NombreCliente   = string.Empty,
        ApellidoCliente = string.Empty,
        EmailCliente    = string.Empty
    };
    db.Compras.Add(c);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{c.Id}", c.Id);
});

app.MapGet("/carritos/{cid:int}", async (int cid, AppDbContext db) =>
    Results.Ok(await db.ItemsCompra.Where(i => i.CompraId == cid).ToListAsync())
);

app.MapPut("/carritos/{cid:int}/confirmar", async (int cid, Compra cli, AppDbContext db) =>
{
    // 1) buscamos la compra existente
    var c = await db.Compras.FindAsync(cid);
    if (c is null) 
        return Results.NotFound();

    // 2) actualizamos los campos desde el JSON del body
    c.NombreCliente   = cli.NombreCliente;
    c.ApellidoCliente = cli.ApellidoCliente;
    c.EmailCliente    = cli.EmailCliente;

    // 3) calculamos el Total y vaciamos los items
    var items = await db.ItemsCompra.Where(i => i.CompraId == cid).ToListAsync();
    c.Total           = items.Sum(i => i.Cantidad * i.PrecioUnitario);
    db.ItemsCompra.RemoveRange(items);

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/carritos/{cid:int}/{pid:int}", async (int cid, int pid, AppDbContext db) =>
{
    var it = await db.ItemsCompra.FirstOrDefaultAsync(i => i.CompraId == cid && i.ProductoId == pid);
    if (it == null) return Results.NotFound();
    var prod = await db.Productos.FindAsync(pid);
    if (prod != null) prod.Stock += it.Cantidad;
    db.ItemsCompra.Remove(it);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/carritos/{cid:int}", async (int cid, AppDbContext db) =>
{
    var items = await db.ItemsCompra.Where(i => i.CompraId == cid).ToListAsync();
    foreach (var it in items)
        if (await db.Productos.FindAsync(it.ProductoId) is { } prod) 
            prod.Stock += it.Cantidad;
    db.ItemsCompra.RemoveRange(items);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapPut("/carritos/{cid:int}/{pid:int}", async (int cid, int pid, ItemUpdate dto, AppDbContext db) =>
{
    // dto.Cantidad = nueva cantidad deseada
    var item = await db.ItemsCompra
                       .FirstOrDefaultAsync(i => i.CompraId == cid && i.ProductoId == pid);
    var prod = await db.Productos.FindAsync(pid);

    if (prod is null) 
        return Results.NotFound("Producto no existe");
    
    // Si no existe el ítem y quieren qty>0, crearlo
    if (item is null)
    {
        if (dto.Cantidad < 1 || prod.Stock < dto.Cantidad)
            return Results.BadRequest("Sin stock suficiente");
        item = new ItemCompra
        {
            CompraId      = cid,
            ProductoId    = pid,
            Cantidad      = dto.Cantidad,
            PrecioUnitario = prod.Precio
        };
        prod.Stock -= dto.Cantidad;
        db.ItemsCompra.Add(item);
    }
    else
    {
        var diff = dto.Cantidad - item.Cantidad;
        // aumento de cantidad
        if (diff > 0)
        {
            if (prod.Stock < diff) 
                return Results.BadRequest("Sin stock suficiente");
            prod.Stock -= diff;
            item.Cantidad = dto.Cantidad;
        }
        // decremento de cantidad
        else if (diff < 0)
        {
            prod.Stock += -diff;
            item.Cantidad = dto.Cantidad;
        }

        // si llega a cero, borrarlo
        if (item.Cantidad == 0)
            db.ItemsCompra.Remove(item);
    }

    await db.SaveChangesAsync();
    // devolvemos la lista actualizada
    var items = await db.ItemsCompra.Where(i => i.CompraId == cid).ToListAsync();
    return Results.Ok(items);
});

// 8) Run
app.Run();

// Endpoints de carritos
app.MapPost("/carritos", async (AppDbContext db) =>
{
    var c = new Compra { Fecha = DateTime.UtcNow, Total = 0 };
    db.Compras.Add(c);
    await db.SaveChangesAsync();
    return Results.Created($"/carritos/{c.Id}", c.Id);
});

app.MapGet("/carritos/{cid:int}", async (int cid, AppDbContext db) =>
{
    var items = await db.ItemsCompra
                        .Where(i => i.CompraId == cid)
                        .ToListAsync();
    return items.Any() ? Results.Ok(items) : Results.NotFound();
});

app.MapDelete("/carritos/{cid:int}", async (int cid, AppDbContext db) =>
{
    var items = await db.ItemsCompra.Where(i => i.CompraId == cid).ToListAsync();
    foreach (var item in items)
    {
        var p = await db.Productos.FindAsync(item.ProductoId);
        if (p != null) p.Stock += item.Cantidad;
    }
    db.ItemsCompra.RemoveRange(items);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/carritos/{cid:int}/{pid:int}", async (int cid, int pid, AppDbContext db) =>
{
    var p = await db.Productos.FindAsync(pid);
    if (p == null || p.Stock < 1) return Results.BadRequest("Sin stock");
    var itm = await db.ItemsCompra.FirstOrDefaultAsync(i => i.CompraId == cid && i.ProductoId == pid);
    if (itm is null)
    {
        itm = new ItemCompra {
            CompraId      = cid,
            ProductoId    = pid,
            Cantidad      = 1,
            PrecioUnitario = p.Precio
        };
        db.ItemsCompra.Add(itm);
    }
    else
    {
        if (p.Stock < itm.Cantidad + 1) return Results.BadRequest("Sin stock");
        itm.Cantidad++;
    }
    p.Stock--;
    await db.SaveChangesAsync();
    return Results.Ok(itm);
});

app.MapDelete("/carritos/{cid:int}/{pid:int}", async (int cid, int pid, AppDbContext db) =>
{
    var itm = await db.ItemsCompra.FirstOrDefaultAsync(i => i.CompraId == cid && i.ProductoId == pid);
    if (itm is null) return Results.NotFound();
    var p = await db.Productos.FindAsync(pid);
    if (p != null) p.Stock += itm.Cantidad;
    db.ItemsCompra.Remove(itm);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/carritos/{cid:int}/confirmar", async (int cid, Compra cli, AppDbContext db) =>
{
    var c = await db.Compras.FindAsync(cid);
    if (c is null) return Results.NotFound();
    var items = await db.ItemsCompra.Where(i => i.CompraId == cid).ToListAsync();
    c.NombreCliente   = cli.NombreCliente;
    c.ApellidoCliente = cli.ApellidoCliente;
    c.EmailCliente    = cli.EmailCliente;
    c.Total           = items.Sum(i => i.Cantidad * i.PrecioUnitario);
    db.ItemsCompra.RemoveRange(items);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();