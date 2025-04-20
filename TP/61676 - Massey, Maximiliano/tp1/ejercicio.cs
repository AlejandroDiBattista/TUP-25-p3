using static System.Console;

struct Contacto
{
    public int id;
    public string nombre;
    public string telefono;
    public string email;
}

class Program
{
    const int MaxContactos = 100;
    static Contacto[] agenda = new Contacto[MaxContactos];
    static int totalContactos = 0;
    static int Nid = 1;

    static void Main()
    {
        while (true)
        {
            MostrarMenu();
            string opciones = ReadLine();
            Clear();

            switch (opciones)
            {
                case "1":
                    AgregarContacto();
                    break;
                case "2":
                    ModificarContacto();
                    break;
                case "3":
                    BorrarContacto();
                    break;
                case "4":
                    ListarContacto();
                    break;
                case "5":
                    BuscarContacto();
                    break;
                case "6":
                    Salir();
                    break;
                default:
                    WriteLine("Opción no válida. Intente de nuevo.");
                    break;
            }
        }
    }
    static void MostrarMenu()
    {
        WriteLine("===== AGENDA DE CONTACTOS =====");

        WriteLine("1. Agregar contacto");

        WriteLine("2. Modificar contacto");

        WriteLine("3. Borrar contacto");

        WriteLine("4. Listar contactos");

        WriteLine("5. Buscar contacto");

        WriteLine("6. Salir");
    }
    static void AgregarContacto()
    {
        if (totalContactos >= MaxContactos)
        {
            WriteLine("LA lista se a llenado, si quiere agregar mas contactos debe borrar uno o agrandar la lista deslde el codigo.");
            return;
        }
        WriteLine("=== AGREGAR CONTACTO ===");
        Write("Nombre: ");
        string nombre = ReadLine();
        Write("Teléfono: ");
        string telefono = ReadLine();
        Write("Email: ");
        string email = ReadLine();
        agenda[totalContactos] = new Contacto { id = Nid++, nombre = nombre, telefono = telefono, email = email };
        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(email))
        {
            WriteLine("por favor intente ingresar los datos de todas las casilas.");
            return;
        }
        totalContactos++;
        WriteLine("Su contacto fue agregado de forma exitosa :)");
    }
    static void ModificarContacto()
    {
        WriteLine("=== MODIFICAR CONTACTO ===");
        Write("Ingrese el ID del contacto que desea modificar: ");
        if (!int.TryParse(ReadLine(), out int idIngresado))
        {
            WriteLine("ID inválido. Intente nuevamente.");
            return;
        }

        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].id == idIngresado)
            {
                WriteLine($"Los datos que estan actual mente en la lista son: \nNombre: {agenda[i].nombre} \nTelefono: {agenda[i].telefono} \nEmail: {agenda[i].email}");
                WriteLine("Ingrese los nuevos datos: ");
                Write("Nuevo Nombre: ");
                string nuevoNombre = ReadLine();
                Write("Nuevo Telefono: ");
                string nuevoTelefono = ReadLine();
                Write("Nuevo Email: ");
                string nuevoEmail = ReadLine();

                if (!string.IsNullOrEmpty(nuevoNombre))
                    agenda[i].nombre = nuevoNombre;
                if (!string.IsNullOrEmpty(nuevoTelefono))
                    agenda[i].telefono = nuevoTelefono;
                if (!string.IsNullOrEmpty(nuevoEmail))
                    agenda[i].email = nuevoEmail;

                WriteLine("Los datos fueron modificados de forma exitosa.");
                return;
            }
        }
        WriteLine("El contacto no fue encontrado. Verifique el ID e intente nuevamente.");
    }

    static void BorrarContacto()
    {
        WriteLine("=== BORRAR CONTACTO ===");
        Write("Ingrese el ID del contacto que desea borrar: ");
        if (!int.TryParse(ReadLine(), out int idIngresado))
        {
            WriteLine("ID inválido. Intente nuevamente.");
            return;
        }

        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].id == idIngresado)
            {
                for (int j = i; j < totalContactos - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                totalContactos--;
                WriteLine("Contacto borrado con éxito.");
                return;
            }
        }
        WriteLine("Contacto no encontrado.");
    }

    static void ListarContacto()
    {
        WriteLine("=== LISTA DE CONTACTOS ===");
        WriteLine("ID\tNombre\tTeléfono\tEmail");
        for (int i = 0; i < totalContactos; i++)
        {
            WriteLine($"ID: {agenda[i].id}, Nombre: {agenda[i].nombre}, Teléfono: {agenda[i].telefono}, Email: {agenda[i].email}");
        }
    }

    static void BuscarContacto()
    {
        WriteLine("===BUSCAR CONTACTO===");
        WriteLine("Ingrese algun termino de busqueda para encontrar su conactito: ");
        string busqueda = ReadLine().ToLower();
        WriteLine("ID\tNombre\tTeléfono\tEmail");
        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].nombre.ToLower().Contains(busqueda) ||
                agenda[i].telefono.ToLower().Contains(busqueda) ||
                agenda[i].email.ToLower().Contains(busqueda))
            {
                WriteLine($"ID: {agenda[i].id}, Nombre: {agenda[i].nombre}, Teléfono: {agenda[i].telefono}, Email: {agenda[i].email}");
            }
        }
    }
    static void Salir()
    {
        WriteLine("Saliendo de la agenda. ¡Hasta luego!");
        Environment.Exit(0);
    }
}