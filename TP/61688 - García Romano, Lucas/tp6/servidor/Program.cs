using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));


builder.Services.AddControllers();

var carritos = new List<Carrito>();
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//permite que el cliente pueda acceder a la API
app.UseCors("AllowClientApp");

//Es para que la api reciva peteciones
app.MapGet("/", () => "Servidor API está en funcionamiento");

//Endpoint de prueba que devuelve un mensaje y la fecha actual
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

//Endpoint devuelve los productos desde la base de datos
app.MapGet("/api/producto", async (TiendaContext db, string buscar) =>
{
    // Busca productos por nombre, descripción, marca o precio
    if (string.IsNullOrEmpty(buscar))
        return Results.Ok(await db.Productos.ToListAsync());

    var filtrados = await db.Productos
        .Where(p =>
            p.Nombre.Contains(buscar) ||
            p.Descripcion.Contains(buscar) ||
            p.Marca.Contains(buscar) ||
            p.Precio.ToString().Contains(buscar)
        ).ToListAsync();

    return Results.Ok(filtrados);
});

//endpoint para inicializar el carrito
app.MapPost("/carritos", () =>
{
    var carrito = new Carrito();
    carritos.Add(carrito);
    return Results.Ok(new { carrito.Id });
});
//llama al Carrito por el ID y devuelve los detalles
app.MapGet("/carritos/{id}", (Guid id) =>
{
    var carrito = carritos.FirstOrDefault(c => c.Id == id);

    if (carrito == null)
        return Results.NotFound(new { Mensaje = "Carrito no encontrado" });
//Busca al carrito Con el ID y si es null da error


    return Results.Ok(carrito.Items);
});

// Vacía el el carrito con el ID 
app.MapGet("/carritos/vaciar/{id}", (Guid id) =>
{
    var carrito = carritos.FirstOrDefault(c => c.Id == id);
    //Busca el carrito en base al ID


    if (carrito == null)
        //Si en null, devuelve error
        return Results.NotFound(new { Mensaje = "No se encontró" });

    carrito.Items.Clear();
    //Vacia el carrito
    return Results.Ok(new { Mensaje = "Carrito Vaciado" });
    //Da un msj que el carrito fue vaciado
});

// Confirma el pedido y realiza la compra
app.MapPut("/carritos/{id}/finalizar", async (Guid id, ClienteDTO infoCliente, TiendaContext db) =>
{
    var carrito = carritos.FirstOrDefault(c => c.Id == id);

    if (carrito == null)
        return Results.NotFound(new { Mensaje = "No se encontró el carrito" });

    if (!carrito.Items.Any())
        return Results.BadRequest(new { Mensaje = "El carrito está vacío, no se puede finalizar la compra" });

    // Validar stock de cada producto
    foreach (var item in carrito.Items)
    {
        var prod = await db.Productos.FindAsync(item.ProductoId);
        if (prod == null || prod.Stock < item.Cantidad)
            return Results.BadRequest(new { Mensaje = $"No hay stock suficiente para {item.Nombre}" });
    }

    // genera la orden de compra y la guarda el detalle
    var nuevaCompra = new Compra
    {
        NombreCliente = infoCliente.Nombre,
        ApellidoCliente = infoCliente.Apellido,
        EmailCliente = infoCliente.Email,
        Total = carrito.Items.Sum(i => i.PrecioUnitario * i.Cantidad),
        Items = carrito.Items.Select(i => new ItemCompra
        {
            ProductoId = i.ProductoId,
            Cantidad = i.Cantidad,
            PrecioUnitario = i.PrecioUnitario
        }).ToList()
    };

    // Actualizar stock
    foreach (var item in carrito.Items)
    {
        //busca el producto por ID
        var prod = await db.Productos.FindAsync(item.ProductoId);
        //si no es null, lo resta del stock
        if (prod != null)
        {
            prod.Stock -= item.Cantidad;
        }
    }
// Guarda los cambios en la base de datos
    db.Compras.Add(nuevaCompra);
    await db.SaveChangesAsync();
//Limpia el carrito al finalizar la compra
    carrito.Items.Clear();
// Devuelve la respuesta con los detalles de la compra
    return Results.Ok(new
    {
        Mensaje = "Compra realizada, Quiere seguir comprando?",
        CompraId = nuevaCompra.Id,
        nuevaCompra.Fecha,
        nuevaCompra.Total,
        Cliente = new { infoCliente.Nombre, infoCliente.Apellido, infoCliente.Email }
    });
});

// Agrega un producto al carrito
app.MapPut("/carritos/{carritoId}/{productoId}", async (
    Guid carritoId,
    string productoId,
    AgregarProductoDTO body,
    TiendaContext db
) =>
{//Busca al carrito por el Id
    var carrito = carritos.FirstOrDefault(c => c.Id == carritoId);
    if (carrito == null)
        return Results.NotFound(new { Mensaje = "Carrito no encontrado" });
    //Lanza un error si no se encuentra
    var producto = await db.Productos.FindAsync(productoId);
    if (producto == null)
        return Results.NotFound(new { Mensaje = "Producto no encontrado" });
    //manda un msj diciendo que el stock es insuficiente
    if (producto.Stock < body.Cantidad)
        return Results.BadRequest(new { Mensaje = "Stock insuficiente" });
    //Busca producto por la Id y si no lo encuentra devuelve error
    if (!int.TryParse(productoId, out int productoIdInt))
        return Results.BadRequest(new { Mensaje = "El id del producto no es válido" });
    var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoIdInt);
    if (itemExistente != null)
    {
        // Cantidad total al actualizar la base de datos
        //y si el producto ya existe en el carrito, actualiza la cantidad
        var total = itemExistente.Cantidad + body.Cantidad;
        if (producto.Stock < total)
            return Results.BadRequest(new { Mensaje = "Stock insuficiente al actualizar cantidad" });

        itemExistente.Cantidad = total;
    }
    else
    {
        //Agrega el producto al carrito
        carrito.Items.Add(new ItemCarrito
        {
            ProductoId = producto.Id,
            Nombre = producto.Nombre,
            Cantidad = body.Cantidad,
            PrecioUnitario = producto.Precio
        });
    }
    //lanza el msj que el carrito fue actualizado
    return Results.Ok(new { Mensaje = "Producto agregado/actualizado al carrito" });
});

//Elimir un producto del carrito
app.MapDelete("/carritos/{carritoId}/{productoId}", (
    Guid carritoId,
    string productoId,
    int? cantidad
) =>
{ //Busca el carrito por el ID
    var carrito = carritos.FirstOrDefault(c => c.Id == carritoId);
    if (carrito == null)
        return Results.NotFound(new { Mensaje = "Carrito no encontrado" });
    //Lanza un error si no se encuentra
    if (!int.TryParse(productoId, out int productoIdInt))
        return Results.BadRequest(new { Mensaje = "El id del producto no es válido" });
    //Busca el producto por la Id
    var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoIdInt);
    if (item == null)
        return Results.NotFound(new { Mensaje = "Producto no está en el carrito" });
    //Si no se encuentra el producto, devuelve error
    if (cantidad == null || cantidad >= item.Cantidad)
    {
        // Quitar completamente
        carrito.Items.Remove(item);
        return Results.Ok(new { Mensaje = "Producto eliminado del carrito" });
    }
    else
    {
        // Reducir cantidad
        item.Cantidad -= cantidad.Value;
        return Results.Ok(new { Mensaje = $"Cantidad reducida. Quedan {item.Cantidad}" });
    }
});


app.Run();