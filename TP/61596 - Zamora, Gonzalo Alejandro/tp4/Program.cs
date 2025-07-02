using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;

class Pregunta {
    public int PreguntaId { get; set; }
    public string Enunciado  { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string Correcta   { get; set; } = "";
}

class DatosContexto : DbContext{
    public DbSet<Pregunta> Preguntas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
        optionsBuilder.UseSqlite("Data Source=examen.db");
    }

}

class Program{
    static void Main(string[] args){
        using (var db = new DatosContexto()){
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            
            var p = new Pregunta {
                Enunciado  = "¿Cuál es el lenguaje de programación desarrollado por Microsoft y utilizado principalmente en .NET?",
                RespuestaA = "Java",
                RespuestaB = "C#",
                RespuestaC = "Python",
                Correcta   = "B"
            };
            db.Preguntas.Add(p);
            db.SaveChanges();
            
            Console.Clear();
            foreach(var pregunta in db.Preguntas){
                Console.WriteLine($"""

                    #{pregunta.PreguntaId:000}
                
                    {p.Enunciado}

                     A) {p.RespuestaA}
                     B) {p.RespuestaB}
                     C) {p.RespuestaC}

                """);
            }
        }
    }
}