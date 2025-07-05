using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }
    public Cliente Titular { get; set; }

    public Cuenta(string numero, decimal saldoInicial)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Puntos = 0;
    }

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

    public abstract void Pagar(decimal monto);
}

public class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) {}

    public override void Pagar(decimal monto)
    {
        if (Extraer(monto))
        {
            if (monto > 1000)
                Puntos += monto * 0.05m;
            else
                Puntos += monto * 0.03m;
        }
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) {}

    public override void Pagar(decimal monto)
    {
        if (Extraer(monto))
            Puntos += monto * 0.02m;
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) {}

    public override void Pagar(decimal monto)
    {
        if (Extraer(monto))
            Puntos += monto * 0.01m;
    }
}

public class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; }
    public List<Operacion> Historial { get; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
        Historial = new List<Operacion>();
    }

    public void Agregar(Cuenta cuenta)
    {
        cuenta.Titular = this;
        Cuentas.Add(cuenta);
    }

    public decimal TotalSaldo => Cuentas.Sum(c => c.Saldo);
    public decimal TotalPuntos => Cuentas.Sum(c => c.Puntos);
}

public class Banco
{
    public string Nombre { get; }
    public List<Cliente> Clientes { get; }
    public List<Operacion> Operaciones { get; }

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        Operaciones = new List<Operacion>();
    }

    public void Agregar(Cliente cliente) => Clientes.Add(cliente);

    public Cuenta BuscarCuenta(string numero)
    {
        return Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numero);
    }

    public void Registrar(Operacion op)
    {
        if (op.Validar(this))
        {
            op.Ejecutar(this);
            Operaciones.Add(op);

            if (op.CuentaOrigen != null)
                op.CuentaOrigen.Titular.Historial.Add(op);

            if (op.CuentaDestino != null && op.CuentaDestino != op.CuentaOrigen)
                op.CuentaDestino.Titular.Historial.Add(op);
        }
    }

    public void Informe()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}\n");
        foreach (var cliente in Clientes)
        {
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.TotalSaldo:0.00} | Puntos Total: $ {cliente.TotalPuntos:0.00}\n");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:0.00} | Puntos: $ {cuenta.Puntos:0.00}");
                foreach (var op in cliente.Historial.Where(o => o.CuentaOrigen == cuenta || o.CuentaDestino == cuenta))
                {
                    Console.WriteLine($"     -  {op.Descripcion()}");
                }
                Console.WriteLine();
            }
        }
    }
}

public abstract class Operacion
{
    public decimal Monto { get; }
    public Cuenta CuentaOrigen { get; protected set; }
    public Cuenta CuentaDestino { get; protected set; }

    public Operacion(decimal monto)
    {
        Monto = monto;
    }

    public abstract bool Validar(Banco banco);
    public abstract void Ejecutar(Banco banco);
    public abstract string Descripcion();
}

public class Deposito : Operacion
{
    private string numeroCuenta;

    public Deposito(string numero, decimal monto) : base(monto)
    {
        numeroCuenta = numero;
    }

    public override bool Validar(Banco banco)
    {
        CuentaDestino = banco.BuscarCuenta(numeroCuenta);
        return CuentaDestino != null;
    }

    public override void Ejecutar(Banco banco)
    {
        CuentaDestino.Depositar(Monto);
    }

    public override string Descripcion()
    {
        return $"Deposito $ {Monto:0.00} a [{CuentaDestino.Numero}/{CuentaDestino.Titular.Nombre}]";
    }
}

public class Retiro : Operacion
{
    private string numeroCuenta;

    public Retiro(string numero, decimal monto) : base(monto)
    {
        numeroCuenta = numero;
    }

    public override bool Validar(Banco banco)
    {
        CuentaOrigen = banco.BuscarCuenta(numeroCuenta);
        return CuentaOrigen != null && CuentaOrigen.Saldo >= Monto;
    }

    public override void Ejecutar(Banco banco)
    {
        CuentaOrigen.Extraer(Monto);
    }

    public override string Descripcion()
    {
        return $"Retiro $ {Monto:0.00} de [{CuentaOrigen.Numero}/{CuentaOrigen.Titular.Nombre}]";
    }
}

public class Pago : Operacion
{
    private string numeroCuenta;

    public Pago(string numero, decimal monto) : base(monto)
    {
        numeroCuenta = numero;
    }

    public override bool Validar(Banco banco)
    {
        CuentaOrigen = banco.BuscarCuenta(numeroCuenta);
        return CuentaOrigen != null && CuentaOrigen.Saldo >= Monto;
    }

    public override void Ejecutar(Banco banco)
    {
        CuentaOrigen.Pagar(Monto);
    }

    public override string Descripcion()
    {
        return $"Pago $ {Monto:0.00} con [{CuentaOrigen.Numero}/{CuentaOrigen.Titular.Nombre}]";
    }
}

public class Transferencia : Operacion
{
    private string origen, destino;

    public Transferencia(string origen, string destino, decimal monto) : base(monto)
    {
        this.origen = origen;
        this.destino = destino;
    }

    public override bool Validar(Banco banco)
    {
        CuentaOrigen = banco.BuscarCuenta(origen);
        CuentaDestino = banco.BuscarCuenta(destino);
        return CuentaOrigen != null && CuentaDestino != null && CuentaOrigen.Saldo >= Monto;
    }

    public override void Ejecutar(Banco banco)
    {
        CuentaOrigen.Extraer(Monto);
        CuentaDestino.Depositar(Monto);
    }

    public override string Descripcion()
    {
        return $"Transferencia $ {Monto:0.00} de [{CuentaOrigen.Numero}/{CuentaOrigen.Titular.Nombre}] a [{CuentaDestino.Numero}/{CuentaDestino.Titular.Nombre}]";
    }
}
class Program 
{
    static void Main() 
    { 
        var lourdes = new Cliente("Lourdes Fernandez"); 
        lourdes.Agregar(new CuentaOro("10001", 1000));

        var elsa = new Cliente("Elsa Albornoz");
        elsa.Agregar(new CuentaPlata("20000", 10000));

         var macro = new Banco("Banco Macro");
         macro.Agregar(lourdes);
        macro.Agregar(elsa);

          macro.Registrar(new Deposito("10001", 200));
          macro.Registrar(new Retiro("10001", 100));
          macro.Registrar(new Transferencia("10001", "20000", 500));
          macro.Registrar(new Pago("10001", 300));

          macro.Informe();
          Console.WriteLine("Fin del programa.");
          Console.ReadKey();
    }
}
