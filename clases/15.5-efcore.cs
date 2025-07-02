#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using static System.Console;

// Librerias para acceder a EF Core
using Microsoft.EntityFrameworkCore;        // Libreria principal de EF Core
using Microsoft.EntityFrameworkCore.Sqlite; // Libreria para SQLite

using Microsoft.Extensions.Logging;         // Libreria para el registro de eventos

SQLitePCL.Batteries_V2.Init(); // Inicializar SQLite PCL

static void Log(string mensaje) => 
    File.AppendAllText("historia.log", $"{mensaje}\n\n");

// Definición de las entidades
public class Contacto {
    public int Id { get; set; }            // Clave primaria
    public string Nombre { get; set; }      // Campos de la tabla
    public string Apellido { get; set; }
    public string Telefono { get; set; } 

    public Contacto(string nombre, string apellido, string telefono) {
        Nombre = nombre;
        Apellido = apellido;
        Telefono = telefono;
    }

    public string NombreCompleto =>         // Propiedad calculada
        $"{Apellido}, {Nombre}";
    public override string ToString() =>    // Método para mostrar el contacto
        $"{Id,2} {NombreCompleto, -30}  {Telefono}";
}

// DbContext : Acceso a la base de datos
// DbSet<T> : Representa una tabla en la base de datos
// OnConfiguring : Configuración del contexto de la base de datos
// UseSqlite : Indica que se usará SQLite como proveedor de base de datos
public class ContactosContext : DbContext {
    public DbSet<Contacto> Contactos { get; set; } // Tabla de `contactos`

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        // Configuración de la base de datos
        optionsBuilder.UseSqlite("Data Source=contactos.db"); 
        // LogTo : Muestra los comandos SQL y eventos en el Logging
        optionsBuilder.LogTo(Log, LogLevel.Information); 
    }
}

using (var db = new ContactosContext()) {

    Log("Iniciando la aplicación...");
    // Elimina los datos (solo para pruebas)
    db.Database.EnsureDeleted(); 
    // Crea la base de datos si no existe
    db.Database.EnsureCreated(); 
    Log("Base de datos creada.");
    
    Log("Añadiendo contactos...");
    db.Contactos.Add(new Contacto("Carlos", "Pérez", "(555)123-4567"));
    db.Contactos.Add(new Contacto("María",  "Gómez", "(555)987-6543"));
    db.Contactos.Add(new Contacto("Juan",   "López", "(381)123-7654"));
    Log("Contactos añadidos.(sin guardar)");
    
    // Guardar los cambios en la base de datos
    db.SaveChanges(); 
    Log("Cambios guardados.");
    
    // Listar contactos
    Log("Creamos una consulta para listar contactos.");
    var alfabetico = db.Contactos
                        .OrderBy(c => c.Apellido)
                        .ThenBy(c => c.Nombre);
    WriteLine("\n=== Lista de Contactos ===");
    WriteLine("ID Nombre Completo                 Telefono");
    Log("Antes del foreach"); 
    foreach (var c in alfabetico) {
        WriteLine(c);
    }
    Log("Lista de contactos mostrada.");

    // LINQ como expresión:
    // var contactos = from c in db.Contactos
    //                 where c.Telefono.StartsWith("(555)")
    //                 orderby c.Apellido, c.Nombre
    //                 select c;

    Log("Contactos filtrados por telefono (555).(sin ejecutar)");
    var contactos555 = db.Contactos
            .Where(c => c.Telefono.StartsWith("(555)"))
            .OrderBy(c => c.Apellido)
            .ThenBy(c => c.Nombre);

    WriteLine("\n=== Contactos con Telefono (555) ===");
    WriteLine("ID Nombre Completo                 Telefono");
    Log("Antes del foreach en (555)");
    foreach (var c in contactos555) {
        WriteLine(c);
    }
    Log("Contactos filtrados mostrados.");

    Log("Contactos (Nombre y Apellido) (sin ejecutar)");
    var nombres = db.Contactos
                        .Select(c => new { c.Nombre, c.Apellido })
                        .Where(c => c.Nombre.StartsWith("C"))
                        .OrderBy(c => c.Apellido);

    WriteLine("\n=== Contactos (Nombre y Apellido) ===");
    foreach(var c in nombres) {
        WriteLine($"- {c.Apellido}, {c.Nombre}");
    }
    Log("Contactos (Nombre y Apellido) mostrados.");
}
