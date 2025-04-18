using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

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

// Clase Pregunta
public class Pregunta {
    public int Numero { get; set; } = 0;
    public string Texto { get; set; } = "";
    public string[] Respuestas { get; set; } = new string[3];
    public int Correcta { get; set; } = 0;
    public int Respuesta { get; set; } = 0;
    
    public bool EsRespondida => Respuesta != 0;
    public bool EsIncorrecta => Respuesta != Correcta;
    public bool EsCorrecta   => Respuesta == Correcta;
    
    public override string ToString() =>
        $"""
    
        ### {Numero:D3} 
        {Texto}
        
        a) {Respuestas[0]}
        b) {Respuestas[1]}
        c) {Respuestas[2]}
        
        """;
    
    // Reemplaza el Mostrar() anterior
    public void Mostrar(bool numerico = false, bool solucion = false){
        string f(string texto) => texto.Replace("```csharp", "").Replace("```", "").Replace("`", "");

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
        var lineas = System.IO.File.ReadAllLines(Origen);
        var respuesta = -1;

        Pregunta pregunta = null;
        foreach (var linea in lineas) {

            if(linea.StartsWith("# ")) continue;
            if(linea.Trim() == "") continue;
            if(linea.Trim() == "---") continue;

            if(linea.StartsWith("###")) {
                if(pregunta != null) Lista.Add(pregunta);
                
                pregunta = new Pregunta();
                pregunta.Numero = int.Parse(linea.Split(" ")[1]);
                respuesta = -1;

                continue;
            };

            if (linea.StartsWith("a) ") || linea.StartsWith("b) ") || linea.StartsWith("c) ")) {
                respuesta = linea[0] - 'a';
                pregunta.Respuestas[respuesta] = linea.Substring(3);
                continue;
            }

            if(respuesta < 0){
                pregunta.Texto += "\n" + linea;
            } else {
                pregunta.Respuestas[respuesta] += "\n" + linea;
            }
        }
        
        Lista.Add(pregunta);
    }   

    public void CargarRespuestas(string origen){
        var lineas = System.IO.File.ReadAllLines(origen);
        foreach (var linea in lineas) {
            if(!linea.Contains(". ")) continue;

            var partes = linea.Split(". ");
            var numero = int.Parse(partes[0]);
            var respuesta = partes[1][^1] - 'a' + 1;

            var pregunta = this.FirstOrDefault(p => p.Numero == numero);
            if(pregunta == null) continue;
            pregunta.Correcta = respuesta;
        }
    }

    public void CargarResultados(string origen) {
        if (!File.Exists(origen)) return;

        var lineas = File.ReadAllLines(origen);
        foreach (var linea in lineas)
        {
            if (!linea.Contains(". ")) continue;

            var partes = linea.Split(new[] {". "}, 2, StringSplitOptions.None);
            if (!int.TryParse(partes[0], out var numero)) continue;

            // Remover posible '*' al final y tomar la letra de respuesta
            var letra = partes[1].TrimEnd('*').Trim();
            if (string.IsNullOrEmpty(letra)) continue;

            var respuestaChar = letra[^1];
            var respuestaNum = respuestaChar - 'a' + 1;

            var pregunta = this.FirstOrDefault(p => p.Numero == numero);
            if (pregunta != null) {
                pregunta.Respuesta = respuestaNum;
            }
        }
    }

    public void GuardarPreguntas(string destino){
        using var sw = new StreamWriter(destino);
        
        sw.WriteLine("# Preguntas para el 1er Parcial");
        foreach (var pregunta in this) {
            sw.WriteLine(pregunta.ToString());
            sw.WriteLine("---");
        }
        sw.Close();
    }

    public void GuardarRespuestas(string destino){
        using var sw = new StreamWriter(destino);
        foreach (var pregunta in this) {
            sw.WriteLine($"{pregunta.Numero:D3}. {(char)(pregunta.Correcta + 'a' - 1)}");
        }
        sw.Close();
    }

    public void ValidarNumeracion() {
        int n = 1;
        foreach (var p in Lista) {
            while (n < p.Numero) {
                WriteLine($"Falta {n}");
                n++;
            }
            n++;
        }
    }

    // Implementación de IEnumerable<Pregunta>
    public IEnumerator<Pregunta> GetEnumerator() => Lista.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<Pregunta> Pendientes() => this.Where(p => !p.EsRespondida);
    public IEnumerable<Pregunta> Respondidas() => this.Where(p => p.EsRespondida);
    public IEnumerable<Pregunta> Correctas() => this.Where(p => p.EsRespondida && p.EsCorrecta);
    public IEnumerable<Pregunta> Incorrectas() => this.Where(p => p.EsRespondida && p.EsIncorrecta);
    public int Count => Lista.Count;
}


// Clase Examen
class Examen {
    public List<Pregunta> Preguntas { get; set; } = new();
    public bool MostrarNumeros { get; set; } = false;
    private Preguntas Base { get; set; }

    // Ajustar firma para recibir diccionario de respuestas previas
    public Examen(Preguntas preguntas, int cantidad){
        Base = preguntas;
        
        var origen = cantidad < 0 ? preguntas.Incorrectas() : preguntas.Pendientes();
        cantidad = Math.Abs(cantidad);
        cantidad = origen.Count() < cantidad ? origen.Count() : cantidad;
        if (cantidad == 0) {
            WriteLine("No hay preguntas pendientes para responder.");
            return;
        }
        
        Random random = new Random();
        Preguntas = origen
            .OrderBy(x => random.Next())
            .Take(cantidad)
            .OrderBy(p => p.Numero)
            .ToList();
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

    public void GuardarRespuestas(string destino) {
        using var sw = new StreamWriter(destino);
        foreach (var p in Base.Respondidas().OrderBy(p => p.Numero)) {
            var mark = p.EsCorrecta ? "" : "*";
            sw.WriteLine($"{p.Numero:D3}. {(char)(p.Respuesta + 'a' - 1)}{mark}");
        }
    }   

    // Cambiar firma para recibir legajo
    public void Evaluar(int legajo){
        string FormatResponse(int responseNum) => 
                    MostrarNumeros 
                        ? responseNum.ToString() 
                        : ((char)('a' + responseNum - 1)).ToString();
        
        // clonar lista original para conservar todas las preguntas
        var todas = Preguntas.ToList();
        
        int cantidad = Preguntas.Count();  // <-- contar preguntas de esta sesión

        int actual = 0;
        bool abortar = false;

        while (actual < Preguntas.Count()) {
            Clear();
            WriteLine($"- Pregunta {actual + 1,2} de {Preguntas.Count} --------------------------------------- Legajo: {legajo} - Examen 1er Parcial -\n");
            // Mostrar opción usando el nuevo Mostrar
            Preguntas[actual].Mostrar(numerico: MostrarNumeros);
            if (Preguntas[actual].Respuesta != 0) {
                WriteLine($"\nRespuesta: {FormatResponse(Preguntas[actual].Respuesta)}\n");
            } else {
                WriteLine("\n\n\n");
            }
            Write("Respuesta (←/→ navegar, a/b/c o 1/2/3 para responder): ");
            var key = ReadKey(true);
            
            var k = key.Key;
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

        if (abortar) return;              // no mostrar ni grabar si se abortó

        Clear();
        WriteLine();
        var incorrectas = Preguntas.Where(p => p.EsIncorrecta);
        if (incorrectas.Any()) {
            WriteLine($"Hay respuestas incorrectas: {incorrectas.Count()}\n");
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
            WriteLine("¡Todas las respuestas fueron correctas! Felicitaciones.");
        }
        Informar();
    }

    private void Informar(){
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
    }

    // private void EsperarTecla(){
    //     Write("Presione una tecla para continuar...");
    //     ReadKey(true);
    //     Write($"\r{' ',40}\r");
    // }
}


// --- EJECUCIÓN DEL EXAMEN ---

Clear();
WriteLine("\n\n### Examen 1er Parcial ###\n\n");

int legajo = ObtenerParametro(0);
int cantidad = ObtenerParametro(1, 10);

while(legajo < 55000 || legajo > 65000) {
    legajo = LeerNumero("Ingrese número de legajo: ");
} 

var preguntas = new Preguntas();
preguntas.CargarPreguntas("./preguntas-examen.md");
preguntas.CargarRespuestas("./respuestas-examen.md");
preguntas.CargarResultados($"./{legajo}.txt");

var examen = new Examen(preguntas, cantidad);
examen.Evaluar(legajo);
examen.GuardarRespuestas($"./{legajo}.txt");



// Fin del programa