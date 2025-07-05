// TP2: Sistema de Cuentas Bancarias

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

// Enumeración para tipos de cuenta
public enum TipoCuenta
{
    Oro,
    Plata,
    Bronce
}

// Excepciones personalizadas
public class OperacionBancariaException : Exception
{
    public OperacionBancariaException(string message) : base(message) { }
}

public class SaldoInsuficienteException : OperacionBancariaException
{
    public SaldoInsuficienteException() : base("Saldo insuficiente para realizar la operación") { }
}

public class CuentaNoEncontradaException : OperacionBancariaException
{
    public CuentaNoEncontradaException(string numeroCuenta) 
        : base($"No se encontró la cuenta {numeroCuenta}") { }
}

public class CuentaNoPerteneceAlBancoException : OperacionBancariaException
{
    public CuentaNoPerteneceAlBancoException(string numeroCuenta)
        : base($"La cuenta {numeroCuenta} no pertenece a este banco") { }
}

// Clase abstracta para operaciones bancarias
public abstract class Operacion
{
    public string NumeroCuenta { get; }
    public decimal Monto { get; }
    public DateTime Fecha { get; }
    public Guid Id { get; }

    protected Operacion(string numeroCuenta, decimal monto)
    {
        if (string.IsNullOrWhiteSpace(numeroCuenta))
            throw new ArgumentException("El número de cuenta no puede estar vacío");

        if (monto <= 0)
            throw new ArgumentException("El monto debe ser mayor que cero");

        NumeroCuenta = numeroCuenta;
        Monto = monto;
        Fecha = DateTime.Now;
        Id = Guid.NewGuid();
    }

    public abstract void Ejecutar(Cuenta cuenta);

    public virtual string ObtenerDetalle()
    {
        return $"{Fecha:yyyy-MM-dd HH:mm:ss} - {GetType().Name} en cuenta {NumeroCuenta}: {Monto:C}";
    }

    public virtual Dictionary<string, string> ObtenerDatosReporte()
    {
        return new Dictionary<string, string>
        {
            {"Tipo", GetType().Name},
            {"Fecha", Fecha.ToString("yyyy-MM-dd HH:mm:ss")},
            {"Cuenta", NumeroCuenta},
            {"Monto", Monto.ToString("C")}
        };
    }
}

// Implementaciones concretas de operaciones
public class Deposito : Operacion
{
    public Deposito(string numeroCuenta, decimal monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Cuenta cuenta)
    {
        cuenta.AumentarSaldo(Monto);
    }
}

public class Retiro : Operacion
{
    public Retiro(string numeroCuenta, decimal monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Cuenta cuenta)
    {
        if (cuenta.ObtenerSaldo() < Monto)
            throw new SaldoInsuficienteException();

        cuenta.DisminuirSaldo(Monto);
    }
}

public class Pago : Operacion
{
    private Cuenta _cuentaAsociada;

    public Pago(string numeroCuenta, decimal monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Cuenta cuenta)
    {
        if (cuenta.ObtenerSaldo() < Monto)
            throw new SaldoInsuficienteException();

        _cuentaAsociada = cuenta; // Guardamos la cuenta para usarla en el reporte
        cuenta.DisminuirSaldo(Monto);
        cuenta.AcumularPuntos(this);
    }

    public override Dictionary<string, string> ObtenerDatosReporte()
    {
        var datos = base.ObtenerDatosReporte();
        if (_cuentaAsociada != null)
        {
            datos.Add("PuntosAcumulados", ((int)(Monto * _cuentaAsociada.ObtenerPorcentajePuntos())).ToString());
        }
        else
        {
            datos.Add("PuntosAcumulados", "0");
        }
        return datos;
    }
}

public class Transferencia : Operacion
{
    public string NumeroCuentaDestino { get; }
    public Banco BancoDestino { get; private set; }

    public Transferencia(string numeroCuentaOrigen, string numeroCuentaDestino, decimal monto) 
        : base(numeroCuentaOrigen, monto)
    {
        if (string.IsNullOrWhiteSpace(numeroCuentaDestino))
            throw new ArgumentException("El número de cuenta destino no puede estar vacío");

        if (numeroCuentaOrigen == numeroCuentaDestino)
            throw new ArgumentException("No se puede transferir a la misma cuenta de origen");

        NumeroCuentaDestino = numeroCuentaDestino;
    }

    public void EstablecerBancoDestino(Banco banco)
    {
        BancoDestino = banco ?? throw new ArgumentNullException(nameof(banco));
    }

    public override void Ejecutar(Cuenta cuentaOrigen)
    {
        if (cuentaOrigen.ObtenerSaldo() < Monto)
            throw new SaldoInsuficienteException();
        
        cuentaOrigen.DisminuirSaldo(Monto);
    }

    public override string ObtenerDetalle()
    {
        return $"{Fecha:yyyy-MM-dd HH:mm:ss} - Transferencia de {Monto:C} desde cuenta {NumeroCuenta} a {NumeroCuentaDestino}";
    }

    public override Dictionary<string, string> ObtenerDatosReporte()
    {
        var datos = base.ObtenerDatosReporte();
        datos.Add("CuentaDestino", NumeroCuentaDestino);
        datos.Add("BancoDestino", BancoDestino?.Nombre ?? "Desconocido");
        return datos;
    }
}

// Clase abstracta para cuentas bancarias
public abstract class Cuenta
{
    public string Numero { get; }
    private decimal _saldo;
    public int Puntos { get; protected set; }
    public TipoCuenta Tipo { get; protected set; }
    public List<Operacion> Historial { get; } = new List<Operacion>();
    public Cliente Titular { get; private set; }
    public Banco Banco { get; private set; }

    protected Cuenta(string numero, decimal saldoInicial)
    {
        if (string.IsNullOrWhiteSpace(numero) || numero.Length != 5 || !numero.All(char.IsDigit))
            throw new ArgumentException("El número de cuenta debe tener exactamente 5 dígitos");

        if (saldoInicial < 0)
            throw new ArgumentException("El saldo inicial no puede ser negativo");

        Numero = numero;
        _saldo = saldoInicial;
    }

    public decimal ObtenerSaldo() => _saldo;
    public void AumentarSaldo(decimal monto) => _saldo += monto;
    public void DisminuirSaldo(decimal monto) => _saldo -= monto;

    public void AsignarTitular(Cliente titular)
    {
        Titular = titular ?? throw new ArgumentNullException(nameof(titular));
    }

    public void AsignarBanco(Banco banco)
    {
        Banco = banco ?? throw new ArgumentNullException(nameof(banco));
    }

    public virtual void RegistrarOperacion(Operacion operacion)
    {
        if (operacion == null)
            throw new ArgumentNullException(nameof(operacion));

        try
        {
            operacion.Ejecutar(this);
            Historial.Add(operacion);
            Titular?.RegistrarOperacion(operacion);
            Banco?.RegistrarOperacionGlobal(operacion);
        }
        catch (OperacionBancariaException)
        {
            Historial.Add(operacion);
            Titular?.RegistrarOperacion(operacion);
            Banco?.RegistrarOperacionGlobal(operacion);
            throw;
        }
    }

    public abstract void AcumularPuntos(Pago pago);
    
    public abstract decimal ObtenerPorcentajePuntos();

    public virtual string ObtenerResumen()
    {
        return $"Cuenta {Numero} ({Tipo}): Saldo {_saldo:C}, Puntos acumulados: {Puntos}";
    }

    public virtual IEnumerable<Dictionary<string, string>> ObtenerHistorialParaReporte()
    {
        return Historial.OrderByDescending(o => o.Fecha)
                      .Select(o => o.ObtenerDatosReporte());
    }
}

// Implementaciones concretas de cuentas
public class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial)
    {
        Tipo = TipoCuenta.Oro;
    }

    public override void AcumularPuntos(Pago pago)
    {
        if (pago.Monto > 1000)
            Puntos += (int)(pago.Monto * 0.05m);
        else
            Puntos += (int)(pago.Monto * 0.03m);
    }

    public override decimal ObtenerPorcentajePuntos()
    {
        return ObtenerSaldo() > 10000 ? 0.05m : 0.03m;
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial)
    {
        Tipo = TipoCuenta.Plata;
    }

    public override void AcumularPuntos(Pago pago)
    {
        Puntos += (int)(pago.Monto * 0.02m);
    }

    public override decimal ObtenerPorcentajePuntos()
    {
        return 0.02m;
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial)
    {
        Tipo = TipoCuenta.Bronce;
    }

    public override void AcumularPuntos(Pago pago)
    {
        Puntos += (int)(pago.Monto * 0.01m);
    }

    public override decimal ObtenerPorcentajePuntos()
    {
        return 0.01m;
    }
}

// Clase Cliente
public class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; } = new List<Cuenta>();
    public List<Operacion> Historial { get; } = new List<Operacion>();

    public Cliente(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del cliente no puede estar vacío");

        Nombre = nombre;
    }

    public void AgregarCuenta(Cuenta cuenta)
    {
        if (cuenta == null)
            throw new ArgumentNullException(nameof(cuenta));

        if (Cuentas.Any(c => c.Numero == cuenta.Numero))
            throw new ArgumentException($"Ya existe una cuenta con el número {cuenta.Numero}");

        cuenta.AsignarTitular(this);
        Cuentas.Add(cuenta);
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        Historial.Add(operacion);
    }

    public string ObtenerResumen()
    {
        return $"Cliente: {Nombre}, Cuentas: {Cuentas.Count}, Saldo total: {Cuentas.Sum(c => c.ObtenerSaldo()):C}";
    }

    public IEnumerable<Dictionary<string, string>> ObtenerHistorialParaReporte()
    {
        return Historial.OrderByDescending(o => o.Fecha)
                      .Select(o => o.ObtenerDatosReporte());
    }
}

// Clase Banco
public class Banco : IDisposable
{
    public string Nombre { get; }
    private readonly List<Cliente> _clientes = new List<Cliente>();
    private readonly List<Operacion> _historialGlobal = new List<Operacion>();
    private readonly Dictionary<string, Cuenta> _cuentas = new Dictionary<string, Cuenta>();

    public Banco(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del banco no puede estar vacío");

        Nombre = nombre;
    }

    public void AgregarCliente(Cliente cliente)
    {
        if (cliente == null)
            throw new ArgumentNullException(nameof(cliente));

        if (_clientes.Any(c => c.Nombre.Equals(cliente.Nombre, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException($"Ya existe un cliente con el nombre {cliente.Nombre}");

        _clientes.Add(cliente);
        
        foreach (var cuenta in cliente.Cuentas)
        {
            if (_cuentas.ContainsKey(cuenta.Numero))
                throw new ArgumentException($"La cuenta {cuenta.Numero} ya existe en el banco");

            cuenta.AsignarBanco(this);
            _cuentas[cuenta.Numero] = cuenta;
        }
    }

    public void RegistrarOperacionGlobal(Operacion operacion)
    {
        _historialGlobal.Add(operacion);
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        if (operacion == null)
            throw new ArgumentNullException(nameof(operacion));

        if (!_cuentas.TryGetValue(operacion.NumeroCuenta, out var cuenta))
            throw new CuentaNoEncontradaException(operacion.NumeroCuenta);

        if (cuenta.Banco != this)
            throw new CuentaNoPerteneceAlBancoException(operacion.NumeroCuenta);

        try
        {
            if (operacion is Transferencia transferencia)
            {
                RegistrarTransferencia(transferencia, cuenta);
            }
            else
            {
                cuenta.RegistrarOperacion(operacion);
            }
        }
        catch (OperacionBancariaException)
        {
            throw;
        }
    }

    private void RegistrarTransferencia(Transferencia transferencia, Cuenta cuentaOrigen)
    {
        // Buscar cuenta destino en este banco primero
        if (_cuentas.TryGetValue(transferencia.NumeroCuentaDestino, out var cuentaDestino))
        {
            using (var transaction = new TransactionScope())
            {
                cuentaOrigen.RegistrarOperacion(transferencia);
                cuentaDestino.AumentarSaldo(transferencia.Monto);
                cuentaDestino.Historial.Add(transferencia);
                cuentaDestino.Titular?.RegistrarOperacion(transferencia);

                transaction.Complete();
            }
        }
        else
        {
            cuentaOrigen.RegistrarOperacion(transferencia);
        }
    }

    public void GenerarReporteCompleto()
    {
        Console.WriteLine($"\n=== REPORTE COMPLETO - {Nombre.ToUpper()} ===");
        Console.WriteLine($"Fecha generación: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        
        GenerarReporteOperacionesGlobales();
        GenerarReporteEstadosCuenta();
        GenerarReportesIndividualesClientes();
    }

    private void GenerarReporteOperacionesGlobales()
    {
        Console.WriteLine("\n--- OPERACIONES GLOBALES ---");
        Console.WriteLine($"Total operaciones: {_historialGlobal.Count}");
        
        var agrupadasPorTipo = _historialGlobal
            .GroupBy(o => o.GetType().Name)
            .OrderByDescending(g => g.Count());

        foreach (var grupo in agrupadasPorTipo)
        {
            Console.WriteLine($"- {grupo.Key}: {grupo.Count()} operaciones, Total: {grupo.Sum(o => o.Monto):C}");
        }

        Console.WriteLine("\nÚltimas 10 operaciones:");
        foreach (var op in _historialGlobal.OrderByDescending(o => o.Fecha).Take(10))
        {
            var datos = op.ObtenerDatosReporte();
            Console.WriteLine($"  {datos["Fecha"]} - {datos["Tipo"]} ({datos["Monto"]})");
            if (op is Transferencia)
                Console.WriteLine($"    Desde: {datos["Cuenta"]}, A: {datos["CuentaDestino"]}");
        }
    }

    private void GenerarReporteEstadosCuenta()
    {
        Console.WriteLine("\n--- ESTADO DE CUENTAS ---");
        Console.WriteLine($"Total cuentas: {_cuentas.Count}");
        Console.WriteLine($"Saldo total: {_cuentas.Values.Sum(c => c.ObtenerSaldo()):C}");

        foreach (var tipo in Enum.GetValues(typeof(TipoCuenta)).Cast<TipoCuenta>())
        {
            var cuentasTipo = _cuentas.Values.Where(c => c.Tipo == tipo).ToList();
            if (cuentasTipo.Any())
            {
                Console.WriteLine($"\nCuentas {tipo}: {cuentasTipo.Count}");
                Console.WriteLine($"Saldo total: {cuentasTipo.Sum(c => c.ObtenerSaldo()):C}");
                Console.WriteLine($"Puntos totales: {cuentasTipo.Sum(c => c.Puntos)}");

                foreach (var cuenta in cuentasTipo.OrderBy(c => c.Numero))
                {
                    Console.WriteLine($"  {cuenta.ObtenerResumen()}");
                }
            }
        }
    }

    private void GenerarReportesIndividualesClientes()
    {
        Console.WriteLine("\n--- REPORTES POR CLIENTE ---");
        foreach (var cliente in _clientes.OrderBy(c => c.Nombre))
        {
            Console.WriteLine($"\n{cliente.ObtenerResumen()}");

            Console.WriteLine("\n  Historial de operaciones:");
            foreach (var op in cliente.Historial.OrderByDescending(o => o.Fecha).Take(5))
            {
                var datos = op.ObtenerDatosReporte();
                Console.WriteLine($"    {datos["Fecha"]} - {datos["Tipo"]} ({datos["Monto"]})");
            }

            Console.WriteLine("\n  Resumen por cuenta:");
            foreach (var cuenta in cliente.Cuentas.OrderBy(c => c.Numero))
            {
                Console.WriteLine($"    {cuenta.ObtenerResumen()}");
                Console.WriteLine($"      Últimas operaciones: {cuenta.Historial.Count}");
                
                var ultimasOps = cuenta.Historial
                    .OrderByDescending(o => o.Fecha)
                    .Take(3)
                    .Select(o => o.ObtenerDatosReporte());

                foreach (var op in ultimasOps)
                {
                    Console.WriteLine($"      {op["Fecha"]} - {op["Tipo"]} ({op["Monto"]})");
                }
            }
        }
    }

    public void Dispose()
    {
        _clientes.Clear();
        _cuentas.Clear();
        _historialGlobal.Clear();
    }
}

// Ejemplo de uso
var bancoNac = new Banco("Banco Nac");
var bancoTup = new Banco("Banco TUP");

// Creación de clientes y cuentas
var raul = new Cliente("Raul Perez");
raul.AgregarCuenta(new CuentaOro("10001", 10000));
raul.AgregarCuenta(new CuentaPlata("10002", 2000));

var sara = new Cliente("Sara Lopez");
sara.AgregarCuenta(new CuentaPlata("10003", 3000));
sara.AgregarCuenta(new CuentaPlata("10004", 4000));

var luis = new Cliente("Luis Gomez");
luis.AgregarCuenta(new CuentaBronce("10005", 5000));

// Asignar clientes a bancos
bancoNac.AgregarCliente(raul);
bancoNac.AgregarCliente(sara);
bancoTup.AgregarCliente(luis);

// Operaciones en Banco Nac
bancoNac.RegistrarOperacion(new Deposito("10001", 500));
bancoNac.RegistrarOperacion(new Retiro("10002", 200));

var transferenciaInterna = new Transferencia("10001", "10002", 300);
bancoNac.RegistrarOperacion(transferenciaInterna);

var transferenciaExterna = new Transferencia("10003", "10005", 500);
transferenciaExterna.EstablecerBancoDestino(bancoTup);
bancoNac.RegistrarOperacion(transferenciaExterna);

bancoNac.RegistrarOperacion(new Pago("10002", 1200)); // Generará puntos

// Operaciones en Banco TUP
bancoTup.RegistrarOperacion(new Deposito("10005", 100));
bancoTup.RegistrarOperacion(new Retiro("10005", 200));

var transferenciaTupANac = new Transferencia("10005", "10002", 300);
transferenciaTupANac.EstablecerBancoDestino(bancoNac);
bancoTup.RegistrarOperacion(transferenciaTupANac);

bancoTup.RegistrarOperacion(new Pago("10005", 400)); // Generará puntos

// Intentar operación inválida
try
{
    bancoTup.RegistrarOperacion(new Retiro("10005", 100000));
}
catch (SaldoInsuficienteException)
{
    Console.WriteLine("Operación fallida (esperado): Saldo insuficiente");
}

bancoNac.GenerarReporteCompleto();
bancoTup.GenerarReporteCompleto();

bancoNac.Dispose();
bancoTup.Dispose();