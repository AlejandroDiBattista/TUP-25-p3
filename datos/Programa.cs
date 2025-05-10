using System.Text.RegularExpressions;
using TUP;

class Program {
    static string ElegirOpcionMenu() {
        Console.Clear();
        Consola.Escribir("=== MENÚ DE OPCIONES ===", ConsoleColor.Cyan);
        Consola.Escribir("1. Listar alumnos");
        Consola.Escribir("2. Publicar trabajo práctico");
        Consola.Escribir("3. Verificar presentación de trabajos práctico");
        Consola.Escribir("4. Faltan presentar trabajo práctico");
        Consola.Escribir("5. Verificar asistencia");
        Consola.Escribir("6. Mostrar recuperación");
        Consola.Escribir("7. Cargar Notas Parcial");
        Consola.Escribir("0. Salir");
        return Consola.ElegirOpcion("\nElija una opción (0-7): ", "01234567");
    }

    static void OpcionListarAlumnos(Clase clase) {
        Consola.Escribir("=== Listado de alumnos ===", ConsoleColor.Cyan);
        clase.ListarAlumnos();
        clase.ExportarDatos();
    }

    static void OpcionCopiarPractico(Clase clase) {
        Consola.Escribir("=== Copiar trabajo práctico ===", ConsoleColor.Cyan);
        string tp   = Consola.LeerCadena("Ingrese el número del trabajo práctico a copiar (ej: 1): ", new[] { "1", "2", "3", "4", "5", "6" });
        bool forzar = Consola.Confirmar("¿Forzar copia incluso si ya existe?");

        clase.NormalizarCarpetas();
        clase.CopiarPractico(int.Parse(tp), forzar);
    }

    static void OpcionVerificarPresentacion(Clase clase, int practico) {
        Consola.Escribir("=== Verificar presentación de trabajo práctico ===", ConsoleColor.Cyan);
        clase.NormalizarCarpetas();
        clase.Reiniciar();
        for (var p = 1; p <= practico; p++) {
            clase.VerificaPresentacionPractico(p);
        }
        var asistencias = Asistencias.Cargar(false);
        clase.CargarAsistencia(asistencias);
        clase.Guardar();
    }

    static void OpcionListarNoPresentaron(Clase clase, int practico) {
        Consola.Escribir($"=== Alumnos que no presentaron práctico {practico} ===", ConsoleColor.Cyan);
        clase.ListarNoPresentaron(practico);
    }

    static void OpcionVerificarAsistencia() {
        Consola.Escribir("=== Verificar asistencia ===", ConsoleColor.Cyan);
        Asistencias.Cargar(true);
    }

    static void OpcionMostrarRecuperacion(Clase clase) {
        Consola.Escribir("=== Generando reporte de recuperación ===", ConsoleColor.Cyan);
        clase.GenerarReporteRecuperacion();
        Consola.Escribir("Reporte 'recuperacion.md' generado.", ConsoleColor.Green);
        clase.DebenRecuperar().ListarAlumnos();
    }

    static void ConvertirNombreATelefono(Clase clase) {
        var alias = new Dictionary<string, string>();
        foreach (var alumno in clase.ConTelefono()) {
            alias[alumno.NombreLimpio] = alumno.TelefonoLimpio;
        }
        alias["Alejandro Di Battista"]     = "3815343458";
        alias["gonzalo zamora"]            = "3813540535";
        alias["~ Gabriel Carabajal"]        = "3815627688";
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

        Consola.Escribir($"Se encontraron {contar} coincidencias.", ConsoleColor.Cyan);
        Consola.EsperarTecla();
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

        Consola.Escribir("\n=== Alumnos con mas creditos (Top 20) ===");
        foreach (var alumno in clase.OrderByDescending(a => a.Creditos).Where(a => a.Creditos > 0).Take(20)) {
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
        Dictionary<string, string> notas = new();

        var archivos = Directory.GetFiles("./asistencias", "historia*.txt");
        foreach (var origen in archivos) {
            CargarNotas(origen, notas);
        }

        foreach (var alumno in clase.ConTelefono()) {
            if (notas.ContainsKey(alumno.TelefonoLimpio)) {
                alumno.Nota1erParcial = int.Parse(notas[alumno.TelefonoLimpio].Substring(6,2));
            }
        }

        Consola.Escribir("\n=== Alumnos menores notas (Top 20) ===");
        foreach (var alumno in clase.OrderByDescending(a => a.Nota1erParcial).Where(a => a.Nota1erParcial > 0).Take(20)) {
            Consola.Escribir($". {alumno.NombreCompleto,-40}   {alumno.Telefono}   {alumno.Nota1erParcial, 2}");
        }
        clase.Guardar("alumos-normal.md");
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
        // Consola.EsperarTecla();
    }
    

    static void Main(string[] args) {
        var clase = Clase.Cargar();
        ConvertirNombreATelefono(clase);

        int practico = 4;

        Consola.Escribir("=== Bienvenido al sistema de gestión de alumnos ===", ConsoleColor.Cyan);
        while (true) {
            string opcion = ElegirOpcionMenu();
            if (opcion == "0") return;
            Console.Clear();

            Action action = opcion switch {
                "1" => () => OpcionListarAlumnos(clase),
                "2" => () => OpcionCopiarPractico(clase),
                "3" => () => OpcionVerificarPresentacion(clase, practico),
                "4" => () => OpcionListarNoPresentaron(clase, practico),
                "5" => () => OpcionVerificarAsistencia(),
                "6" => () => OpcionMostrarRecuperacion(clase),
                "7" => () => {
                        RegistrarCreditos(clase);
                        RegistrarNotas(clase);
                },
                _   => () => {}
            };
            action();
            Consola.EsperarTecla();
        }
    }
}