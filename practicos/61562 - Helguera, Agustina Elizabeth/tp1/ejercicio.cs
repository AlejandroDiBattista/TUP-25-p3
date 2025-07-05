using System;
using System.IO;
using static System.Console;

class Consola
{
    public static string Leer(string texto)
    {
        Write(texto);
        return ReadLine();
    }
}

const int cont_max = 80;
static Contacto[] agenda = new Contacto[cont_max];  
static int CantDeContacto = 0;  
static int proximoId = 1;
static string archivo = "agenda.csv";

cargarArchivo();
while (true)
{
    Clear();
    WriteLine("Agenda de contactos");
    WriteLine("1. Agregar contacto");
    WriteLine("2. Modificar contacto");
    WriteLine("3. Borrar contacto");
    WriteLine("4. Listar contactos");
    WriteLine("5. Buscar contacto");
    WriteLine("6. Salir");

    string opcion = Consola.Leer("Seleccione una opción: ");
    switch (opcion)
    {
        case "1":
            agregar_contacto();
            break;
        case "2":
            modificar_contacto();
            break;
        case "3":
            borrar_contacto();
            break;
        case "4":
            listar_contacto();
            break;
        case "5":
            buscar_contacto();
            break;
        case "6":
            WriteLine("Guardando contactos y saliendo...");
            guardar_archivo();
            return;
        default:
            WriteLine("Opción no válida. Presione una tecla para continuar...");
            ReadKey();
            break;
    }
}

static void cargarArchivo()
{
    if (File.Exists(archivo))
    {
        string[] lineas = File.ReadAllLines(archivo);
        foreach (string linea in lineas)
        {
            string[] datos = linea.Split(',');
            if (datos.Length == 4)
            {
                Contacto cont_nuevo = new Contacto
                {
                    Id = int.Parse(datos[0]),
                    Nombre = datos[1],
                    Telefono = datos[2],
                    Email = datos[3]
                };
                agenda[CantDeContacto++] = cont_nuevo;
                proximoId = cont_nuevo.Id + 1;
            }
        }
    }
}

static void agregar_contacto()
{
    if (CantDeContacto >= cont_max)
    {
        WriteLine("No puedes agregar más contactos.");
        ReadKey();
        return;
    }

    Contacto cont_nuevo = new Contacto
    {
        Id = proximoId++,
        Nombre = Consola.Leer("Nombre: "),
        Telefono = Consola.Leer("Teléfono: "),
        Email = Consola.Leer("Email: ")
    };

    agenda[CantDeContacto++] = cont_nuevo;
    WriteLine($"Contacto agregado con el ID {cont_nuevo.Id}");
    ReadKey();
}

static void modificar_contacto()
{
    int id = int.Parse(Consola.Leer("Ingrese el ID del contacto a modificar: "));
    for (int i = 0; i < CantDeContacto; i++)
    {
        if (agenda[i].Id == id)
        {
            agenda[i].Nombre = Consola.Leer("Nuevo Nombre: ");
            agenda[i].Telefono = Consola.Leer("Nuevo Teléfono: ");
            agenda[i].Email = Consola.Leer("Nuevo Email: ");
            WriteLine("Contacto modificado correctamente.");
            ReadKey();
            return;
        }
    }
    WriteLine("Contacto no encontrado.");
    ReadKey();
}

static void borrar_contacto()
{
    int id = int.Parse(Consola.Leer("Ingrese el ID del contacto a borrar: "));
    for (int i = 0; i < CantDeContacto; i++)
    {
        if (agenda[i].Id == id)
        {
            for (int j = i; j < CantDeContacto - 1; j++)
            {
                agenda[j] = agenda[j + 1];
            }
            CantDeContacto--;
            WriteLine("Contacto eliminado correctamente.");
            ReadKey();
            return;
        }
    }
    WriteLine("Contacto no encontrado.");
    ReadKey();
}

static void listar_contacto()
{
    WriteLine("ID   NOMBRE                      TELÉFONO              EMAIL");
    WriteLine("--------------------------------------------------------------");
    for (int i = 0; i < CantDeContacto; i++)
    {
        Contacto cont_nuevo = agenda[i];
        WriteLine($"{cont_nuevo.Id,-5} {cont_nuevo.Nombre,-25} {cont_nuevo.Telefono,-20} {cont_nuevo.Email,-30}");
    }
    WriteLine("\nPresione una tecla para continuar...");
    ReadKey();
}

static void buscar_contacto()
{
    string nombre = Consola.Leer("Ingrese el nombre del contacto a buscar: ").ToLower();
    bool encontrado = false;

    for (int i = 0; i < CantDeContacto; i++)
    {
        if (agenda[i].Nombre.ToLower().Contains(nombre))
        {
            WriteLine($"ID: {agenda[i].Id}, Nombre: {agenda[i].Nombre}, Teléfono: {agenda[i].Telefono}, Email: {agenda[i].Email}");
            encontrado = true;
        }
    }
    if (!encontrado)
    {
        WriteLine("Contacto no encontrado.");
    }
    ReadKey();
}

static void guardar_archivo()
{
    using (StreamWriter sw = new StreamWriter(archivo))
    {
        for (int i = 0; i < CantDeContacto; i++)
        {
            sw.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
        }
    }
}

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}
