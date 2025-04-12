// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

using System;
using System.Collections.Generic;
using System.Linq;

class Banco {
    public List<Cliente> Clientes {get; private set;} = new List<Cliente>();
    public List<Operacion> HistorialGlobal {get; private set;}  = new List<Operacion>();

    public void AgregarCliente(Cliente cliente) => Clientes.Add(cliente);
    public void RegistrarOperacion(Operacion operacion) => HistorialGlobal.Add(operacion);

    public void MostrarReporte() {
        Console.WriteLine("\n=== Historial Global de Operaciones ===");
        for (int i = 0; i < HistorialGlobal.Count; i++)
            Console.WriteLine(HistorialGlobal[i]);

        Console.WriteLine("\n=== Estado Final de Cuentas ===");
        for (int i = 0; i < Clientes.Count; i++) {
            Console.WriteLine($"\nCliente: {Clientes[i].Nombre}");
            for (int j = 0; j < Clientes[i].Cuentas.Count; j++) {
                Console.WriteLine($"Cuenta {Clientes[i].Cuentas[j].Numero} - Saldo: {Clientes[i].Cuentas[j].Saldo} - Puntos: {Clientes[i].Cuentas[j].Puntos}");
            }
        }
    }
}

class Cliente {
    public string Nombre {get; set;}
    public List<Cuenta> Cuentas {get; set;} = new List<Cuenta>();
    public List<Operacion> HistorialPersonal {get; set;} = new List<Operacion>();

    public Cliente(string nombre) => Nombre = nombre;
    public void AgregarCuenta(Cuenta cuenta) => Cuentas.Add(cuenta);
    public void RegistrarOperacion(Operacion operacion) => HistorialPersonal.Add(operacion);
}

abstract class Cuenta {
    public string Numero { get; }
    public decimal Saldo { get; set; }
    public int Puntos { get; set; } = 0;

    protected Cuenta(string numero, decimal saldoInicial) {
        Numero = numero;
        Saldo = saldoInicial;
    }

    public abstract void AcumularPuntos(decimal monto);
    public void Depositar(decimal monto) => Saldo += monto;
    public bool Extraer(decimal monto) {
        if (Saldo >= monto) { Saldo -= monto; return true; }
        return false;
    }
}

class CuentaOro : Cuenta {
    public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }
    public override void AcumularPuntos(decimal monto) => Puntos += (int)(monto >= 1000 ? monto * 0.05m : monto * 0.03m);
}

class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }
    public override void AcumularPuntos(decimal monto) => Puntos += (int)(monto * 0.02m);
}

class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }
    public override void AcumularPuntos(decimal monto) => Puntos += (int)(monto * 0.01m);
}

abstract class Operacion {
    public string Tipo {get; set;}
    public decimal Monto {get; set;}
    public Cuenta CuentaOrigen {get; set;}
    public Cuenta? CuentaDestino {get; set;}

    protected Operacion(string tipo, decimal monto, Cuenta cuentaOrigen, Cuenta? cuentaDestino = null) {
        Tipo = tipo;
        Monto = monto;
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
    }

    public override string ToString() => CuentaDestino == null
        ? $"{Tipo}: {Monto} en cuenta {CuentaOrigen.Numero}"
        : $"{Tipo}: {Monto} de {CuentaOrigen.Numero} a {CuentaDestino.Numero}";
}

class Deposito : Operacion {
    public Deposito(Cuenta cuenta, decimal monto) : base("Depósito", monto, cuenta) {
        cuenta.Depositar(monto);
    }
}

class Retiro : Operacion {
    public Retiro(Cuenta cuenta, decimal monto) : base("Retiro", monto, cuenta) {
        if (!cuenta.Extraer(monto)) throw new Exception("Fondos insuficientes");
    }
}

class Pago : Operacion {
    public Pago(Cuenta cuenta, decimal monto) : base("Pago", monto, cuenta) {
        if (!cuenta.Extraer(monto)) throw new Exception("Fondos insuficientes");
        cuenta.AcumularPuntos(monto);
    }
}

class Transferencia : Operacion {
    public Transferencia(Cuenta origen, Cuenta destino, decimal monto) : base("Transferencia", monto, origen, destino) {
        if (!origen.Extraer(monto)) throw new Exception("Fondos insuficientes");
        destino.Depositar(monto);
    }
}

class Program {
    static void Main() {
        Banco banco = new Banco();
        Cliente cliente1 = new Cliente("Maximiliano Luna");
        Cliente cliente2 = new Cliente("Maria Gomez");
        Cliente cliente3 = new Cliente("Alejandro Di Battista");
        Cliente cliente4 = new Cliente("Luciana Rodriguez");
        
        Cuenta cuenta1 = new CuentaOro("10000", 5000);
        Cuenta cuenta2 = new CuentaPlata("10001", 3000);
        Cuenta cuenta3 = new CuentaBronce("10002", 10000);
        Cuenta cuenta4 = new CuentaOro("10003", 8000);
        
        cliente1.AgregarCuenta(cuenta1);
        cliente2.AgregarCuenta(cuenta2);
        cliente3.AgregarCuenta(cuenta3);
        cliente4.AgregarCuenta(cuenta4);
        banco.AgregarCliente(cliente1);
        banco.AgregarCliente(cliente2);
        banco.AgregarCliente(cliente3);
        banco.AgregarCliente(cliente4);
        
        var op1 = new Deposito(cuenta1, 2000);
        var op2 = new Retiro(cuenta2, 500);
        var op3 = new Pago(cuenta1, 1500);
        var op4 = new Transferencia(cuenta3, cuenta1, 2000);
        var op5 = new Retiro(cuenta4, 8000);
        
        banco.RegistrarOperacion(op1);
        banco.RegistrarOperacion(op2);
        banco.RegistrarOperacion(op3);
        banco.RegistrarOperacion(op4);
        banco.RegistrarOperacion(op5);
                
        cliente1.RegistrarOperacion(op1);
        cliente1.RegistrarOperacion(op3);
        cliente1.RegistrarOperacion(op4);
        cliente2.RegistrarOperacion(op2);
        cliente2.RegistrarOperacion(op4);
        cliente3.RegistrarOperacion(op5);
        
        
        banco.MostrarReporte();
    }
}
