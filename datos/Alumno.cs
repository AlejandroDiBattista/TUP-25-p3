using TUP;


public class Alumno {
    readonly int MaxPracticos = 20;

    public int Legajo { get; private set; }
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string Telefono { get; private set; }
    public string Comision { get; private set; }    
    public int Orden { get; set; }
    public int Asistencias { get; set; } = 0;
    public int Resultado { get; set; } = 0; 

    public List<EstadoPractico> Practicos { get; set; } = new(); // Almacena el estado de los trabajos prácticos como una lista

    public Alumno(int orden, int legajo, string apellido, string nombre, string telefono, string comision, string practicos, int asistencias = 0, int resultado=0) {
        Orden    = orden;
        Legajo   = legajo;
        Apellido = apellido.Trim();
        Nombre   = nombre.Trim();
        Telefono = telefono;
        Comision = comision;
        Practicos = ConvertirStringAPracticos(practicos);
        Asistencias = asistencias;
        Resultado = resultado;
    }

    private List<EstadoPractico> ConvertirStringAPracticos(string practicosStr) {
        var lista = new List<EstadoPractico>();
        if (string.IsNullOrEmpty(practicosStr)) return lista;
        
        foreach (char c in practicosStr) {
            lista.Add(EstadoPractico.FromString(c.ToString()));
        }
        return lista;
    }

    public string PracticosToString() {
        return string.Join("", Practicos.Select(p => p.ToString()));
    }

    public void Reiniciar(){
        Practicos.Clear();
        Asistencias = 0;
    }

    public bool TieneTelefono => Telefono != "";
    public string NombreCompleto => $"{Apellido}, {Nombre}".Replace("-", "").Replace("*", "").Trim();
    public string Carpeta => $"{Legajo} - {NombreCompleto}";
    public int CantidadPresentados => Practicos.Count(p => p == EstadoPractico.Aprobado);
    public bool Abandono => Asistencias < 4 && CantidadPresentados == 0;

    public string EstadoRecuperacionTP(int practico) {
        if (practico < 1 || practico > 3) return ""; // Solo para TP1, TP2, TP3

        var estado = ObtenerPractico(practico);

        if (practico == 3) {
            // TP3: Recupera si no está aprobado O si tiene errores (Resultado < 0)
            return (estado != EstadoPractico.Aprobado) ? "Recuperar" : (Resultado < 0 ? "Corregir 🚩" : "");
        } else {
            return (estado != EstadoPractico.Aprobado) ? "Recuperar" : "";
        }
    }

    public EstadoPractico ObtenerPractico(int practico) {
        if (practico <= 0 || practico > MaxPracticos) return EstadoPractico.NoPresentado;
        if (practico > Practicos.Count) return EstadoPractico.NoPresentado;
        return Practicos[practico - 1];
    }

    public void PonerPractico(int practico, EstadoPractico estado) {
        if (practico <= 0 || practico > MaxPracticos) return;
        
        // Aseguramos que la lista tenga suficientes elementos
        while (Practicos.Count < practico) {
            Practicos.Add(EstadoPractico.NoPresentado);
        }
        
        Practicos[practico - 1] = estado;
    } 

    public bool EsTelefono(string telefono) {
        var limpio = telefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Trim();
        if (limpio.Length < 7) return false;
        return telefono.CompareTo(limpio) == 0;
    }

    public override string ToString() {
        return $"{Legajo} - {NombreCompleto} - {Telefono} - {Comision} - {PracticosToString()}";
    }

    public override int GetHashCode() {
        return Legajo.GetHashCode();
    }

    public bool Equals(Alumno? otro) {
        if (otro == null) return false;
        return Legajo == otro.Legajo;
    }
    public static Alumno Yo => new (0, 0, "Di Battista", "Alejandro", "(381) 534-3458", "", "++");
}