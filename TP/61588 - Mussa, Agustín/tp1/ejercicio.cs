//using System;       // Para usar la consola  (Console)
//using System.IO;    // Para leer archivos    (File)

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
using static System.Console;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
class Program
{
    static ArrayList Contactos = new ArrayList();
    static string rutaArchivo = "contactos.csv";
    static void Main()
    {   
        CargarContactosDesdeCSV();
        MenuDeOpciones();
    }

    static void AlmacenarContactos()
    {
        WriteLine("----Bienvenido a la agenda de contactos----\n");

        int limiteContactos = 3;
        for (int i = 0; i < limiteContactos; i++)
        {
            WriteLine("Ingrese el nombre del contacto: ");
            string nombre = ReadLine();
            WriteLine("Ingrese el teléfono del contacto: ");
            string telefono = ReadLine();
            WriteLine("Ingrese el correo del contacto: ");
            string correo = ReadLine();

            Contacto nuevoContacto = new Contacto(i + 1, nombre, telefono, correo);
            Contactos.Add(nuevoContacto);

            WriteLine("\n---- Contacto agregado con éxito! ----\n");
        }

        WriteLine("Contactos almacenados:");
        MostrarContactos();
    }

    static void MostrarContactos()
    {
        if (Contactos.Count == 0)
        {
            WriteLine("No hay contactos almacenados.");
            return;
        }

        for (int i = 0; i < Contactos.Count; i++)
        {
            Contacto contacto = (Contacto)Contactos[i];
            WriteLine($"ID: {contacto.Id}, Nombre: {contacto.Nombre}, Teléfono: {contacto.Telefono}, Correo: {contacto.Correo}");
        }
    }

    struct Contacto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        public Contacto(int id, string nombre, string telefono, string correo)
        {
            Id = id;
            Nombre = nombre;
            Telefono = telefono;
            Correo = correo;
        }
    }

    static void ModificarContactos()
    {
        if (Contactos.Count == 0)
        {
            WriteLine("No hay contactos para modificar.");
            return;
        }

        bool seguirModificando = true;
        while (seguirModificando)
        {
            WriteLine("Ingrese el ID del contacto que quiere modificar: ");
            if (!int.TryParse(ReadLine(), out int id))
            {
                WriteLine("Entrada inválida. Debe ingresar un número.");
                continue;
            }

            bool encontrado = false;
            for (int i = 0; i < Contactos.Count; i++)
            {
                Contacto contacto = (Contacto)Contactos[i];
                if (contacto.Id == id)
                {
                    encontrado = true;
                    WriteLine("Ingrese el nuevo nombre del contacto: ");
                    contacto.Nombre = ReadLine();
                    WriteLine("Ingrese el nuevo teléfono del contacto: ");
                    contacto.Telefono = ReadLine();
                    WriteLine("Ingrese el nuevo correo del contacto: ");
                    contacto.Correo = ReadLine();

                    Contactos[i] = contacto;
                    WriteLine("\n---- Contacto modificado con éxito! ----\n");
                    break;
                }
            }

            if (!encontrado)
            {
                WriteLine("Contacto no encontrado.");
            }

            WriteLine("Contactos actualizados:");
            MostrarContactos();

            WriteLine("¿Deseas seguir modificando otros contactos? (si/no): ");
            if (ReadLine().ToLower() != "si")
            {
                seguirModificando = false;
            }
        }
    }

    static void EliminarContactos()
    {
        if (Contactos.Count == 0)
        {
            WriteLine("No hay contactos para eliminar.");
            return;
        }

        WriteLine("Ingrese el ID del Contacto que quiere eliminar: ");
        if (!int.TryParse(ReadLine(), out int IdEliminar))
        {
            WriteLine("Entrada inválida. Debe ingresar un número.");
            return;
        }

        bool encontrado = false;
        for (int i = 0; i < Contactos.Count; i++)
        {
            Contacto contacto = (Contacto)Contactos[i];
            if (contacto.Id == IdEliminar)
            {
                Contactos.RemoveAt(i);
                encontrado = true;
                WriteLine("Contacto eliminado con éxito!");
                break;
            }
        }

        if (!encontrado)
        {
            WriteLine("ID no válido. No se pudo eliminar el contacto.");
        }

        WriteLine("Contactos actualizados:");
        MostrarContactos();
    }
    static void GuardarContactosEnCSV(){
        using (StreamWriter writer = new StreamWriter(rutaArchivo)){
            for(int i = 0; i < Contactos.Count; i++){
                Contacto contacto = (Contacto)Contactos[i];
                writer.WriteLine($"{contacto.Id},{contacto.Nombre},{contacto.Telefono},{contacto.Correo}");
            }
        }
        WriteLine("Contactos guardados en el archivo CSV.");
    }
    static void CargarContactosDesdeCSV(){
        if (!File.Exists(rutaArchivo)){
            return;
        }
        Contactos.Clear();
        using(StreamReader sr = new StreamReader(rutaArchivo)){
            string linea;
            while((linea = sr.ReadLine()) != null){
                string[] datos = linea.Split(',');
                if(datos.Length == 4){
                    int id = int.Parse(datos[0]);
                    string nombre = datos[1];
                    string telefono = datos[2];
                    string correo = datos[3];

                    Contacto contacto = new Contacto(id, nombre, telefono, correo);
                    Contactos.Add(contacto);
                }
            }
        }
        WriteLine("Contactos cargados desde el archivo CSV.");
    }

    static void MenuDeOpciones()
    {
        while (true)
        {
            WriteLine("---- Menú de Opciones ----");
            WriteLine("1. Almacenar Contactos");
            WriteLine("2. Modificar Contactos");
            WriteLine("3. Eliminar Contactos");
            WriteLine("4. Mostrar Contactos");
            WriteLine("5. Salir");
            Write("Seleccione una opción: ");

            if (!int.TryParse(ReadLine(), out int opcion))
            {
                WriteLine("Entrada no válida. Intente de nuevo.");
                continue;
            }

            if (opcion == 1)
            {
                AlmacenarContactos();
            }
            else if (opcion == 2)
            {
                ModificarContactos();
            }
            else if (opcion == 3)
            {
                EliminarContactos();
            }
            else if (opcion == 4)
            {
                MostrarContactos();
            }
            else if (opcion == 5)
            {
                WriteLine("Gracias por usar la agenda de contactos!");
                break;
            }
            else
            {
                WriteLine("Opción no válida. Intente nuevamente.");
            }
        }
    }
}