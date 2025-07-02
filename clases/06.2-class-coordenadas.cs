using static System.Console;

/* Clase para almacenar datos */

// La clase Coordenada es un contenedor de datos
// y métodos relacionados con coordenadas geográficas

class Coordenada{
    public double Lon {get; private set;}
    public double Lat {get; private set;}

    // Constructor base 
    public Coordenada(double lon, double lat){
        Lon = lon;
        Lat = lat;
    }

    // Constructor vacio (reusa el constructor base)
    public Coordenada(): this(0, 0){} // Constructor por defecto

    // Constructor con un string
    public Coordenada(string lugar)
    {
        if (lugar == "Tucuman"){
            Lon = -65.2167;
            Lat = -26.8333;
        } else if (lugar == "Santiago") {
            Lon = -64.2618;
            Lat = -27.782;
        } else {
            Lon = 0;
            Lat = 0;
        }
    }

    // Sobrecarga de constructures: Coordenada(); Coordenada(double, double); Coordenada(string)
   
    // Generadores de coordenadas
    public static Coordenada Santiago(){
        return new Coordenada(-64.2618, -27.782);
    }

    public static Coordenada Tucuman(){
        return new Coordenada(-65.2167, -26.8333);
    }

    public void Mover(double deltaLon, double deltaLat){
        Lon += deltaLon;
        Lat += deltaLat;    
    }

    // Sobrecarga de operadores Mostrar() y Mostrar(int)
    public void Mostrar(){
        WriteLine($"Coordenada: {this}");
    }

    public void Mostrar(int veces){
        for(var i =0; i<veces; i++){
            Mostrar();
        }
    }

    public override string ToString(){
        return $"({Lon}, {Lat})";
    }
}

var ctr = new Coordenada();
var tuc = Coordenada.Tucuman();
var sgo = new Coordenada("Santiago");

Clear();
ctr.Mostrar();
tuc.Mostrar();
sgo.Mostrar(3);


// Ejemplo de ToString() implicito
var x = "Estoy en " + tuc;
WriteLine($"Estoy en {tuc}");
