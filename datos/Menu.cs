namespace TUP;

public class Menu {
    private readonly string Titulo;
    private readonly List<MenuItem> Opciones = new List<MenuItem>();

    private class MenuItem {
        public string Descripcion { get; }
        public Action Accion { get; }

        public MenuItem(string descripcion, Action accion) {
            Descripcion = descripcion;
            Accion = accion;
        }
    }

    public Menu(string titulo = "MENÚ DE OPCIONES") {
        Titulo = titulo;
    }

    public void Agregar(string descripcion, Action accion) {
        Opciones.Add(new MenuItem(descripcion, accion));
    }

    public void Ejecutar() {
        while (true) {
            Consola.Limpiar();
            Consola.Escribir($"=== {Titulo} ===", ConsoleColor.Cyan);
            for (int i = 0; i < Opciones.Count; i++) {
                Consola.Escribir($"{i + 1}. {Opciones[i].Descripcion}");
            }
            Consola.Escribir("0. Salir");
            var eleccion = Consola.ElegirOpcion($"\nElija una opción (0-{Opciones.Count}): ", Opciones.Count);

            if (eleccion == 0) break; 

            Action accionSeleccionada = Opciones[eleccion - 1].Accion;
            Console.Clear(); 
            accionSeleccionada();
            Consola.EsperarTecla(); 
        }
    }
}
