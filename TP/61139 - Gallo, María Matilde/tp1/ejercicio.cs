using System;
using System.IO;

class Program
{
    
    struct Contacto
    {
        public int Id;
        public string Nombre;
        public string Telefono;
        public string Email;
    }

    static void Main()
    {
      
        const int MAX_CONTACTOS = 100;
        Contacto[] contactos = new Contacto[MAX_CONTACTOS];
        int contactoCount = 0;
        
        
        CargarContactos(ref contactos, ref contactoCount);
        
        bool salir = false;
        while (!salir)
        {
           
            MostrarMenu();
            string opcion = Console.ReadLine();
            
            switch (opcion)
            {
                case "1":
                    AgregarContacto(ref contactos, ref contactoCount);
                    break;
                case "2":
                    ModificarContacto(ref contactos, contactoCount);
                    break;
                case "3":
                    BorrarContacto(ref contactos, ref contactoCount);
                    break;
                case "4":
                    ListarContactos(contactos, contactoCount);
                    break;
                case "5":
                    BuscarContacto(contactos, contactoCount);
                    break;
                case "6":
                    Salir(ref contactos, contactoCount);
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción no válida. Intente nuevamente.");
                    break;
            }
        }
    }

    
    static void MostrarMenu()
    {
        Console.Clear();
        Console.WriteLine("----- Menú de Agenda de Contactos -----");
        Console.WriteLine("1. Agregar contacto");
        Console.WriteLine("2. Modificar contacto");
        Console.WriteLine("3. Borrar contacto");
        Console.WriteLine("4. Listar contactos");
        Console.WriteLine("5. Buscar contacto");
        Console.WriteLine("6. Salir");
        Console.Write("Seleccione una opción: ");
    }

    
    static void AgregarContacto(ref Contacto[] contactos, ref int contactoCount)
    {
        if (contactoCount >= contactos.Length)
        {
            Console.WriteLine("La agenda está llena. No se pueden agregar más contactos.");
            return;
        }

        Console.Write("Nombre del contacto: ");
        string nombre = Console.ReadLine();

        Console.Write("Teléfono del contacto: ");
        string telefono = Console.ReadLine();

        Console.Write("Correo electrónico del contacto: ");
        string email = Console.ReadLine();

        Contacto nuevoContacto;
        nuevoContacto.Id = contactoCount + 1;  
        nuevoContacto.Nombre = nombre;
        nuevoContacto.Telefono = telefono;
        nuevoContacto.Email = email;

        contactos[contactoCount] = nuevoContacto;
        contactoCount++;

        Console.WriteLine("Contacto agregado exitosamente.");
        Console.ReadKey();
    }

   
    static void ModificarContacto(ref Contacto[] contactos, int contactoCount)
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());

        bool encontrado = false;

        for (int i = 0; i < contactoCount; i++)
        {
            if (contactos[i].Id == id)
            {
                encontrado = true;

                Console.WriteLine("Modificar contacto:");
                Console.Write("Nuevo nombre (deje en blanco para no cambiar): ");
                string nuevoNombre = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoNombre)) contactos[i].Nombre = nuevoNombre;

                Console.Write("Nuevo teléfono (deje en blanco para no cambiar): ");
                string nuevoTelefono = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoTelefono)) contactos[i].Telefono = nuevoTelefono;

                Console.Write("Nuevo correo electrónico (deje en blanco para no cambiar): ");
                string nuevoEmail = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoEmail)) contactos[i].Email = nuevoEmail;

                Console.WriteLine("Contacto modificado exitosamente.");
                break;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("Contacto no encontrado.");
        }

        Console.ReadKey();
    }

   
    static void BorrarContacto(ref Contacto[] contactos, ref int contactoCount)
    {
        Console.Write("Ingrese el ID del contacto a borrar: ");
        int id = int.Parse(Console.ReadLine());

        bool encontrado = false;
        for (int i = 0; i < contactoCount; i++)
        {
            if (contactos[i].Id == id)
            {
                encontrado = true;

                
                for (int j = i; j < contactoCount - 1; j++)
                {
                    contactos[j] = contactos[j + 1];
                }

                contactoCount--;
                Console.WriteLine("Contacto borrado exitosamente.");
                break;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("Contacto no encontrado.");
        }

        Console.ReadKey();
    }

   
    static void ListarContactos(Contacto[] contactos, int contactoCount)
    {
        Console.WriteLine("ID\tNombre\t\tTeléfono\t\tCorreo Electrónico");
        Console.WriteLine("---------------------------------------------------------------");

        for (int i = 0; i < contactoCount; i++)
        {
            Console.WriteLine($"{contactos[i].Id}\t{contactos[i].Nombre}\t{contactos[i].Telefono}\t{contactos[i].Email}");
        }

        Console.ReadKey();
    }

    
    static void BuscarContacto(Contacto[] contactos, int contactoCount)
    {
        Console.Write("Ingrese el término de búsqueda: ");
        string busqueda = Console.ReadLine().ToLower();

        bool encontrado = false;

        Console.WriteLine("ID\tNombre\t\tTeléfono\t\tCorreo Electrónico");
        Console.WriteLine("---------------------------------------------------------------");

        for (int i = 0; i < contactoCount; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(busqueda) ||
                contactos[i].Telefono.ToLower().Contains(busqueda) ||
                contactos[i].Email.ToLower().Contains(busqueda))
            {
                Console.WriteLine($"{contactos[i].Id}\t{contactos[i].Nombre}\t{contactos[i].Telefono}\t{contactos[i].Email}");
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("No se encontraron resultados.");
        }

        Console.ReadKey();
    }

    
    static void Salir(ref Contacto[] contactos, int contactoCount)
    {
        using (StreamWriter writer = new StreamWriter("agenda.csv"))
        {
            for (int i = 0; i < contactoCount; i++)
            {
                writer.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
            }
        }
        Console.WriteLine("Los contactos se han guardado correctamente.");
    }

    
    static void CargarContactos(ref Contacto[] contactos, ref int contactoCount)
    {
        if (File.Exists("agenda.csv"))
        {
            string[] lineas = File.ReadAllLines("agenda.csv");

            foreach (string linea in lineas)
            {
                string[] datos = linea.Split(',');
                if (datos.Length == 4)
                {
                    Contacto contacto;
                    contacto.Id = int.Parse(datos[0]);
                    contacto.Nombre = datos[1];
                    contacto.Telefono = datos[2];
                    contacto.Email = datos[3];

                    contactos[contactoCount] = contacto;
                    contactoCount++;
                }
            }
        }
    }
}
