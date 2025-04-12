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
Console.WriteLine("Hola, soy el ejercicio 1 del TP1 de la materia Programación 3");
Console.Write("Presionar una tecla para continuar...");
Console.ReadKey();

using System;
using System.IO;
using static System.Console;

namespace TPN_1
{
    public struct Datos
    {
        public string nombre;
        public int telefono;
        public string email;
        public int id;
    }

    public class Constructores
    {
        private readonly Datos[] dat = new Datos[100];
        private int opc;
        private int cantCont = 0;
        private int contID = 1;
        private readonly string archivo = "agenda.csv";

        public Constructores()
        {
            CargarDesdeArchivo();
        }

        private void CargarDesdeArchivo()
        {
         
        }

        public void Menu()
        {
            do
            {
                Clear();
                WriteLine("===== AGENDA DE CONTACTOS =====");
                WriteLine("1) Agregar contacto");
                WriteLine("2) Modificar contacto");
                WriteLine("3) Borrar contacto");
                WriteLine("4) Listar contactos");
                WriteLine("5) Buscar contacto");
                WriteLine("0) Salir");
                Write("Seleccione una opción: ");

                if (int.TryParse(ReadLine(), out opc))
                {
                    switch (opc)
                    {
                        case 0:
                            WriteLine("Presione una tecla para cerrar.");
                            ReadKey();
                            Clear();
                            GuardarEnArchivo();
                            return;
                        case 1:
                            Agregar();
                            break;
                        case 2:
                            Modificar();
                            break;
                        case 3:
                            Borrar();
                            break;
                        case 4:
                            Listar();
                            break;
                        case 5:
                            Busqueda();
                            break;
                        default:
                            ForegroundColor = ConsoleColor.Red;
                            WriteLine("ERROR: Opción no válida.");
                            ResetColor();
                            break;
                    }
                }
                else
                {
                    WriteLine("Entrada no válida, ingrese un número.");
                }

            } while (opc != 0);
        }

        private void Borrar()
        {
    
        }

        private void Modificar()
        {
            WriteLine("=== Modificar Contacto ===");
            Write("Ingrese el ID del contacto a modificar: ");

            if (int.TryParse(ReadLine(), out int idModificar))
            {
                for (int i = 0; i < cantCont; i++)
                {
                    if (dat[i].id == idModificar)
                    {
                        Write("Nuevo Nombre: ");
                        dat[i].nombre = ReadLine();

                        Write("Nuevo Teléfono: ");
                        while (!int.TryParse(ReadLine(), out dat[i].telefono))
                        {
                            Write("Teléfono inválido. Intente nuevamente: ");
                        }

                        Write("Nuevo Email: ");
                        dat[i].email = ReadLine();

                        WriteLine("Contacto modificado correctamente.");
                        ReadKey();
                        return;
                    }
                }

                ForegroundColor = ConsoleColor.Red;
                WriteLine("No se encontró un contacto con ese ID.");
                ResetColor();
            }
            else
            {
                WriteLine("ID inválido, intente nuevamente.");
            }
            ReadKey();
        }

        private void GuardarEnArchivo()
        {
          
        }

        public void Agregar()
        {
            WriteLine("=== Agregar Contacto ===");
            if (cantCont >= dat.Length)
            {
                WriteLine("La agenda está llena, elimine algún contacto.");
                return;
            }

            Datos contNew;
            contNew.id = contID++;

            Write("Nombre   : ");
            contNew.nombre = ReadLine();

            Write("Teléfono : ");
            while (!int.TryParse(ReadLine(), out contNew.telefono))
            {
                Write("Teléfono inválido. Intente nuevamente: ");
            }

            Write("Email    : ");
            contNew.email = ReadLine();

            dat[cantCont] = contNew;
            cantCont++;

            WriteLine($"Contacto agregado con ID = {contNew.id}");
            WriteLine("Presione una tecla para continuar...");
            ReadKey();
            Clear();
        }

        public void Listar()
        {
            WriteLine("=== Lista de Contactos ===");
            WriteLine("ID\tNOMBRE\tTELEFONO\tEMAIL");
            for (int i = 0; i < cantCont; i++)
            {
                WriteLine($"{dat[i].id}\t{dat[i].nombre}\t{dat[i].telefono}\t{dat[i].email}");
            }
            WriteLine("\nPresione una tecla para continuar...");
            ReadKey();
            Clear();
        }

        public void Busqueda()
        {
            WriteLine("=== Buscar Contacto ===");
            Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
            string busc = ReadLine()?.ToLower() ?? "";
            bool encontrado = false;

            WriteLine("\nID\tNOMBRE\tTELEFONO\tEMAIL");

            for (int i = 0; i < cantCont; i++)
            {
                if (dat[i].nombre.ToLower().Contains(busc) ||
                    dat[i].email.ToLower().Contains(busc) ||
                    dat[i].telefono.ToString().Contains(busc))
                {
                    encontrado = true;
                    WriteLine($"{dat[i].id}\t{dat[i].nombre}\t{dat[i].telefono}\t{dat[i].email}");
                }
            }

            if (!encontrado)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("No se encontraron coincidencias.");
                ResetColor();
            }
            WriteLine("Presione una tecla para continuar...");
            ReadKey();
        }
    }

    class Program
    {
        static void Main()
        {
            Constructores agenda = new Constructores();
            agenda.Menu();
        }
    }
}