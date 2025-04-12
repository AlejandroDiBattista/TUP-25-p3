using System;
using System.Collections.Generic;
using System.Linq;

// Clase Banco
public class Banco 
{
    public string Nombre { get; }
    public List<Cliente> Clientes { get; }
    public List<Operacion> Operaciones { get; }

    public Banco(string nombre)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Clientes = new List<Cliente>();
        Operaciones = new List<Operacion>();
    }

    public void AgregarCliente(Cliente cliente)
    {
        if (cliente == null)
            throw new ArgumentNullException(nameof(cliente));
        
        Clientes.Add(cliente);
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        if (operacion == null)
            throw new ArgumentNullException(nameof(operacion));
        
        if (operacion.Ejecutar())
        {
            Operaciones.Add(operacion);
            operacion.Origen.RegistrarOperacion(operacion);
        }
    }

    public void MostrarInforme()
    {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}");
        
        foreach (var cliente in Clientes)
        {
            cliente.MostrarInforme();
        }
    }
}

// Clase Cliente
public class Cliente 
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; }

    public Cliente(string nombre)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Cuentas = new List<Cuenta>();
    }

    public void AgregarCuenta(Cuenta cuenta)
    {
        if (cuenta == null)
            throw new ArgumentNullException(nameof(cuenta));
        
        Cuentas.Add(cuenta);
    }

    public void MostrarInforme()
    {
        Console.WriteLine($"\n  Cliente: {Nombre} | Saldo Total: {Cuentas.Sum(c => c.Saldo):C} | Puntos Total: {Cuentas.Sum(c => c.Puntos):C}");
        
        foreach (var cuenta in Cuentas)
        {
            cuenta.MostrarInforme();
        }
    }
}

// Clase abstracta Cuenta
public abstract class Cuenta 
{
    public string Numero { get; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }
    public List<Operacion> Historial { get; }

    protected Cuenta(string numero, decimal saldo)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("El número de cuenta no puede estar vacío", nameof(numero));
        
        if (saldo < 0)
            throw new ArgumentException("El saldo inicial no puede ser negativo", nameof(saldo));

        Numero = numero;
        Saldo = saldo;
        Puntos = 0;
        Historial = new List<Operacion>();
    }

    public virtual bool Depositar(decimal monto)
    {
        if (monto <= 0)
            return false;
            
        Saldo += monto;
        return true;
    }

    public virtual bool Retirar(decimal monto)
    {
        if (monto <= 0 || monto > Saldo)
            return false;
            
        Saldo -= monto;
        return true;
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        if (operacion == null)
            throw new ArgumentNullException(nameof(operacion));
        
        Historial.Add(operacion);
    }

    public void MostrarInforme()
    {
        Console.WriteLine($"    Cuenta: {Numero} | Saldo: {Saldo:C} | Puntos: {Puntos:C}");
        
        foreach (var operacion in Historial)
        {
            Console.WriteLine($"      - {operacion.Descripcion}");
        }
    }

    public abstract void AcumularPuntos(decimal monto);
}

// Clases derivadas de Cuenta
public class CuentaOro : Cuenta 
{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

public class CuentaPlata : Cuenta 
{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.02m;
    }
}

public class CuentaBronce : Cuenta 
{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.01m;
    }
}

// Clase abstracta Operacion
public abstract class Operacion 
{
    public Cuenta Origen { get; }
    public decimal Monto { get; }

    protected Operacion(Cuenta origen, decimal monto)
    {
        Origen = origen ?? throw new ArgumentNullException(nameof(origen));
        
        if (monto <= 0)
            throw new ArgumentException("El monto debe ser positivo", nameof(monto));
        
        Monto = monto;
    }

    public abstract bool Ejecutar();
    public abstract string Descripcion { get; }
}

// Clases derivadas de Operacion
public class Deposito : Operacion 
{
    public Deposito(Cuenta origen, decimal monto) : base(origen, monto) { }

    public override bool Ejecutar() => Origen.Depositar(Monto);

    public override string Descripcion => $"Depósito de {Monto:C}";
}

public class Retiro : Operacion 
{
    public Retiro(Cuenta origen, decimal monto) : base(origen, monto) { }

    public override bool Ejecutar() => Origen.Retirar(Monto);

    public override string Descripcion => $"Retiro de {Monto:C}";
}

public class Pago : Operacion 
{
    public Pago(Cuenta origen, decimal monto) : base(origen, monto) { }

    public override bool Ejecutar()
    {
        if (Origen.Retirar(Monto))
        {
            Origen.AcumularPuntos(Monto);
            return true;
        }
        return false;
    }

    public override string Descripcion => $"Pago de {Monto:C}";
}

public class Transferencia : Operacion 
{
    public Cuenta Destino { get; }

    public Transferencia(Cuenta origen, Cuenta destino, decimal monto) 
        : base(origen, monto)
    {
        Destino = destino ?? throw new ArgumentNullException(nameof(destino));
    }

    public override bool Ejecutar()
    {
        if (Origen.Retirar(Monto))
        {
            if (Destino.Depositar(Monto))
            {
                return true;
            }
            Origen.Depositar(Monto); // Revertir si falla
        }
        return false;
    }

    public override string Descripcion => $"Transferencia de {Monto:C} a {Destino.Numero}";
}

// Código principal (para dotnet script)
var banco = new Banco("Banco Tucumán");

while (true)
{
    Console.Clear();
    MostrarMenuPrincipal();
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            AgregarCliente();
            break;
        case "2":
            RealizarOperacion();
            break;
        case "3":
            MostrarInforme();
            break;
        case "4":
            return;
        default:
            Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
            Console.ReadKey();
            break;
    }
}

void MostrarMenuPrincipal()
{
    Console.WriteLine("=====================================");
    Console.WriteLine($"       {banco.Nombre}");
    Console.WriteLine("=====================================");
    Console.WriteLine("1. Agregar Cliente");
    Console.WriteLine("2. Realizar Operación");
    Console.WriteLine("3. Mostrar Informe");
    Console.WriteLine("4. Salir");
    Console.Write("Seleccione una opción: ");
}

void AgregarCliente()
{
    Console.Clear();
    Console.WriteLine("=== Agregar Cliente ===");
    Console.Write("Ingrese el nombre del cliente: ");
    var nombre = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(nombre))
    {
        MostrarError("El nombre del cliente no puede estar vacío");
        return;
    }

    var cliente = new Cliente(nombre);

    Console.Write("¿Cuántas cuentas desea agregar? ");
    if (!int.TryParse(Console.ReadLine(), out int cantidadCuentas) || cantidadCuentas <= 0)
    {
        MostrarError("Cantidad inválida de cuentas");
        return;
    }

    for (int i = 0; i < cantidadCuentas; i++)
    {
        Console.WriteLine($"\nCuenta {i + 1}:");
        Console.Write("Ingrese el número de cuenta: ");
        var numero = Console.ReadLine();

        Console.Write("Ingrese el saldo inicial: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal saldo) || saldo < 0)
        {
            MostrarError("Saldo inválido. Cuenta no agregada.");
            continue;
        }

        Console.WriteLine("Seleccione el tipo de cuenta:");
        Console.WriteLine("1. Oro");
        Console.WriteLine("2. Plata");
        Console.WriteLine("3. Bronce");
        var tipo = Console.ReadLine();

        Cuenta cuenta = tipo switch
        {
            "1" => new CuentaOro(numero, saldo),
            "2" => new CuentaPlata(numero, saldo),
            "3" => new CuentaBronce(numero, saldo),
            _ => null
        };

        if (cuenta != null)
        {
            cliente.AgregarCuenta(cuenta);
        }
        else
        {
            MostrarError("Tipo de cuenta inválido. Cuenta no agregada.");
        }
    }

    banco.AgregarCliente(cliente);
    MostrarMensajeExito("Cliente agregado exitosamente");
}

void RealizarOperacion()
{
    Console.Clear();
    Console.WriteLine("=== Realizar Operación ===");
    
    if (!banco.Clientes.Any())
    {
        MostrarError("No hay clientes registrados");
        return;
    }

    Console.Write("Ingrese el nombre del cliente: ");
    var nombreCliente = Console.ReadLine();
    var cliente = banco.Clientes.FirstOrDefault(c => c.Nombre.Equals(nombreCliente, StringComparison.OrdinalIgnoreCase));

    if (cliente == null)
    {
        MostrarError("Cliente no encontrado");
        return;
    }

    Console.WriteLine("Seleccione el tipo de operación:");
    Console.WriteLine("1. Depósito");
    Console.WriteLine("2. Retiro");
    Console.WriteLine("3. Pago");
    Console.WriteLine("4. Transferencia");
    var tipoOperacion = Console.ReadLine();

    if (!MostrarCuentasCliente(cliente))
        return;

    Console.Write("Seleccione una cuenta: ");
    if (!int.TryParse(Console.ReadLine(), out int indiceCuenta) || indiceCuenta <= 0 || indiceCuenta > cliente.Cuentas.Count)
    {
        MostrarError("Cuenta inválida");
        return;
    }

    var cuentaOrigen = cliente.Cuentas[indiceCuenta - 1];

    Console.Write("Ingrese el monto: ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal monto) || monto <= 0)
    {
        MostrarError("Monto inválido");
        return;
    }

    Operacion operacion = tipoOperacion switch
    {
        "1" => new Deposito(cuentaOrigen, monto),
        "2" => new Retiro(cuentaOrigen, monto),
        "3" => new Pago(cuentaOrigen, monto),
        "4" => CrearTransferencia(cuentaOrigen, monto),
        _ => null
    };

    if (operacion != null)
    {
        banco.RegistrarOperacion(operacion);
        MostrarMensajeExito("Operación realizada exitosamente");
    }
    else
    {
        MostrarError("Tipo de operación inválido");
    }
}

bool MostrarCuentasCliente(Cliente cliente)
{
    if (!cliente.Cuentas.Any())
    {
        MostrarError("El cliente no tiene cuentas registradas");
        return false;
    }

    Console.WriteLine("Cuentas del cliente:");
    for (int i = 0; i < cliente.Cuentas.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {cliente.Cuentas[i].Numero} (Saldo: {cliente.Cuentas[i].Saldo:C})");
    }

    return true;
}

Operacion CrearTransferencia(Cuenta cuentaOrigen, decimal monto)
{
    Console.Write("Ingrese el número de cuenta destino: ");
    var numeroDestino = Console.ReadLine();

    var cuentaDestino = banco.Clientes
        .SelectMany(c => c.Cuentas)
        .FirstOrDefault(c => c.Numero.Equals(numeroDestino, StringComparison.OrdinalIgnoreCase));

    if (cuentaDestino != null)
    {
        return new Transferencia(cuentaOrigen, cuentaDestino, monto);
    }

    MostrarError("Cuenta destino no encontrada. Operación cancelada.");
    return null;
}

void MostrarInforme()
{
    Console.Clear();
    Console.WriteLine("=== Informe del Banco ===");
    banco.MostrarInforme();
    Console.WriteLine("\nPresione una tecla para continuar...");
    Console.ReadKey();
}

void MostrarError(string mensaje)
{
    Console.WriteLine($"Error: {mensaje}");
    Console.WriteLine("Presione una tecla para continuar...");
    Console.ReadKey();
}

void MostrarMensajeExito(string mensaje)
{
    Console.WriteLine(mensaje);
    Console.WriteLine("Presione una tecla para continuar...");
    Console.ReadKey();
}