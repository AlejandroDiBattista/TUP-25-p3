using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


public class Pregunta
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = "";
    public string A { get; set; } = "";
    public string B { get; set; } = "";
    public string C { get; set; } = "";
    public char Correcta { get; set; }
}

public class Resultado
{
    public int Id { get; set; }
    public string Alumno { get; set; } = "";
    public int Correctas { get; set; }
    public int Total { get; set; }
    public double Nota { get; set; }
}


public class AppDb : DbContext
{
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<Resultado> Resultados => Set<Resultado>();

    protected override void OnConfiguring(DbContextOptionsBuilder opt)
        => opt.UseSqlite("Data Source=datos.db");
}


class Program
{
    static void Main()
    {
        using var db = new AppDb();
        db.Database.Migrate();

        while (true)
        {
            Console.WriteLine("\n1. Agregar pregunta");
            Console.WriteLine("2. Rendir examen");
            Console.WriteLine("3. Ver resultados");
            Console.WriteLine("4. Salir");
            Console.Write("Opción: ");
            var op = Console.ReadLine();

            if (op == "1") AgregarPregunta(db);
            else if (op == "2") RendirExamen(db);
            else if (op == "3") VerResultados(db);
            else if (op == "4") break;
        }
    }

    static void AgregarPregunta(AppDb db)
    {
        var p = new Pregunta();
        Console.Write("Enunciado: "); p.Enunciado = Console.ReadLine() ?? "";
        Console.Write("Opción A: "); p.A = Console.ReadLine() ?? "";
        Console.Write("Opción B: "); p.B = Console.ReadLine() ?? "";
        Console.Write("Opción C: "); p.C = Console.ReadLine() ?? "";
        Console.Write("Respuesta correcta (A/B/C): "); p.Correcta = char.ToUpper(Console.ReadKey().KeyChar);
        Console.WriteLine();
        db.Preguntas.Add(p);
        db.SaveChanges();
        Console.WriteLine("Pregunta guardada.");
    }

    static void RendirExamen(AppDb db)
    {
        Console.Write("Nombre del alumno: ");
        string nombre = Console.ReadLine() ?? "";

        var preguntas = db.Preguntas.AsEnumerable().OrderBy(x => Guid.NewGuid()).Take(5).ToList();
        if (preguntas.Count == 0) { Console.WriteLine("No hay preguntas."); return; }

        int correctas = 0;
        foreach (var p in preguntas)
        {
            Console.WriteLine($"\n{p.Enunciado}");
            Console.WriteLine($"A) {p.A}");
            Console.WriteLine($"B) {p.B}");
            Console.WriteLine($"C) {p.C}");
            Console.Write("Respuesta: ");
            char r = char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();
            if (r == p.Correcta) correctas++;
        }

        var resultado = new Resultado
        {
            Alumno = nombre,
            Total = preguntas.Count,
            Correctas = correctas,
            Nota = correctas
        };

        db.Resultados.Add(resultado);
        db.SaveChanges();

        Console.WriteLine($"\nTu nota es: {correctas}/{preguntas.Count}");
    }

    static void VerResultados(AppDb db)
    {
        Console.WriteLine("\nResultados:");
        foreach (var r in db.Resultados)
        {
            Console.WriteLine($"Alumno: {r.Alumno} | Nota: {r.Nota}/{r.Total}");
        }
    }
}
