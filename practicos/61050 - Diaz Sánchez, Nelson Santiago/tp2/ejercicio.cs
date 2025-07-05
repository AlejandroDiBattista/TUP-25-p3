using System.Data.Common;

public class  Banco {
    public List<Cliente> Clientes { get; set; }
    public List<Operacion> historialoperaciones { get; set; }
    public Banco() {
        Clientes = new List<Cliente>();
        historialoperaciones = new List<Operacion>();
    } 

    
}

public class Cliente {
    public string nombre { get; set; }
    public string apellido { get; set; }
    public string dni { get; set; }
    public string direccion { get; set; }
    public string telefono { get; set; }
    public List<Cuenta> Cuentas { get; set; }
    public Cliente(string nombre, string apellido, string dni, string direccion, string telefono) {
        this.nombre = nombre;
        this.apellido = apellido;
        this.dni = dni;
        this.direccion = direccion;
        this.telefono = telefono;
        this.Cuentas =new List<Cuenta>();
    } 

    public void AgregarCuenta(Cuenta cuenta) {
        Cuentas.Add(cuenta);
        Console.WriteLine("Cuenta agregada correctamente.");
}

}

public class Operacion {
    public string tipo { get; set; }
    public decimal monto { get; set; }
    public DateTime fecha { get; set; }
    public Operacion(string tipo, decimal monto) {
        this.tipo = tipo;
        this.monto = monto;
        this.fecha = DateTime.Now;
    } 
}

public abstract class Cuenta {
    public string numero {get; set;}
    public decimal saldo {get; set;}
    public int puntos {get; set;}

    public Cuenta (string numero, decimal saldo, int puntos) {
        this.numero = numero;
        this.saldo = saldo;
        this.puntos = puntos;
    }
    public abstract void AcumularPuntos(decimal monto);

}


// Cuentas específicas

// Cuentas específicas
public class CuentaOro : Cuenta {
    public CuentaOro(string numero, decimal saldoInicial = 0) : base(numero, saldoInicial, 0) {}

    public override void AcumularPuntos(decimal monto) {
        puntos += (int)(monto *(monto>1000 ? 0.05m : 0.03m));  // Ahora "Puntos" sí existe
    }
}

public class CuentaPlata : Cuenta {
    public CuentaPlata(string numero, decimal saldoInicial = 0) : base(numero, saldoInicial, 0) {}

    public override void AcumularPuntos(decimal monto) {
        puntos += (int)(monto * 0.02m);
    }
}

public class CuentaBronce : Cuenta {
    public CuentaBronce(string numero, decimal saldoInicial = 0) : base(numero, saldoInicial, 0) {}

    public override void AcumularPuntos(decimal monto) {
        puntos += (int)(monto * 0.01m);
    }
}


        Banco banco = new Banco();

        bool inicial = false;
        while (!inicial) {
        Console.WriteLine("Bienvenido al sistema de gestión de cuentas del banco.");
        Console.WriteLine("Ingresa una de las opciones:");
        Console.WriteLine("1. Crear cliente");
        Console.WriteLine("2. Crear cuenta");
        Console.WriteLine("3. Depositar");
        Console.WriteLine("4. Retirar");
        Console.WriteLine("5. Transferir");
        Console.WriteLine("6. Ver historial de operaciones");
        Console.WriteLine("7. Salir");


        switch (int.Parse(Console.ReadLine())) {
            case 1:
                Console.WriteLine("Creando cliente...");
                Console.WriteLine("Ingrese el nombre del cliente:");
                string nombre = Console.ReadLine();
                Console.WriteLine("Ingrese el apellido del cliente:");
                string apellido = Console.ReadLine();
                Console.WriteLine("Ingrese el DNI del cliente:");
                string dni = Console.ReadLine();
                Console.WriteLine("Ingrese la dirección del cliente:");
                string direccion = Console.ReadLine();
                Console.WriteLine("Ingrese el teléfono del cliente:");
                string telefono = Console.ReadLine();

                Cliente cliente = new Cliente(nombre, apellido, dni, direccion, telefono);
                
                banco.Clientes.Add(cliente);
                Console.ReadKey();
                break;
           case 2: {
    if (banco.Clientes.Count == 0) {
        Console.WriteLine("No hay clientes registrados. Primero crea un cliente.");
        break;
    }

    Console.WriteLine("Ingresar el número de DNI: ");
    string dniCliente = Console.ReadLine();
    Cliente clienteexiste = banco.Clientes.FirstOrDefault(c => c.dni == dniCliente);
    if (clienteexiste == null) {
        Console.WriteLine("Cliente no encontrado.");
        break;
    }

    Console.WriteLine("Ingrese el tipo de cuenta:");
    Console.WriteLine("1. Cuenta Oro");
    Console.WriteLine("2. Cuenta Plata");
    Console.WriteLine("3. Cuenta Bronce");
    int tipoCuentaSeleccionado = int.Parse(Console.ReadLine());

    Console.WriteLine("Ingrese el número de cuenta:");
    string numeroCuenta = Console.ReadLine();

    Console.WriteLine("Ingrese el saldo inicial:");
    decimal saldoInicial = decimal.Parse(Console.ReadLine());

    Cuenta nuevaCuenta = tipoCuentaSeleccionado switch {
        1 => new CuentaOro(numeroCuenta, saldoInicial),
        2 => new CuentaPlata(numeroCuenta, saldoInicial),
        3 => new CuentaBronce(numeroCuenta, saldoInicial),
        _ => null
    };

    if (nuevaCuenta == null) {
        Console.WriteLine("Tipo de cuenta no válido.");
        break;
    }

    
    clienteexiste.AgregarCuenta(nuevaCuenta);
    Console.ReadKey();
    break;
}

case 3: //Depositar
    Console.WriteLine ("Ingrese el DNI de cuenta: ");
    string dniClienteDeposito = Console.ReadLine();
    cliente = banco.Clientes.FirstOrDefault(c => c.dni == dniClienteDeposito);
    if (cliente == null) {
        Console.WriteLine("Cliente no encontrado.");
    }
    else {
        Console.WriteLine("Ingrese el dni de la cuenta: ");
        string numeroCuentaDeposito = Console.ReadLine();
        Cuenta cuentaDeposito = cliente.Cuentas.FirstOrDefault(c => c.numero == numeroCuentaDeposito);
        if (cuentaDeposito == null) {
            Console.WriteLine("Cuenta no encontrada.");
        }
        else {
            Console.WriteLine("Ingrese el monto a depositar: ");
            decimal montoDeposito = decimal.Parse(Console.ReadLine());
            cuentaDeposito.saldo += montoDeposito;
            cuentaDeposito.AcumularPuntos(montoDeposito);
            Operacion operacionDeposito = new Operacion("Deposito", montoDeposito);
            banco.historialoperaciones.Add(operacionDeposito);
            Console.WriteLine("Depósito realizado con éxito.");
        }

    }
            
    Console.ReadKey();
            break;

    case 4: //Retirar
        Console.WriteLine ("Ingrese el DNI de cuenta: ");
        string dniClienteRetiro = Console.ReadLine();
        cliente = banco.Clientes.FirstOrDefault(c => c.dni == dniClienteRetiro);
        if (cliente == null) {
            Console.WriteLine("Cliente no encontrado.");
        }
        else {
            Console.WriteLine("Ingrese el dni de la cuenta: ");
            string numeroCuentaRetiro = Console.ReadLine();
            Cuenta cuentaRetiro = cliente.Cuentas.FirstOrDefault(c => c.numero == numeroCuentaRetiro);
            if (cuentaRetiro == null) {
                Console.WriteLine("Cuenta no encontrada.");
            }
            else {
                Console.WriteLine("Ingrese el monto a retirar: ");
                decimal montoRetiro = decimal.Parse(Console.ReadLine());
                if (montoRetiro > cuentaRetiro.saldo) {
                    Console.WriteLine("Saldo insuficiente.");
                }
                else {
                    cuentaRetiro.saldo -= montoRetiro;
                    Operacion operacionRetiro = new Operacion("Retiro", montoRetiro);
                    banco.historialoperaciones.Add(operacionRetiro);
                    Console.WriteLine("Retiro realizado con éxito.");
            }
            }
        }
        Console.ReadKey();
        break;
        case 5:
    Console.WriteLine("Realizar transferencia:");
    Console.WriteLine("Ingrese el DNI del cliente que realiza la transferencia: ");
    string dniClienteTransferencia = Console.ReadLine();
    cliente = banco.Clientes.FirstOrDefault(c => c.dni == dniClienteTransferencia);
    
    if (cliente == null) {
        Console.WriteLine("Cliente no encontrado.");
    } else {
        Console.WriteLine("Ingrese el número de cuenta del cliente que realiza la transferencia: ");
        string numeroCuentaOrigen = Console.ReadLine();
        Cuenta cuentaOrigen = cliente.Cuentas.FirstOrDefault(c => c.numero == numeroCuentaOrigen);

        if (cuentaOrigen == null) {
            Console.WriteLine("Cuenta de origen no encontrada.");
        } else {
            Console.WriteLine("Ingrese el DNI del cliente que recibe la transferencia: ");
            string dniReceptor = Console.ReadLine();
            Cliente clienteReceptor = banco.Clientes.FirstOrDefault(c => c.dni == dniReceptor);

            if (clienteReceptor == null) {
                Console.WriteLine("Cliente receptor no encontrado.");
            } else {
                Console.WriteLine("Ingrese el número de cuenta del cliente receptor: ");
                string numeroCuentaDestino = Console.ReadLine();
                Cuenta cuentaDestino = clienteReceptor.Cuentas.FirstOrDefault(c => c.numero == numeroCuentaDestino);

                if (cuentaDestino == null) {
                    Console.WriteLine("Cuenta de destino no encontrada.");
                } else {
                    Console.WriteLine("Ingrese el monto a transferir: ");
                    decimal montoTransferencia = decimal.Parse(Console.ReadLine());

                    if (montoTransferencia > cuentaOrigen.saldo) {
                        Console.WriteLine("Saldo insuficiente.");
                    } else {
                        cuentaOrigen.saldo -= montoTransferencia;
                        cuentaDestino.saldo += montoTransferencia;

                        cuentaOrigen.AcumularPuntos(montoTransferencia); // Se acumulan puntos solo al que envía

                        banco.historialoperaciones.Add(new Operacion("Transferencia enviada", montoTransferencia));
                        banco.historialoperaciones.Add(new Operacion("Transferencia recibida", montoTransferencia));

                        Console.WriteLine("Transferencia realizada con éxito.");
                    }
                }
            }
        }
    }
    Console.ReadKey();
    break;

    case 6:
    if (banco.historialoperaciones.Count == 0) {
        Console.WriteLine("No hay operaciones registradas.");
    } else {
        Console.WriteLine("Historial de operaciones:");
        foreach (var operacion in banco.historialoperaciones) {
            Console.WriteLine($"{operacion.fecha}: {operacion.tipo} por ${operacion.monto}");
        }
    }
    Console.ReadKey();
    break;
    case 7:
        Console.WriteLine("Saliendo del sistema...");
        inicial = true;
        break;
            default:
                Console.WriteLine("Opción no válida.");
                Console.ReadKey();
                break;
        }
    }

