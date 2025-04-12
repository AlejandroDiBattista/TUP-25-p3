// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

using System;
using System.Collections.Generic;

public abstract class Cuenta
{
    public string Numero { get; set; }
    public decimal Saldo { get; set; }
    public decimal Puntos { get; set; }

    public Cuenta(string numero, decimal saldo)
    {
        Numero = numero;
        Saldo = saldo;
        Puntos = 0;
    }

    public abstract void AcumularPuntos(decimal monto);
}

public class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto)
    {
        if (monto > 1000)
            Puntos += monto * 0.05m;
        else
            Puntos += monto * 0.03m;
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.02m;
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.01m;
    }
}

public abstract class Operacion
{
    public string NumeroCuenta { get; set; }
    public decimal Monto { get; set; }

    public Operacion(string numeroCuenta, decimal monto)
    {
        NumeroCuenta = numeroCuenta;
        Monto = monto;
    }

    public abstract void Ejecutar(Banco banco);
}

public class Deposito : Operacion
{
    public Deposito(string numeroCuenta, decimal monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        cuenta.Saldo += Monto;
        banco.AgregarOperacion(this);
    }
}

public class Retiro : Operacion
{
    public Retiro(string numeroCuenta, decimal monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        if (cuenta.Saldo >= Monto)
        {
            cuenta.Saldo -= Monto;
            banco.AgregarOperacion(this);
        }
        else
        {
            Console.WriteLine("Fondos insuficientes en la cuenta " + NumeroCuenta);
        }
    }
}

public class Pago : Operacion
{
    public Pago(string numeroCuenta, decimal monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        if (cuenta.Saldo >= Monto)
        {
            cuenta.Saldo -= Monto;
            cuenta.AcumularPuntos(Monto);
            banco.AgregarOperacion(this);
        }
        else
        {
            Console.WriteLine("Fondos insuficientes en la cuenta " + NumeroCuenta);
        }
    }
}

public class Transferencia : Operacion
{
    public string NumeroCuentaDestino { get; set; }

    public Transferencia(string numeroCuenta, string numeroCuentaDestino, decimal monto)
        : base(numeroCuenta, monto)
    {
        NumeroCuentaDestino = numeroCuentaDestino;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuentaOrigen = banco.ObtenerCuenta(NumeroCuenta);
        var cuentaDestino = banco.ObtenerCuenta(NumeroCuentaDestino);
        if (cuentaOrigen.Saldo >= Monto)
        {
            cuentaOrigen.Saldo -= Monto;
            cuentaDestino.Saldo += Monto;
            banco.AgregarOperacion(this);
        }
        else
        {
            Console.WriteLine("Fondos insuficientes en la cuenta origen " + NumeroCuenta);
        }
    }
}

public class Cliente
{
    public string Nombre { get; set; }
    public List<Cuenta> Cuentas { get; set; }
    public List<Operacion> HistorialOperaciones { get; set; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
        HistorialOperaciones = new List<Operacion>();
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public void AgregarOperacion(Operacion operacion)
    {
        HistorialOperaciones.Add(operacion);
    }
}

public class Banco
{
    public string Nombre { get; set; }
    public List<Cliente> Clientes { get; set; }
    public List<Operacion> HistorialOperacionesGlobal { get; set; }

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        HistorialOperacionesGlobal = new List<Operacion>();
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public Cuenta ObtenerCuenta(string numeroCuenta)
    {
        foreach (var cliente in Clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.Numero == numeroCuenta)
                    return cuenta;
            }
        }
        return null;
    }

    public void Registrar(Operacion operacion)
    {
        operacion.Ejecutar(this);
    }

    public void AgregarOperacion(Operacion operacion)
    {
        HistorialOperacionesGlobal.Add(operacion);
    }

    public void Informe()
    {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}");
        foreach (var cliente in Clientes)
        {
            decimal saldoTotal = 0, puntosTotal = 0;
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {saldoTotal:C} | Puntos Total: $ {puntosTotal:C}");
            foreach (var cuenta in cliente.Cuentas)
            {
                saldoTotal += cuenta.Saldo;
                puntosTotal += cuenta.Puntos;
                Console.WriteLine($"\n    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:C} | Puntos: $ {cuenta.Puntos:C}");
                foreach (var operacion in cliente.HistorialOperaciones)
                {
                    Console.WriteLine($"     -  {operacion.GetType().Name} ${operacion.Monto:C} de [{operacion.NumeroCuenta}/{cliente.Nombre}]");
                }
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
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

        // Informe final
        nac.Informe();
        tup.Informe();
    }
}
