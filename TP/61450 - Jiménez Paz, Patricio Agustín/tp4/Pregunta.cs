namespace TP4_61450;

public interface IPregunta
{
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; }
    public string RespuestaA { get; set; }
    public string RespuestaB { get; set; }
    public string RespuestaC { get; set; }
    public string RespuestaCorrecta { get; set; }
    public int CantidadRespondida { get; set; }
    public int CantidadCorrecta { get; set; }
    public int CantidadIncorrecta => CantidadRespondida - CantidadCorrecta;

    public string ToString();
    public void MostrarEstadisticas();
}

public class Pregunta : IPregunta
{
    public int PreguntaId { get; set; }
    public string Enunciado { get; set; } = "";
    public string RespuestaA { get; set; } = "";
    public string RespuestaB { get; set; } = "";
    public string RespuestaC { get; set; } = "";
    public string RespuestaCorrecta { get; set; } = "";
    public int CantidadRespondida { get; set; } = 0;
    public int CantidadCorrecta { get; set; } = 0;
    public int CantidadIncorrecta => CantidadRespondida - CantidadCorrecta;

    public override string ToString()
    {
        return $"""

                #{PreguntaId:000}

                {Enunciado}

                A) {RespuestaA}
                B) {RespuestaB}
                C) {RespuestaC}
            
            """;
    }

    public void MostrarEstadisticas()
    {
        float porcentaje =
            CantidadRespondida > 0 ? (CantidadCorrecta / (float)CantidadRespondida) * 100 : 0;
        Console.WriteLine(
            $"""
                #{PreguntaId:000} - {Enunciado}

                Respondida: {CantidadRespondida} veces
                Correctas: {CantidadCorrecta} veces
                Incorrectas: {CantidadIncorrecta} veces

                Porcentaje de aciertos: {porcentaje:F2}%
                
            """
        );
    }
}
