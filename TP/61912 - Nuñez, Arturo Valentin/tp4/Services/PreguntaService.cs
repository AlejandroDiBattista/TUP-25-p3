using Microsoft.EntityFrameworkCore;
using SistemaExamenes.Console.Data;
using SistemaExamenes.Console.Models;

namespace SistemaExamenes.Console.Services;

public class PreguntaService
{
    private readonly ExamenDbContext _context;
    
    public PreguntaService(ExamenDbContext context)
    {
        _context = context;
    }
    
    public async Task<Pregunta> CrearPreguntaAsync(string enunciado, string alternativaA, string alternativaB, string alternativaC, char respuestaCorrecta)
    {
        var pregunta = new Pregunta
        {
            Enunciado = enunciado,
            AlternativaA = alternativaA,
            AlternativaB = alternativaB,
            AlternativaC = alternativaC,
            RespuestaCorrecta = char.ToUpper(respuestaCorrecta)
        };
        
        _context.Preguntas.Add(pregunta);
        await _context.SaveChangesAsync();
        
        return pregunta;
    }
    
    public async Task<List<Pregunta>> ObtenerPreguntasAleatoriasAsync(int cantidad = 5)
    {
        var totalPreguntas = await _context.Preguntas.CountAsync();
        
        if (totalPreguntas == 0)
            return new List<Pregunta>();
        
        var random = new Random();
        var preguntas = await _context.Preguntas.ToListAsync();
        
        // Seleccionar preguntas aleatorias
        var preguntasAleatorias = new List<Pregunta>();
        var indicesUsados = new HashSet<int>();
        
        while (preguntasAleatorias.Count < cantidad && indicesUsados.Count < totalPreguntas)
        {
            var indice = random.Next(totalPreguntas);
            if (!indicesUsados.Contains(indice))
            {
                indicesUsados.Add(indice);
                preguntasAleatorias.Add(preguntas[indice]);
            }
        }
        
        return preguntasAleatorias;
    }
    
    public async Task<List<Pregunta>> ObtenerTodasLasPreguntasAsync()
    {
        return await _context.Preguntas.ToListAsync();
    }
    
    public async Task<Pregunta?> ObtenerPreguntaPorIdAsync(int id)
    {
        return await _context.Preguntas.FindAsync(id);
    }
    
    public async Task<bool> EliminarPreguntaAsync(int id)
    {
        var pregunta = await _context.Preguntas.FindAsync(id);
        if (pregunta == null)
            return false;
        
        _context.Preguntas.Remove(pregunta);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<int> ObtenerCantidadPreguntasAsync()
    {
        return await _context.Preguntas.CountAsync();
    }
} 