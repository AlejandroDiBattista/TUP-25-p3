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
    static int cantidadContactos = 0;

    static void Main()
    {
        LeerContactosDeArchivo();
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
            Console.Write("Seleccione una opción: ");
            opcion = int.Parse(Console.ReadLine());

            switch (opcion)
            {
                case 1: AgregarContacto(); break;
                case 2: ModificarContacto(); break;
                case 3: BorrarContacto(); break;
                case 4: ListarContactos(); break;
                case 5: BuscarContacto(); break;
                case 0: GuardarContactosEnArchivo(); Console.WriteLine("Saliendo..."); break;
                default: Console.WriteLine("Opción inválida."); break;
            }

            if (opcion != 0)
            {
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
            }

        } while (opcion != 0);
    }

    static void AgregarContacto()
    {
        if (cantidadContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("La agenda está llena.");
            return;
        }

        Contacto nuevoContacto;
        nuevoContacto.Id = cantidadContactos + 1;

        Console.WriteLine("=== Agregar Contacto ===");
        Console.Write("Nombre   : ");
        nuevoContacto.Nombre = Console.ReadLine();
        Console.Write("Teléfono : ");
        nuevoContacto.Telefono = Console.ReadLine();
        Console.Write("Email    : ");
        nuevoContacto.Email = Console.ReadLine();

        agenda[cantidadContactos] = nuevoContacto;
        cantidadContactos++;

        Console.WriteLine($"Contacto agregado con ID = {nuevoContacto.Id}");
    }

    static void ModificarContacto()
    {
        Console.WriteLine("=== Modificar Contacto ===");
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());

        int indice = BuscarIndicePorId(id);
        if (indice == -1)
        {
            Console.WriteLine("ID no encontrado.");
            return;
        }

        Contacto contacto = agenda[indice];
        Console.WriteLine($"Datos actuales => Nombre: {contacto.Nombre}, Teléfono: {contacto.Telefono}, Email: {contacto.Email}");
        Console.WriteLine("(Deje el campo en blanco para no modificar)");

        Console.Write("Nombre    : ");
        string nuevoNombre = Console.ReadLine();
        Console.Write("Teléfono  : ");
        string nuevoTelefono = Console.ReadLine();
        Console.Write("Email     : ");
        string nuevoEmail = Console.ReadLine();

        if (!string.IsNullOrEmpty(nuevoNombre)) contacto.Nombre = nuevoNombre;
        if (!string.IsNullOrEmpty(nuevoTelefono)) contacto.Telefono = nuevoTelefono;
        if (!string.IsNullOrEmpty(nuevoEmail)) contacto.Email = nuevoEmail;

        agenda[indice] = contacto;
        Console.WriteLine("Contacto modificado con éxito.");
    }

    static void BorrarContacto()
    {
        Console.WriteLine("=== Borrar Contacto ===");
        Console.Write("Ingrese el ID del contacto a borrar: ");
        int id = int.Parse(Console.ReadLine());

        int indice = BuscarIndicePorId(id);
        if (indice == -1)
        {
            Console.WriteLine("ID no encontrado.");
            return;
        }

        for (int i = indice; i < cantidadContactos - 1; i++)
        {
            agenda[i] = agenda[i + 1];
        }
        cantidadContactos--;
        Console.WriteLine($"Contacto con ID={id} eliminado con éxito.");
    }

    static void ListarContactos()
    {
        Console.WriteLine("=== Lista de Contactos ===");
        Console.WriteLine($"{"ID",-5}{"NOMBRE",-20}{"TELÉFONO",-15}{"EMAIL",-25}");
        Console.WriteLine(new string('-', 65));

        for (int i = 0; i < cantidadContactos; i++)
        {
            Contacto contacto = agenda[i];
            Console.WriteLine($"{contacto.Id,-5}{contacto.Nombre,-20}{contacto.Telefono,-15}{contacto.Email,-25}");
        }
    }

    static void BuscarContacto()
    {
        Console.WriteLine("=== Buscar Contacto ===");
        Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
        string termino = Console.ReadLine().ToLower();

        Console.WriteLine("Resultados de la búsqueda:");
        Console.WriteLine($"{"ID",-5}{"NOMBRE",-20}{"TELÉFONO",-15}{"EMAIL",-25}");
        Console.WriteLine(new string('-', 65));

        for (int i = 0; i < cantidadContactos; i++)
        {
            Contacto contacto = agenda[i];
            if (contacto.Nombre.ToLower().Contains(termino) ||
                contacto.Telefono.ToLower().Contains(termino) ||
                contacto.Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{contacto.Id,-5}{contacto.Nombre,-20}{contacto.Telefono,-15}{contacto.Email,-25}");
            }
        }
    }

    static int BuscarIndicePorId(int id)
    {
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].Id == id) return i;
        }
        return -1;
    }

    static void LeerContactosDeArchivo()
    {
        if (!File.Exists("agenda.csv")) return;

        string[] lineas = File.ReadAllLines("agenda.csv");
        foreach (string linea in lineas)
        {
            string[] datos = linea.Split(',');
            Contacto contacto = new Contacto
            {
                Id = int.Parse(datos[0]),
                Nombre = datos[1],
                Telefono = datos[2],
                Email = datos[3]
            };
            agenda[cantidadContactos] = contacto;
            cantidadContactos++;
        }
    }

    static void GuardarContactosEnArchivo()
    {
        using (StreamWriter sw = new StreamWriter("agenda.csv"))
        {
            for (int i = 0; i < cantidadContactos; i++)
            {
                Contacto contacto = agenda[i];
                sw.WriteLine($"{contacto.Id},{contacto.Nombre},{contacto.Telefono},{contacto.Email}");
            }
        }
    }
}