using System;
using System.Collections.Generic;

namespace BancoApp
{

    interface ICuenta
    {
        string Numero { get; }
        double Saldo { get; }
        double Puntos { get; }
        void Depositar(double monto);
        bool Extraer(double monto);
        bool Pagar(double monto);
    }

    abstract class CuentaBase : ICuenta
    {
        private string _numero;
        public string Numero => _numero;
        public double Saldo { get; protected set; }
        public double Puntos { get; protected set; }

        protected CuentaBase(string numero)
        {
            _numero = numero;
            Saldo = 0;
            Puntos = 0;
        }

        public virtual void Depositar(double monto)
        {
            if (monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a cero.");
            Saldo += monto;
        }

        public virtual bool Extraer(double monto)
        {
            if (monto <= 0) return false;
            if (Saldo >= monto)
            {
                Saldo -= monto;
                return true;
            }
            return false;
        }

        public bool Pagar(double monto)
        {
            if (Extraer(monto))
            {
                AcumularPuntos(monto);
                return true;
            }
            return false;
        }

        protected abstract void AcumularPuntos(double monto);
    }

    class CuentaOro : CuentaBase
    {
        public CuentaOro(string numero) : base(numero) { }

        protected override void AcumularPuntos(double monto)
        {
            Puntos += monto > 1000 ? monto * 0.05 : monto * 0.03;
        }
    }

    class CuentaPlata : CuentaBase
    {
        public CuentaPlata(string numero) : base(numero) { }

        protected override void AcumularPuntos(double monto)
        {
            Puntos += monto * 0.02;
        }
    }

    class CuentaBronce : CuentaBase
    {
        public CuentaBronce(string numero) : base(numero) { }

        protected override void AcumularPuntos(double monto)
        {
            Puntos += monto * 0.01;
        }
    }

    class Cliente
    {
        public string Nombre { get; }
        public List<ICuenta> Cuentas { get; } = new List<ICuenta>();
        public List<string> Historial { get; } = new List<string>();

        public Cliente(string nombre)
        {
            Nombre = nombre;
        }

        public void AgregarCuenta(ICuenta cuenta)
        {
            Cuentas.Add(cuenta);
        }

        public void RegistrarOperacion(string operacion)
        {
            Historial.Add(operacion);
        }
    }


    class BancoService
    {
        private readonly List<Cliente> _clientes = new List<Cliente>();
        private readonly List<string> _logGlobal = new List<string>();

        public void AgregarCliente(Cliente cliente)
        {
            _clientes.Add(cliente);
        }

        public void Depositar(string nroCuenta, double monto)
        {
            var (cliente, cuenta) = BuscarCuenta(nroCuenta);
            if (cliente != null)
            {
                cuenta.Depositar(monto);
                RegistrarOperacion(cliente, cuenta, $"Depósito de ${monto}");
            }
        }

        public void Extraer(string nroCuenta, double monto)
        {
            var (cliente, cuenta) = BuscarCuenta(nroCuenta);
            if (cliente != null)
            {
                if (cuenta.Extraer(monto))
                    RegistrarOperacion(cliente, cuenta, $"Extracción de ${monto}");
                else
                    Console.WriteLine("Fondos insuficientes o monto inválido.");
            }
        }

        public void Pagar(string nroCuenta, double monto)
        {
            var (cliente, cuenta) = BuscarCuenta(nroCuenta);
            if (cliente != null)
            {
                if (cuenta.Pagar(monto))
                    RegistrarOperacion(cliente, cuenta, $"Pago de ${monto} | Puntos: {cuenta.Puntos}");
                else
                    Console.WriteLine("Pago no realizado.");
            }
        }

        public void Transferir(string origen, string destino, double monto)
        {
            var (cliOrigen, ctaOrigen) = BuscarCuenta(origen);
            var (cliDestino, ctaDestino) = BuscarCuenta(destino);

            if (cliOrigen != null && cliDestino != null)
            {
                if (ctaOrigen.Extraer(monto))
                {
                    ctaDestino.Depositar(monto);
                    RegistrarOperacion(cliOrigen, ctaOrigen, $"Transferencia enviada de ${monto} a {destino}");
                    RegistrarOperacion(cliDestino, ctaDestino, $"Transferencia recibida de ${monto} desde {origen}");
                }
                else
                {
                    Console.WriteLine("Transferencia fallida: fondos insuficientes.");
                }
            }
        }

        public void Reporte()
        {
            Console.WriteLine("---- Log global ----");
            foreach (var op in _logGlobal)
                Console.WriteLine(" - " + op);

            foreach (var cli in _clientes)
            {
                Console.WriteLine($"\nCliente: {cli.Nombre}");
                foreach (var cuenta in cli.Cuentas)
                    Console.WriteLine($"Cuenta {cuenta.Numero} | Saldo: ${cuenta.Saldo} | Puntos: {cuenta.Puntos}");

                Console.WriteLine("Historial:");
                foreach (var op in cli.Historial)
                    Console.WriteLine(" - " + op);
            }
        }

        private (Cliente, ICuenta) BuscarCuenta(string numero)
        {
            foreach (var cli in _clientes)
            {
                foreach (var cuenta in cli.Cuentas)
                {
                    if (cuenta.Numero == numero)
                        return (cli, cuenta);
                }
            }
            Console.WriteLine($"Cuenta {numero} no encontrada.");
            return (null, null);
        }

        private void RegistrarOperacion(Cliente cli, ICuenta cuenta, string detalle)
        {
            var registro = $"[{cuenta.Numero}] {detalle}";
            cli.RegistrarOperacion(registro);
            _logGlobal.Add(registro);
        }
    }


    class Program
    {
        static void Main()
        {
            var banco = new BancoService();

            var juan = new Cliente("Juan");
            juan.AgregarCuenta(new CuentaOro("00001"));
            banco.AgregarCliente(juan);

            var ana = new Cliente("Ana");
            ana.AgregarCuenta(new CuentaPlata("00002"));
            banco.AgregarCliente(ana);

            banco.Depositar("00001", 2000);
            banco.Pagar("00001", 1500);
            banco.Transferir("00001", "00002", 300);
            banco.Extraer("00002", 100);

            banco.Reporte();
        }
    }
}