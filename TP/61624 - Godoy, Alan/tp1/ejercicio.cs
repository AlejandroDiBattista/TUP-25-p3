using static System.Console;
static class C
{
    public static void LimpiarPantalla()
    {
        Clear();
    }

    public static string Leer(string texto)
    {
        Write(texto);
        return ReadLine();
    }
}

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class Program
{
    const int MAX_CONTACTOS = 100;
    static Contacto[] agenda = new Contacto[MAX_CONTACTOS];
    static int cantidadContactos = 0;
    static int siguienteId = 1;
    static string archivo = "agenda.csv";

    static void Main()
    {
        CargarDesdeArchivo();
        while (true)
        {
            C.LimpiarPantalla();
            ForegroundColor = ConsoleColor.Blue;
            WriteLine("AGENDA DE CONTACTOS");
            ResetColor();
            WriteLine("1) Agregar contacto");
            WriteLine("2) Modificar contacto");
            WriteLine("3) Borrar contacto");
            WriteLine("4) Listar contactos");
            WriteLine("5) Buscar contacto");
            WriteLine("0) Salir");
            Write("Selecciona una opción: ");

            string opcion = C.Leer("");

            C.LimpiarPantalla();

            if (opcion == "1") AgregarContacto();
            else if (opcion == "2") ModificarContacto();
            else if (opcion == "3") BorrarContacto();
            else if (opcion == "4") ListarContactos();
            else if (opcion == "5") BuscarContacto();
            else if (opcion == "0")
            {
                GuardarEnArchivo();
                return;
            }
        }
    }

    static void AgregarContacto()
    {
        if (cantidadContactos >= MAX_CONTACTOS)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("No se pueden agregar más contactos.");
            ResetColor();
            ReadKey();
            return;
        }

        Contacto nuevo;
        nuevo.Id = siguienteId++;
        nuevo.Nombre = C.Leer("Nombre: ");
        nuevo.Telefono = C. Leer("Teléfono: ");
        nuevo.Email = C.Leer("Email: ");

        agenda[cantidadContactos] = nuevo;
        cantidadContactos++;
        ForegroundColor = ConsoleColor.Green;
        WriteLine("Contacto agregado con ID = " + nuevo.Id);
        ResetColor();
        ReadKey();
    }

    static void ModificarContacto()
    {
        if(cantidadContactos == 0)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("No hay contactos en la agenda.");
            ResetColor();
            ReadKey();
            return;
        }
        Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(C.Leer(""));

        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                WriteLine($"Actual: Nombre: {agenda[i].Nombre}, Teléfono: {agenda[i].Telefono}, Email: {agenda[i].Email}");

                string nombre = C.Leer("Nuevo Nombre (Enter para no cambiar): ");
                if (!string.IsNullOrEmpty(nombre)) agenda[i].Nombre = nombre;

                string telefono = C.Leer("Nuevo Teléfono (Enter para no cambiar): ");
                if (!string.IsNullOrEmpty(telefono)) agenda[i].Telefono = telefono;

                string email = C.Leer("Nuevo Email (Enter para no cambiar): ");
                if (!string.IsNullOrEmpty(email)) agenda[i].Email = email;
                ForegroundColor = ConsoleColor.Green;
                WriteLine("Contacto modificado con éxito.");
                ResetColor();
                ReadKey();
                return;
            }
        }

        ForegroundColor = ConsoleColor.Red;
        WriteLine("ID no encontrado.");
        ResetColor();
        ReadKey();
    }

    static void BorrarContacto()
    {
        if(cantidadContactos == 0)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("No hay contactos en la agenda.");
            ResetColor();
            ReadKey();
            return;
        }

        Write("Ingrese el ID del contacto a borrar: ");
        int id = int.Parse(C.Leer(""));

        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                for (int j = i; j < cantidadContactos - 1; j++)
                    agenda[j] = agenda[j + 1];
                cantidadContactos--;
                ForegroundColor = ConsoleColor.Green;
                WriteLine("Contacto eliminado con éxito.");
                ResetColor ();
                ReadKey();
                return;
            }
        }
        ForegroundColor = ConsoleColor.Red;
        WriteLine("ID no encontrado.");
        ResetColor();
        ReadKey();

    }

    static void ListarContactos()
    {
        if(cantidadContactos == 0)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("No hay contactos en la agenda.");
            ResetColor();
            ReadKey();
            return;
        }
        WriteLine("ID    Nombre               Teléfono       Email");
        for (int i = 0; i < cantidadContactos; i++)
        {
            WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
        }
        ReadKey();
    }

    static void BuscarContacto()
    {
        if(cantidadContactos == 0)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("No hay contactos en la agenda.");
            ResetColor();
            ReadKey();
            return;
        }
        Write("Ingrese un término de búsqueda: ");
        string termino = C.Leer("").ToLower();

        WriteLine("ID    Nombre               Teléfono       Email");
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) ||
                agenda[i].Telefono.Contains(termino) ||
                agenda[i].Email.ToLower().Contains(termino))
            {
                WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
            }
        }
        ReadKey();
    }

    static void CargarDesdeArchivo()
    {
        if (!File.Exists(archivo)) return;

        string[] lineas = File.ReadAllLines(archivo);
        foreach (var linea in lineas)
        {
            string[] partes = linea.Split(',');
            if (partes.Length == 4 && int.TryParse(partes[0], out int id))
            {
                agenda[cantidadContactos].Id = id;
                agenda[cantidadContactos].Nombre = partes[1];
                agenda[cantidadContactos].Telefono = partes[2];
                agenda[cantidadContactos].Email = partes[3];
                cantidadContactos++;
                siguienteId = id + 1;
            }
        }
    }

    static void GuardarEnArchivo()
    {
    var lineas = new List<string>();
    for (int i = 0; i < cantidadContactos; i++)
    {
        lineas.Add($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
    }

    File.WriteAllLines(archivo, lineas);
    }
}
