namespace TP4_61450;

public interface IAlumno
{
    public int AlumnoId { get; set; }
    public int Legajo { get; set; }
    public string Nombre { get; set; }
    public int CantidadRespondidas { get; set; }
    public int CantidadCorrectas { get; set; }
    public int CantidadIncorrectas => CantidadRespondidas - CantidadCorrectas;
    public float Promedio =>
        CantidadRespondidas > 0 ? (CantidadCorrectas / (float)CantidadRespondidas) * 10 : 0;
    public ICollection<Examen> Examenes { get; set; }
}

public class Alumno : IAlumno
{
    public int AlumnoId { get; set; }
    public required int Legajo { get; set; }
    public required string Nombre { get; set; }
    public int CantidadRespondidas { get; set; } = 0;
    public int CantidadCorrectas { get; set; } = 0;
    public int CantidadIncorrectas => CantidadRespondidas - CantidadCorrectas;
    public float Promedio =>
        CantidadRespondidas > 0 ? (CantidadCorrectas / (float)CantidadRespondidas) * 10 : 0;
    public ICollection<Examen> Examenes { get; set; } = new List<Examen>();

    public static Alumno ObtenerOCrear()
    {
        int legajo = int.Parse(Utilidades.ObtenerEntrada("Ingrese el legajo del alumno: "));

        Alumno alumno;
        using (DatosContexto db = new DatosContexto())
        {
            alumno = db.Alumnos.FirstOrDefault(a => a.Legajo == legajo);

            // Si el alumno no existe, crear uno nuevo
            if (alumno == null)
            {
                string nombre = Utilidades.ObtenerEntrada("Ingrese el nombre del alumno: ");

                alumno = Alumno.Crear(legajo, nombre);
                if (alumno == null)
                {
                    Console.WriteLine("Error al crear el alumno.");
                    return null;
                }
            }
        }
        return alumno;
    }

    public static Alumno Crear(int legajo, string nombre)
    {
        using (DatosContexto db = new DatosContexto())
        {
            // Verificar si el legajo es válido
            if (legajo <= 0)
            {
                Console.WriteLine("El legajo debe ser un número positivo.");
                return null;
            }

            // Verificar si el legajo ya existe
            if (db.Alumnos.Any(a => a.Legajo == legajo))
            {
                Console.WriteLine("El legajo ya existe.");
                return null;
            }

            // Verificar si el nombre es válido
            if (string.IsNullOrWhiteSpace(nombre))
            {
                Console.WriteLine("El nombre no puede estar vacío.");
                return null;
            }

            using var transaccion = db.Database.BeginTransaction();
            try
            {
                // Crear un nuevo alumno
                Alumno nuevo = new Alumno { Legajo = legajo, Nombre = nombre };
                db.Alumnos.Add(nuevo);
                db.SaveChanges();

                transaccion.Commit();
                return nuevo;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                Console.WriteLine("Error al crear el alumno.");
                transaccion.Rollback();
                return null;
            }
        }
    }
}
