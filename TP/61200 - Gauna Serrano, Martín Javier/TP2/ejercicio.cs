// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

class Banco{}
class Cliente{}

abstract class Cuenta{}
class CuentaOro: Cuenta{}
class CuentaPlata: Cuenta{}
class CuentaBronce: Cuenta{}

abstract class Operacion{}
class Deposito: Operacion{}
class Retiro: Operacion{}
class Transferencia: Operacion{}
class Pago: Operacion{}


/// EJEMPLO DE USO ///

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
using System;
using System.Collections.Generic;
using System.Linq;

// Cuenta base
abstract class CuentaBase
{
    public string Numero { get; }
    public double Saldo { get; protected set; }
    public double Puntos { get; protected set; }

    public CuentaBase(string numero) => Numero = numero;

    public virtual void Depositar(double monto) => Saldo += monto;

    public virtual bool Extraer(double monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public bool Pagar(double monto)
    {
        if (Extraer(monto))
        {
            AcumularPuntos(monto);
            return true;
        }
        return false;
    }

    protected abstract void AcumularPuntos(double monto);
}

class CuentaOro : CuentaBase
{
    public CuentaOro(string numero) : base(numero) { }

    protected override void AcumularPuntos(double monto)
        => Puntos += monto > 1000 ? monto * 0.05 : monto * 0.03;
}

class CuentaPlata : CuentaBase
{
    public CuentaPlata(string numero) : base(numero) { }

    protected override void AcumularPuntos(double monto)
        => Puntos += monto * 0.02;
}

class CuentaBronce : CuentaBase
{
    public CuentaBronce(string numero) : base(numero) { }

    protected override void AcumularPuntos(double monto)
        => Puntos += monto * 0.01;
}

class Cliente
{
    public string Nombre { get; }
    public List<CuentaBase> Cuentas { get; } = new();
    public List<string> Historial { get; } = new();

    public Cliente(string nombre) => Nombre = nombre;

    public void AgregarCuenta(CuentaBase cuenta) => Cuentas.Add(cuenta);
    public void RegistrarOperacion(string detalle) => Historial.Add(detalle);
}

class Banco
{
    private List<Cliente> clientes = new();
    private List<string> operaciones = new();

    public void AgregarCliente(Cliente cliente) => clientes.Add(cliente);

    private CuentaBase? BuscarCuenta(string numero) =>
        clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numero);

    private Cliente? BuscarClientePorCuenta(string numero) =>
        clientes.FirstOrDefault(c => c.Cuentas.Any(cuenta => cuenta.Numero == numero));

    private void Registrar(string detalle, string nroCuenta)
    {
        operaciones.Add(detalle);
        var cliente = BuscarClientePorCuenta(nroCuenta);
        if (cliente != null) cliente.RegistrarOperacion(detalle);
    }

    public void Depositar(string nroCuenta, double monto)
    {
        var cuenta = BuscarCuenta(nroCuenta);
        if (cuenta != null)
        {
            cuenta.Depositar(monto);
            Registrar($"DEPÓSITO de ${monto} en {nroCuenta}", nroCuenta);
        }
    }

    public void Extraer(string nroCuenta, double monto)
    {
        var cuenta = BuscarCuenta(nroCuenta);
        if (cuenta != null && cuenta.Extraer(monto))
            Registrar($"RETIRO de ${monto} en {nroCuenta}", nroCuenta);
    }

    public void Pagar(string nroCuenta, double monto)
    {
        var cuenta = BuscarCuenta(nroCuenta);
        if (cuenta != null && cuenta.Pagar(monto))
            Registrar($"PAGO de ${monto} en {nroCuenta} (Puntos: {cuenta.Puntos})", nroCuenta);
    }

    public void Transferir(string origen, string destino, double monto)
    {
        var ctaOrigen = BuscarCuenta(origen);
        var ctaDestino = BuscarCuenta(destino);
        if (ctaOrigen != null && ctaDestino != null && ctaOrigen.Extraer(monto))
        {
            ctaDestino.Depositar(monto);
            Registrar($"TRANSFERENCIA de ${monto} de {origen} a {destino}", origen);
            Registrar($"TRANSFERENCIA recibida de ${monto} desde {origen}", destino);
        }
    }

    public void Reporte()
    {
        Console.WriteLine("---- Operaciones Globales ----");
        operaciones.ForEach(Console.WriteLine);

        foreach (var cli in clientes)
        {
            Console.WriteLine($"\nCliente: {cli.Nombre}");
            foreach (var c in cli.Cuentas)
                Console.WriteLine($"Cuenta {c.Numero} | Saldo: ${c.Saldo} | Puntos: {c.Puntos}");
            Console.WriteLine("Historial:");
            cli.Historial.ForEach(op => Console.WriteLine(" - " + op));
        }
    }
}

class Program
{
    static void Main()
    {
        Banco banco = new();

        var juan = new Cliente("Juan");
        juan.AgregarCuenta(new CuentaOro("00001"));
        banco.AgregarCliente(juan);

        var ana = new Cliente("Ana");
        ana.AgregarCuenta(new CuentaPlata("00002"));
        banco.AgregarCliente(ana);

        banco.Depositar("00001", 2000);
        banco.Pagar("00001", 1500);
        banco.Transferir("00001", "00002", 300);
        banco.Extraer("00002", 100);

        banco.Reporte();
    }
}

