using System;
using System.Collections.Generic;
using System.Linq;

namespace BancoApp
{
    

    abstract class Operacion
    {
        public decimal Monto { get; protected set; }
        public string CuentaOrigen { get; protected set; }
        public string CuentaDestino { get; protected set; }

        public Operacion(decimal monto, string cuentaOrigen, string cuentaDestino = null)
        {
            Monto = monto;
            CuentaOrigen = cuentaOrigen;
            CuentaDestino = cuentaDestino;
        }

        public abstract string Detalle(Dictionary<string, Cliente> mapaCuentas);
    }

    class Deposito : Operacion
    {
        public Deposito(string cuentaDestino, decimal monto) : base(monto, null, cuentaDestino) { }

        public override string Detalle(Dictionary<string, Cliente> mapaCuentas)
        {
            var cliente = mapaCuentas[CuentaDestino].Nombre;
            return $"-  Deposito $ {Monto:0.00} a [{CuentaDestino}/{cliente}]";
        }
    }

    class Retiro : Operacion
    {
        public Retiro(string cuentaOrigen, decimal monto) : base(monto, cuentaOrigen) { }

        public override string Detalle(Dictionary<string, Cliente> mapaCuentas)
        {
            var cliente = mapaCuentas[CuentaOrigen].Nombre;
            return $"-  Retiro $ {Monto:0.00} de [{CuentaOrigen}/{cliente}]";
        }
    }

    class Pago : Operacion
    {
        public Pago(string cuentaOrigen, decimal monto) : base(monto, cuentaOrigen) { }

        public override string Detalle(Dictionary<string, Cliente> mapaCuentas)
        {
            var cliente = mapaCuentas[CuentaOrigen].Nombre;
            return $"-  Pago $ {Monto:0.00} con [{CuentaOrigen}/{cliente}]";
        }
    }

    class Transferencia : Operacion
    {
        public Transferencia(string origen, string destino, decimal monto) : base(monto, origen, destino) { }

        public override string Detalle(Dictionary<string, Cliente> mapaCuentas)
        {
            var clienteOrigen = mapaCuentas[CuentaOrigen].Nombre;
            var clienteDestino = mapaCuentas[CuentaDestino].Nombre;
            return $"-  Transferencia $ {Monto:0.00} de [{CuentaOrigen}/{clienteOrigen}] a [{CuentaDestino}/{clienteDestino}]";
        }
    }

    
    abstract class Cuenta
    {
        public string Numero { get; }
        public decimal Saldo { get; protected set; }
        public decimal Puntos { get; protected set; }

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

        public bool Extraer(decimal monto)
        {
            if (Saldo >= monto)
            {
                Saldo -= monto;
                return true;
            }
            return false;
        }

        public bool Pagar(decimal monto)
        {
            if (Saldo >= monto)
            {
                Saldo -= monto;
                AcumularPuntos(monto);
                return true;
            }
            return false;
        }

        protected abstract void AcumularPuntos(decimal monto);
    }

    class CuentaOro : Cuenta
    {
        public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

        protected override void AcumularPuntos(decimal monto)
        {
            if (monto > 1000)
                Puntos += monto * 0.05m;
            else
                Puntos += monto * 0.03m;
        }
    }

    class CuentaPlata : Cuenta
    {
        public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }

        protected override void AcumularPuntos(decimal monto)
        {
            Puntos += monto * 0.02m;
        }
    }

    class CuentaBronce : Cuenta
    {
        public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }

        protected override void AcumularPuntos(decimal monto)
        {
            Puntos += monto * 0.01m;
        }
    }


    class Cliente
    {
        public string Nombre { get; }
        public List<Cuenta> Cuentas { get; }
        public List<Operacion> Historial { get; }

        public Cliente(string nombre)
        {
            Nombre = nombre;
            Cuentas = new List<Cuenta>();
            Historial = new List<Operacion>();
        }

        public void Agregar(Cuenta cuenta)
        {
            Cuentas.Add(cuenta);
        }

        public Cuenta ObtenerCuenta(string numero)
        {
            return Cuentas.Find(c => c.Numero == numero);
        }

        public decimal SaldoTotal => Cuentas.Sum(c => c.Saldo);
        public decimal PuntosTotal => Cuentas.Sum(c => c.Puntos);
    }

    

    class Banco
    {
        public string Nombre { get; }
        private List<Cliente> Clientes { get; }
        private List<Operacion> Operaciones { get; }

        public Banco(string nombre)
        {
            Nombre = nombre;
            Clientes = new List<Cliente>();
            Operaciones = new List<Operacion>();
        }

        public void Agregar(Cliente cliente)
        {
            Clientes.Add(cliente);
        }

        private Cliente BuscarClientePorCuenta(string numero)
        {
            return Clientes.FirstOrDefault(cliente => cliente.Cuentas.Any(c => c.Numero == numero));
        }

        private Cuenta BuscarCuenta(string numero)
        {
            return BuscarClientePorCuenta(numero)?.ObtenerCuenta(numero);
        }

        public void Registrar(Operacion operacion)
        {
            var mapaCuentas = Clientes
                .SelectMany(c => c.Cuentas.Select(cuenta => new { cuenta.Numero, Cliente = c }))
                .ToDictionary(x => x.Numero, x => x.Cliente);

            Cuenta origen = null;
            Cuenta destino = null;

            if (operacion is Deposito)
            {
                destino = BuscarCuenta(operacion.CuentaDestino);
                if (destino != null)
                {
                    destino.Depositar(operacion.Monto);
                    BuscarClientePorCuenta(destino.Numero)?.Historial.Add(operacion);
                    Operaciones.Add(operacion);
                }
            }
            else if (operacion is Retiro)
            {
                origen = BuscarCuenta(operacion.CuentaOrigen);
                if (origen != null && origen.Extraer(operacion.Monto))
                {
                    BuscarClientePorCuenta(origen.Numero)?.Historial.Add(operacion);
                    Operaciones.Add(operacion);
                }
            }
            else if (operacion is Pago)
            {
                origen = BuscarCuenta(operacion.CuentaOrigen);
                if (origen != null && origen.Pagar(operacion.Monto))
                {
                    BuscarClientePorCuenta(origen.Numero)?.Historial.Add(operacion);
                    Operaciones.Add(operacion);
                }
            }
            else if (operacion is Transferencia)
            {
                origen = BuscarCuenta(operacion.CuentaOrigen);
                destino = BuscarCuenta(operacion.CuentaDestino);
                if (origen != null && destino != null && origen.Extraer(operacion.Monto))
                {
                    destino.Depositar(operacion.Monto);
                    BuscarClientePorCuenta(origen.Numero)?.Historial.Add(operacion);
                    BuscarClientePorCuenta(destino.Numero)?.Historial.Add(operacion);
                    Operaciones.Add(operacion);
                }
            }
        }

        public void Informe()
        {
            Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}");
            foreach (var cliente in Clientes)
            {
                Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal:0.00} | Puntos Total: $ {cliente.PuntosTotal:0.00}");
                foreach (var cuenta in cliente.Cuentas)
                {
                    Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:0.00} | Puntos: $ {cuenta.Puntos:0.00}");
                    foreach (var op in cliente.Historial.Where(o => o.CuentaOrigen == cuenta.Numero || o.CuentaDestino == cuenta.Numero))
                    {
                        var mapaCuentas = Clientes
                            .SelectMany(c => c.Cuentas.Select(cuenta => new { cuenta.Numero, Cliente = c }))
                            .ToDictionary(x => x.Numero, x => x.Cliente);
                        Console.WriteLine("     " + op.Detalle(mapaCuentas));
                    }
                }
            }
        }
    }

    

    class Program
    {
        static void Main(string[] args)
        {
            var raul = new Cliente("Raul Perez");
            raul.Agregar(new CuentaOro("10001", 1000));
            raul.Agregar(new CuentaPlata("10002", 2000));

            var sara = new Cliente("Sara Lopez");
            sara.Agregar(new CuentaPlata("10003", 3000));
            sara.Agregar(new CuentaPlata("10004", 4000));

            var luis = new Cliente("Luis Gomez");
            luis.Agregar(new CuentaBronce("10005", 5000));

            var nac = new Banco("=== Banco Nac ===");
            nac.Agregar(raul);
            nac.Agregar(sara);

            var tup = new Banco("=== Banco TUP ===");
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

          
            nac.Informe();
            tup.Informe();
        }
    }
}

