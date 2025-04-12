using System;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

const int MaxContactos = 100;
Contacto[] contactos = new Contacto[MaxContactos];
int cantidadContactos = 0;
int ultimoId = 0;
const string archivoAgenda = "agenda.csv";

CargarContactosDesdeArchivo();

while (true)
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
    string opcion = Console.ReadLine();

    switch (opcion)
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
            ListarContactos();
            break;
        case "5":
            BuscarContacto();
            break;
        case "0":
            GuardarContactosEnArchivo();
            Console.WriteLine("Saliendo de la aplicación...");
            return;
        default:
            Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
            Console.ReadKey();
            break;
    }
}

void CargarContactosDesdeArchivo()
{
    if (File.Exists(archivoAgenda))
    {
        string[] lineas = File.ReadAllLines(archivoAgenda);
        foreach (string linea in lineas)
        {
            string[] datos = linea.Split(',');
            if (datos.Length == 3 && cantidadContactos < MaxContactos)
            {
                contactos[cantidadContactos] = new Contacto
                {
                    Id = ++ultimoId,
                    Nombre = datos[0],
                    Telefono = datos[1],
                    Email = datos[2]
                };
                cantidadContactos++;
            }
        }
    }
}

void GuardarContactosEnArchivo()
{
    string[] lineas = new string[cantidadContactos];
    for (int i = 0; i < cantidadContactos; i++)
    {
        lineas[i] = $"{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}";
    }
    File.WriteAllLines(archivoAgenda, lineas);
}

void AgregarContacto()
{
    if (cantidadContactos >= MaxContactos)
    {
        Console.WriteLine("La agenda está llena. No se pueden agregar más contactos.");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("=== Agregar Contacto ===");
    Console.Write("Nombre   : ");
    string nombre = Console.ReadLine();
    Console.Write("Teléfono : ");
    string telefono = Console.ReadLine();
    Console.Write("Email    : ");
    string email = Console.ReadLine();

    contactos[cantidadContactos] = new Contacto
    {
        Id = ++ultimoId,
        Nombre = nombre,
        Telefono = telefono,
        Email = email
    };
    cantidadContactos++;

    Console.WriteLine($"Contacto agregado con ID = {ultimoId}");
    Console.WriteLine("Presione cualquier tecla para continuar...");
    Console.ReadKey();
}

void ModificarContacto()
{
    Console.WriteLine("=== Modificar Contacto ===");
    Console.Write("Ingrese el ID del contacto a modificar: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                Console.WriteLine($"Datos actuales => Nombre: {contactos[i].Nombre}, Teléfono: {contactos[i].Telefono}, Email: {contactos[i].Email}");
                Console.Write("(Deje el campo en blanco para no modificar)\n");

                Console.Write("Nombre    : ");
                string nombre = Console.ReadLine();
                Console.Write("Teléfono  : ");
                string telefono = Console.ReadLine();
                Console.Write("Email     : ");
                string email = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nombre)) contactos[i].Nombre = nombre;
                if (!string.IsNullOrWhiteSpace(telefono)) contactos[i].Telefono = telefono;
                if (!string.IsNullOrWhiteSpace(email)) contactos[i].Email = email;

                Console.WriteLine("Contacto modificado con éxito.");
                Console.ReadKey();
                return;
            }
        }
    }
    Console.WriteLine("ID no encontrado. Presione una tecla para continuar...");
    Console.ReadKey();
}

void BorrarContacto()
{
    Console.WriteLine("=== Borrar Contacto ===");
    Console.Write("Ingrese el ID del contacto a borrar: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
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
    }
    Console.WriteLine("ID no encontrado. Presione una tecla para continuar...");
    Console.ReadKey();
}

void ListarContactos()
{
    Console.WriteLine("=== Lista de Contactos ===");
    Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
    for (int i = 0; i < cantidadContactos; i++)
    {
        Console.WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email}");
    }
    Console.WriteLine("Presione cualquier tecla para continuar...");
    Console.ReadKey();
}

void BuscarContacto()
{
    Console.WriteLine("=== Buscar Contacto ===");
    Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
    string termino = Console.ReadLine()?.ToLower();

    Console.WriteLine("Resultados de la búsqueda:");
    Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
    for (int i = 0; i < cantidadContactos; i++)
    {
        if (contactos[i].Nombre.ToLower().Contains(termino) ||
            contactos[i].Telefono.ToLower().Contains(termino) ||
            contactos[i].Email.ToLower().Contains(termino))
        {
            Console.WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email}");
        }
    }
    Console.WriteLine("Presione cualquier tecla para continuar...");
    Console.ReadKey();
}