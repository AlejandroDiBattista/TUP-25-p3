// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

// class Banco{}
// class Cliente{}

// abstract class Cuenta{}
// class CuentaOro: Cuenta{}
// class CuentaPlata: Cuenta{}
// class CuentaBronce: Cuenta{}

// abstract class Operacion{}
// class Deposito: Operacion{}
// class Retiro: Operacion{}
// class Transferencia: Operacion{}
// class Pago: Operacion{}
// using static System.Console;
// using System;
// using System.Collections.Generic;
// using System.Linq;


using System;
using System.Collections.Generic;

// ==== CLASES BASE ====

public abstract class Cuenta
{
    public string Numero { get; set; }
    public double Saldo { get; set; }

    public Cuenta(string numero, double saldo)
    {
        Numero = numero;
        Saldo = saldo;
    }

    public virtual void Depositar(double monto)
    {
        Saldo += monto;
    }

    public virtual bool Retirar(double monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public abstract string TipoCuenta();
}

public class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldo) : base(numero, saldo) { }
    public override string TipoCuenta() => "Oro";
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldo) : base(numero, saldo) { }
    public override string TipoCuenta() => "Plata";
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldo) : base(numero, saldo) { }
    public override string TipoCuenta() => "Bronce";
}

// ==== CLIENTE ====

public class Cliente
{
    public string Nombre { get; set; }
    public List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
    public int Puntos { get; set; } = 0;

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        return Cuentas.Find(c => c.Numero == numero);
    }
}

// ==== OPERACIONES ====

public abstract class Operacion
{
    public string NumeroCuentaOrigen { get; set; }
    public double Monto { get; set; }

    protected Operacion(string numeroCuenta, double monto)
    {
        NumeroCuentaOrigen = numeroCuenta;
        Monto = monto;
    }

    public abstract void Ejecutar(Banco banco);
}

public class Deposito : Operacion
{
    public Deposito(string numeroCuenta, double monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(NumeroCuentaOrigen);
        if (cuenta != null)
        {
            cuenta.Depositar(Monto);
            banco.AsignarPuntos(cuenta, Monto);
        }
    }
}

public class Retiro : Operacion
{
    public Retiro(string numeroCuenta, double monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(NumeroCuentaOrigen);
        if (cuenta != null)
        {
            cuenta.Retirar(Monto);
        }
    }
}

public class Transferencia : Operacion
{
    public string NumeroCuentaDestino { get; set; }

    public Transferencia(string origen, string destino, double monto) : base(origen, monto)
    {
        NumeroCuentaDestino = destino;
    }

    public override void Ejecutar(Banco banco)
    {
        Cuenta origen = banco.BuscarCuenta(NumeroCuentaOrigen);
        Cuenta destino = banco.BuscarCuenta(NumeroCuentaDestino);
        if (origen != null && destino != null && origen.Retirar(Monto))
        {
            destino.Depositar(Monto);
        }
    }
}

public class Pago : Operacion
{
    public Pago(string numeroCuenta, double monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(NumeroCuentaOrigen);
        if (cuenta != null)
        {
            cuenta.Retirar(Monto);
        }
    }
}

// ==== BANCO ====

public class Banco
{
    public string Nombre { get; set; }
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();

    public Banco(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion)
    {
        operaciones.Add(operacion);
        operacion.Ejecutar(this);
    }

    public Cuenta BuscarCuenta(string numeroCuenta)
    {
        foreach (var cliente in clientes)
        {
            var cuenta = cliente.BuscarCuenta(numeroCuenta);
            if (cuenta != null)
                return cuenta;
        }
        return null;
    }

    public Cliente BuscarClientePorCuenta(string numeroCuenta)
    {
        foreach (var cliente in clientes)
        {
            if (cliente.BuscarCuenta(numeroCuenta) != null)
                return cliente;
        }
        return null;
    }

    public void AsignarPuntos(Cuenta cuenta, double monto)
    {
        Cliente cliente = BuscarClientePorCuenta(cuenta.Numero);
        if (cliente == null) return;

        switch (cuenta.TipoCuenta())
        {
            case "Oro": cliente.Puntos += (int)(monto * 0.2); break;
            case "Plata": cliente.Puntos += (int)(monto * 0.1); break;
            case "Bronce": cliente.Puntos += (int)(monto * 0.05); break;
        }
    }

    public void Informe()
    {
        Console.WriteLine($"\n--- Informe del banco {Nombre} ---");
        foreach (var cliente in clientes)
        {
            Console.WriteLine($"Cliente: {cliente.Nombre}, Puntos: {cliente.Puntos}");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"  Cuenta: {cuenta.Numero}, Tipo: {cuenta.TipoCuenta()}, Saldo: {cuenta.Saldo}");
            }
        }
    }
}

// ==== CÓDIGO GLOBAL ====

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

// Operaciones
nac.Registrar(new Deposito("10001", 100));
nac.Registrar(new Retiro("10002", 200));
nac.Registrar(new Transferencia("10001", "10002", 300));
nac.Registrar(new Transferencia("10003", "10004", 500));
nac.Registrar(new Pago("10002", 400));

tup.Registrar(new Deposito("10005", 100));
tup.Registrar(new Retiro("10005", 200));
tup.Registrar(new Transferencia("10005", "10002", 300));
tup.Registrar(new Pago("10005", 400));

// Informes
nac.Informe();
tup.Informe();

