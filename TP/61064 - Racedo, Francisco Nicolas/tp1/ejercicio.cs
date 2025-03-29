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
struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class Program
{
    const int MaxContactos = 100;
    static Contacto[] contactos = new Contacto[MaxContactos];
    static int contadorContactos = 0;
    static void Main(string[] args)
    {
        // Cargar contactos desde el archivo
        CargarContactosDesdeArchivo();

        // Mostrar menu y procesar opciones
        while (true){
            MostrarMenu();
            string opcion = Console.ReadLine();
            
            switch(opcion)
            {
                case "1":
                    AgregarContacto();
                    break;
                case "2":
                    ModificarContacto();
                    break;
                case "3":
                    BorrarContacto();
                    break;
                case "4":
                    MostrarContacto();
                    break;
                case "5":
                    BuscarContacto();
                    break;
                case "6":
                    GuardarContactosEnArchivo();// Guardar contactos en el archivo
                    MostarMensaje("Muchas gracias..Hasta luego");
                    return;
                default:
                    MostrarMensaje("Opción no válida. Intente nuevamente.");//Muestra un mensaje de salida
                    break;
            }
        }  
    }


    static void MostrarMenu()
    {
        Console.Clear();
        Console.WriteLine(new string('-', 50));
        Console.WriteLine("Menú de Agenda de Contactos");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine("1. Agregar contacto");
        Console.WriteLine("2. Modificar contactos");
        Console.WriteLine("3. Borrar contactos");
        Console.WriteLine("4. Mostrar contactos");
        Console.WriteLine("5. Buscar contactos");
        Console.WriteLine("6. Salir");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine("Seleccione una opcion por favor");
    }
    static void MostrarMensaje(string mensaje)
    {
        Console.WriteLine(mensaje);
        Console.WriteLine("Presione ENTER para continuar por favor");
        Console.ReadLine();
    }

    static void AgregarCcontacto()
    {
        for (int i = 0; i < contactos.Length; i++)
        {
            if (contactos[i].Id == 0)//Buscar en el array una posicion vacia
            {
                Console.WriteLine(new string('-',50));
                Contacto nuevoContacto = new Contacto
                {
                    Id = i+1, //Asigna el Id de acuerdo a la posicion
                    Nombre = LeerEntrada("Ingrese el nombre por favor: "),
                    Telefono = LeerEntrada("Ingrese el telefono por favor: "),
                    Email = LeerEntrada("Ingrese el email por favor: ")
                };
                contactos[i] = nuevoContacto;
                contadorContactos++;
                Console.WriteLine(new string('-',50));
                MostrarMensaje("Contacto agregado correctamente.");
                return;
        }
    }
    MostrarMensaje("La agenda esta llena. No se pueden agregar mas contactos. Lo sentimos.");
}
static void ModificarContacto()
{
    Console.WriteLine(new string('-',70));
    int Id = LeerId("Ingrese el Id del contacto a modificar por favor");
    int indice = BuscarIndicePorId(id);
    if (indice == -1)
    {
        MostrarMensaje("El contacto no existe. No se encontro.");
        return;
    }
    contactos[indice].Nombre = LeerEntrada("Ingrese el nuevo nombre por favor.(Si no desea cambiarlo..no haga nada.): ",contactos[indice].Nombre);
    contactos[indice].Telefono = LeerEntrada("Ingrese el nuevo telefono por favor.(Si no desea cambiarlo..no haga nada.): ",contactos[indice].Telefono);
    contactos[indice].Email = LeerEntrada("Ingrese el nuevo email por favor.(Si no desea cambiarlo..no haga nada.): ",contactos[indice].Email);
    Console.WriteLine('-',70);
    MostrarMensaje("El contacto a sido modificado correctamente");
}
static void BorrarContacto()
{
    int id = LeerId("Ingrese el Id del contacto que desea borrar por favor: ");
    int indice = BuscarIndicePorId(id);
    if (indice == -1)
    {
        MostrarMensaje("El contacto no existe. No se encontro.");
        return;
    }

    for (int i = indice;i < contadorContactos -1; i++)
    {
        contactos[i] = contactos[i+1];
        contactos[i].Id = i+1; //Actualiza el Id del contacto que se desplaza
    }
    //Limpia el ultimo elemento del array
    contactos[contadorContactos -1] = default;

    contadorContactos--;
    Console.WriteLine(new string('-',50));
    MostrarMensaje("El contacto fue borrado correctamente.");
}

static void MostrarContacto()
{
    const int anchoTotal = 80; // Ancho total de la tabla
    string separador = new string('-', anchoTotal); //Cambia la const por un string normal
    Console.WriteLine("|{0,5} | {1,20} | {2,15} | {3,30} |","ID","Nombre","Telefono","Email");
    Console.WriteLine(separador);

    for (int i = 0; i < contadorContactos; i++)
    {
        Console.WriteLine("|{0,5} | {1,20} | {2,15} | {3,30} |",contactos[i].Id,contactos[i].Nombre,contactos[i].Telefono,contactos[i].Email);
        Console.WriteLine(separador);
    }
    MostrarMensaje("");
}
static void BuscarContacto()
{
    Console.WriteLine(new string('-',70));
    Console.WriteLine("|{0,5} | {1,20} | {2,15} | {3,30} |","ID","Nombre","Telefono","Email");
    Console.WriteLine(new string('-',70));

    string termino = LeerEntrada("Ingrese el termino de busqueda: ").ToLower();
    bool encontardo = false;

    for (int i = 0; i < contadorContactos; i++)
    {
        string nombre = contactos[i].Nombre?ToLower();
        string telefono = contactos[i].Telefono?ToLower();
        string email = contactos[i].Email?ToLower();

        if (nombre?.Contains(termino) == true || telefono?.Contains(termino) == true || email?.Contains(termino) == true)
        {
            Console.WriteLine("|{0,5} | {1,20} | {2,15} | {3,30} |",contactos[i].Id,contactos[i].Nombre,contactos[i].Telefono,contactos[i].Email);
            Console.WriteLine(new string('-',70));
            encontardo = true;
        }
    }

    if (!encontardo)
    {
        MostrarMensaje("No se encontarron contactos con el termino de busqueda sugerido. Por favor..intente nuevamente");
    }
    else
    {
        MostrarMensaje("");
    }
}

static int BuscarIndicePorId(int id)
{
    for (int i = 0; i < contadorContactos; i++)
    {
        if (contactos[i].Id == id)
        {
            return i;
        }
    }
    return -1;
}

static void CargarContactosDesdeArchivo()
{
    if (!File.existe("agenda.csv"))return;

    string[] lineas = File.ReadAllLines("agenda.csv");

    for (int i = 0; i < lineas.Length; i++)
    {
        string[] datos = lineas[i].Split(',');
        if (datos.Length == 4 && BuscarIndicePorId(int.Parse(datos[0])) == -1)
        {
            contactos[contadorContactos++] = new Contacto
            {
                Id = int.Parse(datos[0]),
                Nombre = datos[1],
                Telefono = datos[2],
                Email = datos[3]
            };
        }
    }
}

static void GuardarContactosEnArchivo()
{
    using (var sw = new StreamWriter("agenda.csv"))
    {
        for (int i = 0; i < contadorContactos; i++)
        {
            sw.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
        }
    }
}

static string LeerEntrada(string mensaje, string valorPorDefecto = "")
{
    Console.WriteLine(mensaje);
    string entrada = Console.ReadLine();
    return string.IsNullOrEmpty(entrada) ? valorPorDefecto : entrada;
}

static int LeerId(string mensaje)
{
    Console.WriteLine(mensaje);
    return int.TryParse(Console.ReadLine(),out int id) ? id : -1; 
}
}
