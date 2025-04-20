using System.Text;
using System.Text.RegularExpressions;

namespace TUP;

public class Asistencia {
    public string Telefono { get; set; }
    public List<DateTime> Fechas { get; set; } = new();

    public Asistencia(string telefono) {
        Telefono = telefono;
    }

    public override string ToString() {
        var builder = new StringBuilder();
        builder.AppendLine($"Estudiante: {Telefono}");
        builder.AppendLine("Fechas de asistencia:");
        foreach (var date in Fechas.Distinct()) {
            builder.AppendLine($"  - {date:dd/MM/yyyy}");
        }
        return builder.ToString();
    }
}

public static class Asistencias {
    static bool ContieneEmoji(string texto){
        bool IsEmoji(Rune rune){
        int value = rune.Value;
        return (value >= 0x1F600 && value <= 0x1F64F) ||  // emoticonos
               (value >= 0x1F300 && value <= 0x1F5FF) ||  // pictogramas
               (value >= 0x1F680 && value <= 0x1F6FF) ||  // transporte
               (value >= 0x1F900 && value <= 0x1F9FF) ||  // gestos
               (value >= 0x1FA70 && value <= 0x1FAFF);    // más emojis
        }
        foreach (var rune in texto.EnumerateRunes()){
            if (IsEmoji(rune))
                return true;
        }
        return false;
    }

    public static List<Asistencia> CargarAsistencias(string origen) {
        var camino = $"/Users/adibattista/Documents/GitHub/tup-25-p3/datos/{origen}";
        if (!File.Exists(camino)) {
            Console.WriteLine($"El archivo {camino} no existe.");
            return new List<Asistencia>();
        }

        var lineas = File.ReadAllLines(camino);
        var estudiantes = new Dictionary<string, Asistencia>();
        
        DateTime fechaActual = DateTime.MinValue;
        
        foreach (var linea in lineas) {
            if (string.IsNullOrWhiteSpace(linea.Trim()))
                continue;

            var dateMatch = Regex.Match(linea.Trim(), @"^(\d{1,2})/(\d{1,2})$");
            if (dateMatch.Success) {
                int dia = int.Parse(dateMatch.Groups[1].Value);
                int mes = int.Parse(dateMatch.Groups[2].Value);
                fechaActual = new DateTime(DateTime.Now.Year, mes, dia);
                continue;
            }

            var mensajeMatch = Regex.Match(linea.Trim(), @"(\d{2}):(\d{2})\s+De:\s+(\d+|unknown)\s+-\s+Mensaje:\s+(.*)");
            if (mensajeMatch.Success && fechaActual != DateTime.MinValue) {
                int horas = int.Parse(mensajeMatch.Groups[1].Value);
                int minutos = int.Parse(mensajeMatch.Groups[2].Value);
                string telefono = mensajeMatch.Groups[3].Value;
                string mensaje  = mensajeMatch.Groups[4].Value;

                if (telefono == "unknown")
                    continue;

                if (ContieneEmoji(mensaje)) {
                    if (!estudiantes.ContainsKey(telefono)) {
                        estudiantes[telefono] = new Asistencia(telefono);
                    }

                    if (!estudiantes[telefono].Fechas.Any(d => d.Date == fechaActual.Date)) {
                        estudiantes[telefono].Fechas.Add(fechaActual);
                    }
                }
            }
        }

        return estudiantes.Values.ToList();
    }

    public static List<Asistencia> Cargar(bool listar = false) {
        var salida = new Dictionary<string, List<DateTime>>();

        foreach (var origen in Directory.GetFiles("./asistencias", "*.md")){   
            List<Asistencia> estudiantes = CargarAsistencias(origen);
            foreach (var estudiante in estudiantes) {
                if (!salida.ContainsKey(estudiante.Telefono)) {
                    salida[estudiante.Telefono] = new List<DateTime>();
                }
                salida[estudiante.Telefono].AddRange(estudiante.Fechas);
            }
        }   

        // Cuenta cuantas veces hay asistencias en la fecha
        List<Asistencia> asistencias = salida
            .Select(item => new Asistencia(item.Key) { Fechas = item.Value })
            .ToList();
        
        if(listar){
            int antes = 0;
            var contador = new Dictionary<DateTime, int>();
            foreach (var (telefono, fechas) in salida){
                foreach (var fecha in fechas.Distinct()) {
                    contador[fecha.Date] = contador.GetValueOrDefault(fecha.Date) + 1;
                    antes++;
                }
            }
            // Filtra las fechas que tienen más de 30 asistencias
            Consola.Escribir("=== Fechas y cantidad de asistencias ===", ConsoleColor.Green);
            foreach (var entrada in contador.OrderBy(c => c.Key)){
                Consola.Escribir($"{entrada.Key:dd/MM/yyyy}: {entrada.Value} veces");
            }
            Consola.Escribir("===\n");
        
            Consola.Escribir("=== Asistencias ===", ConsoleColor.Cyan);
            Consola.Escribir($"Hay {asistencias.Count} asistencias", ConsoleColor.Cyan);
        }
        // Ordena la lista por el número de asistencias
        return asistencias;
    }
}


