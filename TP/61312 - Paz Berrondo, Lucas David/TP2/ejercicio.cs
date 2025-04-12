using System;
using System.Collections.Generic;
using System.Globalization;

abstract class Operacion
{
    public decimal Monto { get; protected set; }
    public abstract string Tipo { get; }
    public abstract string Mensaje();
    public abstract bool Ejecutar();
}

// ---------------------------
// Clases de Operaciones
// ---------------------------

class Deposito : Operacion
{
    public string CuentaDestino { get; private set; }
    public override string Tipo => "Deposito";

    public Deposito(string cuentaDestino, decimal monto)
    {
        CuentaDestino = cuentaDestino;
        Monto = monto;
    }

    public override string Mensaje()
    {
        var cuenta = Banco.ObtenerCuenta(CuentaDestino);
        string cliente = cuenta?.Propietario?.Nombre ?? "Desconocido";
        return $"-  {Tipo} {Formatear(Monto)} a [{CuentaDestino}/{cliente}]";
    }

    public override bool Ejecutar()
    {
        var cuenta = Banco.ObtenerCuenta(CuentaDestino);
        if (cuenta == null) return false;
        cuenta.Depositar(Monto);
        cuenta.Operaciones.Add(this);
        return true;
    }

    private string Formatear(decimal valor)
    {
        return "$ " + valor.ToString("N2", new CultureInfo("es-ES"));
    }
}

class Retiro : Operacion
{
    public string CuentaOrigen { get; private set; }
    public override string Tipo => "Retiro";

    public Retiro(string cuentaOrigen, decimal monto)
    {
        CuentaOrigen = cuentaOrigen;
        Monto = monto;
    }

    public override string Mensaje()
    {
        var cuenta = Banco.ObtenerCuenta(CuentaOrigen);
        string cliente = cuenta?.Propietario?.Nombre ?? "Desconocido";
        return $"-  {Tipo} {Formatear(Monto)} de [{CuentaOrigen}/{cliente}]";
    }

    public override bool Ejecutar()
    {
        var cuenta = Banco.ObtenerCuenta(CuentaOrigen);
        if (cuenta == null) return false;
        bool exito = cuenta.Extraer(Monto);
        if (exito) cuenta.Operaciones.Add(this);
        return exito;
    }

    private string Formatear(decimal valor)
    {
        return "$ " + valor.ToString("N2", new CultureInfo("es-ES"));
    }
}

class Pago : Operacion
{
    public string CuentaOrigen { get; private set; }
    public override string Tipo => "Pago";

    public Pago(string cuentaOrigen, decimal monto)
    {
        CuentaOrigen = cuentaOrigen;
        Monto = monto;
    }

    public override string Mensaje()
    {
        var cuenta = Banco.ObtenerCuenta(CuentaOrigen);
        string cliente = cuenta?.Propietario?.Nombre ?? "Desconocido";
        return $"-  {Tipo} {Formatear(Monto)} con [{CuentaOrigen}/{cliente}]";
    }

    public override bool Ejecutar()
    {
        var cuenta = Banco.ObtenerCuenta(CuentaOrigen);
        if (cuenta == null) return false;
        bool exito = cuenta.Pagar(Monto);
        if (exito) cuenta.Operaciones.Add(this);
        return exito;
    }

    private string Formatear(decimal valor)
    {
        return "$ " + valor.ToString("N2", new CultureInfo("es-ES"));
    }
}

class Transferencia : Operacion
{
    public string CuentaOrigen { get; private set; }
    public string CuentaDestino { get; private set; }
    public override string Tipo => "Transferencia";

    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto)
    {
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
        Monto = monto;
    }

    public override string Mensaje()
    {
        var origen = Banco.ObtenerCuenta(CuentaOrigen);
        var destino = Banco.ObtenerCuenta(CuentaDestino);
        string clienteOrigen = origen?.Propietario?.Nombre ?? "Desconocido";
        string clienteDestino = destino?.Propietario?.Nombre ?? "Desconocido";
        return $"-  {Tipo} {Formatear(Monto)} de [{CuentaOrigen}/{clienteOrigen}] a [{CuentaDestino}/{clienteDestino}]";
    }

    public override bool Ejecutar()
    {
        var origen = Banco.ObtenerCuenta(CuentaOrigen);
        var destino = Banco.ObtenerCuenta(CuentaDestino);
        if (origen == null || destino == null) return false;
        bool exito = origen.Transferir(Monto, destino);
        if (exito)
        {
            origen.Operaciones.Add(this);
            destino.Operaciones.Add(this);
        }
        return exito;
    }

    private string Formatear(decimal valor)
    {
        return "$ " + valor.ToString("N2", new CultureInfo("es-ES"));
    }
}

// ---------------------------
// Cuentas
// ---------------------------

abstract class Cuenta
{
    public string Numero { get; private set; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }
    public List<Operacion> Operaciones { get; private set; }
    public Cliente Propietario { get; set; }

    public Cuenta(string numero, decimal saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Puntos = 0;
        Operaciones = new List<Operacion>();
        Banco.RegistrarCuentaGlobal(this);
    }

    public abstract decimal CalcularPuntos(decimal monto);

    public virtual void Depositar(decimal monto)
    {
        Saldo += monto;
    }

    public virtual bool Extraer(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public virtual bool Pagar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            Puntos += CalcularPuntos(monto);
            return true;
        }
        return false;
    }

    public virtual bool Transferir(decimal monto, Cuenta destino)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            destino.Depositar(monto);
            return true;
        }
        return false;
    }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override decimal CalcularPuntos(decimal monto) =>
        monto > 1000 ? monto * 0.05m : monto * 0.03m;
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override decimal CalcularPuntos(decimal monto) => monto * 0.02m;
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override decimal CalcularPuntos(decimal monto) => monto * 0.01m;
}

// ---------------------------
// Cliente
// ---------------------------

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
        cuenta.Propietario = this;
        Cuentas.Add(cuenta);
    }

    public decimal SaldoTotal => Cuentas.Sum(c => c.Saldo);
    public decimal PuntosTotal => Cuentas.Sum(c => c.Puntos);
}

// ---------------------------
// Banco
// ---------------------------

class Banco
{
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; }
    private List<Operacion> OperacionesGlobales;

    private static Dictionary<string, Cuenta> CuentasGlobal = new();

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        OperacionesGlobales = new List<Operacion>();
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public static void RegistrarCuentaGlobal(Cuenta cuenta)
    {
        if (!CuentasGlobal.ContainsKey(cuenta.Numero))
            CuentasGlobal.Add(cuenta.Numero, cuenta);
    }

    public static Cuenta ObtenerCuenta(string numero)
    {
        CuentasGlobal.TryGetValue(numero, out var cuenta);
        return cuenta;
    }

    public void Registrar(Operacion op)
    {
        var cuentasInvolucradas = new List<string>();

        if (op is Deposito d) cuentasInvolucradas.Add(d.CuentaDestino);
        else if (op is Retiro r) cuentasInvolucradas.Add(r.CuentaOrigen);
        else if (op is Pago p) cuentasInvolucradas.Add(p.CuentaOrigen);
        else if (op is Transferencia t)
        {
            cuentasInvolucradas.Add(t.CuentaOrigen);
            cuentasInvolucradas.Add(t.CuentaDestino);
        }

        foreach (var numero in cuentasInvolucradas)
        {
            var cuenta = ObtenerCuenta(numero);
            if (cuenta == null || !PerteneceAlBanco(cuenta))
            {
                Console.WriteLine($"Error: La cuenta {numero} no pertenece al banco {Nombre} o no existe.");
                return;
            }
        }

        bool exito = op.Ejecutar();
        if (exito)
        {
            OperacionesGlobales.Add(op);
        }
        else
        {
            Console.WriteLine($"Error al ejecutar la operación: {op.Mensaje()}");
        }
    }

    private bool PerteneceAlBanco(Cuenta cuenta)
    {
        foreach (var cliente in Clientes)
            if (cliente.Cuentas.Contains(cuenta)) return true;
        return false;
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}\n");
        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: {Formatear(cliente.SaldoTotal)} | Puntos Total: {Formatear(cliente.PuntosTotal)}\n");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: {Formatear(cuenta.Saldo)} | Puntos: {Formatear(cuenta.Puntos)}");
                foreach (var op in cuenta.Operaciones)
                {
                    Console.WriteLine("     " + op.Mensaje());
                }
                Console.WriteLine();
            }
        }
    }

    private string Formatear(decimal valor) =>
        valor.ToString("N2", new CultureInfo("es-ES"));
}

// ---------------------------
// Programa Principal
// ---------------------------

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
