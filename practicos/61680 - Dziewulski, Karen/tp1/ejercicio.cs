using System;
using System.IO;

class Program{
    static string Archivo = "agenda.csv";
    static Contacto[] ListaC = new Contacto[10];
    static int contador = 0;

    struct Contacto{
        public int Id;
        public string Nombre;
        public string Telefono;
        public string Gmail;
    }

    static void Main(string[] args){
        CargarArchivo();  

        bool salir = false;
        
        for (int intentos = 0; !salir; intentos++){
            Console.Clear();
            Console.WriteLine("Bienvenido a la agenda de contactos!");
            Console.WriteLine("Seleccione una opción :D");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Eliminar contacto");
            Console.WriteLine("4. Lista de contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");

            int opcion = Validacion();

            if (opcion == 1){
                AgregarContacto();
                GuardarContactosEnArchivo();  
            }
            else if (opcion == 2){
                ModificarContacto();
                GuardarContactosEnArchivo();  
            }
            else if (opcion == 3){
                EliminarContacto();
                GuardarContactosEnArchivo(); 
            }
            else if (opcion == 4){
                ListadeContactos();
            }
            else if (opcion == 5){
                BuscarContacto();
            }
            else if (opcion == 6){
                Console.WriteLine("Saliendo...");
                salir = true;  
            }
            else{
                Console.WriteLine("Opción inválida. Intente de nuevo.");
            }

            Console.WriteLine("Presione cualquier tecla para regresar al menú.");
            Console.ReadKey();  
        }
    }

    static int Validacion(){
        int opcion = 0;
        for (int intentos = 0; intentos < 3; intentos++){
            Console.Write("Seleccione una opción: ");
            string input = Console.ReadLine();
            
            if (int.TryParse(input, out opcion)) 
            {
                if (opcion >= 1 && opcion <= 6){
                    return opcion;  
                }else{
                    Console.WriteLine("Ingrese una opción válida entre 1 y 6.");
                }
            }else{
                Console.WriteLine("Ingrese un número válido.");
            }
        }
        return 0;
    }

    static void CargarArchivo(){

        if (File.Exists(Archivo)){

            string[] lineas = File.ReadAllLines(Archivo);

            for (int i = 0; i < lineas.Length; i++){
                string[] datos = lineas[i].Split(',');

                if (datos.Length == 4){
                    Contacto contacto = new Contacto{
                        Id = int.Parse(datos[0]),
                        Nombre = datos[1],
                        Telefono = datos[2],
                        Gmail = datos[3]
                    };

                    if (contador < ListaC.Length){
                        ListaC[contador] = contacto;
                        contador++;
                    }
                }
            }
        }
    }

    static void GuardarContactosEnArchivo(){
        using (StreamWriter writer = new StreamWriter(Archivo)){
            for (int i = 0; i < contador; i++){
                Contacto c = ListaC[i];
                writer.WriteLine($"{c.Id},{c.Nombre},{c.Telefono},{c.Gmail}");
            }
        }
    }

   
    static void ModificarContacto(){
        Console.Clear();
        Console.WriteLine("----- Modificar Contacto -----");
        Console.WriteLine("Ingrese el ID del contacto que desea modificar: ");
        int id = int.Parse(Console.ReadLine());
        bool encontrado = false;

        for (int i = 0; i < contador; i++){
            if (ListaC[i].Id == id)
            {
                Console.WriteLine("Ingrese el nuevo nombre del contacto (deje en blanco si no desea cambiar): ");
                string nuevoNombre = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoNombre))
                    ListaC[i].Nombre = nuevoNombre;

                Console.WriteLine("Ingrese el nuevo teléfono del contacto (deje en blanco si no desea cambiar): ");
                string nuevoTelefono = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoTelefono))
                    ListaC[i].Telefono = nuevoTelefono;

                Console.WriteLine("Ingrese el nuevo Gmail del contacto (deje en blanco si no desea cambiar): ");
                string nuevoGmail = Console.ReadLine();
                if (!string.IsNullOrEmpty(nuevoGmail))
                    ListaC[i].Gmail = nuevoGmail;

                Console.WriteLine("Contacto modificado correctamente.");
                encontrado = true;
                break;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("No se encontró un contacto con ese ID.");
        }
    }

    static void EliminarContacto()
{
    Console.Clear();
    Console.WriteLine("----- Eliminar Contacto -----");
    Console.WriteLine("Ingrese el ID del contacto que desea eliminar: ");
    int id = int.Parse(Console.ReadLine());
    bool encontrado = false;

    for (int i = 0; i < contador; i++)
    {
        if (ListaC[i].Id == id)
        {
            for (int j = i; j < contador - 1; j++)
            {
                ListaC[j] = ListaC[j + 1];
            }
            contador--;
            Console.WriteLine("Contacto eliminado correctamente.");
            encontrado = true;
            break;
        }
    }

    if (!encontrado)
    {
        Console.WriteLine("No se encontró un contacto con ese ID.");
    }
}

static void AgregarContacto()
{
    Console.Clear();
    Console.WriteLine("----- Agregar Contacto -----");
    Console.WriteLine("Ingrese el nombre del contacto: ");
    string nombre = Console.ReadLine();
    Console.WriteLine("Ingrese el teléfono del contacto: ");
    string telefono = Console.ReadLine();
    Console.WriteLine("Ingrese el Gmail del contacto: ");
    string gmail = Console.ReadLine();

    int nuevoId = 1;
    for (int i = 0; i < contador; i++)
    {
        if (ListaC[i].Id >= nuevoId)
        {
            nuevoId = ListaC[i].Id + 1;
        }
    }

    Contacto nuevoContacto = new Contacto
    {
        Id = nuevoId,
        Nombre = nombre,
        Telefono = telefono,
        Gmail = gmail
    };

    if (contador < ListaC.Length)
    {
        ListaC[contador] = nuevoContacto;
        contador++;
        Console.WriteLine("Contacto agregado correctamente.");
    }
    else
    {
        Console.WriteLine("No hay espacio para más contactos.");
    }
}


    static void ListadeContactos()
    {
        Console.Clear();
        Console.WriteLine("----- Lista de Contactos -----");
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "Nombre", "Telefono", "Gmail");
        Console.WriteLine("-------------------------------------------");
        for (int i = 0; i < contador; i++)
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", ListaC[i].Id, ListaC[i].Nombre, ListaC[i].Telefono, ListaC[i].Gmail);
        }
    }

    static void BuscarContacto()
    {
        Console.Clear();
        Console.WriteLine("----- Buscar Contacto -----");
        Console.WriteLine("Ingrese algun dato que desea encontrar: ");
        string busqueda = Console.ReadLine().ToLower();
        bool encontrado = false;

        for (int i = 0; i < contador; i++)
        {
            if (ListaC[i].Nombre.ToLower().Contains(busqueda) || ListaC[i].Telefono.ToLower().Contains(busqueda) || ListaC[i].Gmail.ToLower().Contains(busqueda))
            {
                Console.WriteLine($"ID: {ListaC[i].Id}, Nombre: {ListaC[i].Nombre}, Telefono: {ListaC[i].Telefono}, Gmail: {ListaC[i].Gmail}");
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("No se encontraron resultados para la búsqueda.");
        }
    }
}
