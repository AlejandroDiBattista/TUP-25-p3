// Ayuda: 
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo
//.

// Escribir la solucion al TP1 en este archivo. (Borre el ejemplo de abajo)
using System;       // Para usar la consola  (Console)
using System.IO;    // Para leer archivos    (File)


struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

const int MAX_CONTACTOS = 100;
static Contacto[] agenda = new Contacto[MAX_CONTACTOS];
static int totalContactos = 0;
static string archivo = "agenda.csv";

// Función para cargar los contactos desde el archivo CSV
if (File.Exists(archivo))
{
    string[] lineas = File.ReadAllLines(archivo);
    foreach (var linea in lineas)
    {
        string[] datos = linea.Split(',');
        if (datos.Length == 4)
        {
            Contacto contacto = new Contacto();
            contacto.Id = int.Parse(datos[0]);
            contacto.Nombre = datos[1];
            contacto.Telefono = datos[2];
            contacto.Email = datos[3];
            agenda[totalContactos++] = contacto;
        }
    }
}

// Función para guardar los contactos en el archivo CSV
void GuardarContactos()
{
    string[] lineas = new string[totalContactos];
    for (int i = 0; i < totalContactos; i++)
    {
        lineas[i] = $"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}";
    }
    File.WriteAllLines(archivo, lineas);
    Console.WriteLine("Contactos guardados correctamente.");
}

// Función para agregar un contacto
void AgregarContacto()
{
    if (totalContactos >= MAX_CONTACTOS)
    {
        Console.WriteLine("No se pueden agregar más contactos.");
        return;
    }

    Contacto nuevoContacto;
    nuevoContacto.Id = totalContactos + 1; // Asigna un ID único incrementando
    Console.Write("Nombre: ");
    nuevoContacto.Nombre = Console.ReadLine();
    Console.Write("Teléfono: ");
    nuevoContacto.Telefono = Console.ReadLine();
    Console.Write("Email: ");
    nuevoContacto.Email = Console.ReadLine();

    agenda[totalContactos] = nuevoContacto;
    totalContactos++;
    Console.WriteLine("Contacto agregado con éxito.");
}

// Función para modificar un contacto por su ID
void ModificarContacto()
{
    Console.Write("Ingrese el ID del contacto a modificar: ");
    int id = int.Parse(Console.ReadLine());
    for (int i = 0; i < totalContactos; i++)
    {
        if (agenda[i].Id == id)
        {
            Console.Write("Nuevo nombre (dejar vacío para no cambiar): ");
            string nuevoNombre = Console.ReadLine();
            Console.Write("Nuevo teléfono (dejar vacío para no cambiar): ");
            string nuevoTelefono = Console.ReadLine();
            Console.Write("Nuevo email (dejar vacío para no cambiar): ");
            string nuevoEmail = Console.ReadLine();

            if (!string.IsNullOrEmpty(nuevoNombre)) agenda[i].Nombre = nuevoNombre;
            if (!string.IsNullOrEmpty(nuevoTelefono)) agenda[i].Telefono = nuevoTelefono;
            if (!string.IsNullOrEmpty(nuevoEmail)) agenda[i].Email = nuevoEmail;

            Console.WriteLine("Contacto modificado con éxito.");
            return;
        }
    }
    Console.WriteLine("No se encontró el contacto con ese ID.");
}

// Función para borrar un contacto por su ID
void BorrarContacto()
{
    Console.Write("Ingrese el ID del contacto a eliminar: ");
    int id = int.Parse(Console.ReadLine());
    for (int i = 0; i < totalContactos; i++)
    {
        if (agenda[i].Id == id)
        {
            for (int j = i; j < totalContactos - 1; j++)
                agenda[j] = agenda[j + 1];

            totalContactos--;
            Console.WriteLine("Contacto eliminado con éxito.");
            return;
        }
    }
    Console.WriteLine("No se encontró el contacto con ese ID.");
}

// Función para listar todos los contactos
void ListarContactos()
{
    Console.WriteLine("\nID   | Nombre                | Teléfono          | Email");
    Console.WriteLine("---------------------------------------------------------");
    for (int i = 0; i < totalContactos; i++)
    {
        Console.WriteLine($"{agenda[i].Id,-5} | {agenda[i].Nombre,-20} | {agenda[i].Telefono,-15} | {agenda[i].Email}");
    }
}

// Función para buscar un contacto por nombre, teléfono o email
void BuscarContacto()
{
    Console.Write("Ingrese el término de búsqueda: ");
    string termino = Console.ReadLine().ToLower();
    Console.WriteLine("\nID   | Nombre                | Teléfono          | Email");
    Console.WriteLine("---------------------------------------------------------");
    for (int i = 0; i < totalContactos; i++)
    {
        if (agenda[i].Nombre.ToLower().Contains(termino) ||
            agenda[i].Telefono.Contains(termino) ||
            agenda[i].Email.ToLower().Contains(termino))
        {
            Console.WriteLine($"{agenda[i].Id,-5} | {agenda[i].Nombre,-20} | {agenda[i].Telefono,-15} | {agenda[i].Email}");
        }
    }
}

// Función para mostrar el menú y ejecutar acciones
void MostrarMenu()
{
    int opcion;
    do
    {
        Console.Clear();
        Console.WriteLine("===== GESTOR DE CONTACTOS =====");
        Console.WriteLine("1) Agregar contacto");
        Console.WriteLine("2) Modificar contacto");
        Console.WriteLine("3) Borrar contacto");
        Console.WriteLine("4) Listar contactos");
        Console.WriteLine("5) Buscar contacto");
        Console.WriteLine("0) Salir");
        Console.Write("Elija una opción: ");
        opcion = int.Parse(Console.ReadLine());

        switch (opcion)
        {
            case 1: AgregarContacto(); break;
            case 2: ModificarContacto(); break;
            case 3: BorrarContacto(); break;
            case 4: ListarContactos(); break;
            case 5: BuscarContacto(); break;
            case 0: GuardarContactos(); break;
            default: Console.WriteLine("Opción no válida."); break;
        }

        if (opcion != 0)
        {
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    } while (opcion != 0);
}

// Ejecuta el programa
MostrarMenu();
