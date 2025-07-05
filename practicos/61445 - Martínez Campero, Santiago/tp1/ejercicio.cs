using System;
using System.IO;

public struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Gmail;

    public Contacto(int id, string nombre, string telefono, string gmail)
    {
        Id = id;
        Nombre = nombre;
        Telefono = telefono;
        Gmail = gmail;
    }
}

const int LimiteDeContactos = 100;
Contacto[] contactos = new Contacto[LimiteDeContactos];
int cantidadContactos = 0;

void MostrarMenuDeInicio()
{
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
        if (!int.TryParse(Console.ReadLine(), out opcion))
        {
            Console.WriteLine("Opción no válida. Intente de nuevo.");
            Console.ReadKey();
            continue;
        }

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
            case 0:
                GuardarContactosEnArchivo();
                Console.WriteLine("Saliendo de la aplicación...");
                break;
            default:
                Console.WriteLine("Opción no válida. Intente de nuevo.");
                Console.ReadKey();
                break;
        }
    } while (opcion != 0);
}

void AgregarContacto()
{
    if (cantidadContactos >= LimiteDeContactos)
    {
        Console.WriteLine("No se pueden agregar más contactos. Límite alcanzado.");
        Console.ReadKey();
        return;
    }

    Contacto nuevoContacto = new Contacto();
    nuevoContacto.Id = cantidadContactos + 1;

    Console.Write("Ingrese el nombre: ");
    nuevoContacto.Nombre = Console.ReadLine();
    Console.Write("Ingrese el teléfono: ");
    nuevoContacto.Telefono = Console.ReadLine();
    Console.Write("Ingrese el Gmail: ");
    nuevoContacto.Gmail = Console.ReadLine();

    contactos[cantidadContactos] = nuevoContacto;
    cantidadContactos++;

    Console.WriteLine($"Contacto agregado con ID = {nuevoContacto.Id}");
    Console.ReadKey();
}

void ModificarContacto()
{
    Console.Write("Ingrese el ID del contacto a modificar: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID no válido.");
        Console.ReadKey();
        return;
    }

    for (int i = 0; i < cantidadContactos; i++)
    {
        if (contactos[i].Id == id)
        {
            Console.WriteLine($"Datos actuales => Nombre: {contactos[i].Nombre}, Teléfono: {contactos[i].Telefono}, Gmail: {contactos[i].Gmail}");
            Console.WriteLine("(Deja el campo en blanco para no modificar)");

            Console.Write("Nuevo nombre: ");
            string nombre = Console.ReadLine();
            if (!string.IsNullOrEmpty(nombre))
            {
                contactos[i].Nombre = nombre;
            }

            Console.Write("Nuevo teléfono: ");
            string telefono = Console.ReadLine();
            if (!string.IsNullOrEmpty(telefono))
            {
                contactos[i].Telefono = telefono;
            }

            Console.Write("Nuevo Gmail: ");
            string gmail = Console.ReadLine();
            if (!string.IsNullOrEmpty(gmail))
            {
                contactos[i].Gmail = gmail;
            }

            Console.WriteLine("Contacto modificado con éxito.");
            Console.ReadKey();
            return;
        }
    }
    Console.WriteLine("Contacto no encontrado.");
    Console.ReadKey();
}

void BorrarContacto()
{
    Console.Write("Ingrese el ID del contacto que desea borrar: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID no válido.");
        Console.ReadKey();
        return;
    }

    for (int i = 0; i < cantidadContactos; i++)
    {
        if (contactos[i].Id == id)
        {
            for (int j = i; j < cantidadContactos - 1; j++)
            {
                contactos[j] = contactos[j + 1];
            }
            cantidadContactos--;
            Console.WriteLine($"Contacto con ID={id} eliminado con éxito.");
            Console.ReadKey();
            return;
        }
    }

    Console.WriteLine("Contacto no encontrado.");
    Console.ReadKey();
}

void ListarContactos()
{
    Console.WriteLine("=== Lista de Contactos ===");
    Console.WriteLine("ID    NOMBRE               TELÉFONO       GMAIL");

    for (int i = 0; i < cantidadContactos; i++)
    {
        Console.WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Gmail}");
    }
    Console.ReadKey();
}

void BuscarContacto()
{
    Console.Write("Ingrese un término de búsqueda (nombre, teléfono o Gmail): ");
    string termino = Console.ReadLine().ToLower();
    Console.WriteLine("Resultados de la búsqueda:");
    Console.WriteLine("ID    NOMBRE               TELÉFONO       GMAIL");
    for (int i = 0; i < cantidadContactos; i++)
    {
        if (contactos[i].Nombre.ToLower().Contains(termino) ||
            contactos[i].Telefono.ToLower().Contains(termino) ||
            contactos[i].Gmail.ToLower().Contains(termino))
        {
            Console.WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Gmail}");
        }
    }
    Console.ReadKey();
}

void CargarContactosDesdeArchivo()
{
    if (File.Exists("agenda.csv"))
    {
        string[] lineas = File.ReadAllLines("agenda.csv");
        for (int i = 0; i < lineas.Length; i++)
        {
            if (cantidadContactos >= LimiteDeContactos)
            {
                Console.WriteLine("Se alcanzó el límite de contactos. No se cargarán más contactos del archivo.");
                break;
            }

            string[] datos = lineas[i].Split(',');
            if (datos.Length != 3)
            {
                Console.WriteLine($"Línea inválida en el archivo: {lineas[i]}");
                continue;
            }

            Contacto contacto = new Contacto
            {
                Id = cantidadContactos + 1,
                Nombre = datos[0].Trim(),
                Telefono = datos[1].Trim(),
                Gmail = datos[2].Trim()
            };

            contactos[cantidadContactos] = contacto;
            cantidadContactos++;
        }
    }
}

void GuardarContactosEnArchivo()
{
    string[] lineas = new string[cantidadContactos];

    for (int i = 0; i < cantidadContactos; i++)
    {
        lineas[i] = $"{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Gmail}";
    }

    File.WriteAllLines("agenda.csv", lineas);
}


CargarContactosDesdeArchivo();
MostrarMenuDeInicio();