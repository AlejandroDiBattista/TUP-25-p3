using static System.Console;
using System.Collections.Generic;
using System.Globalization;

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
        return CuentasRegistradas.ContainsKey(numero) ? CuentasRegistradas[numero] : null;
    }
    public static void Registrar(Cuenta cuenta){
        if (!CuentasRegistradas.ContainsKey(cuenta.Numero)){
            CuentasRegistradas.Add(cuenta.Numero, cuenta);
        }
    }
    public void Registrar(Operacion operacion){
        Operaciones.Add(operacion);
        if (operacion.Ejecutar()){
            operacion.Origen.Historial.Add(operacion.Descripcion());
            if (operacion is Transferencia t && t.Destino != null){
                t.Destino.Historial.Add(operacion.Descripcion());
            }
            operacion.Origen.Puntaje(operacion.Monto);
        }else{
            WriteLine("Operacion fallida.");
        }
    }
    public void Informe(){
        WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}");
        foreach(var cliente in Clientes){
            decimal saldoTotal = 0;
            decimal puntosTotal = 0;
            foreach(var cuenta in cliente.Cuentas){
                saldoTotal += cuenta.Saldo;
                puntosTotal += cuenta.Puntos;
            }
            WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: $ {saldoTotal:N2} | Puntos Total: $ {puntosTotal:N2}");
            foreach(var cuenta in cliente.Cuentas){
                WriteLine($"\n    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:N2} | Puntos: $ {cuenta.Puntos:N2}");
                foreach(var linea in cuenta.Historial){
                    WriteLine("     -  " + linea);
                }
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
        cuenta.Duenio = this;
        Cuentas.Add(cuenta);
        Banco.Registrar(cuenta);
    }
}

abstract class Cuenta{
    public string Numero;
    public decimal Saldo;
    public decimal Puntos = 0;
    public List<string> Historial = new List<string>();
    public Cliente Duenio;
    public Cuenta(string numero, decimal saldo){
        Numero = numero;
        Saldo = saldo;
    }
    public bool Depositar(decimal cantidad){
        if (cantidad <=0) return false;
        Saldo += cantidad;
        return true;
    }
    public bool Extraer(decimal cantidad){
        if (cantidad <= 0 || cantidad > Saldo) return false;
        Saldo -= cantidad;
        return true;
    }
    public abstract void Puntaje(decimal monto);
}

class CuentaOro: Cuenta{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo){}
    public override void Puntaje(decimal monto){
        Puntos += monto >= 1000 ? monto * 0.05m : monto * 0.03m;
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
        return Origen?.Depositar(Monto) ?? false;
    }
    public override string Descripcion(){
        return $"Deposito $ {Monto:N2} a [{Origen?.Numero}/{Origen?.Duenio?.Nombre}]";
    }
}
class Retiro: Operacion{
    public Retiro(string numero, decimal monto) : base(numero, monto){}
    public override bool Ejecutar(){
        return Origen?.Extraer(Monto) ?? false;
    }
    public override string Descripcion(){
        return $"Retiro $ {Monto:N2} de [{Origen?.Numero}/{Origen?.Duenio?.Nombre}]";
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
        return $"Transferencia $ {Monto:N2} de [{Origen?.Numero}/{Origen?.Duenio?.Nombre}] a [{Destino?.Numero}/{Destino?.Duenio?.Nombre}]";
    }
}
class Pago: Operacion{
    public Pago(string numero, decimal monto) : base(numero, monto){}
    public override bool Ejecutar(){
        return Origen?.Extraer(Monto) ?? false;
    }
    public override string Descripcion(){
        return $"Pago $ {Monto:N2} con [{Origen?.Numero}/{Origen?.Duenio?.Nombre}]";
    }
}


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

nac.Registrar(new Deposito("10001", 100));
nac.Registrar(new Retiro("10002", 200));
nac.Registrar(new Transferencia("10001", "10002", 300));
nac.Registrar(new Transferencia("10003", "10004", 500));
nac.Registrar(new Pago("10002", 400));

tup.Registrar(new Deposito("10005", 100));
tup.Registrar(new Retiro("10005", 200));
tup.Registrar(new Transferencia("10005", "10002", 300));
tup.Registrar(new Pago("10005", 400));

nac.Informe();
tup.Informe();
    

// codigo que solucione...