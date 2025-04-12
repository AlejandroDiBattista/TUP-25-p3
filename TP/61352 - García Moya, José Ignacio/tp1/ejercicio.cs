using System;
using System.IO;

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
    static int totalContactos = 0;
    static int siguienteId = 1;

    static void Main()
    {
        CargarContactos();
        int opcion;
        do
        {
            Console.Clear();
            Console.WriteLine("===== AGENDA DE CONTACTOS =====");
            Console.WriteLine("1) Agregar contacto");
            Console.WriteLine("2) Modificar contacto");
            Console.WriteLine("3) Borrar contacto");
            Console.WriteLine("4) Listar contactos");
            Console.WriteLine("5) Buscar contacto");
            Console.WriteLine("0) Salir");
            Console.Write("Seleccione una opción: ");
            opcion = int.Parse(Console.ReadLine());

            switch (opcion)
            {
                case 1: AgregarContacto(); break;
                case 2: ModificarContacto(); break;
                case 3: BorrarContacto(); break;
                case 4: ListarContactos(); break;
                case 5: BuscarContacto(); break;
                case 0: GuardarContactos(); break;
                default: Console.WriteLine("Opción inválida."); break;
            }
        } while (opcion != 0);
    }

    static void AgregarContacto()
    {
        if (totalContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("No se pueden agregar más contactos.");
            return;
        }
        Contacto nuevo;
        nuevo.Id = siguienteId++;
        Console.Write("Nombre: ");
        nuevo.Nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        nuevo.Telefono = Console.ReadLine();
        Console.Write("Email: ");
        nuevo.Email = Console.ReadLine();
        agenda[totalContactos++] = nuevo;
        Console.WriteLine("Contacto agregado con ID = " + nuevo.Id);
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());
        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                Console.WriteLine($"Datos actuales => Nombre: {agenda[i].Nombre}, Teléfono: {agenda[i].Telefono}, Email: {agenda[i].Email}");
                Console.Write("Nuevo Nombre (deje en blanco para no cambiar): ");
                string nombre = Console.ReadLine();
                if (!string.IsNullOrEmpty(nombre)) agenda[i].Nombre = nombre;
                Console.Write("Nuevo Teléfono: ");
                string telefono = Console.ReadLine();
                if (!string.IsNullOrEmpty(telefono)) agenda[i].Telefono = telefono;
                Console.Write("Nuevo Email: ");
                string email = Console.ReadLine();
                if (!string.IsNullOrEmpty(email)) agenda[i].Email = email;
                Console.WriteLine("Contacto modificado con éxito.");
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese el ID del contacto a borrar: ");
        int id = int.Parse(Console.ReadLine());
        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                for (int j = i; j < totalContactos - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                totalContactos--;
                Console.WriteLine("Contacto eliminado con éxito.");
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }

    static void ListarContactos()
    {
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        for (int i = 0; i < totalContactos; i++)
        {
            Console.WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
        }
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese un término de búsqueda: ");
        string termino = Console.ReadLine().ToLower();
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        for (int i = 0; i < totalContactos; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) ||
                agenda[i].Telefono.Contains(termino) ||
                agenda[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
            }
        }
    }

    static void CargarContactos()
    {
        if (!File.Exists("agenda.csv")) return;
        string[] lineas = File.ReadAllLines("agenda.csv");
        foreach (string linea in lineas)
        {
            string[] datos = linea.Split(';');
            Contacto c = new Contacto
            {
                Id = int.Parse(datos[0]),
                Nombre = datos[1],
                Telefono = datos[2],
                Email = datos[3]
            };
            agenda[totalContactos++] = c;
            siguienteId = c.Id + 1;
        }
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter("agenda.csv"))
        {
            for (int i = 0; i < totalContactos; i++)
            {
                sw.WriteLine($"{agenda[i].Id};{agenda[i].Nombre};{agenda[i].Telefono};{agenda[i].Email}");
            }
        }
    }
}
