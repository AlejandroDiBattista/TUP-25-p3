// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

using System;
using System.Collections.Generic;

abstract class Cuenta
{
    public string Numero { get; private set; }
    public double Saldo { get; protected set; }
    public double Puntos { get; protected set; }
    public Cliente Dueño { get; set; }

    public Cuenta(string num, double plata)
    {
        Numero = num;
        Saldo = plata;
        Puntos = 0;
    }

    public void Depositar(double monto) => Saldo += monto;
    public bool Extraer(double monto)
    {
        if (Saldo >= monto) { Saldo -= monto; return true; }
        return false;
    }
    public abstract void Pagar(double monto);
}

class CuentaOro : Cuenta
{
    public CuentaOro(string num, double plata) : base(num, plata) { }
    public override void Pagar(double monto)
    {
        if (Extraer(monto))
        {
            Puntos += monto > 1000 ? monto * 0.05 : monto * 0.03;
        }
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string num, double plata) : base(num, plata) { }
    public override void Pagar(double monto)
    {
        if (Extraer(monto))
            Puntos += monto * 0.02;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string num, double plata) : base(num, plata) { }
    public override void Pagar(double monto)
    {
        if (Extraer(monto))
            Puntos += monto * 0.01;
    }
}

class Cliente
{
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }
    public List<Operacion> Historial { get; private set; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
        Historial = new List<Operacion>();
    }

    public void Agregar(Cuenta c)
    {
        c.Dueño = this;
        Cuentas.Add(c);
    }

    public double TotalSaldo()
    {
        double tot = 0;
        foreach (var c in Cuentas) tot += c.Saldo;
        return tot;
    }

    public double TotalPuntos()
    {
        double tot = 0;
        foreach (var c in Cuentas) tot += c.Puntos;
        return tot;
    }
}

abstract class Operacion
{
    public double Monto { get; protected set; }
    public string Tipo { get; protected set; }
    public abstract void Ejecutar(Banco banco);
    public abstract string Describir();
}

class Deposito : Operacion
{
    private string cuentaDestino;
    private Cliente cliente;

    public Deposito(string numCuenta, double monto)
    {
        cuentaDestino = numCuenta;
        Monto = monto;
        Tipo = "Deposito";
    }

    public override void Ejecutar(Banco banco)
    {
        var c = banco.BuscarCuenta(cuentaDestino);
        if (c != null)
        {
            c.Depositar(Monto);
            cliente = c.Dueño;
            cliente.Historial.Add(this);
            banco.Historial.Add(this);
        }
    }

    public override string Describir() =>
        $"-  Deposito $ {Monto:0.00} a [{cuentaDestino}/{cliente.Nombre}]";
}

class Retiro : Operacion
{
    private string cuentaOrigen;
    private Cliente cliente;

    public Retiro(string numCuenta, double monto)
    {
        cuentaOrigen = numCuenta;
        Monto = monto;
        Tipo = "Retiro";
    }

    public override void Ejecutar(Banco banco)
    {
        var c = banco.BuscarCuenta(cuentaOrigen);
        if (c != null && c.Extraer(Monto))
        {
            cliente = c.Dueño;
            cliente.Historial.Add(this);
            banco.Historial.Add(this);
        }
    }

    public override string Describir() =>
        $"-  Retiro $ {Monto:0.00} de [{cuentaOrigen}/{cliente.Nombre}]";
}

class Pago : Operacion
{
    private string cuentaOrigen;
    private Cliente cliente;

    public Pago(string numCuenta, double monto)
    {
        cuentaOrigen = numCuenta;
        Monto = monto;
        Tipo = "Pago";
    }

    public override void Ejecutar(Banco banco)
    {
        var c = banco.BuscarCuenta(cuentaOrigen);
        if (c != null)
        {
            c.Pagar(Monto);
            cliente = c.Dueño;
            cliente.Historial.Add(this);
            banco.Historial.Add(this);
        }
    }

    public override string Describir() =>
        $"-  Pago $ {Monto:0.00} con [{cuentaOrigen}/{cliente.Nombre}]";
}

class Transferencia : Operacion
{
    private string cuentaOrigen, cuentaDestino;
    private Cliente origenCliente, destinoCliente;

    public Transferencia(string origen, string destino, double monto)
    {
        cuentaOrigen = origen;
        cuentaDestino = destino;
        Monto = monto;
        Tipo = "Transferencia";
    }

    public override void Ejecutar(Banco banco)
    {
        var desde = banco.BuscarCuenta(cuentaOrigen);
        var hasta = banco.BuscarCuenta(cuentaDestino);
        if (desde != null && hasta != null && desde.Extraer(Monto))
        {
            hasta.Depositar(Monto);
            origenCliente = desde.Dueño;
            destinoCliente = hasta.Dueño;
            origenCliente.Historial.Add(this);
            destinoCliente.Historial.Add(this);
            banco.Historial.Add(this);
        }
    }

    public override string Describir() =>
        $"-  Transferencia $ {Monto:0.00} de [{cuentaOrigen}/{origenCliente.Nombre}] a [{cuentaDestino}/{destinoCliente.Nombre}]";
}

class Banco
{
    public string Nombre { get; private set; }
    private List<Cliente> Clientes;
    public List<Operacion> Historial;

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        Historial = new List<Operacion>();
    }

    public void Agregar(Cliente c) => Clientes.Add(c);

    public Cuenta BuscarCuenta(string num)
    {
        foreach (var cli in Clientes)
            foreach (var c in cli.Cuentas)
                if (c.Numero == num)
                    return c;
        return null;
    }

    public void Registrar(Operacion op) => op.Ejecutar(this);

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}\n");
        foreach (var cli in Clientes)
        {
            Console.WriteLine(
              $"  Cliente: {cli.Nombre} | Saldo Total: $ {cli.TotalSaldo():0.00} | Puntos Total: $ {cli.TotalPuntos():0.00}\n");
            foreach (var c in cli.Cuentas)
            {
                Console.WriteLine(
                  $"    Cuenta: {c.Numero} | Saldo: $ {c.Saldo:0.00} | Puntos: $ {c.Puntos:0.00}");
                foreach (var op in cli.Historial)
                    if (op.Describir().Contains(c.Numero))
                        Console.WriteLine("     " + op.Describir());
                Console.WriteLine();
            }
        }
    }
}



/// EJEMPLO DE USO ///

// Definiciones 

var julieta = new Cliente("Julieta Coronel");
julieta.Agregar(new CuentaOro("10001", 1000));
julieta.Agregar(new CuentaPlata("10002", 2000));

var nancy = new Cliente("Nancy Paez");
nancy.Agregar(new CuentaPlata("10003", 3000));
nancy.Agregar(new CuentaPlata("10004", 4000));

var luis = new Cliente("Luis Gomez");
luis.Agregar(new CuentaBronce("10005", 5000));

var nac = new Banco("Banco Nac");
nac.Agregar(julieta);
nac.Agregar(nancy);
nac.Agregar(luis);

// Registrar operaciones
nac.Registrar(new Deposito("10001", 100));
nac.Registrar(new Retiro("10002", 200));
nac.Registrar(new Transferencia("10001", "10002", 300));
nac.Registrar(new Transferencia("10003", "10004", 500));
nac.Registrar(new Pago("10002", 400));
nac.Registrar(new Deposito("10005", 100));
nac.Registrar(new Retiro("10005", 200));
nac.Registrar(new Transferencia("10005", "10002", 300));
nac.Registrar(new Pago("10005", 400));

// Informe final
nac.Informe();
Console.WriteLine("\n\nPresione una tecla para continuar...");