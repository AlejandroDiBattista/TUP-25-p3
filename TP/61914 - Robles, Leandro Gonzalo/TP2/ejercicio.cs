using System;
using System.Collections.Generic;

public class Operacion
{
    public DateTime Fecha { get; } = DateTime.Now;
    public string Tipo { get; set; }
    public decimal Monto { get; set; }
    public string Detalle { get; set; }

    public Operacion(string tipo, decimal monto, string detalle)
    {
        Tipo = tipo;
        Monto = monto;
        Detalle = detalle;
    }
}

public class Banco
{
    private HashSet<string> numerosDeCuentaUsados = new();
    private Random random = new();
    public List<Cliente> Clientes { get; private set; } = new();
    public List<Operacion> OperacionesGlobales { get; private set; } = new();

    public void AgregarCliente(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public string GenerarNumeroCuenta()
    {
        string numero;
        do
        {
            numero = random.Next(10000, 99999).ToString();
        } while (numerosDeCuentaUsados.Contains(numero));

        numerosDeCuentaUsados.Add(numero);
        return numero;
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        OperacionesGlobales.Add(operacion);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var cliente in Clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.Numero == numero)
                    return cuenta;
            }
        }
        return null;
    }

    public Cliente BuscarClientePorCuenta(Cuenta cuenta)
    {
        foreach (var cliente in Clientes)
        {
            if (cliente.Cuentas.Contains(cuenta))
                return cliente;
        }
        return null;
    }
}

public class Cliente
{
    public string Nombre { get; private set; }
    public List<Cuenta> Cuentas { get; private set; } = new();
    public List<Operacion> HistorialOperaciones { get; private set; } = new();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void AgregarCuenta(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public void RegistrarOperacion(Operacion op)
    {
        HistorialOperaciones.Add(op);
    }
}

public abstract class Cuenta
{
    public string Numero { get; private set; }
    public decimal Saldo { get; protected set; } = 0;
    public int Puntos { get; protected set; } = 0;

    public Cuenta(string numero)
    {
        Numero = numero;
    }

    public virtual void Depositar(decimal monto)
    {
        Saldo += monto;
    }

    public virtual bool Retirar(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public virtual void Pagar(decimal monto)
    {
        if (Retirar(monto))
            AcumularPuntos(monto);
    }

    protected abstract void AcumularPuntos(decimal monto);
}

public class CuentaOro : Cuenta
{
    public CuentaOro(string numero) : base(numero) { }

    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += (int)(monto * (monto > 1000 ? 0.05m : 0.03m));
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero) : base(numero) { }

    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += (int)(monto * 0.02m);
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero) : base(numero) { }

    protected override void AcumularPuntos(decimal monto)
    {
        Puntos += (int)(monto * 0.01m);
    }
}


        Banco banco = new Banco();

        while (true)
        {
            Console.WriteLine("\n=== MENÚ DEL BANCO ===");
            Console.WriteLine("1. Crear cliente y cuenta");
            Console.WriteLine("2. Depositar");
            Console.WriteLine("3. Retirar");
            Console.WriteLine("4. Pagar");
            Console.WriteLine("5. Transferir");
            Console.WriteLine("6. Mostrar informe");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.Write("Nombre del cliente: ");
                    string nombre = Console.ReadLine();
                    Cliente cliente = new Cliente(nombre);
                    banco.AgregarCliente(cliente);

                    Console.WriteLine("Tipo de cuenta (1=Oro, 2=Plata, 3=Bronce): ");
                    string tipo = Console.ReadLine();
                    string numero = banco.GenerarNumeroCuenta();
                    Cuenta cuenta = tipo switch
                    {
                        "1" => new CuentaOro(numero),
                        "2" => new CuentaPlata(numero),
                        "3" => new CuentaBronce(numero),
                        _ => null
                    };

                    if (cuenta != null)
                    {
                        cliente.AgregarCuenta(cuenta);
                        Console.WriteLine($"\n✔ Cuenta tipo {cuenta.GetType().Name.Replace("Cuenta", "")} creada exitosamente.");
                        Console.WriteLine($"🧾 Número de cuenta asignado: {cuenta.Numero}\n");
                    }
                    else
                        Console.WriteLine("Tipo de cuenta inválido.");
                    break;

                case "2":
                    Console.Write("Número de cuenta: ");
                    numero = Console.ReadLine();
                    cuenta = banco.BuscarCuenta(numero);
                    if (cuenta != null)
                    {
                        Console.Write("Monto a depositar: ");
                        decimal monto = decimal.Parse(Console.ReadLine());
                        cuenta.Depositar(monto);
                        var op = new Operacion("Depósito", monto, $"Depósito de {monto} en cuenta {numero}");
                        banco.RegistrarOperacion(op);
                        banco.BuscarClientePorCuenta(cuenta)?.RegistrarOperacion(op);
                        Console.WriteLine("Depósito realizado.");
                    }
                    else
                        Console.WriteLine("Cuenta no encontrada.");
                    break;

                case "3":
                    Console.Write("Número de cuenta: ");
                    numero = Console.ReadLine();
                    cuenta = banco.BuscarCuenta(numero);
                    if (cuenta != null)
                    {
                        Console.Write("Monto a retirar: ");
                        decimal monto = decimal.Parse(Console.ReadLine());
                        cuenta.Retirar(monto);
                        var op = new Operacion("Retiro", monto, $"Retiro de {monto} en cuenta {numero}");
                        banco.RegistrarOperacion(op);
                        banco.BuscarClientePorCuenta(cuenta)?.RegistrarOperacion(op);
                        Console.WriteLine("Retiro realizado.");
                    }
                    else
                        Console.WriteLine("Cuenta no encontrada.");
                    break;

                case "4":
                    Console.Write("Número de cuenta: ");
                    numero = Console.ReadLine();
                    cuenta = banco.BuscarCuenta(numero);
                    if (cuenta != null)
                    {
                        Console.Write("Monto a pagar: ");
                        decimal monto = decimal.Parse(Console.ReadLine());
                        cuenta.Pagar(monto);
                        var op = new Operacion("Pago", monto, $"Pago de {monto} desde cuenta {numero}");
                        banco.RegistrarOperacion(op);
                        banco.BuscarClientePorCuenta(cuenta)?.RegistrarOperacion(op);
                        Console.WriteLine("Pago realizado.");
                    }
                    else
                        Console.WriteLine("Cuenta no encontrada.");
                    break;

                case "5":
                    Console.Write("Número de cuenta origen: ");
                    string origen = Console.ReadLine();
                    Cuenta ctaOrigen = banco.BuscarCuenta(origen);
                    Console.Write("Número de cuenta destino: ");
                    string destino = Console.ReadLine();
                    Cuenta ctaDestino = banco.BuscarCuenta(destino);
                    if (ctaOrigen != null && ctaDestino != null)
                    {
                        Console.Write("Monto a transferir: ");
                        decimal monto = decimal.Parse(Console.ReadLine());
                        if (ctaOrigen.Retirar(monto))
                        {
                            ctaDestino.Depositar(monto);
                            var op = new Operacion("Transferencia", monto, $"Transferencia de {monto} desde cuenta {origen} a cuenta {destino}");
                            banco.RegistrarOperacion(op);
                            banco.BuscarClientePorCuenta(ctaOrigen)?.RegistrarOperacion(op);
                            Console.WriteLine("Transferencia realizada.");
                        }
                        else
                            Console.WriteLine("Fondos insuficientes en la cuenta origen.");
                    }
                    else
                        Console.WriteLine("Cuenta/s no encontrada/s.");
                    break;

                case "6":
                    Console.WriteLine("\n===== INFORME GLOBAL DE OPERACIONES =====");
                    foreach (var op in banco.OperacionesGlobales)
                        Console.WriteLine($"{op.Fecha} - {op.Detalle}");

                    Console.WriteLine("\n===== ESTADO FINAL DE CUENTAS =====");
                    foreach (var clienteInf in banco.Clientes)
                        foreach (var cta in clienteInf.Cuentas)
                            Console.WriteLine($"Cuenta {cta.Numero} - Saldo: {cta.Saldo}, Puntos: {cta.Puntos}");

                    Console.WriteLine("\n===== HISTORIAL DE OPERACIONES POR CLIENTE =====");
                    foreach (var cl in banco.Clientes)
                    {
                        Console.WriteLine($"Cliente: {cl.Nombre}");
                        foreach (var op in cl.HistorialOperaciones)
                            Console.WriteLine($"{op.Fecha} - {op.Detalle}");
                        Console.WriteLine();
                    }
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
    

