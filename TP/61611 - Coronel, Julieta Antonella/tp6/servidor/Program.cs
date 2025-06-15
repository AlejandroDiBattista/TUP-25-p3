using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using servidor.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services
    .AddDbContext<ContactosDb>(opt => opt.UseSqlite("Data Source=tienda.db"));

var app = builder.Build();




// 🔁 Reiniciar stock al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContactosDb>();

    var productos = db.Productos.ToList();

    foreach (var producto in productos)
    {
        producto.Stock = 25;
    }

    db.SaveChanges();
}

// Habilitar CORS
app.UseCors();

var carritos = new Dictionary<string, List<CarritoDto>>();

app.MapGet("api/carritos", () =>
{
    string carritoId = Guid.NewGuid().ToString();
    carritos[carritoId] = new List<CarritoDto>();
    return Results.Ok(carritoId);
});

app.MapGet("api/carritos/{carritoId}", (string carritoId) =>
{
    if (!carritos.ContainsKey(carritoId))
        return Results.NotFound("No encontramos un carrito con los datos enviados.");

    return Results.Ok(carritos[carritoId]);
});

app.MapDelete("api/carritos/{carritoId}", (string carritoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.NotFound("No se encontró el carrito a eliminar.");

    carritos.Remove(carritoId);
    return Results.Ok("El carrito fue eliminado con éxito.");
});

app.MapGet("api/carritos/{carritoId}/{productoId}", async (ContactosDb db, string carritoId, int productoId) =>
{
    if (!ValidarCarritoId(carritoId))
        carritos[carritoId] = new List<CarritoDto>();

    var producto = await db.Productos.FirstOrDefaultAsync(x => x.Id == productoId);
    if (producto == null)
        return Results.NotFound("Producto no encontrado.");

    // 🔐 Verificamos stock antes de agregar
    var cantidadActual = carritos[carritoId]
        .Where(x => x.ProductoId == productoId)
        .Select(x => x.Cantidad)
        .FirstOrDefault();

    if (cantidadActual >= producto.Stock)
        return Results.BadRequest($"No se puede agregar más del stock disponible ({producto.Stock}).");

    if (cantidadActual > 0)
    {
        carritos[carritoId].First(x => x.ProductoId == productoId).Cantidad++;
    }
    else
    {
        carritos[carritoId].Add(new CarritoDto
        {
            ProductoId = productoId,
            Cantidad = 1,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio
        });
    }

    return Results.Ok("Producto agregado al carrito.");
});

app.MapDelete("api/carritos/{carritoId}/eliminar/{productoId}", (string carritoId, int productoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.NotFound("Carrito no encontrado");

    var item = carritos[carritoId].FirstOrDefault(x => x.ProductoId == productoId);
    if (item == null)
        return Results.NotFound("Producto no encontrado en el carrito");

    carritos[carritoId].Remove(item);
    return Results.Ok("Producto eliminado del carrito");
});


app.MapDelete("api/carritos/{carritoId}/vaciar", (string carritoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.NotFound("Carrito no encontrado");

    carritos[carritoId].Clear();
    return Results.Ok("Carrito vaciado con éxito");
});

app.MapGet("/productos/{carritoId}", (string carritoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.NotFound("No se encontró el carrito.");

    IList<CarritoDto> carritoDtos = carritos[carritoId];
    return Results.Ok(carritoDtos);
});

// Endpoint para ver todos los productos
// GET /productos (+ búsqueda por query).
app.MapGet("api/productos", async (ContactosDb db, string parametroBusqueda = "") =>
{
    var test = carritos;
    if (!string.IsNullOrEmpty(parametroBusqueda))
    {
        return Results.Ok(await db.Productos.Where(x => x.Nombre.ToLower().Contains(parametroBusqueda.ToLower()) ||
        x.Descripcion.ToLower().Contains(parametroBusqueda.ToLower()))
        .ToListAsync());
    }
    else
    {
        return Results.Ok(await db.Productos.ToListAsync());
    }
});

// POST Compras
app.MapPost("api/nuevaCompra", async (ContactosDb db, NuevaCompraDto nuevaCompraDto) =>
{
    if (!ValidarCarritoId(nuevaCompraDto.CarritoId))
        return Results.NotFound("Seleccione productos para su carrito.");

    IList<CarritoDto> items = carritos[nuevaCompraDto.CarritoId];
    if (items != null && items.Count > 0)
    {
        db.Compras.Add(new Compras
        {
            Fecha = DateTime.Now.ToString("dd/MM/yyyy"),
            NombreCliente = nuevaCompraDto.Nombre,
            ApellidoCliente = nuevaCompraDto.Apellido,
            EmailCliente = nuevaCompraDto.Email,
            Total = items.Sum(x => x.Precio)
        });

        int idCompra = await db.SaveChangesAsync();

        foreach (CarritoDto carritoDto in items)
            {
                var producto = await db.Productos.FindAsync(carritoDto.ProductoId);
                if (producto == null)
                    continue;

                if (producto.Stock < carritoDto.Cantidad)
                    return Results.BadRequest($"Stock insuficiente para el producto {producto.Nombre}");

                producto.Stock -= carritoDto.Cantidad; // 🔴 Acá se descuenta el stock

                db.ItemsCompra.Add(new ItemsCompra
                {
                    ProductoId = carritoDto.ProductoId,
                    CompraId = idCompra,
                    Cantidad = carritoDto.Cantidad,
                    PrecioUnitario = carritoDto.Precio
                });
            }

        await db.SaveChangesAsync();
        carritos[nuevaCompraDto.CarritoId].Clear();
        return Results.Ok("La compra fue grabada con éxito.");
    }

    return Results.NotFound("Su carrito no tiene productos.");
    // db.Compras.Add(new Compras
    // {
    //     Fecha = DateTime.Now.ToString("dd/MM/yyyy"),
    //     NombreCliente = nuevaCompraDto.Nombre,
    //     ApellidoCliente = nuevaCompraDto.Apellido,
    //     EmailCliente = nuevaCompraDto.Email
    // });

    // int idCompra = await db.SaveChangesAsync();

    // foreach (CarritoDto carritoDto in nuevaCompraDto.CarritoDtos)
    // {
    //     db.ItemsCompras.Add(new ItemsCompra
    //     {
    //         ProductoId = carritoDto.ProductoId,
    //         CompraId = idCompra,
    //         Cantidad = carritoDto.Cantidad,
    //         PrecioUnitario = carritoDto.Precio
    //     });
    // }

    // return Results.Ok("La compra fue grabada con éxito.");
});

app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

app.MapPost("api/nuevoProducto", async (ContactosDb db, [FromBody] Productos producto) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Ok("Producto agregado con éxito.");
});

bool ValidarCarritoId(string carritoId)
{
    return carritos.ContainsKey(carritoId);
}



app.MapPut("api/carritos/{carritoId}/disminuir/{productoId}", (string carritoId, int productoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.NotFound("Carrito no encontrado");

    var item = carritos[carritoId].FirstOrDefault(x => x.ProductoId == productoId);
    if (item == null)
        return Results.NotFound("Producto no encontrado en el carrito");

    item.Cantidad--;

    if (item.Cantidad <= 0)
        carritos[carritoId].Remove(item);

    return Results.Ok("Producto disminuido correctamente");
});

// Endpoint para obtener todos los contactos
// app.MapGet("/contactos", async (ContactosDb db) => await db.Contactos.ToListAsync());

// // Endpoint para obtener un contacto por id
// app.MapGet("/contactos/{id}", async (ContactosDb db, int id) =>
// {
//     var contacto = await db.Contactos.FindAsync(id);
//     return contacto is not null ? Results.Ok(contacto) : Results.NotFound();
// });

// // Endpoint para crear un nuevo contacto
// app.MapPost("/contactos", async (ContactosDb db, Contacto contacto) =>
// {
//     db.Contactos.Add(contacto);
//     await db.SaveChangesAsync();
//     return Results.Created($"/contactos/{contacto.Id}", contacto);
// });


// // Endpoint para eliminar un contacto
// app.MapDelete("/contactos/{id}", async (ContactosDb db, int id) =>
// {
//     var contacto = await db.Contactos.FindAsync(id);
//     if (contacto is null) return Results.NotFound();
//     db.Contactos.Remove(contacto);
//     await db.SaveChangesAsync();
//     return Results.NoContent();
// });

// // Inicialización de la base de datos y datos de ejemplo
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<ContactosDb>();
//     db.Database.EnsureCreated();
//     if (!db.Contactos.Any())
//     {
//         db.Contactos.AddRange(
//             new Contacto { Nombre = "Juan", Apellido = "Pérez", Telefono = "123456", Email = "juan@mail.com" },
//             new Contacto { Nombre = "Ana", Apellido = "García", Telefono = "654321", Email = "ana@mail.com" }
//         );
//         db.SaveChanges();
//     }
// }



app.MapGet("api/carritos/{carritoId}/detalle", (string carritoId) =>
{
    if (!ValidarCarritoId(carritoId))
        return Results.BadRequest("El carrito no existe.");

    var detalle = carritos[carritoId];
    return Results.Ok(detalle);
});

app.MapGet("/", () => "Hello World!");

app.Run();

public class Contacto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Email { get; set; } = null!;
}
public class ContactosDb : DbContext
{
    public ContactosDb(DbContextOptions<ContactosDb> options) : base(options) { }
    public DbSet<Productos> Productos { get; set; }
    public DbSet<Compras> Compras { get; set; }
    public DbSet<ItemsCompra> ItemsCompra { get; set; }
}