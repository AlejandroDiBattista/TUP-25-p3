using System;
using System.IO;


struct Contacto
{
public int Id;
public string Nombre;
public string Telefono;
public string Email;
}

const int maxContactos = 100;
Contacto[] contactos = new Contacto[maxContactos];
int contactoCount = 0;
string archivo = "agenda.csv";
CargarContactos(archivo, ref contactos, ref contactoCount);

int opcion;
do
{
    MostrarMenu();
    opcion = int.Parse(Console.ReadLine());

    switch (opcion)
    {
        case 1:
            AgregarContacto(ref contactos, ref contactoCount);
            break;
        case 2:
            ModificarContacto(ref contactos, contactoCount);
            break;
        case 3:
            BorrarContacto(ref contactos, ref contactoCount);
            break;
        case 4:
            ListarContactos(contactos, contactoCount);
            break;
        case 5:
            BuscarContacto(contactos, contactoCount);
            break;
        case 0:
            GuardarContactos(archivo, contactos, contactoCount);
            break;
        default:
            Console.WriteLine("Opción no válida, por favor intente de nuevo.");
            break;
    }

} while (opcion != 0);
  
static void MostrarMenu()
{
    Console.Clear();
    Console.WriteLine("AGENDAR CONTACTOS");
    Console.WriteLine("1) Agregar contacto");
    Console.WriteLine("2) Modificar contacto");
    Console.WriteLine("3) Borrar contacto");
    Console.WriteLine("4) Listar contactos");
    Console.WriteLine("5) Buscar contacto");
    Console.WriteLine("0) Salir");
    Console.Write("Seleccione una opción: ");
}
static void AgregarContacto(ref Contacto[] contactos, ref int contactoCount)
{
    if (contactoCount >= contactos.Length)
    {
        Console.WriteLine("La agenda está llena.");
        Console.ReadKey();
        return;
    }

    Console.Clear();
    Console.WriteLine("=== Agregar Contacto ===");

    Console.Write("Nombre: ");
    string nombre = Console.ReadLine();
    Console.Write("Teléfono: ");
    string telefono = Console.ReadLine();
    Console.Write("Email: ");
    string email = Console.ReadLine();

    contactos[contactoCount].Id = contactoCount + 1;
    contactos[contactoCount].Nombre = nombre;
    contactos[contactoCount].Telefono = telefono;
    contactos[contactoCount].Email = email;

    Console.WriteLine($"Contacto agregado con ID = {contactos[contactoCount].Id}");
    contactoCount++;
    Console.ReadKey();
}
static void ModificarContacto(ref Contacto[] contactos, int contactoCount)
{
    Console.Clear();
    Console.WriteLine("=== Modificar Contacto ===");
    Console.Write("Ingrese el ID del contacto a modificar: ");
    int id = int.Parse(Console.ReadLine());

    if (id <= 0 || id > contactoCount)
    {
        Console.WriteLine("Contacto no encontrado.");
        Console.ReadKey();
        return;
    }

    int index = id - 1;
    Console.WriteLine($"Datos actuales => Nombre: {contactos[index].Nombre}, Teléfono: {contactos[index].Telefono}, Email: {contactos[index].Email}");

    Console.Write("Nuevo nombre (deje vacío para no cambiar): ");
    string nombre = Console.ReadLine();
    Console.Write("Nuevo teléfono (deje vacío para no cambiar): ");
    string telefono = Console.ReadLine();
    Console.Write("Nuevo email (deje vacío para no cambiar): ");
    string email = Console.ReadLine();

    if (!string.IsNullOrEmpty(nombre)) contactos[index].Nombre = nombre;
    if (!string.IsNullOrEmpty(telefono)) contactos[index].Telefono = telefono;
    if (!string.IsNullOrEmpty(email)) contactos[index].Email = email;

    Console.WriteLine("Contacto modificado con éxito.");
    Console.ReadKey();
}
static void BorrarContacto(ref Contacto[] contactos, ref int contactoCount)
{
    Console.Clear();
    Console.WriteLine("=== Borrar Contacto ===");
    Console.Write("Ingrese el ID del contacto a borrar: ");
    int id = int.Parse(Console.ReadLine());

    if (id <= 0 || id > contactoCount)
    {
        Console.WriteLine("Contacto no encontrado.");
        Console.ReadKey();
        return;
    }

    int index = id - 1;
    for (int i = index; i < contactoCount - 1; i++)
    {
        contactos[i] = contactos[i + 1];
    }

    contactoCount--;
    Console.WriteLine($"Contacto con ID={id} eliminado con éxito.");
    Console.ReadKey();
}
static void ListarContactos(Contacto[] contactos, int contactoCount)
{
    Console.Clear();
    Console.WriteLine("Lista de Contactos");
    Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");

    for (int i = 0; i < contactoCount; i++)
    {
        Console.WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email}");
    }

    Console.ReadKey();
}
static void BuscarContacto(Contacto[] contactos, int contactoCount)
{
    Console.Clear();
    Console.WriteLine("Buscar Contacto");
    Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
    string termino = Console.ReadLine().ToLower();

    Console.WriteLine("Resultado de la búsqueda:");
    Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");

    bool encontrado = false;
    for (int i = 0; i < contactoCount; i++)
    {
        if (contactos[i].Nombre.ToLower().Contains(termino) ||
            contactos[i].Telefono.Contains(termino) ||
            contactos[i].Email.ToLower().Contains(termino))
        {
            Console.WriteLine($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email}");
            encontrado = true;
        }
    }

    if (!encontrado)
    {
        Console.WriteLine("No se encontraron resultados.");
    }

    Console.ReadKey();
}
static void CargarContactos(string archivo, ref Contacto[] contactos, ref int contactoCount)
{
    if (File.Exists(archivo))
    {
        string[] lineas = File.ReadAllLines(archivo);

        foreach (string linea in lineas)
        {
            string[] datos = linea.Split(',');

            if (datos.Length == 4)
            {
                contactos[contactoCount].Id = contactoCount + 1;
                contactos[contactoCount].Nombre = datos[0];
                contactos[contactoCount].Telefono = datos[1];
                contactos[contactoCount].Email = datos[2];
                contactoCount++;
            }
        }
    }
}
static void GuardarContactos(string archivo, Contacto[] contactos, int contactoCount)
{
    Console.Clear();
    Console.WriteLine("Guardando los contactos...");
    using (StreamWriter sw = new StreamWriter(archivo))
    {
        for (int i = 0; i < contactoCount; i++)
        {
            sw.WriteLine($"{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
        }
    }
    Console.WriteLine("Cambios guardados correctamente.");
    Console.ReadKey();
}
