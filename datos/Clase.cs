using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using TUP;

class Clase : IEnumerable<Alumno> {
    public List<Alumno> alumnos = new List<Alumno>();
    const string LineaComision = @"##.*\s(?<comision>C\d)";
    // const string LineaAlumno = @"(?<index>\d+)\.\s*(?<legajo>\d{5})\s*(?<nombre>[^,]+)\s*,\s*(?<apellido>[^(]+)(?:\s*(?<telefono>\(\d+\)\s*\d+(?:[- ]\d+)*))?";
    const string LineaAlumno = @"(?<index>\d+)\.\s+(?<legajo>\d{5})\s+(?<nombre>[^,]+)";

    public List<string> Comisiones => alumnos.Select(a => a.Comision).Distinct().OrderBy(c => c).ToList();
    public IEnumerable<Alumno> Alumnos => alumnos.OrderBy(a => a.NombreCompleto).ToList();

    public Clase(IEnumerable<Alumno>? alumnos = null) {
        this.alumnos = alumnos?.ToList() ?? new List<Alumno>();
    }
    public static Clase Cargar(string origen="./alumnos.md") {
        string comision = "C0";
        Clase clase = new();

        Console.Clear();
        foreach (var linea in File.ReadLines(origen)) {
            var texto = linea.Trim();
            var matchComision = Regex.Match(texto, LineaComision);
            if (matchComision.Success) {
                comision = matchComision.Groups["comision"].Value;
                continue;
            } 

            var matchAlumno = Regex.Match(texto, LineaAlumno);
            if (matchAlumno.Success) {
                Console.WriteLine(texto);
                var partes = Regex.Split(linea, @"\s{2,}"); // Split on 2 or more spaces
                var index       = int.Parse(partes[0].TrimEnd('.'));
                var legajo      = int.Parse(partes[1]);
                var nombre      = partes[2].Trim();
                var telefono    = partes[3].Trim();
                var asistencias = int.Parse(partes[4]);
                var practicos   = partes[5].Trim();
                var creditos    = int.Parse(partes[6]);
                var parcial     = int.Parse(partes[7]);
                // Alumno(int orden, int legajo, string apellido, string nombre, string telefono, string comision, string practicos, int asistencias = 0, int resultado=0, int notas=0) {

                Alumno alumno = new Alumno(
                    index,
                    legajo,
                    nombre.Split(", ")[0].Trim(),
                    nombre.Split(", ")[1].Trim(),
                    telefono,
                    comision,
                    practicos,
                    asistencias,
                    creditos,
                    parcial
                );

                clase.alumnos.Add(alumno);
                continue;
            } 
        }
        
        return clase;
    }

    // Métodos de filtrado
    public Clase EnComision(string comision) => new (alumnos.Where(a => a.Comision == comision));
    public Clase ConTelefono(bool incluirTelefono=true) => new (alumnos.Where(a => incluirTelefono == a.TieneTelefono ));
    public Clase ConAbandono(bool abandono) => new (alumnos.Where(a => a.Abandono == abandono));
    public Clase ConNombre(string nombre) => new (alumnos.Where(a => a.NombreCompleto.Contains(nombre, StringComparison.OrdinalIgnoreCase)));
    public Clase ConPractico(int numero, EstadoPractico estado) => new (alumnos.Where(a => a.ObtenerPractico(numero) == estado));
    public Clase ConAusentes(int cantidad) => new(Alumnos.Where(a => a.Practicos.Count(p => p == EstadoPractico.NoPresentado) >= cantidad));
    public Clase ConAprobados(int cantidad) => new(Alumnos.Where(a => a.Practicos.Count(p => p == EstadoPractico.Aprobado) >= cantidad));
    public Clase OrdenandoPorNombre() => new (alumnos.OrderBy(a => a.Apellido).ThenBy(a => a.Nombre));
    public Clase OrdenandoPorLegajo() => new (alumnos.OrderBy(a => a.Legajo));
    
    // Métodos de modificación
    public void Agregar(Alumno alumno) {
        if (alumno != null) {
            alumnos.Add(alumno);
        }
    }   

    public void Agregar(IEnumerable<Alumno> alumnos) {
        foreach(var alumno in alumnos) {
            Agregar(alumno);
        }
    }

    public void Guardar(string destino="./alumnos.md") {
        using (StreamWriter writer = new(destino)) {
            writer.WriteLine("# Listado de alumnos");
            foreach(var comision in Comisiones) {
                var orden = 1;
                writer.WriteLine($"\n## Comisión {comision}");
                foreach(var a in EnComision(comision).OrdenandoPorNombre()) {
                    a.Orden = orden++;
                    var linea = $"{a.Orden:D2}.  {a.Legajo,5}  {a.NombreCompleto,-35}  {a.Telefono,-14}   {a.Asistencias,2}  {a.PracticosStr,-10}  {a.Creditos,2}  {a.Parcial,2}  {a.Nota,4:F1}";
                    writer.WriteLine(linea);
                }
            }
        }
    }

    public void GuardarVCards(string destino) {
        Consola.Escribir($"- Exportando vCards a {destino}");
        using (StreamWriter writer = new(destino)) {
            foreach(var alumno in ConTelefono(true).OrdenandoPorNombre()) {
                var linea = $"""
                BEGIN:VCARD
                VERSION:3.0
                N:{alumno.Apellido};{alumno.Nombre};;;
                FN:{alumno.Nombre} {alumno.Apellido}
                ORG:TUP-25-P3-{alumno.Comision}
                TEL;TYPE=CELL:{alumno.Telefono}
                TEL;TYPE=Legajo:{alumno.Legajo}
                END:VCARD
                """;
                writer.WriteLine(linea);
            }
        }
    }
    
    public void NormalizarCarpetas() {
        const string Base = "../TP";
        Directory.CreateDirectory(Base);

        Consola.Escribir($"▶︎ Creando carpetas en {Path.GetFullPath(Base)}", ConsoleColor.Green);
        var cambios = 0;
        foreach (var alumno in OrdenandoPorLegajo() ) {
            var carpetaDeseada = alumno.Carpeta;
            var rutaCompleta = Path.Combine(Base, carpetaDeseada);

            if (Directory.Exists(rutaCompleta)) continue;
            
            var carpetasExistentes = Directory.GetDirectories(Base, $"{alumno.Legajo}*");

            if (carpetasExistentes.Length > 0) {
                var carpetaEncontrada = carpetasExistentes[0];
                if (Path.GetFileName(carpetaEncontrada) != carpetaDeseada) {
                    Directory.Move(carpetaEncontrada, rutaCompleta);
                    Consola.Escribir($"  - Renombrando carpeta:\n  <[{Path.GetFileName(carpetaEncontrada)}]\n  >[{carpetaDeseada}]", ConsoleColor.Yellow);
                    cambios++;
                }
            } else {
                Directory.CreateDirectory(rutaCompleta);
                Consola.Escribir($"  - Creando carpeta:\n  >[{carpetaDeseada}]", ConsoleColor.Yellow);
                cambios++;
            }
        }
        Consola.Escribir($"● {cambios} carpetas cambiadas", ConsoleColor.Green);
    }

    public static int ContarLineasEfectivas(string archivo) {
        var lineas = File.ReadAllLines(archivo).TakeWhile(linea => !linea.Contains("PRUEBAS AUTOMATIZADAS"));
        return lineas.Count(linea =>
            !linea.Trim().Equals("") &&                     // No es una línea vacía
            !linea.TrimStart().StartsWith("Console.") &&    // No es un mensaje de consola
            !linea.TrimStart().StartsWith("using") &&       // No es una directiva using
            !linea.TrimStart().StartsWith("//") &&          // No es un comentario
            !linea.Trim().Equals("{") &&                       // No es solo una llave de apertura
            !linea.Trim().Equals("}")                       // No es solo una llave de cierre
        );
    }


    public void VerificaPresentacionPractico(int practico) {
        const string Base = "../TP";
        
        var fuente = practico == 4 ? "Program.cs" : "ejercicio.cs";

        Consola.Escribir($"\n=== Verificación de presentación del trabajo práctico TP{practico} ===", ConsoleColor.Blue);
        var enunciado = Path.Combine("../enunciados", $"tp{practico}", fuente);
        int lineas = ContarLineasEfectivas(enunciado);
        Consola.Escribir($" - Enunciado tiene {lineas} líneas efectivas", ConsoleColor.Cyan);
        foreach(var comision in Comisiones) {
            var presentados = 0;
            var ausentes = 0;
            var errores = 0;
            foreach(var alumno in EnComision(comision)){
                var archivo = Path.Combine(Base, alumno.Carpeta, $"tp{practico}", fuente);
                EstadoPractico estado = EstadoPractico.Error;
                if (File.Exists(archivo)) {
                    int lineasEfectivas = ContarLineasEfectivas(archivo) - lineas;
                    
                    estado = lineasEfectivas >= 20  ? EstadoPractico.Aprobado : EstadoPractico.NoPresentado;
                    if (estado == EstadoPractico.Aprobado) {
                        presentados++;
                    }
                    if (estado == EstadoPractico.NoPresentado) {
                        ausentes++;
                    }
                    alumno.PonerPractico(practico, estado);
                    
                    if(practico == 3 && lineasEfectivas > 20) { // Si es TP3, ejecutar el programa y verificar el resultado
                        alumno.Resultado = ResultadoEjecutar(archivo);
                    }
                    
                    var color = lineasEfectivas < 20 ? ConsoleColor.Yellow : ConsoleColor.White;
                    if(alumno.Resultado < 0){
                        errores++;
                        color = ConsoleColor.Red;
                    }
                    if(alumno.Resultado > 0 ){
                        color = ConsoleColor.Green;
                    }
                    Consola.Escribir($" - {alumno.Legajo}  {alumno.NombreCompleto, -60}  {lineasEfectivas, 3} {estado}", color);
                }
            }
            Consola.Escribir($"Comisión {comision} \n Presentados: {presentados,3}\n Ausentes   : {ausentes,3}\n Errores:     {errores,3}", ConsoleColor.Cyan);
        }
    }
    
    public static string EjecutarCSharp(string origen){
        var runInfo = new ProcessStartInfo{
            FileName = "dotnet", Arguments = $"script \"{origen}\"",
            RedirectStandardOutput = true, RedirectStandardError = true,
            UseShellExecute = false,       CreateNoWindow = true
        };

        using (var runProcess = Process.Start(runInfo) ?? throw new InvalidOperationException("Failed to start process")){
            string salida = runProcess.StandardOutput.ReadToEnd();
            string error  = runProcess.StandardError.ReadToEnd();
            runProcess.WaitForExit();
            return salida + error;
        }
    }

    public static int ResultadoEjecutar(string origen){
        var salida = EjecutarCSharp(origen);
        if (salida.Contains(": error")) {
            return - salida.Split("\n").Count(line => line.Contains(": error"));
        } else {
            return salida.Split("\n").Count(line => line.Contains("[OK]"));
        }
    }

    public void CopiarPractico(int practico, bool forzar=false) {
        const string Base = "../TP";
        const string Enunciados = "../enunciados";
        Consola.Escribir($" ▶︎ Copiando trabajo práctico de TP{practico}", ConsoleColor.Cyan);
        var carpetaOrigen = Path.Combine(Enunciados, $"tp{practico}");
        
        if (!Directory.Exists(carpetaOrigen)) {
            Consola.Escribir($"Error: No se encontró el enunciado del trabajo práctico '{practico}' en {carpetaOrigen}", ConsoleColor.Red);
            return;
        }

        foreach (var alumno in Alumnos.OrderBy(a => a.Legajo)) {
            var carpetaDestino = Path.Combine(Base, alumno.Carpeta, $"tp{practico}");
            if(forzar && Directory.Exists(carpetaDestino)) {
                Directory.Delete(carpetaDestino, true);
            }
            Directory.CreateDirectory(carpetaDestino);

            Consola.Escribir($" - Copiando a {carpetaDestino}", ConsoleColor.Yellow);
            foreach (var archivo in Directory.GetFiles(carpetaOrigen)) {
                var nombreArchivo = Path.GetFileName(archivo);
                if(nombreArchivo == "enunciado.md") continue;
                
                var destinoArchivo = Path.Combine(carpetaDestino, nombreArchivo);
                if (!File.Exists(destinoArchivo)) {
                    File.Copy(archivo, destinoArchivo);
                }
            }
        }
        Consola.Escribir($" ● Copia de trabajo práctico completa", ConsoleColor.Green);
    }
    
    public void ExportarDatos() {
        Consola.Escribir($" ▶︎ Generando listado de alumnos (hay {alumnos.Count()} alumnos.)", ConsoleColor.Cyan);
        foreach(var comision in Comisiones) {
            var alumnosComision = ConTelefono(true).EnComision(comision);
            alumnosComision.GuardarVCards( $"./alumnos-{comision}.vcf");
        }
        ConTelefono(true).GuardarVCards("./alumnos.vcf");
        Guardar("./resultados.md");
        Consola.Escribir($" ● Exportación completa", ConsoleColor.Green);
    }

    public void ListarAlumnos(){
        foreach(var comision in Comisiones){
            Consola.Escribir($"\n=== Comisión {comision} ===", ConsoleColor.Blue);    
            foreach (var alumno in EnComision(comision).OrdenandoPorNombre()) {
                var emojis = alumno.Practicos.Select(p => p.Emoji).ToList();
                if(alumno.Resultado < 0){
                    emojis[2] = "🔴";
                }
                var asistencia = string.Join("", emojis);
                string linea = $"{alumno.Legajo} - {alumno.NombreCompleto, -40} {$"{alumno.Telefono}", -15}";
                linea = $" {linea,-65} {alumno.Asistencias, 2}  {asistencia}  {alumno.Nota, 4:0.0}";

                Consola.Escribir(linea);
            }
            Consola.Escribir($"Total alumnos en comisión {comision}: {EnComision(comision).Count()}", ConsoleColor.Yellow);
        }
        Consola.Escribir($"\nTotal general de alumnos: {alumnos.Count}", ConsoleColor.Green);
    }

    private void ListarPorComision(IEnumerable<Alumno> listado, string comision, string mensaje) {
        if (!listado.Any()) { 
            Consola.Escribir($"No hay {mensaje} en la comisión {comision}", ConsoleColor.Green); }
        else {
            Consola.Escribir($"\n=== Comisión {comision} ===", ConsoleColor.Blue);
            Consola.Escribir("```");        
            foreach (var alumno in listado) { 
                Consola.Escribir($"{alumno.Legajo} {alumno.NombreCompleto,-32} {alumno.Asistencias,2} {alumno.CantidadPresentados,2}", ConsoleColor.Red); 
            }
            Consola.Escribir("```");        
            Consola.Escribir($"Total {mensaje}: {listado.Count()}", ConsoleColor.Yellow);
        }
    }

    public void ListarNoPresentaron(int practico = 1) {
        Consola.Escribir($"\nListado de alumnos ausentes en el TP{practico}:", ConsoleColor.Yellow);
        foreach (var comision in Comisiones) {
            var listado = EnComision(comision).ConPractico(practico, EstadoPractico.NoPresentado).ConAbandono(false);
            ListarPorComision(listado, comision, "alumnos ausentes");
        }
        var totalAusentes = ConPractico(practico, EstadoPractico.NoPresentado).alumnos.Count;
        Consola.Escribir($"\nTOTAL: {totalAusentes} de {alumnos.Count} alumnos", ConsoleColor.Yellow);
    }

    public void ListarAusentes(int cantidad) {
        Consola.Escribir($"\nListado de alumnos con {cantidad} o más ausencias:", ConsoleColor.Yellow);
        foreach (var comision in Comisiones) {
            var listado = EnComision(comision).ConAusentes(cantidad);
            ListarPorComision(listado, comision, $"alumnos con {cantidad} o más ausencias");
        }
        var totalAusentes = ConAusentes(cantidad).alumnos.Count;
        Consola.Escribir($"\nTOTAL: {totalAusentes} de {alumnos.Count} alumnos", ConsoleColor.Yellow);
    }

    public void Reiniciar(){
        alumnos.ForEach(a => a.Reiniciar());
    }

    public void CargarAsistencia(List<Asistencia> asistencias){
        foreach(var asistencia in asistencias){
            // Consola.Escribir($"- Cargando asistencia de {asistencia.Telefono} ({asistencia.Fechas.Count} fechas)", ConsoleColor.Cyan);
            var alumno = Buscar(asistencia.Telefono);
            if (alumno == null) {
                Consola.Escribir($" - No se encontró el alumno con teléfono: {asistencia.Telefono}", ConsoleColor.Red);
                continue;
            }
            alumno.Asistencias = asistencia.Fechas.Count;
        }
    }

    public Alumno? Buscar(string telefono){
        // Limpiar el teléfono para dejar solo dígitos
        var soloNumeros = new string(telefono.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(soloNumeros) || soloNumeros.Length != 10) {
            Consola.Escribir($"Teléfono inválido para búsqueda: '{telefono}'", ConsoleColor.Red);
            return null;
        }
        // Formatear como (XXX) XXX-XXXX
        var telefonoFormateado = $"({soloNumeros.Substring(0, 3)}) {soloNumeros.Substring(3, 3)}-{soloNumeros.Substring(6, 4)}";
        return ConTelefono(true).FirstOrDefault(alumno => alumno.Telefono == telefonoFormateado);
    }

    public Alumno? Buscar(int legajo) => alumnos.FirstOrDefault(a => a.Legajo == legajo);

    public IEnumerator<Alumno> GetEnumerator() => alumnos.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()    => GetEnumerator();
}
