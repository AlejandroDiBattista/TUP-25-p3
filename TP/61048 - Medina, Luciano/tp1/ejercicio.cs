#!/usr/bin/env dotnet-script

using System;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Teléfono;
    public string Email;
}

const int MAX_CONTACTOS = 100;
static Contacto[] agenda = new Contacto[MAX_CONTACTOS];
static int contador = 0;
const string ARCHIVO = "agenda.csv";

// Load existing contacts
CargarContactos();

// Main program loop
int opcion;
do
{
    Console.Clear();
    MostrarMenu();

    if (!int.TryParse(Console.ReadLine(), out opcion))
    {
        Console.WriteLine("Entrada no válida. Intente de nuevo.");
        continue;
    }

    Console.Clear();

    switch (opcion)
    {
        case 1: AgregarContacto(); break;
        case 2: ModificarContacto(); break;
        case 3: BorrarContacto(); break;
        case 4: ListarContactos(); break;
        case 5: BuscarContacto(); break;
        case 0:
            GuardarContactos();
            Console.WriteLine("Saliendo...");
            break;
        default:
            Console.WriteLine("Opción no válida. Intente de nuevo.");
            break;
    }

    Console.WriteLine("\nPresione cualquier tecla para continuar...");
    Console.ReadKey();

} while (opcion != 0);

// Function definitions
static void MostrarMenu()
{
    Console.WriteLine("agenda de contactos");
    Console.WriteLine("1) Agregar contacto");
    Console.WriteLine("2) Modificar contacto");
    Console.WriteLine("3) eliminar contacto");
    Console.WriteLine("4) Listar contactos");
    Console.WriteLine("5) Buscar contacto");
    Console.WriteLine("0) Salir");
    Console.Write("Seleccionar una opción: ");
}

static void AgregarContacto()
{
    if (contador >= MAX_CONTACTOS)
    {
        Console.WriteLine("La agenda está llena.");
        return;
    }

    Console.Write("Nombre: ");
    string nombre = Console.ReadLine();
    Console.Write("Teléfono: ");
    string teléfono = Console.ReadLine();
    Console.Write("Email: ");
    string email = Console.ReadLine();

    contador++;
    agenda[contador - 1] = new Contacto { Id = contador, Nombre = nombre, Teléfono = teléfono, Email = email };

    Console.WriteLine($"Contacto agregado con ID = {contador}");
}

static void ModificarContacto()
{
    Console.Write("Ingrese el ID del contacto a modificar: ");
    int id = int.Parse(Console.ReadLine());
    for (int i = 0; i < contador; i++)
    {
        if (agenda[i].Id == id)
        {
            Console.WriteLine($"Modificando: {agenda[i].Nombre}, {agenda[i].Teléfono}, {agenda[i].Email}");
            Console.Write("Nuevo nombre (deje en blanco para no cambiar): ");
            string nombre = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nombre)) agenda[i].Nombre = nombre;
            Console.Write("Nuevo teléfono: ");
            string teléfono = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(teléfono)) agenda[i].Teléfono = teléfono;
            Console.Write("Nuevo email: ");
            string email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email)) agenda[i].Email = email;
            Console.WriteLine("Contacto modificado correctamente.");
            return;
        }
    }
    Console.WriteLine("Contacto no encontrado.");
}

static void BorrarContacto()
{
    Console.Write("Ingrese el ID del contacto a borrar: ");
    int id = int.Parse(Console.ReadLine());
    for (int i = 0; i < contador; i++)
    {
        if (agenda[i].Id == id)
        {
            for (int j = i; j < contador - 1; j++)
            {
                agenda[j] = agenda[j + 1];
            }
            contador--;
            Console.WriteLine("Contacto eliminado.");
            return;
        }
    }
    Console.WriteLine("Contacto no encontrado.");
}

static void ListarContactos()
{
    Console.WriteLine("ID\tNombre\t\tTeléfono\tEmail");
    for (int i = 0; i < contador; i++)
    {
        Console.WriteLine($"{agenda[i].Id}\t{agenda[i].Nombre}\t{agenda[i].Teléfono}\t{agenda[i].Email}");
    }
}

static void BuscarContacto()
{
    Console.Write("Ingrese término de búsqueda: ");
    string termino = Console.ReadLine().ToLower();
    Console.WriteLine("Resultados:");
    for (int i = 0; i < contador; i++)
    {
        if (agenda[i].Nombre.ToLower().Contains(termino) ||
            agenda[i].Teléfono.ToLower().Contains(termino) ||
            agenda[i].Email.ToLower().Contains(termino))
        {
            Console.WriteLine($"{agenda[i].Id}\t{agenda[i].Nombre}\t{agenda[i].Teléfono}\t{agenda[i].Email}");
        }
    }
}

static void CargarContactos()
{
    if (!File.Exists(ARCHIVO)) return;
    string[] lineas = File.ReadAllLines(ARCHIVO);
    int maxId = 0;

    foreach (string linea in lineas)
    {
        string[] partes = linea.Split(',');
        if (partes.Length == 4)
        {
            int id = int.Parse(partes[0]);
            agenda[contador] = new Contacto
            {
                Id = id,
                Nombre = partes[1],
                Teléfono = partes[2],
                Email = partes[3]
            };
            if (id > maxId) maxId = id;
            contador++;
        }
    }

    contador = maxId;
}

static void GuardarContactos()
{
    using (StreamWriter sw = new StreamWriter(ARCHIVO))
    {
        for (int i = 0; i < contador; i++)
        {
            sw.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Teléfono},{agenda[i].Email}");
        }
    }
}
