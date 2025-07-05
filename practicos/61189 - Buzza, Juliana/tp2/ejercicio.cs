using System;
using System.Collections.Generic;
using System.Linq;

abstract class Cuenta
{
    public string Numero { get; private set; }
    public double Saldo { get; protected set; }
    public double Puntos { get; protected set; }

    public Cuenta(string numero, double saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Puntos = 0;
    }

    public virtual void Depositar(double monto)
    {
        Saldo += monto;
    }

    public virtual bool Extraer(double monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public virtual void Pagar(double monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            AcumularPuntos(monto);
        }
    }

    public abstract void AcumularPuntos(double monto);
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldoInicial) : base(numero, saldoInicial) { }

    public override void AcumularPuntos(double monto)
    {
        if (monto > 1000)
            Puntos += monto * 0.05;
        else
            Puntos += monto * 0.03;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldoInicial) : base(numero, saldoInicial) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.02;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldoInicial) : base(numero, saldoInicial) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += monto * 0.01;
    }
}

class Cliente
{
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }
    public List<Operacion> Historial { get; private set; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
        Historial = new List<Operacion>();
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public double TotalSaldo()
    {
        return Cuentas.Sum(c => c.Saldo);
    }

    public double TotalPuntos()
    {
        return Cuentas.Sum(c => c.Puntos);
    }

    public bool TieneCuenta(Cuenta cuenta)
    {
        return Cuentas.Contains(cuenta);
    }
}


abstract class Operacion
{
    public double Monto { get; protected set; }
    public abstract void Ejecutar(Banco banco);
    public abstract string Descripcion(Banco banco);
}

class Deposito : Operacion
{
    private string cuentaDestino;

    public Deposito(string cuentaDestino, double monto)
    {
        this.cuentaDestino = cuentaDestino;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(cuentaDestino);
        if (cuenta != null)
        {
            cuenta.Depositar(Monto);
        }
    }

    public override string Descripcion(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(cuentaDestino);
        var cliente = banco.BuscarClientePorCuenta(cuenta);
        return $"-  Deposito $ {Monto:F2} a [{cuenta?.Numero}/{cliente?.Nombre}]";
    }
}

class Retiro : Operacion
{
    private string cuentaOrigen;

    public Retiro(string cuentaOrigen, double monto)
    {
        this.cuentaOrigen = cuentaOrigen;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(cuentaOrigen);
        if (cuenta != null)
        {
            cuenta.Extraer(Monto);
        }
    }

    public override string Descripcion(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(cuentaOrigen);
        var cliente = banco.BuscarClientePorCuenta(cuenta);
        return $"-  Retiro $ {Monto:F2} de [{cuenta?.Numero}/{cliente?.Nombre}]";
    }
}

class Pago : Operacion
{
    private string cuentaOrigen;

    public Pago(string cuentaOrigen, double monto)
    {
        this.cuentaOrigen = cuentaOrigen;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(cuentaOrigen);
        if (cuenta != null)
        {
            cuenta.Pagar(Monto);
        }
    }

    public override string Descripcion(Banco banco)
    {
        var cuenta = banco.BuscarCuenta(cuentaOrigen);
        var cliente = banco.BuscarClientePorCuenta(cuenta);
        return $"-  Pago $ {Monto:F2} con [{cuenta?.Numero}/{cliente?.Nombre}]";
    }
}

class Transferencia : Operacion
{
    private string cuentaOrigen;
    private string cuentaDestino;

    public Transferencia(string origen, string destino, double monto)
    {
        this.cuentaOrigen = origen;
        this.cuentaDestino = destino;
        this.Monto = monto;
    }

    public override void Ejecutar(Banco banco)
    {
        var origen = banco.BuscarCuenta(cuentaOrigen);
        var destino = banco.BuscarCuenta(cuentaDestino);

        if (origen != null && destino != null && origen.Extraer(Monto))
        {
            destino.Depositar(Monto);
        }
    }

    public override string Descripcion(Banco banco)
    {
        var origen = banco.BuscarCuenta(cuentaOrigen);
        var destino = banco.BuscarCuenta(cuentaDestino);
        var origenCliente = banco.BuscarClientePorCuenta(origen);
        var destinoCliente = banco.BuscarClientePorCuenta(destino);

        string origenInfo = origen != null && origenCliente != null
            ? $"{origen.Numero}/{origenCliente.Nombre}"
            : $"{cuentaOrigen}/(otro banco)";

        string destinoInfo = destino != null && destinoCliente != null
            ? $"{destino.Numero}/{destinoCliente.Nombre}"
            : $"{cuentaDestino}/(otro banco)";

        return $"-  Transferencia $ {Monto:F2} de [{origenInfo}] a [{destinoInfo}]";
    }
}

class Banco
{
    public string Nombre { get; private set; }
    private List<Cliente> clientes;
    private List<Operacion> operaciones;

    public Banco(string nombre)
    {
        Nombre = nombre;
        clientes = new List<Cliente>();
        operaciones = new List<Operacion>();
    }

    public void Agregar(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var cliente in clientes)
        {
            var cuenta = cliente.Cuentas.FirstOrDefault(c => c.Numero == numero);
            if (cuenta != null)
                return cuenta;
        }
        return null;
    }

    public Cliente BuscarClientePorCuenta(Cuenta cuenta)
    {
        foreach (var cliente in clientes)
        {
            if (cliente.TieneCuenta(cuenta))
                return cliente;
        }
        return null;
    }

    public void Registrar(Operacion operacion)
    {
        operacion.Ejecutar(this);
        operaciones.Add(operacion);

        if (operacion is Deposito dep)
        {
            var cuenta = BuscarCuenta(dep.GetType().GetField("cuentaDestino", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(dep).ToString());
            var cliente = BuscarClientePorCuenta(cuenta);
            cliente?.Historial.Add(operacion);
        }
        else if (operacion is Retiro ret)
        {
            var cuenta = BuscarCuenta(ret.GetType().GetField("cuentaOrigen", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(ret).ToString());
            var cliente = BuscarClientePorCuenta(cuenta);
            cliente?.Historial.Add(operacion);
        }
        else if (operacion is Pago pag)
        {
            var cuenta = BuscarCuenta(pag.GetType().GetField("cuentaOrigen", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(pag).ToString());
            var cliente = BuscarClientePorCuenta(cuenta);
            cliente?.Historial.Add(operacion);
        }
        else if (operacion is Transferencia tra)
        {
            var cuenta1 = BuscarCuenta(tra.GetType().GetField("cuentaOrigen", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(tra).ToString());
            var cuenta2 = BuscarCuenta(tra.GetType().GetField("cuentaDestino", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(tra).ToString());
            var cliente1 = BuscarClientePorCuenta(cuenta1);
            var cliente2 = BuscarClientePorCuenta(cuenta2);
            cliente1?.Historial.Add(operacion);
            if (cliente2 != cliente1)
                cliente2?.Historial.Add(operacion);
        }
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {clientes.Count}");
        foreach (var cliente in clientes)
        {
            Console.WriteLine($"\n  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.TotalSaldo():N2} | Puntos Total: $ {cliente.TotalPuntos():N2}");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"\n    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:N2} | Puntos: $ {cuenta.Puntos:N2}");
                foreach (var operacion in cliente.Historial)
                {
                    if (operacion.Descripcion(this).Contains(cuenta.Numero))
                        Console.WriteLine("     " + operacion.Descripcion(this));
                }
            }
        }
        Console.WriteLine();
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