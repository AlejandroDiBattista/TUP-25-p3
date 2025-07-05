using System;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}
// Variables globales
const int maximocont = 10;
Contacto[] agenda = new Contacto[maximocont];
int contadorID = 1;
int totalContactos = 0;

// Cargar datos al inicio
CargarDesdeArchivo();

// Menú principal
while (true)
{
    Console.WriteLine("\n1. Agregar\n2. Modificar\n3. Eliminar\n4. Listar\n5. Buscar\n6. Guardar y salir");
    string opcion = Console.ReadLine();

    if (opcion == "6")
    {
        GuardarEnArchivo();
        break;
    }
    else if (opcion == "1") AgregarContacto();
    else if (opcion == "2") ModificarContacto();
    else if (opcion == "3") EliminarContacto();
    else if (opcion == "4") ListarContactos();
    else if (opcion == "5") BuscarContacto();
    else Console.WriteLine("Opción no válida.");
}

// Métodos
void AgregarContacto()
{
    if (totalContactos >= maximocont)
    {
        Console.WriteLine("la agenda esta llena");
        return;
    }

    Console.Write("Ingrese el nombre: ");
    string nombre = Console.ReadLine();

    Console.Write("Ingrese el teléfono: ");
    string telefono = Console.ReadLine();

    Console.Write("Ingrese el email: ");
    string email = Console.ReadLine();

    agenda[totalContactos] = new Contacto
    {
        Id = contadorID,
        Nombre = nombre,
        Telefono = telefono,
        Email = email
    };
    contadorID++;
    totalContactos++;

    Console.WriteLine("Contacto agregado");

    for (int i = 0; i < totalContactos; i++)
    {
        Console.WriteLine($"{agenda[i].Id,-3} | {agenda[i].Nombre,-15} | {agenda[i].Telefono,-15} | {agenda[i].Email}");
    }
}

void ModificarContacto()
{
    Console.Write("Ingrese el ID del contacto: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }

    for (int i = 0; i < totalContactos; i++)
    {
        if (agenda[i].Id == id)
        {
            Console.Write($"Nuevo nombre ({agenda[i].Nombre}): ");
            string nombre = Console.ReadLine();
            if (!string.IsNullOrEmpty(nombre)) agenda[i].Nombre = nombre;

            Console.Write($"Nuevo teléfono ({agenda[i].Telefono}): ");
            string telefono = Console.ReadLine();
            if (!string.IsNullOrEmpty(telefono)) agenda[i].Telefono = telefono;

            Console.Write($"Nuevo email ({agenda[i].Email}): ");
            string email = Console.ReadLine();
            if (!string.IsNullOrEmpty(email)) agenda[i].Email = email;

            Console.WriteLine("Contacto modificado.");
            return;
        }
    }
    Console.WriteLine("ID no encontrado.");
}

void EliminarContacto()
{
    Console.Write("Ingrese el ID del contacto a eliminar: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }

    for (int i = 0; i < totalContactos; i++)
    {
        if (agenda[i].Id == id)
        {
            for (int j = i; j < totalContactos - 1; j++)
            {
                agenda[j] = agenda[j + 1];
            }
            totalContactos--;
            Console.WriteLine("Contacto eliminado.");
            return;
        }
    }
    Console.WriteLine("ID no encontrado.");
}

void ListarContactos()
{
    if (totalContactos == 0)
    {
        Console.WriteLine("No hay contactos para mostrar.");
        return;
    }

    Console.WriteLine("\n--- LISTA DE CONTACTOS ---");
    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}", "ID", "Nombre", "Teléfono", "Email");
    Console.WriteLine(new string('-', 75));

    for (int i = 0; i < totalContactos; i++)
    {
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-30}",
            agenda[i].Id,
            agenda[i].Nombre,
            agenda[i].Telefono,
            agenda[i].Email);
    }
}

void BuscarContacto()
{
    Console.Write("Ingrese término de búsqueda: ");
    string termino = Console.ReadLine().ToLower();

    bool encontrado = false;
    Console.WriteLine("\n--- RESULTADOS ---");
    for (int i = 0; i < totalContactos; i++)
    {
        if (agenda[i].Nombre.ToLower().Contains(termino) ||
            agenda[i].Telefono.ToLower().Contains(termino) ||
            agenda[i].Email.ToLower().Contains(termino))
        {
            Console.WriteLine($"{agenda[i].Id,-5} | {agenda[i].Nombre,-20} | {agenda[i].Telefono,-15} | {agenda[i].Email}");
            encontrado = true;
        }
    }
    if (!encontrado) Console.WriteLine("No se encontraron coincidencias.");
}

void GuardarEnArchivo()
{
    using (StreamWriter archivo = new StreamWriter("agenda.csv"))
    {
        for (int i = 0; i < totalContactos; i++)
        {
            archivo.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
        }
    }
    Console.WriteLine("Datos guardados en agenda.csv");
}

void CargarDesdeArchivo()
{
    if (!File.Exists("agenda.csv")) return;

    string[] lineas = File.ReadAllLines("agenda.csv");
    for (int i = 0; i < lineas.Length; i++)
    {
        string[] datos = lineas[i].Split(',');
        if (datos.Length == 4)
        {
            agenda[totalContactos] = new Contacto
            {
                Id = int.Parse(datos[0]),
                Nombre = datos[1],
                Telefono = datos[2],
                Email = datos[3]
            };
            totalContactos++;
            contadorID = agenda[totalContactos - 1].Id + 1;
        }
    }
}
