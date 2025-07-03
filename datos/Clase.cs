using System.Collections;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

using TUP;

class Clase : IEnumerable<Alumno>
{
    public List<Alumno> alumnos = new List<Alumno>();
    const string LineaComision = @"##.*\s(?<comision>C\d)";
    // const string LineaAlumno = @"(?<index>\d+)\.\s*(?<legajo>\d{5})\s*(?<nombre>[^,]+)\s*,\s*(?<apellido>[^(]+)(?:\s*(?<telefono>\(\d+\)\s*\d+(?:[- ]\d+)*))?";
    const string LineaAlumno = @"(?<index>\d+)\.\s+(?<legajo>\d{5})\s+(?<nombre>[^,]+)";

    public List<string> Comisiones => alumnos.Select(a => a.Comision).Distinct().OrderBy(c => c).ToList();
    public IEnumerable<Alumno> Alumnos => alumnos.OrderBy(a => a.NombreCompleto).ToList();

    public Clase(IEnumerable<Alumno>? alumnos = null)
    {
        this.alumnos = alumnos?.ToList() ?? new List<Alumno>();
    }

    public static Clase Cargar(string origen = "./alumnos.md")
    {
        string comision = "C0";
        Clase clase = new();

        foreach (var linea in File.ReadLines(origen))
        {
            var texto = linea.Trim();
            var matchComision = Regex.Match(texto, LineaComision);
            if (matchComision.Success)
            {
                comision = matchComision.Groups["comision"].Value;
                continue;
            }

            var matchAlumno = Regex.Match(texto, LineaAlumno);
            if (matchAlumno.Success)
            {
                var partes = Regex.Split(linea, @"\s{2,}"); // Split on 2 or more spaces
                var index = int.Parse(partes[0].TrimEnd('.'));
                var legajo = int.Parse(partes[1]);
                var nombre = partes[2].Trim();
                var telefono = partes[3].Trim();
                var asistencias = int.Parse(partes[4]);
                var practicos = partes[5].Trim();
                var creditos = int.Parse(partes[6]);
                var parcial = int.Parse(partes[7]);
                var github = partes.Length > 9 ? partes[9].Trim() : "";
                // Console.WriteLine($"Legajo: {legajo} - Nombre: {nombre} - Teléfono: {telefono} - Asistencias: {asistencias} - Practicos: {practicos} - Créditos: {creditos} - Parcial: {parcial} - GitHub: {github}");
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
                    parcial, 
                    github
                );

                clase.alumnos.Add(alumno);
                continue;
            }
        }

        return clase;
    }

    // Métodos de filtrado
    public Clase EnComision(string comision) => new(alumnos.Where(a => a.Comision == comision));
    public Clase ConTelefono(bool incluirTelefono = true) => new(alumnos.Where(a => incluirTelefono == a.TieneTelefono));
    public Clase ConAbandono(bool abandono) => new(alumnos.Where(a => a.Abandono == abandono));
    public Clase ConNombre(string nombre) => new(alumnos.Where(a => a.NombreCompleto.Contains(nombre, StringComparison.OrdinalIgnoreCase)));
    public Clase ConPractico(int numero, EstadoPractico estado) => new(alumnos.Where(a => a.ObtenerPractico(numero) == estado));
    public Clase ConAusentes(int cantidad) => new(Alumnos.Where(a => a.Practicos.Count(p => p == EstadoPractico.NoPresentado) >= cantidad));
    public Clase ConAprobados(int cantidad) => new(Alumnos.Where(a => a.Practicos.Count(p => p == EstadoPractico.Aprobado) >= cantidad));
    public Clase OrdenandoPorNombre() => new(alumnos.OrderBy(a => a.Apellido).ThenBy(a => a.Nombre));
    public Clase OrdenandoPorLegajo() => new(alumnos.OrderBy(a => a.Legajo));
    public Clase SinGithub() => new(alumnos.Where(a => a.GitHub == ""));
    public Clase EnProgreso(int practico=6) => new(alumnos.Where(a => a.ObtenerPractico(practico) == EstadoPractico.EnProgreso));
    public Clase ConError(int practico=6) => new(alumnos.Where(a => a.ObtenerPractico(practico) == EstadoPractico.Error));
    public Clase NoPresentaron(int practico = 1) => new(alumnos.Where(a => a.ObtenerPractico(practico) == EstadoPractico.NoPresentado));
    public Clase Presentaron(int practico = 6) => new(alumnos.Where(a => a.ObtenerPractico(practico) == EstadoPractico.Aprobado || a.ObtenerPractico(practico) == EstadoPractico.EnProgreso));
    public Clase C3() => this.EnComision("C3");
    public Clase C5() => this.EnComision("C5");
    public Clase Continuan() => new(alumnos.Where(a => a.Continuan));
    public Clase ConResultado(EstadoMateria estado) => new(alumnos.Where(a => a.Estado == estado));
    public Clase Completar() => new(alumnos.Where(a => a.Faltantes > 0 && a.Faltantes <= 3 && a.Continuan));
    public Clase ConResultadoFinal(ResultadoFinal resultado) => new(alumnos.Where(a => a.ResultadoFinal == resultado));

    // Métodos de modificación
    public void Agregar(Alumno alumno)
    {
        if (alumno != null)
        {
            alumnos.Add(alumno);
        }
    }

    public void Agregar(IEnumerable<Alumno> alumnos)
    {
        foreach (var alumno in alumnos)
        {
            Agregar(alumno);
        }
    }

    public void Guardar(string destino = "./alumnos.md")
    {
        using (StreamWriter writer = new(destino))
        {
            writer.WriteLine("# Listado de alumnos");
            foreach (var comision in Comisiones)
            {
                var orden = 1;
                writer.WriteLine($"\n## Comisión {comision}");
                foreach (var a in EnComision(comision).OrdenandoPorNombre())
                {
                    a.Orden = orden++;
                    var linea = $"{a.Orden:D2}.  {a.Legajo,5}  {a.NombreCompleto,-35}  {a.Telefono,-14}   {a.Asistencias,2}  {a.PracticosStr,-10}  {a.Creditos,2}  {a.Parcial,2}  {a.Nota,4:F1}  {a.GitHub}";
                    writer.WriteLine(linea);
                }
            }
        }
    }

    public void GuardarVCards(string destino)
    {
        Consola.Escribir($"- Exportando vCards a {destino}");
        using (StreamWriter writer = new(destino))
        {
            foreach (var alumno in ConTelefono(true).OrdenandoPorNombre())
            {
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

    public void NormalizarCarpetas()
    {
        const string Base = "../TP";
        Directory.CreateDirectory(Base);

        Consola.Escribir($"▶︎ Creando carpetas en {Path.GetFullPath(Base)}", ConsoleColor.Green);
        var cambios = 0;
        foreach (var alumno in OrdenandoPorLegajo())
        {
            var carpetaDeseada = alumno.Carpeta;
            var rutaCompleta = Path.Combine(Base, carpetaDeseada);

            if (Directory.Exists(rutaCompleta)) continue;

            var carpetasExistentes = Directory.GetDirectories(Base, $"{alumno.Legajo}*");

            if (carpetasExistentes.Length > 0)
            {
                var carpetaEncontrada = carpetasExistentes[0];
                if (Path.GetFileName(carpetaEncontrada) != carpetaDeseada)
                {
                    Directory.Move(carpetaEncontrada, rutaCompleta);
                    Consola.Escribir($"  - Renombrando carpeta:\n  <[{Path.GetFileName(carpetaEncontrada)}]\n  >[{carpetaDeseada}]", ConsoleColor.Yellow);
                    cambios++;
                }
            }
            else
            {
                Directory.CreateDirectory(rutaCompleta);
                Consola.Escribir($"  - Creando carpeta:\n  >[{carpetaDeseada}]", ConsoleColor.Yellow);
                cambios++;
            }
        }
        Consola.Escribir($"● {cambios} carpetas cambiadas", ConsoleColor.Green);
    }

    public static int ContarLineasEfectivas(string archivo)
    {
        var lineas = File.ReadAllLines(archivo).TakeWhile(linea => !linea.Contains("PRUEBAS AUTOMATIZADAS"));
        return lineas.Count(linea =>
            !linea.Trim().Equals("") &&                     // No es una línea vacía
            !linea.TrimStart().StartsWith("Console.") &&    // No es un mensaje de consola
            !linea.TrimStart().StartsWith("using") &&       // No es una directiva using
            !linea.TrimStart().StartsWith("//") &&          // No es un comentario
            !linea.Trim().Equals("{") &&                    // No es solo una llave de apertura
            !linea.Trim().Equals("}")                       // No es solo una llave de cierre
        );
    }


    public void VerificaPresentacionPractico(int practico)
    {
        const string Base = "../TP";

        var fuente = practico switch {
            4 => "Program.cs",
            5 => "Servidor.cs",
            6 => "servidor/Program.cs",
            7 => "calculadora.html",
            _ => "ejercicio.cs"
        };

        Consola.Escribir($"\n=== Verificación de presentación del trabajo práctico TP{practico} ===", ConsoleColor.Blue);
        var enunciado = Path.Combine("../enunciados", $"tp{practico}", fuente);
        int lineas = ContarLineasEfectivas(enunciado);
        Consola.Escribir($" - Enunciado tiene {lineas} líneas efectivas", ConsoleColor.Cyan);
        foreach (var comision in Comisiones)
        {
            var presentados = 0;
            var ausentes = 0;
            var errores = 0;
            foreach (var alumno in EnComision(comision))
            {
                var archivo = Path.Combine(Base, alumno.Carpeta, $"tp{practico}", fuente);
                EstadoPractico estado = EstadoPractico.Error;
                if (File.Exists(archivo))
                {
                    int lineasEfectivas = ContarLineasEfectivas(archivo) - lineas;

                    estado = lineasEfectivas >= 20 ? EstadoPractico.Aprobado : EstadoPractico.NoPresentado;
                    if (estado == EstadoPractico.Aprobado)
                    {
                        presentados++;
                    }
                    if (estado == EstadoPractico.NoPresentado)
                    {
                        ausentes++;
                    }
                    alumno.PonerPractico(practico, estado);

                    if (practico == 3 && lineasEfectivas > 20)
                    { // Si es TP3, ejecutar el programa y verificar el resultado
                        alumno.Resultado = ResultadoEjecutar(archivo);
                    }

                    if (practico == 6)
                    { // Si es TP4, ejecutar el programa y verificar el resultado

                        if (lineasEfectivas >= 0)
                        {
                            estado = EstadoPractico.EnProgreso;
                        }
                        else
                        {
                            estado = EstadoPractico.NoPresentado;
                        }
                        alumno.PonerPractico(practico, estado);
                    }

                    var color = lineasEfectivas < 20 ? ConsoleColor.Yellow : ConsoleColor.White;
                    if (alumno.Resultado < 0)
                    {
                        errores++;
                        color = ConsoleColor.Red;
                    }
                    if (alumno.Resultado > 0)
                    {
                        color = ConsoleColor.Green;
                    }
                    Consola.Escribir($" - {alumno.Legajo}  {alumno.NombreCompleto,-60}  {lineasEfectivas,3} {estado}", color);
                }
            }
            Consola.Escribir($"Comisión {comision} \n Presentados: {presentados,3}\n Ausentes   : {ausentes,3}\n Errores:     {errores,3}", ConsoleColor.Cyan);
        }
    }

    public static string EjecutarCSharp(string origen)
    {
        var runInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"script \"{origen}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var runProcess = Process.Start(runInfo) ?? throw new InvalidOperationException("Failed to start process"))
        {
            string salida = runProcess.StandardOutput.ReadToEnd();
            string error = runProcess.StandardError.ReadToEnd();
            runProcess.WaitForExit();
            return salida + error;
        }
    }

    public static int ResultadoEjecutar(string origen)
    {
        var salida = EjecutarCSharp(origen);
        if (salida.Contains(": error"))
        {
            return -salida.Split("\n").Count(line => line.Contains(": error"));
        }
        else
        {
            return salida.Split("\n").Count(line => line.Contains("[OK]"));
        }
    }

    public bool EjecutarSistema(int legajo)
    {
        string? carpeta = CarpetaDeAlumnoPorLegajo(legajo);
        if (carpeta is null) {
            Console.WriteLine("No se encontró la carpeta del alumno.");
            return false;
        }
        string Base = Path.Combine("../TP",carpeta, "tp6");
        Consola.Escribir($"▶︎ Ejecutando sistema para el alumno {legajo} en {Base}", ConsoleColor.Cyan);
        var resultado = Corredor.CorrerSistema(Base).GetAwaiter().GetResult();
        switch (resultado) {
            case ResultadoEjecucion.FallaServidor:
                Consola.Escribir("> Fallo Servidor:", ConsoleColor.Red);
                return false;
            case ResultadoEjecucion.FallaCliente:
                Consola.Escribir("> Fallo Cliente:", ConsoleColor.Red);
                return false;
            case ResultadoEjecucion.FallaNavegador:
                Consola.Escribir(" > Fallo Navegador:", ConsoleColor.Red);
                return false;
            case ResultadoEjecucion.FallaBaseDatos:
                Consola.Escribir("> Fallo Base de Datos - Faltan migraciones:", ConsoleColor.Red);
                return false;
            case ResultadoEjecucion.FallaGeneral:
                Consola.Escribir("> Fallo General:", ConsoleColor.Red);
                return false;
        }
        return resultado == ResultadoEjecucion.Ok;
    }

    public void CopiarPractico(int practico, bool forzar = false)
    {
        const string Base = "../TP";
        const string Enunciados = "../enunciados";
        Consola.Escribir($" ▶︎ Copiando trabajo práctico de TP{practico}", ConsoleColor.Cyan);
        var carpetaOrigen = Path.Combine(Enunciados, $"tp{practico}");

        if (!Directory.Exists(carpetaOrigen))
        {
            Consola.Escribir($"Error: No se encontró el enunciado del trabajo práctico '{practico}' en {carpetaOrigen}", ConsoleColor.Red);
            return;
        }

        foreach (var alumno in Alumnos.OrderBy(a => a.Legajo))
        {
            var carpetaDestino = Path.Combine(Base, alumno.Carpeta, $"tp{practico}");
            if (forzar && Directory.Exists(carpetaDestino))
            {
                Directory.Delete(carpetaDestino, true);
            }
            Directory.CreateDirectory(carpetaDestino);

            Consola.Escribir($" - Copiando a {carpetaDestino}", ConsoleColor.Yellow);
            CopiarDirectorioRecursivamente(carpetaOrigen, carpetaDestino, forzar);
        }
        Consola.Escribir($" ● Copia de trabajo práctico completa", ConsoleColor.Green);
    }

    private void CopiarDirectorioRecursivamente(string origen, string destino, bool forzar) {
        // Copiar todos los archivos del directorio
        foreach (var archivo in Directory.GetFiles(origen)) {
            var nombreArchivo = Path.GetFileName(archivo);
            if (nombreArchivo == "enunciado.md") continue;

            var destinoArchivo = Path.Combine(destino, nombreArchivo);
            if (!File.Exists(destinoArchivo) || forzar) {
                File.Copy(archivo, destinoArchivo, forzar);
            }
        }

        // Copiar recursivamente todos los subdirectorios
        foreach (var directorio in Directory.GetDirectories(origen)) {
            var nombreDirectorio = Path.GetFileName(directorio);
            var destinoDirectorio = Path.Combine(destino, nombreDirectorio);
            
            if (!Directory.Exists(destinoDirectorio)) {
                Directory.CreateDirectory(destinoDirectorio);
            }
            
            CopiarDirectorioRecursivamente(directorio, destinoDirectorio, forzar);
        }
    }

    public void ExportarDatos()
    {
        Consola.Escribir($" ▶︎ Generando listado de alumnos (hay {alumnos.Count()} alumnos.)", ConsoleColor.Cyan);
        foreach (var comision in Comisiones)
        {
            var alumnosComision = ConTelefono(true).EnComision(comision);
            alumnosComision.GuardarVCards($"./alumnos-{comision}.vcf");
        }
        ConTelefono(true).GuardarVCards("./alumnos.vcf");
        Guardar("./resultados.md");
        Consola.Escribir($" ● Exportación completa", ConsoleColor.Green);
    }

    public void ListarAlumnos()
    {
        foreach (var comision in Comisiones)
        {
            Consola.Escribir($"\n=== Comisión {comision} ===", ConsoleColor.Blue);
            var lista = EnComision(comision).ConAbandono(false).OrdenandoPorNombre();
            foreach (var alumno in lista)
            {
                var emojis = alumno.Practicos.Select(p => p.Emoji).ToList();
                if (alumno.Resultado < 0)
                {
                    emojis[2] = "🔴";
                }
                var asistencia = string.Join("", emojis);
                string linea = $"{alumno.Legajo} - {alumno.NombreCompleto,-40} {$"{alumno.Telefono}",-15}";
                linea = $" {linea,-65} {alumno.Asistencias,2}  {asistencia}  {alumno.Nota,4:0.0}";

                Consola.Escribir(linea);
            }
            Consola.Escribir($"Total alumnos en comisión {comision}: {EnComision(comision).Count()}", ConsoleColor.Yellow);
        }
        Consola.Escribir($"\nTotal general de alumnos: {alumnos.Count}", ConsoleColor.Green);
    }
    

    public void ListarTP7() {
        var lista = ConAbandono(false).OrdenandoPorLegajo();
        foreach (var alumno in lista)
            if (alumno.ObtenerPractico(7) == EstadoPractico.Aprobado)
                Consola.Escribir($"{alumno.Legajo} 🟢");
            else
                Consola.Escribir($"{alumno.Legajo} 🔴");
    }


    public void Informar(string titulo, bool conObservacion = true)
    {
        if (!alumnos.Any()) return;
        Consola.Escribir($"\n## {titulo}");
        Consola.Escribir("```");
        bool primero = true;
        foreach (var alumno in OrdenandoPorNombre())
        {
            var emojis = string.Join("", alumno.Practicos.Select(p => p.Emoji).ToList());
            string linea = $"{alumno.Legajo} {alumno.NombreCompleto,-35}   {alumno.Asistencias,2}   {emojis}";
            if (conObservacion && (alumno.Estado == EstadoMateria.Recuperar || alumno.Estado == EstadoMateria.Corregir) && alumno.Observaciones != "")
            {
                linea += $"\n      {alumno.Observaciones}";
                if (!primero) linea = "\n" + linea;
            }

            Consola.Escribir(linea);
            if (primero) primero = false;
        }
        Consola.Escribir("```");
    }

    private void ListarPorComision(IEnumerable<Alumno> listado, string comision, string mensaje)
    {
        if (!listado.Any())
        {
            Consola.Escribir($"No hay {mensaje} en la comisión {comision}", ConsoleColor.Green);
        }
        else
        {
            Consola.Escribir($"\n=== Comisión {comision} ===", ConsoleColor.Blue);
            Consola.Escribir("```");
            foreach (var alumno in listado)
            {
                Consola.Escribir($"{alumno.Legajo} {alumno.NombreCompleto,-32} {alumno.Asistencias,2} {alumno.CantidadPresentados,2}", ConsoleColor.Red);
            }
            Consola.Escribir("```");
            Consola.Escribir($"Total {mensaje}: {listado.Count()}", ConsoleColor.Yellow);
        }
    }

    public void ListarNoPresentaron(int practico = 1)
    {
        Consola.Escribir($"\nListado de alumnos ausentes en el TP{practico}:", ConsoleColor.Yellow);
        foreach (var comision in Comisiones)
        {
            var listado = EnComision(comision).ConPractico(practico, EstadoPractico.NoPresentado).ConAbandono(false);
            ListarPorComision(listado, comision, "alumnos ausentes");
        }
        var totalAusentes = ConPractico(practico, EstadoPractico.NoPresentado).alumnos.Count;
        Consola.Escribir($"\nTOTAL: {totalAusentes} de {alumnos.Count} alumnos", ConsoleColor.Yellow);
    }

    public void ListarAusentes(int cantidad)
    {
        Consola.Escribir($"\nListado de alumnos con {cantidad} o más ausencias:", ConsoleColor.Yellow);
        foreach (var comision in Comisiones)
        {
            var listado = EnComision(comision).ConAusentes(cantidad);
            ListarPorComision(listado, comision, $"alumnos con {cantidad} o más ausencias");
        }
        var totalAusentes = ConAusentes(cantidad).alumnos.Count;
        Consola.Escribir($"\nTOTAL: {totalAusentes} de {alumnos.Count} alumnos", ConsoleColor.Yellow);
    }

    public void Reiniciar()
    {
        alumnos.ForEach(a => a.Reiniciar());
    }

    public void CargarAsistencia(List<Asistencia> asistencias)
    {
        foreach (var asistencia in asistencias)
        {
            // Consola.Escribir($"- Cargando asistencia de {asistencia.Telefono} ({asistencia.Fechas.Count} fechas)", ConsoleColor.Cyan);
            var alumno = Buscar(asistencia.Telefono);
            if (alumno == null)
            {
                Consola.Escribir($" - No se encontró el alumno con teléfono: {asistencia.Telefono}", ConsoleColor.Red);
                continue;
            }
            alumno.Asistencias = asistencia.Fechas.Count;
        }
    }

    public Alumno? Buscar(string telefono)
    {
        // Limpiar el teléfono para dejar solo dígitos
        var soloNumeros = new string(telefono.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(soloNumeros) || soloNumeros.Length != 10)
        {
            Consola.Escribir($"Teléfono inválido para búsqueda: '{telefono}'", ConsoleColor.Red);
            return null;
        }
        // Formatear como (XXX) XXX-XXXX
        var telefonoFormateado = $"({soloNumeros.Substring(0, 3)}) {soloNumeros.Substring(3, 3)}-{soloNumeros.Substring(6, 4)}";
        return ConTelefono(true).FirstOrDefault(alumno => alumno.Telefono == telefonoFormateado);
    }

    public Alumno? Buscar(int legajo) => alumnos.FirstOrDefault(a => a.Legajo == legajo);

    public IEnumerator<Alumno> GetEnumerator() => alumnos.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public IList<int> Legajos => alumnos.Select(a => a.Legajo).ToList();

    private string ObtenerTokenGithub() {
        try
        {
            // Buscar solo en el archivo de configuración local
            string configPath = Path.Combine(".", "github-config.json");
            if (File.Exists(configPath)) {
                var config = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(configPath));
                if (config != null && config.TryGetValue("GitHubToken", out string? token) && !string.IsNullOrEmpty(token)) {
                    return token;
                }
            }

            // Mensaje de advertencia si no se encuentra el token
            Console.WriteLine($"ADVERTENCIA: No se encontró el archivo de configuración '{configPath}'");
            Console.WriteLine("Cree este archivo con el siguiente formato:");
            Console.WriteLine(@"{""GitHubToken"": ""su_token_github_aquí""}");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener token: {ex.Message}");
            Console.ReadLine();
        }
        
        return string.Empty;
    }

    public Dictionary<int, string> AveriguarUsuarioGithub(int maximo = 1000) {
        var cliente = new HttpClient();
        
        // Manejo seguro del token de GitHub
        string githubToken = ObtenerTokenGithub();
        var repo = "AlejandroDiBattista/TUP-25-p3";

        cliente.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DotnetScript", "1.0"));
        
        // Solo usar autorización si tenemos un token válido
        if (!string.IsNullOrEmpty(githubToken)) {
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", githubToken);
        }

        var mapeo = new Dictionary<int, string>();
        int page = 1;

        int contar = 0;
        while (true) {
            var url = $"https://api.github.com/repos/{repo}/pulls?state=all&per_page=100&page={page}";
            var response = cliente.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode) {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || 
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden) {
                    Console.WriteLine($"Error de autorización: {response.StatusCode}. Verifique el token de GitHub.");
                } else if ((int)response.StatusCode == 429) { // Too Many Requests
                    Console.WriteLine("Se ha alcanzado el límite de tasa de la API de GitHub. Espere unos minutos antes de intentar de nuevo.");
                    if (response.Headers.Contains("X-RateLimit-Reset")) {
                        var resetTime = response.Headers.GetValues("X-RateLimit-Reset").FirstOrDefault();
                        if (!string.IsNullOrEmpty(resetTime) && long.TryParse(resetTime, out long epochTime)) {
                            var resetDateTime = DateTimeOffset.FromUnixTimeSeconds(epochTime).LocalDateTime;
                            Console.WriteLine($"Puede intentar nuevamente después de: {resetDateTime}");
                        }
                    }
                } else {
                    Console.WriteLine($"Error en la API de GitHub: {response.StatusCode}");
                }
                break;
            }
            
            // Verificar los límites de tasa de la API
            if (response.Headers.Contains("X-RateLimit-Remaining")) {
                var remaining = response.Headers.GetValues("X-RateLimit-Remaining").FirstOrDefault();
                if (remaining == "1") {
                    Console.WriteLine("¡Advertencia! Está a punto de alcanzar el límite de tasa de la API de GitHub.");
                }
            }

            using var stream = response.Content.ReadAsStreamAsync().Result;
            var prs = JsonSerializer.Deserialize<JsonElement>(stream);

            if (prs.GetArrayLength() == 0)
                break;

            foreach (var pr in prs.EnumerateArray()) {
                var titulo = pr.GetProperty("title").GetString() ?? "";
                var usuario = pr.GetProperty("user").GetProperty("login").GetString() ?? "";

                foreach (var a in SinGithub()) {
                    if (titulo.Contains(a.Legajo.ToString()))
                    {
                        mapeo[a.Legajo] = usuario;
                        a.GitHub = usuario;
                        Console.WriteLine($"{contar}). Legajo: {a.Legajo} - Usuario: {usuario}");
                        if (contar++ > maximo)
                            return mapeo;
                    }
                }
            }

            page++;
        }

        return mapeo;
    }

    public string? CarpetaDeAlumnoPorLegajo(int legajo) {
        var alumno = Buscar(legajo);
        if (alumno != null) {
            return alumno.Carpeta;
        } else {
            return null;
        }
    }

}
