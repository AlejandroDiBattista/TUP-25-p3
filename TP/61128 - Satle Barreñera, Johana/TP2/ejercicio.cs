using System;
using System.Collections.Generic;

namespace SistemaBancario
{
    class Cuenta
    {
        public string Numero { get; }
        public decimal Saldo { get; protected set; }
        public decimal Puntos { get; protected set; }
        public virtual string Tipo => "Cuenta";

        public Cuenta(string numero, decimal saldoInicial)
        {
            Numero = numero;
            Saldo = saldoInicial;
            Puntos = 0;
        }

        public void Depositar(decimal monto)
        {
            Saldo += monto;
        }

        public void Extraer(decimal monto)
        {
            if (Saldo < monto) throw new InvalidOperationException("Fondos insuficientes.");
            Saldo -= monto;
        }

        public virtual void Pagar(decimal monto)
        {
            if (Saldo < monto) throw new InvalidOperationException("Fondos insuficientes.");
            Saldo -= monto;
        }

        public virtual string ObtenerEstado()
        {
            return $"Cuenta: {Numero} | Saldo: $ {Saldo:0.00} | Puntos: $ {Puntos:0.00}";
        }
    }

    class CuentaOro : Cuenta
    {
        public override string Tipo => "Oro";

        public CuentaOro(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

        public override void Pagar(decimal monto)
        {
            base.Pagar(monto);
            if (monto > 1000) Puntos += monto * 0.05m;
            else Puntos += monto * 0.03m;
        }
    }

    class CuentaPlata : Cuenta
    {
        public override string Tipo => "Plata";

        public CuentaPlata(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

        public override void Pagar(decimal monto)
        {
            base.Pagar(monto);
            Puntos += monto * 0.02m;
        }
    }

    class CuentaBronce : Cuenta
    {
        public override string Tipo => "Bronce";

        public CuentaBronce(string numero, decimal saldoInicial) : base(numero, saldoInicial) { }

        public override void Pagar(decimal monto)
        {
            base.Pagar(monto);
            Puntos += monto * 0.01m;
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

        public void AgregarCuenta(Cuenta cuenta)
        {
            Cuentas.Add(cuenta);
        }

        public decimal SaldoTotal() => Cuentas.Sum(c => c.Saldo);
        public decimal PuntosTotal() => Cuentas.Sum(c => c.Puntos);
    }

    class Banco
    {
        public string Nombre { get; }
        private List<Cliente> clientes;
        private List<string> operacionesGlobales;

        public Banco(string nombre)
        {
            Nombre = nombre;
            clientes = new List<Cliente>();
            operacionesGlobales = new List<string>();
        }

        public void AgregarCliente(Cliente cliente)
        {
            clientes.Add(cliente);
        }

        private Cuenta BuscarCuenta(string numeroCuenta, out Cliente propietario)
        {
            foreach (var cliente in clientes)
            {
                foreach (var cuenta in cliente.Cuentas)
                {
                    if (cuenta.Numero == numeroCuenta)
                    {
                        propietario = cliente;
                        return cuenta;
                    }
                }
            }
            propietario = null;
            return null;
        }

        public void Depositar(string cuentaNumero, decimal monto)
        {
            var cuenta = BuscarCuenta(cuentaNumero, out var cliente);
            cuenta.Depositar(monto);
            string desc = $"Deposito $ {monto:0.00} a [{cuentaNumero}/{cliente.Nombre}]";
            cliente.Historial.Add(desc);
            operacionesGlobales.Add(desc);
        }

        public void Retirar(string cuentaNumero, decimal monto)
        {
            var cuenta = BuscarCuenta(cuentaNumero, out var cliente);
            cuenta.Extraer(monto);
            string desc = $"Retiro $ {monto:0.00} de [{cuentaNumero}/{cliente.Nombre}]";
            cliente.Historial.Add(desc);
            operacionesGlobales.Add(desc);
        }

        public void Pagar(string cuentaNumero, decimal monto)
        {
            var cuenta = BuscarCuenta(cuentaNumero, out var cliente);
            cuenta.Pagar(monto);
            string desc = $"Pago $ {monto:0.00} con [{cuentaNumero}/{cliente.Nombre}]";
            cliente.Historial.Add(desc);
            operacionesGlobales.Add(desc);
        }

        public void Transferir(string origen, string destino, decimal monto)
        {
            var cuentaOrigen = BuscarCuenta(origen, out var clienteOrigen);
            var cuentaDestino = BuscarCuenta(destino, out var clienteDestino);
            cuentaOrigen.Extraer(monto);
            cuentaDestino.Depositar(monto);
            string desc = $"Transferencia $ {monto:0.00} de [{origen}/{clienteOrigen.Nombre}] a [{destino}/{clienteDestino.Nombre}]";
            clienteOrigen.Historial.Add(desc);
            clienteDestino.Historial.Add(desc);
            operacionesGlobales.Add(desc);
        }

        public void Informe()
        {
            Console.WriteLine($"Banco: {Nombre} | Clientes: {clientes.Count}\n");
            foreach (var cliente in clientes)
            {
                Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal():0.00} | Puntos Total: $ {cliente.PuntosTotal():0.00}\n");
                foreach (var cuenta in cliente.Cuentas)
                {
                    Console.WriteLine("    " + cuenta.ObtenerEstado());
                    foreach (var op in cliente.Historial.Where(op => op.Contains(cuenta.Numero)))
                        Console.WriteLine("     -  " + op);
                }
                Console.WriteLine();
            }
        }
    }

    class ejercicio
    {
        static void Main(string[] args)
        {
            var johana = new Cliente("Johana Satle");
            johana.AgregarCuenta(new CuentaOro("12345", 1200));
            johana.AgregarCuenta(new CuentaPlata("12346", 300));

            var maria = new Cliente("María López");
            maria.AgregarCuenta(new CuentaBronce("12347", 300));

            var banco = new Banco("Banco Central");
            banco.AgregarCliente(johana);
            banco.AgregarCliente(maria);

            banco.Depositar("12345", 200);
            banco.Retirar("12346", 100);
            banco.Pagar("12345", 1100);
            banco.Transferir("12346", "12347", 200);

            banco.Informe();
        }
    }
}