public class Cuenta{
    public string Numero { get; set; }
    public decimal Saldo { get; set; }
    public decimal Puntos { get; set; } = 0m;

    public Cuenta(string numero, decimal saldo=0){
        Numero = numero;
        Saldo = saldo < 0 ? 0 : saldo;
    }
    
    public override string ToString(){
        return $"Cuenta({Numero}, {Saldo:C0})";
    }

    public void Poner(decimal cantidad){
        if(cantidad <=0 ) return;
        Saldo += cantidad;
    }
    
    public void Sacar(decimal cantidad){
        if( cantidad < 0 || cantidad > Saldo) return;
        Saldo -= cantidad;
    }

    public virtual void AcumularPuntos(decimal cantidad){}
}

// Gracias a que 'saldo' tiene un valor por defecto la puedo llamar sin saldo.
Cuenta a = new Cuenta("10002");
WriteLine($"Cuenta : {a}");

Cuenta b = new Cuenta("20001", -2000); // aunque podria pasarle (en este caso lo rechaza por < 0)
WriteLine(b);

Cuenta c = new Cuenta("30000", saldo: 5000); // O nombrar los parametros para mayor claridad.
WriteLine(c);



// Se podria implementar una cuenta en la que se indique el porcentaje de puntos a acumular
// ojo... esta es una implementacion parcial ya que no contempla las cuenta Oro en donde el porcentaje depende de la cantidad
// del monto de la operacion.
// La pongo como muestra de una cuenta configurable.

public class CuentaPorcentaje : Cuenta {
    public decimal Porcentaje { get; set; }; // Con este valor podemos controlar el porcentaje.

    public CuentaPorcentaje(string numero, decimal saldo=0, decimal porcentaje = 0.0m) : base(numero, saldo){
        Porcentaje = porcentaje;
    }
    
    public override string ToString(){
        return $"Cuenta({Numero}, {Saldo:C0}, {Puntos:C0})";
    }

    public void Pagar(decimal cantidad){
        Sacar(cantidad);
        AcumularPuntos(cantidad);
    }

    public override void AcumularPuntos(decimal cantidad){
        Puntos += cantidad * Porcentaje;
    }

}

// La implementacion es sencilla pero no contempla el caso de las cuentas Oro en donde el porcentaje depende de la cantidad
// Oro: 3% , Plata: 2%, Bronce: 1%


a = new CuentaPorcentaje("10002", porcentaje: 0.03m); // Para las cuentas Oro
WriteLine(a);

b = new CuentaPorcentaje("20001", 2000, porcentaje: 0.02m); // Para las cuentas Plata
WriteLine(b);

c = new CuentaPorcentaje("30000", saldo: 5000, porcentaje: 0.01m); // Para las cuentas Bronce
WriteLine(c);

// Otra implementacion alternativas es pasar un 'Tipo' que controle como se acumula los puntos.
// En este caso el tipo de cuenta es un string...

public class CuentaConTipo : Cuenta{
    public string Tipo { get; set; } = "Oro";

    public CuentaConTipo(string numero, decimal saldo, string tipo) : base(numero, saldo){
        Tipo = tipo;
    }
    
    public override string ToString(){
        return $"Cuenta({Numero}, {Saldo:C0}, {Tipo}, {Puntos:C0})";
    }

    public override void AcumularPuntos(decimal cantidad){
        decimal tasa = 0.0m; // Initialize tasa
        switch (Tipo)
        {
            case "Oro":
                if (cantidad > 1000)
                    tasa = 0.05m;
                else
                    tasa = 0.03m;
                break;
            case "Plata":
                tasa = 0.02m;
                break;
            case "Bronce":
                tasa = 0.01m;
                break;
            default:
                tasa = 0.0m;
                break;
        }
        Puntos += cantidad * tasa;

        // Alternativa con expresion switch e if como expresion
        // Puntos += cantidad * (Tipo switch {
        //     "Oro"    => cantidad > 1000 ? 0.05m : 0.03m,
        //     "Plata"  => 0.02m,
        //     "Bronce" => 0.01m,
        //     _        => 0.0m
        // });
    }
}

Clear();
WriteLine("Sistema de Millaje (Puntos YPF)");
WriteLine("=====================================");
var cuentaOro = new CuentaConTipo("10002", 1000, "Oro");
WriteLine(cuentaOro);

var cuentaPlata = new CuentaConTipo("20001", 2000, "Plata");
WriteLine(cuentaPlata);

var cuentaBronce = new CuentaConTipo("30000", 3000, "Bronce");
WriteLine(cuentaBronce);

// Esta implementacion funciona... pero tiene dos incovenientes.
// 1. Puede ser compleja ya que la funcion AcumularPuntos puede ser muy grande.
// 2. No es extensible ya que si quiero agregar un nuevo tipo de cuenta tengo que modificar la funcion AcumularPuntos

// IMPLEMENTACION CON HERENCIA Y POLIMORFISMO

class CuentaOro : Cuenta{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo){}
    
    public override void AcumularPuntos(decimal cantidad){
        decimal tasa = cantidad > 1000 ? 0.05m : 0.03m;
        Puntos += cantidad * tasa;
    }
}
class CuentaPlata : Cuenta{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo){}
    
    public override void AcumularPuntos(decimal cantidad){
        Puntos += cantidad * 0.02m;
    }
}
class CuentaBronce : Cuenta{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo){}
    
    public override void AcumularPuntos(decimal cantidad){
        Puntos += cantidad * 0.01m;
    }
}

// Ahora podemos crear una cuenta de cada tipo y no tenemos que modificar la funcion AcumularPuntos
var cuentaOro = new CuentaOro("10002", 1000);
WriteLine(cuentaOro);
var cuentaPlata = new CuentaPlata("20001", 2000);
WriteLine(cuentaPlata);
var cuentaBronce = new CuentaBronce("30000", 3000);
WriteLine(cuentaBronce);

// Podrieamos agregar un nuevo tipo de cuenta sin modificar 
class CuentaPlatinum : Cuenta{
    public CuentaPlatinum(string numero, decimal saldo) : base(numero, saldo){}
    
    public override void AcumularPuntos(decimal cantidad){
        Puntos += cantidad * 0.10m;
    }
}
var cuentaPlatinum = new CuentaPlatinum("40000", 4000);
WriteLine(cuentaPlatinum);

// Esto tiene dos ventajas:
// 1. La funcionalidad de AcumularPuntos es mas simple y clara.
// 2. Si quiero agregar un nuevo tipo de cuenta no tengo que modificar la clase Cuenta.

