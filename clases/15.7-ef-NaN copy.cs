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
public class Contacto {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }

    public ICollection<Entrada> Entradas { get; set; } = new List<Entrada>();
}

public class Entrada {
    public int Id { get; set; }
    public string Valor { get; set; }

    public int ContactoId { get; set; }
    public Contacto Contacto { get; set; }
    
    public int TipoId { get; set; }
    public Tipo Tipo { get; set; }
}

public class Tipo {
    public int Id { get; set; }
    public string Nombre { get; set; }

    public ICollection<Entrada> Entradas { get; set; } = new List<Entrada>();
}

public class AgendaContext : DbContext {
    public DbSet<Contacto> Contactos { get; set; }
    public DbSet<Entrada> Entradas { get; set; }
    public DbSet<Tipo> Tipos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=agenda.db");
        optionsBuilder.LogTo(Log, LogLevel.Information); // Muestra comandos SQL y eventos en consola
    }

    // protected override void OnModelCreating(ModelBuilder modelBuilder) {
    //     // Relación Entrada -> Contacto (muchos a uno)
    //     modelBuilder.Entity<Entrada>()
    //         .HasOne(e => e.Contacto)
    //         .WithMany(c => c.Entradas)
    //         .HasForeignKey(e => e.ContactoId)
    //         .OnDelete(DeleteBehavior.Cascade);

    //     // Relación Entrada -> Tipo (muchos a uno)
    //     modelBuilder.Entity<Entrada>()
    //         .HasOne(e => e.Tipo)
    //         .WithMany(t => t.Entradas)
    //         .HasForeignKey(e => e.TipoId)
    //         .OnDelete(DeleteBehavior.Cascade);

    //     // Relación Contacto -> Entrada (uno a muchos)
    //     modelBuilder.Entity<Contacto>()
    //         .HasMany(c => c.Entradas)
    //         .WithOne(e => e.Contacto)
    //         .HasForeignKey(e => e.ContactoId)
    //         .OnDelete(DeleteBehavior.Cascade);

    //     // Relación Tipo -> Entrada (uno a muchos)
    //     modelBuilder.Entity<Tipo>()
    //         .HasMany(t => t.Entradas)
    //         .WithOne(e => e.Tipo)
    //         .HasForeignKey(e => e.TipoId)
    //         .OnDelete(DeleteBehavior.Cascade);
    // }
}

using (var db = new AgendaContext()) {
    db.Database.EnsureDeleted(); // Elimina la base de datos si existe
    db.Database.EnsureCreated(); // Crea la base de datos si no existe

    // Crear los tipos de entrada
    var tipoEmail     = new Tipo { Nombre = "Email" };
    var tipoTelefono  = new Tipo { Nombre = "Teléfono" };
    var tipoDireccion = new Tipo { Nombre = "Dirección" };
    
    db.Tipos.AddRange(tipoEmail, tipoTelefono, tipoDireccion);
    db.SaveChanges();

    var contactos = new List<Contacto> {
        new Contacto {
            Nombre = "Juan",
            Apellido = "Pérez",
            Entradas = new List<Entrada> {
                new Entrada { Tipo = tipoEmail,     Valor = "juan.perez@email.com" },
                new Entrada { Tipo = tipoTelefono,  Valor = "(123) 456-789" },
                new Entrada { Tipo = tipoDireccion, Valor = "Calle Falsa 123" }
            }
        },
        new Contacto {
            Nombre = "Ana",
            Apellido = "García",
            Entradas = new List<Entrada> {
                new Entrada { Tipo = tipoTelefono,  Valor = "(987) 654-321" },
                new Entrada { Tipo = tipoDireccion, Valor = "Av. Siempre Viva 742" }
            }
        },
        new Contacto {
            Nombre = "Luis",
            Apellido = "Martínez",
            Entradas = new List<Entrada> {
                new Entrada { Tipo = tipoEmail,     Valor = "luis.martinez@email.com" },
                new Entrada { Tipo = tipoDireccion, Valor = "Boulevard Central 456" }
            }
        },
        new Contacto {
            Nombre = "María",
            Apellido = "López",
            Entradas = new List<Entrada> {
                new Entrada { Tipo = tipoEmail,     Valor = "maria.lopez@email.com" },
                new Entrada { Tipo = tipoTelefono,  Valor = "(444) 555-666" },
            }
        },
        new Contacto {
            Nombre = "Carlos",
            Apellido = "Sánchez",
            Entradas = new List<Entrada> {
                new Entrada { Tipo = tipoEmail,     Valor = "carlos.sanchez@email.com" },
                new Entrada { Tipo = tipoTelefono,  Valor = "(333) 222-111" },
                new Entrada { Tipo = tipoDireccion, Valor = "Pasaje Los Robles 99" }
            }
        }
    };

    db.Contactos.AddRange(contactos);
    db.SaveChanges();

    var ale   = new Contacto { Nombre = "Alejandro", Apellido = "Di Battista" };
    var tel   = new Entrada  { Tipo = tipoTelefono, Valor = "(123) 456-7890" };
    var email = new Entrada  { Tipo = tipoEmail,    Valor = "adibattista@gmail.com"};

    db.Contactos.Add(ale);
    ale.Entradas.Add(tel);
    ale.Entradas.Add(email);
    db.SaveChanges();

    // Listar contactos
    WriteLine("\n=== Lista de Contactos ===");
    foreach(var c in db.Contactos.Include(c => c.Entradas).ThenInclude(e => e.Tipo).OrderBy(c => c.Apellido).ThenBy(c => c.Nombre)) {
        WriteLine($"\n{c.Id,2} {c.Apellido}, {c.Nombre}");
        foreach(var e in c.Entradas) {
            WriteLine($"  - {e.Tipo.Nombre}: {e.Valor}");
        }
    }

    WriteLine("\n=== Contactos con Teléfono ===");
    var contactosConTelefono = db.Contactos
        .Include(c => c.Entradas)
            .ThenInclude(e => e.Tipo)
        .Where(c => c.Entradas.Any(e => e.Tipo.Nombre == "Teléfono"))
        .OrderBy(c => c.Apellido)
        .ThenBy(c => c.Nombre)
        .ToList();

    foreach (var c in contactosConTelefono) {
        WriteLine($"\n{c.Id,2} {c.Apellido}, {c.Nombre}");
        foreach (var e in c.Entradas.Where(e => e.Tipo.Nombre == "Teléfono")) {
            WriteLine($"  - Teléfono: {e.Valor}");
        }
    }

    // Listarme todos los contacto para cada tipo de entrada
    WriteLine("\n=== Contactos por Tipo de Entrada ===");
    var contactosPorTipo = db.Tipos
        .Include(t => t.Entradas)
            .ThenInclude(e => e.Contacto)
        .OrderBy(t => t.Nombre)
        .ToList();

    foreach (var t in contactosPorTipo) {
        WriteLine($"\nTipo: {t.Nombre}");
        foreach (var e in t.Entradas) {
            WriteLine($"  - {e.Contacto.Apellido}, {e.Contacto.Nombre}: {e.Valor}");
        }
    }

    // Actualizar un contacto
}
