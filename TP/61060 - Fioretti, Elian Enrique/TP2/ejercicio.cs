using static System.Console;
using System.Collections.Generic;

class Banco{
    public string Nombre;
    public List<Cliente> Clientes;
    public List<Operacion> Operaciones;
    public static Dictionary<string, Cuenta> CuentasRegistradas = new Dictionary<string, Cuenta>();
    public Banco(string nombre){
        Nombre = nombre;
        Clientes = new List<Cliente>();
        Operaciones = new List<Operacion>();
    }
    public void Agregar(Cliente cliente){
        Clientes.Add(cliente);
    }
    public static Cuenta Buscar(string numero){
        if (CuentasRegistradas.ContainsKey(numero)){
            return CuentasRegistradas[numero];
        }
        else{
            return null;
        }
    }
    public static void Registrar(Cuenta cuenta){
        if (CuentasRegistradas.ContainsKey(cuenta.Numero)){
            WriteLine($"La cuenta numero {cuenta.Numero} ya se encuentra registrada.");
        }
        else{
            CuentasRegistradas.Add(cuenta.Numero, cuenta);
        }
    }
    public void Registrar(Operacion operacion){
        Operaciones.Add(operacion);
        if (!operacion.Ejecutar()){
            WriteLine("Operacion fallida.");
        }
    }
    public void Informe(){
        WriteLine($"Informe del banco {Nombre}.");
        foreach(var cliente in Clientes){
            WriteLine($"Cliente: {cliente.Nombre}.");
            foreach(var cuenta in cliente.Cuentas){
                WriteLine($"    - Cuenta {cuenta.Numero}: Saldo {cuenta.Saldo}.");
            }
        }
    }
}
class Cliente{
    public string Nombre;
    public List<Cuenta> Cuentas;
    public Cliente(string nombre){
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }
    public void Agregar(Cuenta cuenta){
        Cuentas.Add(cuenta);
        Banco.Registrar(cuenta);
    }
}

abstract class Cuenta{
    public string Numero;
    public decimal Saldo;
    public decimal Puntos = 0;
    public Cuenta(string numero, decimal saldo){
        Numero = numero;
        Saldo = saldo;
    }
    public bool Depositar(decimal cantidad){
        if (cantidad <=0) return false;
        else{
            Saldo += cantidad;
            return true;
        }
    }
    public bool Extraer(decimal cantidad){
        if (cantidad <= 0 || cantidad > Saldo) return false;
        else{
            Saldo -= cantidad;
            return true;
        }
    }
    public abstract void Puntaje(decimal monto);
}
class CuentaOro: Cuenta{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo){}
    public override void Puntaje(decimal monto){
        if (monto >= 1000) Puntos += 0.05m;
        else Puntos += 0.03m;
    }
}
class CuentaPlata: Cuenta{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo){}
    public override void Puntaje(decimal monto){
        Puntos += monto * 0.02m;
    }
}
class CuentaBronce: Cuenta{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo){}
    public override void Puntaje(decimal monto){
        Puntos += monto * 0.01m;
    }
}

abstract class Operacion{
    public Cuenta Origen;
    public decimal Monto;
    public Operacion(string numero, decimal monto){
        Origen = Banco.Buscar(numero);
        Monto = monto;
    }
    public abstract bool Ejecutar();
    public abstract string Descripcion();
}
class Deposito: Operacion{
    public Deposito(string numero, decimal monto) : base(numero, monto){}
    public override bool Ejecutar(){
        return Origen.Depositar(Monto);
    }
    public override string Descripcion(){
        return $"Deposito de monto {Monto} a cuenta {Origen?.Numero}.";
    }
}
class Retiro: Operacion{
    public Retiro(string numero, decimal monto) : base(numero, monto){}
    public override bool Ejecutar(){
        return Origen.Extraer(Monto);
    }
    public override string Descripcion(){
        return $"Extraccion de monto {Monto} desde cuenta {Origen?.Numero}.";
    }
}
class Transferencia: Operacion{
    public Cuenta Destino;
    public Transferencia(string origen, string destino, decimal monto) : base(origen, monto){
        Destino = Banco.Buscar(destino);
    }
    public override bool Ejecutar(){
        if (Origen == null || Destino == null) return false;
        if (!Origen.Extraer(Monto)) return false;
        if (!Destino.Depositar(Monto)){
            Origen.Depositar(Monto);
            return false;
        }
        return true;
    }
    public override string Descripcion(){
        return $"Transferencia de {Monto} hecha desde {Origen?.Numero} hacia {Destino?.Numero}.";
    }
}
class Pago: Operacion{
    public Pago(string numero, decimal monto) : base(numero, monto){}
    public override bool Ejecutar(){
        return Origen.Extraer(Monto);
    }
    public override string Descripcion(){
        return $"Pago de {Monto} hecho desde {Origen?.Numero}.";
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