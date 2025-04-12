using System;
using System.Collections.Generic;

abstract class Operacion
{
    public DateTime Fecha { get; }
    public decimal Monto { get; }
    public string Descripcion { get; }

    protected Operacion(decimal monto, string descripcion)
    {
        Fecha = DateTime.Now;
        Monto = monto;
        Descripcion = descripcion;
    }

    public abstract void Ejecutar();
}

class Deposito : Operacion
{
    private Cuenta cuenta;

    public Deposito(Cuenta cuenta, decimal monto) : base(monto, "Depósito")
    {
        this.cuenta = cuenta;
    }

    public override void Ejecutar()
    {
        cuenta.Saldo += Monto;
        cuenta.AcumularPuntos(Monto);
        Console.WriteLine($"Depósito de ${Monto} realizado en cuenta {cuenta.NumeroCuenta}");
    }
}

class Retiro : Operacion
{
    private Cuenta cuenta;

    public Retiro(Cuenta cuenta, decimal monto) : base(monto, "Retiro")
    {
        this.cuenta = cuenta;
    }

    public override void Ejecutar()
    {
        if (cuenta.Saldo >= Monto)
        {
            cuenta.Saldo -= Monto;
            Console.WriteLine($"Retiro de ${Monto} realizado en cuenta {cuenta.NumeroCuenta}");
        }
        else
        {
            Console.WriteLine($"Fondos insuficientes en cuenta {cuenta.NumeroCuenta}.");
        }
    }
}

class Pago : Operacion
{
    private Cuenta cuenta;

    public Pago(Cuenta cuenta, decimal monto) : base(monto, "Pago")
    {
        this.cuenta = cuenta;
    }

    public override void Ejecutar()
    {
        if (cuenta.Saldo >= Monto)
        {
            cuenta.ProcesarPago(Monto);
            cuenta.AcumularPuntos(Monto);
            Console.WriteLine($"Pago de ${Monto} realizado en cuenta {cuenta.NumeroCuenta}");
        }
        else
        {
            Console.WriteLine($"Fondos insuficientes para realizar el pago en cuenta {cuenta.NumeroCuenta}");
        }
    }
}

class Transferencia : Operacion
{
    private Cuenta cuentaOrigen;
    private Cuenta cuentaDestino;

    public Transferencia(Cuenta origen, Cuenta destino, decimal monto) : base(monto, "Transferencia")
    {
        cuentaOrigen = origen;
        cuentaDestino = destino;
    }

    public override void Ejecutar()
    {
        if (cuentaOrigen.Saldo >= Monto)
        {
            cuentaOrigen.Saldo -= Monto;
            cuentaDestino.Saldo += Monto;

            cuentaOrigen.AcumularPuntos(Monto);
            cuentaDestino.AcumularPuntos(Monto);

            Console.WriteLine($"Transferencia de ${Monto} realizada desde {cuentaOrigen.NumeroCuenta} hacia {cuentaDestino.NumeroCuenta}");
        }
        else
        {
            Console.WriteLine($"Fondos insuficientes en cuenta origen {cuentaOrigen.NumeroCuenta}");
        }
    }
}


abstract class Cuenta
{
    private static int contadorCuentas = 10000;

    public string NumeroCuenta { get; }
    public decimal Saldo { get; set; }

    protected Cuenta()
    {
        NumeroCuenta = (++contadorCuentas).ToString();
        Saldo = 0;
    }

    public abstract void ProcesarPago(decimal monto);

    public virtual void AcumularPuntos(decimal monto) { }
}

class CuentaOro : Cuenta
{
    public int Puntos { get; private set; }

    public override void ProcesarPago(decimal monto)
    {
        Saldo += monto;
        Console.WriteLine($"Pago de ${monto} procesado en la cuenta {NumeroCuenta}");
    }

    public override void AcumularPuntos(decimal monto)
    {
        decimal porcentaje = monto > 1000 ? 0.05m : 0.03m;
        Puntos += (int)(monto * porcentaje);
        Console.WriteLine($"Se acumularon {((int)(monto * porcentaje))} puntos en la cuenta {NumeroCuenta}. Total: {Puntos}");
    }
}

class CuentaPlata : Cuenta
{
    public int Puntos { get; private set; }

    public override void ProcesarPago(decimal monto)
    {
        Saldo += monto;
        Console.WriteLine($"Pago de ${monto} procesado en la cuenta {NumeroCuenta}");
    }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += (int)(monto * 0.02m);
        Console.WriteLine($"Se acumularon {(int)(monto * 0.02m)} puntos en la cuenta {NumeroCuenta}. Total: {Puntos}");
    }
}

class CuentaBronce : Cuenta
{
    public int Puntos { get; private set; }

    public override void ProcesarPago(decimal monto)
    {
        Saldo += monto;
        Console.WriteLine($"Pago de ${monto} procesado en la cuenta {NumeroCuenta}");
    }

    public override void AcumularPuntos(decimal monto)
    {
        Puntos += (int)(monto * 0.01m);
        Console.WriteLine($"Se acumularon {(int)(monto * 0.01m)} puntos en la cuenta {NumeroCuenta}. Total: {Puntos}");
    }
}

class Cliente
{
    public string Nombre { get; }
    public List<Cuenta> Cuentas { get; }

    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta>();
    }

    public void AgregarCuenta(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
        Console.WriteLine($"Cuenta {cuenta.NumeroCuenta} asignada a {Nombre}.");
    }
}

class Banco
{
    private List<Cliente> clientes;
    private List<Operacion> operaciones;

    public Banco()
    {
        clientes = new List<Cliente>();
        operaciones = new List<Operacion>();
    }

    public void AgregarCliente(Cliente cliente)
    {
        clientes.Add(cliente);
        Console.WriteLine($"Cliente {cliente.Nombre} ya está registrado.");
    }

    public Cliente BuscarCliente(string nombre)
    {
        return clientes.Find(c => c.Nombre == nombre);
    }

    public void EjecutarOperacion(Operacion operacion)
    {
        operacion.Ejecutar();
        operaciones.Add(operacion);
    }

   public void MostrarClientes()
{
    Console.WriteLine("Clientes del Banco:");
    foreach (var cliente in clientes)
    {
        Console.WriteLine($"- {cliente.Nombre}");
        foreach (var cuenta in cliente.Cuentas)
        {
            string puntosInfo = "";

            
            if (cuenta is CuentaOro cuentaOro)
            {
                puntosInfo = $" | Puntos acumulados: {cuentaOro.Puntos}";
            }
            else if (cuenta is CuentaPlata cuentaPlata)
            {
                puntosInfo = $" | Puntos acumulados: {cuentaPlata.Puntos}";
            }
            else if (cuenta is CuentaBronce cuentaBronce)
            {
                puntosInfo = $" | Puntos acumulados: {cuentaBronce.Puntos}";
            }

            Console.WriteLine($" La Cuenta: {cuenta.NumeroCuenta} | Saldo: ${cuenta.Saldo}{puntosInfo}.");
        }
    }
}
    
}


        Banco banco = new Banco();
        bool salir = false;

        while (!salir)
        {
            Console.Clear();
            Console.WriteLine("BIENVENIDO AL MENÚ DEL BANCO");
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Si eres nuevo, #1 registrate como cliente y #2 Crea una cuenta .");
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine("1. Registrar cliente");
            Console.WriteLine("2. Crear cuenta para cliente");
            Console.WriteLine("3. Depositar dinero");
            Console.WriteLine("4. Extraer dinero");
            Console.WriteLine("5. Realizar pago");
            Console.WriteLine("6. Transferir dinero");
            Console.WriteLine("7. Informe de clientes");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opción: ");
            
            string opcion = Console.ReadLine();
            switch (opcion)
            {
                case "1":
                    Console.Write("Ingrese el nombre del nuevo cliente: ");
                    string nombre = Console.ReadLine();
                    banco.AgregarCliente(new Cliente(nombre));
                    Console.WriteLine($"Presiona una tecla para continuar...");
                    Console.ReadKey();
                    break;

                case "2":
                    Console.Write("Ingrese el nombre del cliente: ");
                    string nombreCliente = Console.ReadLine();
                    Cliente cliente = banco.BuscarCliente(nombreCliente);

                    if (cliente == null)
                    {
                        Console.WriteLine("El Cliente no fue encontrado.");
                        break;
                    }

                    Console.WriteLine("Seleccione el tipo de cuenta:");
                    Console.WriteLine("1. Oro");
                    Console.WriteLine("2. Plata");
                    Console.WriteLine("3. Bronce");
                    Console.Write("Opción: ");
                    string tipoCuenta = Console.ReadLine();

                    Cuenta nuevaCuenta = tipoCuenta 
                    switch
                    {
                        "1" => new CuentaOro(),
                        "2" => new CuentaPlata(),
                        "3" => new CuentaBronce(),
                        _ => null
                    };

                    if (nuevaCuenta != null)
                    {
                        cliente.AgregarCuenta(nuevaCuenta);
                    }
                    else
                    {
                        Console.WriteLine("Tipo de cuenta inválido.");
                    }
                    Console.WriteLine($"Presiona una tecla para continuar...");
                    Console.ReadKey();
                    break;

                case "3":
                    Console.Write("Ingrese el nombre del cliente: ");
                    nombreCliente = Console.ReadLine();
                    cliente = banco.BuscarCliente(nombreCliente);
                    
                    if (cliente == null || cliente.Cuentas.Count == 0)
                    {
                        Console.WriteLine("El Cliente o la cuenta no fue encontrada.");
                        break;
                    }

                    Console.Write("Ingrese el número de la cuenta: ");
                    string numCuenta = Console.ReadLine();
                    Cuenta cuenta = cliente.Cuentas.Find(c => c.NumeroCuenta == numCuenta);

                    if (cuenta == null)
                    {
                        Console.WriteLine("La Cuenta no fue encontrada.");
                        break;
                    }

                    Console.Write("Ingrese el monto que desea depositár: ");
                    decimal monto = Convert.ToDecimal(Console.ReadLine());
                    banco.EjecutarOperacion(new Deposito(cuenta, monto));
                    Console.WriteLine($"presiona una tecla para continuar...");
                    Console.ReadKey();
                    break;

                case "4":
                    Console.Write("Ingrese el nombre del cliente: ");
                    nombreCliente = Console.ReadLine();
                    cliente = banco.BuscarCliente(nombreCliente);

                    if (cliente == null || cliente.Cuentas.Count == 0)
                    {
                        Console.WriteLine("El Cliente o La cuenta no fue encontrada.");
                        break;
                    }

                    Console.Write("Ingrese el número de la cuenta: ");
                    numCuenta = Console.ReadLine();
                    cuenta = cliente.Cuentas.Find(c => c.NumeroCuenta == numCuenta);

                    if (cuenta == null)
                    {
                        Console.WriteLine("La Cuenta no fue encontrada.");
                        break;
                    }

                    Console.Write("Ingrese monto a retirar: ");
                    monto = Convert.ToDecimal(Console.ReadLine());
                    banco.EjecutarOperacion(new Retiro(cuenta, monto));
                    Console.WriteLine($"presiona una tecla para continuar...");
                    Console.ReadKey();
                    break;
                
                case "5":
                    Console.Write("Ingrese el nombre del cliente: ");
                    nombreCliente = Console.ReadLine();
                    cliente = banco.BuscarCliente(nombreCliente);

                     if (cliente == null || cliente.Cuentas.Count == 0)
                    {
                       Console.WriteLine("Cliente o cuenta no encontrada.");
                        break;
                    }

                    Console.Write("Ingrese el número de la cuenta: ");
                    numCuenta = Console.ReadLine();
                     cuenta = cliente.Cuentas.Find(c => c.NumeroCuenta == numCuenta);

                    if (cuenta == null)
                    {
                      Console.WriteLine("Cuenta no encontrada.");
                        break;
                    }

                    Console.Write("Ingrese el monto a pagar: ");
                    monto = Convert.ToDecimal(Console.ReadLine());
                     banco.EjecutarOperacion(new Pago(cuenta, monto));
                     Console.WriteLine($"presiona una tecla para continuar...");
                    Console.ReadKey();
                    break;
                     
                case "6":
                     Console.Write("Ingrese el nombre del cliente de la cuenta origen: ");
                       nombreCliente = Console.ReadLine();
                       cliente = banco.BuscarCliente(nombreCliente);

                        if (cliente == null || cliente.Cuentas.Count == 0)
                        {
                            Console.WriteLine("Cliente o cuenta no encontrada.");
                             break;
                        }

                         Console.Write("Ingrese el número de la cuenta ORIGEN: ");
                         string cuentaOrigenStr = Console.ReadLine();
                          Cuenta cuentaOrigen = cliente.Cuentas.Find(c => c.NumeroCuenta == cuentaOrigenStr);

                          if (cuentaOrigen == null)
                           {
                             Console.WriteLine("Cuenta origen no encontrada.");
                              break;
                            }

                             Console.Write("Ingrese el nombre del cliente de la cuenta destino: ");
                                string nombreDestino = Console.ReadLine();
                              Cliente clienteDestino = banco.BuscarCliente(nombreDestino);

                             if (clienteDestino == null || clienteDestino.Cuentas.Count == 0)
                             {
                                Console.WriteLine("Cliente destino o cuenta no encontrada.");
                                  break;
                             }

                               Console.Write("Ingrese el número de la cuenta DESTINO: ");
                                string cuentaDestinoStr = Console.ReadLine();
                              Cuenta cuentaDestino = clienteDestino.Cuentas.Find(c => c.NumeroCuenta == cuentaDestinoStr);

                              if (cuentaDestino == null)
                              {
                                Console.WriteLine("Cuenta destino no encontrada.");
                                break;
                              }

                               Console.Write("Ingrese el monto a transferir: ");
                                monto = Convert.ToDecimal(Console.ReadLine());
                                banco.EjecutarOperacion(new Transferencia(cuentaOrigen, cuentaDestino, monto));
                                Console.WriteLine($"presiona una tecla para continuar...");
                                Console.ReadKey();
                                 break;

                case "7":
                    banco.MostrarClientes();
                    Console.WriteLine($"presiona una tecla para continuar...");
                    Console.ReadKey();
                    break;
                case "0":
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción inválida.");
                    Console.WriteLine($"presiona una tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }

