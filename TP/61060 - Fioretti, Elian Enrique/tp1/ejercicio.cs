// Load Library
using static System.Console;
using System.IO;
// Code
struct Contact{
    public int Id;
    public string Name, Phone, Mail;
    public Contact(int id, string nombre, string telefono, string mail){
        Id = id;
        Name = nombre;
        Phone = telefono;
        Mail = mail;
    }
}
class Program{
// File
    static Contact[] ReadFile(string pathFile){
        if(!File.Exists(pathFile)) return new Contact[0];
        string[] array_LoadFile = File.ReadAllLines(pathFile);
        int amountContact = array_LoadFile.Length - 1;
        if(amountContact <= 0) return new Contact[0];
        Contact[] Contacts = new Contact[amountContact];
        for (int i = 1; i < array_LoadFile.Length; i++){
            string[] array_ContactsParts = array_LoadFile[i].Split(",");
            Contacts[i - 1] = new Contact(
                int.Parse(array_ContactsParts[0].Trim()),
                array_ContactsParts[1].Trim(),
                array_ContactsParts[2].Trim(),
                array_ContactsParts[3].Trim()
            );
        }
        return Contacts;
    }
    static void SaveFile(string pathFile, Contact[] Contacts){
        using (StreamWriter writer = new StreamWriter(pathFile)){
            writer.WriteLine("Id,Name,Phone,Mail");
            for (int i = 0; i < Contacts.Length; i++){
                if (i == Contacts.Length - 1){
                    writer.Write($"{Contacts[i].Id},{Contacts[i].Name},{Contacts[i].Phone},{Contacts[i].Mail}");
                }
                else{
                    writer.WriteLine($"{Contacts[i].Id},{Contacts[i].Name},{Contacts[i].Phone},{Contacts[i].Mail}");
                }
            }
        }
    }
    static void AppEndFile(string pathFile, Contact newContact){
        if (!File.Exists(pathFile)){
            File.WriteAllText(pathFile, "Id,Name,Phone,Mail");
        }
        File.AppendAllText(pathFile, $"\n{newContact.Id},{newContact.Name},{newContact.Phone},{newContact.Mail}");
    }
// Screen
    public class Screen{
        static string pathFile = "agenda.csv";
    // Menu Principal
        public static void screen_AgendaDeContactos(){
            Clear();
            WriteLine("¡:=========================:¡");
            WriteLine("|    Agenda de Contactos    |");
            WriteLine("|:-------------------------:|");
            WriteLine("| 1 - Agregar               |");
            WriteLine("| 2 - Modificar             |");
            WriteLine("| 3 - Borrar                |");
            WriteLine("| 4 - Listar                |");
            WriteLine("| 5 - Buscar                |");
            WriteLine("| 0 - Salir                 |");
            WriteLine("!:=========================:!");
            Write    ("  = - Opcion a ingresar: ");
            string option = ReadLine() ?? "Fraccionabilidad";
            if (option == "0") screen_Salir();
            else if (option == "1") screen_AgregarContacto_IngresoDatos();
            else if (option == "2") screen_ModificarContacto_BuscarDatos(option);
            else if (option == "3") screen_ModificarContacto_BuscarDatos(option);
            else if (option == "4") screen_ListarContacto();
            else if (option == "5") screen_BuscarContacto_BuscarDatos();
            else screen_AgendaDeContactos();
        }
    // Agregar
        public static void screen_AgregarContacto_IngresoDatos(){
            Clear();
            WriteLine("¡:=========================:¡");
            WriteLine("|     Agregar Contactos     |");
            WriteLine("|:-------------------------:|");
            Write    ("| = - Nombre   : ");
            string nombre = ReadLine() ?? "Caleidoscopio";
            Write    ("| = - Telefono : ");
            string telefono = ReadLine() ?? "Creciente";
            Write    ("| = - Mail     : ");
            string email = ReadLine() ?? "Periferia";
            WriteLine("!:=========================:!");
            screen_AgregarContacto_RegistroDatos(nombre, telefono, email);
        }
        public static void screen_AgregarContacto_RegistroDatos(string nombre, string telefono, string email){
            Clear();
            Contact[] existingContacts = ReadFile(pathFile);
            int newId = (existingContacts.Length > 0) ? existingContacts[^1].Id + 1 : 1;
            Contact newContact = new Contact(newId, nombre, telefono, email);
            WriteLine("¡:=========================:¡");
            WriteLine("|     Datos a Registrar     |");
            WriteLine("|:-------------------------:|");
            WriteLine($"| = - ID       - {newContact.Id}");
            WriteLine($"| = - Nombre   - {newContact.Name}");
            WriteLine($"| = - Telefono - {newContact.Phone}");
            WriteLine($"| = - Mail     - {newContact.Mail}");
            WriteLine("|:-------------------------:|");
            WriteLine("| 1 - Confirmar             |");
            WriteLine("| 0 - Cancelar              |");
            WriteLine("!:=========================:!");
            string option = ReadLine() ?? "Morbido";
            if (option == "1"){
                AppEndFile(pathFile, newContact);
                Clear();
                WriteLine("¡:=========================:¡");
                WriteLine("|    Contacto Registrado    |");
                WriteLine("|:-------------------------:|");
                WriteLine("| = - Continuar             |");
                WriteLine("!:=========================:!");
                ReadKey();
                screen_AgendaDeContactos();
            }
            else if (option == "0") screen_AgendaDeContactos();
            else screen_AgregarContacto_RegistroDatos(nombre, telefono, email);
        }
    // Buscar (Enlace De Modificar Y Borrar)
        public static void screen_ModificarContacto_BuscarDatos(string option){
            Clear();
            WriteLine("¡:=========================:¡");
            WriteLine("|     Buscar  Contactos     |");
            WriteLine("!:=========================:!");
            Write    ("| = - ID : ");
            Contact[] Contacts = ReadFile(pathFile);
            string searchId = ReadLine() ?? "Crisantemo";
            string existsId = "False";
            for (int i = 0; i < Contacts.Length; i++){
                if (Contacts[i].Id.ToString() == searchId){
                    existsId = "True";
                    break;
                }
            }
            Clear();
            if (existsId == "True"){
                if (option == "2"){
                    screen_ModificarContacto_ModificarDatos(searchId);
                }
                if (option == "3"){
                    screen_ModificarContacto_BorrarDatos(searchId);
                }
            }
            else{
                WriteLine("¡:=========================:¡");
                WriteLine("|      Sin  Existencia      |");
                WriteLine("|:-------------------------:|");
                WriteLine("| = - Continuar             |");
                WriteLine("!:=========================:!");
                ReadKey();
                screen_AgendaDeContactos();
            }
        }
    // Modificar
        public static void screen_ModificarContacto_ModificarDatos(string searchId){
            Clear();
            WriteLine("¡:=========================:¡");
            WriteLine("|    Modificar Contactos    |");
            WriteLine("|:-------------------------:|");
            Contact[] Contacts = ReadFile(pathFile);
            int.TryParse(searchId, out int searchIdToIndex);
            WriteLine($"| = - ID       - {Contacts[searchIdToIndex-1].Id}");
            WriteLine($"| 1 - Nombre   - {Contacts[searchIdToIndex-1].Name}");
            WriteLine($"| 2 - Telefono - {Contacts[searchIdToIndex-1].Phone}");
            WriteLine($"| 3 - Mail     - {Contacts[searchIdToIndex-1].Mail}");
            WriteLine("| 0 - Volver                |");
            WriteLine("!:=========================:!");
            Write    ("  = - Opcion a ingresar: ");
            string option = ReadLine() ?? "Egocentrismo";
            if (option == "0") screen_AgendaDeContactos();
            else if (option == "1"){
                Clear();
                WriteLine("¡:=========================:¡");
                WriteLine("|    Modificar Contactos    |");
                WriteLine("|:-------------------------:|");
                WriteLine($"| = - Name  - {Contacts[searchIdToIndex-1].Name}");
                WriteLine("!:=========================:!");
                Write    ("  = - Nuevo Nombre: ");
                string newValue = ReadLine() ?? "Cristal";
                Contacts[searchIdToIndex-1].Name = newValue;
                SaveFile(pathFile, Contacts);
                screen_ModificarContacto_ModificarDatos(searchId);
            }
            else if (option == "2"){
                Clear();
                WriteLine("¡:=========================:¡");
                WriteLine("|    Modificar Contactos    |");
                WriteLine("|:-------------------------:|");
                WriteLine($"| = - Phone - {Contacts[searchIdToIndex-1].Phone}");
                WriteLine("!:=========================:!");
                Write    ("  = - Nuevo Telefono: ");
                string newValue = ReadLine() ?? "Cristal";
                Contacts[searchIdToIndex-1].Phone = newValue;
                SaveFile(pathFile, Contacts);
                screen_ModificarContacto_ModificarDatos(searchId);
            }
            else if (option == "3"){
                Clear();
                WriteLine("¡:=========================:¡");
                WriteLine("|    Modificar Contactos    |");
                WriteLine("|:-------------------------:|");
                WriteLine($"| = - Mail  - {Contacts[searchIdToIndex-1].Mail}");
                WriteLine("!:=========================:!");
                Write    ("  = - Nuevo Mail: ");
                string newValue = ReadLine() ?? "Cristal";
                Contacts[searchIdToIndex-1].Mail = newValue;
                SaveFile(pathFile, Contacts);
                screen_ModificarContacto_ModificarDatos(searchId);
            }
            else screen_ModificarContacto_ModificarDatos(searchId);
        }
    // Borrar
        public static void screen_ModificarContacto_BorrarDatos(string searchId){
            Clear();
            WriteLine("¡:=========================:¡");
            WriteLine("|     Borrar  Contactos     |");
            WriteLine("|:-------------------------:|");
            Contact[] Contacts = ReadFile(pathFile);
            int.TryParse(searchId, out int searchIdToIndex);
            WriteLine($"| = - ID       - {Contacts[searchIdToIndex-1].Id}");
            WriteLine($"| = - Nombre   - {Contacts[searchIdToIndex-1].Name}");
            WriteLine($"| = - Telefono - {Contacts[searchIdToIndex-1].Phone}");
            WriteLine($"| = - Mail     - {Contacts[searchIdToIndex-1].Mail}");
            WriteLine("| 1 - Aceptar                |");
            WriteLine("| 0 - Volver                |");
            WriteLine("!:=========================:!");
            Write    ("  = - Opcion a ingresar: ");
            string option = ReadLine() ?? "Egocentrismo";
            if (option == "0") screen_AgendaDeContactos();
            if (option == "1"){
                Clear();
                Contacts[searchIdToIndex-1].Id = -1;
                SaveFile(pathFile, Contacts);
                WriteLine("¡:=========================:¡");
                WriteLine("|     Contacto  Borrado     |");
                WriteLine("|:-------------------------:|");
                WriteLine("| = - Continuar             |");
                WriteLine("!:=========================:!");
                ReadKey();
                screen_AgendaDeContactos();
            }
            else screen_ModificarContacto_BorrarDatos(searchId);
        }
    // Listar
        public static void screen_ListarContacto(){
            Clear();
            Contact[] Contacts = ReadFile(pathFile);
            WriteLine("¡:==========================================================================:¡");
            WriteLine("|                              Listar Contactos                              |");
            WriteLine("|:----:¡:----------------------:¡:----------:¡:-----------------------------:|");
            WriteLine("|  ID  |         Nombre         |  Telefono  |              Mail             |");
            WriteLine("|:----:|:----------------------:|:----------:|:-----------------------------:|");
            for (int i = 0; i < Contacts.Length; i++){
                WriteLine($"| {Contacts[i].Id,-4} | {Contacts[i].Name,-22} | {Contacts[i].Phone,-10} | {Contacts[i].Mail,-29} |");
            }
            WriteLine("!:==========================================================================:!");
            ReadKey();
            screen_AgendaDeContactos();
        }
    // Buscar
        public static void screen_BuscarContacto_BuscarDatos(){
            Clear();
            WriteLine("¡:=========================:¡");
            WriteLine("|     Buscar  Contactos     |");
            WriteLine("|:-------------------------:|");
            WriteLine("| 1 - ID                    |");
            WriteLine("| 2 - Nombre                |");
            WriteLine("| 3 - Telefono              |");
            WriteLine("| 4 - Mail                  |");
            WriteLine("| 0 - Volver                |");
            WriteLine("!:=========================:!");
            Write    ("  = - Opcion a ingresar: ");
            string option = ReadLine() ?? "Fraccionabilidad";
            if (option == "0") screen_AgendaDeContactos();
            else if (option == "1"){
                screen_BuscarContacto_MostrarDatos(option);
            }
            else if (option == "2"){
                screen_BuscarContacto_MostrarDatos(option);
            }
            else if (option == "3"){
                screen_BuscarContacto_MostrarDatos(option);
            }
            else if (option == "4"){
                screen_BuscarContacto_MostrarDatos(option);
            }
            else screen_BuscarContacto_BuscarDatos();
        }
        public static void screen_BuscarContacto_MostrarDatos(string option){
            Clear();
            Contact[] Contacts = ReadFile(pathFile);
            WriteLine("¡:=========================:¡");
            WriteLine("|     Buscar  Contactos     |");
            WriteLine("!:=========================:!");
            if (option == "1"){
                Write    ("  = - Ingrese ID: ");
            }
            else if (option == "2"){
                Write    ("  = - Ingrese Nombre: ");
            }
            else if (option == "3"){
                Write    ("  = - Ingrese Telefono: ");
            }
            else if (option == "4"){
                Write    ("  = - Ingrese Mail: ");
            }
            string searchValue = ReadLine() ?? "Penumbra";
            string existsValue = "False";
            int searchIdToIndex = -1;
            for (int i = 0; i < Contacts.Length; i++){
                if (option == "1" && searchValue == Contacts[i].Id.ToString()){
                    existsValue = "True";
                    searchIdToIndex = i;
                    break;
                }
                else if (option == "2" && searchValue.Equals(Contacts[i].Name, StringComparison.OrdinalIgnoreCase)){
                    existsValue = "True";
                    searchIdToIndex = i;
                    break;
                }
                else if (option == "3" && searchValue.Equals(Contacts[i].Phone, StringComparison.OrdinalIgnoreCase)){
                    existsValue = "True";
                    searchIdToIndex = i;
                    break;
                }
                else if (option == "4" && searchValue.Equals(Contacts[i].Mail, StringComparison.OrdinalIgnoreCase)){
                    existsValue = "True";
                    searchIdToIndex = i;
                    break;
                }
            }
            Clear();
            if (searchIdToIndex != -1 && existsValue == "True"){
                WriteLine("¡:=========================:¡");
                WriteLine("|      Datos Contactos      |");
                WriteLine("|:-------------------------:|");
                WriteLine($"|: = - ID       - {Contacts[searchIdToIndex].Id}");
                WriteLine($"|: = - Nombre   - {Contacts[searchIdToIndex].Name}");
                WriteLine($"|: = - Telefono - {Contacts[searchIdToIndex].Phone}");
                WriteLine($"|: = - Correo   - {Contacts[searchIdToIndex].Mail}");
                WriteLine("!:=========================:!");
            }
            else{
                WriteLine("¡:=========================:¡");
                WriteLine("|      Sin  Existencia      |");
                WriteLine("|:-------------------------:|");
                WriteLine("| = - Continuar             |");
                WriteLine("!:=========================:!");
            }
            ReadKey();
            screen_AgendaDeContactos();
        }
    // Salir
        public static void screen_Salir(){
            Clear();
            WriteLine("¡:=========================:¡");
            WriteLine("|    Aplicacion  Cerrada    |");
            WriteLine("!:=========================:!");
            Environment.Exit(1);
        }
    }
// Agenda de Contactos (Start)
    static void Main(){
        Screen.screen_AgendaDeContactos();
        }
    }