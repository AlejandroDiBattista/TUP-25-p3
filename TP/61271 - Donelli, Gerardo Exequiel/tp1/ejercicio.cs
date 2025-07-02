using System;       // Para usar la consola  (Console)
using System.IO;    // Para leer archivos    (File)

// Ayuda: 
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo

// Escribir la solucion al TP1 en este archivo. (Borre el ejemplo de abajo)
//Console.WriteLine("Hola, soy el ejercicio 1 del TP1 de la materia Programación 3");
//Console.Write("Presionar una tecla para continuar...");
//Console.ReadKey();


struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

const int MAX_CONTACTOS = 100;
static Contacto[] contactos = new Contacto[MAX_CONTACTOS];
static int cantidad = 0;
static int ultimoId = 0;
static string archivoContactos = "agenda.csv";

LeerArchivo(); // Cargar contactos desde el archivo al inicio
Console.WriteLine("Agenda cargada. Total de contactos: {0}", cantidad);

int opcion;
do//********** MENU **********/
{
    Console.WriteLine("\n--- AGENDA DE CONTACTOS ---");
    Console.WriteLine("1. Agregar contacto");
    Console.WriteLine("2. Modificar contacto");
    Console.WriteLine("3. Borrar contacto");
    Console.WriteLine("4. Listar contactos");
    Console.WriteLine("5. Buscar contacto");
    Console.WriteLine("6. Salir");
    Console.Write("Seleccione una opción: ");
    opcion = Convert.ToInt32(Console.ReadLine());

    if (opcion == 1) AgregarContacto();
    else if (opcion == 2) ModificarContacto();
    else if (opcion == 3) BorrarContacto();
    else if (opcion == 4) ListarContactos();
    else if (opcion == 5) BuscarContacto();

} while (opcion != 6);

GuardarArchivo();// Guardar contactos en el archivo al salir
Console.Clear();
Console.WriteLine("Agenda guardada. ¡Hasta luego!");

void LeerArchivo()// Método para leer el archivo y cargar los contactos
// Se asume que el archivo tiene el formato: ID;Nombre;Teléfono;Email
{
    if (File.Exists(archivoContactos))
    {
        string[] lineas = File.ReadAllLines(archivoContactos);
        for (int i = 0; i < lineas.Length && i < MAX_CONTACTOS; i++)
        {
            string[] partes = lineas[i].Split(';');
            if (partes.Length == 4)
            {
                contactos[cantidad].Id = Convert.ToInt32(partes[0]);
                contactos[cantidad].Nombre = partes[1];
                contactos[cantidad].Telefono = partes[2];
                contactos[cantidad].Email = partes[3];
                if (contactos[cantidad].Id > ultimoId) ultimoId = contactos[cantidad].Id;
                cantidad++;
            }
        }
    }
}

void GuardarArchivo()// Método para guardar los contactos en el archivo
{
    using (StreamWriter sw = new StreamWriter(archivoContactos))
    {
        for (int i = 0; i < cantidad; i++)
        {
            sw.WriteLine($"{contactos[i].Id};{contactos[i].Nombre};{contactos[i].Telefono};{contactos[i].Email}");
        }
    }
}

void AgregarContacto()
{
    if (cantidad >= MAX_CONTACTOS)
    {
        Console.WriteLine("La agenda está llena.");
        return;
    }

    ultimoId++;
    contactos[cantidad].Id = ultimoId;

    Console.Write("Nombre: ");
    contactos[cantidad].Nombre = Console.ReadLine();
    Console.Write("Teléfono: ");
    contactos[cantidad].Telefono = Console.ReadLine();
    Console.Write("Email: ");
    contactos[cantidad].Email = Console.ReadLine();

    cantidad++;
    Console.WriteLine("Contacto agregado exitosamente.");
}

void ModificarContacto()
{
    Console.Write("Ingrese el ID del contacto a modificar: ");
    int id = Convert.ToInt32(Console.ReadLine());
    int pos = BuscarPorId(id);
    if (pos == -1)
    {
        Console.WriteLine("ID no encontrado.");
        return;
    }

    Console.WriteLine($"Modificando a {contactos[pos].Nombre}. Deje vacío si no desea cambiar.");
    Console.Write("Nuevo Nombre: ");
    string nuevoNombre = Console.ReadLine();
    if (nuevoNombre != "") contactos[pos].Nombre = nuevoNombre;

    Console.Write("Nuevo Teléfono: ");
    string nuevoTelefono = Console.ReadLine();
    if (nuevoTelefono != "") contactos[pos].Telefono = nuevoTelefono;

    Console.Write("Nuevo Email: ");
    string nuevoEmail = Console.ReadLine();
    if (nuevoEmail != "") contactos[pos].Email = nuevoEmail;

    Console.WriteLine("Contacto modificado.");
}

void BorrarContacto()
{
    Console.Write("Ingrese el ID del contacto a borrar: ");
    int id = Convert.ToInt32(Console.ReadLine());
    int pos = BuscarPorId(id);
    if (pos == -1)
    {
        Console.WriteLine("ID no encontrado.");
        return;
    }

    for (int i = pos; i < cantidad - 1; i++)
    {
        contactos[i] = contactos[i + 1];
    }
    cantidad--;
    Console.WriteLine("Contacto eliminado.");
}

void ListarContactos()
{
    if (cantidad == 0)
    {
        Console.WriteLine("No hay contactos en la agenda.");
        return;
    }

    Console.WriteLine("\n--- LISTA DE CONTACTOS ---");
    Console.WriteLine(new string('-', 70));
    Console.WriteLine("Total de contactos: {0}", cantidad);
    
    Console.WriteLine("\n{0,-5} | {1,-20} | {2,-15} | {3,-25}", "ID", "Nombre", "Teléfono", "Email");
    Console.WriteLine(new string('-', 70));

    for (int i = 0; i < cantidad; i++)
    {
        Console.WriteLine("{0,-5} | {1,-20} | {2,-15} | {3,-25}",
            contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
    }
}

void BuscarContacto() // Método para buscar contactos por nombre, teléfono o email
{
    Console.Write("Ingrese término de búsqueda: ");
    string termino = Console.ReadLine().ToLower();

    bool encontrado = false; // Variable para verificar si se encontró algún contacto

    Console.WriteLine("\n{0,-5} | {1,-20} | {2,-15} | {3,-25}", "ID", "Nombre", "Teléfono", "Email");
    Console.WriteLine(new string('-', 70));

    for (int i = 0; i < cantidad; i++)
    {
        if (contactos[i].Nombre.ToLower().Contains(termino) ||
            contactos[i].Telefono.ToLower().Contains(termino) ||
            contactos[i].Email.ToLower().Contains(termino))
        {
            Console.WriteLine("{0,-5} | {1,-20} | {2,-15} | {3,-25}",
                contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
            encontrado = true; // Si se encuentra un contacto, se marca como encontrado
        }
    }

    if (!encontrado)
    {
        Console.WriteLine("No se encontraron contactos que coincidan con el término de búsqueda.");
    }
}


int BuscarPorId(int id)
{
    for (int i = 0; i < cantidad; i++)
    {
        if (contactos[i].Id == id)
            return i;
    }
    return -1;
}
// Fin del programa