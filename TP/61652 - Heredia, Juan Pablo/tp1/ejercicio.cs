using System.Data.Common;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using static System.Console;

struct Contactos{
    public int Id=0;
    public static int id=1;
    public string Nombre,Telefono,Email;
    public static string[,] array_contactos = new string [4,300];
    public Contactos(int id,string nombre,string telefono,string email){
        Id=id;
        Nombre=nombre;
        Telefono=telefono;
        Email=email;
    }
    public static void Menu_agenda(){
        Clear();
        WriteLine("_______________________________");
        WriteLine("|      Agenda de contactos    |");
        WriteLine("|_____________________________|");
        WriteLine("|   1.    Agregar contacto    |");
        WriteLine("|   2.    Modificar contacto  |");
        WriteLine("|   3.    Borrar Contacto     |");
        WriteLine("|   4.    Lista de contactos  |");
        WriteLine("|   5.    Buscar contacto     |");
        WriteLine("|   6.         Salir          |");
        WriteLine("|_____________________________|");
        Write("Ingrese una opcion = ");
        string opcion= ReadLine()?? "";
        if (opcion=="0"){}
        else if(opcion =="1"){Contactos.Agregar_contacto();}
        else if(opcion =="2"){Contactos.Buscar_Modificar();}
        else if(opcion =="3"){Contactos.Borrar_Contacto();}
        else if(opcion =="4"){Contactos.Lista_Contactos();}
        else if(opcion =="5"){Contactos.Buscar_Contacto();}
        else if(opcion =="6"){Contactos.Cerrar();}
        else Menu_agenda();
    }
    public static void Agregar_contacto(){
        if(id>=300){
            WriteLine("___________________________________");
            WriteLine("|No se puede agregar mas contactos|");
            WriteLine("|Limite de 300 alcanzado          |");
            WriteLine("|_________________________________|");
            ReadKey();
            Menu_agenda();
            return;
        }
        Clear();
        WriteLine("________________________");
        WriteLine("|  Datos de contacto   |");
        WriteLine("|                      |");
        Write    ("| Ingrese Nombre: ");
        string nombre=ReadLine() ?? "";
        while(string.IsNullOrEmpty(nombre)){ 
            Clear();
            WriteLine("Nombre no valido ingresar de nuevo");
            Write    ("Ingrese Nombre: ");
            nombre=ReadLine() ?? "";
            }
        Write    ("| Ingrese Telefono: ");
        string telefono=ReadLine() ??"";
        while(string.IsNullOrEmpty(telefono) || !long.TryParse(telefono, out _)){
            Clear();
            WriteLine("Telefono no valido");
            Write    ("Ingrese Telefono: ");
            telefono=ReadLine() ?? "";
        }
        Write    ("|  Ingrese Email: ");
        string email=ReadLine() ?? "";
        while(string.IsNullOrEmpty(email)|| long.TryParse(email, out _)){
            Clear();
            WriteLine("Email no valido");
            Write (" Ingrese Email: ");
            email=ReadLine() ?? "";
        }
        WriteLine("|______________________|");
        Confirmar(nombre,telefono,email);
    }
    public static void Confirmar(string nombre,string telefono,string email){
        Clear();
        Contactos nuevocontacto= new Contactos(id,nombre,telefono,email);
        WriteLine("________________________");
        WriteLine("|  Confirmar Contacto  |");
        WriteLine("|______________________|");
        WriteLine($"| ID:  {nuevocontacto.Id}");
        WriteLine($"| Nombre: {nuevocontacto.Nombre}");
        WriteLine($"| Telefono: {nuevocontacto.Telefono}");
        WriteLine($"| Email: {nuevocontacto.Email}");
        WriteLine("|_______________________");
        WriteLine("|      Confirmar = S   |");
        WriteLine("|       Cancelar = N   |");
        WriteLine("|______________________|");
        Write("Ingrese una opcion S/N:  ");
        string opcion=ReadLine() ?? "";
        if (opcion.ToLower() =="s"){ 
            array_contactos[0,id] = nuevocontacto.Id.ToString();
            array_contactos[1,id] = nuevocontacto.Nombre;
            array_contactos[2,id] = nuevocontacto.Telefono;
            array_contactos[3,id] = nuevocontacto.Email;
            id++;
            Contactos.Guardarcontactos();
            Clear();
            WriteLine("_______________________");
            WriteLine("|  Contacto Guardado  |");
            WriteLine("|_____________________|");
            ReadKey();
            Contactos.Menu_agenda();
        }
        else if(opcion.ToLower()=="n"){
            Clear();
            Contactos.Menu_agenda();
        }
        else{Confirmar(nombre,telefono,email);}
    }
    public static void Buscar_Modificar(){
        Clear();
        WriteLine("___________________________");
        WriteLine("|   Contacto a modificar  |");
        WriteLine("| 1.Buscar por ID         |");
        WriteLine("| 2.Cancelar              |");
        WriteLine("|_________________________|");
        Write("Ingrese una opcion: ");
        string opcion=ReadLine() ?? "";
        string IDencontrado= "no";
        int indicador=0;
        if (opcion == "1"){
            WriteLine("_______________");
            Write    ("|Ingresar ID:  ");
            string buscarid=ReadLine() ?? "";
            for (int i=0; i<=id; i++){
                if(!string.IsNullOrWhiteSpace(array_contactos[0,i])){
                    int.TryParse(buscarid,out int ID_N);
                    if (int.TryParse(array_contactos[0,i],out int ID_Ar)&& ID_Ar== ID_N){
                        IDencontrado="si"; indicador=i;
                    }
                }
            }
            Clear();
            if(IDencontrado=="si"){Contacto_A_Modificar(IDencontrado,indicador);}
            else if(IDencontrado=="no"){
                WriteLine("________________________");
                WriteLine("|Contacto no encontrado|");
                WriteLine("|______________________|");
                ReadKey();
                Menu_agenda();}
        }
        else if(opcion=="2") {Contactos.Menu_agenda();}
        else{Buscar_Modificar();}
    }
    public static void Lista_Contactos(){
        Clear();
        for (int i=0;i<id;i++){
            WriteLine($"ID:{array_contactos[0,i]}|Nombre:{array_contactos[1,i]}|Telefono:{array_contactos[2,i]}|Email:{array_contactos[3,i]}");
            WriteLine($"______________________________________________________________________");
        }
        ReadKey();
        Menu_agenda();

    }
    public static void Guardarcontactos(){
        string archivocontactos="agenda.csv";
        string lineas= "ID,Nombre,Telefono,Email\n";
        for (int i=0;i<id;i++){
            if(!string.IsNullOrEmpty(array_contactos[0,i])&& !string.IsNullOrEmpty(array_contactos[1,i])&& !string.IsNullOrEmpty(array_contactos[2,i])&& !string.IsNullOrEmpty(array_contactos[3,i])){
            lineas +=$"{array_contactos[0,i]},{array_contactos[1,i]},{array_contactos[2,i]},{array_contactos[3,i]}\n";}
        }
        File.WriteAllText(archivocontactos,lineas);
    }
    public static void Abrir_Archivo_contacto(){
        string archivocontactos="agenda.csv";
        if (File.Exists(archivocontactos)){
            string perfiles= File.ReadAllText(archivocontactos);
            string[] lineas=perfiles.Split(new[] {"\n"},StringSplitOptions.RemoveEmptyEntries);
            id=1;
            for(int i=1;i<lineas.Length;i++){
                string[] perfil=lineas[i].Split(",");
                if (perfil.Length==4 && !string.IsNullOrWhiteSpace(perfil[0])&& !string.IsNullOrWhiteSpace(perfil[1])&& !string.IsNullOrWhiteSpace(perfil[2])&& !string.IsNullOrWhiteSpace(perfil[3])){
                    array_contactos[0,id]=perfil[0];
                    array_contactos[1,id]=perfil[1];
                    array_contactos[2,id]=perfil[2];
                    array_contactos[3,id]=perfil[3];
                    id++;
                }
            }
        }
        else {
        archivocontactos="agenda.csv";
        string lineas= "ID,Nombre,Telefono,Email\n";
        File.WriteAllText(archivocontactos,lineas);
        }
    }
        public static void Contacto_A_Modificar(string IDencontrado,int indicador){
        Clear();
        WriteLine("_________________________");
        WriteLine($"-. ID: {array_contactos[0,indicador]}");
        WriteLine($"1. Nombre: {array_contactos[1,indicador]}");
        WriteLine($"2. Telefono: {array_contactos[2,indicador]}");
        WriteLine($"3. Email: {array_contactos[3,indicador]}");
        WriteLine("4. Volver               ");
        WriteLine("_________________________");
        Write(" Ingrese una opcion a modificar: ");
        string opcion=ReadLine() ?? "";
        if (opcion=="1"){
            Write($"Ingrese nuevo Nombre: ");
            string NuevoNombre=ReadLine()??"";
            while(string.IsNullOrEmpty(NuevoNombre)){
            Write($"Ingrese nuevo Nombre Valido: ");
            NuevoNombre=ReadLine()??"";
            }
            array_contactos[1,indicador]=NuevoNombre;
            Guardarcontactos();
            Write("Nombre modificado correctamente");
            ReadKey();
            Menu_agenda();
            }
        else if(opcion=="2"){
            Write("Ingrese nuevo telefono: ");
            string NuevoTelefono=ReadLine()??"";
            while(string.IsNullOrEmpty(NuevoTelefono) || !long.TryParse(NuevoTelefono, out _)){
                Write("Ingrese nuevo Telefono Valido: ");
                NuevoTelefono=ReadLine()??"";
            }
            array_contactos[2,indicador]=NuevoTelefono;
            Guardarcontactos();
            Write("Telefono modificado correctamente");
            ReadKey();
            Menu_agenda();
        }
        else if(opcion=="3"){
            Write("Ingrese nuevo Email: ");
            string NuevoEmail=ReadLine()??"";
            while(string.IsNullOrEmpty(NuevoEmail) || long.TryParse(NuevoEmail, out _)){
                Write("Ingrese nuevo Email Valido: ");
                NuevoEmail=ReadLine()??"";
            }
            array_contactos[3,indicador]=NuevoEmail;
            Guardarcontactos();
            Write("Email modificado correctamente");
            ReadKey();
            Menu_agenda();
        }
        else if(opcion=="4"){Contactos.Menu_agenda();}
        else{Contactos.Contacto_A_Modificar(IDencontrado,indicador);}
    }
    public static void Buscar_Contacto(){
        Clear();
        WriteLine("___________________________");
        WriteLine("|   Ingrese ID o nombre   |");
        WriteLine("| 1.Buscar                |");
        WriteLine("| 2.Cancelar              |");
        WriteLine("|_________________________|");
        Write("Ingrese una opcion: ");
        string opcion=ReadLine() ?? "";
        string encontrado= "no";
        int indicador=0;
        if (opcion == "1"){
            WriteLine("___________________________");
            Write    ("|Ingresar ID o Nombre:  ");
            string ID_Nombre=ReadLine() ?? "";
            for (int i=0; i<id; i++){
                if(!string.IsNullOrWhiteSpace(array_contactos[0,i])&& !string.IsNullOrWhiteSpace(array_contactos[1,i])){
                    if (int.TryParse(ID_Nombre,out int ID_N)){
                        if(int.TryParse(array_contactos[0,i], out int ID_Ar)&& ID_Ar== ID_N){
                        encontrado="si";
                        indicador=i;
                        break;}
                    }
                    else if (array_contactos[1,i].ToLower()==ID_Nombre.ToLower()){
                        encontrado="si";
                        indicador=i;
                        break;}
                }
            }
            if(encontrado=="si"){
            Clear();
            WriteLine($"___________________________");
            WriteLine($"|         Contacto         ");
            WriteLine($"| ID: {array_contactos[0,indicador]}");
            WriteLine($"| Nombre: {array_contactos[1,indicador]}");
            WriteLine($"| Telefono: {array_contactos[2,indicador]}");
            WriteLine($"| Email: {array_contactos[3,indicador]}");
            WriteLine($"|__________________________");
            }
            if(encontrado=="no") {
                WriteLine("________________________");
                WriteLine("|Contacto no encontrado|");
                WriteLine("|______________________|");
            }
            Write("Enter para volver al Menu");
            ReadKey();
            Menu_agenda();
        }
        else if(opcion=="2") {Contactos.Menu_agenda();}
        else{Buscar_Contacto();}
    }
    public static void Borrar_Contacto(){
        Clear();
        WriteLine("____________________________________");
        WriteLine("| Ingresar ID de contacto a borrar |");
        WriteLine("|__________________________________|");
        Write(": ");
        string ID_Nombre=ReadLine()??"";
        string encontrado="no";
        int indicador=-1;
        for(int i=0; i<id;i++){
            if(!string.IsNullOrWhiteSpace(array_contactos[0,i]) && !string.IsNullOrWhiteSpace(array_contactos[1,i])){
                if(int.TryParse(ID_Nombre,out int ID_N)){
                    if (int.TryParse(array_contactos[0,i], out int ID_Ar)&& ID_Ar == ID_N){
                    encontrado="si";
                    indicador=i; 
                    break;
                    }
                }
            }
        }
        if(encontrado=="si"){
            Clear();
            WriteLine($"_______________________");
            WriteLine($"|  Confirmar Contacto  ");
            WriteLine($"| ID: {array_contactos[0,indicador]}");
            WriteLine($"| Nombre: {array_contactos[1,indicador]}");
            WriteLine($"| Telefono: {array_contactos[2,indicador]}");
            WriteLine($"| Email: {array_contactos[3,indicador]}");
            WriteLine($"|______________________");
            WriteLine($"| 1. Cofirmar         |");
            WriteLine($"| 2. Cancelar         |");
            WriteLine($"|_____________________|");
            Write("Ingrese una opcion: ");
            string opcion=ReadLine()??"";
            if (opcion=="1"){
                Clear();
                for (int i=indicador; i<id-1;i++){
                    array_contactos[0,i]=array_contactos[0,i+1];
                    array_contactos[1,i]=array_contactos[1,i+1];
                    array_contactos[2,i]=array_contactos[2,i+1];
                    array_contactos[3,i]=array_contactos[3,i+1];
                }
                array_contactos[0,id-1]="";
                array_contactos[1,id-1]="";
                array_contactos[2,id-1]="";
                array_contactos[3,id-1]="";
                id--;
                for (int i=0;i<id;i++){
                    array_contactos[0,i+1]=(i+1).ToString();
                }
                Guardarcontactos();
                Abrir_Archivo_contacto();
                WriteLine("______________________");
                WriteLine("| Contacto Eliminado |");
                WriteLine("|____________________|");
            }
            else if(opcion=="2"){Menu_agenda();}
        }
        else if (encontrado=="no"){
            WriteLine("________________________");
            WriteLine("|Contacto no encontrado|");
            WriteLine("|______________________|");
        }
        Write("Precionar enter para volver al Menu");
        ReadKey();
        Menu_agenda();
    }
    public static void Cerrar(){
        Clear();
        WriteLine("____________________");
        WriteLine("|                  |");
        WriteLine("|Cerrando Agenda...|");
        WriteLine("|__________________|");
        Guardarcontactos();
        Environment.Exit(2);
    }
}
public class Inicio{
    public static void Main(){
        Contactos.Abrir_Archivo_contacto();
        Contactos.Menu_agenda();
    }
}