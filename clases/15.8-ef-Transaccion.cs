#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using static System.Console;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

using Microsoft.Extensions.Logging;

SQLitePCL.Batteries_V2.Init();

static void Log(string mensaje) => 
    File.AppendAllText("historia.log", $"{mensaje}\n\n");

// Definición de las entidades (n:m)
public class Producto {
    public int Id { get; set; }
    public string Descripcion { get; set; }
    public double Precio { get; set; }
    public int Stock { get; set; }
}

public class AlmacenContext : DbContext {
    public DbSet<Producto> Productos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=agenda.db");
        optionsBuilder.LogTo(Log, LogLevel.Information); // Muestra comandos SQL y eventos en consola
    }

}

using (var db = new AlmacenContext()) {
    db.Database.EnsureDeleted(); // Elimina la base de datos si existe
    db.Database.EnsureCreated(); // Crea la base de datos si no existe

    if(db.Productos.Count() == 0) {
        db.Productos.Add(new Producto { Descripcion = "Producto 1", Precio = 10.0, Stock = 100 });
        db.Productos.Add(new Producto { Descripcion = "Producto 2", Precio = 20.0, Stock = 200 });
        db.SaveChanges();
    }

    // Listar productos
    foreach (var producto in db.Productos) {   
        Console.WriteLine($"{producto.Id} {producto.Descripcion} {producto.Precio} {producto.Stock}");
    }       

    using var transaccion = db.Database.BeginTransaction();
    try {
        // Insertar un nuevo contacto
        db.Productos.Add(new Producto { Descripcion = "Producto 3", Precio = 30.0, Stock = 300 });
        db.SaveChanges();

        // Actualizar un contacto
        var producto = db.Productos.First();
        producto.Stock += 10;
        db.SaveChanges();
        throw new Exception("Error de prueba"); // Simular un error
        transaccion.Commit();
    } catch (Exception) {
        transaccion.Rollback();
        Console.WriteLine("Error en la transacción");
        // Manejar el error
    }

    // Listar productos después de la transacción
    foreach (var producto in db.Productos) {   
        Console.WriteLine($"{producto.Id} {producto.Descripcion} {producto.Precio} {producto.Stock}");
    }

}