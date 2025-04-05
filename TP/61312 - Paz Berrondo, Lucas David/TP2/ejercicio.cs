using System;
using System.Collections.Generic;
using System.Globalization;
abstract class Operacion
{
    public decimal Monto { get; protected set; }
    public abstract string Tipo { get; }
    public abstract string Mensaje();
}

// Operación de Depósito
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

    private string Formatear(decimal valor)
    {
        return "$ " + valor.ToString("N2", new CultureInfo("es-ES"));
    }
}

// Operación de Retiro
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

    private string Formatear(decimal valor)
    {
        return "$ " + valor.ToString("N2", new CultureInfo("es-ES"));
    }
}

// Operación de Pago
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

    private string Formatear(decimal valor)
    {
        return "$ " + valor.ToString("N2", new CultureInfo("es-ES"));
    }
}

// Operación de Transferencia
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

    private string Formatear(decimal valor)
    {
        return "$ " + valor.ToString("N2", new CultureInfo("es-ES"));
    }
}

// Clase abstracta para Cuentas
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

    // Calcular puntos al realizar un pago.
    public abstract decimal CalcularPuntos(decimal monto);

    // Métodos para las operaciones
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
            decimal puntosGanados = CalcularPuntos(monto);
            Puntos += puntosGanados;
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

// Cuenta Oro: Acumula 5% para pagos > 1000, 3% para montos menores.
class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override decimal CalcularPuntos(decimal monto)
    {
        if (monto > 1000)
            return monto * 0.05m;
        else
            return monto * 0.03m;
    }
}

// Cuenta Plata: Acumula 2% en pagos.
class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override decimal CalcularPuntos(decimal monto)
    {
        return monto * 0.02m;
    }
}

// Cuenta Bronce: Acumula 1% en pagos.
class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    public override decimal CalcularPuntos(decimal monto)
    {
        return monto * 0.01m;
    }
}

// Clase Cliente
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

    public decimal SaldoTotal
    {
        get
        {
            decimal total = 0;
            foreach (var cuenta in Cuentas)
            {
                total += cuenta.Saldo;
            }
            return total;
        }
    }

    public decimal PuntosTotal
    {
        get
        {
            decimal total = 0;
            foreach (var cuenta in Cuentas)
            {
                total += cuenta.Puntos;
            }
            return total;
        }
    }
}

// Clase Banco
class Banco
{
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; }
    private List<Operacion> OperacionesGlobales;

    // Diccionario global para acceder a cuentas por número
    private static Dictionary<string, Cuenta> CuentasGlobal = new Dictionary<string, Cuenta>();

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        OperacionesGlobales = new List<Operacion>();
    }

    // Agregar un cliente al banco
    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    // Registrar una cuenta en el diccionario global
    public static void RegistrarCuentaGlobal(Cuenta cuenta)
    {
        if (!CuentasGlobal.ContainsKey(cuenta.Numero))
        {
            CuentasGlobal.Add(cuenta.Numero, cuenta);
        }
        else
        {
            throw new Exception($"La cuenta {cuenta.Numero} ya está registrada.");
        }
    }

    // Obtencion de una cuenta desde el diccionario global
    public static Cuenta ObtenerCuenta(string numero)
    {
        if (CuentasGlobal.ContainsKey(numero))
            return CuentasGlobal[numero];
        return null;
    }

    // Registrar operación
    public void Registrar(Operacion op)
    {
        string cuentaVerificar = "";
        if (op is Deposito)
        {
            cuentaVerificar = ((Deposito)op).CuentaDestino;
        }
        else if (op is Retiro)
        {
            cuentaVerificar = ((Retiro)op).CuentaOrigen;
        }
        else if (op is Pago)
        {
            cuentaVerificar = ((Pago)op).CuentaOrigen;
        }
        else if (op is Transferencia)
        {
            cuentaVerificar = ((Transferencia)op).CuentaOrigen;
        }

        var cuentaOrigen = ObtenerCuenta(cuentaVerificar);
        if (cuentaOrigen == null)
        {
            Console.WriteLine($"Error: La cuenta {cuentaVerificar} no existe.");
            return;
        }
        bool pertenece = false;
        foreach (var cliente in Clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta == cuentaOrigen)
                {
                    pertenece = true;
                    break;
                }
            }
            if (pertenece) break;
        }
        if (!pertenece)
        {
            Console.WriteLine($"Error: La cuenta {cuentaVerificar} no pertenece al banco {Nombre}.");
            return;
        }

        bool exito = false;
        if (op is Deposito)
        {
            cuentaOrigen.Depositar(op.Monto);
            exito = true;
        }
        else if (op is Retiro)
        {
            exito = cuentaOrigen.Extraer(op.Monto);
            if (!exito)
                Console.WriteLine($"Error: Fondos insuficientes en la cuenta {cuentaOrigen.Numero} para retirar {op.Monto}.");
        }
        else if (op is Pago)
        {
            exito = cuentaOrigen.Pagar(op.Monto);
            if (!exito)
                Console.WriteLine($"Error: Fondos insuficientes en la cuenta {cuentaOrigen.Numero} para pagar {op.Monto}.");
        }
        else if (op is Transferencia)
        {
            var opTrans = (Transferencia)op;
            var cuentaDestino = ObtenerCuenta(opTrans.CuentaDestino);
            if (cuentaDestino == null)
            {
                Console.WriteLine($"Error: La cuenta destino {opTrans.CuentaDestino} no existe.");
            }
            else
            {
                exito = cuentaOrigen.Transferir(op.Monto, cuentaDestino);
                if (!exito)
                    Console.WriteLine($"Error: Fondos insuficientes en la cuenta {cuentaOrigen.Numero} para transferir {op.Monto}.");
                else
                {
                    cuentaDestino.Operaciones.Add(op);
                }
            }
        }

        if (exito)
        {
            OperacionesGlobales.Add(op);
            cuentaOrigen.Operaciones.Add(op);
        }
    }

    // Informe final del banco
    public void Informe()
    {
        Console.WriteLine();
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}");
        Console.WriteLine();
        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: {Formatear(cliente.SaldoTotal)} | Puntos Total: {Formatear(cliente.PuntosTotal)}");
            Console.WriteLine();
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

    private string Formatear(decimal valor)
    {
        return valor.ToString("N2", new CultureInfo("es-ES"));
    }
}

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

// Ejecutar operaciones
nac.Registrar(new Deposito("10001", 100));
nac.Registrar(new Retiro("10002", 200));
nac.Registrar(new Transferencia("10001", "10002", 300));
nac.Registrar(new Transferencia("10003", "10004", 500));
nac.Registrar(new Pago("10002", 400));

tup.Registrar(new Deposito("10005", 100));
tup.Registrar(new Retiro("10005", 200));
tup.Registrar(new Transferencia("10005", "10002", 300));
tup.Registrar(new Pago("10005", 400));

// Generar informe
nac.Informe();
tup.Informe();