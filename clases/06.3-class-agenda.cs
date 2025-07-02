using static System.Console;
using System.IO;


class Persona {
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }

    private int _edad;
    public int Edad {
        get { return _edad; }
        // Ejemplo de validación: Solo se puede aumentar la edad
        set { if (value > _edad) _edad = value; }
    }
    
    public Persona(string nombre, string apellido, int edad) {
        Id = GenerarId();
        Nombre = nombre;
        Apellido = apellido;
        Edad = edad;
    }
    
    public string NombreCompleto {
        get { return $"{Nombre} {Apellido}"; }
    }

    public void CumplirAño() {
        Edad += + 1;
    }
    
    public override string ToString() {
        return $"{NombreCompleto} tiene {Edad} años";
    }
    
    // Gestiona el ID de las personas 
    static int proximoId = 1000;
    static int GenerarId() {
        return proximoId++;
    }
}

// Una agenda es una colección de personas.
class Agenda {
    int cantidad;
    Persona[] personas;

    public Agenda(int maximo = 3) {
        cantidad = 0;
        personas = new Persona[maximo];
    }

    public void Agregar(Persona p) {
        // Si se alcanza la capacidad, se aumenta
        if (cantidad == personas.Length) {
            AumentarCapacidad(2);
        }
        personas[cantidad++] = p;
    }

    private void AumentarCapacidad(int incremento = 2) {
        // Array.Resize(ref personas, personas.Length + incremento);
        var copia = new Persona[personas.Length + incremento];
        for (var i = 0; i < cantidad; i++) {
            copia[i] = personas[i];
        }
        personas = copia;
    }

    public void Borrar(int id) {
        for (var i = 0; i < cantidad; i++) {
            if (personas[i].Id == id) { // Ubica a la persona
                // Array.Copy(personas, i + 1, personas, i, cantidad - i - 1);
                for (var j = i; j < cantidad - 1; j++) { 
                    personas[j] = personas[j + 1];
                }
                cantidad--;
                break;
            }
        }
    }
    
    public void Mostrar() {
        WriteLine($"\nLista de personas ({cantidad} de {personas.Length})");
        for (var i = 0; i < cantidad; i++) {
            WriteLine($" {i + 1}. {personas[i],-30} #{personas[i].Id}");
        }
    }
    
    // Guarda el archivo en formato CSV (sin Id)
    public void Guardar(string filePath) {
        string[] lines = new string[cantidad + 1]; // +1 para la cabecera
        lines[0] = "Nombre,Apellido,Edad"; // Cabecera
        for (var i = 0; i < cantidad; i++) {
            var p = personas[i];
            lines[i + 1] = $"{p.Nombre},{p.Apellido},{p.Edad}";
        }
        File.WriteAllLines(filePath, lines);
    }
    
    // Lee un archivo CSV y agrega las personas encontradas
    public static Agenda Cargar(string filePath) {
        if (!File.Exists(filePath))
            return new Agenda();
            
        string[] lines = File.ReadAllLines(filePath);
        
        var agenda = new Agenda();
        for (var i = 1; i < lines.Length; i++) {
            var partes = lines[i].Split(',');
            string nombre   = partes[0];
            string apellido = partes[1];
            int edad        = int.Parse(partes[2]);
            agenda.Agregar(new Persona(nombre, apellido, edad));
        }
        return agenda;
    }
}

// Ejemplo de uso 
Clear();
var a = new Agenda();
a.Agregar(new Persona("Juan", "Pérez", 30));
a.Mostrar();

a.Agregar(new Persona("Maria", "Gomez", 25));
a.Agregar(new Persona("Pedro", "Ramirez", 40));
a.Mostrar();

a.Agregar(new Persona("Ana", "Lopez", 20));
a.Mostrar();
a.Agregar(new Persona("Luis", "Martinez", 35));
a.Mostrar();


// Guarda la agenda en un archivo CSV
string archivo = "./agenda.csv";
a.Guardar(archivo);

// La carga desde el archivo en una nueva agenda
var b = Agenda.Cargar(archivo); // Usar funcion estatica que crea una nueva agenda
b.Mostrar();
