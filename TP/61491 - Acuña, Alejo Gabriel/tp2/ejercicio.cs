using System;
using System.Collections.Generic;
using System.Linq;


public abstract class Operacion
{
    public decimal Monto { get; protected set; }
    public DateTime Fecha { get; private set; }

    protected Operacion(decimal monto)
    {
        Monto = monto;
        Fecha = DateTime.Now;
    }

    public abstract void Ejecutar();
    public abstract string Detalle();
}

public class Deposito : Operacion
{
    private Cuenta CuentaDestino;

    public Deposito(Cuenta cuentaDestino, decimal monto) : base(monto)
    {
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar()
    {
        CuentaDestino.Depositar(Monto);
    }

    public override string Detalle()
    {
        return $"Depósito de {Monto:C} a [{CuentaDestino.Numero}/{CuentaDestino.Cliente.Nombre}]";
    }
}

public class Retiro : Operacion
{
    private Cuenta CuentaOrigen;

    public Retiro(Cuenta cuentaOrigen, decimal monto) : base(monto)
    {
        CuentaOrigen = cuentaOrigen;
    }

    public override void Ejecutar()
    {
        if (!CuentaOrigen.Extraer(Monto))
        {
            throw new InvalidOperationException("Fondos insuficientes.");
        }
    }

    public override string Detalle()
    {
        return $"Retiro de {Monto:C} de [{CuentaOrigen.Numero}/{CuentaOrigen.Cliente.Nombre}]";
    }
}

public class Transferencia : Operacion
{
    private Cuenta CuentaOrigen;
    private Cuenta CuentaDestino;

    public Transferencia(Cuenta cuentaOrigen, Cuenta cuentaDestino, decimal monto) : base(monto)
    {
        CuentaOrigen = cuentaOrigen;
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar()
    {
        if (!CuentaOrigen.Extraer(Monto))
        {
            throw new InvalidOperationException("Fondos insuficientes para la transferencia.");
        }
        CuentaDestino.Depositar(Monto);
    }

    public override string Detalle()
    {
        return $"Transferencia de {Monto:C} desde la cuenta {CuentaOrigen.Numero} a la cuenta {CuentaDestino.Numero}";
    }
}

public class Pago : Operacion
{
    private Cuenta CuentaOrigen;

    public Pago(Cuenta cuentaOrigen, decimal monto) : base(monto)
    {
        CuentaOrigen = cuentaOrigen;
    }

    public override void Ejecutar()
    {
        if (!CuentaOrigen.Extraer(Monto))
        {
            throw new InvalidOperationException("Fondos insuficientes para el pago.");
        }
        CuentaOrigen.AcumularPuntos(Monto);
    }

    public override string Detalle()
    {
        return $"Pago de {Monto:C} desde la cuenta {CuentaOrigen.Numero}";
    }
}

public abstract class Cuenta
{
    public string Numero { get; private set; }
    public decimal Saldo { get; protected set; }
    public decimal Puntos { get; protected set; }
    public List<Operacion> Historial { get; private set; }
    public Cliente Cliente { get; private set; } 

    public Cuenta(string numero, Cliente cliente)
    {
        Numero = numero;
        Cliente = cliente; 
        Saldo = 0;
        Puntos = 0;
        Historial = new List<Operacion>();
    }

    public void Depositar(decimal monto)
    {
        Saldo += monto;
    }

    public bool Extraer(decimal monto)
    {
        if (Saldo >= monto)
        {
            Saldo -= monto;
            return true;
        }
        return false;
    }

    public abstract void AcumularPuntos(decimal monto);

    public void RegistrarOperacion(Operacion operacion)
    {
        Historial.Add(operacion);
    }
}

public class CuentaOro : Cuenta
{
    public CuentaOro(string numero, Cliente cliente) : base(numero, cliente) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

public class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, Cliente cliente) : base(numero, cliente) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.02m;
    }
}

public class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, Cliente cliente) : base(numero, cliente) { }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += monto * 0.01m;
    }
}

public class Cliente
{
    public string Nombre { get; private set; }
    public string DNI { get; private set; }
    public List<Cuenta> Cuentas { get; private set; }

    public Cliente(string nombre, string dni)
    {
        Nombre = nombre;
        DNI = dni;
        Cuentas = new List<Cuenta>();
    }

    public void AgregarCuenta(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
    }

    public decimal SaldoTotal()
    {
        return Cuentas.Sum(c => c.Saldo);
    }

    public decimal PuntosTotal()
    {
        return Cuentas.Sum(c => c.Puntos);
    }
}

public class Banco
{
    public string Nombre { get; private set; }
    public List<Cliente> Clientes { get; private set; }
    public List<Operacion> HistorialGlobal { get; private set; }

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        HistorialGlobal = new List<Operacion>();
    }

    public void AgregarCliente(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public void RegistrarOperacion(Operacion operacion)
    {
        operacion.Ejecutar();
        HistorialGlobal.Add(operacion);
    }
}

Banco banco = new Banco("Banco Nac");

bool salir = false;

while (!salir)
{
    Console.Clear(); 
    Console.WriteLine("\n--- Menú del Banco ---");
    Console.WriteLine("1. Registrar Usuario");
    Console.WriteLine("2. Crear Cuenta");
    Console.WriteLine("3. Realizar Depósito");
    Console.WriteLine("4. Realizar Retiro");
    Console.WriteLine("5. Realizar Transferencia");
    Console.WriteLine("6. Realizar Pago");
    Console.WriteLine("7. Generar Informe");
    Console.WriteLine("8. Salir");
    Console.Write("Seleccione una opción: ");

    string opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1": 
            Console.Clear();
            string dniUsuario;
            bool dniValido = false;

            do
            {
                Console.Write("Ingrese el DNI del usuario (8 dígitos): ");
                dniUsuario = Console.ReadLine();

                if (dniUsuario.Length != 8 || !dniUsuario.All(char.IsDigit))
                {
                    Console.WriteLine("El DNI debe tener exactamente 8 dígitos y contener solo números. Intente nuevamente.");
                }
                else if (banco.Clientes.Any(c => c.DNI == dniUsuario))
                {
                    Console.WriteLine("El DNI ya está registrado. Intente con otro.");
                }
                else
                {
                    dniValido = true;
                }
            } while (!dniValido);

            string nombreUsuario;
            bool nombreValido = false;

            do
            {
                Console.Write("Ingrese el nombre del usuario: ");
                nombreUsuario = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(nombreUsuario) || !nombreUsuario.All(char.IsLetter))
                {
                    Console.WriteLine("El nombre del usuario debe contener solo letras. Intente nuevamente.");
                }
                else
                {
                    nombreValido = true;
                }
            } while (!nombreValido);

            Cliente nuevoCliente = new Cliente(nombreUsuario, dniUsuario);
            banco.AgregarCliente(nuevoCliente);
            Console.WriteLine($"Usuario {nombreUsuario} con DNI {dniUsuario} registrado exitosamente.");

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "2": 
            Console.Clear();
            Console.Write("Ingrese el DNI del usuario: ");
            string dniCuenta = Console.ReadLine();

            Cliente clienteCuenta = banco.Clientes.FirstOrDefault(c => c.DNI == dniCuenta);
            if (clienteCuenta == null)
            {
                Console.WriteLine("El usuario no está registrado. Por favor, registre al usuario antes de crear una cuenta.");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }

            string numeroCuenta;
            bool cuentaValida = false;
            do
            {
                Console.Write("Ingrese el número de la cuenta (5 dígitos): ");
                numeroCuenta = Console.ReadLine();

                if (numeroCuenta.Length != 5 || !numeroCuenta.All(char.IsDigit))
                {
                    Console.WriteLine("El número de cuenta debe tener exactamente 5 dígitos y contener solo números. Intente nuevamente.");
                }
                else if (banco.Clientes.SelectMany(c => c.Cuentas).Any(c => c.Numero == numeroCuenta))
                {
                    Console.WriteLine("El número de cuenta ya existe. Intente con otro.");
                }
                else
                {
                    cuentaValida = true;
                }
            } while (!cuentaValida);

            string tipoCuenta;
            bool tipoCuentaValido = false;
            do
            {
                Console.WriteLine("Seleccione el tipo de cuenta: 1. Oro, 2. Plata, 3. Bronce");
                tipoCuenta = Console.ReadLine();

                if (tipoCuenta != "1" && tipoCuenta != "2" && tipoCuenta != "3")
                {
                    Console.WriteLine("Opción inválida. Debe elegir 1, 2 o 3. Intente nuevamente.");
                }
                else
                {
                    tipoCuentaValido = true;
                }
            } while (!tipoCuentaValido);

            Cuenta nuevaCuenta = tipoCuenta switch
            {
                "1" => new CuentaOro(numeroCuenta, clienteCuenta),
                "2" => new CuentaPlata(numeroCuenta, clienteCuenta),
                "3" => new CuentaBronce(numeroCuenta, clienteCuenta),
                _ => null
            };

            if (nuevaCuenta != null)
            {
                clienteCuenta.AgregarCuenta(nuevaCuenta);
                Console.WriteLine($"Clave {numeroCuenta} creada con éxito para {clienteCuenta.Nombre}.");
            }
            else
            {
                Console.WriteLine("Tipo de cuenta inválido.");
            }

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "3":
            Console.Clear();
            Console.Write("Ingrese el número de la cuenta para el depósito: ");
            string cuentaDeposito = Console.ReadLine();
            Cuenta cuentaDep = banco.Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == cuentaDeposito);
            if (cuentaDep != null)
            {
                Console.WriteLine($"Cuenta seleccionada: {cuentaDep.Numero} | Cliente: {cuentaDep.Cliente.Nombre} | Saldo: {cuentaDep.Saldo:C}");
                Console.Write("Ingrese el monto a depositar: ");
                decimal montoDeposito = decimal.Parse(Console.ReadLine());
                banco.RegistrarOperacion(new Deposito(cuentaDep, montoDeposito));
                Console.WriteLine($"Depósito de {montoDeposito:C} realizado con éxito.");
            }
            else
            {
                Console.WriteLine("Cuenta no encontrada.");
            }
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "4":
            Console.Clear();
            Console.Write("Ingrese el número de la cuenta para el retiro: ");
            string cuentaRetiro = Console.ReadLine();
            Cuenta cuentaRet = banco.Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == cuentaRetiro);
            if (cuentaRet != null)
            {
                Console.WriteLine($"Cuenta seleccionada: {cuentaRet.Numero} | Cliente: {cuentaRet.Cliente.Nombre} | Saldo: {cuentaRet.Saldo:C}");
                bool operacionExitosa = false;
                while (!operacionExitosa)
                {
                    Console.Write("Ingrese el monto a retirar: ");
                    decimal montoRetiro = decimal.Parse(Console.ReadLine());
                    if (montoRetiro > cuentaRet.Saldo)
                    {
                        Console.WriteLine("El monto excede el saldo disponible.");
                        Console.WriteLine("1. Ingresar un nuevo monto");
                        Console.WriteLine("2. Regresar al menú principal");
                        string opcionRetiro = Console.ReadLine();
                        if (opcionRetiro == "2") break;
                    }
                    else
                    {
                        banco.RegistrarOperacion(new Retiro(cuentaRet, montoRetiro));
                        Console.WriteLine($"Retiro de {montoRetiro:C} realizado con éxito.");
                        operacionExitosa = true;
                    }
                }
            }
            else
            {
                Console.WriteLine("Cuenta no encontrada.");
            }
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "5":
            Console.Clear();
            Console.Write("Ingrese el número de la cuenta origen: ");
            string cuentaOrigen = Console.ReadLine();
            Cuenta cuentaOri = banco.Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == cuentaOrigen);
            if (cuentaOri != null)
            {
                Console.WriteLine($"Cuenta origen seleccionada: {cuentaOri.Numero} | Cliente: {cuentaOri.Cliente.Nombre} | Saldo: {cuentaOri.Saldo:C}");
            }
            else
            {
                Console.WriteLine("Cuenta origen no encontrada.");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }

            Console.Write("Ingrese el número de la cuenta destino: ");
            string cuentaDestino = Console.ReadLine();
            Cuenta cuentaDest = banco.Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == cuentaDestino);
            if (cuentaDest != null)
            {
                Console.WriteLine($"Cuenta destino seleccionada: {cuentaDest.Numero} | Cliente: {cuentaDest.Cliente.Nombre} | Saldo: {cuentaDest.Saldo:C}");
            }
            else
            {
                Console.WriteLine("Cuenta destino no encontrada.");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }

            bool operacionExitosaTransferencia = false;
            while (!operacionExitosaTransferencia)
            {
                Console.Write("Ingrese el monto a transferir: ");
                decimal montoTransferencia = decimal.Parse(Console.ReadLine());
                if (montoTransferencia > cuentaOri.Saldo)
                {
                    Console.WriteLine("El monto excede el saldo disponible en la cuenta origen.");
                    Console.WriteLine("1. Ingresar un nuevo monto");
                    Console.WriteLine("2. Regresar al menú principal");
                    string opcionTransferencia = Console.ReadLine();
                    if (opcionTransferencia == "2") break;
                }
                else
                {
                    banco.RegistrarOperacion(new Transferencia(cuentaOri, cuentaDest, montoTransferencia));
                    Console.WriteLine($"Transferencia de {montoTransferencia:C} realizada con éxito.");
                    operacionExitosaTransferencia = true;
                }
            }
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "6":
            Console.Clear();
            Console.Write("Ingrese el número de la cuenta para el pago: ");
            string cuentaPago = Console.ReadLine();
            Cuenta cuentaPag = banco.Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == cuentaPago);
            if (cuentaPag != null)
            {
                Console.WriteLine($"Cuenta seleccionada: {cuentaPag.Numero} | Cliente: {cuentaPag.Cliente.Nombre} | Saldo: {cuentaPag.Saldo:C}");
                bool operacionExitosaPago = false;
                while (!operacionExitosaPago)
                {
                    Console.Write("Ingrese el monto a pagar: ");
                    decimal montoPago = decimal.Parse(Console.ReadLine());
                    if (montoPago > cuentaPag.Saldo)
                    {
                        Console.WriteLine("El monto excede el saldo disponible.");
                        Console.WriteLine("1. Ingresar un nuevo monto");
                        Console.WriteLine("2. Regresar al menú principal");
                        string opcionPago = Console.ReadLine();
                        if (opcionPago == "2") break;
                    }
                    else
                    {
                        banco.RegistrarOperacion(new Pago(cuentaPag, montoPago));
                        Console.WriteLine($"Pago de {montoPago:C} realizado con éxito.");
                        operacionExitosaPago = true;
                    }
                }
            }
            else
            {
                Console.WriteLine("Cuenta no encontrada.");
            }
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "7": 
            Console.Clear();
            Console.WriteLine($"Banco: {banco.Nombre} | Clientes: {banco.Clientes.Count}");
            foreach (var clienteInforme in banco.Clientes)
            {
                Console.WriteLine($"  Cliente: {clienteInforme.Nombre} | Saldo Total: {clienteInforme.SaldoTotal():C} | Puntos Total: {clienteInforme.PuntosTotal():C}");
                foreach (var cuenta in clienteInforme.Cuentas)
                {
                    string tipoCuentaInforme = cuenta is CuentaOro ? "Oro" :
                                               cuenta is CuentaPlata ? "Plata" :
                                               cuenta is CuentaBronce ? "Bronce" : "Desconocido";

                    Console.WriteLine($"    Cuenta: {cuenta.Numero} | Tipo: {tipoCuentaInforme} | Saldo: {cuenta.Saldo:C} | Puntos: {cuenta.Puntos:C}");
                
                    foreach (var operacion in cuenta.Historial)
                    {
                        Console.WriteLine($"      - {operacion.Detalle()}");
                    }
                }
            }
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;

        case "8":
            salir = true;
            Console.WriteLine("Gracias, vuelva pronto.");
            break;

        default:
            Console.WriteLine("Opción inválida. Intente nuevamente.");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
    }
}