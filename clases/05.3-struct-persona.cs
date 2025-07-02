using static System.Console;

static void MostrarError(string mensaje){
    BackgroundColor = ConsoleColor.Red;
    ForegroundColor = ConsoleColor.Yellow;
    WriteLine($" ERROR: {mensaje} ");
    ResetColor();
}


class Persona{
    public string Nombre { get;  set; }  // Propiedad automática
    public string Apellido { get; set; }
    int edad; // Campo privado 


    public Persona(string nombre, string apellido, int edad){
        Nombre = nombre;
        Apellido = apellido;
        Edad = edad;  // Observe que almacena una un propiedad
    }

    public void Mostrar(){
        WriteLine($"\nNombre: {Nombre} Apellido: {Apellido}, Edad: {Edad}\n");
    }

    public string NombreCompleto {
        get {
            return $"{Apellido}, {Nombre} ";
        }

        set {
            var partes = value.Split(',');
            if (partes.Length != 2) {
                MostrarError("Nombre completo no válido");
                return;
            }
            Apellido = partes[0].Trim();
            Nombre = partes[1].Trim();
        }
    }
    public int Edad {
        get {
            return edad;
        }
        set {
            if (value <= 0) {
                MostrarError("Edad no válida (debe ser mayor a 0)");
                return;
            }
            edad = value;
        }
    }

    // Sobreescribe el método ToString() para mostrar la información 
    // de la persona en un formato legible.
    public override string ToString(){
        return $"{NombreCompleto} ({Edad} años)";
    }
}

Clear();
// Creamos una nueva instancia de la clase Persona
var a = new Persona("Juan", "Pérez", 30);   
a.Mostrar();

// Intentemas asignar una edad negativa
a.Edad = -100; // No se asigna por ser menor a 0
a.Mostrar(); 

// Podemos asignar una edad válida
a.Edad = 25;            // Asigna la edad
a.Edad = a.Edad + 1;    // Incrementamos la edad en 1 año (Cumpleaños)
a.Edad += 1;            // iden anterior
a.Edad++;               // idem anterior
a.Mostrar();

// Cambiamos el nombre completo
WriteLine($"  Nombre completo: {a.NombreCompleto}");
WriteLine($"  Edad: {a.Edad}");

a.NombreCompleto = "Lopez, Clotilde"; // Asigna nombre completo

WriteLine("\nCambio `NombreCompleto` y se modifica `Nombre` y `Apellido`");
WriteLine($"  Nombre completo: {a.NombreCompleto}");
WriteLine($"  Apellido: {a.Apellido} \n  Nombre: {a.Nombre}\n");

a.Nombre = "Juan";
WriteLine("\nCambio `Nombre` y se modifica `NombreCompleto`");
WriteLine($"  Nombre completo: {a.NombreCompleto}");
WriteLine($"  Apellido: {a.Apellido} Nombre: {a.Nombre}");

WriteLine("-------------------------");
WriteLine(a.ToString());

void demoConversionString(){
    /// Ejemplo de como usar el método ToString()
    /// para mostrar la información de la persona

    var x0 = "Hola " + 1.ToString(); 
    // En el contexto de necesitar un string aplica automaticamente `ToString()`
    var x1 = "Hola " + 1;               

    var x2 = "Hola " + a.ToString();    // Forma explicita
    var x3 = "Hola " + a;               // Llama automaticamente ToString()
    var x4 = $"Hola {a}";               // Llama automaticamente ToString()

    WriteLine($"X0 es {x0}");
    WriteLine($"X1 es {x1}");
    WriteLine($"X2 es {x2}");
    WriteLine($"X3 es {x3}");
    WriteLine($"X4 es {x4}");
    WriteLine("Persona es {a}");
}

void demoCopiaPorValor(){
// Son tipo por valor
    var b = a; // Realmente copia todos los valores
    WriteLine("-------------------------");
    WriteLine("B es una copia de A");
    WriteLine($"  Persona A: {a}");
    WriteLine($"  Persona B: {b}");
 
    b.Nombre = "Maria";
    WriteLine("\nCambio `Nombre` de B pero no modifica A");
    WriteLine($"  Persona A: {a}");
    WriteLine($"  Persona B: {b}");
}