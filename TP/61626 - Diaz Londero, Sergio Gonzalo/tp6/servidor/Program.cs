// --------------------------------------------------------------------------------------
// Program.cs - Backend Minimal API para Tienda Online (Blazor WASM + EF Core/SQLite)
// Archivo exhaustivamente comentado línea por línea y por bloque.
// --------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc; // Para atributos y tipos de ASP.NET Core
using Microsoft.EntityFrameworkCore; // Para Entity Framework Core
using Servidor.Dtos; // Para los DTOs definidos en la carpeta Dtos

// Crea el builder de la aplicación web
var builder = WebApplication.CreateBuilder(args);

// Configura CORS para permitir peticiones desde el cliente Blazor (localhost:5177)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177") // Solo permite este origen
              .AllowAnyHeader() // Permite cualquier header
              .AllowAnyMethod(); // Permite cualquier método HTTP
    });
});

// Configura el contexto de base de datos con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db")); // Usa SQLite y crea/usa el archivo app.db

// Construye la aplicación
var app = builder.Build();

// Aplica la política de CORS definida arriba
app.UseCors("AllowClientApp");

// Crea la base de datos y carga datos iniciales (seed) si está vacía
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // Crea la base si no existe

    // Si no hay productos, agrega un set inicial
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Botin de Futbol", Descripcion = "Botín profesional", Precio = 199999m, Stock = 15, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/botin2.jpg" },
            new Producto { Nombre = "Buzo Deportivo", Descripcion = "Buzo de algodón con capucha", Precio = 80000m, Stock = 25, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/buzo.jpg" },
            new Producto { Nombre = "Campera de Abrigo", Descripcion = "Campera impermeable ", Precio = 130000m, Stock = 18, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/campera.jpg" },
            new Producto { Nombre = "Pantalon Jogger", Descripcion = "Pantalón cómodo estilo urbano", Precio = 60000m, Stock = 30, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/pantalon.jpg" },
            new Producto { Nombre = "Remera Estampada", Descripcion = "Remera de algodón ", Precio = 35000m, Stock = 40, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/remera2.jpg" },
            new Producto { Nombre = "Remera Basica", Descripcion = "Remera lisa color blanco", Precio = 30000m, Stock = 50, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/remera.jpg" },
            new Producto { Nombre = "Short Deportivo", Descripcion = "Short de secado rápido para entrenamiento", Precio = 30000m, Stock = 35, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/short.jpg" },
            new Producto { Nombre = "Zapatilla Urbana", Descripcion = "Zapatilla moderna y cómoda para el día a día", Precio = 180000m, Stock = 20, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/zapatilla2.jpg" },
            new Producto { Nombre = "Zapatilla Running", Descripcion = "Zapatilla ideal para correr largas distancias", Precio = 190000m, Stock = 22, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catMujer/zapatilla.jpg" },
            new Producto { Nombre = "Zapatilla Clasica", Descripcion = "Zapatilla estilo retro de lona", Precio = 70000m, Stock = 28, ImagenUrl = "https://nextgen-grupo2.netlify.app/img/catHombre/zapatilla.jpg" }
        );
        db.SaveChanges(); // Guarda los productos iniciales
    }
}

// --- ENDPOINTS API ---

// GET /productos - Devuelve la lista de productos, permite filtrar por nombre (q)
app.MapGet("/productos", async ([FromQuery] string? q, AppDbContext db) =>
{
    var query = db.Productos.AsQueryable(); // Obtiene todos los productos
    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(p => p.Nombre.Contains(q)); // Filtra por nombre si hay query
    return await query.ToListAsync(); // Devuelve la lista
});

// POST /carritos - Crea un nuevo carrito (compra vacía)
app.MapPost("/carritos", async (AppDbContext db) =>
{
    var compra = new Compra
    {
        Fecha = DateTime.Now,
        NombreCliente = "",
        ApellidoCliente = "",
        EmailCliente = "",
        Total = 0
    };
    db.Compras.Add(compra); // Agrega la compra vacía
    await db.SaveChangesAsync(); // Guarda en la base
    return Results.Ok(compra.Id); // Devuelve el ID del carrito
});

// GET /carritos/{carritoId} - Devuelve los ítems de un carrito
app.MapGet("/carritos/{carritoId}", async (int carritoId, AppDbContext db) =>
{
    var items = await db.ItemsCompra
        .Include(i => i.Producto)
        .Where(i => i.CompraId == carritoId)
        .ToListAsync();
    return Results.Ok(items); // Devuelve los ítems
});

// DELETE /carritos/{carritoId} - Elimina todos los ítems de un carrito
app.MapDelete("/carritos/{carritoId}", async (int carritoId, AppDbContext db) =>
{
    var items = db.ItemsCompra.Where(i => i.CompraId == carritoId);
    db.ItemsCompra.RemoveRange(items); // Elimina todos los ítems
    await db.SaveChangesAsync();
    return Results.Ok();
});

// PUT /carritos/{carritoId}/{productoId}?cantidad=3 - Agrega o suma producto al carrito
app.MapPut("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, [FromQuery] int cantidad, AppDbContext db) =>
{
    if (cantidad <= 0)
        return Results.BadRequest("La cantidad debe ser mayor a cero.");

    // Busca el producto por su ID
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado.");

    // Busca si ya existe un ítem de ese producto en el carrito
    var item = await db.ItemsCompra
        .FirstOrDefaultAsync(i => i.CompraId == carritoId && i.ProductoId == productoId);

    if (item == null)
    {
        // Si no existe, valida stock y lo agrega al carrito
        if (producto.Stock < cantidad)
            return Results.BadRequest("Stock insuficiente");

        item = new ItemCompra
        {
            CompraId = carritoId,
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioUnitario = producto.Precio
        };
        db.ItemsCompra.Add(item);
        producto.Stock -= cantidad; // Descuenta stock al agregar
    }
    else
    {
        // Si ya existe, suma la cantidad y descuenta stock
        if (producto.Stock < cantidad)
            return Results.BadRequest("Stock insuficiente");

        item.Cantidad += cantidad;
        producto.Stock -= cantidad;
    }

    await db.SaveChangesAsync(); // Guarda cambios
    return Results.Ok();
});

// DELETE /carritos/{carritoId}/{productoId} - Elimina un producto del carrito y devuelve stock
app.MapDelete("/carritos/{carritoId}/{productoId}", async (int carritoId, int productoId, AppDbContext db) =>
{
    // Busca el ítem en el carrito
    var item = await db.ItemsCompra
        .FirstOrDefaultAsync(i => i.CompraId == carritoId && i.ProductoId == productoId);

    if (item == null) return Results.NotFound();

    // Devuelve el stock al producto
    var producto = await db.Productos.FindAsync(productoId);
    if (producto != null)
    {
        producto.Stock += item.Cantidad;
    }

    db.ItemsCompra.Remove(item); // Elimina el ítem
    await db.SaveChangesAsync();
    return Results.Ok();
});

// PUT /carritos/{carritoId}/confirmar - Confirma la compra y asigna datos del cliente
app.MapPut("/carritos/{carritoId}/confirmar", async (int carritoId, ClienteDto cliente, AppDbContext db) =>
{
    // Busca la compra por ID
    var compra = await db.Compras.FindAsync(carritoId);
    if (compra == null) return Results.NotFound();

    // Obtiene los ítems del carrito
    var items = await db.ItemsCompra.Where(i => i.CompraId == carritoId).ToListAsync();
    if (!items.Any()) return Results.BadRequest("Carrito vacío");

    // Valida que el email esté presente
    if (string.IsNullOrWhiteSpace(cliente.Email))
        return Results.BadRequest("Email requerido");

    // Asigna los datos del cliente y el total
    compra.NombreCliente = cliente.Nombre;
    compra.ApellidoCliente = cliente.Apellido;
    compra.EmailCliente = cliente.Email;
    compra.Total = items.Sum(i => i.Cantidad * i.PrecioUnitario);
    await db.SaveChangesAsync();
    // Devuelve solo un mensaje simple para evitar ciclos de serialización
    return Results.Ok(new { mensaje = "Compra confirmada correctamente" });
});

app.Run(); // Inicia la aplicación
