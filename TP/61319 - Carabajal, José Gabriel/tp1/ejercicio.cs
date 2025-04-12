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
struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class Program //clase
{
    const int MAX_CONTACTOS = 100;
    static Contacto[] agenda = new Contacto[MAX_CONTACTOS];
    static int contador = 0;
    static string archivo = "agenda.csv";

    static void Main()
    {
        CargarContactos();
        while (true)
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

            string opcion = Console.ReadLine();
            switch (opcion)
            {
                case "1": AgregarContacto(); break;
                case "2": ModificarContacto(); break;
                case "3": BorrarContacto(); break;
                case "4": ListarContactos(); break;
                case "5": BuscarContacto(); break;
                case "0": GuardarContactos(); Console.WriteLine("Saliendo de la aplicación..."); return;
                default: Console.WriteLine("Opción no válida."); break;
            }
        }
    }

    static void AgregarContacto()
    {
        if (contador >= MAX_CONTACTOS)
        {
            Console.WriteLine("La agenda está llena.");
            return;
        }

        int nuevoId = contador == 0 ? 1 : agenda[contador - 1].Id + 1;

        Contacto nuevo;
        nuevo.Id = nuevoId;
        Console.WriteLine("\n=== Agregar Contacto ===");
        Console.Write("Nombre   : "); nuevo.Nombre = Console.ReadLine();
        Console.Write("Teléfono : "); nuevo.Telefono = Console.ReadLine();
        Console.Write("Email    : "); nuevo.Email = Console.ReadLine();

        agenda[contador] = nuevo;
        contador++;
        Console.WriteLine($"Contacto agregado con ID = {nuevoId}");
        EsperarTecla();
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());

        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Id == id)
            {
                Contacto modificado = agenda[i];
                Console.WriteLine($"Datos actuales => Nombre: {modificado.Nombre}, Teléfono : {modificado.Telefono}, Email: {modificado.Email}");
                Console.WriteLine("(Deje el campo en blanco para no modificar)");

                Console.Write("Nombre   : ");
                string nombre = Console.ReadLine();
                if (!string.IsNullOrEmpty(nombre)) modificado.Nombre = nombre;

                Console.Write("Teléfono : ");
                string telefono = Console.ReadLine();
                if (!string.IsNullOrEmpty(telefono)) modificado.Telefono = telefono;

                Console.Write("Email    : ");
                string email = Console.ReadLine();
                if (!string.IsNullOrEmpty(email)) modificado.Email = email;

                agenda[i] = modificado;
                Console.WriteLine("\nContacto modificado con éxito.");
                EsperarTecla();
                return;
            }
        }
        Console.WriteLine("ID no encontrado.");
        EsperarTecla();
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
                Console.WriteLine($"Contacto con ID={id} eliminado con éxito.");
                EsperarTecla();
                return;
            }
        }
        Console.WriteLine("ID no encontrado.");
        EsperarTecla();
    }

    static void ListarContactos()
    {
        Console.WriteLine("\n=== Lista de Contactos ===");
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        Console.WriteLine("------------------------------------------------");
        for (int i = 0; i < contador; i++)
        {
            Console.WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
        }
        EsperarTecla();
    }

    static void BuscarContacto()
    {
        Console.Write("\nIngrese un término de búsqueda (nombre, teléfono o email): ");
        string termino = Console.ReadLine().ToLower();
        bool encontrado = false;

        Console.WriteLine("\nResultados de la búsqueda:");
        Console.WriteLine("ID    NOMBRE               TELÉFONO       EMAIL");
        Console.WriteLine("------------------------------------------------");
        
        for (int i = 0; i < contador; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) ||
                agenda[i].Telefono.ToLower().Contains(termino) ||
                agenda[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{agenda[i].Id,-5} {agenda[i].Nombre,-20} {agenda[i].Telefono,-15} {agenda[i].Email}");
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("No se encontraron contactos con ese criterio.");
        }
        EsperarTecla();
    }

    static void EsperarTecla()
    {
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static void CargarContactos()
    {
        if (!File.Exists(archivo)) return;
        string[] lineas = File.ReadAllLines(archivo);
        foreach (string linea in lineas)
        {
            string[] datos = linea.Split(',');
            if (datos.Length == 4)
            {
                agenda[contador] = new Contacto { Id = int.Parse(datos[0]), Nombre = datos[1], Telefono = datos[2], Email = datos[3] };
                contador++;
            }
        }
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter(archivo))
        {
            for (int i = 0; i < contador; i++)
            {
                sw.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
            }
        }
    }
}
