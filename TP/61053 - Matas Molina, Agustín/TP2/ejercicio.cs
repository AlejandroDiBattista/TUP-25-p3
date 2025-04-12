using System;
using System.Collections.Generic;

public class Banco
{
    public string Nombre { get; set; }
    private List<Cliente> clientes = new List<Cliente>();
    private List<Operacion> operaciones = new List<Operacion>();

    public Banco(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public Cliente ObtenerCliente(string nombre)
    {
        return clientes.Find(c => c.Nombre == nombre);
    }

    public Cuenta ObtenerCuenta(string numero)
    {
        foreach (var cliente in clientes)
        {
            var cuenta = cliente.ObtenerCuenta(numero);
            if (cuenta != null)
            {
                return cuenta;
            }
        }
        return null;
    }

    public void Registrar(Operacion operacion)
    {
        operaciones.Add(operacion);
        operacion.Ejecutar(this);
    }

    public void Informe()
    {
        Console.WriteLine($"Informe del Banco {Nombre}");
        foreach (var operacion in operaciones)
        {
            Console.WriteLine(operacion.Detalle());
        }
        foreach (var cliente in clientes)
        {
            cliente.Informe();
        }
    }

    public void RegistrarCliente()
    {
        Console.WriteLine("Ingrese su nombre para registrarse en el banco:");
        string nombreCliente = Console.ReadLine();
        Cliente cliente = new Cliente(nombreCliente);

        Console.WriteLine($"¿Cuántas cuentas desea registrar para {nombreCliente}?");
        if (!int.TryParse(Console.ReadLine(), out int cantidadCuentas) || cantidadCuentas <= 0)
        {
            Console.WriteLine("Cantidad de cuentas no válida. Intente nuevamente.");
            return;
        }

        for (int i = 0; i < cantidadCuentas; i++)
        {
            Console.WriteLine($"Ingrese el número de la cuenta {i + 1} (formato XXXXX):");
            string numeroCuenta = Console.ReadLine();

            Console.WriteLine("Seleccione el tipo de cuenta:");
            Console.WriteLine("1. Oro");
            Console.WriteLine("2. Plata");
            Console.WriteLine("3. Bronce");
            if (!int.TryParse(Console.ReadLine(), out int tipoCuenta) || tipoCuenta < 1 || tipoCuenta > 3)
            {
                Console.WriteLine("Tipo de cuenta inválido. Intente nuevamente.");
                i--; // Repetir el registro de esta cuenta
                continue;
            }

            Console.WriteLine($"Ingrese el saldo inicial para la cuenta {numeroCuenta}:");
            if (!double.TryParse(Console.ReadLine(), out double saldoInicial) || saldoInicial < 0)
            {
                Console.WriteLine("Saldo inicial no válido. Intente nuevamente.");
                i--; // Repetir el registro de esta cuenta
                continue;
            }

            switch (tipoCuenta)
            {
                case 1:
                    cliente.Agregar(new CuentaOro(numeroCuenta, saldoInicial));
                    break;
                case 2:
                    cliente.Agregar(new CuentaPlata(numeroCuenta, saldoInicial));
                    break;
                case 3:
                    cliente.Agregar(new CuentaBronce(numeroCuenta, saldoInicial));
                    break;
            }
        }

        Agregar(cliente);
        Console.WriteLine($"Cliente {nombreCliente} registrado exitosamente.");
    }
}

public class Cliente
{
    public string Nombre { get; set; }
    private List<Cuenta> cuentas = new List<Cuenta>();

    public Cliente(string nombre)
    {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta)
    {
        cuentas.Add(cuenta);
    }

    public Cuenta ObtenerCuenta(string numero)
    {
        return cuentas.Find(c => c.Numero == numero);
    }

    public void Informe()
    {
        Console.WriteLine($"Cliente: {Nombre}");
        foreach (var cuenta in cuentas)
        {
            Console.WriteLine(cuenta.Detalle());
        }
    }
}

public abstract class Cuenta
{
    public string Numero { get; set; }
    public double Saldo { get; set; }
    public int Puntos { get; set; }

    public Cuenta(string numero, double saldoinicial)
    {
        Numero = numero;
        Saldo = saldoinicial;
        Puntos = 0;
    }

    public abstract void AcumularPuntos(double monto);

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

    public string Detalle()
    {
        return $"Cuenta {Numero} - Saldo: {Saldo:C} - Puntos: {Puntos}";
    }
}

public class CuentaOro : Cuenta
{
    public CuentaOro(string numero, double saldoinicial) : base(numero, saldoinicial) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += (int)(monto >= 1000 ? monto * 0.05 : monto * 0.03);
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, double saldoinicial) : base(numero, saldoinicial) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += (int)(monto * 0.02);
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, double saldoinicial) : base(numero, saldoinicial) { }

    public override void AcumularPuntos(double monto)
    {
        Puntos += (int)(monto * 0.01);
    }
}

public abstract class Operacion
{
    public string NumeroCuenta { get; set; }
    public double Monto { get; set; }

    protected Operacion(string numeroCuenta, double monto)
    {
        NumeroCuenta = numeroCuenta;
        Monto = monto;
    }

    public abstract void Ejecutar(Banco banco);
    public abstract string Detalle();
}

public class Deposito : Operacion
{
    public Deposito(string numeroCuenta, double monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        cuenta?.Depositar(Monto);
    }

    public override string Detalle()
    {
        return $"Depósito en cuenta {NumeroCuenta} por {Monto:C}";
    }
}

public class Retiro : Operacion
{
    public Retiro(string numeroCuenta, double monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        if (cuenta != null && !cuenta.Extraer(Monto))
        {
            Console.WriteLine($"Fondos insuficientes en la cuenta {NumeroCuenta}");
        }
    }

    public override string Detalle()
    {
        return $"Retiro de cuenta {NumeroCuenta} por {Monto:C}";
    }
}

public class Pago : Operacion
{
    public Pago(string numeroCuenta, double monto) : base(numeroCuenta, monto) { }

    public override void Ejecutar(Banco banco)
    {
        var cuenta = banco.ObtenerCuenta(NumeroCuenta);
        if (cuenta != null && cuenta.Extraer(Monto))
        {
            cuenta.AcumularPuntos(Monto);
        }
        else
        {
            Console.WriteLine($"Fondos insuficientes para el pago en la cuenta {NumeroCuenta}");
        }
    }

    public override string Detalle()
    {
        return $"Pago desde cuenta {NumeroCuenta} por {Monto:C}";
    }
}

public class Transferencia : Operacion
{
    public string CuentaDestino { get; set; }

    public Transferencia(string cuentaOrigen, string cuentaDestino, double monto) : base(cuentaOrigen, monto)
    {
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar(Banco banco)
    {
        var cuentaOrigen = banco.ObtenerCuenta(NumeroCuenta);
        var cuentaDestino = banco.ObtenerCuenta(CuentaDestino);

        if (cuentaOrigen != null && cuentaDestino != null && cuentaOrigen.Extraer(Monto))
        {
            cuentaDestino.Depositar(Monto);
            Console.WriteLine($"Transferencia realizada de {NumeroCuenta} a {CuentaDestino} por {Monto:C}");
        }
        else
        {
            Console.WriteLine($"Error en la transferencia de {NumeroCuenta} a {CuentaDestino}. Verifique los datos.");
        }
    }

    public override string Detalle()
    {
        return $"Transferencia de {NumeroCuenta} a {CuentaDestino} por {Monto:C}";
    }
}

public class InteraccionUsuario
{
    Console.WriteLine("BIENVENIDO AL BANCO");
    public static void Menu(Banco banco)
    {
        bool salir = false;

        while (!salir)
        {
            Console.WriteLine("\nSeleccione una operación:");
            Console.WriteLine("1. Registrarse");
            Console.WriteLine("2. Iniciar sesión");
            Console.WriteLine("3. Salir");

            if (!int.TryParse(Console.ReadLine(), out int opcion))
            {
                Console.WriteLine("Entrada no válida. Por favor, ingrese un número.");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    banco.RegistrarCliente();
                    break;

                case 2:
                    Console.WriteLine("Ingrese su nombre para iniciar sesión:");
                    string nombreCliente = Console.ReadLine();
                    Cliente cliente = banco.ObtenerCliente(nombreCliente);

                    if (cliente != null)
                    {
                        Console.WriteLine($"Bienvenido, {nombreCliente}.");
                        MenuUsuario(banco, cliente);
                    }
                    else
                    {
                        Console.WriteLine("Cliente no encontrado. Por favor, regístrese primero.");
                    }
                    break;

                case 3:
                    salir = true;
                    Console.WriteLine("Gracias por usar el sistema bancario. ¡Hasta luego!");
                    break;

                default:
                    Console.WriteLine("Opción no válida. Intente nuevamente.");
                    break;
            }
        }
    }

    public static void MenuUsuario(Banco banco, Cliente cliente)
    {
        bool salir = false;

        while (!salir)
        {
            Console.WriteLine("\nSeleccione una operación:");
            Console.WriteLine("1. Depositar");
            Console.WriteLine("2. Retirar");
            Console.WriteLine("3. Pagar");
            Console.WriteLine("4. Transferir");
            Console.WriteLine("5. Mostrar informe");
            Console.WriteLine("6. Salir");

            if (!int.TryParse(Console.ReadLine(), out int opcion))
            {
                Console.WriteLine("Entrada no válida. Por favor, ingrese un número.");
                continue;
            }

            switch (opcion)
            {
                case 1:
                    Console.WriteLine("Ingrese el número de cuenta:");
                    string cuentaDeposito = Console.ReadLine();
                    Console.WriteLine("Ingrese el monto a depositar:");
                    if (!double.TryParse(Console.ReadLine(), out double montoDeposito) || montoDeposito <= 0)
                    {
                        Console.WriteLine("Monto no válido. Intente nuevamente.");
                        continue;
                    }
                    banco.Registrar(new Deposito(cuentaDeposito, montoDeposito));
                    Console.WriteLine("Depósito realizado exitosamente.");
                    break;

                case 2:
                    Console.WriteLine("Ingrese el número de cuenta:");
                    string cuentaRetiro = Console.ReadLine();
                    Console.WriteLine("Ingrese el monto a retirar:");
                    if (!double.TryParse(Console.ReadLine(), out double montoRetiro) || montoRetiro <= 0)
                    {
                        Console.WriteLine("Monto no válido. Intente nuevamente.");
                        continue;
                    }
                    banco.Registrar(new Retiro(cuentaRetiro, montoRetiro));
                    Console.WriteLine("Retiro realizado exitosamente.");
                    break;

                case 3:
                    Console.WriteLine("Ingrese el número de cuenta:");
                    string cuentaPago = Console.ReadLine();
                    Console.WriteLine("Ingrese el monto a pagar:");
                    if (!double.TryParse(Console.ReadLine(), out double montoPago) || montoPago <= 0)
                    {
                        Console.WriteLine("Monto no válido. Intente nuevamente.");
                        continue;
                    }
                    banco.Registrar(new Pago(cuentaPago, montoPago));
                    Console.WriteLine("Pago realizado exitosamente.");
                    break;

                case 4:
                    Console.WriteLine("Ingrese el número de cuenta origen:");
                    string cuentaOrigen = Console.ReadLine();
                    Console.WriteLine("Ingrese el número de cuenta destino:");
                    string cuentaDestino = Console.ReadLine();
                    Console.WriteLine("Ingrese el monto a transferir:");
                    if (!double.TryParse(Console.ReadLine(), out double montoTransferencia) || montoTransferencia <= 0)
                    {
                        Console.WriteLine("Monto no válido. Intente nuevamente.");
                        continue;
                    }
                    banco.Registrar(new Transferencia(cuentaOrigen, cuentaDestino, montoTransferencia));
                    Console.WriteLine("Transferencia realizada exitosamente.");
                    break;

                case 5:
                    cliente.Informe();
                    break;

                case 6:
                    salir = true;
                    Console.WriteLine("Sesión cerrada. ¡Hasta luego!");
                    break;

                default:
                    Console.WriteLine("Opción no válida. Intente nuevamente.");
                    break;
            }
        }
    }
}
Banco banco = new Banco("Banco Ejemplo");
InteraccionUsuario.Menu(banco);



