// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.
using System.Collections.Generic;
using static System.Console;
class Banco{
    public string Nombre;
    public List<Cliente> Clientes;
    public List<Operacion> Operaciones;
    public static Dictionary<string, Cuenta> CuentasR= new Dictionary<string, Cuenta>();
    public Banco(string nombre){
        Nombre=nombre;
        Clientes= new List<Cliente>();
        Operaciones= new List<Operacion>();
    }
    public void Agregar(Cliente cliente){
    Clientes.Add(cliente);
    }
    public static Cuenta Existe(string numero){
        if(CuentasR.ContainsKey(numero)){
            return CuentasR[numero];}
        else { return null;}
    }
    public static void Registrar(Cuenta cuenta){
        if(CuentasR.ContainsKey(cuenta.Numero)){
            WriteLine($"La cuenta {cuenta.Numero} ya existe");
        }
        else{
            CuentasR.Add(cuenta.Numero,cuenta);
        }
    }
    public void Registrar(Operacion operacion){
        Operaciones.Add(operacion);
        if(!operacion.iniciar()){
            WriteLine("Fallo la Operacion");
        }
    }
    public void Informe(){
        WriteLine($"Informe del banco{Nombre}.");
        foreach(var cliente in Clientes){
            WriteLine($"Cliente: {cliente.Nombre}");
            foreach(var cuenta in cliente.Cuentas){
                WriteLine($" Cuenta {cuenta.Numero}: Saldo {cuenta.Saldo}");
            }
        }
    }
}
class Cliente{
    public string Nombre;
    public List<Cuenta> Cuentas;
    public Cliente(string nombre){
        Nombre=nombre;
        Cuentas=new List<Cuenta>();
    }
    public void Agregar(Cuenta cuenta){
        Cuentas.Add(cuenta);
        Banco.Registrar(cuenta);
    }
}
abstract class Cuenta{
    public string Numero;
    public decimal Saldo;
    public decimal Puntos=0;
    public Cuenta(string numero,decimal saldo){
        Numero=numero;
        Saldo=saldo;
    }
    public bool Depositar(decimal Dinero){
        if(Dinero<=0){return false;}
        else{
        Saldo+=Dinero;
        return true;
        }
    }
    public bool Extraccion(decimal Dinero){
        if(Dinero<=0|| Dinero>Saldo){return false;}
        else{
            Saldo-=Dinero;
            return true;
        }
    }
    public abstract void SumaPuntos(decimal monto);
}
class CuentaOro: Cuenta{
    public CuentaOro(string numero,decimal saldo): base(numero,saldo){}
    public override void SumaPuntos(decimal monto){
        if (monto >=750){Puntos += 0.09m;}
        else Puntos+=0.05m;
    }
}
class CuentaPlata: Cuenta{
    public CuentaPlata(string numero,decimal saldo): base(numero,saldo){}
    public override void SumaPuntos(decimal monto){
    Puntos+= monto*0.03m;
    }
}
class CuentaBronce: Cuenta{
    public CuentaBronce(string numero,decimal saldo): base(numero,saldo){}
    public override void SumaPuntos(decimal monto){
    Puntos+= monto*0.012m;
    }
}

abstract class Operacion{
    public Cuenta NumeroCuenta;
    public decimal Monto;
    public Operacion(string numero,decimal monto){
        NumeroCuenta=Banco.Existe(numero);
        Monto=monto;
    }
    public abstract bool iniciar();
    public abstract string MostrarOperacion();
}
class Deposito: Operacion{
    public Deposito(string numero, decimal monto): base(numero,monto){}
    public override bool iniciar(){
        return NumeroCuenta.Depositar(Monto);
    }
    public override string MostrarOperacion(){
        return $"Se a depositado el monto de {Monto} a la cuenta {NumeroCuenta?.Numero}.";
    }
}
class Retiro: Operacion{
    public Retiro(string numero, decimal monto): base(numero,monto){}
    public override bool iniciar(){
        return NumeroCuenta.Extraccion(Monto);
    }
    public override string MostrarOperacion(){
        return $"Se a Extraido el monto de {Monto} desde la cuenta {NumeroCuenta?.Numero}.";
    }
}
class Transferencia: Operacion{
    public Cuenta Referencia;
    public Transferencia(string numeroCuenta,string referencia, decimal monto): base(numeroCuenta,monto){
        Referencia=Banco.Existe(referencia);
    }
    public override bool iniciar(){
        if (NumeroCuenta == null|| Referencia==null){return false;}
        if(!NumeroCuenta.Extraccion(Monto)){return false;}
        if(!Referencia.Depositar(Monto)){
            NumeroCuenta.Depositar(Monto);
            return false;
        }
        return true;
    }
    public override string MostrarOperacion(){
        return $"Transferencia de {Monto} hecho desde la cuenta {NumeroCuenta?.Numero}.";
    }
}
class Pago: Operacion{
    public Pago(string numero, decimal monto): base(numero,monto){}
    public override bool iniciar(){
        return NumeroCuenta.Extraccion(Monto);
    }
    public override string MostrarOperacion(){
        return $"Pago realizado de {Monto} desde la cuenta {NumeroCuenta?.Numero}.";
    }
}


/// EJEMPLO DE USO ///

// Definiciones 

var raul = new Cliente("Raul Perez");
    raul.Agregar(new CuentaOro("10001", 1000));
    raul.Agregar(new CuentaPlata("10002", 2000));

var sara = new Cliente("Sara Lopez");
    sara.Agregar(new CuentaPlata("10003", 3000));
    sara.Agregar(new CuentaPlata("10004", 4000));

var luis = new Cliente("Luis Gomez");
    luis.Agregar(new CuentaBronce("10005", 5000));

var nac = new Banco("Banco Nac");
nac.Agregar(raul);
nac.Agregar(sara);

var tup = new Banco("Banco TUP");
tup.Agregar(luis);


// Registrar Operaciones
nac.Registrar(new Deposito("10001", 100));
nac.Registrar(new Retiro("10002", 200));
nac.Registrar(new Transferencia("10001", "10002", 300));
nac.Registrar(new Transferencia("10003", "10004", 500));
nac.Registrar(new Pago("10002", 400));

tup.Registrar(new Deposito("10005", 100));
tup.Registrar(new Retiro("10005", 200));
tup.Registrar(new Transferencia("10005", "10002", 300));
tup.Registrar(new Pago("10005", 400));


// Informe final
nac.Informe();
tup.Informe();

