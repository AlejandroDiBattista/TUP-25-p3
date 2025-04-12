using System;
using System.Collections.Generic;

class CuentaBancaria
{
    static void Main(string[] args)
    {
        Banco banco = new Banco();

        Cliente cliente1 = new Cliente("nombre");

        Cuenta cuentaOro = new CuentaOro("númerocuenta");
        cliente1.AgregarCuentas(cuentaOro);
        banco.AgregarCliente(cliente1);

        Deposito deposito = new Deposito(1000, cuentaOro);
        deposito.Ejecutar();
        cliente1.RegistrarOperación(deposito);
        banco.RegistrarOperación(deposito);

        Pago pago = new Pago(300, cuentaOro);
        pago.Ejecutar();
        cliente1.RegistrarOperación(pago);
        banco.RegistrarOperación(pago);

        Retiro retiro = new Retiro(200, cuentaOro);
        retiro.Ejecutar();
        cliente1.RegistrarOperación(retiro);
        banco.RegistrarOperación(retiro);

        banco.GererarInforme();
    }
}

public class Banco
{
    public List<Cliente> Clientes { get; set; }
    public List<Operación> HistorialOperacionesGlobal { get; set; }
    public Banco()
    {
        Clientes = new List<Cliente>();
        HistorialOperacionesGlobal = new List<Operación>();
    }

    public void AgregarCliente(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public void RegistrarOperación(Operación operación)
    {
        HistorialOperacionesGlobal.Add(operación);
    }

    public void GererarInforme()
    {
        Console.WriteLine("Informe Global de operaciónes:")
        foreach (var operación in HistorialOperacionesGlobal)
        {
           Console.WriteLine(operación);
        }

        Console.WriteLine("Estado final de cuentas:");  
        foreach (var cliente in Clientes)  
        {  
           foreach (var cuenta in cliente.Cuentas)  
          {  
            Console.WriteLine($"Cuenta {cuenta.NúmeroCuenta} Saldo {cuenta.Saldo} Puntos {cuenta.PuntosAcumulados}");  
          }  
        }  
      
        foreach (var cliente in Clientes)  
        {  
           Console.WriteLine($"historial de operaciones de {clientes.Nombre}:");  
           foreach (var operaciónes in cliente.HistorialOperaciones)  
           {  
            Console.WriteLine(operación)  
           }
        }
    }  
}
public class Cliente
{
    public string Nombre { get; set; }
    public List<Cuenta> Cuentas { get; set; }
    public List<Operación> HistorialOperaciones { get; set; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
        HistorialOperaciones = new List<Operación>();
    }

    public void AgregarCuentas(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public void RegistrarOperación(Operación operación)
    {
        HistorialOperaciones.Add(operación);
    }
}

public abstract class Cuenta
{
   public string NúmeroCuenta { get; set; }
   public decimal Saldo { get; set; }
   public decimal PuntosAcumulados { get; set; }

   public Cuenta(string numeroCuenta)
   {
    NúmeroCuenta = numeroCuenta;
    Saldo = 0;
    PuntosAcumulados = 0;
   }  

   public abstract void ProcesarPago(decimal monto);  
   public abstract void AcumularPuntos(decimal monto);  
   public virtual void Depositar(decimal monto);  
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
}

public class CuentaOro : Cuenta
{
   public CuentaOro(string numeroCuenta) : base(numeroCuenta)
   public override void ProcesarPago(decimal monto)  
    {  
      Saldo -= monto;  
      AcumularPuntos(monto);  
    }

    public override void AcumularPuntos(decimal monto)
    {
        PuntosAcumulados += monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numeroCuenta) : base(numeroCuenta)
    public override void ProcesarPago(decimal monto)
    {
        Saldo -= monto;
        AcumularPuntos(monto);
    }
    public override void AcumularPuntos(decimal monto)
    {
        PuntosAcumulados += monto * 0.02m;
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numeroCuenta) : base(numeroCuenta) 
    public override void ProcesarPago(decimal monto)
    {
        Saldo -= monto;
        AcumularPuntos(monto);
    }
    public override void AcumularPuntos(decimal monto)
    {
        PuntosAcumulados += monto * 0.01m;
    }
}

public abstract class Operación

{
    public decimal Monto { get; set; }
    public Cuenta CuentaOrigen { get; set; }
    public Cuenta CuentaDestino { get; set; }

    public Operación(decimal monto, Cuenta cuentaOrigen, Cuenta cuentaDestino = null)
    {
    Monto = monto;
    CuentaOrigen = cuentaOrigen;
    CuentaDestino = cuentaDestino;
    }

    public abstract void Ejecutar();
}

public class Deposito : Operación
{
    public Deposito(decimal monto, Cuenta cuentaOrigen) : base(monto, cuentaOrigen)
    public override void Ejecutar()
    {
        CuentaOrigen.Depositar(Monto);
    }  
    public override string ToString()  
    {  
        return $"Deposito de {Monto} en la cuenta {CuentaOrigen.NúmeroCuenta}";  
    }
}
public class Retiro : Operación
{
    public Retiro(decimal monto, Cuenta CuentaOrigen) : base(monto, CuentaOrigen)
    public override void Ejecutar()
    {
        if (CuentaOrigen.Extraer(Monto))
        {
            Console.WriteLine("Fondos insuficientes para la operación de retiro..");
        }
    }

    public override string ToString()
    {
        return $"Retiro de {Monto} de la cuenta {CuentaOrigen.NúmeroCuenta}";
    }
}

public class Pago : Operación
{
    public Pago(decimal monto, Cuenta cuentaOrigen) : base(monto, cuentaOrigen)
    public override void Ejecutar()
    {
        CuentaOrigen.ProcesarPago(Monto);
    }
    public override string ToString()
    {
        return $"Pago de {Monto} dede la cuenta {CuentaOrigen.NúmeroCuenta}";
    }  
}

public class Transferencia : Operación
{
    public Transferencia(decimal monto, Cuenta cuentaOrigen, Cuenta cuentaDestino) : base(monto, cuentaOrigen, cuentaDestino)
    public override void Ejecutar()
    {
        if (CuentaOrigen.Extraer(Monto))
        {
            CuentaDestino.Depositar(Monto);
        }
        else
        {
            console.WriteLine("Forndos insuficientes para realizar la transferencia..");
        }
    }

    public override string ToString()
    {
        return $"Transferencia de {Monto} desde la cuenta {CuentaOrigen.NúmeroCuenta} hacia la {CuentaDestino.NúmeroCuenta}";

    }
}