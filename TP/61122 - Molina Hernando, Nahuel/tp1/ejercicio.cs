using System;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Correo; 
}

class Programa
{
    const int MAX_CONTACTOS = 100;
    static Contacto[] contactos = new Contacto[MAX_CONTACTOS];
    static int contadorContactos = 0;

    static void AgregarContacto()
    {
        if (contadorContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("No se pueden agregar más contactos.");
            Console.ReadKey();
            return;
        }

        Contacto nuevoContacto = new Contacto();
        nuevoContacto.Id = contadorContactos + 1;
        Console.WriteLine("Ingrese los datos del nuevo contacto:");
        Console.Write("Nombre: ");
        nuevoContacto.Nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        nuevoContacto.Telefono = Console.ReadLine();
        Console.Write("Correo: ");
        nuevoContacto.Correo = Console.ReadLine();

        contactos[contadorContactos] = nuevoContacto;
        contadorContactos++;

        Console.WriteLine("Contacto agregado exitosamente.");
        Console.ReadKey();
    }

    static void BorrarContacto()
    {
        Console.WriteLine("Borrar contacto");
        Console.Write("Ingrese el ID del contacto a borrar: ");
        if (!int.TryParse(Console.ReadLine(), out int id) || id < 1 || id > contadorContactos)
        {
            Console.WriteLine("ID inválido.");
            Console.ReadKey();
            return;
        }

        for (int i = id - 1; i < contadorContactos - 1; i++)
        {
            contactos[i] = contactos[i + 1];
        }
        contadorContactos--;

        Console.WriteLine("Contacto borrado exitosamente.");
        Console.ReadKey();
    }

    static void BuscarContacto()
    {
        Console.WriteLine("Buscar contacto");
        Console.Write("Ingrese el nombre del contacto a buscar: ");
        string nombre = Console.ReadLine().ToLower();

        bool encontrado = false;
        for (int i = 0; i < contadorContactos; i++)
        {
            Contacto contacto = contactos[i];
            if (contacto.Nombre.ToLower().Contains(nombre))
            {
                Console.WriteLine($"ID: {contacto.Id}, Nombre: {contacto.Nombre}, Teléfono: {contacto.Telefono}, Correo: {contacto.Correo}");
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("Contacto no encontrado.");
        }
        Console.ReadKey();
    }

    static void CargarContactosDesdeArchivo()
    {
        if (!File.Exists("agenda.csv")) return;

        string[] lineas = File.ReadAllLines("agenda.csv");
        foreach (string linea in lineas)
        {
            string[] datos = linea.Split(',');
            if (datos.Length == 4)
            {
                contactos[contadorContactos] = new Contacto
                {
                    Id = int.Parse(datos[0]),
                    Nombre = datos[1],
                    Telefono = datos[2],
                    Correo = datos[3]
                };
                contadorContactos++;
            }
        }
    }

    static void GuardarContactosEnArchivo()
    {
        using (StreamWriter sw = new StreamWriter("agenda.csv"))
        {
            for (int i = 0; i < contadorContactos; i++)
            {
                sw.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Correo}");
            }
        }
    }

    static void ListarContactos()
    {
        Console.WriteLine("Lista de contactos:");
        for (int i = 0; i < contadorContactos; i++)
        {
            Contacto contacto = contactos[i];
            Console.WriteLine($"ID: {contacto.Id}, Nombre: {contacto.Nombre}, Teléfono: {contacto.Telefono}, Correo: {contacto.Correo}");
        }
        Console.ReadKey();
    }

    static void Main(string[] args)
    {
        CargarContactosDesdeArchivo();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("======== Agenda de Contactos ========");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out int opcion))
            {
                Console.WriteLine("Por favor, ingrese un número válido.");
                Console.ReadKey();
                continue;
            }

            switch (opcion)
            {
                case 1:
                    AgregarContacto();
                    break;
                case 2:
                    ModificarContactos();
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
                    GuardarContactosEnArchivo();
                    Console.WriteLine("nos vemoss pronto");
                    return;
                default:
                    Console.WriteLine("Opción no válida. Intente de nuevo.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void ModificarContactos()
    {
        Console.WriteLine("Modificar contacto");
        Console.Write("Ingrese el ID del contacto a modificar: ");
        if (!int.TryParse(Console.ReadLine(), out int id) || id < 1 || id > contadorContactos)
        {
            Console.WriteLine("ID inválido.");
            Console.ReadKey();
            return;
        }

        Contacto contacto = contactos[id - 1];
        Console.WriteLine($"Modificar contacto: {contacto.Nombre}");
        Console.Write("Nuevo nombre (dejar vacío para no modificar): ");
        string nuevoNombre = Console.ReadLine();
        if (!string.IsNullOrEmpty(nuevoNombre))
            contacto.Nombre = nuevoNombre;

        Console.Write("Nuevo teléfono (dejar vacío para no modificar): ");
        string nuevoTelefono = Console.ReadLine();
        if (!string.IsNullOrEmpty(nuevoTelefono))
            contacto.Telefono = nuevoTelefono;

        Console.Write("Nuevo correo (dejar vacío para no modificar): ");
        string nuevoCorreo = Console.ReadLine();
        if (!string.IsNullOrEmpty(nuevoCorreo))
            contacto.Correo = nuevoCorreo;

        contactos[id - 1] = contacto;
        Console.WriteLine("Contacto modificado exitosamente.");
        Console.ReadKey();
    }
}