using System;
using System.Collections.Generic;
using System.Linq;

// **** CLASES ****

abstract class Cuenta
{
    private static int contadorCuentas = 1000;
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }

    public Cuenta(decimal saldoInicial)
    {
        Numero = (contadorCuentas++).ToString();
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

    public void Acreditar(decimal monto)
    {
        Saldo += monto;
    }

    public override string ToString()
    {
        return $"Cuenta Nº: {Numero} | Saldo: ${Saldo} | Puntos: {Puntos}";
    }
}

class CuentaOro : Cuenta
{
    public CuentaOro(decimal saldoInicial) : base(saldoInicial) { }

    public override void Pagar(decimal monto)
    {
        if (Extraer(monto))
        {
            Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
        }
    }
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(decimal saldoInicial) : base(saldoInicial) { }

    public override void Pagar(decimal monto)
    {
        if (Extraer(monto))
        {
            Puntos += monto * 0.02m;
        }
    }
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(decimal saldoInicial) : base(saldoInicial) { }

    public override void Pagar(decimal monto)
    {
        if (Extraer(monto))
        {
            Puntos += monto * 0.01m;
        }
    }
}

class Cliente
{
    private static int contadorClientes = 1;
    public int NumeroCliente { get; }
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; }

    public Cliente(string nombre)
    {
        NumeroCliente = contadorClientes++;
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public Cuenta ObtenerCuenta(string numero)
    {
        return Cuentas.FirstOrDefault(c => c.Numero == numero);
    }

    public override string ToString()
    {
        return $"Cliente Nº: {NumeroCliente} | Nombre: {Nombre}";
    }
}

class Banco
{
    public string Nombre { get; }
    public List<Cliente> Clientes { get; }

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        return Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numero);
    }

    public void MostrarTodasLasCuentas()
    {
        foreach (var cliente in Clientes)
        {
            Console.WriteLine(cliente);
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine("   " + cuenta);
            }
        }
    }
}

// ******* EJECUCIÓN *********

Banco banco = new Banco("Banco UTN");

while (true)
{
    Console.WriteLine("\n===== Menú Principal =====");
    Console.WriteLine("1. Agregar Cliente");
    Console.WriteLine("2. Agregar Cuenta");
    Console.WriteLine("3. Depositar");
    Console.WriteLine("4. Retirar");
    Console.WriteLine("5. Pagar");
    Console.WriteLine("6. Salir");
    Console.Write("Seleccione una opción: ");

    string opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            Console.Write("Ingrese el nombre del cliente: ");
            string nombre = Console.ReadLine();
            banco.Agregar(new Cliente(nombre));
            Console.WriteLine("Cliente agregado.");
            break;

        case "2":
            Console.WriteLine("Clientes disponibles:");
            foreach (var c in banco.Clientes)
            {
                Console.WriteLine(c);
            }

            Console.Write("Ingrese el número del cliente: ");
            if (!int.TryParse(Console.ReadLine(), out int numeroCliente))
            {
                Console.WriteLine("Número inválido.");
                break;
            }

            Cliente cliente = banco.Clientes.FirstOrDefault(c => c.NumeroCliente == numeroCliente);
            if (cliente != null)
            {
                Console.WriteLine("Seleccione el tipo de cuenta:");
                Console.WriteLine("1. Oro");
                Console.WriteLine("2. Plata");
                Console.WriteLine("3. Bronce");
                string tipo = Console.ReadLine();

                Console.Write("Saldo inicial: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal saldo))
                {
                    Console.WriteLine("Monto inválido.");
                    break;
                }

                Cuenta nuevaCuenta = tipo switch
                {
                    "1" => new CuentaOro(saldo),
                    "2" => new CuentaPlata(saldo),
                    "3" => new CuentaBronce(saldo),
                    _ => null
                };

                if (nuevaCuenta != null)
                {
                    cliente.Agregar(nuevaCuenta);
                    Console.WriteLine($"Cuenta agregada: {nuevaCuenta}");
                }
                else
                {
                    Console.WriteLine("Tipo inválido.");
                }
            }
            else
            {
                Console.WriteLine("Cliente no encontrado.");
            }
            break;

        case "3":
            Console.WriteLine("Cuentas disponibles:");
            banco.MostrarTodasLasCuentas();

            Console.Write("Ingrese el número de cuenta: ");
            string numeroDep = Console.ReadLine();
            Cuenta cuentaDep = banco.BuscarCuenta(numeroDep);
            if (cuentaDep != null)
            {
                Console.Write("Ingrese el monto: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal montoDep))
                {
                    cuentaDep.Depositar(montoDep);
                    Console.WriteLine("Depósito realizado.");
                }
                else
                {
                    Console.WriteLine("Monto inválido.");
                }
            }
            else
            {
                Console.WriteLine("Cuenta no encontrada.");
            }
            break;

        case "4":
            Console.WriteLine("Cuentas disponibles:");
            banco.MostrarTodasLasCuentas();

            Console.Write("Ingrese el número de cuenta: ");
            string numeroExt = Console.ReadLine();
            Cuenta cuentaExt = banco.BuscarCuenta(numeroExt);
            if (cuentaExt != null)
            {
                Console.Write("Ingrese el monto: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal montoExt))
                {
                    if (cuentaExt.Extraer(montoExt))
                    {
                        Console.WriteLine("Retiro realizado.");
                    }
                    else
                    {
                        Console.WriteLine("Fondos insuficientes.");
                    }
                }
                else
                {
                    Console.WriteLine("Monto inválido.");
                }
            }
            else
            {
                Console.WriteLine("Cuenta no encontrada.");
            }
            break;

        case "5":
            Console.WriteLine("Cuentas disponibles:");
            banco.MostrarTodasLasCuentas();

            Console.Write("Ingrese el número de cuenta: ");
            string numeroPag = Console.ReadLine();
            Cuenta cuentaPag = banco.BuscarCuenta(numeroPag);
            if (cuentaPag != null)
            {
                Console.Write("Ingrese el monto a pagar: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal montoPag))
                {
                    cuentaPag.Pagar(montoPag);
                    Console.WriteLine("Pago realizado, puntos acumulados: " + cuentaPag.Puntos);
                }
                else
                {
                    Console.WriteLine("Monto inválido.");
                }
            }
            else
            {
                Console.WriteLine("Cuenta no encontrada.");
            }
            break;

        case "6":
            Console.WriteLine("¡Gracias por usar el sistema bancario!");
            return;

        default:
            Console.WriteLine("Opción no válida.");
            break;
    }
}