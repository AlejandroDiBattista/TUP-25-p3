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

struct Contacto{
    public int Id;
    public string Nombre;
    public string Email;
    public string Telefono;
}
class Program{
 const int maxContactos = 15;
 static Contacto[] contactos = new Contacto[maxContactos];
 static int cantidadContactos = 2;
 static string archivo = "agenda.csv";
 static void Main()
    {
     contactos[0] = new Contacto {Id = 1, Nombre = "Juan Perez", Email = "jperez@mail.com", Telefono = "1234567890"};
     contactos[1] = new Contacto {Id = 2, Nombre = "Maria Lopez", Email = "mlopez@mail.com", Telefono = "0987654321"};
     CargarContactos();

      while(true){

        Console.WriteLine("\n =========================");
        console.WriteLine("AGENDA DE CONTACTOS");
        console.WriteLine("=========================");
        console.WriteLine("1. Agregar contacto");
        console.WriteLine("2. Modificar contacto");
        console.WriteLine("3. Eliminar contacto");
        console.WriteLine("4. Buscar contacto");
        console.WriteLine("5. Listar contactos");
        console.WriteLine("6. Salir");
        console.WriteLine("=========================");
        console.WriteLine("Por favor, seleccione una opcion: ");
        int opcion = int.Parse(Console.ReadLine());
        
        switch(opcion){
          case 1: AgregarContacto();
          break;
          case 2: ModificarContacto();
          break;
          case 3: EliminarContacto();
          break;
          case 4: BuscarContacto();
          break;
          case 5: ListarContactos();
          break;
          case 6: Salir();
          break;
          default: Console.WriteLine("Opción no válida. Intente nuevamente.");
          break;
          }
        }
    }
}

 static void AgregarContacto(){

    if (cantidadContactos >= maxContactos){
    Console.WriteLine("Lo siento, no se pueden agregar más contactos.");
    return;
    }

    Contacto nuevo;
    nuevo.Id = cantidadContactos + 1;
    Console.WriteLine("Ingrese el nombre del contacto: ");
    nuevo.Nombre = Console.ReadLine();
    Console.WriteLine("Ingrese el correo del contacto: ");
    nuevo.Email = Console.ReadLine();
    Console.WriteLine("Ingrese el telefono del contacto: ");
    nuevo.Telefono = Console.ReadLine();

    contactos[cantidadContactos] = nuevo;
    cantidadContactos++;
    Console.WriteLine("El contacto fue guardado con exito.");
    Console.WriteLine("Presione cualquier tecla para volver al menu principal.");
    Console.ReadKey();
    Console.Clear();
}

static void ModificarContacto(){

    Console.WriteLine("Por favor ingrese el Id de su contacto a modificar, teniendo en cuenta que los dos contactos que vienen por defecto utilizan el 1 y 2 como identificador, y preste atención a las indicaciones: ");
    if (int.TryParse(Console.ReadLine(), out int id)){
        for (int i = 0; i < cantidadContactos; i++){
            if (contactos[i].Id == id){

             console.WriteLine("ingrese el nuevo nombre del contacto, de no querer modificarlos, deje un espacio vacio: ");
             string nombre = console.ReadLine();
             if (!string.IsNullOrEmpty(nombre)) agenda[i].Nombre = nombre;
             console.WriteLine("ingrese el nuevo email del contacto, de no querer modificarlos, deje un espacio vacio: ");
             string email = console.ReadLine();
             if (!string.IsNullOrEmpty(email)) agenda[i].Email = email;
             console.WriteLine("ingrese el nuevo numero de telefono del contacto, de no querer modificarlos, deje un espacio vacio: ");
             string telefono = console.ReadLine();
             if (!string.IsNullOrEmpty(telefono)) agenda[i].Telefono = telefono;
             Console.WriteLine("Su contacto fue modificado con éxito. Presione cualquier tecla para volver al menú principal.");
             Console.ReadKey();
             Console.Clear();
             return;
            }
        }
    }
}

static void EliminarContacto(){
    console.WriteLine("por favor ingrese el Id del contacto a eliminar, teniendo en cuenta que los dos que contactos que vienen por defecto ocupan el Id numero 1 y 2: ");
    if (int.TryParse (console.ReadLine(), out int id)){
        for (int i = 0; i < contadorcontactos; i++){
            if (contactos[i].Id == id){
                for (int j = i; j < cantidadContactos - 1; j++){
                    contactos[j] = contactos[j + 1];
                }
                cantidadContactos--;
                Console.WriteLine("Su contacto fue eliminado con éxito. Presione cualquier tecla para volver al menú principal.");
                Console.ReadKey();
                Console.Clear();
                return;
            }
        }
    }

    console.WriteLine("el Id que ingreso no es valido, por favor intente nuevamente.");
    console.ReadKey();
    conosle.Clear();
}

static void BuscarContacto(){

    console.Write("ingrese el nombre del contacto a buscar: ");
    string buscado = console.ReadLine().ToLower();
    bool encontrado = false;
    Console.WriteLine("\nId   Nombre       Teléfono     Email");
    Console.WriteLine("--------------------------------------");

    for (int i = 0; i < cantidadContactos; i++){
     if (contactos[i].Nombre.ToLower().Contains(buscado) ||
         contactos[i].Email.ToLower().Contains(buscado) ||
         contactos[i].Telefono.ToLower().Contains(buscado)){

        Console.WriteLine($"{contactos[i].Id,-4} {contactos[i].Nombre,-12} {contactos[i].Telefono,-12} {contactos[i].Email}");
        encontrado = true;
    }
}
    if (!encontrado){
        Console.WriteLine("No se encontraron contactos que coincidan con la busqueda, vuelve a intentarlo.");
    }
    Console.WriteLine("Presione cualquier tecla para volver al menu principal.");
    Console.ReadKey();
    Console.Clear();
}

static void ListarContactos(){
    Console.WriteLine("Lista de contactos: ");
    Console.WriteLine("\nId   Nombre       Teléfono     Email");
    Console.WriteLine("--------------------------------------");
    for (int i = 0; i < cantidadContactos; i++){
        Console.WriteLine($"{contactos[i].Id,-4} {contactos[i].Nombre,-12} {contactos[i].Telefono,-12} {contactos[i].Email,-12}");
    }
}

static void CargarContactos(){
    if (File.Exists(archivo)){
        string[] lineas = File.ReadLines(archivo).ToArray();
        for (int i = 0; i < lineas.Length; i++){
            string[] partes = lineas[i].Split(',');
            contactos[i] = new Contacto{
                Id = int.Parse(partes[0]),
                Nombre = partes[1],
                Email = partes[2],
                Telefono = partes[3]
            };
        }
        cantidadcontactos = lineas.Length;
    }
}

static void GuardarContactos(){
    string[] lineas = new string[cantidadcontactos];
    for (int i = 0; i < cantidadcontactos; i++){
        lineas[i] = $"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Email},{contactos[i].Telefono}";
    }
    File.WriteAllLines(archivo, lineas);
}

    Console.WriteLine("Los contactos fueron guardados con exito.");
    Console.WriteLine("Presione cualquier tecla para volver al menu principal.");
    Console.ReadKey();
    Console.Clear();