using TUP;

using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Diagnostics;

// CONFIGURACIÓN

public enum EstadoMateria {
    Promocionado,
    Recuperar,
    Presentar,
    Corregir,
    Abandono,
    Revisar,
}

public enum ResultadoFinal
{
    Promocionado,
    Regular,
    Libre
}

public class Alumno
{
    readonly int MaxPracticos = 20;

    public int Legajo { get; private set; }
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string Telefono { get; private set; }
    public string Comision { get; private set; }
    public int Orden { get; set; }
    public int Asistencias { get; set; } = 0;
    public int Creditos { get; set; } = 0;
    public int Parcial { get; set; } = 0;
    public int Resultado { get; set; } = 0;
    public string GitHub { get; set; } = "";
    public string Observaciones { get; set; } = "";
    public ResultadoFinal ResultadoFinal { get; set; } = ResultadoFinal.Libre;
    
    // INPUT
    // Aprobado
    // EnProgreso
    // Desaprobado
    // Error
    // NoPresentado
    // OUTPUT
    // Promocionado
    // Recuperar
    // Presentar
    // Corregir
    // Abandono
    // Revisar

    public EstadoMateria Estado
    {
        get
        {
            var estado = ObtenerPractico(6);
            if (CantidadPresentados >= 4 && estado == EstadoPractico.Aprobado) return EstadoMateria.Promocionado;
            if (estado == EstadoPractico.EnProgreso) return EstadoMateria.Recuperar;
            if (estado == EstadoPractico.NoPresentado) return EstadoMateria.Presentar;
            if (estado == EstadoPractico.Error) return EstadoMateria.Corregir;
            if (Abandono) return EstadoMateria.Abandono;

            return EstadoMateria.Revisar; // Lógica para determinar el estado de la materia
        }
    }

    public double Nota => Math.Round(Math.Min(Parcial + Math.Min(Creditos, 20), 60) / 6.0, 1);
    public string PracticosStr => string.Join("", Practicos.Select(p => p.ToString()));

    public List<EstadoPractico> Practicos { get; set; } = new(); // Almacena el estado de los trabajos prácticos como una lista

    public Alumno(int orden, int legajo, string apellido, string nombre, string telefono, string comision, string practicos, int asistencias, int creditos, int parcial, string github = "")
    {
        Orden = orden;
        Legajo = legajo;
        Apellido = apellido.Trim();
        Nombre = nombre.Trim();
        Telefono = telefono;
        Comision = comision;
        Practicos = ConvertirStringAPracticos(practicos);
        Asistencias = asistencias;
        Creditos = creditos;
        Parcial = parcial;
        GitHub = github.Trim();
    }

    private List<EstadoPractico> ConvertirStringAPracticos(string practicosStr)
    {
        var lista = new List<EstadoPractico>();
        if (string.IsNullOrEmpty(practicosStr)) return lista;

        foreach (char c in practicosStr)
        {
            lista.Add(EstadoPractico.FromString(c.ToString()));
        }
        return lista;
    }

    public void Reiniciar()
    {
        Practicos.Clear();
        Asistencias = 0;
    }

    public string TelefonoLimpio => Telefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Trim();
    public string NombreLimpio => $"{Nombre} {Apellido}".Replace("-", "").Replace("*", "").Trim();

    public bool TieneTelefono => Telefono != "";
    public string NombreCompleto => $"{Apellido}, {Nombre}".Replace("-", "").Replace("*", "").Trim();
    public string Carpeta => $"{Legajo} - {NombreCompleto}";
    public int CantidadPresentados => Practicos.Count(p => p == EstadoPractico.Aprobado);
    public bool Abandono => CantidadPresentados < 3;
    public bool Continuan => CantidadPresentados >= 3;
    // public bool Abandono  => Asistencias < 4   && CantidadPresentados == 0;
    // public bool Continuan => Asistencias >= 15 && CantidadPresentados >= 3;

    // Nueva propiedad: cuenta los prácticos no presentados, sin tomar en cuenta el último
    public int Faltantes => Practicos
                                .Select((p, i) => new { Estado = p, Indice = i })
                                .Where(x => x.Indice != 2 && x.Indice != 5)
                                .Count(x => x.Estado == EstadoPractico.NoPresentado);

    public EstadoPractico ObtenerPractico(int practico)
    {
        if (practico <= 0 || practico > MaxPracticos) return EstadoPractico.NoPresentado;
        if (practico > Practicos.Count) return EstadoPractico.NoPresentado;
        return Practicos[practico - 1];
    }

    public void PonerPractico(int practico, EstadoPractico estado)
    {
        if (practico <= 0 || practico > MaxPracticos) return;

        while (Practicos.Count < practico)
        {
            Practicos.Add(EstadoPractico.NoPresentado);
        }

        Practicos[practico - 1] = estado;
    }

    public bool EsTelefono(string telefono)
    {
        var limpio = telefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Trim();
        if (limpio.Length < 7) return false;
        return telefono.CompareTo(limpio) == 0;
    }

    public override string ToString()
    {
        return $"{Legajo} - {NombreCompleto} - {Telefono} - {Comision} - {PracticosStr} - {Asistencias} - {Parcial} - {Nota}";
    }

    public override int GetHashCode()
    {
        return Legajo.GetHashCode();
    }

    public bool Equals(Alumno? otro)
    {
        if (otro == null) return false;
        return Legajo == otro.Legajo;
    }

    public static Alumno Yo => new(0, 0, "Di Battista", "Alejandro", "(381) 534-3458", "", "++", 0, 0, 0);
}
