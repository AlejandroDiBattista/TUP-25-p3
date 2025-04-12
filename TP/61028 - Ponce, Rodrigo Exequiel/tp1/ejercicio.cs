using System;
using System.IO;

public struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class Program
{
    const int MAX_CONTACTOS = 10;
    static Contacto[] contactos = new Contacto[MAX_CONTACTOS];
    static int totalContactos = 0;
    static int siguienteId = 1;
    const string ARCHIVO = "agenda.csv";

    
        CargarContactos();
        while (true)
        {
            Console.Clear();
            Console.WriteLine("MENU");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": AgregarContacto(); break;
                case "2": ModificarContacto(); break;
                case "3": BorrarContacto(); break;
                case "4": ListarContactos(); break;
                case "5": BuscarContacto(); break;
                case "6": GuardarContactos(); return;
                default: Console.WriteLine("Opción no válida. Presione ENTER para continuar..."); Console.ReadLine(); break;
            }
        }
    

    static void AgregarContacto()
    {
        if (totalContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("La agenda está llena.");
            return;
        }

        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();

        contactos[totalContactos] = new Contacto { Id = siguienteId++, Nombre = nombre, Telefono = telefono, Email = email };
        totalContactos++;
        Console.WriteLine("Contacto agregado. Presione ENTER para continuar...");
        Console.ReadLine();
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());
        for (int i = 0; i < totalContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                Console.Write($"Nuevo nombre ({contactos[i].Nombre}): ");
                string nombre = Console.ReadLine();
                Console.Write($"Nuevo teléfono ({contactos[i].Telefono}): ");
                string telefono = Console.ReadLine();
                Console.Write($"Nuevo email ({contactos[i].Email}): ");
                string email = Console.ReadLine();
                
                if (!string.IsNullOrWhiteSpace(nombre)) contactos[i].Nombre = nombre;
                if (!string.IsNullOrWhiteSpace(telefono)) contactos[i].Telefono = telefono;
                if (!string.IsNullOrWhiteSpace(email)) contactos[i].Email = email;
                
                Console.WriteLine("Contacto modificado. Presione ENTER para continuar...");
                Console.ReadLine();
                return;
            }
        }
        Console.WriteLine("ID no encontrado. Presione ENTER para continuar...");
        Console.ReadLine();
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese el ID del contacto a eliminar: ");
        int id = int.Parse(Console.ReadLine());
        for (int i = 0; i < totalContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                for (int j = i; j < totalContactos - 1; j++)
                {
                    contactos[j] = contactos[j + 1];
                }
                totalContactos--;
                Console.WriteLine("Contacto eliminado. Presione ENTER para continuar...");
                Console.ReadLine();
                return;
            }
        }
        Console.WriteLine("ID no encontrado. Presione ENTER para continuar...");
        Console.ReadLine();
    }

    static void ListarContactos()
    {
        Console.WriteLine("ID   Nombre                 Teléfono        Email");
        Console.WriteLine("--------------------------------------------------");
        for (int i = 0; i < totalContactos; i++)
        {
            Console.WriteLine($"{contactos[i].Id,-4} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email,-25}");
        }
        Console.WriteLine("Presione ENTER para continuar...");
        Console.ReadLine();
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese término de búsqueda: ");
        string termino = Console.ReadLine().ToLower();
        Console.WriteLine("ID   Nombre                 Teléfono        Email");
        Console.WriteLine("--------------------------------------------------");
        for (int i = 0; i < totalContactos; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(termino) ||
                contactos[i].Telefono.ToLower().Contains(termino) ||
                contactos[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{contactos[i].Id,-4} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email,-25}");
            }
        }
        Console.WriteLine("Presione ENTER para continuar...");
        Console.ReadLine();
    }

    static void CargarContactos()
    {
        if (File.Exists(ARCHIVO))
        {
            string[] lineas = File.ReadAllLines(ARCHIVO);
            foreach (string linea in lineas)
            {
                string[] datos = linea.Split(',');
                contactos[totalContactos] = new Contacto { Id = int.Parse(datos[0]), Nombre = datos[1], Telefono = datos[2], Email = datos[3] };
                totalContactos++;
                siguienteId = Math.Max(siguienteId, contactos[totalContactos - 1].Id + 1);
            }
        }
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter(ARCHIVO))
        {
            for (int i = 0; i < totalContactos; i++)
            {
                sw.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
            }
        }
    }
}