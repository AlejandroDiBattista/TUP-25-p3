using Microsoft.EntityFrameworkCore;
using servidor.Modelos;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Conexión con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

// ENDPOINTS PRODUCTOS

app.MapGet("/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync()
);

app.MapGet("/productos/{id}", async (int id, TiendaContext db) =>
    await db.Productos.FindAsync(id) is Producto producto
        ? Results.Ok(producto)
        : Results.NotFound()
);

app.MapPost("/productos", async (Producto producto, TiendaContext db) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/productos/{producto.Id}", producto);
});

app.MapPut("/productos/{id}", async (int id, Producto datosProducto, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    producto.Nombre = datosProducto.Nombre;
    producto.Precio = datosProducto.Precio;
    producto.Stock = datosProducto.Stock;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapDelete("/productos/{id}", async (int id, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    db.Productos.Remove(producto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ENDPOINTS COMPRAS (CARRITO)

app.MapPost("/compras", async (Compra compra, TiendaContext db) =>
{
    // Validaciones y lógica aquí...
    compra.Fecha = DateTime.Now;
    compra.Total = compra.Items.Sum(item =>
    {
        var producto = db.Productos.Find(item.ProductoId);
        return producto != null ? producto.Precio * item.Cantidad : 0;
    });

    // Descontar stock de cada producto
    foreach (var item in compra.Items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
        {
            if (producto.Stock < item.Cantidad)
            {
                return Results.BadRequest($"No hay suficiente stock para el producto {producto.Nombre}");
            }
            producto.Stock -= item.Cantidad;
        }
    }

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    return Results.Created($"/compras/{compra.Id}", new { compra.Id, compra.Total, compra.Fecha });
});

app.MapGet("/compras", async (TiendaContext db) =>
    await db.Compras.Include(c => c.Items).ToListAsync()
);

app.MapGet("/compras/{id}", async (int id, TiendaContext db) =>
    await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id) is Compra compra
        ? Results.Ok(compra)
        : Results.NotFound()
);

app.MapPost("/reservar", async (ReservaRequest reserva, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(reserva.ProductoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    if (producto.Stock < reserva.Cantidad)
        return Results.BadRequest("No hay suficiente stock para reservar");

    producto.Stock -= reserva.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/liberar", async (ReservaRequest reserva, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(reserva.ProductoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    producto.Stock += reserva.Cantidad;
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/", () => "Servidor API está en funcionamiento");


// Iniciar el servidor web en un hilo separado para permitir el menú de administrador

var menuThread = new Thread(() =>
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
        bool salir = false;
        while (!salir)
        {
            Console.WriteLine("\n--- MENÚ ADMINISTRADOR ---");
            Console.WriteLine("1. Listar productos");
            Console.WriteLine("2. Modificar stock de un producto");
            Console.WriteLine("3. Modificar precio de un producto");
            Console.WriteLine("4. Eliminar producto");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    foreach (var p in db.Productos.ToList())
                        Console.WriteLine($"{p.Id}: {p.Nombre} - Stock: {p.Stock} - Precio: {p.Precio}");
                    break;
                case "2":
                    Console.Write("Ingrese el ID del producto: ");
                    if (int.TryParse(Console.ReadLine(), out int idStock))
                    {
                        var prod = db.Productos.FirstOrDefault(p => p.Id == idStock);
                        if (prod != null)
                        {
                            Console.Write($"Stock actual: {prod.Stock}. Nuevo stock: ");
                            if (int.TryParse(Console.ReadLine(), out int nuevoStock))
                            {
                                prod.Stock = nuevoStock;
                                db.SaveChanges();
                                Console.WriteLine("Stock actualizado.");
                            }
                            else
                            {
                                Console.WriteLine("Valor inválido.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Producto no encontrado.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID inválido.");
                    }
                    break;
                case "3":
                    Console.Write("Ingrese el ID del producto: ");
                    if (int.TryParse(Console.ReadLine(), out int idPrecio))
                    {
                        var prod = db.Productos.FirstOrDefault(p => p.Id == idPrecio);
                        if (prod != null)
                        {
                            Console.Write($"Precio actual: {prod.Precio}. Nuevo precio: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal nuevoPrecio))
                            {
                                prod.Precio = nuevoPrecio;
                                db.SaveChanges();
                                Console.WriteLine("Precio actualizado.");
                            }
                            else
                            {
                                Console.WriteLine("Valor inválido.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Producto no encontrado.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID inválido.");
                    }
                    break;
                case "4":
                    Console.Write("Ingrese el ID del producto a eliminar: ");
                    if (int.TryParse(Console.ReadLine(), out int idEliminar))
                    {
                        var prod = db.Productos.FirstOrDefault(p => p.Id == idEliminar);
                        if (prod != null)
                        {
                            db.Productos.Remove(prod);
                            db.SaveChanges();
                            Console.WriteLine("Producto eliminado.");
                        }
                        else
                        {
                            Console.WriteLine("Producto no encontrado.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID inválido.");
                    }
                    break;
                case "5":
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }
});

menuThread.Start();

app.Run(); // El servidor web inicia y queda escuchando en localhost

public class ReservaRequest
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}

