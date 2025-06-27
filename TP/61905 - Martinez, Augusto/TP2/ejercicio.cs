using System;
using System.Collections.Generic;

public class Banco
{
    private string nombre;
    private List<Cliente> clientes;
    public List<Operacion> HistorialGlobal { get; private set; }

    public Banco(string nombre)
    {
        this.nombre = nombre;
        clientes = new List<Cliente>();
        HistorialGlobal = new List<Operacion>();
    }

    public void Agregar(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public Cliente ObtenerClientePorNombre(string nombre)
    {
        return clientes.Find(c => c.Nombre == nombre);
    }

    public void Registrar(Operacion operacion)
    {
        operacion.Ejecutar();
        HistorialGlobal.Add(operacion);
    }

    public void Informe()
    {
        Console.WriteLine($"Banco: {nombre} | Clientes: {clientes.Count}");

        foreach (var cliente in clientes)
        {
            double saldoTotal = cliente.SaldoTotal();
            double puntosTotal = cliente.PuntosTotal();

            Console.WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: $ {saldoTotal:F2} | Puntos Total: $ {puntosTotal:F2}");

            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"\n    Cuenta: {cuenta.NumeroCuenta} | Saldo: $ {cuenta.Saldo:F2} | Puntos: $ {cuenta.Puntos:F2}");

                foreach (var operacion in cuenta.Historial)
                {
                    operacion.MostrarDetalles();
                }
            }
        }
    }
}

public class Cliente
{
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public double SaldoTotal()
    {
        double saldoTotal = 0;
        foreach (var cuenta in Cuentas)
        {
            saldoTotal += cuenta.Saldo;
        }
        return saldoTotal;
    }

    public double PuntosTotal()
    {
        double puntosTotal = 0;
        foreach (var cuenta in Cuentas)
        {
            puntosTotal += cuenta.Puntos;
        }
        return puntosTotal;
    }
}

public abstract class Cuenta
{
    public string NumeroCuenta { get; private set; }
    public double Saldo { get; protected set; }
    public double Puntos { get; protected set; }
    public List<Operacion> Historial { get; private set; }

    public Cuenta(string numeroCuenta, double saldoInicial = 0)
    {
        NumeroCuenta = numeroCuenta;
        Saldo = saldoInicial;
        Puntos = 0;
        Historial = new List<Operacion>();
    }

    public virtual void Depositar(double monto)
    {
        Saldo += monto;
    }

    public virtual bool Extraer(double monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public virtual void RealizarPago(double monto)
    {
        Saldo -= monto;
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        Historial.Add(operacion);
    }
}

public class CuentaOro : Cuenta
{
    public CuentaOro(string numeroCuenta, double saldoInicial) : base(numeroCuenta, saldoInicial) { }

    public override void RealizarPago(double monto)
    {
        base.RealizarPago(monto);
        AcumularPuntos(monto);
    }

    public void AcumularPuntos(double monto)
    {
        Puntos += monto > 1000 ? monto * 0.05 : monto * 0.03;
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numeroCuenta, double saldoInicial) : base(numeroCuenta, saldoInicial) { }

    public override void RealizarPago(double monto)
    {
        base.RealizarPago(monto);
        AcumularPuntos(monto);
    }

    public void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.02;
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numeroCuenta, double saldoInicial) : base(numeroCuenta, saldoInicial) { }

    public override void RealizarPago(double monto)
    {
        base.RealizarPago(monto);
        AcumularPuntos(monto);
    }

    public void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.01;
    }
}

public abstract class Operacion
{
    public double Monto { get; protected set; }
    public Cuenta CuentaOrigen { get; protected set; }
    public Cuenta CuentaDestino { get; protected set; }

    public Operacion(double monto, Cuenta cuentaOrigen, Cuenta cuentaDestino = null)
    {
        Monto = monto;
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
    }

    public abstract void Ejecutar();

    public virtual void MostrarDetalles()
    {
        if (CuentaDestino != null)
        {
            Console.WriteLine($"     -  {this.GetType().Name} $ {Monto:F2} de [{CuentaOrigen.NumeroCuenta}] a [{CuentaDestino.NumeroCuenta}]");
        }
        else
        {
            Console.WriteLine($"     -  {this.GetType().Name} $ {Monto:F2} de [{CuentaOrigen.NumeroCuenta}]");
        }
    }
}

public class Deposito : Operacion
{
    public Deposito(Cuenta cuenta, double monto) : base(monto, cuenta) { }

    public override void Ejecutar()
    {
        CuentaOrigen.Depositar(Monto);
        CuentaOrigen.RegistrarOperacion(this);
        Console.WriteLine($"     -  Depósito $ {Monto:F2} a la cuenta [{CuentaOrigen.NumeroCuenta}]");
    }
}

public class Retiro : Operacion
{
    public Retiro(Cuenta cuenta, double monto) : base(monto, cuenta) { }

    public override void Ejecutar()
    {
        if (CuentaOrigen.Extraer(Monto))
        {
            CuentaOrigen.RegistrarOperacion(this);
            Console.WriteLine($"     -  Retiro $ {Monto:F2} de la cuenta [{CuentaOrigen.NumeroCuenta}]");
        }
        else
        {
            Console.WriteLine($"     -  Fondos insuficientes para retiro $ {Monto:F2} de la cuenta [{CuentaOrigen.NumeroCuenta}]");
        }
    }
}

public class Pago : Operacion
{
    public Pago(Cuenta cuenta, double monto) : base(monto, cuenta) { }

    public override void Ejecutar()
    {
        CuentaOrigen.RealizarPago(Monto);
        if (CuentaOrigen is CuentaOro cuentaOro)
        {
            cuentaOro.AcumularPuntos(Monto);
        }
        else if (CuentaOrigen is CuentaPlata cuentaPlata)
        {
            cuentaPlata.AcumularPuntos(Monto);
        }
        else if (CuentaOrigen is CuentaBronce cuentaBronce)
        {
            cuentaBronce.AcumularPuntos(Monto);
        }

        CuentaOrigen.RegistrarOperacion(this);
        Console.WriteLine($"     -  Pago $ {Monto:F2} con la cuenta [{CuentaOrigen.NumeroCuenta}]");
    }
}

public class Transferencia : Operacion
{
    public Transferencia(Cuenta cuentaOrigen, Cuenta cuentaDestino, double monto)
        : base(monto, cuentaOrigen, cuentaDestino) { }

    public override void Ejecutar()
    {
        if (CuentaOrigen.Extraer(Monto))
        {
            CuentaDestino.Depositar(Monto);
            CuentaOrigen.RegistrarOperacion(this);
            CuentaDestino.RegistrarOperacion(this);
            Console.WriteLine($"     -  Transferencia $ {Monto:F2} de [{CuentaOrigen.NumeroCuenta}] a [{CuentaDestino.NumeroCuenta}]");
        }
        else
        {
            Console.WriteLine($"     -  Fondos insuficientes para transferencia $ {Monto:F2} de [{CuentaOrigen.NumeroCuenta}] a [{CuentaDestino.NumeroCuenta}]");
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

 
        nac.Registrar(new Deposito(raul.Cuentas[0], 100));   
        nac.Registrar(new Retiro(raul.Cuentas[1], 200));     
        nac.Registrar(new Transferencia(raul.Cuentas[0], sara.Cuentas[0], 300)); 
        nac.Registrar(new Transferencia(sara.Cuentas[0], sara.Cuentas[1], 500)); 
        nac.Registrar(new Pago(raul.Cuentas[1], 400));      
        tup.Registrar(new Deposito(luis.Cuentas[0], 100));   
        tup.Registrar(new Retiro(luis.Cuentas[0], 200));     
        tup.Registrar(new Transferencia(luis.Cuentas[0], raul.Cuentas[1], 300)); 
        tup.Registrar(new Pago(luis.Cuentas[0], 400));       
        nac.Informe();
        tup.Informe();
    }
}
