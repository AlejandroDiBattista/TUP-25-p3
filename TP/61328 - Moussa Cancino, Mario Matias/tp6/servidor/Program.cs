using Microsoft.EntityFrameworkCore;
using servidor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");


// =================================================================
// ENDPOINTS DE LA API
// =================================================================

app.MapGet("/api/productos", async (TiendaDbContext db, string? q) => {
    var productosQuery = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        productosQuery = productosQuery.Where(p =>
            p.Nombre.ToLower().Contains(q.ToLower()) ||
            p.Descripcion.ToLower().Contains(q.ToLower())
        );
    }
    var productos = await productosQuery.ToListAsync();
    return Results.Ok(productos);
});

app.MapPost("/api/carritos", async (TiendaDbContext db) => {
    var nuevoCarrito = new Carrito();
    db.Carritos.Add(nuevoCarrito);
    await db.SaveChangesAsync();
    return Results.Created($"/api/carritos/{nuevoCarrito.Id}", new { carritoId = nuevoCarrito.Id });
});

app.MapPut("/api/carritos/{carritoId:guid}/productos/{productoId:int}", async (Guid carritoId, int productoId, TiendaDbContext db) => {
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito is null)
    {
        return Results.NotFound("El carrito no existe.");
    }
    var producto = await db.Productos.FindAsync(productoId);
    if (producto is null)
    {
        return Results.NotFound("El producto no existe.");
    }
    var itemEnCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (itemEnCarrito is not null)
    {
        if (producto.Stock > itemEnCarrito.Cantidad)
        {
            itemEnCarrito.Cantidad++;
        }
        else
        {
            return Results.BadRequest("No hay suficiente stock para agregar otra unidad de este producto.");
        }
    }
    else
    {
        if (producto.Stock > 0)
        {
            var nuevoItem = new CarritoItem
            {
                ProductoId = producto.Id,
                Cantidad = 1
            };
            carrito.Items.Add(nuevoItem);
        }
        else
        {
            return Results.BadRequest("Producto sin stock.");
        }
    }
    await db.SaveChangesAsync();
    return Results.Ok(carrito);
});

// <<< CÓDIGO AÑADIDO INICIA AQUÍ >>>
// Endpoint para eliminar un producto de un carrito
app.MapDelete("/api/carritos/{carritoId:guid}/productos/{productoId:int}", async (Guid carritoId, int productoId, TiendaDbContext db) => {
    // 1. Buscamos el carrito, incluyendo sus items.
    var carrito = await db.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == carritoId);
    if (carrito is null)
    {
        return Results.NotFound("El carrito no existe.");
    }

    // 2. Buscamos el item específico dentro del carrito.
    var itemEnCarrito = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
    if (itemEnCarrito is null)
    {
        // Si el item no está en el carrito, no hay nada que borrar.
        return Results.NotFound("El producto no se encuentra en el carrito.");
    }

    // 3. Aplicamos la lógica de eliminación.
    if (itemEnCarrito.Cantidad > 1)
    {
        // Si hay más de uno, simplemente reducimos la cantidad.
        itemEnCarrito.Cantidad--;
    }
    else
    {
        // Si solo hay uno, eliminamos el item por completo de la base de datos.
        db.CarritoItems.Remove(itemEnCarrito);
    }

    // 4. Guardamos los cambios.
    await db.SaveChangesAsync();

    // 5. Devolvemos el carrito actualizado.
    return Results.Ok(carrito);
});
// <<< CÓDIGO AÑADIDO TERMINA AQUÍ >>>


app.Run();