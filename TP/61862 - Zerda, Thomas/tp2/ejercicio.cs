using System;
using System.Collections.Generic;

class Banco
{
    public List<Cliente> clientes = new List<Cliente>();
    public List<Operacion> historialGeneral = new List<Operacion>();

    public void AgregarCliente(Cliente c)
    {
        clientes.Add(c);
    }

    public void RegistrarOperacion(Operacion op)
    {
        historialGeneral.Add(op);
        foreach (var c in clientes)
        {
            foreach (var cuenta in c.cuentas)
            {
                if (op.InvolucraCuenta(cuenta))
                {
                    c.historial.Add(op);
                }
            }
        }
    }

    public void MostrarInforme()
    {
        Console.WriteLine("=== TODAS LAS OPERACIONES ===");
        foreach (var op in historialGeneral)
        {
            Console.WriteLine(op.MostrarDetalle());
        }

        Console.WriteLine("\n=== ESTADO FINAL DE CUENTAS ===");
        foreach (var cli in clientes)
        {
            foreach (var cuenta in cli.cuentas)
            {
                Console.WriteLine($"Cuenta {cuenta.Numero}: Saldo = {cuenta.Saldo}, Puntos = {cuenta.PuntosAcumulados}");
            }
        }
    }

    public void MostrarHistorialPorCliente()
    {
        Console.WriteLine("\n=== HISTORIAL POR CLIENTE ===");
        foreach (var cli in clientes)
        {
            Console.WriteLine($"Cliente: {cli.Nombre}");
            foreach (var op in cli.historial)
            {
                Console.WriteLine(op.MostrarDetalle());
            }
        }
    }
}

class Cliente
{
    public string Nombre;
    public List<Cuenta> cuentas = new List<Cuenta>();
    public List<Operacion> historial = new List<Operacion>();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void AgregarCuenta(Cuenta cuenta)
    {
        cuentas.Add(cuenta);
    }
}

abstract class Cuenta
{
    public string Numero;
    public decimal Saldo;
    public decimal PuntosAcumulados;

    public Cuenta(string numero, decimal saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
        PuntosAcumulados = 0;
    }

    public void Depositar(decimal monto)
    {
        Saldo += monto;
    }

    public void Extraer(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
        }
    }

    public abstract void Pagar(decimal monto);

    public void Transferir(decimal monto, Cuenta destino)
    {
        if (Saldo >= monto)
        {
            Extraer(monto);
            destino.Depositar(monto);
        }
    }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override void Pagar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            if (monto > 1000)
            {
                PuntosAcumulados += monto * 0.05m;
            }
            else
            {
                PuntosAcumulados += monto * 0.03m;
            }
        }
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

    public override void Pagar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            PuntosAcumulados += monto * 0.02m;
        }
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

    public override void Pagar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            PuntosAcumulados += monto * 0.01m;
        }
    }
}

abstract class Operacion
{
    public decimal Monto;
    public DateTime Fecha;

    public Operacion(decimal monto)
    {
        Monto = monto;
        Fecha = DateTime.Now;
    }

    public abstract string MostrarDetalle();
    public abstract bool InvolucraCuenta(Cuenta cuenta);
}

class Deposito : Operacion
{
    public Cuenta destino;

    public Deposito(decimal monto, Cuenta cuenta) : base(monto)
    {
        destino = cuenta;
        cuenta.Depositar(monto);
    }

    public override string MostrarDetalle()
    {
        return $"{Fecha}: Depósito de {Monto} en {destino.Numero}";
    }

    public override bool InvolucraCuenta(Cuenta cuenta)
    {
        return cuenta == destino;
    }
}

class Retiro : Operacion
{
    public Cuenta origen;

    public Retiro(decimal monto, Cuenta cuenta) : base(monto)
    {
        origen = cuenta;
        cuenta.Extraer(monto);
    }

    public override string MostrarDetalle()
    {
        return $"{Fecha}: Retiro de {Monto} en {origen.Numero}";
    }

    public override bool InvolucraCuenta(Cuenta cuenta)
    {
        return cuenta == origen;
    }
}

class Pago : Operacion
{
    public Cuenta cuenta;

    public Pago(decimal monto, Cuenta cuenta) : base(monto)
    {
        this.cuenta = cuenta;
        cuenta.Pagar(monto);
    }

    public override string MostrarDetalle()
    {
        return $"{Fecha}: Pago de {Monto} en {cuenta.Numero}";
    }

    public override bool InvolucraCuenta(Cuenta cuenta)
    {
        return this.cuenta == cuenta;
    }
}

class Transferencia : Operacion
{
    public Cuenta origen;
    public Cuenta destino;

    public Transferencia(decimal monto, Cuenta origen, Cuenta destino) : base(monto)
    {
        this.origen = origen;
        this.destino = destino;
        origen.Transferir(monto, destino);
    }

    public override string MostrarDetalle()
    {
        return $"{Fecha}: Transferencia de {Monto} de {origen.Numero} a {destino.Numero}";
    }

    public override bool InvolucraCuenta(Cuenta cuenta)
    {
        return cuenta == origen || cuenta == destino;
    }
}

class Program
{
    static void Main()
    {
        Banco banco = new Banco();

        Cliente juan = new Cliente("Juan Pérez");
        Cuenta cuentaOro = new CuentaOro("00001", 5000);
        juan.AgregarCuenta(cuentaOro);

        Cliente maria = new Cliente("María García");
        Cuenta cuentaPlata = new CuentaPlata("00002", 3000);
        Cuenta cuentaBronce = new CuentaBronce("00003", 1500);
        maria.AgregarCuenta(cuentaPlata);
        maria.AgregarCuenta(cuentaBronce);

        banco.AgregarCliente(juan);
        banco.AgregarCliente(maria);

        banco.RegistrarOperacion(new Deposito(1000, cuentaOro));
        banco.RegistrarOperacion(new Retiro(200, cuentaPlata));
        banco.RegistrarOperacion(new Pago(1200, cuentaOro));
        banco.RegistrarOperacion(new Transferencia(300, cuentaPlata, cuentaBronce));

        banco.MostrarInforme();
        banco.MostrarHistorialPorCliente();
    }
}
