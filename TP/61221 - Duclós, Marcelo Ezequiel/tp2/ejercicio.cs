using System;
using System.Collections.Generic;

//TP2: Sistema de Cuentas Bancarias//

class Banco
{
    public string Nombre { get; set; }
    private List<Cliente> clientes;
    private List<Operacion> operacionesGlobales;

    public Banco(string nombre)
    {
        Nombre = nombre;
        clientes = new List<Cliente>();
        operacionesGlobales = new List<Operacion>();
    }

    public void Agregar(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public void Registrar(Operacion op)
    {
        if (op.Ejecutar(this))
        {
            operacionesGlobales.Add(op);
        }
    }

    public bool BuscarCuenta(string num, out Cliente clienteEncontrado, out Cuenta cuentaEncontrada)
    {
        foreach (Cliente cli in clientes)
        {
            foreach (Cuenta cta in cli.Cuentas)
            {
                if (cta.Numero == num)
                {
                    clienteEncontrado = cli;
                    cuentaEncontrada = cta;
                    return true;
                }
            }
        }
        clienteEncontrado = null;
        cuentaEncontrada = null;
        return false;
    }

    public void Informe()
    {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {clientes.Count}\n");

        foreach (Cliente cli in clientes)
        {
            decimal saldoTotal = 0;
            decimal puntosTotal = 0;
            foreach (Cuenta cta in cli.Cuentas)
            {
                saldoTotal += cta.Saldo;
                puntosTotal += cta.Puntos;
            }
            Console.WriteLine($"  Cliente: {cli.Nombre} | Saldo Total: $ {saldoTotal:N2} | Puntos Total: $ {puntosTotal:N2}\n");

            foreach (Cuenta cta in cli.Cuentas)
            {
                Console.WriteLine($"    Cuenta: {cta.Numero} | Saldo: $ {cta.Saldo:N2} | Puntos: $ {cta.Puntos:N2}");
                foreach (string detalle in cta.Historial)
                {
                    Console.WriteLine("     -  " + detalle);
                }
                Console.WriteLine();
            }
        }
        Console.WriteLine();
    }
}


class Cliente
{
    public string Nombre { get; set; }
    public List<Cuenta> Cuentas { get; set; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }
}


abstract class Cuenta
{
    public string Numero { get; private set; }
    public decimal Saldo { get; set; }
    public decimal Puntos { get; set; } = 0;
    public List<string> Historial { get; set; } = new List<string>();

    public Cuenta(string numero, decimal saldo = 0)
    {
        Numero = numero;
        Saldo = saldo;
    }

    public virtual bool Depositar(decimal cantidad)
    {
        if (cantidad < 0)
            return false;
        Saldo += cantidad;
        Historial.Add($"Deposito $ {cantidad:N2} a [{Numero}]");
        return true;
    }

    public virtual bool Extraer(decimal cantidad)
    {
        if (cantidad < 0 || cantidad > Saldo)
            return false;
        Saldo -= cantidad;
        Historial.Add($"Retiro $ {cantidad:N2} de [{Numero}]");
        return true;
    }

    public virtual bool Pagar(decimal cantidad)
    {
        if (cantidad < 0 || cantidad > Saldo)
            return false;
        Saldo -= cantidad;
        decimal pts = AcumularPuntos(cantidad);
        Puntos += pts;
        Historial.Add($"Pago $ {cantidad:N2} con [{Numero}] (+" + pts.ToString("N2") + " puntos)");
        return true;
    }

    public virtual bool Transferir(decimal cantidad, Cuenta destino)
    {
        if (cantidad < 0 || cantidad > Saldo)
            return false;
        Saldo -= cantidad;
        Historial.Add($"Transferencia $ {cantidad:N2} de [{Numero}] a [{destino.Numero}]");
        destino.RecibirTransferencia(cantidad, this);
        return true;
    }

    public virtual void RecibirTransferencia(decimal cantidad, Cuenta origen)
    {
        Saldo += cantidad;
        Historial.Add($"Transferencia $ {cantidad:N2} de [{origen.Numero}] a [{Numero}]");
    }

    public abstract decimal AcumularPuntos(decimal cantidad);
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldo = 0) : base(numero, saldo) { }

    public override decimal AcumularPuntos(decimal cantidad)
    {
        return cantidad > 1000 ? cantidad * 0.05m : cantidad * 0.03m;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldo = 0) : base(numero, saldo) { }

    public override decimal AcumularPuntos(decimal cantidad)
    {
        return cantidad * 0.02m;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldo = 0) : base(numero, saldo) { }

    public override decimal AcumularPuntos(decimal cantidad)
    {
        return cantidad * 0.01m;
    }
}


abstract class Operacion
{
    public decimal Monto { get; set; }

    public Operacion(decimal monto)
    {
        Monto = monto;
    }

    public abstract bool Ejecutar(Banco banco);
}

class Deposito : Operacion
{
    public string CuentaDestino { get; set; }

    public Deposito(string cuentaDestino, decimal monto) : base(monto)
    {
        CuentaDestino = cuentaDestino;
    }

    public override bool Ejecutar(Banco banco)
    {
        if (banco.BuscarCuenta(CuentaDestino, out Cliente cli, out Cuenta cta))
        {
            return cta.Depositar(Monto);
        }
        else
        {
            Console.WriteLine("Cuenta " + CuentaDestino + " no encontrada para Deposito.");
            return false;
        }
    }
}

class Retiro : Operacion
{
    public string CuentaOrigen { get; set; }

    public Retiro(string cuentaOrigen, decimal monto) : base(monto)
    {
        CuentaOrigen = cuentaOrigen;
    }

    public override bool Ejecutar(Banco banco)
    {
        if (banco.BuscarCuenta(CuentaOrigen, out Cliente cli, out Cuenta cta))
        {
            return cta.Extraer(Monto);
        }
        else
        {
            Console.WriteLine("Cuenta " + CuentaOrigen + " no encontrada para Retiro.");
            return false;
        }
    }
}

class Transferencia : Operacion
{
    public string CuentaOrigen { get; set; }
    public string CuentaDestino { get; set; }

    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto) : base(monto)
    {
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
    }

    public override bool Ejecutar(Banco banco)
    {
        if (banco.BuscarCuenta(CuentaOrigen, out Cliente cliOrigen, out Cuenta ctaOrigen) &&
            banco.BuscarCuenta(CuentaDestino, out Cliente cliDestino, out Cuenta ctaDestino))
        {
            return ctaOrigen.Transferir(Monto, ctaDestino);
        }
        else
        {
            Console.WriteLine("Cuenta origen o destino no encontrada para Transferencia.");
            return false;
        }
    }
}

class Pago : Operacion
{
    public string CuentaOrigen { get; set; }

    public Pago(string cuentaOrigen, decimal monto) : base(monto)
    {
        CuentaOrigen = cuentaOrigen;
    }

    public override bool Ejecutar(Banco banco)
    {
        if (banco.BuscarCuenta(CuentaOrigen, out Cliente cli, out Cuenta cta))
        {
            return cta.Pagar(Monto);
        }
        else
        {
            Console.WriteLine("Cuenta " + CuentaOrigen + " no encontrada para Pago.");
            return false;
        }
    }
}


class Program
{
    public static void Main(string[] args)
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

        Console.WriteLine("Presione cualquier tecla para salir...");
        Console.ReadKey();
    }
}

