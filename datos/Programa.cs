using TUP;

class Program {
    static string ElegirOpcionMenu() {
        Console.Clear();
        Consola.Escribir("=== MENÚ DE OPCIONES ===", ConsoleColor.Cyan);
        Consola.Escribir("1. Listar alumnos");
        Consola.Escribir("2. Exportar datos");
        Consola.Escribir("3. Normalizar carpetas");
        Consola.Escribir("4. Copiar trabajo práctico");
        Consola.Escribir("5. Verificar presentación de trabajo práctico");
        Consola.Escribir("6. Listar trabajos prácticos no presentados");
        Consola.Escribir("7. Listar alumnos que no presentaron nada");
        Consola.Escribir("8. Cargar asistencias");
        Consola.Escribir("0. Salir");
        return Consola.ElegirOpcion("\nElija una opción (0-8): ", "012345678");
    }

    static void RenameTP3Directories(string path="../TP") {
        foreach (var dir in Directory.GetDirectories(path)) {
            string folderName = Path.GetFileName(dir);
            if (folderName.Equals("TP3", StringComparison.Ordinal)) {
                string newDir = Path.Combine(Path.GetDirectoryName(dir)!, "tp3");
                Console.WriteLine($"Renombrando: {dir} -> {newDir}");
                Directory.Move(dir, newDir);
            }
        }
    }

    static void Main(string[] args) {
        RenameTP3Directories();
        var clase = Clase.Cargar();
        var practico = 3;

        Consola.Escribir("=== Bienvenido al sistema de gestión de alumnos ===", ConsoleColor.Cyan);
        while (true) {
            string opcion = ElegirOpcionMenu();
            if (opcion == "0") return;
            Console.Clear();
            Asistencias.Cargar();

            switch (opcion) {
                case "1":
                    Consola.Escribir("=== Listar alumnos ===", ConsoleColor.Cyan);
                    clase.ListarAlumnos();
                    break;
                case "2":
                    Consola.Escribir("=== Exportar datos ===", ConsoleColor.Cyan);
                    clase.ExportarDatos();
                    break;
                case "3":
                    Consola.Escribir("=== Normalizar carpetas ===", ConsoleColor.Cyan);
                    clase.NormalizarCarpetas();
                    break;
                case "4":
                    Consola.Escribir("=== Copiar trabajo práctico ===", ConsoleColor.Cyan);
                    string tp = Consola.LeerCadena("Ingrese el número del trabajo práctico a copiar (ej: 1): ", new[] { "1", "2", "3" });
                    bool forzar = Consola.Confirmar("¿Forzar copia incluso si ya existe?");

                    clase.CopiarPractico(int.Parse(tp), forzar);
                    break;
                case "5":
                    Consola.Escribir($"=== Verificar presentación de trabajo práctico ===", ConsoleColor.Cyan);
                    clase.NormalizarCarpetas();
                    clase.Reiniciar();
                    for(var p = 1; p <= practico; p++) {
                        clase.VerificaPresentacionPractico(p);
                    }
                    var asistencias = Asistencias.Cargar(false);
                    clase.CargarAsistencia(asistencias);
                    clase.Guardar();
                    clase = Clase.Cargar();
                    break;
                case "6":
                    Consola.Escribir("=== Listar trabajos prácticos no presentados ===", ConsoleColor.Cyan);
                    Consola.Escribir($"Resumen:\n  - Aprobados: {clase.ConPractico(1, EstadoPractico.Aprobado).Count()}\n  - Desaprobados: {clase.ConPractico(1,EstadoPractico.Desaprobado).Count()}\n  - No presentados: {clase.ConPractico(1,EstadoPractico.NoPresentado).Count()}", ConsoleColor.Cyan);
                    clase.ListarNoPresentaron(practico);
                    break;
                case "7":
                    Consola.Escribir("=== Listar alumnos que no presentaron nada ===", ConsoleColor.Cyan);
                    clase.ListarNoPresentaron(1);
                    break;
                case "8":
                    Consola.Escribir("=== Listar alumnos que no presentaron nada ===", ConsoleColor.Cyan);
                    Asistencias.Cargar(true);
                    break;
            }
            Consola.EsperarTecla();
        }
    }
}