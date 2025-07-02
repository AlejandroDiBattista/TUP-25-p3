using System.Text.RegularExpressions;
using System.IO.Compression;
using TUP;
using System.Globalization;

// Clase auxiliar para el Menú

class Program {
    


    static void ListarAlumnos(Clase clase)
    {
        Consola.Escribir("=== Listado de alumnos ===", ConsoleColor.Cyan);
        clase.ListarAlumnos();
        clase.ExportarDatos();
    }

    static void CopiarPractico(Clase clase) {
        Consola.Escribir("=== Copiar trabajo práctico ===", ConsoleColor.Cyan);
        string tp   = Consola.LeerCadena("Ingrese el número del trabajo práctico a copiar (ej: 1): ", new[] { "1", "2", "3", "4", "5", "6" , "7"});
        bool forzar = Consola.Confirmar("¿Forzar copia incluso si ya existe?");

        clase.NormalizarCarpetas();
        clase.CopiarPractico(int.Parse(tp), forzar);
    }

    static void VerificarPresentacion(Clase clase, int practico, bool guardar= true) {
        Consola.Escribir("=== Verificar presentación de trabajo práctico ===", ConsoleColor.Cyan);
        clase.NormalizarCarpetas();
        clase.Reiniciar();
        // for (var p = 1; p <= practico; p++) {
        for (var p = practico; p <= practico; p++) {
            clase.VerificaPresentacionPractico(p);
        }
        var asistencias = Asistencias.Cargar(false);
        clase.CargarAsistencia(asistencias);
        if(guardar) clase.Guardar();
    }

    static void ListarNoPresentaron(Clase clase, int practico) {
        Consola.Escribir($"=== Alumnos que no presentaron práctico {practico} ===", ConsoleColor.Cyan);
        clase.ListarNoPresentaron(practico);
    }

    static void VerificarAsistencia() {
        Consola.Escribir("=== Verificar asistencia ===", ConsoleColor.Cyan);
        Asistencias.Cargar(true);
    }

    static void ConvertirNombreATelefono(Clase clase) {
        var alias = new Dictionary<string, string>();
        foreach (var alumno in clase.ConTelefono()) {
            alias[alumno.NombreLimpio] = alumno.TelefonoLimpio;
        }
        alias["Alejandro Di Battista"]     = "3815343458";
        alias["gonzalo zamora"]            = "3813540535";
        alias["~ Gabriel Carabajal"]       = "3815627688";
        alias["Cristian Ivan Soraire"]     = "X";
        alias["Abigail * Medina Costilla"] = "3816557818";
        alias["~ Agustín Morales"]          = "3815459105";
        alias["~ lauu🥶"]                   = "3812130484";

        var contar   = 0;
        var archivos = Directory.GetFiles("./asistencias", "historia*.txt");
        foreach (var origen in archivos) {
            List<string> salida = new();
            var lineas = File.ReadAllLines(origen);
            foreach (var linea in lineas) {
                var texto = linea.Trim();
                if (texto == "") continue;
                
                foreach(var (nombre, telefono) in alias) {
                    if (texto.Contains(nombre, StringComparison.OrdinalIgnoreCase)) {
                        texto = texto.Replace(nombre, telefono, StringComparison.OrdinalIgnoreCase);
                        contar++;
                    }
                }
                salida.Add(texto);
            }
            Consola.Escribir($"Se encontraron {contar} coincidencias en el archivo {origen}.");
            File.WriteAllLines(origen, salida);
        }

        Consola.Escribir($"Se encontraron {contar} coincidencias al convertir telefonos", ConsoleColor.Cyan);
    }

    static void RegistrarCreditos(Clase clase) {
        Dictionary<string, HashSet<string>> creditos = new();

        var archivos = Directory.GetFiles("./asistencias", "historia*.txt");
        foreach (var origen in archivos) {
            CargarCreditos(origen, creditos);
        }

        foreach (var alumno in clase.ConTelefono()) {
            if (creditos.ContainsKey(alumno.TelefonoLimpio)) {
                alumno.Creditos = creditos[alumno.TelefonoLimpio].Count;
            }
        }

        Consola.Escribir("\n=== Alumnos con mas creditos (Top 10) ===");
        foreach (var alumno in clase.OrderByDescending(a => a.Creditos).Where(a => a.Creditos > 0).Take(10)) {
            Consola.Escribir($". {alumno.NombreCompleto,-40}   {alumno.Telefono}   {alumno.Creditos, 2}");
        }

        clase.Guardar("alumos-normal.md");
    }

    static void CargarCreditos(string origen, Dictionary<string, HashSet<string>> creditos) {
        var patronTelefono  = new Regex(@"\b(\d{10})\b");
        var patronCredito   = new Regex(@"\b[0-9a-fA-F]{6}\b");
        var contarTelefonos = 0;
        var contarCreditos  = 0;
        var lineas   = File.ReadLines(origen);
        var telefono = "";

        foreach (var linea in lineas) {
            var matchTelefono = patronTelefono.Match(linea);
            if (matchTelefono.Success) {
                telefono = matchTelefono.Groups[1].Value;
                if (!creditos.ContainsKey(telefono)) {
                    creditos[telefono] = new HashSet<string>();
                }
                contarTelefonos++;
            }

            if(telefono.Trim() == "") continue;

            var matchCreditos = patronCredito.Matches(linea);
            foreach (Match credito in matchCreditos) {
                creditos[telefono].Add(credito.Value);
                contarCreditos++;
            }
        }

        Consola.Escribir($"Hay {lineas.Count()} líneas en el archivo con {contarTelefonos} telefonos y {contarCreditos} creditos.");
        // Consola.EsperarTecla();
    }

    static void RegistrarNotas(Clase clase) {
        Dictionary<string, HashSet<string>> creditos = new();
        Dictionary<string, string> notas = new();

        var archivos = Directory.GetFiles("./asistencias", "historia*.txt");
        
        foreach (var origen in archivos) {
            CargarCreditos(origen, creditos);
            CargarNotas(origen, notas);
        }

        foreach (var alumno in clase.ConTelefono()) {
            if (creditos.ContainsKey(alumno.TelefonoLimpio)) {
                alumno.Creditos = creditos[alumno.TelefonoLimpio].Count;
            }
            if (notas.ContainsKey(alumno.TelefonoLimpio)) {
                alumno.Parcial = int.Parse(notas[alumno.TelefonoLimpio].Substring(6,2));
            }
        }

        Consola.Escribir("\n=== Alumnos con examen perfecto ===");
        var i = 1;
        foreach (var alumno in clase.OrderByDescending(a => a.Parcial).Where(a => a.Parcial == 60)) {
            Consola.Escribir($"{i++,2}. {alumno.NombreCompleto,-40}   {alumno.Telefono}   {alumno.Parcial, 2}");
        }
        clase.Guardar("alumnos.md");
    }

    static void CargarNotas(string origen, Dictionary<string, string> notas) {
        var patronTelefono  = new Regex(@"\b(\d{10})\b");
        var patronNota      = new Regex(@"\b(\d{5}-\d{2}-\d{4})\b");
        var contarTelefonos = 0;
        var contarNotas     = 0;
        var lineas   = File.ReadLines(origen);
        var telefono = "";

        foreach (var linea in lineas) {
            var matchTelefono = patronTelefono.Match(linea);
            if (matchTelefono.Success) {
                telefono = matchTelefono.Groups[1].Value;
                contarTelefonos++;
            }

            if(telefono.Trim() == "") continue;

            var matchNota = patronNota.Match(linea);
            if (matchNota.Success) {
                notas[telefono] = matchNota.Groups[1].Value;
                contarNotas++;
            }
        }

        Consola.Escribir($"Hay {lineas.Count()} líneas en el archivo con {contarTelefonos} telefonos y {contarNotas} notas.");
    }

    static void CopiarHistoriaChat(Clase clase) {

        ProcessLatestZipForComision("c3");
        ProcessLatestZipForComision("c5");

        ConvertirNombreATelefono(clase);

        Consola.Escribir("Ok", ConsoleColor.Green); // Único mensaje al finalizar
    }

    static void ProcessLatestZipForComision(string comision) {
        string capetaOrigen   = "/Users/adibattista/Downloads";
        string carpetaDestino = "/Users/adibattista/Documents/GitHub/tup-25-p3/datos/asistencias";

        var origen  = Path.Combine(capetaOrigen);
        var destino = Path.Combine(carpetaDestino);
        try
        {
            var archivos = Directory.GetFiles(origen, $"WhatsApp*{comision}*.zip");
            Consola.Escribir($"Se encontraron {archivos.Length} archivos zip para la comisión {comision}.", ConsoleColor.Cyan);
            var ultimo = archivos.Select(f => new FileInfo(f)).OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
            if (ultimo == null) return; // No hay archivos zip para esta comision, salimos sin hacer nien

            string targetFileName = $"historia-{comision}.txt";
            string destinationFilePath = Path.Combine(destino, targetFileName);

            using (ZipArchive archive = ZipFile.OpenRead(ultimo.FullName)) {
                ZipArchiveEntry? chatEntry = archive.Entries.FirstOrDefault(entry =>
                    entry.Name.Equals("_chat.txt") || entry.FullName.Equals("_chat.txt")
                );
                if (chatEntry != null) {
                    using (StreamReader reader = new StreamReader(chatEntry.Open())) {
                        string chatContent = reader.ReadToEnd();
                        File.WriteAllText(destinationFilePath, chatContent);
                    }
                }
            }
            // Delete all previous WhatsApp zip files for this commission
            foreach (var file in archivos){
                File.Delete(file);
            }
        }
        catch (Exception ex)
        {
            Consola.Escribir($"Error al procesar el archivo zip de la comisión {comision}.", ConsoleColor.Red);
            Consola.Escribir($"El error es {ex.Message}", ConsoleColor.Red);
            return; // Si hay un error, salimos sin hacer nada más
        }
    }

    static void RegistrarTodo(Clase clase, int practico) {
        CopiarHistoriaChat(clase);
        VerificarAsistencia();
        VerificarPresentacion(clase, practico);
        RegistrarNotas(clase);
    }

    static void ListarUsuariosGithub(Clase clase) {
        Consola.Limpiar();
        Consola.Escribir("=== Listar usuarios sin GitHub ===", ConsoleColor.Cyan);
        clase.SinGithub().ListarAlumnos();
        if(Consola.Confirmar("¿Desea continuar y verificar los usuarios de GitHub?")) {
            Consola.Escribir("Revisando Github...", ConsoleColor.Cyan);
            var usuarios = clase.AveriguarUsuarioGithub(100);
            clase.Guardar("alumnos.md");
            if (usuarios.Count > 0)
            {
                Consola.Escribir("=== Usuarios encontrados ===", ConsoleColor.Green);
                foreach (var par in usuarios)
                {
                    Consola.Escribir($"Legajo: {par.Key} -> Usuario: {par.Value}");
                }
            }
        }
    }

    static void AgregarResultado(int legajo, string resultado) {
        string archivo = "resultados-p2.md";
        string lineaNueva = $"- {legajo} : {resultado}";
        List<string> lineas = new();
        bool encontrado = false;

        if (File.Exists(archivo)) {
            lineas = File.ReadAllLines(archivo).ToList();
            for (int i = 0; i < lineas.Count; i++) {
                if (lineas[i].TrimStart().StartsWith($"- {legajo} ")) {
                    lineas[i] = lineaNueva;
                    encontrado = true;
                    break;
                }
            }
        }
        if (!encontrado) {
            lineas.Add(lineaNueva);
        }
        File.WriteAllLines(archivo, lineas);
    }

    static void ProbarTP6(Clase clase) {
        Consola.Limpiar();
        Consola.Escribir("=== Probar TP6 ===", ConsoleColor.Cyan);
        int error = 0;
        foreach (var alumno in clase.Presentaron(6))
        {
            var resultado = clase.EjecutarSistema(alumno.Legajo);
            Consola.Escribir($"\n\n{alumno.Legajo} - {alumno.NombreCompleto}", ConsoleColor.Cyan);
            var estado = resultado ? EstadoPractico.EnProgreso : EstadoPractico.Error;

            var evaluacion = Consola.LeerCadena("Resultado:") ?? "";
            if (evaluacion.Trim() == "fin")
                return;
            if (evaluacion.Contains("ok"))
                estado = EstadoPractico.Aprobado;
            AgregarResultado(alumno.Legajo, evaluacion);
            alumno.PonerPractico(6, estado);
            clase.Guardar();
            if (!resultado) error++;
        }
        Consola.Escribir($"Se encontraron {error} errores al correr el TP6.", ConsoleColor.Red);
    }

    static void ProbarPorLegajo(Clase clase) {
        Consola.Limpiar();
        Consola.Escribir("=== Probar por Legajo ===", ConsoleColor.Cyan);
        int legajo = Consola.LeerEntero("Ingrese el legajo del alumno a probar: ");
        var alumno = clase.Buscar(legajo);
        if (alumno == null) {
            Consola.Escribir($"No se encontró un alumno con el legajo {legajo}.", ConsoleColor.Red);
            return;
        }
        var resultado = clase.EjecutarSistema(alumno.Legajo);
        Consola.Escribir($"{alumno.Legajo} {alumno.NombreCompleto} Resultado: {resultado}", resultado ? ConsoleColor.Green : ConsoleColor.Red);
    }

    static void CargarResultados(Clase clase) {
        string archivo = "resultados-p2.md";

        var lineas = File.ReadAllLines(archivo);
        foreach (var linea in lineas) {
            var partes = linea.Split(':');
            if (partes.Length < 2) continue;

            int legajo = int.Parse(partes[0].Trim().Substring(2));
            string resultado = partes[1].Trim();

            var alumno = clase.Buscar(legajo);
            if (alumno == null) continue;

            if (resultado.Contains("ok"))
            {
                alumno.PonerPractico(6, EstadoPractico.Aprobado);
            }
            else if (resultado.Contains("error -"))
            {
                alumno.PonerPractico(6, EstadoPractico.Error);
                alumno.Observaciones = partes[1].Replace("error -", "").Trim();
            }
            else if (resultado.Contains("no presentado"))
            {
                alumno.PonerPractico(6, EstadoPractico.NoPresentado);
            }
            else
            {
                alumno.PonerPractico(6, EstadoPractico.EnProgreso);
                alumno.Observaciones = partes[1].Trim();
            }

            Consola.Escribir($"Legajo {legajo} actualizado con resultado: {resultado}", ConsoleColor.Green);
        }
    }

// OUTPUT
    // Promocionado
    // Recuperar
    // Presentar
    // Corregir
    // Abandono
    // Revisar

    static void InformePractico(Clase clase)
    {
        CargarResultados(clase);
        foreach (var comision in clase.Comisiones)
        {
            Consola.Escribir("---");
            var lista = clase.EnComision(comision).Continuan();
            Consola.Escribir($"# Informe de la comisión {comision}");
            lista.ConResultado(EstadoMateria.Promocionado).Informar("Alumnos que aprobaron");
            lista.ConResultado(EstadoMateria.Recuperar).Informar("Alumnos que deben recuperar (tiene errores)");
            lista.ConResultado(EstadoMateria.Corregir).Informar("Alumnos que deben corregir (no funciona)");
            lista.ConResultado(EstadoMateria.Presentar).Informar("Alumnos que no presentaron");
            lista.ConResultado(EstadoMateria.Revisar).Informar("Alumnos a revisar",false);
            // clase.EnComision(comision).ConAbandono(true).Informar("Alumnos que abandonaron");
            // clase.Completar().Informar("Alumnos que deben completar trabajos prácticos");
            Consola.Escribir("\n");
        }
    }

    static void InformeResultadoFinal(Clase clase)
    {
        foreach (var comision in clase.Comisiones)
        {
            Consola.Escribir("---");
            Consola.Escribir($"== Informe de la comisión {comision} ==");
            var lista = clase.EnComision(comision).OrdenandoPorLegajo();
            Consola.Escribir("*Promocionan*", ConsoleColor.Green);
            foreach (var a in lista.Where(a => a.ResultadoFinal == ResultadoFinal.Promocionado))
            {
                Consola.Escribir($"* {a.Legajo} - {a.NombreCompleto}");
            }
            Consola.Escribir("\n*Regularizan*", ConsoleColor.Yellow);
            foreach (var a in lista.Where(a => a.ResultadoFinal == ResultadoFinal.Regular))
            {
                Consola.Escribir($"* {a.Legajo} - {a.NombreCompleto}");
            }
            Consola.Escribir("\n*Libres*", ConsoleColor.Red);
            foreach (var a in lista.Where(a => a.ResultadoFinal == ResultadoFinal.Libre))
            {
                Consola.Escribir($"* {a.Legajo} - {a.NombreCompleto}");
            }
        }
    }

    static void InformeResultadoFinalWApps(Clase clase)
    {
        foreach (var comision in clase.Comisiones)
        {
            Consola.Escribir($"*== Resultados {comision} ==*");
            var lista = clase.EnComision(comision).OrdenandoPorLegajo();
            Consola.Escribir("*Promocionados*\n```");
            foreach (var a in lista.Where(a => a.ResultadoFinal == ResultadoFinal.Promocionado))
            {
                Consola.Escribir($"{a.Legajo} - {a.NombreCompleto}");
            }
            Consola.Escribir("```\n\n*Regulares*\n```");
            foreach (var a in lista.Where(a => a.ResultadoFinal == ResultadoFinal.Regular))
            {
                Consola.Escribir($"{a.Legajo} - {a.NombreCompleto}");
            }
            Consola.Escribir("```\n\n*Libres*\n```");
            foreach (var a in lista.Where(a => a.ResultadoFinal == ResultadoFinal.Libre))
            {
                Consola.Escribir($"{a.Legajo} - {a.NombreCompleto}");
            }
            Consola.Escribir("```");
        }
    }
    static void CopiarTP7(Clase clase)
    {
        clase.VerificaPresentacionPractico(7);

        var alumnosPresentaron = clase.Presentaron(7);
        Consola.Escribir($"=== Copiando calculadoras de {alumnosPresentaron.Count()} alumnos que presentaron TP7 ===", ConsoleColor.Cyan);

        // Crear carpeta calculadoras si no existe
        string carpetaDestino = "./calculadoras";
        if (!Directory.Exists(carpetaDestino))
        {
            Directory.CreateDirectory(carpetaDestino);
        }

        int copiados = 0;
        int errores = 0;

        foreach (var alumno in alumnosPresentaron)
        {
            try
            {
                string archivoOrigen = Path.Combine($"../TP/{alumno.Carpeta}/tp7/calculadora.html");
                string archivoDestino = Path.Combine(carpetaDestino, $"{alumno.Legajo}.html");

                if (File.Exists(archivoOrigen))
                {
                    File.Copy(archivoOrigen, archivoDestino, true);
                    Consola.Escribir($"✓ Copiado: {alumno.Legajo} - {alumno.NombreCompleto}", ConsoleColor.Green);
                    copiados++;
                }
                else
                {
                    Consola.Escribir($"✗ No encontrado: {alumno.Legajo} - {alumno.NombreCompleto} (archivo no existe)", ConsoleColor.Yellow);
                    errores++;
                }
            }
            catch (Exception ex)
            {
                Consola.Escribir($"✗ Error: {alumno.Legajo} - {alumno.NombreCompleto} ({ex.Message})", ConsoleColor.Red);
                errores++;
            }
        }

        Consola.Escribir($"\n=== Resumen ===", ConsoleColor.Cyan);
        Consola.Escribir($"Archivos copiados exitosamente: {copiados}", ConsoleColor.Green);
        if (errores > 0)
        {
            Consola.Escribir($"Errores o archivos no encontrados: {errores}", ConsoleColor.Red);
        }

        clase.ListarAlumnos();
    }

    static void CompletarTP7(List<int> local, List<int> remoto) {
            try {
                string[] lineasResultados = File.ReadAllLines("../RESULTADOS.md");
                List<string> lineasModificadas = new List<string>();

                foreach (string linea in lineasResultados) {
                    string lineaModificada = ProcesarLineaTP7(linea, local, remoto);
                    lineasModificadas.Add(lineaModificada);
                }

                File.WriteAllLines("RESULTADOS-TP7.md", lineasModificadas);

            } catch (Exception ex) {
                Consola.Escribir($"Error al procesar archivos: {ex.Message}", ConsoleColor.Red);
            }
        }

    static void RegistrarPromocion(Clase clase, string comision) {
    // Ruta del archivo de la comisión
    string archivo = $"../{comision}.md";
    if (!File.Exists(archivo)) {
        Consola.Escribir($"No se encontró el archivo {archivo}", ConsoleColor.Red);
        return;
    }

    // Leer todas las líneas
    var lineas = File.ReadAllLines(archivo);

    // Marcadores
    string marcadorPromo = "Alumnos que promocionan";
    string marcadorRegu  = "Alumnos que regularizan";
    string marcadorLibre = "Alumnos libres";

    // Estados
    var promoLegajos = new List<int>();
    var reguLegajos = new List<int>();

    // Estado de búsqueda
    string estado = "";
    foreach (var linea in lineas) {
        string l = linea.Trim();
        if (l.Contains(marcadorPromo)) {
            estado = "promo";
            continue;
        } else if (l.Contains(marcadorRegu)) {
            estado = "regu";
            continue;
        } else if (l.Contains(marcadorLibre)) {
            estado = "";
            continue;
        }

        // Buscar legajo al inicio de la línea
        if (estado == "promo" || estado == "regu") {
            var match = System.Text.RegularExpressions.Regex.Match(l, @"^(\d{5})");
            if (match.Success) {
                int legajo = int.Parse(match.Groups[1].Value);
                if (estado == "promo") {
                    promoLegajos.Add(legajo);
                } else if (estado == "regu") {
                    reguLegajos.Add(legajo);
                }
            }
        }
    }

    // Actualizar estado en la clase
    foreach (var legajo in promoLegajos) {
        if (clase.Buscar(legajo) is { } alumno) {
            alumno.ResultadoFinal = ResultadoFinal.Promocionado;
        }
    }
    
    foreach (var legajo in reguLegajos) {
        if (clase.Buscar(legajo) is { } alumno) {
            alumno.ResultadoFinal = ResultadoFinal.Regular;
        }
    }

    var libres = clase.EnComision(comision).Where(a => a.ResultadoFinal == ResultadoFinal.Libre);

    Consola.Escribir($"Se actualizaron en {comision}:\n  {promoLegajos.Count,2} promocionados\n  {reguLegajos.Count,2} regularizados\n  {libres.Count(),2} libres.", ConsoleColor.Green);
}

    static List<int> ExtraerCalculadorasRemoto()
    {
        List<int> legajos = new();
        if (!File.Exists("calculadoras.md"))
        {
            throw new FileNotFoundException("No se encontró el archivo calculadoras.md");
        }
        string[] lineas = File.ReadAllLines("calculadoras.md");
        Regex regexLegajo = new(@"^(\d{5})");
        foreach (string linea in lineas)
        {
            Match match = regexLegajo.Match(linea.Trim());
            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int legajo))
                {
                    legajos.Add(legajo);
                }
            }
        }
        return legajos;
    }
    
    static string ProcesarLineaTP7(string linea, List<int> local, List<int> remota) {
        Regex regexLineaAlumno = new Regex(@"^(\d{5})(\s+.*)$");
        Match match = regexLineaAlumno.Match(linea);

        if (match.Success) {
            int legajo = int.Parse(match.Groups[1].Value);
            string circulo;
            bool enLocal  = local.Contains(legajo);
            bool enRemoto = remota.Contains(legajo);

            if (enLocal && enRemoto) {
                circulo = "🟢";
            } else if (enLocal && !enRemoto) {
                circulo = "⚫";
            } else if (!enLocal && enRemoto) {
                circulo = "🟡";
            } else {
                circulo = "🔴";
            }
            return $"{linea.Trim()} {circulo}";
        }

        // Si no es una línea de alumno, devolver sin modificar
        return linea;
    }

    static List<int> LegajosPresentaronTP7(Clase clase) {
        // Devuelve una lista de legajos de alumnos que presentaron el TP7
        return clase.Presentaron(7).Select(a => a.Legajo).ToList();
    }

    static void Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        var clase = Clase.Cargar();

        int practico = 7;

        var menu = new TUP.Menu("Bienvenido al sistema de gestión de alumnos");
        menu.Agregar("Listar alumnos", () => ListarAlumnos(clase));
        // menu.Agregar("Publicar trabajo práctico", () => CopiarPractico(clase));
        // menu.Agregar("Registrar Asistencia & Notas", () => RegistrarTodo(clase, practico));
        // menu.Agregar("Registrar Resultados", () => CargarResultados(clase));
        // menu.Agregar("Faltan presentar TP", () => ListarNoPresentaron(clase, practico));
        // menu.Agregar("Faltan Github", () => ListarUsuariosGithub(clase));
        // menu.Agregar("  P2: Ejecutar", () => ProbarTP6(clase));
        // menu.Agregar("  P2: Presentaron", () => clase.Presentaron(6).ListarAlumnos());
        // menu.Agregar("  Copiar TP7", () => CopiarTP7(clase));
        // menu.Agregar("  P2: Con error ", () => clase.ConError(6).ListarAlumnos());
        menu.Agregar("Probar por Legajo", () => ProbarPorLegajo(clase));
        menu.Agregar("Generar informe Final", () =>
        {
            RegistrarPromocion(clase, "C3");
            RegistrarPromocion(clase, "C5");
            InformeResultadoFinal(clase);
        });
        menu.Agregar("Generar informe Final por WhatApps", () =>
        {
            RegistrarPromocion(clase, "C3");
            RegistrarPromocion(clase, "C5");
            InformeResultadoFinalWApps(clase);
        });


        // menu.Agregar("Traer TP7", () =>
        // {
        //     clase.VerificaPresentacionPractico(7);
        //     var local = clase.Presentaron(7).Select(a => a.Legajo).ToList();
        //     var remoto = ExtraerCalculadorasRemoto();
        //     CompletarTP7(local, remoto);
        // });


        menu.Ejecutar();

        Consola.Escribir("Saliendo del programa...", ConsoleColor.DarkGray);
    }

}