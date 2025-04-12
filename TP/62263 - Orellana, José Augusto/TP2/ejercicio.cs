using System;
using System.Collections.Generic;

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.
class Banco
{
    public string Nombre { get; }
    public List<Cliente> Clientes;
    public List<Operacion> Operaciones;

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        Operaciones = new List<Operacion>();
        RegistroBancos.RegistrarBanco(this);
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion)
    {
        Operaciones.Add(operacion);
        operacion.Ejecutar(this);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var cliente in Clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.Numero == numero) return cuenta;
            }
        }
        return null;
    }

    public string BuscarTitular(string numero)
    {
        foreach (var cliente in Clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.Numero == numero) return cliente.Nombre;
            }
        }
        return null;
    }

    public void Informe()
    {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}");
        foreach (var cliente in Clientes)
        {
            decimal saldoTotal = 0;
            decimal puntosTotal = 0;

            foreach (var cuenta in cliente.Cuentas)
            {
                saldoTotal += cuenta.Saldo;
                puntosTotal += cuenta.Puntos;
            }
            Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: ${saldoTotal:F2} | Puntos Total: ${puntosTotal:F2}");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: ${cuenta.Saldo:F2} | Puntos: ${cuenta.Puntos:F2}");

                foreach (var detalle in cuenta.Historial)
                {
                    Console.WriteLine($"     -  {detalle}");
                }
                Console.WriteLine();
            }
        }
    }  
}

static class RegistroBancos
{
    public static List<Banco> Bancos = new List<Banco>();

    public static void RegistrarBanco(Banco banco)
    {
        Bancos.Add(banco);
    }

    public static Cuenta BuscarCuentaGlobal(string numero)
    {
        foreach (var banco in Bancos)
        {
            var cuenta = banco.BuscarCuenta(numero);
            if (cuenta != null) return cuenta;
        }
        return null;
    }

    public static string BuscarTitularGlobal(string numero)
    {
        foreach (var banco in Bancos)
        {
            var titular = banco.BuscarTitular(numero);
            if (titular != null) return titular;
        }
        return null;
    }
}

class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas {get; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }
}

abstract class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; set; }
    public decimal Puntos { get; set; }
    public List<string> Historial { get; }

    public Cuenta(string numero, decimal saldoInicial = 0)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Puntos = 0;
        Historial = new List<string>();
    }

    public abstract void AcumularPuntos(decimal monto, string numero, Banco banco);

    public void Depositar(decimal monto, string titular, Banco banco)
    {
        Saldo += monto;
        Historial.Add($"Deposito: ${monto:F2} a [{Numero}/{titular}]");
    }

    public bool Retirar(decimal monto, string titular)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            Historial.Add($"Retiro: ${monto:F2} de [{Numero}/{titular}]");
            return true;
        }
        return false;
    }
}

class CuentaOro: Cuenta
{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) {}

    public override void AcumularPuntos(decimal monto, string numero, Banco banco)
    {
        Puntos += monto >= 1000 ? monto * 0.05m : monto * 0.03m;
        Saldo -= monto;
        string titular = banco.BuscarTitular(numero);
        Historial.Add($"Pago: ${monto:F2} con [{Numero}/{titular}]");
    }
}
class CuentaPlata: Cuenta
{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) {}

    public override void AcumularPuntos(decimal monto, string numero, Banco banco)
    {
        Puntos += monto * 0.02m;
        Saldo -= monto;
        string titular = banco.BuscarTitular(numero);
        Historial.Add($"Pago: ${monto:F2} con [{Numero}/{titular}]");
    }
}
class CuentaBronce: Cuenta
{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) {}

    public override void AcumularPuntos(decimal monto, string numero, Banco banco)
    {
        if (Saldo >= monto)
        {
            Puntos += monto * 0.01m;
            Saldo -= monto;
            string titular = banco.BuscarTitular(numero);
            Historial.Add($"Pago: ${monto:F2} con [{Numero}/{titular}]");
        }
    }
}

abstract class Operacion
{
    public string CuentaOrigen { get; }
    public decimal Monto { get; }

    public Operacion(string cuentaOrigen, decimal monto)
    {
        CuentaOrigen = cuentaOrigen;
        Monto = monto;
    }

    public abstract void Ejecutar(Banco banco);
}

class Deposito: Operacion
{
    public Deposito(string cuentaOrigen, decimal monto) : base(cuentaOrigen, monto) {}

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(CuentaOrigen);
        string titular = banco.BuscarTitular(CuentaOrigen);
        if (cuenta != null && titular != null) cuenta.Depositar(Monto, titular, banco);
    }
}


class Retiro: Operacion
{
    public Retiro(string cuentaOrigen, decimal monto) : base(cuentaOrigen, monto) {}

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(CuentaOrigen);
        string titular = banco.BuscarTitular(CuentaOrigen);
        if (cuenta != null && titular != null) cuenta.Retirar(Monto, titular);
    }
}
class Transferencia: Operacion
{
    public string CuentaDestino { get; }

    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto) : base(cuentaOrigen, monto)
    {
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar(Banco banco)
    {
        Cuenta origen = banco.BuscarCuenta(CuentaOrigen);
        Cuenta destino = RegistroBancos.BuscarCuentaGlobal(CuentaDestino);
        string titularOrigen = RegistroBancos.BuscarTitularGlobal(CuentaOrigen);
        string titularDestino = RegistroBancos.BuscarTitularGlobal(CuentaDestino);


        if(origen != null && destino != null && titularOrigen!= null && titularDestino != null)
        {
            if(origen.Saldo >= Monto)
            {
                origen.Saldo -= Monto;
                destino.Saldo += Monto;

                string detalle = $"Transferencia: ${Monto:F2} de [{CuentaOrigen}/{titularOrigen}] a [{CuentaDestino}/{titularDestino}]";
                origen.Historial.Add(detalle);
                destino.Historial.Add(detalle);
            }
        }
    }
}
class Pago: Operacion
{
    public Pago(string cuentaOrigen, decimal monto) : base(cuentaOrigen, monto) {}

    public override void Ejecutar(Banco banco)
    {
        Cuenta cuenta = banco.BuscarCuenta(CuentaOrigen);
        if (cuenta != null) cuenta.AcumularPuntos(Monto, CuentaOrigen, banco);
    }
}


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

