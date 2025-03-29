

// Ayuda: 
// dotnet script "C:\guillermo\mis proyectos\TUP-25-p3\TP\61988 - Villada, Guillermo Nicolás\tp1\ejercicio.cs" comando para ejecutar este programa
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo

// Escribir la solucion al TP1 en este archivo. (Borre el ejemplo de abajo)
using System;

struct Contacto
{
    public int ID;
    public string Nombre;
    public string Telefono;
    public string Email;
}

Contacto[] agenda = new Contacto[100]; // Array para almacenar hasta 100 contactos
int cantidadContactos = 0; // Número actual de contactos
int ultimoID = 1000; // Último ID asignado

// Cargar contactos desde el archivo CSV al iniciar el programa
CargarContactosDesdeArchivo();

while (true)
{
    Console.Clear();
    Console.WriteLine("Agenda de Contactos");
    Console.WriteLine("1. Agregar contacto");
    Console.WriteLine("2. Modificar contacto");
    Console.WriteLine("3. Borrar contacto");
    Console.WriteLine("4. Listar contactos");
    Console.WriteLine("5. Buscar contacto");
    Console.WriteLine("6. Salir");
    Console.Write("Seleccione una opción: ");
    string opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1": AgregarContacto(); break;
        case "2": ModificarContacto(); break;
        case "3": BorrarContacto(); break;
        case "4": ListarContactos(); break;
        case "5": BuscarContacto(); break;
        case "6": GuardarAgendaCompletaEnArchivo(); return;
        default: Console.WriteLine("Opción inválida. Presione una tecla para continuar..."); Console.ReadKey(); break;
    }
}

void CargarContactosDesdeArchivo()
{
    string rutaArchivo = @"C:\guillermo\mis proyectos\TUP-25-p3\TP\61988 - Villada, Guillermo Nicolás\tp1\agenda.csv"; // Ruta absoluta
    if (System.IO.File.Exists(rutaArchivo))
    {
        string[] lineas = System.IO.File.ReadAllLines(rutaArchivo);

        // Ignorar la primera línea (encabezado)
        for (int i = 1; i < lineas.Length; i++)
        {
            string linea = lineas[i];
            string[] datos = linea.Split(',');
            if (datos.Length == 3)
            {
                ultimoID++;
                agenda[cantidadContactos] = new Contacto
                {
                    ID = ultimoID,
                    Nombre = datos[0],
                    Telefono = datos[1],
                    Email = datos[2]
                };
                cantidadContactos++;
            }
        }
    }
    else
    {
        // Si el archivo no existe, crea uno vacío con el encabezado
        System.IO.File.WriteAllText(rutaArchivo, "nombre,telefono,email\n");
    }
}

void GuardarAgendaCompletaEnArchivo()
{
    string rutaArchivo = @"C:\guillermo\mis proyectos\TUP-25-p3\TP\61988 - Villada, Guillermo Nicolás\tp1\agenda.csv"; // Ruta absoluta
    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(rutaArchivo))
    {
        writer.WriteLine("nombre,telefono,email"); // Escribir encabezado
        for (int i = 0; i < cantidadContactos; i++)
        {
            writer.WriteLine($"{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
        }
    }
}

void AgregarContacto()
{
    if (cantidadContactos >= agenda.Length)
    {
        Console.WriteLine("La agenda está llena. No se pueden agregar más contactos.");
        Console.ReadKey();
        return;
    }

    Console.Clear();
    Console.WriteLine("Agregar Contacto");
    Console.Write("Nombre: ");
    string nombre = Console.ReadLine();
    Console.Write("Teléfono: ");
    string telefono = Console.ReadLine();
    Console.Write("Email: ");
    string email = Console.ReadLine();

    ultimoID++;
    Contacto nuevoContacto = new Contacto { ID = ultimoID, Nombre = nombre, Telefono = telefono, Email = email };
    agenda[cantidadContactos] = nuevoContacto;
    cantidadContactos++;

    // Guardar la agenda completa en el archivo
    GuardarAgendaCompletaEnArchivo();

    Console.WriteLine("Contacto agregado exitosamente. Presione una tecla para continuar...");
    Console.ReadKey();
}

void ModificarContacto()
{
    Console.Clear();
    Console.WriteLine("Modificar Contacto");
    Console.Write("Ingrese el ID del contacto: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].ID == id)
            {
                Console.Write($"Nombre ({agenda[i].Nombre}): ");
                string nuevoNombre = Console.ReadLine();
                Console.Write($"Teléfono ({agenda[i].Telefono}): ");
                string nuevoTelefono = Console.ReadLine();
                Console.Write($"Email ({agenda[i].Email}): ");
                string nuevoEmail = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nuevoNombre)) agenda[i].Nombre = nuevoNombre;
                if (!string.IsNullOrWhiteSpace(nuevoTelefono)) agenda[i].Telefono = nuevoTelefono;
                if (!string.IsNullOrWhiteSpace(nuevoEmail)) agenda[i].Email = nuevoEmail;

                // Guardar la agenda completa en el archivo
                GuardarAgendaCompletaEnArchivo();

                Console.WriteLine("Contacto modificado y guardado en el archivo. Presione una tecla para continuar...");
                Console.ReadKey();
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }
    else
    {
        Console.WriteLine("ID inválido.");
    }
    Console.ReadKey();
}

void BorrarContacto()
{
    Console.Clear();
    Console.WriteLine("Borrar Contacto");
    Console.Write("Ingrese el ID del contacto: ");
    if (int.TryParse(Console.ReadLine(), out int id))
    {
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (agenda[i].ID == id)
            {
                // Desplazar los contactos hacia atrás para llenar el hueco
                for (int j = i; j < cantidadContactos - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                cantidadContactos--;

                // Guardar la agenda completa en el archivo
                GuardarAgendaCompletaEnArchivo();

                Console.WriteLine("Contacto eliminado y actualizado exitosamente. Presione una tecla para continuar...");
                Console.ReadKey();
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }
    else
    {
        Console.WriteLine("ID inválido.");
    }
    Console.ReadKey();
}

void ListarContactos()
{
    Console.Clear();
    Console.WriteLine("Lista de Contactos");
    Console.WriteLine("ID\tNombre\t\tTeléfono\t\tEmail");
    for (int i = 0; i < cantidadContactos; i++)
    {
        Console.WriteLine($"{agenda[i].ID}\t{agenda[i].Nombre}\t\t{agenda[i].Telefono}\t\t{agenda[i].Email}");
    }
    Console.WriteLine("Presione una tecla para continuar...");
    Console.ReadKey();
}

void BuscarContacto()
{
    Console.Clear();
    Console.WriteLine("Buscar Contacto");
    Console.Write("Ingrese término de búsqueda: ");
    string termino = Console.ReadLine().ToLower();

    Console.WriteLine("Resultados:");
    Console.WriteLine("ID\tNombre\t\tTeléfono\t\tEmail");
    for (int i = 0; i < cantidadContactos; i++)
    {
        if (agenda[i].Nombre.ToLower().Contains(termino) ||
            agenda[i].Telefono.ToLower().Contains(termino) ||
            agenda[i].Email.ToLower().Contains(termino))
        {
            Console.WriteLine($"{agenda[i].ID}\t{agenda[i].Nombre}\t\t{agenda[i].Telefono}\t\t{agenda[i].Email}");
        }
    }
    Console.WriteLine("Presione una tecla para continuar...");
    Console.ReadKey();
}