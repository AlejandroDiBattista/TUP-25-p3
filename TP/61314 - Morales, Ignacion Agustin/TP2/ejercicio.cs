using System;
using System.Collections.Generic;
using System.Linq;

namespace BancoSantander
{
    public abstract class Cuenta
    {
        public string Numero { get; set; }
        public decimal Saldo { get; protected set; }

        public Cuenta(string numero)
        {
            Numero = numero;
            Saldo = 0;
        }

        public abstract void Depositar(decimal monto);
        public abstract void Extraer(decimal monto);
    }

    public class CuentaOro : Cuenta
    {
        public CuentaOro(string numero) : base(numero) { }

        public override void Depositar(decimal monto)
        {
            Saldo += monto;
        }

        public override void Extraer(decimal monto)
        {
            if (monto <= Saldo)
                Saldo -= monto;
            else
                throw new InvalidOperationException("No hay fondos.");
        }
    }

    public class CuentaPlata : Cuenta
    {
        public CuentaPlata(string numero) : base(numero) { }

        public override void Depositar(decimal monto)
        {
            Saldo += monto * 1.02m; 
        }

        public override void Extraer(decimal monto)
        {
            if (monto <= Saldo)
                Saldo -= monto;
            else
                throw new InvalidOperationException("Fondos insuficientes.");
        }
    }

    public class CuentaBronce : Cuenta
    {
        public CuentaBronce(string numero) : base(numero) { }

        public override void Depositar(decimal monto)
        {
            Saldo += monto * 1.01m; 
        }

        public override void Extraer(decimal monto)
        {
            if (monto <= Saldo)
                Saldo -= monto;
            else
                throw new InvalidOperationException("Fondos insuficientes.");
        }
    }

    public abstract class Operacion
    {
        protected decimal Monto { get; set; }
        protected Cuenta Cuenta { get; set; }

        protected Operacion(decimal monto, Cuenta cuenta)
        {
            Monto = monto;
            Cuenta = cuenta;
        }

        public abstract void Realizar();
    }

    public class Deposito : Operacion
    {
        public Deposito(decimal monto, Cuenta cuenta) : base(monto, cuenta) { }

        public override void Realizar()
        {
            Cuenta.Depositar(Monto);
        }
    }

    public class Retiro : Operacion
    {
        public Retiro(decimal monto, Cuenta cuenta) : base(monto, cuenta) { }

        public override void Realizar()
        {
            Cuenta.Extraer(Monto);
        }

    public class Banco
    {
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();

        public void AgregarCliente(Cliente cliente)
        {
            Clientes.Add(cliente);
        }

        public Cuenta BuscarCuenta(string numero)
        {
            return Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numero);
        }

        public void Reporte()
        {
            foreach (var cliente in Clientes)
            {
                Console.WriteLine(cliente.ToString());
            }
        }

        public void RegistrarOperacion(Operacion operacion, Cliente cliente)
        {
            operacion.Realizar();
            cliente.AgregarOperacion(operacion);
        }
    }
    public class Cliente
    {
        public string Nombre { get; set; }
        public List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
        public List<Operacion> Operaciones { get; set; } = new List<Operacion>();

        public Cliente(string nombre)
        {
            Nombre = nombre;
        }

        public void AgregarCuenta(Cuenta cuenta)
        {
            Cuentas.Add(cuenta);
        }

        public void AgregarOperacion(Operacion operacion)
        {
            Operaciones.Add(operacion);
        }

        public override string ToString()
        {
            return $"Cliente: {Nombre}, Cuentas: {string.Join(", ", Cuentas.Select(c => c.Numero))}";
        }
    }

    class Program
    {
        static Banco banco = new();

        static void Main(string[] args)
        {
            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("====== MENÚ BANCO SANTANDER ======");
                Console.WriteLine("1. Registrar nuevo cliente y cuenta");
                Console.WriteLine("2. Realizar operación bancaria");
                Console.WriteLine("3. Ver reporte completo");
                Console.WriteLine("4. Salir");
                Console.Write("Seleccione una opción: ");
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        RegistrarClienteYCuenta();
                        break;
                    case "2":
                        RealizarOperacion();
                        break;
                    case "3":
                        banco.Reporte();
                        Console.ReadKey();
                        break;
                    case "4":
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Intente nuevamente.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void RegistrarClienteYCuenta()
        {
            Console.Write("Nombre del cliente: ");
            string nombre = Console.ReadLine();
            Cliente cliente = new(nombre);

            Console.Write("Número de cuenta (formato XXXXX): ");
            string numero = Console.ReadLine();

            Console.WriteLine("Tipo de cuenta (1: Oro, 2: Plata, 3: Bronce): ");
            string tipo = Console.ReadLine();

            Cuenta cuenta = tipo switch
            {
                "1" => new CuentaOro(numero),
                "2" => new CuentaPlata(numero),
                "3" => new CuentaBronce(numero),
                _ => throw new InvalidOperationException("Tipo de cuenta inválido.")
            };

            cliente.AgregarCuenta(cuenta);
            banco.AgregarCliente(cliente);
            Console.WriteLine("Cliente y cuenta registrados exitosamente.");
            Console.ReadKey();
        }

        static void RealizarOperacion()
        {
            Console.Write("Número de cuenta: ");
            string numero = Console.ReadLine();

            Cuenta cuenta = banco.BuscarCuenta(numero);
            if (cuenta == null)
            {
                Console.WriteLine("Cuenta no encontrada.");
                Console.ReadKey();
                return;
            }

            Cliente cliente = banco.Clientes.First(c => c.Cuentas.Contains(cuenta));

            Console.WriteLine("Operación (1: Depositar, 2: Extraer, 3: Pagar, 4: Transferir): ");
            string opcion = Console.ReadLine();

            Console.Write("Monto: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal monto))
            {
                Console.WriteLine("Monto inválido.");
                return;
            }

            switch (opcion)
            {
                case "1":
                    var dep = new Deposito(monto, cuenta);
                    banco.RegistrarOperacion(dep, cliente);
                    Console.WriteLine("Depósito realizado.");
                    break;
                case "2":
                    var ret = new Retiro(monto, cuenta);
                    banco.RegistrarOperacion(ret, cliente);
                    Console.WriteLine("Retiro realizado.");
                    break;
                case "3":
                    var pag = new Pago(monto, cuenta);
                    banco.RegistrarOperacion(pag, cliente);
                    Console.WriteLine("Pago realizado.");
                    break;
                case "4":
                    Console.Write("Número de cuenta destino: ");
                    string destinoNumero = Console.ReadLine();
                    var destino = banco.BuscarCuenta(destinoNumero);

                    if (destino == null)
                    {
                        Console.WriteLine("Cuenta destino no encontrada.");
                        break;
                    }

                    var trans = new Transferencia(monto, cuenta, destino);
                    banco.RegistrarOperacion(trans, cliente);
                    Console.WriteLine("Transferencia realizada.");
                    break;
                default:
                    Console.WriteLine("Operación inválida.");
                    break;
            }

            Console.ReadKey();
        }
    }
    public class Pago : Operacion
    {
        public Pago(decimal monto, Cuenta cuenta) : base(monto, cuenta) { }

        public override void Realizar()
        {
            Cuenta.Extraer(Monto);
        }
    }
    public class Transferencia : Operacion
    {
        private Cuenta CuentaDestino { get; set; }

        public Transferencia(decimal monto, Cuenta cuentaOrigen, Cuenta cuentaDestino) : base(monto, cuentaOrigen)
        {
            CuentaDestino = cuentaDestino;
        }

        public override void Realizar()
        {
            Cuenta.Extraer(Monto);
            CuentaDestino.Depositar(Monto);
        }
    }
}
  }

