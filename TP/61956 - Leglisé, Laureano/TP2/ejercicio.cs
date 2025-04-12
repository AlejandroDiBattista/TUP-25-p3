using System;
using System.Collections.Generic;

abstract class Cuenta
{
    public string Numero { get; private set; }
    public double Saldo { get; protected set; }
    public double Puntos { get; protected set; }
    public Cliente Titular { get; set; }
    public List<Operacion> Historial { get; private set; }

    public Cuenta(string numero, double saldo)
    {
        Numero = numero;
        Saldo = saldo;
        Puntos = 0;
        Historial = new List<Operacion>();
    }

    public void Depositar(double monto)
    {
        Saldo += monto;
    }

    public bool Extraer(double monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public void AgregarOperacion(Operacion op)
    {
        Historial.Add(op);
    }

    public abstract void AplicarPago(double monto);
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldo) : base(numero, saldo) { }

    public override void AplicarPago(double monto)
    {
        if (Extraer(monto))
        {
            if (monto > 1000)
                Puntos += monto * 0.05;
            else
                Puntos += monto * 0.03;
        }
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldo) : base(numero, saldo) { }

    public override void AplicarPago(double monto)
    {
        if (Extraer(monto))
        {
            Puntos += monto * 0.02;
        }
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldo) : base(numero, saldo) { }

    public override void AplicarPago(double monto)
    {
        if (Extraer(monto))
        {
            Puntos += monto * 0.01;
        }
    }
}

class Cliente
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
        cuenta.Titular = this;
        Cuentas.Add(cuenta);
    }

    public double TotalSaldo()
    {
        double total = 0;
        foreach (var cuenta in Cuentas)
            total += cuenta.Saldo;
        return total;
    }

    public double TotalPuntos()
    {
        double total = 0;
        foreach (var cuenta in Cuentas)
            total += cuenta.Puntos;
        return total;
    }
}

abstract class Operacion
{
    public double Monto { get; protected set; }
    public abstract void Ejecutar(Banco banco);
    public abstract string Descripcion();
}

class Deposito : Operacion
{
    private string numero;

    public Deposito(string numero, double monto)
    {
        this.numero = numero;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(numero);
        cuenta?.Depositar(Monto);
        cuenta?.AgregarOperacion(this);
    }

    public override string Descripcion()
    {
        return $"Deposito $ {Monto:0.00} a [{numero}/{(Banco.NombreTitular(numero))}]";
    }
}

class Retiro : Operacion
{
    private string numero;

    public Retiro(string numero, double monto)
    {
        this.numero = numero;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(numero);
        if (cuenta != null && cuenta.Extraer(Monto))
            cuenta.AgregarOperacion(this);
    }

    public override string Descripcion()
    {
        return $"Retiro $ {Monto:0.00} de [{numero}/{(Banco.NombreTitular(numero))}]";
    }
}

class Pago : Operacion
{
    private string numero;

    public Pago(string numero, double monto)
    {
        this.numero = numero;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(numero);
        cuenta?.AplicarPago(Monto);
        cuenta?.AgregarOperacion(this);
    }

    public override string Descripcion()
    {
        return $"Pago $ {Monto:0.00} con [{numero}/{(Banco.NombreTitular(numero))}]";
    }
}

class Transferencia : Operacion
{
    private string origen;
    private string destino;

    public Transferencia(string origen, string destino, double monto)
    {
        this.origen = origen;
        this.destino = destino;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuentaOrigen = banco.BuscarCuenta(origen);
        var cuentaDestino = banco.BuscarCuenta(destino);
        if (cuentaOrigen != null && cuentaDestino != null && cuentaOrigen.Extraer(Monto))
        {
            cuentaDestino.Depositar(Monto);
            cuentaOrigen.AgregarOperacion(this);
            cuentaDestino.AgregarOperacion(this);
        }
    }

    public override string Descripcion()
    {
        return $"Transferencia $ {Monto:0.00} de [{origen}/{(Banco.NombreTitular(origen))}] a [{destino}/{(Banco.NombreTitular(destino))}]";
    }
}

class Banco
{
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; }
    public List<Operacion> Operaciones { get; private set; }
    public static Dictionary<string, string> Titulares = new Dictionary<string, string>();

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        Operaciones = new List<Operacion>();
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
        foreach (var cuenta in cliente.Cuentas)
            Titulares[cuenta.Numero] = cliente.Nombre;
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var cliente in Clientes)
            foreach (var cuenta in cliente.Cuentas)
                if (cuenta.Numero == numero)
                    return cuenta;
        return null;
    }

    public void Registrar(Operacion operacion)
    {
        operacion.Ejecutar(this);
        Operaciones.Add(operacion);
    }

    public static string NombreTitular(string numero)
    {
        return Titulares.ContainsKey(numero) ? Titulares[numero] : "Desconocido";
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}\n");
        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.TotalSaldo():0.00} | Puntos Total: $ {cliente.TotalPuntos():0.00}\n");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:0.00} | Puntos: $ {cuenta.Puntos:0.00}");
                foreach (var op in cuenta.Historial)
                    Console.WriteLine($"     -  {op.Descripcion()}");
                Console.WriteLine();
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

        nac.Informe();
        tup.Informe();
    }
}
