using System;
using System.Collections.Generic;
using System.Linq;

class Cuenta
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public Cliente Titular { get; }
    public decimal Puntos { get; protected set; }

    public Cuenta(string numero, decimal saldoInicial, Cliente titular)
    {
        Numero = numero;
        Saldo = saldoInicial;
        Titular = titular;
        Puntos = 0;
    }

    public virtual void AcumularPuntos(decimal monto) {}
    public void Depositar(decimal monto) => Saldo += monto;
    public bool Extraer(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }
    public override void AcumularPuntos(decimal monto) => Puntos += monto >= 1000 ? monto * 0.05m : monto * 0.03m;
}

class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }
    public override void AcumularPuntos(decimal monto) => Puntos += monto * 0.02m;
}

class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldoInicial, Cliente titular) : base(numero, saldoInicial, titular) { }
    public override void AcumularPuntos(decimal monto) => Puntos += monto * 0.01m;
}

class Cliente
{
    public int Id { get; }
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; }

    public Cliente(int id, string nombre)
    {
        Id = id;
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void AgregarCuenta(Cuenta cuenta) => Cuentas.Add(cuenta);
}

class Operacion
{
    public decimal Monto { get; }
    public string CuentaOrigen { get; }
    public string CuentaDestino { get; }

    public Operacion(decimal monto, string cuentaOrigen, string cuentaDestino = null)
    {
        Monto = monto;
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
    }

    public virtual void Ejecutar(Banco banco) {}
}

class Deposito : Operacion
{
    public Deposito(string cuentaDestino, decimal monto) : base(monto, cuentaDestino) { }
    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(CuentaOrigen);
        cuenta?.Depositar(Monto);
    }
}

class Retiro : Operacion
{
    public Retiro(string cuentaOrigen, decimal monto) : base(monto, cuentaOrigen) { }
    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(CuentaOrigen);
        cuenta?.Extraer(Monto);
    }
}

class Pago : Operacion
{
    public Pago(string cuentaOrigen, decimal monto) : base(monto, cuentaOrigen) { }
    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(CuentaOrigen);
        if (cuenta?.Extraer(Monto) == true)
            cuenta.AcumularPuntos(Monto);
    }
}

class Transferencia : Operacion
{
    public Transferencia(string cuentaOrigen, string cuentaDestino, decimal monto) : base(monto, cuentaOrigen, cuentaDestino) { }
    public override void Ejecutar(Banco banco)
    {
        var origen = banco.ObtenerCuenta(CuentaOrigen);
        var destino = banco.ObtenerCuenta(CuentaDestino);
        if (origen?.Extraer(Monto) == true)
            destino?.Depositar(Monto);
    }
}

class Banco
{
    public string Nombre { get; }
    private List<Cliente> clientes;
    private List<Operacion> operaciones;
    private int ultimoId = 1;

    public Banco(string nombre)
    {
        Nombre = nombre;
        clientes = new List<Cliente>();
        operaciones = new List<Operacion>();
    }

    public Cliente AgregarCliente(string nombre)
    {
        var cliente = new Cliente(ultimoId++, nombre);
        clientes.Add(cliente);
        return cliente;
    }

    public Cliente ObtenerClientePorId(int id) => clientes.FirstOrDefault(c => c.Id == id);
    public Cuenta ObtenerCuenta(string numero) => clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numero);

    public void RegistrarOperacion(Operacion operacion)
    {
        operaciones.Add(operacion);
        operacion.Ejecutar(this);
    }

    public void GenerarInforme()
    {
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {clientes.Count}");
        foreach (var cliente in clientes)
        {
            decimal saldoTotal = cliente.Cuentas.Sum(c => c.Saldo);
            decimal puntosTotal = cliente.Cuentas.Sum(c => c.Puntos);
            Console.WriteLine($"  Cliente: {cliente.Nombre} (ID: {cliente.Id}) | Saldo Total: $ {saldoTotal:F2} | Puntos: $ {puntosTotal:F2}");
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:F2} | Puntos: $ {cuenta.Puntos:F2}");
            }
        }
        Console.WriteLine();
    }
}

// Menú
var banco = new Banco("Banco Nac");

while (true)
{
    Console.WriteLine("\n--- MENÚ BANCO ---");
    Console.WriteLine("1. Crear cliente");
    Console.WriteLine("2. Agregar cuenta a cliente");
    Console.WriteLine("3. Realizar depósito");
    Console.WriteLine("4. Realizar retiro");
    Console.WriteLine("5. Realizar transferencia");
    Console.WriteLine("6. Realizar pago");
    Console.WriteLine("7. Ver informe");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            Console.Write("Nombre del cliente: ");
            var nombre = Console.ReadLine();
            var nuevoCliente = banco.AgregarCliente(nombre);
            Console.WriteLine($"Cliente creado con ID: {nuevoCliente.Id}");
            break;

        case "2":
            Console.Write("ID del cliente: ");
            if (!int.TryParse(Console.ReadLine(), out int idCliente))
            {
                MostrarError("ID inválido.");
                break;
            }

            var cliente = banco.ObtenerClientePorId(idCliente);
            if (cliente == null)
            {
                MostrarError("Cliente no encontrado.");
                break;
            }

            Console.Write("Tipo de cuenta (Oro/Plata/Bronce): ");
            var tipo = Console.ReadLine()?.ToLower();
            Console.Write("Número de cuenta: ");
            var numero = Console.ReadLine();
            Console.Write("Saldo inicial: ");
            decimal saldo = decimal.Parse(Console.ReadLine());

            Cuenta cuenta = tipo switch
            {
                "oro" => new CuentaOro(numero, saldo, cliente),
                "plata" => new CuentaPlata(numero, saldo, cliente),
                "bronce" => new CuentaBronce(numero, saldo, cliente),
                _ => null
            };

            if (cuenta != null)
            {
                cliente.AgregarCuenta(cuenta);
                Console.WriteLine("Cuenta agregada.");
            }
            else
            {
                MostrarError("Tipo de cuenta inválido.");
            }
            break;

        case "3":
            Console.Write("Número de cuenta destino: ");
            var cd = Console.ReadLine();
            Console.Write("Monto a depositar: ");
            decimal md = decimal.Parse(Console.ReadLine());
            banco.RegistrarOperacion(new Deposito(cd, md));
            Console.WriteLine("Depósito realizado.");
            break;

        case "4":
            Console.Write("Número de cuenta origen: ");
            var cr = Console.ReadLine();
            Console.Write("Monto a retirar: ");
            decimal mr = decimal.Parse(Console.ReadLine());
            banco.RegistrarOperacion(new Retiro(cr, mr));
            Console.WriteLine("Retiro realizado.");
            break;

        case "5":
            Console.Write("Cuenta origen: ");
            var co = Console.ReadLine();
            Console.Write("Cuenta destino: ");
            var cdest = Console.ReadLine();
            Console.Write("Monto: ");
            decimal mt = decimal.Parse(Console.ReadLine());
            banco.RegistrarOperacion(new Transferencia(co, cdest, mt));
            Console.WriteLine("Transferencia realizada.");
            break;

        case "6":
            Console.Write("Cuenta a pagar: ");
            var cp = Console.ReadLine();
            Console.Write("Monto: ");
            decimal mp = decimal.Parse(Console.ReadLine());
            banco.RegistrarOperacion(new Pago(cp, mp));
            Console.WriteLine("Pago realizado.");
            break;

        case "7":
            banco.GenerarInforme();
            break;

        case "0":
            Console.WriteLine("Saliendo...");
            return;

        default:
            MostrarError("Opción no válida.");
            break;
    }
}

void MostrarError(string mensaje)
{
    var anterior = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("ERROR: " + mensaje);
    Console.ForegroundColor = anterior;
}

