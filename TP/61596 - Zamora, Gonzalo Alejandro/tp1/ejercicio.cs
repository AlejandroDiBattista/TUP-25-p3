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
    static Contacto[] contactos = new Contacto[MAX_CONTACTOS];
    static int contadorId = 0;

    static void Main(string[] args)
    {
        CargarContactos();

        while (true)
        {
            Console.WriteLine("Menú:");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");

            Console.Write("Ingrese su opción: ");
            int opcion = Convert.ToInt32(Console.ReadLine());

            switch (opcion)
            {
                case 1:
                    AgregarContacto();
                    break;
                case 2:
                    ModificarContacto();
                    break;
                case 3:
                    BorrarContacto();
                    break;
                case 4:
                    ListarContactos();
                    break;
                case 5:
                    BuscarContacto();
                    break;
                case 6:
                    GuardarContactos();
                    return;
                default:
                    Console.WriteLine("Opción inválida. Inténtelo de nuevo.");
                    break;
            }
        }
    }

    static void AgregarContacto()
    {
        if (contadorId < MAX_CONTACTOS)
        {
            Console.Write("Ingrese el nombre del contacto: ");
            string nombre = Console.ReadLine();

            Console.Write("Ingrese el teléfono del contacto: ");
            string telefono = Console.ReadLine();

            Console.Write("Ingrese el email del contacto: ");
            string email = Console.ReadLine();

            Contacto contacto = new Contacto
            {
                Id = contadorId + 1,
                Nombre = nombre,
                Telefono = telefono,
                Email = email
            };

            contactos[contadorId] = contacto;
            contadorId++;

            Console.WriteLine("Contacto agregado con éxito.");
        }
        else
        {
            Console.WriteLine("No se pueden agregar más contactos.");
        }
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = Convert.ToInt32(Console.ReadLine());

        for (int i = 0; i < contadorId; i++)
        {
            if (contactos[i].Id == id)
            {
                Console.Write("Ingrese el nuevo nombre del contacto (presione Enter para no cambiar): ");
                string nombre = Console.ReadLine();
                if (nombre != "") contactos[i].Nombre = nombre;

                Console.Write("Ingrese el nuevo teléfono del contacto (presione Enter para no cambiar): ");
                string telefono = Console.ReadLine();
                if (telefono != "") contactos[i].Telefono = telefono;

                Console.Write("Ingrese el nuevo email del contacto (presione Enter para no cambiar): ");
                string email = Console.ReadLine();
                if (email != "") contactos[i].Email = email;

                Console.WriteLine("Contacto modificado con éxito.");
                return;
            }
        }

        Console.WriteLine("Contacto no encontrado.");
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese el ID del contacto a borrar: ");
        int id = Convert.ToInt32(Console.ReadLine());

        for (int i = 0; i < contadorId; i++)
        {
            if (contactos[i].Id == id)
            {
                for (int j = i; j < contadorId - 1; j++)
                {
                    contactos[j] = contactos[j + 1];
                }

                contadorId--;
                Console.WriteLine("Contacto borrado con éxito.");
                return;
            }
        }

        Console.WriteLine("Contacto no encontrado.");
    }

    static void ListarContactos()
    {
        Console.WriteLine("Lista de contactos:");
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "Nombre", "Teléfono", "Email");

        for (int i = 0; i < contadorId; i++)
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
        }
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese el término de búsqueda: ");
        string termino = Console.ReadLine().ToLower();

        Console.WriteLine("Resultados de la búsqueda:");
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "Nombre", "Teléfono", "Email");

        bool encontrado = false;
        for (int i = 0; i < contadorId; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(termino) ||
                contactos[i].Telefono.ToLower().Contains(termino) ||
                contactos[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("No se encontraron resultados.");
        }
    }

    static void CargarContactos()
    {
        if (File.Exists("agenda.csv"))
        {
            string[] lineas = File.ReadAllLines("agenda.csv");

            foreach (string linea in lineas)
            {
                string[] campos = linea.Split(';');

                if (campos.Length == 4)
                {
                    Contacto contacto = new Contacto
                    {
                        Id = Convert.ToInt32(campos[0]),
                        Nombre = campos[1],
                        Telefono = campos[2],
                        Email = campos[3]
                    };

                    if (contadorId < MAX_CONTACTOS)
                    {
                        contactos[contadorId] = contacto;
                        contadorId++;
                    }
                }
            }
        }
    }

    static void GuardarContactos()
    {
        string[] lineas = new string[contadorId];

        for (int i = 0; i < contadorId; i++)
        {
            lineas[i] = string.Format("{0};{1};{2};{3}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
        }

        File.WriteAllLines("agenda.csv", lineas);
    }
}