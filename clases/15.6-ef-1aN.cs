#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using static System.Console;

// Librerias para acceder a EF Core
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

// Librerias para el registro de eventos
using Microsoft.Extensions.Logging;

// Inicializar SQLite PCL
SQLitePCL.Batteries_V2.Init();

static void Log(string mensaje) => 
    File.AppendAllText("historia.log", $"{mensaje}\n\n");

// Definición de las entidades
public class Contacto {
    public int Id { get; set; }             // Clave primaria   (Campo extra)
    public string Nombre { get; set; }      // Campos de la tabla
    public string Apellido { get; set; }    

    // Relación uno a muchos con Entradas
    public ICollection<Entrada> Entradas { get; set; } = new List<Entrada>();
}

public class Entrada {
    public int Id { get; set; }             // Clave primaria   (Campo extra)
    public string Tipo { get; set; }
    public string Valor { get; set; }

    public int ContactoId { get; set; }     // Clave foránea    (Campo extra)
    public Contacto Contacto { get; set; }  // Relación con Contacto
}

public class AgendaContext : DbContext {
    // Dos tablas: Contactos y Entradas
    public DbSet<Contacto> Contactos { get; set; }
    public DbSet<Entrada>  Entradas  { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=agenda.db");
        optionsBuilder.LogTo(Log, LogLevel.Information); // Muestra comandos SQL y eventos en consola
    }

    // protected override void OnModelCreating(ModelBuilder modelBuilder) {
    //     modelBuilder.Entity<Contacto>()
    //         .HasMany(c => c.Entradas)
    //         .WithOne(e => e.Contacto)
    //         .HasForeignKey(e => e.ContactoId)
    //         .OnDelete(DeleteBehavior.Cascade);
    // }
}

// 
using (var db = new AgendaContext()) {
    db.Database.EnsureDeleted(); // Elimina la base de datos si existe
    db.Database.EnsureCreated(); // Crea la base de datos si no existe

    var contactos = new List<Contacto> {
        new Contacto {
            Nombre   = "Juan",
            Apellido = "Pérez",
            Entradas = new List<Entrada> {
                new Entrada { Tipo = "Email",     Valor = "juan.perez@email.com" },
                new Entrada { Tipo = "Teléfono",  Valor = "(123) 456-7890" },
                new Entrada { Tipo = "Dirección", Valor = "Calle Falsa 123" }
            }
        },
        new Contacto {
            Nombre   = "Ana",
            Apellido = "García",
            Entradas = new List<Entrada> {
                new Entrada { Tipo = "Email",     Valor = "ana.garcia@email.com" },
                new Entrada { Tipo = "Teléfono",  Valor = "(987) 654-3210" },
                new Entrada { Tipo = "Dirección", Valor = "Av. Siempre Viva 742" }
            }
        },
        new Contacto {
            Nombre   = "Luis",
            Apellido = "Martínez",
            Entradas = new List<Entrada> {
                new Entrada { Tipo = "Email",     Valor = "luis.martinez@email.com" },
                new Entrada { Tipo = "Teléfono",  Valor = "(555) 123-4560" },
                new Entrada { Tipo = "Dirección", Valor = "Boulevard Central 456" }
            }
        },
        new Contacto {
            Nombre   = "María",
            Apellido = "López",
            Entradas = new List<Entrada> {
                new Entrada { Tipo = "Email",     Valor = "maria.lopez@email.com" },
                new Entrada { Tipo = "Teléfono",  Valor = "(444) 555-6660" },
                new Entrada { Tipo = "Dirección", Valor = "Ruta 8 Km 12" }
            }
        },
        new Contacto {
            Nombre   = "Carlos",
            Apellido = "Sánchez",
            Entradas = new List<Entrada> {
                new Entrada { Tipo = "Email",     Valor = "carlos.sanchez@email.com" },
                new Entrada { Tipo = "Teléfono",  Valor = "(333) 222-1110" },
                new Entrada { Tipo = "Dirección", Valor = "Pasaje Los Robles 99" }
            }
        }
    };

    // Agregar una lista de contactos a la base de datos
    db.Contactos.AddRange(contactos);
    Log("=== Antes de grabar ===");
    db.SaveChanges();
    Log("=== Despues de grabar ===");

    var alejandro = new Contacto { Nombre = "Alejandro", Apellido = "Di Battista" };
    
    var telefono  = new Entrada  { Tipo = "Teléfono", Valor = "(123) 456-7890" };
    var email     = new Entrada  { Tipo = "Email",    Valor = "adibattista@gmail.com"};

    alejandro.Entradas.Add(telefono);
    alejandro.Entradas.Add(email);

    db.Contactos.Add(alejandro);

    Log($"=== Antes de grabar   ID: {alejandro.Id} ===");
    db.SaveChanges();
    Log($"=== Despues de grabar ID: {alejandro.Id} ===");

    // Listar contactos
    WriteLine("\n=== Lista de Contactos ===");

    // El problema es que no estamos usando Include(), así que las entradas no deberían cargarse
    // El código podría estar funcionando por "lazy loading" implícito o porque estamos en un contexto de memoria
    // Cambiemos para hacerlo correctamente con Include()
    var listaContactos = db.Contactos
        .Include(c => c.Entradas)  // Cargar explícitamente las entradas
        .OrderBy(c => c.Apellido)
        .ThenBy(c => c.Nombre);

    foreach(var c in listaContactos) {
        WriteLine($"\n{c.Id,2}) {c.Apellido}, {c.Nombre}");
        foreach(var e in c.Entradas) {
            WriteLine($"   {e.Tipo, -10}: {e.Valor} ({e.Contacto.Entradas.Count})");
        }
    }
}
