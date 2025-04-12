#!/usr/bin/env dotnet-script

using System;
using System.IO;

struct Contacto {
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

const int MAX_CONTACTOS = 100;
static Contacto[] contactos = new Contacto[MAX_CONTACTOS];
static int contadorContactos = 0;

// Cargar contactos existentes
CargarContactosDesdeArchivo();

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

    string input = Console.ReadLine() ?? "0";
    if (int.TryParse(input, out opcion))
    {
        switch (opcion)
        {
            case 1: AgregarContacto(); break;
            case 2: ModificarContacto(); break;
            case 3: BorrarContacto(); break;
            case 4: ListarContactos(); break;
            case 5: BuscarContacto(); break;
            case 0: Console.WriteLine("Saliendo de la aplicación..."); break;
            default:
                Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
        }
    }
    else
    {
        Console.WriteLine("Por favor, ingrese un número válido. Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }
} 
while (opcion != 0);

GuardarContactosEnArchivo();

void AgregarContacto(){
    if (contadorContactos >= MAX_CONTACTOS)
    {
        Console.WriteLine("No se pueden agregar más contactos.");
        Console.ReadKey();
        return;
    }

    Console.Clear();
    Console.WriteLine("=== Agregar Contacto ===");
    Contacto nuevoContacto = new Contacto();
    nuevoContacto.Id = contadorContactos + 1;

    Console.Write("Nombre: ");
    nuevoContacto.Nombre = Console.ReadLine() ?? String.Empty;
    Console.Write("Teléfono: ");
    nuevoContacto.Telefono = Console.ReadLine() ?? String.Empty;
    Console.Write("Email: ");
    nuevoContacto.Email = Console.ReadLine() ?? String.Empty;

    contactos[contadorContactos] = nuevoContacto;
    contadorContactos++;
    Console.WriteLine($"Contacto agregado con ID = {nuevoContacto.Id}");
    Console.ReadKey();
}

void ModificarContacto(){
    Console.Clear();
    Console.WriteLine("=== Modificar Contacto ===");
    Console.Write("Ingrese el ID del contacto a modificar: ");
    int id;

    if (!int.TryParse(Console.ReadLine(), out id) || id < 1 || id > contadorContactos)
    {
        Console.WriteLine("Contacto no encontrado.");
        Console.ReadKey();
        return;
    }

    Contacto contacto = contactos[id - 1];
    Console.WriteLine($"Datos actuales => Nombre: {contacto.Nombre}, Teléfono: {contacto.Telefono}, Email: {contacto.Email}");
    Console.WriteLine("(Deje el campo en blanco para no modificar)\n");

    Console.Write("Nombre: ");
    string nuevoNombre = Console.ReadLine() ?? String.Empty;
    Console.Write("Teléfono: ");
    string nuevoTelefono = Console.ReadLine() ?? String.Empty;
    Console.Write("Email: ");
    string nuevoEmail = Console.ReadLine() ?? String.Empty;

    if (!string.IsNullOrWhiteSpace(nuevoNombre))
        contacto.Nombre = nuevoNombre;

    if (!string.IsNullOrWhiteSpace(nuevoTelefono))
        contacto.Telefono = nuevoTelefono;

    if (!string.IsNullOrWhiteSpace(nuevoEmail))
        contacto.Email = nuevoEmail;

    contactos[id - 1] = contacto;
    Console.WriteLine("Contacto modificado con éxito.");
    Console.ReadKey();
}
void BorrarContacto()
{
    Console.Clear();
    Console.WriteLine("=== Borrar Contacto ===");
    Console.Write("Ingrese el ID del contacto a borrar: ");
    int id;
    if (!int.TryParse(Console.ReadLine(), out id) || id < 1 || id > contadorContactos)
    {
        Console.WriteLine("Contacto no encontrado.");
        Console.ReadKey();
        return;
    }

    for (int i = id - 1; i < contadorContactos - 1; i++)
    {
        contactos[i] = contactos[i + 1];
    }

    contadorContactos--;
    Console.WriteLine($"Contacto con ID={id} eliminado con éxito.");
    Console.ReadKey();
}

void ListarContactos()
{
    Console.Clear();
    Console.WriteLine("=== Lista de Contactos ===");
    Console.WriteLine("ID NOMBRE TELÉFONO EMAIL");
    for (int i = 0; i < contadorContactos; i++)
    {
        Contacto c = contactos[i];
        Console.WriteLine($"{c.Id,-4} {c.Nombre,-20} {c.Telefono,-15} {c.Email}");
    }
    Console.ReadKey();
}

void BuscarContacto()
{
    Console.Clear();
    Console.WriteLine("=== Buscar Contacto ===");
    Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
    string termino = Console.ReadLine() ?? String.Empty;

    Console.WriteLine("Resultados de la búsqueda:");
    Console.WriteLine("ID NOMBRE TELÉFONO EMAIL");
    for (int i = 0; i < contadorContactos; i++)
    {
        Contacto c = contactos[i];
        if (c.Nombre.ToLower().Contains(termino) || c.Telefono.ToLower().Contains(termino) || c.Email.ToLower().Contains(termino))
        {
            Console.WriteLine($"{c.Id,-4} {c.Nombre,-20} {c.Telefono,-15} {c.Email}");
        }
    }
    Console.ReadKey();
}

void CargarContactosDesdeArchivo()
{
    string path = "agenda.csv";
    if (File.Exists(path))
    {
        string[] lineas = File.ReadAllLines(path);
        for (int i = 0; i < lineas.Length; i++)
        {
            string[] datos = lineas[i].Split(',');
            if (datos.Length == 4)
            {
                Contacto c = new Contacto
                {
                    Id = int.Parse(datos[0]),
                    Nombre = datos[1],
                    Telefono = datos[2],
                    Email = datos[3]
                };
                contactos[contadorContactos] = c;
                contadorContactos++;
            }
        }
    }
}

void GuardarContactosEnArchivo()
{
    string path = "agenda.csv";
    using (StreamWriter writer = new StreamWriter(path))
    {
        for (int i = 0; i < contadorContactos; i++)
        {
            Contacto c = contactos[i];
            writer.WriteLine($"{c.Id},{c.Nombre},{c.Telefono},{c.Email}");
        }
    }
    Console.WriteLine("Contactos guardados en el archivo.");
}
