// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Banco
{
    public static List<Banco> TodosLosBancos = new List<Banco>();
    public string Nombre;
    public List<Cliente> Clientes;
    public List<Operacion> RegistroOperaciones;

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        RegistroOperaciones = new List<Operacion>();
        TodosLosBancos.Add(this);
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public Cuenta BuscarCuenta(string numeroCuenta)
    {
        foreach (var cliente in Clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.Numero == numeroCuenta)
                {
                    return cuenta;
                }
            }
        }
        return null;
    }

    public bool Registrar(Operacion operacion)
    {
        bool resultado = operacion.Ejecutar(this);
        if (resultado)
        {
            RegistroOperaciones.Add(operacion);
        }
        return resultado;
    }

    public void Informe()
    {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}");
        Console.WriteLine();

        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal():N2} | Puntos Total: $ {cliente.PuntosTotal():N2}");
            Console.WriteLine();

            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:N2} | Puntos: $ {cuenta.Puntos:N2}");
                foreach (var op in cuenta.Operaciones)
                {
                    Console.WriteLine($"     -  {op.Descripcion()}");
                }
                Console.WriteLine();
            }
        }
    }
}

class Cliente
{
    public string Nombre;
    public List<Cuenta> Cuentas;

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta)
    {
        cuenta.Propietario = this;
        Cuentas.Add(cuenta);
    }

    public double SaldoTotal()
    {
        double total = 0;
        foreach (var cuenta in Cuentas)
        {
            total += cuenta.Saldo;
        }
        return total;
    }

    public double PuntosTotal()
    {
        double total = 0;
        foreach (var cuenta in Cuentas)
        {
            total += cuenta.Puntos;
        }
        return total;
    }
}

abstract class Cuenta
{
    public string Numero;
    public double Saldo;
    public double Puntos;
    public Cliente Propietario;
    public List<Operacion> Operaciones;

    public Cuenta(string numero, double saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Puntos = 0;
        Operaciones = new List<Operacion>();
    }

    public void AgregarOperacion(Operacion op)
    {
        Operaciones.Add(op);
    }

    public virtual bool Depositar(double monto)
    {
        Saldo += monto;
        return true;
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

    public virtual bool Transferir(double monto, Cuenta destino)
    {
        if (Retirar(monto))
        {
            destino.Depositar(monto);
            return true;
        }
        return false;
    }

    public abstract bool Pagar(double monto);

    public override string ToString()
    {
        return $"{Numero}/{Propietario.Nombre}";
    }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldoInicial) : base(numero, saldoInicial) { }

    public override bool Pagar(double monto)
    {
        if (Retirar(monto))
        {
            if (monto > 1000)
            {
                Puntos += monto * 0.05;
            }
            else
            {
                Puntos += monto * 0.03;
            }
            return true;
        }
        return false;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldoInicial) : base(numero, saldoInicial) { }

    public override bool Pagar(double monto)
    {
        if (Retirar(monto))
        {
            Puntos += monto * 0.02;
            return true;
        }
        return false;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldoInicial) : base(numero, saldoInicial) { }

    public override bool Pagar(double monto)
    {
        if (Retirar(monto))
        {
            Puntos += monto * 0.01;
            return true;
        }
        return false;
    }
}

abstract class Operacion
{
    public DateTime Fecha;
    public double Monto;

    public Operacion(double monto)
    {
        Fecha = DateTime.Now;
        Monto = monto;
    }

    public abstract bool Ejecutar(Banco banco);
    public abstract string Descripcion();
}

class Deposito : Operacion
{
    public string NumeroCuenta;
    public Cuenta CuentaDestino;

    public Deposito(string numeroCuenta, double monto) : base(monto)
    {
        NumeroCuenta = numeroCuenta;
    }

    public override bool Ejecutar(Banco banco)
    {
        CuentaDestino = banco.BuscarCuenta(NumeroCuenta);
        if (CuentaDestino != null)
        {
            bool resultado = CuentaDestino.Depositar(Monto);
            if (resultado)
            {
                CuentaDestino.AgregarOperacion(this);
            }
            return resultado;
        }
        return false;
    }

    public override string Descripcion()
    {
        return $"Deposito $ {Monto:N2} a [{CuentaDestino}]";
    }
}

class Retiro : Operacion
{
    public string NumeroCuenta;
    public Cuenta CuentaOrigen;

    public Retiro(string numeroCuenta, double monto) : base(monto)
    {
        NumeroCuenta = numeroCuenta;
    }

    public override bool Ejecutar(Banco banco)
    {
        CuentaOrigen = banco.BuscarCuenta(NumeroCuenta);
        if (CuentaOrigen != null)
        {
            bool resultado = CuentaOrigen.Retirar(Monto);
            if (resultado)
            {
                CuentaOrigen.AgregarOperacion(this);
            }
            return resultado;
        }
        return false;
    }

    public override string Descripcion()
    {
        return $"Retiro $ {Monto:N2} de [{CuentaOrigen}]";
    }
}

class Transferencia : Operacion
{
    public string NumeroCuentaOrigen;
    public string NumeroCuentaDestino;
    public Cuenta CuentaOrigen;
    public Cuenta CuentaDestino;

    public Transferencia(string numeroCuentaOrigen, string numeroCuentaDestino, double monto) : base(monto)
    {
        NumeroCuentaOrigen = numeroCuentaOrigen;
        NumeroCuentaDestino = numeroCuentaDestino;
    }

    public override bool Ejecutar(Banco banco)
    {
        CuentaOrigen = banco.BuscarCuenta(NumeroCuentaOrigen);
        
        CuentaDestino = banco.BuscarCuenta(NumeroCuentaDestino);
        if (CuentaDestino == null)
        {
            foreach (var otroBanco in Banco.TodosLosBancos)
            {
                if (otroBanco != banco)
                {
                    CuentaDestino = otroBanco.BuscarCuenta(NumeroCuentaDestino);
                    if (CuentaDestino != null) break;
                }
            }
        }

        if (CuentaOrigen != null && CuentaDestino != null)
        {
            bool resultado = CuentaOrigen.Transferir(Monto, CuentaDestino);
            if (resultado)
            {
                CuentaOrigen.AgregarOperacion(this);
                CuentaDestino.AgregarOperacion(this);
            }
            return resultado;
        }
        return false;
    }

    public override string Descripcion()
    {
        return $"Transferencia $ {Monto:N2} de [{CuentaOrigen}] a [{CuentaDestino}]";
    }
}

class Pago : Operacion
{
    public string NumeroCuenta;
    public Cuenta CuentaOrigen;

    public Pago(string numeroCuenta, double monto) : base(monto)
    {
        NumeroCuenta = numeroCuenta;
    }

    public override bool Ejecutar(Banco banco)
    {
        CuentaOrigen = banco.BuscarCuenta(NumeroCuenta);
        if (CuentaOrigen != null)
        {
            bool resultado = CuentaOrigen.Pagar(Monto);
            if (resultado)
            {
                CuentaOrigen.AgregarOperacion(this);
            }
            return resultado;
        }
        return false;
    }

    public override string Descripcion()
    {
        return $"Pago $ {Monto:N2} con [{CuentaOrigen}]";
    }
}

class Program
{
    static void Main(string[] args)
    {
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
    }
}