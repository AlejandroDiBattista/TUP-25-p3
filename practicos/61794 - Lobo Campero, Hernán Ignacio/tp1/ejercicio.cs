using System;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

const int MAX_CONTACTOS = 100;
Contacto[] agenda = new Contacto[MAX_CONTACTOS];
int contadorContactos = 0;
int siguienteId = 1;

CargarAgenda();

bool salir = false;
while (!salir)
{
    MostrarMenu();
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
            EliminarContacto();
            break;
        case "4":
            MostrarContactos();
            break;
        case "5":
            BuscarContactos();
            break;
        case "0":
            salir = true;
            break;
        default:
            Console.WriteLine("Opción no válida");
            break;
    }

    if (!salir)
    {
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}

GuardarAgenda();
Console.WriteLine("Saliendo de la aplicación...");

void MostrarMenu()
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
}

void AgregarContacto()
{
    Console.Clear();
    Console.WriteLine("=== Agregar Contacto ===");

    if (contadorContactos >= MAX_CONTACTOS)
    {
        Console.WriteLine("La agenda está llena. No se pueden agregar más contactos.");
        return;
    }

    Console.Write("Nombre: ");
    string nombre = Console.ReadLine();

    Console.Write("Teléfono: ");
    string telefono = Console.ReadLine();

    Console.Write("Email: ");
    string email = Console.ReadLine();

    agenda[contadorContactos].Id = siguienteId;
    agenda[contadorContactos].Nombre = nombre;
    agenda[contadorContactos].Telefono = telefono;
    agenda[contadorContactos].Email = email;

    contadorContactos++;
    siguienteId++;

    Console.WriteLine($"Contacto agregado con ID = {agenda[contadorContactos - 1].Id}");
}

void ModificarContacto()
{
    Console.Clear();
    Console.WriteLine("=== Modificar Contacto ===");
    Console.Write("Ingrese el ID del contacto a modificar: ");
    
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID no válido");
        return;
    }

    int indice = BuscarIndicePorId(id);
    if (indice == -1)
    {
        Console.WriteLine($"No se encontró contacto con ID {id}");
        return;
    }

    Console.WriteLine($"Datos actuales: Nombre: {agenda[indice].Nombre}, Teléfono: {agenda[indice].Telefono}, Email: {agenda[indice].Email}");
    Console.WriteLine("(Deje en blanco para no modificar)");

    Console.Write("Nuevo nombre: ");
    string nombre = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(nombre))
    {
        agenda[indice].Nombre = nombre;
    }

    Console.Write("Nuevo teléfono: ");
    string telefono = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(telefono))
    {
        agenda[indice].Telefono = telefono;
    }

    Console.Write("Nuevo email: ");
    string email = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(email))
    {
        agenda[indice].Email = email;
    }

    Console.WriteLine("Contacto modificado exitosamente");
}

void EliminarContacto()
{
    Console.Clear();
    Console.WriteLine("=== Borrar Contacto ===");
    Console.Write("Ingrese el ID del contacto a eliminar: ");
    
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID no válido");
        return;
    }

    int indice = BuscarIndicePorId(id);
    if (indice == -1)
    {
        Console.WriteLine($"No se encontró contacto con ID {id}");
        return;
    }

    for (int i = indice; i < contadorContactos - 1; i++)
    {
        agenda[i] = agenda[i + 1];
    }

    contadorContactos--;
    Console.WriteLine("Contacto eliminado exitosamente");
}

void MostrarContactos()
{
    Console.Clear();
    Console.WriteLine("=== Lista de Contactos ===");

    if (contadorContactos == 0)
    {
        Console.WriteLine("No hay contactos en la agenda");
        return;
    }

    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");
    Console.WriteLine(new string('-', 75));

    for (int i = 0; i < contadorContactos; i++)
    {
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", 
            agenda[i].Id, 
            agenda[i].Nombre, 
            agenda[i].Telefono, 
            agenda[i].Email);
    }
}

void BuscarContactos()
{
    Console.Clear();
    Console.WriteLine("=== Buscar Contacto ===");
    Console.Write("Ingrese término de búsqueda: ");
    string termino = Console.ReadLine().ToLower();

    bool encontrado = false;
    
    Console.WriteLine("\nResultados:");
    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");
    Console.WriteLine(new string('-', 75));

    for (int i = 0; i < contadorContactos; i++)
    {
        if (agenda[i].Nombre.ToLower().Contains(termino) || 
            agenda[i].Telefono.ToLower().Contains(termino) || 
            agenda[i].Email.ToLower().Contains(termino))
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", 
                agenda[i].Id, 
                agenda[i].Nombre, 
                agenda[i].Telefono, 
                agenda[i].Email);
            encontrado = true;
        }
    }

    if (!encontrado)
    {
        Console.WriteLine("No se encontraron coincidencias");
    }
}

int BuscarIndicePorId(int id)
{
    for (int i = 0; i < contadorContactos; i++)
    {
        if (agenda[i].Id == id)
        {
            return i;
        }
    }
    return -1;
}

void CargarAgenda()
{
    string rutaArchivo = "./agenda.csv"; // Ruta relativa al archivo en la carpeta actual

    if (!File.Exists(rutaArchivo))
    {
        return;
    }

    string[] lineas = File.ReadAllLines(rutaArchivo);
    for (int i = 0; i < lineas.Length && contadorContactos < MAX_CONTACTOS; i++)
    {
        string[] campos = lineas[i].Split(',');
        if (campos.Length == 4)
        {
            agenda[contadorContactos].Id = int.Parse(campos[0]);
            agenda[contadorContactos].Nombre = campos[1];
            agenda[contadorContactos].Telefono = campos[2];
            agenda[contadorContactos].Email = campos[3];

            contadorContactos++;

            if (agenda[contadorContactos - 1].Id >= siguienteId)
            {
                siguienteId = agenda[contadorContactos - 1].Id + 1;
            }
        }
    }
}

void GuardarAgenda()
{
    string rutaArchivo = "./agenda.csv"; // Ruta relativa al archivo en la carpeta actual

    using (StreamWriter sw = new StreamWriter(rutaArchivo))
    {
        for (int i = 0; i < contadorContactos; i++)
        {
            sw.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
        }
    }
}
