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
    static Contacto[] contactos = new Contacto[MAX_CONTACTOS];
    static int cantidadContactos = 0;
    static int proximoId = 1;

    static void Main()
    {
        CargarDesdeArchivo();

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
            if (!int.TryParse(Console.ReadLine(), out opcion)) opcion = -1;

            Console.Clear();
            switch (opcion)
            {
                case 1: AgregarContacto(); break;
                case 2: ModificarContacto(); break;
                case 3: BorrarContacto(); break;
                case 4: ListarContactos(); break;
                case 5: BuscarContacto(); break;
                case 0: GuardarEnArchivo(); Console.WriteLine("Saliendo de la aplicación..."); break;
                default: Console.WriteLine("Opción inválida."); break;
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
            Console.WriteLine("No se pueden agregar más contactos.");
            return;
        }

        Console.WriteLine("=== Agregar Contacto ===");
        Console.Write("Nombre   : ");
        string nombre = Console.ReadLine();
        Console.Write("Teléfono : ");
        string telefono = Console.ReadLine();
        Console.Write("Email    : ");
        string email = Console.ReadLine();

        contactos[cantidadContactos].Id = proximoId++;
        contactos[cantidadContactos].Nombre = nombre;
        contactos[cantidadContactos].Telefono = telefono;
        contactos[cantidadContactos].Email = email;
        cantidadContactos++;

        Console.WriteLine($"Contacto agregado con ID = {proximoId - 1}");
    }

    static void ModificarContacto()
    {
        Console.WriteLine("=== Modificar Contacto ===");
        Console.Write("Ingrese el ID del contacto a modificar: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;

        int index = BuscarPorId(id);
        if (index == -1)
        {
            Console.WriteLine("ID no encontrado.");
            return;
        }

        Console.WriteLine($"Datos actuales => Nombre: {contactos[index].Nombre}, Teléfono : {contactos[index].Telefono}, Email: {contactos[index].Email}");
        Console.WriteLine("(Deje el campo en blanco para no modificar)");

        Console.Write("Nombre    : ");
        string nombre = Console.ReadLine();
        if (nombre != "") contactos[index].Nombre = nombre;

        Console.Write("Teléfono  : ");
        string telefono = Console.ReadLine();
        if (telefono != "") contactos[index].Telefono = telefono;

        Console.Write("Email     : ");
        string email = Console.ReadLine();
        if (email != "") contactos[index].Email = email;

        Console.WriteLine("Contacto modificado con éxito.");
    }

    static void BorrarContacto()
    {
        Console.WriteLine("=== Borrar Contacto ===");
        Console.Write("Ingrese el ID del contacto a borrar: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;

        int index = BuscarPorId(id);
        if (index == -1)
        {
            Console.WriteLine("ID no encontrado.");
            return;
        }

        for (int i = index; i < cantidadContactos - 1; i++)
        {
            contactos[i] = contactos[i + 1];
        }
        cantidadContactos--;
        Console.WriteLine($"Contacto con ID={id} eliminado con éxito.");
    }

    static void ListarContactos()
    {
        Console.WriteLine("=== Lista de Contactos ===");
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");
        for (int i = 0; i < cantidadContactos; i++)
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}",
                contactos[i].Id,
                contactos[i].Nombre,
                contactos[i].Telefono,
                contactos[i].Email);
        }
    }

    static void BuscarContacto()
    {
        Console.WriteLine("=== Buscar Contacto ===");
        Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
        string termino = Console.ReadLine().ToLower();

        Console.WriteLine("Resultados de la búsqueda:");
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(termino) ||
                contactos[i].Telefono.ToLower().Contains(termino) ||
                contactos[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}",
                    contactos[i].Id,
                    contactos[i].Nombre,
                    contactos[i].Telefono,
                    contactos[i].Email);
            }
        }
    }

    static int BuscarPorId(int id)
    {
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (contactos[i].Id == id) return i;
        }
        return -1;
    }

    static void CargarDesdeArchivo()
    {
        if (!File.Exists("agenda.csv")) return;

        string[] lineas = File.ReadAllLines("agenda.csv");
        for (int i = 0; i < lineas.Length && cantidadContactos < MAX_CONTACTOS; i++)
        {
            string[] partes = lineas[i].Split(',');
            if (partes.Length == 4)
            {
                contactos[cantidadContactos].Id = int.Parse(partes[0]);
                contactos[cantidadContactos].Nombre = partes[1];
                contactos[cantidadContactos].Telefono = partes[2];
                contactos[cantidadContactos].Email = partes[3];
                cantidadContactos++;

                if (contactos[cantidadContactos - 1].Id >= proximoId)
                    proximoId = contactos[cantidadContactos - 1].Id + 1;
            }
        }
    }

    static void GuardarEnArchivo()
    {
        using (StreamWriter writer = new StreamWriter("agenda.csv"))
        {
            for (int i = 0; i < cantidadContactos; i++)
            {
                writer.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
            }
        }
    }
}
 
