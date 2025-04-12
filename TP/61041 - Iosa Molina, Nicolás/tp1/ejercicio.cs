using System;
using System.IO;
using System.Text.RegularExpressions;
static int proximoId = 1;
struct Contacto
{
    public int id { get; private set; }
    public string nombre { get; set; }
    public string telefono { get; set; }
    public string email { get; set; }
    public Contacto(string nombre, string telefono, string email)
    {
        this.id = proximoId++;
        this.nombre = nombre;
        this.telefono = telefono;
        this.email = email;
    }
    public Contacto(int id, string nombre, string telefono, string email)
    {
        this.id = id;
        this.nombre = nombre;
        this.telefono = telefono;
        this.email = email;
    }
    public override string ToString()
    {
        return $"Nombre: {nombre}, Teléfono: {telefono}, Email: {email}";
    }
}
struct Consola
{
    public static void Limpiar()
    {
        Console.Clear();
    }
    public static void Escribir(string color, string texto)
    {
        Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color, true);
        Console.Write(texto);
        Console.ResetColor();
    }
    public static void EscribirLinea(string color, string texto)
    {
        Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color, true);
        Console.WriteLine(texto);
        Console.ResetColor();
    }
    public static int LeerEntero(string color)
    {
        Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color, true);
        string leido = Console.ReadLine();
        Console.ResetColor();
        return int.Parse(leido);
    }
    public static string LeerCadena(string color)
    {
        Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color, true);
        string leido = Console.ReadLine();
        Console.ResetColor();
        return leido;
    }
    public static void Error(string texto)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(texto);
        Console.ResetColor();
        Continuar();
    }
    public static void Continuar()
    {
        Consola.Escribir("Green", "Presione ");
        Consola.Escribir("Yellow", "cualquier tecla para continuar...");
        Console.ReadKey();
    }
}
static void Agenda()
{
    Contacto[] contactos = new Contacto[100];
    int cantidad = 0;
    ReadFile();
    Menu();
    int Push(string nombre, string telefono, string email)
    {
        Contacto contacto = new Contacto(nombre, telefono, email);
        contactos[cantidad++] = contacto;
        return contacto.id;
    }
    void Pop(int posicion)
    {
        if (posicion == -1)
        {
            return;
        }
        for (int i = posicion; i < cantidad - 1; i++)
        {
            contactos[i] = contactos[i + 1];
        }
        contactos[cantidad - 1] = new Contacto(0, null, null, null);
        cantidad--;
    }
    void ReadFile()
    {
        string[] lineas = File.ReadAllLines("agenda.csv");
        for (int i = 1; i < lineas.Length; i++)
        {
            string[] partes = lineas[i].Split(',');
            if (partes.Length == 3)
            {
                contactos[cantidad++] = new Contacto(partes[0], partes[1], partes[2]);
            }
            else
            {
                Consola.EscribirLinea("Red", $"Línea inválida en el archivo: {lineas[i]}");
            }
        }
    }
    void WriteFile()
    {
        string[] lineas = new string[cantidad + 1];
        lineas[0] = "nombre,telefono,email";
        for (int i = 0; i < cantidad; i++)
        {
            lineas[i + 1] = $"{contactos[i].nombre},{contactos[i].telefono},{contactos[i].email}";
        }
        File.WriteAllLines("agenda.csv", lineas);
    }
    void Modify(int posicion, string nombre, string telefono, string email)
    {
        if (nombre.Length > 0)
        {
            contactos[posicion].nombre = nombre;
        }
        if (telefono.Length > 0)
        {
            contactos[posicion].telefono = telefono;
        }
        if (email.Length > 0)
        {
            contactos[posicion].email = email;
        }
    }
    int FindByID(int _id)
    {
        for (int i = 0; i < cantidad; i++)
        {
            if (contactos[i].id == _id)
            {
                return i;
            }
        }
        return -1;
    }
    void FindByValues(string busqueda)
    {
        var encontro = 0;
        for (int i = 0; i < cantidad; i++)
        {
            if (contactos[i].nombre.IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0 ||
            contactos[i].telefono.IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0 ||
            contactos[i].email.IndexOf(busqueda, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                encontro++;
                if (encontro == 1)
                {
                    Consola.EscribirLinea("Green", "Resultados de la búsqueda:");
                    Consola.Escribir("Green", "ID    ");
                    Consola.Escribir("Yellow", "NOMBRE               TELÉFONO       EMAIL\n");
                }
                Consola.Escribir("Green", $"{contactos[i].id,-6}");
                Consola.Escribir("Yellow", $"{contactos[i].nombre,-21}");
                Consola.Escribir("Magenta", $"{contactos[i].telefono,-15}");
                Consola.EscribirLinea("Yellow", $"{contactos[i].email}");
            }
        }
        if (encontro == 0)
        {
            Consola.Error("No se encontraron resultados.");
        }
        else
        {
            Consola.Continuar();
        }
    }
    string validateNombre(string estado)
    {
        while (true)
        {
            Consola.Escribir("Green", "Nombre   ");
            Consola.Escribir("Yellow", ": ");
            var nombre = Consola.LeerCadena("Magenta");
            if (estado == "editando" && nombre.Length == 0)
            {
                return nombre;
            }
            if (nombre.Length > 1 && nombre.Length < 50 && Regex.IsMatch(nombre, @"^[a-zA-Z\sáéíóúÁÉÍÓÚñÑ]+$"))
            {
                return nombre;
            }
            else
            {
                Consola.Limpiar();
                Consola.EscribirLinea("Red", "Nombre inválido. Intente nuevamente.");
            }
        }
    }
    string validateTelefono(string estado)
    {
        while (true)
        {
            Consola.Escribir("Green", "Telefono ");
            Consola.Escribir("Yellow", ": ");
            var telefono = Consola.LeerCadena("Magenta");
            if (estado == "editando" && telefono.Length == 0)
            {
                return telefono;
            }
            if (telefono.Length > 5 && telefono.Length < 15 && Regex.IsMatch(telefono, @"^\+?[0-9\s\-()]+$"))
            {
                return telefono;
            }
            else
            {
                Consola.Limpiar();
                Consola.EscribirLinea("Red", "Teléfono inválido. Intente nuevamente.");
            }
        }
    }
    string validateEmail(string estado)
    {
        while (true)
        {
            Consola.Escribir("Green", "Email    ");
            Consola.Escribir("Yellow", ": ");
            var email = Consola.LeerCadena("Magenta");
            if (estado == "editando" && email.Length == 0)
            {
                return email;
            }
            if (email.Length > 0 && email.Length < 50 && Regex.IsMatch(email, @"^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[a-zA-Z0-9]+$"))
            {
                return email;
            }
            else
            {
                Consola.Limpiar();
                Consola.EscribirLinea("Red", "Email inválido. Intente nuevamente.");
            }
        }
    }
    void Menu()
    {
        while (true)
        {
            Consola.Limpiar();
            Consola.EscribirLinea("Yellow", "===== AGENDA DE CONTACTOS =====");
            Consola.Escribir("Green", "1");
            Consola.Escribir("Red", ") ");
            Consola.EscribirLinea("Yellow", "Agregar contacto");
            Consola.Escribir("Green", "2");
            Consola.Escribir("Red", ") ");
            Consola.EscribirLinea("Yellow", "Modificar contacto");
            Consola.Escribir("Green", "3");
            Consola.Escribir("Red", ") ");
            Consola.EscribirLinea("Yellow", "Borrar contactos");
            Consola.Escribir("Green", "4");
            Consola.Escribir("Red", ") ");
            Consola.EscribirLinea("Yellow", "Listar contactos");
            Consola.Escribir("Green", "5");
            Consola.Escribir("Red", ") ");
            Consola.EscribirLinea("Yellow", "Buscar contacto");
            Consola.Escribir("Green", "6");
            Consola.Escribir("Red", ") ");
            Consola.EscribirLinea("Yellow", "Salir");
            Consola.Escribir("Green", "Seleccione ");
            Consola.Escribir("Yellow", "una opción: ");
            string opcion = Consola.LeerCadena("Magenta");
            switch (opcion)
            {
                case "1":
                    MenuAgregar();
                    break;
                case "2":
                    MenuModificar();
                    break;
                case "3":
                    MenuBorrar();
                    break;
                case "4":
                    MenuListar();
                    break;
                case "5":
                    MenuBuscar();
                    break;
                case "6":
                    MenuSalir();
                    break;
                default:
                    Consola.Limpiar();
                    Consola.Error("Opción inválida.");
                    break;
            }
        }
    }
    void MenuAgregar()
    {
        Consola.Limpiar();
        if (cantidad >= 100)
        {
            Consola.Error("No se pueden agregar más contactos.");
            return;
        }
        Consola.EscribirLinea("Yellow", "=== Agregar Contacto ===");
        string nombre = validateNombre("");
        string telefono = validateTelefono("");
        string email = validateEmail("");
        var id = Push(nombre, telefono, email);
        Consola.Escribir("Green", "Contacto ");
        Consola.Escribir("Yellow", "agregado con ID = ");
        Consola.EscribirLinea("Magenta", id.ToString());
        WriteFile();
        Consola.Continuar();
    }
    void MenuModificar()
    {
        Consola.Limpiar();
        if (cantidad < 1)
        {
            Consola.Error("No hay contactos en la agenda.");
            return;
        }
        Consola.EscribirLinea("Yellow", "=== Modificar Contacto ===");
        Consola.Escribir("Green", "Ingrese ");
        Consola.Escribir("Yellow", "el ID del contacto a modificar: ");
        int id = Consola.LeerEntero("Magenta");
        int posicion = FindByID(id);
        if (posicion == -1)
        {
            Consola.Error("No se encontró el contacto.");
            return;
        }
        Consola.Escribir("Green", "\nDatos ");
        Consola.Escribir("Yellow", "actuales ");
        Consola.Escribir("Magenta", "=> ");
        Consola.Escribir("Yellow", contactos[posicion].ToString());
        Consola.EscribirLinea("Green", "\n(Deje el campo en blanco para no modificar)\n\n");
        string nombre = validateNombre("editando");
        string telefono = validateTelefono("editando");
        string email = validateEmail("editando");
        if (nombre.Length == 0 && telefono.Length == 0 && email.Length == 0)
        {
            Consola.Error("No se modificó ningún campo.");
            return;
        }
        Modify(posicion, nombre, telefono, email);
        Consola.Escribir("Green", "\nContacto ");
        Consola.EscribirLinea("Yellow", "modificado con éxito.");
        WriteFile();
        Consola.Continuar();
    }
    void MenuBorrar()
    {
        Consola.Limpiar();
        if (cantidad < 1)
        {
            Consola.Error("No hay contactos en la agenda.");
            return;
        }
        Consola.EscribirLinea("Yellow", "=== Borrar Contacto ===");
        Consola.Escribir("Green", "Ingrese ");
        Consola.Escribir("Yellow", "el ID del contacto a borrar: ");
        int id = Consola.LeerEntero("Magenta");
        var posicion = FindByID(id);
        if (posicion == -1)
        {
            Consola.Error("No se encontró el contacto.");
            return;
        }
        Pop(posicion);
        Consola.Escribir("Green", "\nContacto ");
        Consola.Escribir("Yellow", "con ID = ");
        Consola.Escribir("Magenta", id.ToString());
        Consola.EscribirLinea("Yellow", " eliminado con éxito.");
        Consola.Continuar();
    }
    void MenuListar()
    {
        Consola.Limpiar();
        if (cantidad <= 0)
        {
            Consola.Error("No hay contactos en la agenda.");
            return;
        }
        Consola.EscribirLinea("Yellow", "=== Lista de Contactos ===");
        Consola.Escribir("Green", "ID    ");
        Consola.Escribir("Yellow", "NOMBRE               TELÉFONO       EMAIL\n");
        for (int i = 0; i < cantidad; i++)
        {
            Consola.Escribir("Green", $"{contactos[i].id,-6}");
            Consola.Escribir("Yellow", $"{contactos[i].nombre,-21}");
            Consola.Escribir("Magenta", $"{contactos[i].telefono,-15}");
            Consola.EscribirLinea("Yellow", $"{contactos[i].email}");
        }
        WriteFile();
        Consola.Continuar();
    }
    void MenuBuscar()
    {
        Consola.Limpiar();
        if (cantidad <= 0)
        {
            Consola.Error("No hay contactos en la agenda.");
            return;
        }
        Consola.EscribirLinea("Yellow", "=== Buscar Contacto ===");
        Consola.Escribir("Green", "Ingrese ");
        Consola.Escribir("Yellow", "un término de búsqueda ");
        Consola.Escribir("Magenta", "(nombre, teléfono o email)");
        Consola.Escribir("Yellow", ": ");
        string busqueda = Consola.LeerCadena("Magenta");
        FindByValues(busqueda);
    }
    void MenuSalir()
    {
        Consola.Limpiar();
        Consola.Escribir("Green", "Saliendo ");
        Consola.Escribir("Yellow", "de la aplicación...");
        Environment.Exit(0);
    }
}
Agenda();