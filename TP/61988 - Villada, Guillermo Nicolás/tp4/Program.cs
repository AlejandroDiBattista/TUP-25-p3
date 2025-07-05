using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

public class Pregunta
{
    public int Id { get; set; } // Asegúrate de que el campo Id está definido como clave primaria
    public string Texto { get; set; }
    public List<Respuesta> Respuestas { get; set; } // Relación de uno a muchos con Respuestas
}

public class Respuesta
{
    public int Id { get; set; } // Id de la respuesta
    public string Texto { get; set; }
    public bool EsCorrecta { get; set; }
    public int PreguntaId { get; set; } // Clave foránea hacia la Pregunta
    public Pregunta Pregunta { get; set; } // Relación de muchos a uno con Pregunta
}

public class DatosContexto : DbContext
{
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Respuesta> Respuestas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=examen.db"); // Archivo SQLite donde se almacenará la base de datos
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        using (var db = new DatosContexto())
        {
            // Asegurarse de que la base de datos esté creada
            db.Database.EnsureCreated();

            MenuPrincipal(db);
        }
    }

    public static void MenuPrincipal(DatosContexto db)
    {
        Console.WriteLine("Bienvenido al sistema de examen!");
        Console.WriteLine("Ingresa tu nombre para comenzar el examen:");

        string nombre = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            Console.WriteLine("Nombre inválido. El examen no puede comenzar.");
            return;
        }

        Console.WriteLine($"Hola, {nombre}! Vamos a comenzar con el examen.");
        TomarExamen(db);
    }

    public static void TomarExamen(DatosContexto db)
    {
        // Obtienes las preguntas y las ordenas aleatoriamente en memoria
        var preguntas = db.Preguntas.ToList();
        var random = new Random();
        preguntas = preguntas.OrderBy(x => random.Next()).ToList(); // Ordena aleatoriamente

        Console.WriteLine("¡Comencemos el examen!");

        foreach (var pregunta in preguntas)
        {
            Console.WriteLine($"Pregunta: {pregunta.Texto}");

            // Mostrar las respuestas posibles
            var respuestas = pregunta.Respuestas.OrderBy(r => random.Next()).ToList(); // Respuestas aleatorias

            foreach (var respuesta in respuestas)
            {
                Console.WriteLine($"{respuesta.Id}. {respuesta.Texto}");
            }

            // Pedir al usuario que elija una respuesta
            int respuestaElegida;
            while (true)
            {
                Console.Write("Ingresa el número de tu respuesta: ");
                if (int.TryParse(Console.ReadLine(), out respuestaElegida) &&
                    respuestas.Any(r => r.Id == respuestaElegida))
                {
                    break;
                }
                Console.WriteLine("Por favor ingresa una opción válida.");
            }

            var respuestaCorrecta = respuestas.FirstOrDefault(r => r.EsCorrecta);
            if (respuestaElegida == respuestaCorrecta.Id)
            {
                Console.WriteLine("¡Respuesta correcta!");
            }
            else
            {
                Console.WriteLine($"Respuesta incorrecta. La respuesta correcta es: {respuestaCorrecta.Texto}");
            }
        }

        Console.WriteLine("¡Has completado el examen! Gracias por participar.");
    }
}
