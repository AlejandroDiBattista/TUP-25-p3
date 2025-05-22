#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

// ATENCION -> Correr con `dotnet script 19.1-ApiRest.csx --restore`

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore; 

// Crea un constructor de aplicaciones web (WebApplication) usando los argumentos de línea de comandos.
// Este objeto builder se utiliza para configurar servicios y la aplicación antes de ejecutarla.
var builder = WebApplication.CreateBuilder();

// Configura la cadena de conexión para la base de datos SQLite.
var connectionString = "Data Source=productos.db";

// Registra el contexto de base de datos `AlmacenDb` en el contenedor de servicios de la aplicación.
// Esto permite que el contexto se inyecte automáticamente donde se necesite.
// Se configura para usar SQLite como proveedor de base de datos, utilizando la cadena de conexión definida antes.
builder.Services.AddDbContext<AlmacenDb>(options => {
    options.UseSqlite(connectionString);
});

// Construye la aplicación web a partir de la configuración realizada en el builder.
// El objeto app representa la aplicación web lista para definir rutas y ejecutarse.
var app = builder.Build();

// ========================
// Definición de Endpoints
// ========================

// Endpoint HTTP GET para obtener todos los productos.
// Ruta: /productos
// Utiliza el contexto de base de datos para consultar todos los productos y devolverlos en la respuesta.
// La base de datos se 'injecta' automáticamente porque esta registrada como 'servicio'
app.MapGet("/productos", async (AlmacenDb db) => { 
    // Obtiene de forma asíncrona la lista de todos los productos almacenados en la base de datos.
    var productos = await db.Productos.ToListAsync();

    // Retorna una respuesta HTTP 200 OK con la lista de productos en formato JSON.
    return Results.Ok(productos);
});

// Endpoint HTTP GET para obtener un producto específico por su ID.
// Ruta: /productos/{id}
// Recibe el ID del producto como parámetro de la ruta y el contexto de base de datos.
app.MapGet("/productos/{id}", async (int id, AlmacenDb db) => { 
    // Busca de forma asíncrona el producto con el ID especificado en la base de datos.
    var producto = await db.Productos.FindAsync(id);

    // Si no se encuentra el producto, retorna una respuesta 404 Not Found con un mensaje personalizado.
    if (producto == null) {
        return Results.NotFound($"Producto con ID {id} no encontrado");
    } else {
        // Si el producto existe, retorna una respuesta 200 OK con el producto encontrado.
        return Results.Ok(producto);
    }
});

// Endpoint HTTP POST para crear un nuevo producto.
// Ruta: /productos
// Recibe un objeto Producto en el cuerpo de la solicitud y el contexto de base de datos.
app.MapPost("/productos", async (Producto producto, AlmacenDb db) => { 
    // Agrega el nuevo producto al contexto de la base de datos (aún no se guarda en la base de datos física).
    db.Productos.Add(producto);

    // Guarda los cambios de forma asíncrona en la base de datos, persistiendo el nuevo producto.
    await db.SaveChangesAsync();

    // Retorna una respuesta 201 Created con la ubicación del nuevo producto y el objeto creado.
    return Results.Created($"/productos/{producto.Id}", producto);
});

// Endpoint HTTP PUT para actualizar un producto existente.
// Ruta: /productos/{id}
// Recibe el ID del producto a actualizar, el objeto Producto con los nuevos datos y el contexto de base de datos.
app.MapPut("/productos/{id}", async (int id, Producto productoActualizado, AlmacenDb db) => { 
    // Busca de forma asíncrona el producto con el ID especificado en la base de datos.
    var producto = await db.Productos.FindAsync(id);

    // Si no se encuentra el producto, retorna una respuesta 404 Not Found con un mensaje personalizado.
    if (producto == null) {
        return Results.NotFound($"Producto con ID {id} no encontrado");
    } else {
        // Si el producto existe, actualiza sus propiedades con los valores recibidos.
        producto.Descripcion = productoActualizado.Descripcion;
        producto.Precio = productoActualizado.Precio;
        producto.Cantidad = productoActualizado.Cantidad;

        // Guarda los cambios de forma asíncrona en la base de datos.
        await db.SaveChangesAsync();

        // Retorna una respuesta 200 OK con el producto actualizado.
        return Results.Ok(producto);
    }
});

// Endpoint HTTP DELETE para eliminar un producto existente.
// Ruta: /productos/{id}
// Recibe el ID del producto a eliminar y el contexto de base de datos.
app.MapDelete("/productos/{id}", async (int id, AlmacenDb db) => { 
    // Busca de forma asíncrona el producto con el ID especificado en la base de datos.
    var producto = await db.Productos.FindAsync(id);

    // Si no se encuentra el producto, retorna una respuesta 404 Not Found con un mensaje personalizado.
    if (producto == null) {
        return Results.NotFound($"Producto con ID {id} no encontrado");
    } else {
        // Si el producto existe, lo marca para ser eliminado de la base de datos.
        db.Productos.Remove(producto);

        // Guarda los cambios de forma asíncrona en la base de datos, eliminando el producto.
        await db.SaveChangesAsync();

        // Retorna una respuesta 200 OK con un mensaje de éxito.
        return Results.Ok($"Producto con ID {id} eliminado correctamente");
    }
});

// ==================================
// Inicialización de la Base de Datos
// ==================================

// Crea un nuevo ámbito de servicios para obtener instancias de servicios registrados en el contenedor de dependencias.
// Esto es necesario para ejecutar código que requiere servicios fuera del ciclo de vida de una solicitud HTTP.
using (var scope = app.Services.CreateScope()) {
    // Obtiene una instancia del contexto de base de datos AppDbContext desde el proveedor de servicios del ámbito.
    var dbContext = scope.ServiceProvider.GetRequiredService<AlmacenDb>();
    // Se asegura que la base de datos esté creada; si no existe, la crea automáticamente.
    dbContext.Database.EnsureCreated();
}

// Inicia la aplicación web y comienza a escuchar solicitudes HTTP en el puerto configurado.
app.Run();

/// Entity Framework Core ///

// Definición de la entidad Producto
public class Producto {
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
}

// Definición del contexto de base de datos AlmacenDb
public class AlmacenDb : DbContext {
    public DbSet<Producto> Productos { get; set; }
    public AlmacenDb(DbContextOptions<AlmacenDb> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        // Datos semilla para pruebas
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Descripcion = "Producto 1", Precio = 100.50m, Cantidad = 10 },
            new Producto { Id = 2, Descripcion = "Producto 2", Precio = 200.75m, Cantidad = 20 },
            new Producto { Id = 3, Descripcion = "Producto 3", Precio = 300.99m, Cantidad = 30 }
        );
    }
}
