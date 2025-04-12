#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

abstract class Operacion
{
    public string? Descripcion { get; protected set; }
    public decimal Monto { get; protected set; }
    public abstract void Ejecutar(Banco banco);
}

class Deposito : Operacion
{
    private string numeroCuenta;
    public Deposito(string numeroCuenta, decimal monto)
    {
        this.numeroCuenta = numeroCuenta;
        Monto = monto;
    }
    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(numeroCuenta);
        if (cuenta != null)
        {
            cuenta.Depositar(Monto);
            cuenta.AgregarOperacion($"Deposito $ {Formato(Monto)} a [{cuenta.Numero}/{cuenta.Titular.Nombre}]");
        }
    }

    private string Formato(decimal monto) =>
        monto.ToString("N2").Replace('.', '#').Replace(',', '.').Replace('#', ',');
}

class Retiro : Operacion
{
    private string numeroCuenta;
    public Retiro(string numeroCuenta, decimal monto)
    {
        this.numeroCuenta = numeroCuenta;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(numeroCuenta);
        if (cuenta != null && cuenta.Extraer(Monto))
        {
            cuenta.AgregarOperacion($"Retiro $ {Formato(Monto)} de [{cuenta.Numero}/{cuenta.Titular.Nombre}]");
        }
    }

    private string Formato(decimal monto) =>
        monto.ToString("N2").Replace('.', '#').Replace(',', '.').Replace('#', ',');
}

class Pago : Operacion
{
    private string numeroCuenta;
    public Pago(string numeroCuenta, decimal monto)
    {
        this.numeroCuenta = numeroCuenta;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(numeroCuenta);
        if (cuenta != null && cuenta.Pagar(Monto))
        {
            cuenta.AgregarOperacion($"Pago $ {Formato(Monto)} con [{cuenta.Numero}/{cuenta.Titular.Nombre}]");
        }
    }

    private string Formato(decimal monto) =>
        monto.ToString("N2").Replace('.', '#').Replace(',', '.').Replace('#', ',');
}

class Transferencia : Operacion
{
    private string origen;
    private string destino;
    public Transferencia(string origen, string destino, decimal monto)
    {
        this.origen = origen;
        this.destino = destino;
        Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuentaOrigen = banco.ObtenerCuenta(origen);
        var cuentaDestino = banco.BuscarCuentaEnBancos(destino);

        if (cuentaOrigen != null && cuentaDestino != null && cuentaOrigen.Transferir(Monto, cuentaDestino))
        {
            string desc = $"Transferencia $ {Formato(Monto)} de [{cuentaOrigen.Numero}/{cuentaOrigen.Titular.Nombre}] a [{cuentaDestino.Numero}/{cuentaDestino.Titular.Nombre}]";
            cuentaOrigen.AgregarOperacion(desc);
            cuentaDestino.AgregarOperacion(desc);
        }
    }

    private string Formato(decimal monto) =>
        monto.ToString("N2").Replace('.', '#').Replace(',', '.').Replace('#', ',');
}

abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; } = 0;
    public Cliente Titular { get; }
    private List<string> historial = new();

    public Cuenta(string numero, decimal saldo, Cliente titular)
    {
        Numero = numero;
        Saldo = saldo;
        Titular = titular;
    }

    public void Depositar(decimal monto) => Saldo += monto;
    public bool Extraer(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public bool Pagar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            CalcularPuntos(monto);
            return true;
        }
        return false;
    }

    public bool Transferir(decimal monto, Cuenta destino)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            destino.Saldo += monto;
            return true;
        }
        return false;
    }

    public void AgregarOperacion(string descripcion) => historial.Add(descripcion);

    public void MostrarHistorial()
    {
        foreach (var linea in historial)
        {
            Console.WriteLine("     -  " + linea);
        }
    }

    public virtual void CalcularPuntos(decimal monto) { }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldo, Cliente titular) : base(numero, saldo, titular) { }

    public override void CalcularPuntos(decimal monto)
    {
        Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldo, Cliente titular) : base(numero, saldo, titular) { }

    public override void CalcularPuntos(decimal monto) => Puntos += monto * 0.02m;
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldo, Cliente titular) : base(numero, saldo, titular) { }

    public override void CalcularPuntos(decimal monto) => Puntos += monto * 0.01m;
}

class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; } = new();
    public Cliente(string nombre) => Nombre = nombre;
    public void Agregar(Cuenta cuenta) => Cuentas.Add(cuenta);
    public decimal SaldoTotal => Cuentas.Sum(c => c.Saldo);
    public decimal PuntosTotal => Cuentas.Sum(c => c.Puntos);
}

class Banco
{
    public string Nombre { get; }
    private List<Cliente> clientes = new();
    private static List<Banco> bancosGlobal = new();

    public Banco(string nombre)
    {
        Nombre = nombre;
        bancosGlobal.Add(this);
    }

    public void Agregar(Cliente cliente) => clientes.Add(cliente);
    public void Registrar(Operacion op) => op.Ejecutar(this);

    public Cuenta? ObtenerCuenta(string numero) =>
        clientes.SelectMany(c => c.Cuentas).FirstOrDefault(cta => cta.Numero == numero);

    public Cuenta? BuscarCuentaEnBancos(string numero) =>
        bancosGlobal.SelectMany(b => b.clientes).SelectMany(c => c.Cuentas).FirstOrDefault(cta => cta.Numero == numero);

    public void Informe()
    {
        var encabezado = Nombre == "Banco Nac"
            ? $"Banco: {Nombre} | Clientes: {clientes.Count}"
            : $"Banco : {Nombre} | Clientes: {clientes.Count}";

        Console.WriteLine("\n" + encabezado + "\n");

        foreach (var cliente in clientes)
        {
            string saldo = Formato(cliente.SaldoTotal);
            string puntos = Formato(cliente.PuntosTotal);
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {saldo} | Puntos Total: $ {puntos}\n");

            foreach (var cuenta in cliente.Cuentas)
            {
                string saldoCta = Formato(cuenta.Saldo);
                string puntosCta = Formato(cuenta.Puntos);
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {saldoCta} | Puntos: $ {puntosCta}");
                cuenta.MostrarHistorial();
                Console.WriteLine();
            }
        }
    }

    private string Formato(decimal monto) =>
        monto.ToString("N2").Replace('.', '#').Replace(',', '.').Replace('#', ',');
}




var raul = new Cliente("Raul Perez");
raul.Agregar(new CuentaOro("10001", 1000, raul));
raul.Agregar(new CuentaPlata("10002", 2000, raul));

var sara = new Cliente("Sara Lopez");
sara.Agregar(new CuentaPlata("10003", 3000, sara));
sara.Agregar(new CuentaPlata("10004", 4000, sara));

var luis = new Cliente("Luis Gomez");
luis.Agregar(new CuentaBronce("10005", 5000, luis));

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
