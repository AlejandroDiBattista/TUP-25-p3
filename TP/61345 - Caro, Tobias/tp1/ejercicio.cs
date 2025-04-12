using System;       // Para usar la consola  (Console)
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.AccessControl;    // Para leer archivos    (File)

public struct Persona
{
    // Propiedades
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }

    // Constructor
    public Persona(int id, string nombre, string telefono, string email)
    {
        Id = id;
        Nombre = nombre;
        Telefono = telefono;
        Email = email;
    }

    // Método para mostrar información
    public void MostrarInformacion()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"ID: {Id}, Nombre: {Nombre}, Teléfono: {Telefono}, Email: {Email}");
        Console.ForegroundColor = ConsoleColor.White;
    }
}


class Program
{
    static void Main()
    {
        int delay = 50; 

        int CantidadPersonas = 1 + 3; //cantidad de personas + 1 para el id 0
        string[] lineas = File.ReadAllLines("agenda.csv");
        string[] lineasCSV = new string[CantidadPersonas];
                int ContadorId = 1; //primer id

        if (lineas.Length > 0)
        {
            ContadorId = lineas.Length - 1;
        }

                    for (int i = 0; i < lineas.Length; i++)
                    {
                        if (lineas[i] == string.Empty) //si la primera linea es vacia, no se guarda nada
                        {
                            if (i == 0)
                            {
                                ContadorId = i+1;
                            }
                            else
                            {
                                ContadorId = i;
                            }
                            break;
                        }
                        else if (lineas[i] != string.Empty)
                        {
                            Console.WriteLine($"linea no vacia: {lineas[i]}");
                        }
                    }
        


        
        Persona[] Id = new Persona[CantidadPersonas]; //cantidad de personas

        void Inicio()
        {
            //Console.Clear();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  A G E N D A   U T N");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
            Console.Write("[1] "); Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("Agregar contacto");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[2] "); Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("Modificar contacto");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[3] "); Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("Borrar contacto");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[4] "); Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("Listar contacto");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[5] "); Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("Buscar contacto");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[0] "); Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("Salir");
        Console.WriteLine();
        Console.Write("Seleccioná una opción: ");
        }

        bool valido = false;

        //BORRARLE EL VALOR A SELECCION, NO DEBERIA ESTAR DECLARADO
        int seleccion;

        void MenuSeleccion()
        {
            do{
                Inicio();
                    
                    if (int.TryParse(Console.ReadLine(), out seleccion))
                    {
                        while (seleccion <0 || seleccion >5)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Número invalido, seleccionar nuevamente.");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("Selecciona una opción: ");
                            if (int.TryParse(Console.ReadLine(), out seleccion));
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Clear();
                        Console.WriteLine($"Seleccionaste la opción: {seleccion}");
                        if (seleccion == 0) { Console.Clear(); Console.WriteLine("Chau"); Thread.Sleep(delay); Environment.Exit(0); }
                        valido = true;
                        Thread.Sleep(delay);
                    }
                    else
                    {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Número invalido, seleccionar nuevamente.");
                            Console.ForegroundColor = ConsoleColor.White;
                    }
            } while(valido == false);
            Console.Clear();
        }

        do
        {
        MenuSeleccion();

        Console.ForegroundColor = ConsoleColor.Magenta;
        switch(seleccion)
        {
            case 1:
            if (ContadorId < CantidadPersonas - 1)
            {
                    bool otrocontacto = true;
                    Console.Clear();

                    
                    lineasCSV = new string[CantidadPersonas];
                    while(otrocontacto == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Agregar contacto");
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.White;
                        
                        
                        Console.Write("Nombre: ");
                        string Nombre = Console.ReadLine();
                        if (Nombre == null || Nombre == string.Empty)
                        do {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Nombre vacio, media pila");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("Nombre: ");
                        Nombre = Console.ReadLine();
                        } while (Nombre == null || Nombre == string.Empty);
                        
                        Console.Write("Telefono: ");
                        string Telefono = Console.ReadLine();
                        if (Telefono == null || Telefono == string.Empty)
                        do {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Telefono vacio, media pila");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("Telefono: ");
                        Telefono = Console.ReadLine();
                        } while (Telefono == null || Telefono == string.Empty);

                        Console.Write("Email: ");
                        string Email = Console.ReadLine();
                        if (Email == null || Email == string.Empty)
                        do {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Email vacio, media pila");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("Email: ");
                        Email = Console.ReadLine();
                        } while (Email == null || Email == string.Empty);
                        

                        Console.ForegroundColor = ConsoleColor.Green;
                        
                        
                        //CREACION DEL CONTACTO
                        Id[0] = new Persona(0, "Nombre", "Telefono", "Email");
                        Id[ContadorId] = new Persona(ContadorId, Nombre, Telefono, Email);
                        Console.Clear();
                        Console.WriteLine("variable local");
                        Id[ContadorId].MostrarInformacion();
                        //CREACION DEL CONTACTO

                        //JOIN DE LA VARIABLE //join=sin espacios
                        string localCSV = string.Join(",", ContadorId, Nombre, Telefono, Email);
                        // Console.WriteLine("variable ya joineada");
                        // Console.WriteLine(localCSV);

                        
                        lineasCSV[ContadorId] = localCSV;

                        File.WriteAllLines("agenda.csv", lineasCSV);
                        Console.WriteLine("Se guardó el contacto en el archivo agenda.csv");

                        // string[] lineas = File.ReadAllLines("agenda.csv");
                        // Console.WriteLine("Contenido de agenda.csv:");
                        // Console.WriteLine(string.Join("\n", lineas));


                        //JOIN DE LA VARIABLE
                    

                        //EXPORTADO A CSV EXTERNO
                        

                        //SPLIT DE LA VARIABLE
                        // string[] splitArray = localCSV.Split(new char[] { ',' });
                        //     for (int i = 0; i < splitArray.Length; i++)
                        //     {
                        //         splitArray[i] = splitArray[i].Trim(); // Elimina espacios en blanco innecesarios
                        //         Console.Write($"Elemento {i}: {splitArray[i]}");          // Muestra cada elemento del array
                        //     }
                        //SPLIT DE LA VARIABLE



                        Console.WriteLine();
                        Console.WriteLine($"{ContadorId} "+ "de" + $" {CantidadPersonas - 1} contactos agregados.");
                        if (ContadorId >= CantidadPersonas - 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("");
                            Console.WriteLine("Se alcanzó el limite de contactos");
                            Console.WriteLine("Presioná cualquier tecla para continuar");
                            Console.ReadKey();
                            otrocontacto = false;
                        }
                        else
                        {
                            ContadorId++;
                                                    Console.WriteLine();
                        Console.Write("¿Agregar otro contacto? (si/no): ");
                        string respuesta = Console.ReadLine().ToLower();
                        if (respuesta == "si" || respuesta == "s")
                        {
                            otrocontacto = true;
                            Console.Clear();
                        }
                        else if (respuesta == "no" || respuesta == "n")
                        {
                            otrocontacto = false;
                            Console.Clear();
                            

                            
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Opción inválida, seleccioná nuevamente.");
                            otrocontacto = true;
                        }
                        }
                        
                    }
                    
            }
            else {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ContadorId} "+ "de" + $" {CantidadPersonas - 1} contactos disponibles.");
                Console.WriteLine("No se pueden agregar más contactos, presioná cualquier tecla para continuar.");
                Console.ReadKey();
            }
                break;

            case 2:
                    Console.Clear();
        Console.WriteLine("Modificar contacto");
        Console.Write("Ingrese el ID del contacto a modificar: ");
        if (int.TryParse(Console.ReadLine(), out int idAModificar))
        {
            // Leer todas las líneas del archivo CSV
            lineas = File.ReadAllLines("agenda.csv");
            bool encontrado = false;

            for (int i = 0; i < lineas.Length; i++)
            {
                string[] datos = lineas[i].Split(',');
                if (datos.Length == 4 && int.TryParse(datos[0], out int id) && id == idAModificar)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Contacto encontrado");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"ID: {datos[0]}, Nombre: {datos[1]}, Teléfono: {datos[2]}, Email: {datos[3]}");
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.Write("Nuevo Nombre: ");
                    string nuevoNombre = Console.ReadLine();
                    Console.Write("Nuevo Teléfono: ");
                    string nuevoTelefono = Console.ReadLine();
                    Console.Write("Nuevo Email: ");
                    string nuevoEmail = Console.ReadLine();

                    lineas[i] = $"{id},{nuevoNombre},{nuevoTelefono},{nuevoEmail}";

                    encontrado = true;
                    break;
            }
        }

        if (encontrado)
        {
            // Sobreescribir el archivo con las líneas modificadas
            File.WriteAllLines("agenda.csv", lineas);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Contacto modificado");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"No se encontró un contacto con ID {idAModificar}.");
        }
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("ID inválido. Por favor, ingresa un ID válido.");
    }
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Presioná cualquier tecla para continuar.");
    Console.ReadKey();
                break;
            case 3:
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Borrar contacto");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Ingrese el ID del contacto a borrar: ");
                if (int.TryParse(Console.ReadLine(), out int idABorrar))
                {
                    lineas = File.ReadAllLines("agenda.csv");
                    for (int i = 0; i < lineas.Length; i++)
                    {
                        string[] datos = lineas[i].Split(',');
                        bool Borrado = false;
                        if (datos.Length == 4 && int.TryParse(datos[0], out int id))
                        {
                            if (id == idABorrar)
                            {
                                Console.Clear();
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Contacto con ID {id} borrado.");
                                lineas[i] = null; // Marcar la línea como nula para eliminarla
                                File.WriteAllLines("agenda.csv", lineas); // Sobreescribir el archivo
                                Borrado = true;
                                break; // Salir del bucle una vez que se ha borrado el contacto
                            }
                        }
                        else
                        {
                            if (Borrado == false)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"No se encontró un contacto con ID {idABorrar}.");
                            }
                        }
                    }
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Presioná cualquier tecla para salir");
                            Console.ReadKey();
                            Console.ForegroundColor = ConsoleColor.White;
                }
                
                break;
            case 4:
                Console.WriteLine("Listar contacto");
                lineas = File.ReadAllLines("agenda.csv");
                Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.White;
                            
                            Console.WriteLine("Contenido de agenda.csv:");
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Green;
                            for (int i = 1; i < lineas.Length; i++)
                            {
                                string[] datos = lineas[i].Split(','); //separar por comas
                                if (datos.Length == 4) // ID Nombre Teléfono Email
                                {
                                    Console.WriteLine($"ID: {datos[0]}, Nombre: {datos[1]}, Teléfono: {datos[2]}, Email: {datos[3]}");
                                }
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Presioná cualquier tecla para continuar");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.ReadKey();
                break;
            case 5:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Buscar contacto");
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write("Ingrese el ID del contacto: ");
                    lineas = File.ReadAllLines("agenda.csv");

                    if (int.TryParse(Console.ReadLine(), out int idbuscado))
                    {
                        bool encontrado = false;

                        // Buscar el contacto por ID utilizando un ciclo for
                        for (int i = 0; i < lineas.Length; i++)
                        {
                            // Split la línea en partes
                            string[] datos = lineas[i].Split(',');

                            if (datos.Length == 4) // Asegurarse de que hay 4 elementos
                            {
                                // Comprobar si el ID coincide con el ID buscado
                                if (int.TryParse(datos[0], out int id) && id == idbuscado)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"ID: {datos[0]}, Nombre: {datos[1]}, Teléfono: {datos[2]}, Email: {datos[3]}");
                                    encontrado = true;
                                    break; // Si encontramos el contacto, salimos del bucle
                                }
                            }
                        }

                        if (!encontrado)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("No se encontró un contacto con ese ID.");
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ID inválido, presioná cualquier tecla para salir");
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadKey();

                break;
        }
        
        } while (seleccion != 0);
    }
}