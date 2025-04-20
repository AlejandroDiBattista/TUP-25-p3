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
        Consola.Escribir("0. Salir");
        return Consola.ElegirOpcion("\nElija una opción (0-6): ", "0123456");
    }


    static void OpcionListarAlumnos(Clase clase) {
        Consola.Escribir("=== Listado de alumnos ===", ConsoleColor.Cyan);
        clase.DebenRecuperar().ListarAlumnos();
        clase.ExportarDatos();
    }

    static void OpcionCopiarPractico(Clase clase) {
        Consola.Escribir("=== Copiar trabajo práctico ===", ConsoleColor.Cyan);
        string tp   = Consola.LeerCadena("Ingrese el número del trabajo práctico a copiar (ej: 1): ", new[] { "1", "2", "3" });
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
    }

    static void Main(string[] args) {
        var clase = Clase.Cargar();
        int practico = 3;

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
                _   => () => {}
            };
            action();
            Consola.EsperarTecla();
        }
    }
}