using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using static System.Console;

using System.Security.Cryptography;
using System.Text;

// static int codificar(int pregunta, int respuesta) {
//     string input = $"{pregunta}-{respuesta}";
//     using var sha = SHA256.Create();
//     byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
//     int value = ((hash[0] << 8) | hash[1]) % 10000;
//     return value < 0 ? -value : value;
// }
static string codificar(int pregunta, int respuesta) {
    if (respuesta < 1 || respuesta > 3) return "";
    string input = $"{pregunta}-{respuesta}";
    using var sha = SHA256.Create();
    byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
    int value = Math.Abs((hash[0] << 8) | hash[1]) % 10000;
    return $"{value:D4}";
}

static int decodificar(int pregunta, string codigo) {
    for(var r = 1; r <= 3; r++) {
        if (codificar(pregunta, r) == (codigo ?? "")) return r;
    }
    return 0;
}

static void TestCodificacion() {
    for(var r = 1; r <= 3; r++) {
        for(var p = 1; p <= 999; p++) {
            var codigo = codificar(p, r);
            if(decodificar(p,codigo) == r) continue;
            WriteLine($"Error en {p} : {r} -> {codigo}");
        }
    }
}
TestCodificacion();

static void EsperarTecla(){
        Write("Presione una tecla para continuar...");
        ReadKey(true);
        Write($"\r{' ',40}\r");
    }

static int LeerNumero(string prompt) {
    Write(prompt);
    string input = ReadLine()?.Trim() ?? "";
    
    if (int.TryParse(input, out int result))
        return result;
    return 0;
}

static int ObtenerParametro(int indice, int asumir = 0){
    string[] args = Environment.GetCommandLineArgs().Skip(2).ToArray();
    if (args == null || indice < 0 || indice >= args.Length)
        return asumir;
        
    if (!int.TryParse(args[indice], out int valor))
        return asumir;
        
    return valor;
}

// Método auxiliar para obtener el path del archivo fuente
static string Ubicar(string nombre) {
    string GetSourceFilePath([System.Runtime.CompilerServices.CallerFilePath] string path = null) => path;
    string baseDir = Path.GetDirectoryName(GetSourceFilePath());
    string path = Path.Combine(baseDir, nombre);
    if (File.Exists(path)) return path;
    return Path.Combine(Environment.CurrentDirectory, nombre);
}

// Clase Pregunta
public class Pregunta {
    public int Numero { get; set; } = 0;
    public string Texto { get; set; } = "";
    public string[] Respuestas { get; set; } = new string[3];
    public int Correcta { get; set; } = 0; // Respuesta correcta (1, 2 o 3)
    public int Respuesta { get; set; } = 0;
    
    public bool EsRespondida => Respuesta != 0;
    public bool EsIncorrecta => Respuesta != Correcta;
    public bool EsCorrecta   => Respuesta == Correcta;
    
    public override string ToString() =>        
        $"""
    
        ### {Numero:D3} {codificar(Numero, Correcta)}
        {Texto}
        
        a) {Respuestas[0]}
        b) {Respuestas[1]}
        c) {Respuestas[2]}
        
        """;

    public void Mostrar(bool numerico = false, bool solucion = false){
        string f(string texto) => texto.Replace("```csharp", "").Replace("```", "");//.Replace("`", "");

        string opciones = numerico ?  "123" : "abc";
        int[] respuestas = solucion ? [Correcta - 1] : [0, 1, 2];
        WriteLine($"""
    
        ### {Numero:D3} 
        {f(Texto)}

        """);

        foreach(var r in respuestas)
            WriteLine($"{opciones[r]}) {f(Respuestas[r])}");
    }
}

// Clase contenedora de Preguntas
public class Preguntas : IEnumerable<Pregunta>  {
    List<Pregunta> Lista { get; set; } = new();

    public void CargarPreguntas(string Origen){
        var lineas = File.ReadAllLines(Origen);
        var respuesta = -1;
        Pregunta pregunta = null;
        foreach (var linea in lineas) {
            if(linea.StartsWith("# ") || linea.Trim() == "" || linea.Trim() == "---") continue;

            // Comienza la pregunta
            if (Regex.Match(linea, @"^###\s*(\d+)(?:\s+(\d{1,4}))?") is var preg && preg.Success) {
                int numero = int.Parse(preg.Groups[1].Value);
                string codigo = preg.Groups[2].Success ? preg.Groups[2].Value : "";

                if (pregunta is not null) Lista.Add(pregunta);
                
                pregunta = new Pregunta();
                pregunta.Numero = numero;
                pregunta.Correcta = decodificar(pregunta.Numero, codigo);
                respuesta = -1;
                WriteLine($"Cargando pregunta {pregunta.Numero} ({pregunta.Correcta}) {codigo}");
                continue;
            };

            // Comienza la respuesta
            if (Regex.Match(linea, @"^([abc])[\)\.]\s*(.*)") is var resp && resp.Success) {
                respuesta = resp.Groups[1].Value[0] - 'a';
                // Si hay texto, lo asigna; si no, deja la respuesta vacía
                pregunta.Respuestas[respuesta] = resp.Groups[2].Success ? resp.Groups[2].Value.Trim() : "";
                continue;
            }

            // Agrega texto a la pregunta o respuesta
            if(respuesta < 0){
                pregunta.Texto += $"\n{linea}";
            } else {
                pregunta.Respuestas[respuesta] += $"\n{linea}";
            }
        }
        
        Lista.Add(pregunta);
    }   

    public void CargarRespuestas(string origen){
        var lineas = File.ReadAllLines(origen);
        foreach (var linea in lineas) {
            var match = Regex.Match(linea, @"^\s*(\d+)\s*\.?\s*([abc])\b");
            if (!match.Success) continue;

            int numero = int.Parse(match.Groups[1].Value);
            char letra = char.ToLower(match.Groups[2].Value[^1]);

            var pregunta = this.FirstOrDefault(p => p.Numero == numero);
            if(pregunta is null) continue;
            pregunta.Correcta = letra - 'a' + 1;
        }
    }

    public void CargarResultados(string origen) {
        var lineas = File.ReadAllLines(origen);
        foreach (var linea in lineas) {
            var match = Regex.Match(linea, @"^\s*(\d+)\s*\.?\s*([abc])\b");
            if (!match.Success) continue;

            int numero = int.Parse(match.Groups[1].Value);
            char letra = char.ToLower(match.Groups[2].Value[^1]);
            int respuesta = letra - 'a' + 1;

            var pregunta = this.FirstOrDefault(p => p.Numero == numero);
            if (pregunta != null) {
                pregunta.Respuesta = respuesta;
            }
        }
    }

    public void GuardarPreguntas(string destino){
        using (var sw = new StreamWriter(destino)) {
            sw.WriteLine("# Preguntas para el 1er Parcial");
            foreach (var pregunta in this) {
                sw.WriteLine(pregunta.ToString());
                sw.WriteLine("---");
            }
        }
    }

    public void GuardarRespuestas(string destino){
        using (var sw = new StreamWriter(destino)) {
            foreach (var pregunta in this.OrderBy(p => p.Numero)) {
                sw.WriteLine($"{pregunta.Numero:D3}. {(char)('a' + pregunta.Correcta - 1)}");
            }
        }
    }

    public void Renumerar(){
        int n = 1;
        foreach (var p in Lista) {
            if (p.Numero != n) {
                WriteLine($"Renumerando {p.Numero} a {n}");
                p.Numero = n;
            }
            n++;
        }
    }

    public static Preguntas Cargar(bool cargarRespuestas = false){
        var preguntas = new Preguntas();
        preguntas.CargarPreguntas(Ubicar("preguntas-examen.md"));
        if(cargarRespuestas) 
            preguntas.CargarRespuestas(Ubicar("respuestas-examen.md"));
        preguntas.Renumerar();
        preguntas.GuardarPreguntas(Ubicar("preguntas-examen.md"));
        preguntas.GuardarRespuestas(Ubicar("respuestas-examen.md"));
        return preguntas;
    }

    public bool Validar() {
        bool valido = true;
        // Verificar numeración consecutiva
        int esperado = 1;
        foreach (var q in Lista.OrderBy(p => p.Numero)) {
            if (q.Numero != esperado) {
                WriteLine($"Error en {nameof(Validar)}: Falta la pregunta número {esperado}.");
                valido = false;
                esperado = q.Numero;
            }
            esperado++;
        }
        // Verificar que haya exactamente 3 respuestas y estén definidas
        foreach (var q in Lista) {
            if (q.Respuestas == null || q.Respuestas.Length != 3) {
                WriteLine($"Error en {nameof(Validar)}: La pregunta {q.Numero} no tiene exactamente 3 respuestas.");
                valido = false;
            } else {
                for (int i = 0; i < 3; i++) {
                    if (string.IsNullOrWhiteSpace(q.Respuestas[i])) {
                        WriteLine($"Error en {nameof(Validar)}: La pregunta {q.Numero} tiene la respuesta {i+1} vacía.");
                        valido = false;
                    }
                }
            }
            // Verificar que la respuesta correcta esté en 1..3
            if (q.Correcta < 1 || q.Correcta > 3) {
                WriteLine($"Error en {nameof(Validar)}: La pregunta {q.Numero} tiene un índice de respuesta correcta inválido ({q.Correcta}).");
                valido = false;
            }
        }
        return valido;
    }

    public List<Pregunta> GenerarExamen(int cantidad) {
        var origen = cantidad < 0 ? Incorrectas() : Pendientes();
        cantidad = Math.Min(Math.Abs(cantidad), origen.Count());
        Random random = new Random();
        return origen.OrderBy(x => random.Next()).Take(cantidad).OrderBy(p => p.Numero).ToList();
    }

    public IEnumerator<Pregunta> GetEnumerator() => Lista.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<Pregunta> Pendientes()  => this.Where(p => !p.EsRespondida);
    public IEnumerable<Pregunta> Respondidas() => this.Where(p => p.EsRespondida);
    public IEnumerable<Pregunta> Correctas()   => this.Where(p => p.EsRespondida && p.EsCorrecta);
    public IEnumerable<Pregunta> Incorrectas() => this.Where(p => p.EsRespondida && p.EsIncorrecta);

    public int Count => Lista.Count;
}


class Examen {
    public List<Pregunta> Preguntas { get; set; } = new();
    public bool MostrarNumeros { get; set; } = false;
    private Preguntas Base { get; set; }

    private DateTime HoraInicio; // Registrar hora de inicio
    private int Legajo { get; set; } = 0;

    public Examen(Preguntas preguntas, int cantidad, int legajo = 0) {
        Base = preguntas;
        Legajo = legajo;
        HoraInicio = DateTime.Now;
        Preguntas = preguntas.GenerarExamen(cantidad);
        Preguntas.ForEach( p => p.Respuesta = 0);
    }

    static bool IsNumericKey(ConsoleKey key) {
        return key == ConsoleKey.D1 || key == ConsoleKey.NumPad1 ||
               key == ConsoleKey.D2 || key == ConsoleKey.NumPad2 ||
               key == ConsoleKey.D3 || key == ConsoleKey.NumPad3;
    }
    
    static bool IsLetterKey(ConsoleKey key) {
        return key == ConsoleKey.A || key == ConsoleKey.B || key == ConsoleKey.C;
    }
    
    public int Nota() =>
        Preguntas.Count(p => p.Respuesta == p.Correcta);

    public void Evaluar(){

        string FormatResponse(int respuesta) => 
                    MostrarNumeros ? respuesta.ToString() : ('a' + respuesta - 1).ToString();
        
        var todas = Preguntas.ToList();
        int cantidad = Preguntas.Count();
        int actual = 0;
        bool abortar = false;
        TimeSpan limite = TimeSpan.FromMinutes(20);

        if (Preguntas.Count() == 0) {
            WriteLine("\n🎉 Felicitaciones. Respondiste todas las preguntas. 🎉\n\n");
            Environment.Exit(0); // Terminar el programa
            return;
        }

        while (actual < Preguntas.Count()) {
            DateTime tickInicio = DateTime.Now;
            ConsoleKeyInfo? key = null;
            int lastSecond = -1;
            Pregunta preguntaActual = Preguntas[actual];

            while (key == null) {
                TimeSpan transcurrido = DateTime.Now - HoraInicio;
                TimeSpan restante = limite - transcurrido;
                if (restante < TimeSpan.Zero) restante = TimeSpan.Zero;

                int segRest = (int)restante.TotalSeconds;
                if (lastSecond != segRest) {
                    Clear();
                    WriteLine($"- Pregunta {actual + 1,2} de {Preguntas.Count} --- Legajo: {Legajo} - Examen 1er Parcial - Tiempo restante: {restante.Minutes:D2}:{restante.Seconds:D2} -\n");
                    preguntaActual.Mostrar(numerico: MostrarNumeros);
                    if (preguntaActual.Respuesta != 0) {
                        WriteLine($"\nRespuesta: {FormatResponse(preguntaActual.Respuesta)}\n");
                    } else {
                        WriteLine("\n\n\n");
                    }
                    Write("Respuesta (←/→ navegar, a/b/c o 1/2/3 para responder): ");
                    lastSecond = segRest;
                }

                if ((DateTime.Now - HoraInicio) >= limite) {
                    WriteLine("\n\nSe acabó el tiempo del examen.");
                    EsperarTecla();
                    actual = Preguntas.Count; // salir del bucle principal
                    break;
                }

                if (KeyAvailable) {
                    key = ReadKey(true);
                } else {
                    System.Threading.Thread.Sleep(100);
                }
            }
            if (key == null) break; // por si se acabó el tiempo

            var k = key.Value.Key;
            if (IsNumericKey(k) || IsLetterKey(k)) MostrarNumeros = IsNumericKey(k);

            int resp = k switch {
                ConsoleKey.A or ConsoleKey.D1 or ConsoleKey.NumPad1 => 1,
                ConsoleKey.B or ConsoleKey.D2 or ConsoleKey.NumPad2 => 2,
                ConsoleKey.C or ConsoleKey.D3 or ConsoleKey.NumPad3 => 3,
                _ => 0
            };
            
            if (resp > 0) {
                Preguntas[actual].Respuesta = resp;
                actual++;
                continue;
            }
            
            switch (k) {
                case ConsoleKey.Escape:
                case ConsoleKey.X:
                    abortar = true;
                    actual = Preguntas.Count;
                    break;
                case ConsoleKey.LeftArrow:
                    if (actual > 0) actual--;
                    break;
                case ConsoleKey.RightArrow:
                    if (actual < Preguntas.Count - 1) actual++;
                    break;
                default:
                    break;
            }
        }

        if (abortar) return;

        TimeSpan duracion = DateTime.Now - HoraInicio;

        Clear();
        WriteLine();
        var incorrectas = Preguntas.Where(p => p.EsIncorrecta);
        if (incorrectas.Any()) {
            WriteLine($"Hay {incorrectas.Count()} {(incorrectas.Count() == 1 ? "respuesta incorrecta" : "respuestas incorrectas")}\n");
            int i = 1;
            foreach(var p in incorrectas) {
                WriteLine($"{i++}) \n");
                WriteLine($"Tu respuesta fue {FormatResponse(p.Respuesta)} la correcta era {FormatResponse(p.Correcta)}\n");
                WriteLine("------------");
                p.Mostrar(solucion: true, numerico: MostrarNumeros);
                WriteLine("------------");
                EsperarTecla();
                Clear();
            }
        } else {
            WriteLine("¡Todas las respuestas fueron correctas! 🎉 Felicitaciones.");
        }
        Informar();
    }

    // Modificar para recibir duración
    private void Informar(){
        TimeSpan duracion = DateTime.Now - HoraInicio;

        // Nota y porcentaje de la sesión
        Clear();
        var cantidad = Preguntas.Count;
        var total = Base.Count();
        var respondidas = Base.Respondidas().Count();
        var correctas   = Base.Correctas().Count();
        var incorrectas = Base.Incorrectas().Count();

        WriteLine($"""

        ### Examen 1er Parcial ###

            --- Último examen ---
                     Nota: {Nota()} de {cantidad};
               Porcentaje: {(Nota() * 100) / cantidad}%
            
            --- Evaluación Total ---
                Preguntas: {total,3}
              Respondidas: {respondidas,3}
                Correctas: {correctas,3}
              Incorrectas: {incorrectas,3} 
               Porcentaje: {(correctas * 100) / respondidas}%
             
        """);

        // Mostrar duración y segundos por pregunta si corresponde
        // Quitar .Value porque duracion es TimeSpan, no TimeSpan?
        if (duracion != null) {
            double segundos = duracion.TotalSeconds;
            double segPorPregunta = cantidad > 0 ? segundos / cantidad : 0;
            WriteLine($"    Tiempo total: {duracion.Minutes:D2}:{duracion.Seconds:D2} ({segundos:F1} segundos)");
            WriteLine($"    Segundos por pregunta: {segPorPregunta:F2}");
        }
    }
}

// --- EJECUCIÓN DEL EXAMEN ---

var preguntas = Preguntas.Cargar();

Clear();
WriteLine("\n\n### Examen 1er Parcial ###\n\n");

int legajo   = ObtenerParametro(0);
int cantidad = ObtenerParametro(1, 10);
while(legajo < 55000 || legajo > 65000) {
    legajo = LeerNumero("Ingrese número de legajo: ");
} 

preguntas.CargarResultados(Ubicar($"{legajo}.txt"));

var examen = new Examen(preguntas, cantidad);
// Llamar a Evaluar sin argumentos
examen.Evaluar();
// preguntas.GuardarRespuestas(Ubicar($"{legajo}.txt"));

// Fin del programa