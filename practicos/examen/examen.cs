using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using static System.Console;

using System.Security.Cryptography;
using System.Text;


static void EsperarTecla(){
    Write("Presione una tecla para continuar...");
    ReadKey(true);
    Write($"\r{' ',40}\r");
}

static bool Confirmar(string mensaje = "¿Está seguro?") {
    Write($"{mensaje} (s/n) : ");
    ConsoleKeyInfo key = ReadKey(true);
    while(true) {
        if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.Y || key.Key == ConsoleKey.NumPad1 || key.Key == ConsoleKey.D1) {
            WriteLine("Si\n");
            return true;
        } else if (key.Key == ConsoleKey.N || key.Key == ConsoleKey.NumPad0 || key.Key == ConsoleKey.D0) {
            WriteLine("No\n");
            return false;
        }
        key = ReadKey(true);
    }
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


// Clase Pregunta
public class Pregunta {
    public int Numero { get; set; } = 0;
    public string Texto { get; set; } = "";
    public string[] Respuestas { get; set; } = new string[3];
    public int Correcta { get; set; } = 0; 
    public int Respuesta { get; set; } = 0;
    public string Original { get; set; } = "";
    
    public bool EsRespondida => Respuesta != 0;
    public bool EsIncorrecta => Respuesta != Correcta;
    public bool EsCorrecta   => Respuesta == Correcta;
    
    public char RespuestaLetra { 
        get => (char)('a' + Respuesta - 1);
        set => Respuesta = (value < 'a' || value > 'c') ? 0 : value - 'a' + 1;
    }

    public char CorrectaLetra { 
        get => (char)('a' + Correcta - 1);
        set => Correcta = (value < 'a' || value > 'c') ? 0 : value - 'a' + 1;
    }

    public override string ToString() =>        
        $"""
    
        ### {Numero:D3} {codificar(Numero, Correcta)}
        {Texto}
        
        a) {Respuestas[0]}
        b) {Respuestas[1]}
        c) {Respuestas[2]}
        
        """;

    static public Pregunta Cargar(string texto){
        var respuesta = -1;
        var pregunta = new Pregunta();
        var lineas = texto.Split("\n");
        pregunta.Original = texto;
        WriteLine($"Cargando {texto}");
        foreach(var linea in lineas){
            if(linea.StartsWith("# ") || linea.Trim() == "" || linea.Trim() == "---") continue;
            WriteLine($"Cargando {linea}");

            // Comienza la pregunta
            if (Regex.Match(linea, @"^###\s*(\d+)(?:\s+(\d{1,4}))?") is var preg && preg.Success) {
                int numero = int.Parse(preg.Groups[1].Value);
                string codigo = preg.Groups[2].Success ? preg.Groups[2].Value : "";

                pregunta.Numero   = numero;
                pregunta.Correcta = decodificar(pregunta.Numero, codigo);
                respuesta = -1;
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
            if (respuesta < 0) {
                pregunta.Texto += $"\n{linea}";
            } else {
                pregunta.Respuestas[respuesta] += $"\n{linea}";
            }
        }

        return pregunta;
    }

    public void Mostrar(bool numerico = false, bool solucion = false) {
        string f(string texto) => texto?.Replace("```csharp", "").Replace("```", "") ?? "";

        string opciones = numerico ? "123" : "abc";
        int[] respuestas = solucion ? [Correcta - 1] : [0, 1, 2];

        WriteLine($"""
    
        ### {Numero:D3} 
        {f(Texto)}

        """);

        foreach(var r in respuestas) {
            // Validar que r esté dentro del rango válido para opciones y Respuestas
            if (r >= 0 && r < opciones.Length && r < Respuestas.Length) {
                WriteLine($"{opciones[r]}) {f(Respuestas[r])}");
            }
        }

        if(!solucion) {
            if (Respuesta > 0 && Respuesta <= opciones.Length) {
                WriteLine($"\nRespuesta: {opciones[Respuesta-1]})\n");
            } else {
                WriteLine("\n\n\n");
            }
        }
    }

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

}

// Clase contenedora de Preguntas
public class Preguntas : IEnumerable<Pregunta>  {
    List<Pregunta> Lista { get; set; } = new();

    public static Preguntas Cargar(bool cargarRespuestas = false){
        var preguntas = new Preguntas();
        preguntas.CargarPreguntas("preguntas-examen.md");
        if(cargarRespuestas) preguntas.CargarRespuestas("respuestas-examen.md");
        preguntas.Renumerar();
        preguntas.GuardarPreguntas("preguntas-examen.md");
        preguntas.GuardarRespuestas("respuestas-examen.md");
        return preguntas;
    }

    public void CargarPreguntas(string Origen){
        Origen = Ubicar(Origen);
        if (!File.Exists(Origen)) return;

        var texto = File.ReadAllText(Origen);
        foreach (var pregunta in texto.Split("---")) {
            if(pregunta.Trim() == "") continue;
            Lista.Add(Pregunta.Cargar(pregunta));
        }
    }   

    public void CargarRespuestas(string origen){
        origen = Ubicar(origen);
        if(!File.Exists(origen)) return;
        foreach (var pregunta in this) { pregunta.Correcta = 0; }

        var lineas = File.ReadAllLines(origen);
        foreach (var linea in lineas) {
            var match = Regex.Match(linea, @"^\s*(\d+)\s*\.?\s*([abc])\b");
            if (!match.Success) continue;

            int numero = int.Parse(match.Groups[1].Value);
            char letra = char.ToLower(match.Groups[2].Value[0]);

            if (this.FirstOrDefault(p => p.Numero == numero) is Pregunta pregunta) {
                pregunta.Correcta = letra - 'a' + 1;
            }
        }
    }

    public void CargarResultados(string origen) {
        origen = Ubicar(origen);
        if(!File.Exists(origen)) return;
        foreach (var pregunta in this) { pregunta.Respuesta = 0; }

        var lineas = File.ReadAllLines(origen);
        foreach (var linea in lineas) {
            var match = Regex.Match(linea, @"^\s*(\d+)\s*\.?\s*([abc])\b");
            if (!match.Success) continue;

            int numero = int.Parse(match.Groups[1].Value);
            char letra = char.ToLower(match.Groups[2].Value[0]);

            if (this.FirstOrDefault(p => p.Numero == numero) is Pregunta pregunta) {
                pregunta.Respuesta = letra - 'a' + 1;
            }
        }
    }

    public void GuardarPreguntas(string destino){
        destino = Ubicar(destino);
        using (var sw = new StreamWriter(destino)) {
            sw.WriteLine("# Preguntas para el 1er Parcial");
            foreach (var pregunta in this.OrderBy(p => p.Numero)) {
                sw.WriteLine(pregunta.ToString());
                sw.WriteLine("---");
            }
        }
    }

    public void GuardarRespuestas(string destino){
        destino = Ubicar(destino);
        using (var sw = new StreamWriter(destino)) {
            foreach (var pregunta in this.OrderBy(p => p.Numero)) {
                sw.WriteLine($"{pregunta.Numero:D3}. {(char)('a' + pregunta.Correcta - 1)}");
            }
        }
    }

    public void GuardarResultados(string destino){
        destino = Ubicar(destino);
        using (var sw = new StreamWriter(destino)) {
            foreach (var pregunta in this.Respondidas().OrderBy(p => p.Numero)) {
                sw.WriteLine($"{pregunta.Numero:D3}. {(char)('a' + pregunta.Respuesta - 1)} {(pregunta.EsIncorrecta ? "❌" : "✅")}");
            }
        }
    }

    public void InformarResultados(){
        int total = this.Count();
        int respondidas = this.Respondidas().Count();
        int correctas = this.Correctas().Count();
        int incorrectas = this.Incorrectas().Count();
        WriteLine($"\nResumen de resultados cargados:");
        WriteLine($"  Total de preguntas: {total,3}");
        WriteLine($"         Respondidas: {respondidas,3}");
        WriteLine($"           Correctas: {correctas,3}");
        WriteLine($"         Incorrectas: {incorrectas,3}\n");
        EsperarTecla();
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

    public List<Pregunta> GenerarExamen(int cantidad, int semilla = 0) {
        var origen = cantidad < 0 ? Incorrectas() : Pendientes();
        cantidad = Math.Min(Math.Abs(cantidad), origen.Count());
        Random random = new Random(semilla);
        return origen.OrderBy(x => random.Next()).Take(cantidad).OrderBy(p => p.Numero).ToList();
    }

    static string Ubicar(string nombre) {
        string GetSourceFilePath([System.Runtime.CompilerServices.CallerFilePath] string path = null) => path;
        string baseDir = Path.GetDirectoryName(GetSourceFilePath());
        string path = Path.Combine(baseDir, nombre);
        if (File.Exists(path)) return path;
        return Path.Combine(Environment.CurrentDirectory, nombre);
    }

    public IEnumerator<Pregunta> GetEnumerator() => Lista.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<Pregunta> Pendientes()  => this.Where(p => !p.EsRespondida);
    public IEnumerable<Pregunta> Respondidas() => this.Where(p => p.EsRespondida);
    public IEnumerable<Pregunta> Correctas()   => this.Where(p => p.EsRespondida && p.EsCorrecta);
    public IEnumerable<Pregunta> Incorrectas() => this.Where(p => p.EsRespondida && p.EsIncorrecta);
}


class Examen {
    public List<Pregunta> Preguntas { get; set; } = new();
    public bool MostrarNumeros { get; set; } = false;
    private Preguntas Base { get; set; }

    private DateTime HoraInicio, HoraFinal; // Registrar hora de inicio
    private int Legajo { get; set; } = 0;

    public Examen(Preguntas preguntas, int cantidad, int legajo = 0, int semilla=0) {
        Base = preguntas;
        Legajo = legajo;
        HoraInicio = DateTime.Now;
        Preguntas = preguntas.GenerarExamen(cantidad, semilla);
        Preguntas.ForEach( p => p.Respuesta = 0);
    }

    static bool EsNumerico(ConsoleKey key) {
        return key == ConsoleKey.D1 || key == ConsoleKey.NumPad1 ||
               key == ConsoleKey.D2 || key == ConsoleKey.NumPad2 ||
               key == ConsoleKey.D3 || key == ConsoleKey.NumPad3;
    }
    
    static bool EsAlfabetico(ConsoleKey key) {
        return key == ConsoleKey.A || key == ConsoleKey.B || key == ConsoleKey.C;
    }
    
    public int Nota() =>
        Preguntas.Count(p => p.Respuesta == p.Correcta);

    string FormatResponse(int respuesta) => 
                MostrarNumeros ? respuesta.ToString() : ((char)('a' + respuesta - 1)).ToString();
    
    public bool Evaluar() {
        int cantidad = Preguntas.Count();
        int actual = 0;
        HoraFinal = DateTime.Now;

        if (cantidad == 0) {
            WriteLine("\n🎉 Felicitaciones. Respondiste todas las preguntas. 🎉\n\n");
            return true;
        }

        while (actual < cantidad) {
            // Validar índices para evitar errores
            if (actual < 0) { actual = 0; }
            if (actual >= cantidad) { break; }

            Pregunta pregunta = Preguntas[actual];
            ConsoleKeyInfo? key = null;

            // Mostrar la pregunta completa solo cuando cambia
            Clear();
            WriteLine($"- Pregunta {actual + 1,2} de {cantidad} --- Legajo: {Legajo} - Examen 1er Parcial -");
            WriteLine();
            pregunta.Mostrar(numerico: MostrarNumeros);
            Write("Respuesta (←/→ navegar, a/b/c o 1/2/3 para responder): ");

            // Esperar una tecla sin temporizador
            key = ReadKey(true);

            var k = key.Value.Key;

            if (EsNumerico(k) || EsAlfabetico(k)) { MostrarNumeros = EsNumerico(k); }

            int respuesta = k switch {
                ConsoleKey.A or ConsoleKey.D1 or ConsoleKey.NumPad1 => 1,
                ConsoleKey.B or ConsoleKey.D2 or ConsoleKey.NumPad2 => 2,
                ConsoleKey.C or ConsoleKey.D3 or ConsoleKey.NumPad3 => 3,
                _ => 0
            };

            if (respuesta > 0) {
                Preguntas[actual].Respuesta = respuesta;
                actual++;
                continue;
            }

            switch (k) {
                case ConsoleKey.Escape or ConsoleKey.X:
                    return false;
                case ConsoleKey.LeftArrow:
                    actual--;
                    if (actual < 0) { actual = 0; }
                    break;
                case ConsoleKey.RightArrow:
                    actual++;
                    if (actual >= cantidad) { actual = cantidad - 1; }
                    break;
            }
        }

        HoraFinal = DateTime.Now;
        return true;
    }

    public bool ExamenPerfecto(){
        return Preguntas.Where(p => p.EsIncorrecta).Count() == 0;
    }

    public void ReiniciarExamen(){
        Preguntas.ForEach(p => p.Respuesta = 0);
        HoraInicio = DateTime.Now;
        HoraFinal = DateTime.Now;
    }

    public void Enseñar() {
        var incorrectas = Preguntas.Where(p => p.EsIncorrecta).ToList();
        var cantidadIncorrectas = incorrectas.Count();
        if (cantidadIncorrectas == 0) return;

        Clear();
        WriteLine($"\nHay {(cantidadIncorrectas == 1 ? "una respuesta incorrecta" : $"{cantidadIncorrectas} respuestas incorrectas")}.\n");
        if(!Confirmar("¿Desea ver las respuestas incorrectas?")) return;

        MostrarNumeros = false;
        if (incorrectas.Any()) {
            int i = 1;
            foreach(var p in incorrectas) {
                Clear();
                WriteLine($"Hay {cantidadIncorrectas} {(cantidadIncorrectas == 1 ? "respuesta incorrecta" : "respuestas incorrectas")}\n");
                WriteLine($"{i++} de {cantidadIncorrectas} --- Tu respuesta fue {FormatResponse(p.Respuesta)}) la correcta era {FormatResponse(p.Correcta)}) ---\n");
                WriteLine("\n-----------------------------------------------------------");
                p.Mostrar(solucion: true, numerico: MostrarNumeros);
                WriteLine("\n-----------------------------------------------------------\n");
                EsperarTecla();
            }
        } else {
            WriteLine("¡Todas las respuestas fueron correctas! 🎉 Felicitaciones.");
        }
    }

    // Modificar para recibir duración
    public void Informar() {
        Clear();
        var cantidad = Preguntas.Count;
        var total = Base.Count();
        var respondidas = Base.Respondidas().Count();
        var correctas   = Base.Correctas().Count();
        var incorrectas = Base.Incorrectas().Count();

        WriteLine($"""

        ### Examen 1er Parcial ###

            --- Resultado Examen ---
            
                     Nota: {Nota()} de {cantidad}
               Porcentaje: {(cantidad > 0 ? (Nota() * 100) / cantidad : 100)}%
            
            --- Evaluación Total ---

                Preguntas: {total, 3}
              Respondidas: {respondidas, 3}
                Correctas: {correctas, 3}
              Incorrectas: {incorrectas, 3} 
               Porcentaje: {(correctas * 100) / respondidas}%
             
        """);
    }
}

// --- EJECUCIÓN DEL EXAMEN ---
int GenerarSemilla() {
    // Usar GUID para obtener bytes aleatorios y generar una semilla entre 1000 y 9999
    var guidBytes = Guid.NewGuid().ToByteArray();
    int valor = BitConverter.ToInt32(guidBytes, 0);
    valor = Math.Abs(valor % 9000) + 1000;
    return valor;
}

// Genera un código de 6 dígitos hexadecimales a partir del legajo y la semilla
string GenerarCodigo(int legajo, int semilla) {
    // 4 dígitos para la semilla (hex), 2 para control (hex)
    int semilla4 = semilla & 0xFFFF; // 16 bits
    int control = ((legajo ^ semilla4) + 0xABCD) & 0xFF; // 8 bits de control simple
    return $"{semilla4:X4}{control:X2}";
}

// Valida el código y retorna la semilla si es válido, o 0 si no lo es
int ValidarCodigo(int legajo, string codigo) {
    if (string.IsNullOrWhiteSpace(codigo) || codigo.Length != 6)
        return 0;
    if (!int.TryParse(codigo[..4], System.Globalization.NumberStyles.HexNumber, null, out int semilla4))
        return 0;
    if (!int.TryParse(codigo.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out int control))
        return 0;
    int esperado = ((legajo ^ semilla4) + 0xABCD) & 0xFF;
    return control == esperado ? semilla4 : 0;
}

var preguntas = Preguntas.Cargar();
preguntas.Validar();

Clear();
WriteLine("\n\n### Examen 1er Parcial ###\n\n");

// Configuración del examen
int legajo   = ObtenerParametro(0);
int cantidad = ObtenerParametro(1, 10);
while(legajo < 55000 || legajo > 65000) {
    legajo = LeerNumero("Ingrese número de legajo: ");
} 

preguntas.CargarResultados($"{legajo}.txt");
// preguntas.InformarResultados();

int semilla = GenerarSemilla();
var examen = new Examen(preguntas, cantidad, legajo, semilla);

while(true){
    if(!examen.Evaluar()) break;

    if(examen.ExamenPerfecto()) {
        examen.Informar();
        WriteLine("\n🎉 Felicitaciones. Respondiste todas las preguntas correctamente. 🎉\n");
        if(preguntas.Count() == 10){
            WriteLine($"Su código es: {GenerarCodigo(legajo, semilla)}\n\nCompartirlo en el grupo para conseguir los creditos\n");
        }
        break;
    } else {
        preguntas.GuardarResultados($"{legajo}.txt");
        examen.Enseñar();
        if(Confirmar("¿Desea repetir el examen para conseguir los creditos?")) {
            examen.ReiniciarExamen();
        } else {
            examen.Informar();
            break;
        }
    }
};


// Fin del programa