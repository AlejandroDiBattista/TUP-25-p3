using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

public abstract class Cuenta
{
    public string Numero { get; private set; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; } = 0;
    public Cliente Propietario { get; set; }

    public Cuenta(string numero, decimal saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
    }

    public void Depositar(decimal monto)
    {
        Saldo += monto;
    }

    public bool Extraer(decimal monto)
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
            AcumularPuntos(monto);
            return true;
        }
        return false;
    }

    protected abstract void AcumularPuntos(decimal monto);

    public override string ToString()
    {
        return $"{Numero}/{Propietario.Nombre}";
    }
}

public class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    protected override void AcumularPuntos(decimal monto)
    {
        if (monto > 1000)
        {
            Puntos += monto * 0.05m;
        }
        else
        {
            Puntos += monto * 0.03m;
        }
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.02m;
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.01m;
    }
}

public class Cliente
{
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; } = new List<Cuenta>();
    public List<Operacion> Historial { get; private set; } = new List<Operacion>();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta)
    {
        cuenta.Propietario = this;
        Cuentas.Add(cuenta);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        return Cuentas.FirstOrDefault(c => c.Numero == numero);
    }

    public decimal SaldoTotal => Cuentas.Sum(c => c.Saldo);
    public decimal PuntosTotal => Cuentas.Sum(c => c.Puntos);

    public void AgregarOperacion(Operacion operacion)
    {
        Historial.Add(operacion);
    }
}

public abstract class Operacion
{
    public decimal Monto { get; protected set; }
    public Cuenta CuentaOrigen { get; set; }
    public Cuenta CuentaDestino { get; set; }

    public abstract string ObtenerDescripcion();
    public abstract bool Ejecutar();
}

public class Deposito : Operacion
{
    private string _numeroCuenta;

    public Deposito(string numeroCuenta, decimal monto)
    {
        _numeroCuenta = numeroCuenta;
        Monto = monto;
    }

    public string NumeroCuenta => _numeroCuenta;

    public override bool Ejecutar()
    {
        CuentaOrigen.Depositar(Monto);
        return true;
    }

    public override string ObtenerDescripcion()
    {
        CultureInfo culture = new CultureInfo("es-ES");
        return $"Deposito $ {Monto.ToString("N2", culture)} a [{CuentaOrigen}]";
    }
}

public class Retiro : Operacion
{
    private string _numeroCuenta;

    public Retiro(string numeroCuenta, decimal monto)
    {
        _numeroCuenta = numeroCuenta;
        Monto = monto;
    }

    public string NumeroCuenta => _numeroCuenta;

    public override bool Ejecutar()
    {
        return CuentaOrigen.Extraer(Monto);
    }

    public override string ObtenerDescripcion()
    {
        CultureInfo culture = new CultureInfo("es-ES");
        return $"Retiro $ {Monto.ToString("N2", culture)} de [{CuentaOrigen}]";
    }
}

public class Pago : Operacion
{
    private string _numeroCuenta;

    public Pago(string numeroCuenta, decimal monto)
    {
        _numeroCuenta = numeroCuenta;
        Monto = monto;
    }

    public string NumeroCuenta => _numeroCuenta;

    public override bool Ejecutar()
    {
        return CuentaOrigen.Pagar(Monto);
    }

    public override string ObtenerDescripcion()
    {
        CultureInfo culture = new CultureInfo("es-ES");
        return $"Pago $ {Monto.ToString("N2", culture)} con [{CuentaOrigen}]";
    }
}

public class Transferencia : Operacion
{
    private string _numeroCuentaOrigen;
    private string _numeroCuentaDestino;

    public Transferencia(string numeroCuentaOrigen, string numeroCuentaDestino, decimal monto)
    {
        _numeroCuentaOrigen = numeroCuentaOrigen;
        _numeroCuentaDestino = numeroCuentaDestino;
        Monto = monto;
    }

    public string NumeroCuentaOrigen => _numeroCuentaOrigen;
    public string NumeroCuentaDestino => _numeroCuentaDestino;

    public override bool Ejecutar()
    {
        if (CuentaOrigen.Extraer(Monto))
        {
            CuentaDestino.Depositar(Monto);
            return true;
        }
        return false;
    }

    public override string ObtenerDescripcion()
    {
        CultureInfo culture = new CultureInfo("es-ES");
        return $"Transferencia $ {Monto.ToString("N2", culture)} de [{CuentaOrigen}] a [{CuentaDestino}]";
    }
}

public class Banco
{
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; } = new List<Cliente>();
    public List<Operacion> RegistroOperaciones { get; private set; } = new List<Operacion>();
    private static List<Banco> TodosLosBancos = new List<Banco>();

    public Banco(string nombre)
    {
        Nombre = nombre;
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
            var cuenta = cliente.BuscarCuenta(numeroCuenta);
            if (cuenta != null)
            {
                return cuenta;
            }
        }
        return null;
    }

    public static Cuenta BuscarCuentaGlobal(string numeroCuenta)
    {
        foreach (var banco in TodosLosBancos)
        {
            var cuenta = banco.BuscarCuenta(numeroCuenta);
            if (cuenta != null)
            {
                return cuenta;
            }
        }
        return null;
    }

    public Cliente BuscarCliente(string numeroCuenta)
    {
        foreach (var cliente in Clientes)
        {
            var cuenta = cliente.BuscarCuenta(numeroCuenta);
            if (cuenta != null)
            {
                return cliente;
            }
        }
        return null;
    }

    public static Cliente BuscarClienteGlobal(string numeroCuenta)
    {
        foreach (var banco in TodosLosBancos)
        {
            var cliente = banco.BuscarCliente(numeroCuenta);
            if (cliente != null)
            {
                return cliente;
            }
        }
        return null;
    }

    public void Registrar(Operacion operacion)
    {
        if (operacion is Deposito deposito)
        {
            var cuenta = BuscarCuenta(deposito.NumeroCuenta);
            if (cuenta == null)
            {
                Console.WriteLine($"Error: Cuenta no encontrada.");
                return;
            }
            
            deposito.CuentaOrigen = cuenta;
            
            if (deposito.Ejecutar())
            {
                RegistroOperaciones.Add(deposito);
                var cliente = BuscarCliente(cuenta.Numero);
                cliente.AgregarOperacion(deposito);
            }
        }
        else if (operacion is Retiro retiro)
        {
            var cuenta = BuscarCuenta(retiro.NumeroCuenta);
            if (cuenta == null)
            {
                Console.WriteLine($"Error: Cuenta no encontrada.");
                return;
            }
            
            retiro.CuentaOrigen = cuenta;
            
            if (retiro.Ejecutar())
            {
                RegistroOperaciones.Add(retiro);
                var cliente = BuscarCliente(cuenta.Numero);
                cliente.AgregarOperacion(retiro);
            }
        }
        else if (operacion is Pago pago)
        {
            var cuenta = BuscarCuenta(pago.NumeroCuenta);
            if (cuenta == null)
            {
                Console.WriteLine($"Error: Cuenta no encontrada.");
                return;
            }
            
            pago.CuentaOrigen = cuenta;
            
            if (pago.Ejecutar())
            {
                RegistroOperaciones.Add(pago);
                var cliente = BuscarCliente(cuenta.Numero);
                cliente.AgregarOperacion(pago);
            }
        }
        else if (operacion is Transferencia transferencia)
        {
            var cuentaOrigen = BuscarCuenta(transferencia.NumeroCuentaOrigen);
            
            if (cuentaOrigen == null)
            {
                Console.WriteLine($"Error: Cuenta origen no encontrada.");
                return;
            }
            
            var cuentaDestino = BuscarCuentaGlobal(transferencia.NumeroCuentaDestino);
            
            if (cuentaDestino == null)
            {
                Console.WriteLine($"Error: Cuenta destino no encontrada.");
                return;
            }
            
            transferencia.CuentaOrigen = cuentaOrigen;
            transferencia.CuentaDestino = cuentaDestino;
            
            if (transferencia.Ejecutar())
            {
                RegistroOperaciones.Add(transferencia);
                var clienteOrigen = BuscarCliente(cuentaOrigen.Numero);
                clienteOrigen.AgregarOperacion(transferencia);
                
    
                var clienteDestino = BuscarClienteGlobal(cuentaDestino.Numero);
                
                
                if (clienteOrigen != clienteDestino)
                {
                    clienteDestino.AgregarOperacion(transferencia);
                }
            }
        }
    }

    public void Informe()
    {
        CultureInfo culture = new CultureInfo("es-ES");
        
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}");
        Console.WriteLine();

        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal.ToString("N2", culture)} | Puntos Total: $ {cliente.PuntosTotal.ToString("N2", culture)}");
            Console.WriteLine();

            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo.ToString("N2", culture)} | Puntos: $ {cuenta.Puntos.ToString("N2", culture)}");
                
                
                var operacionesCuenta = cliente.Historial
                    .Where(op => (op.CuentaOrigen == cuenta) || (op.CuentaDestino == cuenta))
                    .ToList();
                
                foreach (var op in operacionesCuenta)
                {
                    Console.WriteLine($"     -  {op.ObtenerDescripcion()}");
                }
                
                Console.WriteLine();
            }
        }
        Console.WriteLine();
    }
}

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