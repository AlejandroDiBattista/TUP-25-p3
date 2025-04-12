using System;
using System.IO;


public struct Contacto {
    public int Id = 0;
    public string Nombre;
    public string Telefono;
    public string Email;
    
    public Contacto(string nombre, string telefono, string email) {
        Nombre = nombre;
        Telefono = telefono;
        Email = email;
        Id = Contacto.ProximoID++;
    }

    public bool Contains(string texto) {
        return Nombre.ToLower().Contains(texto.ToLower()) || 
               Telefono.ToLower().Contains(texto.ToLower()) || 
               Email.ToLower().Contains(texto.ToLower());
    }

    static int ProximoID = 1;
}

static string LeerTexto(string texto){
    Console.Write($"  {texto, 10}: ");
    return Console.ReadLine()?.Trim() ?? ""; // Me aseguro que siembre retorne un string
}

static int LeerNumero(string mensaje="Seleccione una opción"){
    int opcion;
    while(true) {
        string linea = LeerTexto(mensaje);
        if(int.TryParse(linea, out opcion)){
            return opcion;
        }
        Console.WriteLine("Por favor ingrese un numero...");
    } 
}

static void Pausa(string mensaje=""){  
    Console.Write($"{mensaje}. Presione una tecla para continuar...");
    Console.ReadKey();
    Console.WriteLine();
}

void MostrarMenu(){
    Console.Clear();
    Console.WriteLine("===== AGENDA DE CONTACTOS =====");
    Console.WriteLine("1) Agregar contacto");
    Console.WriteLine("2) Modificar contacto");
    Console.WriteLine("3) Borrar contacto");
    Console.WriteLine("4) Listar contactos");
    Console.WriteLine("5) Buscar contacto");
    Console.WriteLine("0) Salir");
}

static int LeerId(){
    while(true){
        string texto = LeerTexto("Ingrese ID del contacto: ");
        if (int.TryParse(texto, out int id)){
            return id;
        }
        Console.WriteLine("ID inválido. Intente nuevamente.");
    }
}

const int MAX_CONTACTOS = 100;
static Contacto[] contactos = new Contacto[MAX_CONTACTOS];
static int cantidad = 0;
static int nextId = 1;
static string archivo = "agenda.csv";

static void AgregarContacto() {
    if(cantidad >= MAX_CONTACTOS) {
        Pausa("Límite de contactos alcanzado");
        return;
    }
    Console.Clear();
    Console.WriteLine("=== Agregar Contacto ===");
    string nombre   = LeerTexto("Nombre");
    string telefono = LeerTexto("Telefono");
    string email    = LeerTexto("Email");
    
    contactos[cantidad++] = new Contacto(nombre, telefono, email);
    Pausa();
}

static void ModificarContacto() {
    Console.Clear();
    var id = LeerId();
    int index = BuscarIndicePorId(id);
    if(index == -1) {
        Pausa("Contacto no encontrado");
        return;
    }
    Console.WriteLine($"Datos actuales => Nombre: {contactos[index].Nombre}, Teléfono: {contactos[index].Telefono}, Email: {contactos[index].Email}");
    Console.WriteLine("(Deje el campo en blanco para no modificar)");
    
    string nombre   = LeerTexto("Nombre");
    string telefono = LeerTexto("Teléfono");
    string email    = LeerTexto("Email");
    if(nombre != "") contactos[index].Nombre = nombre;
    if(telefono!="") contactos[index].Telefono = telefono;
    if(email != "")  contactos[index].Email = email;
    Console.WriteLine("Contacto modificado con éxito.");
    Pausa();
}

static void BorrarContacto() {
    Console.Clear();
    Console.WriteLine("=== Borrar Contacto ===");
    int id = LeerId();
    int index = BuscarIndicePorId(id);
    if (index == -1){
        Pausa("Contacto no encontrado");
        return;
    }
    for(int i = index; i < cantidad - 1; i++) {
        contactos[i] = contactos[i+1];
    }
    cantidad--;
    Console.WriteLine($"Contacto con ID={id} eliminado con éxito.");
    Pausa();
}

static void ListarContactos() {
    Console.Clear();
    Console.WriteLine("=== Lista de Contactos ===");
    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");
    for(int i = 0; i < cantidad; i++) {
        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
    }
    Pausa();
}

static void BuscarContacto() {
    Console.Clear();
    Console.WriteLine("=== Buscar Contacto ===");
    string termino = LeerTexto("Ingrese un término de búsqueda (nombre, teléfono o email)");
    Console.WriteLine("\nResultados de la búsqueda:");
    Console.WriteLine("{0,-5} {1,-25} {2,-15} {3,-25}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");
    bool encontrado = false;
    for(int i = 0; i < cantidad; i++) {
        if(contactos[i].Contains(termino)) {
            Console.WriteLine("{0,-5} {1,-25} {2,-15} {3,-25}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
            encontrado = true;
        }
    }
    if(!encontrado) {
        Console.WriteLine("No se encontraron contactos.");
    }
    Pausa();
}

static int BuscarIndicePorId(int id) {
    for(int i = 0; i < cantidad; i++) {
        if(contactos[i].Id == id)
            return i;
    }
    return -1;
}

static void LeerAgenda() {
    if(File.Exists(archivo)) {
        foreach (string line in File.ReadLines(archivo).Skip(1)) {
            if(string.IsNullOrWhiteSpace(line)) continue;
            string[] parts = line.Split(',');
            if(parts.Length < 3) continue;
            Contacto contacto = new Contacto(parts[0], parts[1], parts[2]);
            contactos[cantidad++] = contacto;
        }
    }
}

static void GuardarAgenda() {
    var lines = new List<string>();
    lines.Add("nombre,telefono,email");
    for (int i = 0; i < cantidad; i++) {
        lines.Add($"{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
    }
    File.WriteAllLines(archivo, lines);
}

LeerAgenda();
int opcion;
do {
    MostrarMenu();
    opcion = LeerNumero("Ingrese una opcion (0-6)" );
    switch(opcion) {
        case 1: AgregarContacto();  break;
        case 2: ModificarContacto();break;
        case 3: BorrarContacto();   break;
        case 4: ListarContactos();  break;
        case 5: BuscarContacto();   break;
    }
} while(opcion != 0);
GuardarAgenda();
