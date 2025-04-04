using System;
using System.Collections.Generic;
    
        var fabri = new Cliente("Fabrizio Mazza");
        fabri.Agregar(new CuentaPlata("10001", 5000));

        var agus = new Cliente("Agustina Sarmiento");
        agus.Agregar(new CuentaPlata("10002", 3000));

        var lourdes = new Cliente("Lourdes Gómez");
        lourdes.Agregar(new CuentaOro("10003", 1000));
        lourdes.Agregar(new CuentaPlata("10004", 5000));
        lourdes.Agregar(new CuentaBronce("10005", 6000));

        var santi = new Cliente("Santiago Gutiérrez");
        santi.Agregar(new CuentaOro("10006", 2000));

        var nac = new Banco("Banco Nac");
        nac.Agregar(fabri);
        nac.Agregar(agus);

        var tup = new Banco("Banco TUP");
        tup.Agregar(lourdes);
        tup.Agregar(santi);

        nac.Registrar(new Deposito("10001", 100));
        nac.Registrar(new Retiro("10002", 200));
        nac.Registrar(new Transferencia("10001", "10002", 300));
        nac.Registrar(new Pago("10002", 400));

        tup.Registrar(new Deposito("10005", 100));
        tup.Registrar(new Retiro("10005", 200));
        tup.Registrar(new Transferencia("10005", "10002", 300));
        tup.Registrar(new Pago("10005", 400));

        nac.Informe();
        tup.Informe();

        Console.WriteLine("PULSE UNA TECLA PARA FINALIZAR...");
        Console.ReadKey();
    
class Banco
{
    public string Nombre { get; private set; }
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();

    public Banco(string nombre)
    {
        Console.WriteLine("");
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion)
    {
        operaciones.Add(operacion);
        operacion.Ejecutar(clientes);

         var cliente = clientes.FirstOrDefault(c => c.cuentas.Any(cta => cta.Numero == operacion.CuentaOrigen));
         
        if (cliente != null)
        {
            cliente.RegistrarOperacion(operacion);
        }
    }

    public void Informe()
    {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {clientes.Count}\n");

        foreach (var cliente in clientes)
        {
            cliente.MostrarResumen();
            Console.WriteLine();
        }
    }
}


class Cliente
{
    public string Nombre { get; private set; }
    public List<Cuenta> cuentas = new List<Cuenta>();
    public List<Operacion> historial = new List<Operacion>();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta)
    {
        cuentas.Add(cuenta);
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        historial.Add(operacion);
    }

    public void MostrarResumen()
    {
        Console.WriteLine($"  Cliente: {Nombre} | Saldo Total: $ {cuentas.Sum(c => c.Saldo):0.00} | Puntos Total: $ {cuentas.Sum(c => c.Puntos):0.00}\n");
        
        foreach (var cuenta in cuentas)
        {
            cuenta.MostrarResumen();
        }

        if (historial.Count > 0)
        
        {

        Console.WriteLine("");
        Console.WriteLine("    Historial de operaciones:");
        Console.WriteLine("");

        foreach (var operacion in historial)
        {
            Console.WriteLine($"      {operacion.Descripcion()}");
        }
        }
    }
}

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

    public abstract void AplicarPago(double monto);

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

    public void MostrarResumen()
    {
        Console.WriteLine($"    Cuenta: {Numero} | Saldo: $ {Saldo:0.00} | Puntos: $ {Puntos:0.00}");
    }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldo) : base(numero, saldo) { }

    public override void AplicarPago(double monto)
    {
        double porcentaje = monto > 1000 ? 0.05 : 0.03;
        Puntos += monto * porcentaje;
        Saldo -= monto;
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldo) : base(numero, saldo) { }

    public override void AplicarPago(double monto)
    {
        Puntos += monto * 0.02;
        Saldo -= monto;
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldo) : base(numero, saldo) { }

    public override void AplicarPago(double monto)
    {
        Puntos += monto * 0.01;
        Saldo -= monto;
    }
}

abstract class Operacion
{
    public string CuentaOrigen;
    protected double Monto;

    protected Operacion(string cuentaOrigen, double monto)
    {
        CuentaOrigen = cuentaOrigen;
        Monto = monto;
    }

    public abstract void Ejecutar(List<Cliente> clientes);

    public virtual string Descripcion()
    {
    return $"Operación en cuenta {CuentaOrigen} por $ {Monto:0.00}";
    }
}


class Deposito : Operacion
{
    public Deposito(string cuenta, double monto) : base(cuenta, monto) { }

    public override void Ejecutar(List<Cliente> clientes)
    {
        var cuenta = clientes.SelectMany(c => c.cuentas).FirstOrDefault(c => c.Numero == CuentaOrigen);
        cuenta?.Depositar(Monto);
    }

    public override string Descripcion()
    {
    return $"Depósito de $ {Monto:0.00} en cuenta {CuentaOrigen}";
    }
}

class Retiro : Operacion
{
    public Retiro(string cuenta, double monto) : base(cuenta, monto) { }

    public override void Ejecutar(List<Cliente> clientes)
    {
        var cuenta = clientes.SelectMany(c => c.cuentas).FirstOrDefault(c => c.Numero == CuentaOrigen);
        cuenta?.Extraer(Monto);
    }
    public override string Descripcion()
    {
    return $"Retiro de $ {Monto:0.00} en cuenta {CuentaOrigen}";
    }

}

class Pago : Operacion
{
    public Pago(string cuenta, double monto) : base(cuenta, monto) { }

    public override void Ejecutar(List<Cliente> clientes)
    {
        var cuenta = clientes.SelectMany(c => c.cuentas).FirstOrDefault(c => c.Numero == CuentaOrigen);
        cuenta?.AplicarPago(Monto);
    }

    public override string Descripcion()
    {
    return $"Pago de $ {Monto:0.00} desde cuenta {CuentaOrigen}";
    }

}

class Transferencia : Operacion
{
    private string CuentaDestino;

    public Transferencia(string cuentaOrigen, string cuentaDestino, double monto) : base(cuentaOrigen, monto)
    {
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar(List<Cliente> clientes)
    {
        var cuentaOrigen = clientes.SelectMany(c => c.cuentas).FirstOrDefault(c => c.Numero == CuentaOrigen);
        var cuentaDestino = clientes.SelectMany(c => c.cuentas).FirstOrDefault(c => c.Numero == CuentaDestino);

        if (cuentaOrigen != null && cuentaDestino != null && cuentaOrigen.Extraer(Monto))
        {
            cuentaDestino.Depositar(Monto);
        }
    }

    public override string Descripcion()
    {
    return $"Transferencia de $ {Monto:0.00} de cuenta {CuentaOrigen} a cuenta {CuentaDestino}";
    }

}