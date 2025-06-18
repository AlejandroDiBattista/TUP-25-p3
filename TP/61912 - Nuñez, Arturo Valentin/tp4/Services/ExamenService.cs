using Microsoft.EntityFrameworkCore;
using SistemaExamenes.Console.Data;
using SistemaExamenes.Console.Models;

namespace SistemaExamenes.Console.Services;

public class ExamenService
{
    private readonly ExamenDbContext _context;
    private readonly PreguntaService _preguntaService;
    
    public ExamenService(ExamenDbContext context, PreguntaService preguntaService)
    {
        _context = context;
        _preguntaService = preguntaService;
    }
    
    public async Task<ResultadoExamen> RealizarExamenAsync(string nombreAlumno, List<Pregunta> preguntas, List<char> respuestas)
    {
        if (preguntas.Count != respuestas.Count)
            throw new ArgumentException("La cantidad de preguntas y respuestas debe ser igual");
        
        var resultadoExamen = new ResultadoExamen
        {
            NombreAlumno = nombreAlumno,
            TotalPreguntas = preguntas.Count,
            FechaExamen = DateTime.Now
        };
        
        var respuestasExamen = new List<RespuestaExamen>();
        var respuestasCorrectas = 0;
        
        for (int i = 0; i < preguntas.Count; i++)
        {
            var pregunta = preguntas[i];
            var respuesta = char.ToUpper(respuestas[i]);
            var esCorrecta = respuesta == pregunta.RespuestaCorrecta;
            
            if (esCorrecta)
                respuestasCorrectas++;
            
            var respuestaExamen = new RespuestaExamen
            {
                PreguntaId = pregunta.Id,
                RespuestaSeleccionada = respuesta,
                EsCorrecta = esCorrecta
            };
            
            respuestasExamen.Add(respuestaExamen);
        }
        
        resultadoExamen.CantidadRespuestasCorrectas = respuestasCorrectas;
        resultadoExamen.NotaFinal = CalcularNota(respuestasCorrectas, preguntas.Count);
        
        _context.ResultadosExamen.Add(resultadoExamen);
        await _context.SaveChangesAsync();
        
        // Asignar el ID del resultado a las respuestas
        foreach (var respuesta in respuestasExamen)
        {
            respuesta.ResultadoExamenId = resultadoExamen.Id;
        }
        
        _context.RespuestasExamen.AddRange(respuestasExamen);
        await _context.SaveChangesAsync();
        
        return resultadoExamen;
    }
    
    private decimal CalcularNota(int respuestasCorrectas, int totalPreguntas)
    {
        if (totalPreguntas == 0) return 0;
        return Math.Round((decimal)respuestasCorrectas / totalPreguntas * 5, 2);
    }
    
    public async Task<List<ResultadoExamen>> ObtenerTodosLosResultadosAsync()
    {
        return await _context.ResultadosExamen
            .Include(r => r.RespuestasExamen)
            .ThenInclude(re => re.Pregunta)
            .OrderByDescending(r => r.FechaExamen)
            .ToListAsync();
    }
    
    public async Task<List<ResultadoExamen>> ObtenerResultadosPorAlumnoAsync(string nombreAlumno)
    {
        return await _context.ResultadosExamen
            .Include(r => r.RespuestasExamen)
            .ThenInclude(re => re.Pregunta)
            .Where(r => r.NombreAlumno.ToLower().Contains(nombreAlumno.ToLower()))
            .OrderByDescending(r => r.FechaExamen)
            .ToListAsync();
    }
    
    public async Task<List<ResultadoExamen>> ObtenerRankingMejoresAlumnosAsync(int top = 10)
    {
        return await _context.ResultadosExamen
            .GroupBy(r => r.NombreAlumno)
            .Select(g => new
            {
                NombreAlumno = g.Key,
                MejorNota = g.Max(r => r.NotaFinal),
                FechaMejorNota = g.Where(r => r.NotaFinal == g.Max(r => r.NotaFinal)).First().FechaExamen
            })
            .OrderByDescending(x => x.MejorNota)
            .Take(top)
            .Select(x => new ResultadoExamen
            {
                NombreAlumno = x.NombreAlumno,
                NotaFinal = x.MejorNota,
                FechaExamen = x.FechaMejorNota
            })
            .ToListAsync();
    }
    
    public async Task<List<object>> ObtenerEstadisticasPorPreguntaAsync()
    {
        var estadisticas = await _context.RespuestasExamen
            .Include(re => re.Pregunta)
            .GroupBy(re => new { re.PreguntaId, re.Pregunta.Enunciado })
            .Select(g => new
            {
                PreguntaId = g.Key.PreguntaId,
                Enunciado = g.Key.Enunciado,
                VecesRespondida = g.Count(),
                RespuestasCorrectas = g.Count(re => re.EsCorrecta),
                PorcentajeCorrecto = g.Count() > 0 ? (double)g.Count(re => re.EsCorrecta) / g.Count() * 100 : 0
            })
            .OrderBy(x => x.PreguntaId)
            .ToListAsync();
        
        return estadisticas.Cast<object>().ToList();
    }
} 