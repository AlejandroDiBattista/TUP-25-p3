namespace TUP;

public static class Consola {
    public static void Limpiar() {
        Console.Clear();
    }

    // Helper method to print colored text and reset color afterwards
    public static void Escribir(string text, ConsoleColor color = ConsoleColor.White) {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
    
    // Helper method to wait for a key press
    public static void EsperarTecla(string mensaje = "\nPresione cualquier tecla para continuar...") {
        Console.Write(mensaje);
        Console.ReadKey(true);
        Console.WriteLine();
    }
    
    // Helper method to read a string input with optional validation
    public static string LeerCadena(string mensaje = "", string[]? valoresPosibles = null) {
        string entrada;
        bool entradaValida;
        
        do {
            Console.Write(mensaje ?? "");
            entrada = Console.ReadLine() ?? "";
            
            // Si no hay valores posibles específicos, cualquier entrada es válida
            if (valoresPosibles == null || valoresPosibles.Length == 0) {
                entradaValida = true;
            } else {
                entradaValida = valoresPosibles.Contains(entrada);
                if (!entradaValida) {
                    Console.WriteLine($"Entrada no válida. \nOpciones permitidas: {string.Join(", ", valoresPosibles)}");
                }
            }
        } while (!entradaValida);
        
        return entrada;
    }
    
    
    // Helper method to get a valid option from a range
    // Método para elegir una opción de un conjunto de valores posibles
    public static int ElegirOpcion(string mensaje, int maximo)
    {
        int resultado = -1;
        Console.Write(mensaje);
        do
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                resultado = 0;
            }
            else if (char.IsDigit(keyInfo.KeyChar))
            {
                resultado = int.Parse(keyInfo.KeyChar.ToString());
            }
        } while (resultado < 0 || resultado > maximo);

        Console.WriteLine(resultado);
        return resultado;
    }
    
    public static bool Confirmar(string mensaje) {
        ConsoleKeyInfo key;
        while(true) {
            Console.Write($"{mensaje} (S/N): ");
            key = Console.ReadKey(true); // true hides the pressed key
            char keyChar = char.ToUpper(key.KeyChar);
            Console.WriteLine(keyChar); // Show which key was pressed
            
            if (keyChar != 'S' && keyChar != 'N') {
                Escribir("Por favor, presione S o N.", ConsoleColor.Red);
                continue;
            }
            return keyChar == 'S';
        }
    }

    // Método para leer un entero con validación opcional de valores permitidos
    public static int LeerEntero(string mensaje = "", int[]? valoresPosibles = null) {
        int resultado;
        bool entradaValida;
        do {
            Console.Write(mensaje ?? "");
            string entrada = Console.ReadLine() ?? "";
            entradaValida = int.TryParse(entrada, out resultado);
            if (!entradaValida) {
                Escribir("Por favor, ingrese un número entero válido.", ConsoleColor.Red);
                continue;
            }
            if (valoresPosibles != null && valoresPosibles.Length > 0) {
                entradaValida = valoresPosibles.Contains(resultado);
                if (!entradaValida) {
                    Escribir($"Entrada no válida. Opciones permitidas: {string.Join(", ", valoresPosibles)}", ConsoleColor.Red);
                }
            }
        } while (!entradaValida);
        return resultado;
    }
}
