using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var clase = Clase.Cargar();

// Ejecutar el sistema para el alumno 61956
Console.WriteLine("=== Probando sistema para alumno 61956 ===");
var alumno = clase.Buscar(61956);
if (alumno == null) {
    Console.WriteLine("No se encontró el alumno con legajo 61956");
    return;
}

Console.WriteLine($"Alumno: {alumno.NombreCompleto}");
Console.WriteLine($"Ejecutando sistema...");

var resultado = clase.EjecutarSistema(alumno.Legajo);
Console.WriteLine($"Resultado: {resultado}");

Console.WriteLine("=== Prueba completada ===");
