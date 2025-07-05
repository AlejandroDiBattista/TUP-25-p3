using System;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

const int MAX_CONTACTOS = 1000;
Contacto[] contactos = new Contacto[MAX_CONTACTOS];
int totalContactos = 0;
int ultimoId = 0;
const string ARCHIVO = "agenda.csv";

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
    if (!int.TryParse(Console.ReadLine(), out opcion))
        opcion = -1;

    switch (opcion)
    {
        case 1: AgregarContacto(); 
            break;
        case 2: ModificarContacto(); 
            break;
        case 3: BorrarContacto();
            break;
        case 4: ListarContactos(); 
            break;
        case 5: BuscarContacto(); 
            break;
        case 0: GuardarContactos(); Console.WriteLine("\nSaliendo de la aplicación..."); 
            break;
        default: Console.WriteLine("Opción inválida."); 
            break;
    }

    if (opcion != 0)
    {
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }

} while (opcion != 0);

static void CargarContactos()
{
    if (!File.Exists(ARCHIVO)) return;

    string[] lineas = File.ReadAllLines(ARCHIVO);
    for (int i = 0; i < lineas.Length && totalContactos < MAX_CONTACTOS; i++)
    {
        string[] partes = lineas[i].Split(',');
        if (partes.Length == 4)
        {
            int id;
            if (int.TryParse(partes[0], out id))
            {
                contactos[totalContactos].Id = id;
                contactos[totalContactos].Nombre = partes[1];
                contactos[totalContactos].Telefono = partes[2];
                contactos[totalContactos].Email = partes[3];
                if (id > ultimoId) ultimoId = id;
                totalContactos++;
            }
        }
    }
}

static void GuardarContactos()
{
    string[] lineas = new string[totalContactos];
    for (int i = 0; i < totalContactos; i++)
    {
        lineas[i] = $"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}";
    }
    File.WriteAllLines(ARCHIVO, lineas);
}

static void AgregarContacto()
{
    if (totalContactos >= MAX_CONTACTOS)
    {
        Console.WriteLine("No se pueden agregar más contactos.");
        return;
    }

    Console.WriteLine("\n=== Agregar Contacto ===");
    Console.Write("Nombre   : ");
    string nombre = Console.ReadLine();
    Console.Write("Teléfono : ");
    string telefono = Console.ReadLine();
    Console.Write("Email    : ");
    string email = Console.ReadLine();

    ultimoId++;
    contactos[totalContactos].Id = ultimoId;
    contactos[totalContactos].Nombre = nombre;
    contactos[totalContactos].Telefono = telefono;
    contactos[totalContactos].Email = email;
    totalContactos++;

    Console.WriteLine($"Contacto agregado con ID = {ultimoId}");
}

static void ModificarContacto()
{
    Console.WriteLine("\n=== Modificar Contacto ===");
    Console.Write("Ingrese el ID del contacto a modificar: ");
    int id;
    if (!int.TryParse(Console.ReadLine(), out id)) return;

    int index = BuscarIndicePorId(id);
    if (index == -1)
    {
        Console.WriteLine("ID no encontrado.");
        return;
    }

    Console.WriteLine($"Datos actuales => Nombre: {contactos[index].Nombre}, Teléfono : {contactos[index].Telefono}, Email: {contactos[index].Email}");
    Console.WriteLine("(Deje el campo en blanco para no modificar)");

    Console.Write("Nombre    : ");
    string nuevoNombre = Console.ReadLine();
    if (nuevoNombre != "") contactos[index].Nombre = nuevoNombre;

    Console.Write("Teléfono  : ");
    string nuevoTelefono = Console.ReadLine();
    if (nuevoTelefono != "") contactos[index].Telefono = nuevoTelefono;

    Console.Write("Email     : ");
    string nuevoEmail = Console.ReadLine();
    if (nuevoEmail != "") contactos[index].Email = nuevoEmail;

    Console.WriteLine("Contacto modificado con éxito.");
}

static void BorrarContacto()
{
    Console.WriteLine("\n=== Borrar Contacto ===");
    Console.Write("Ingrese el ID del contacto a borrar: ");
    int id;
    if (!int.TryParse(Console.ReadLine(), out id)) return;

    int index = BuscarIndicePorId(id);
    if (index == -1)
    {
        Console.WriteLine("ID no encontrado.");
        return;
    }

    for (int i = index; i < totalContactos - 1; i++)
    {
        contactos[i] = contactos[i + 1];
    }
    totalContactos--;

    Console.WriteLine($"Contacto con ID={id} eliminado con éxito.");
}

static void ListarContactos()
{
    Console.WriteLine("\n=== Lista de Contactos ===");
    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");
    for (int i = 0; i < totalContactos; i++)
    {
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
    }
}

static void BuscarContacto()
{
    Console.WriteLine("\n=== Buscar Contacto ===");
    Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
    string termino = Console.ReadLine().ToLower();

    Console.WriteLine("\nResultados de la búsqueda:");
    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");

    bool encontrado = false;
    for (int i = 0; i < totalContactos; i++)
    {
        string nombre = contactos[i].Nombre.ToLower();
        string telefono = contactos[i].Telefono.ToLower();
        string email = contactos[i].Email.ToLower();

        if (nombre.Contains(termino) || telefono.Contains(termino) || email.Contains(termino))
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
            encontrado = true;
        }
    }

    if (!encontrado)
    {
        Console.WriteLine("No se encontraron coincidencias.");
    }
}

static int BuscarIndicePorId(int id)
{
    for (int i = 0; i < totalContactos; i++)
    {
        if (contactos[i].Id == id) return i;
    }
    return -1;
}
