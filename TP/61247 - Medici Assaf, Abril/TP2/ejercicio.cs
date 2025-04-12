using System;
using System.Collections.Generic;
using System.Linq;

namespace BancoApp
{
    abstract class Cuenta
    {
        public string Numero { get; }
        public double Saldo { get; protected set; }
        public double Puntos { get; protected set; }
        public Cliente Titular { get; set; }
        public double SaldoInicial { get; }


        public Cuenta(string numero, double saldoInicial)
        {
            Numero = numero;
            Saldo = saldoInicial;
            SaldoInicial = saldoInicial;
            Puntos = 0;
        }

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

        public virtual bool Pagar(double monto)
        {
            if (Saldo >= monto)
            {
                Saldo -= monto;
                AcumularPuntos(monto);
                return true;
            }
            return false;
        }

        protected abstract void AcumularPuntos(double monto);
    }

    class CuentaOro : Cuenta
    {
        public CuentaOro(string numero, double saldoInicial) : base(numero, saldoInicial) { }

        protected override void AcumularPuntos(double monto)
        {
            if (monto > 1000)
                Puntos += monto * 0.05;
            else
                Puntos += monto * 0.03;
        }
    }

    class CuentaPlata : Cuenta
    {
        public CuentaPlata(string numero, double saldoInicial) : base(numero, saldoInicial) { }

        protected override void AcumularPuntos(double monto)
        {
            Puntos += monto * 0.02;
        }
    }

    class CuentaBronce : Cuenta
    {
        public CuentaBronce(string numero, double saldoInicial) : base(numero, saldoInicial) { }

        protected override void AcumularPuntos(double monto)
        {
            Puntos += monto * 0.01;
        }
    }

    abstract class Operacion
    {
        public double Monto { get; set; }
        public abstract void Ejecutar(Banco banco);
    }

    class Deposito : Operacion
    {
        public string NumeroCuenta { get; set; }

        public Deposito(string numeroCuenta, double monto)
        {
            NumeroCuenta = numeroCuenta;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var cuenta = banco.BuscarCuenta(NumeroCuenta);
            cuenta?.Depositar(Monto);
            banco.RegistrarOperacionGlobal($"Deposito $ {Monto:F2} a [{NumeroCuenta}/{cuenta.Titular.Nombre}]");
            cuenta?.Titular.RegistrarOperacionPersonal($"Deposito $ {Monto:F2} a [{NumeroCuenta}/{cuenta.Titular.Nombre}]");
        }
    }

    class Retiro : Operacion
    {
        public string NumeroCuenta { get; set; }

        public Retiro(string numeroCuenta, double monto)
        {
            NumeroCuenta = numeroCuenta;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var cuenta = banco.BuscarCuenta(NumeroCuenta);
            if (cuenta != null && cuenta.Extraer(Monto))
            {
                banco.RegistrarOperacionGlobal($"Retiro $ {Monto:F2} de [{NumeroCuenta}/{cuenta.Titular.Nombre}]");
                cuenta.Titular.RegistrarOperacionPersonal($"Retiro $ {Monto:F2} de [{NumeroCuenta}/{cuenta.Titular.Nombre}]");
            }
        }
    }

    class Pago : Operacion
    {
        public string NumeroCuenta { get; set; }

        public Pago(string numeroCuenta, double monto)
        {
            NumeroCuenta = numeroCuenta;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var cuenta = banco.BuscarCuenta(NumeroCuenta);
            if (cuenta != null && cuenta.Pagar(Monto))
            {
                banco.RegistrarOperacionGlobal($"Pago $ {Monto:F2} con [{NumeroCuenta}/{cuenta.Titular.Nombre}]");
                cuenta.Titular.RegistrarOperacionPersonal($"Pago $ {Monto:F2} con [{NumeroCuenta}/{cuenta.Titular.Nombre}]");
            }
        }
    }

    class Transferencia : Operacion
    {
        public string Origen { get; set; }
        public string Destino { get; set; }

        public Transferencia(string origen, string destino, double monto)
        {
            Origen = origen;
            Destino = destino;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var cuentaOrigen = banco.BuscarCuenta(Origen);
            var cuentaDestino = banco.BuscarCuenta(Destino);
            if (cuentaOrigen != null && cuentaDestino != null && cuentaOrigen.Extraer(Monto))
            {
                cuentaDestino.Depositar(Monto);
                banco.RegistrarOperacionGlobal($"Transferencia $ {Monto:F2} de [{Origen}/{cuentaOrigen.Titular.Nombre}] a [{Destino}/{cuentaDestino.Titular.Nombre}]");
                cuentaOrigen.Titular.RegistrarOperacionPersonal($"Transferencia $ {Monto:F2} de [{Origen}/{cuentaOrigen.Titular.Nombre}] a [{Destino}/{cuentaDestino.Titular.Nombre}]");
                cuentaDestino.Titular.RegistrarOperacionPersonal($"Transferencia $ {Monto:F2} de [{Origen}/{cuentaOrigen.Titular.Nombre}] a [{Destino}/{cuentaDestino.Titular.Nombre}]");
            }
        }
    }

    class Cliente
    {
        public string Nombre { get; }
        public List<Cuenta> Cuentas { get; }
        public List<string> Historial { get; }

        public Cliente(string nombre)
        {
            Nombre = nombre;
            Cuentas = new List<Cuenta>();
            Historial = new List<string>();
        }

        public void Agregar(Cuenta cuenta)
        {
            cuenta.Titular = this;
            Cuentas.Add(cuenta);
        }

        public void RegistrarOperacionPersonal(string operacion)
        {
            Historial.Add(operacion);
        }

        public double TotalSaldo() => Cuentas.Sum(c => c.Saldo);
        public double TotalPuntos() => Cuentas.Sum(c => c.Puntos);
    }

    class Banco
    {
        public string Nombre { get; }
        private List<Cliente> Clientes = new();
        private List<string> HistorialGlobal = new();

        public Banco(string nombre)
        {
            Nombre = nombre;
        }

        public void Agregar(Cliente cliente)
        {
            Clientes.Add(cliente);
        }

        public Cuenta BuscarCuenta(string numero)
        {
            return Clientes.SelectMany(c => c.Cuentas).FirstOrDefault(c => c.Numero == numero);
        }

        public void Registrar(Operacion operacion)
        {
            operacion.Ejecutar(this);
        }

        public void RegistrarOperacionGlobal(string registro)
        {
            HistorialGlobal.Add(registro);
        }

        public void Informe()
        {
            Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}\n");
            Console.WriteLine("Listado de clientes:\n");

            foreach (var cliente in Clientes)
            {
                Console.WriteLine($"Cliente: {cliente.Nombre}");

                foreach (var cuenta in cliente.Cuentas)
                {
                    Console.WriteLine($" Cuenta: {cuenta.Numero} | Saldo Inicial: {cuenta.SaldoInicial:N2} | Saldo: {cuenta.Saldo:N2} | Puntos: {cuenta.Puntos:N2}");

                    Console.WriteLine(" - Transacciones realizadas:");

                    foreach (var op in cliente.Historial.Where(h => h.Contains(cuenta.Numero)))
                    {
                        Console.WriteLine($"    - {op}");
                    }

                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }
        
    }

    class Program
    {
        static void Main()
        {   //EJEMPLIFICACION TP

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

            nac.Registrar(new Deposito("10001", 100));
            nac.Registrar(new Retiro("10002", 200));
            nac.Registrar(new Transferencia("10001", "10002", 300));
            nac.Registrar(new Transferencia("10003", "10004", 500));
            nac.Registrar(new Pago("10002", 400));

            tup.Registrar(new Deposito("10005", 100));
            tup.Registrar(new Retiro("10005", 200));
            tup.Registrar(new Transferencia("10005", "10002", 300));
            tup.Registrar(new Pago("10005", 400));



            //EJEMPLIFICACION PROPIA
            var abril = new Cliente("Abril Medici");
            abril.Agregar(new CuentaOro("10006", 3000));
            abril.Agregar(new CuentaPlata("10007", 500));

            var marcos = new Cliente("Marcos Martinez");
            marcos.Agregar(new CuentaPlata("10008", 4590));

            var pedro = new Cliente("Pedro Sanchez");
            pedro.Agregar(new CuentaBronce("10009", 300));

            var Ciudad = new Banco("Banco Ciudad");
            Ciudad.Agregar(abril);
            Ciudad.Agregar(marcos);

            var macro = new Banco("Banco Macro");
            macro.Agregar(pedro);

            Ciudad.Registrar(new Deposito("10006", 100));
            Ciudad.Registrar(new Retiro("10007", 2040));
            Ciudad.Registrar(new Transferencia("10006", "10007", 35600));
            Ciudad.Registrar(new Transferencia("10008", "10004", 500));
            Ciudad.Registrar(new Pago("10007", 400));

            macro.Registrar(new Deposito("10009", 100));
            macro.Registrar(new Retiro("10009", 200));
            macro.Registrar(new Transferencia("10009", "10007", 300));
            macro.Registrar(new Pago("10009", 400));

            Console.WriteLine("--------------- INFORMES ---------------");
            Console.WriteLine("-------------- Banco Ciudad --------------");
            Ciudad.Informe();
            Console.WriteLine("-------------- Banco Macro --------------");
            macro.Informe();
            Console.WriteLine("-------------- Banco Nac --------------");
            nac.Informe();
            Console.WriteLine("-------------- Banco TUP --------------");
            tup.Informe();
            
        }
    }
}
