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
using static System.Console;
using System.IO;

class Program2
{
    
    static Contacto[] contactos = new Contacto[50];
    static int cuentaContactos = 0;
    static void Main()
    {
        CargarContactosDesdeArchivo();
        MenuPrincipal();
        GuardarContactosEnArchivo(); 
    }
//CLASE CONTACTO
    class Contacto
    {
        private static int cuentaId = 1;
        public int Id {get; set;}
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        public Contacto( string nombre, string telefono, string correo)
        {
            Id = cuentaId++;
            Nombre = nombre;
            Telefono = telefono;
            Correo = correo;
        }
        
        public override string ToString()
        {
            return $" Id: {Id} - Nombre: {Nombre} - Telefono: {Telefono} - Correo: {Correo}";
        }    
    }
//AGREGAR CONTACTO
    public static void AgregarContacto()
    {
        if (cuentaContactos >= 50)
        {
            WriteLine("La agenda está llena, no se pueden agregar más contactos");
            return;
        }
        else{
        Write("Ingrese el nombre: ");
        string nombre = ReadLine();
        Write("Ingrese el telefono: ");
        string telefono = ReadLine();
        Write("Ingrese el correo: ");
        string correo = ReadLine();
        contactos[cuentaContactos] = new Contacto( nombre, telefono, correo);
        cuentaContactos++;
        WriteLine("Contacto agregado con éxito");
        GuardarContactosEnArchivo();
        WriteLine("Presione cualquier tecla para continuar");
        ReadKey();
        MenuPrincipal();
        }
    }
//MODIFICAR CONTACTO
    public static void ModificarContacto()
    {
        if (cuentaContactos == 0)
        {
            WriteLine("No hay contactos para modificar");
            return;
        }
        WriteLine("Lista de contactos");
        for(int i = 0; i < cuentaContactos; i++)
        {
            if (contactos[i] != null)
            {
                WriteLine(contactos[i]);
            }   
        }
        Write("Ingrese el Id del contacto a modificar: ");
        int id = Convert.ToInt32(ReadLine());
        for(int i = 0; i < cuentaContactos; i++)
        {
            if (contactos[i] != null && contactos[i].Id == id)
            {
                Write("Ingrese el nombre: ");
                string nombre = ReadLine();
                Write("Ingrese el telefono: ");
                string telefono = ReadLine();
                Write("Ingrese el correo: ");
                string correo = ReadLine();
                if (!string.IsNullOrWhiteSpace(nombre)) contactos[i].Nombre = nombre;
                if (!string.IsNullOrWhiteSpace(telefono)) contactos[i].Telefono = telefono;
                if (!string.IsNullOrWhiteSpace(correo)) contactos[i].Correo = correo;
                WriteLine("Contacto modificado con éxito");
                GuardarContactosEnArchivo();
                WriteLine("Presione cualquier tecla para continuar");
                ReadKey();
                MenuPrincipal();
            }
        }
        WriteLine("Id de contacto no encontrado");
        WriteLine("Presione cualquier tecla para continuar");
        ReadKey();
    }
//BORRAR CONTACTO
    public static void BorrarContacto()
    {
        if (cuentaContactos == 0)
        {
            WriteLine("No hay contactos para borrar.");
            WriteLine("Presione cualquier tecla para continuar...");
            ReadKey();
            return;
        }

        WriteLine("Lista de contactos:");
        for (int i = 0; i < cuentaContactos; i++)
        {
            if (contactos[i] != null)
            {
                WriteLine(contactos[i]);
            }
        }

        Write("Ingrese el Id del contacto a borrar: ");
        if (!int.TryParse(ReadLine(), out int id))
        {
            WriteLine("Id inválido. Presione cualquier tecla para continuar...");
            ReadKey();
            return;
        }

        for (int i = 0; i < cuentaContactos; i++)
        {
            if (contactos[i] != null && contactos[i].Id == id)
            {
                
                for (int j = i; j < cuentaContactos - 1; j++)
                {
                    contactos[j] = contactos[j + 1];
                }

                
                contactos[cuentaContactos - 1] = null;
                cuentaContactos--;

                WriteLine("Contacto borrado con éxito.");
                GuardarContactosEnArchivo();
                WriteLine("Presione cualquier tecla para continuar...");
                ReadKey();
                return; 
            }
        }

        WriteLine("Id de contacto no encontrado.");
        WriteLine("Presione cualquier tecla para continuar...");
        ReadKey();
    }
//LISTAR CONTACTO
    public static void MostrarContacto()
    {
        if (cuentaContactos == 0)
        {
            WriteLine("No hay contactos para mostrar");
            return;
        }
        for(int i = 0; i < cuentaContactos; i++)
        {
            if (contactos[i] != null)
            {
                WriteLine(contactos[i]);
            }   
        }
        WriteLine("Presione cualquier tecla para continuar");
        ReadKey();
        MenuPrincipal();
    }
//BUSCAR CONTACTO
    public static void BuscarContacto()
    {
        Write("Ingrese el ID del contacto a buscar: ");
        string Id = ReadLine();
        for(int i = 0; i < cuentaContactos; i++)
        {
            if (contactos[i] != null && contactos[i].Id.ToString()== Id)
            {
                WriteLine(contactos[i]);
                WriteLine("Presione cualquier tecla para continuar");
                ReadKey();
                MenuPrincipal();
            }
        }
        WriteLine("Contacto no encontrado");
        WriteLine("Presione cualquier tecla para continuar");
        ReadKey();
        MenuPrincipal();
    } 

//GUARDAR CONTACTOS
    public static void GuardarContactosEnArchivo()
    {
        using (StreamWriter writer = new StreamWriter("contactos.csv"))
        {
            writer.WriteLine("Id,Nombre,Telefono,Correo");
            for (int i = 0; i < cuentaContactos; i++)
            {
                if (contactos[i] != null)
                {
                    writer.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Correo}");
                }
            }
        }
        WriteLine("Contactos guardados.");
    }
//CARGA CONTACTOS
    public static void CargarContactosDesdeArchivo()
    {
        if (File.Exists("contactos.csv"))
        {
            using (StreamReader reader = new StreamReader("contactos.csv"))
            {
                string linea;
                linea = reader.ReadLine();

                while ((linea = reader.ReadLine()) != null)
                {
                    if (cuentaContactos < contactos.Length) {
                        var partes = linea.Split(','); 
                        if (partes.Length == 4) 
                        {
                            contactos[cuentaContactos++] = new Contacto(partes[1].Trim(), partes[2].Trim(), partes[3].Trim())
                            {
                                Id = int.Parse(partes[0].Trim())
                            };
                        }
                        else
                        {
                            WriteLine($"Línea inválida en el archivo: {linea}");
                        }
                    }
                    else
                    {
                        WriteLine("El archivo contiene más contactos de los que se pueden cargar (máximo 50).");
                        break;
                    }
                }
            }
            WriteLine("Contactos cargados.");
        }
        else
        {
            WriteLine("El archivo 'contactos.csv' no existe.");
        }
    }
//MENU PRINCIPAL
    public static void MenuPrincipal()
    {
        Clear();
        WriteLine("----- Menu Principal -----|");
        WriteLine("1.   Agregar Contacto     |");
        WriteLine("2.   Modificar Contacto   |");
        WriteLine("3.   Borrar Contacto      |");
        WriteLine("4.   Listar Contactos     |");
        WriteLine("5.   Buscar Contacto      |");
        WriteLine("6.   Salir                |");
        WriteLine("--------------------------|");
        Write("Seleccione una opción: ");
        int opcion = Convert.ToInt32(Console.ReadLine());
        
        if (opcion == 1)
        {
            WriteLine("Agregar Contacto");
            AgregarContacto();
        }
        else if (opcion == 2)
        {
            WriteLine("Modificar Contacto");
            ModificarContacto();
        }
        else if (opcion == 3)
        {
            WriteLine("Borrar Contacto");
            BorrarContacto();
        }
        else if (opcion == 4)
        {
            WriteLine("Lista Contactos");
            MostrarContacto();
        }
        else if (opcion == 5)
        {
            WriteLine("Buscar Contacto");
            BuscarContacto();
        }
        else if (opcion == 6)
        {
            WriteLine("Saliendo del programa...");
            return;
        }
        else
        {
            WriteLine("Opción no válida");
            WriteLine("Presione cualquier tecla para continuar");
            ReadKey();
            MenuPrincipal();
        }
    }   
}