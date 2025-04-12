using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;  

// Ayuda: 
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo

// SOLUCIÓN----------------------------//


public struct Contacto
{
    public int id;
    public string nombre;
    public string telefono;
    public string email;
}

public class AgendaContactos
{
    static void Main(string[] args)
    {
        Contacto[] agenda = new Contacto[10];
        int ultimoId = 0;

        CargarContactosDesdeArchivo(agenda, ref ultimoId);

        int opcion = 0;
        while (opcion != 6)
        {
            Console.WriteLine("\n1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");

            Console.Write("Ingrese la opción: ");
            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Opción inválida. Intente nuevamente.");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    AgregarContacto(agenda, ref ultimoId);
                    break;
                case 2:
                    ModificarContacto(agenda, ultimoId);
                    break;
                case 3:
                    BorrarContacto(agenda, ref ultimoId);
                    break;
                case 4:
                    ListarContactos(agenda, ultimoId);
                    break;
                case 5:
                    BuscarContacto(agenda, ultimoId);
                    break;
                case 6:
                    GuardarContactosEnArchivo(agenda, ultimoId);
                    Console.WriteLine("¡Hasta luego!");
                    break;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    }

    static void AgregarContacto(Contacto[] agenda, ref int ultimoId)
    {
        if (ultimoId >= agenda.Length)
        {
            Console.WriteLine("Agenda llena, no se pueden agregar más contactos.");
            return;
        }

        Console.Write("Ingrese el nombre del contacto: ");
        string nombre = Console.ReadLine() ?? "";

string telefono;
do
{
    Console.Write("Ingrese el teléfono del contacto (solo números): ");
    telefono = Console.ReadLine() ?? "";  
} while (!Validaciones.EsTelefonoValido(telefono));

        string email;
do
{
    Console.Write("Ingrese el email del contacto (formato válido): ");
    email = Console.ReadLine() ?? "";  
} while (!Validaciones.EsEmailValido(email));

        agenda[ultimoId] = new Contacto
        {
            id = ultimoId + 1,
            nombre = nombre,
            telefono = telefono,
            email = email
        };

        ultimoId++;
        Console.WriteLine("Contacto agregado correctamente.");
    }

    static void ListarContactos(Contacto[] agenda, int ultimoId)
    {
        Console.WriteLine("\nLista de contactos:");
        Console.WriteLine("ID\tNombre\t\tTeléfono\tEmail");

        for (int i = 0; i < ultimoId; i++)
        {
            Console.WriteLine($"{agenda[i].id}\t{agenda[i].nombre}\t{agenda[i].telefono}\t{agenda[i].email}");
        }
    }

    static void ModificarContacto(Contacto[] agenda, int ultimoId)
    {
        Console.Write("Ingrese el ID del contacto que desea modificar: ");
        if (!int.TryParse(Console.ReadLine(), out int idModificar))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        for (int i = 0; i < ultimoId; i++)
        {
            if (agenda[i].id == idModificar)
            {
                Console.WriteLine("Contacto encontrado.");

                Console.Write("Nuevo nombre (Enter para mantener): ");
                string nuevoNombre = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoNombre)) agenda[i].nombre = nuevoNombre;

                Console.Write("Nuevo teléfono (Enter para mantener): ");
                string nuevoTelefono = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoTelefono)) agenda[i].telefono = nuevoTelefono;

                Console.Write("Nuevo email (Enter para mantener): ");
                string nuevoEmail = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoEmail)) agenda[i].email = nuevoEmail;

                Console.WriteLine("Contacto modificado correctamente.");
                return;
            }
        }
        Console.WriteLine("No se encontró ningún contacto con el ID ingresado.");
    }

    static void BorrarContacto(Contacto[] agenda, ref int ultimoId)
    {
        Console.Write("Ingrese el ID del contacto que desea borrar: ");
        if (!int.TryParse(Console.ReadLine(), out int idBorrar))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        for (int i = 0; i < ultimoId; i++)
        {
            if (agenda[i].id == idBorrar)
            {
                Console.WriteLine("Contacto encontrado. Eliminando...");

                for (int j = i; j < ultimoId - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }

                agenda[ultimoId - 1] = new Contacto(); 
                ultimoId--;

                Console.WriteLine("Contacto borrado correctamente.");
                return;
            }
        }
        Console.WriteLine("No se encontró ningún contacto con el ID ingresado.");
    }

    static void BuscarContacto(Contacto[] agenda, int ultimoId)
    {
        Console.Write("Ingrese el término de búsqueda: ");
        string terminoBusqueda = (Console.ReadLine() ?? "").ToLower();

        Console.WriteLine("\nResultados de la búsqueda:");
        Console.WriteLine("ID\tNombre\t\tTeléfono\tEmail");

        for (int i = 0; i < ultimoId; i++)
        {
            if (agenda[i].nombre.ToLower().Contains(terminoBusqueda) ||
                agenda[i].telefono.ToLower().Contains(terminoBusqueda) ||
                agenda[i].email.ToLower().Contains(terminoBusqueda))
            {
                Console.WriteLine($"{agenda[i].id}\t{agenda[i].nombre}\t{agenda[i].telefono}\t{agenda[i].email}");
            }
        }
    }

    static void CargarContactosDesdeArchivo(Contacto[] agenda, ref int ultimoId)
    {
        if (File.Exists("agenda.csv"))
        {
            string[] lineas = File.ReadAllLines("agenda.csv");
            foreach (string linea in lineas)
            {
                string[] valores = linea.Split(',');
                if (valores.Length == 4)
                {
                    agenda[ultimoId] = new Contacto
                    {
                        id = int.Parse(valores[0]),
                        nombre = valores[1],
                        telefono = valores[2],
                        email = valores[3]
                    };
                    ultimoId++;
                }
            }
        }
    }

    static void GuardarContactosEnArchivo(Contacto[] agenda, int ultimoId)
    {
        List<string> lineas = new List<string>();
        for (int i = 0; i < ultimoId; i++)
        {
            lineas.Add($"{agenda[i].id},{agenda[i].nombre},{agenda[i].telefono},{agenda[i].email}");
        }
        File.WriteAllLines("agenda.csv", lineas);
    }
}


public class Validaciones
{
    public static bool EsTelefonoValido(string telefono)
    {
        return !string.IsNullOrEmpty(telefono) && Regex.IsMatch(telefono, "^[0-9]+$") &&
               telefono.Length >= 7 && telefono.Length <= 15;
    }

    public static bool EsEmailValido(string email)
    {
        string patron = @"^[a-zA-Z0-9.!#$%&'+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)$";
        return !string.IsNullOrEmpty(email) && Regex.IsMatch(email, patron);
    }
}
