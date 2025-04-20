using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaBancario
{
    // Clase abstracta Cuenta
    public abstract class Cuenta
    {
        public string Numero { get; private set; }
        public double Saldo { get; protected set; }
        public double Puntos { get; protected set; }

        public Cuenta(string numero)
        {
            Numero = numero;
            Saldo = 0;
            Puntos = 0;
        }

        public virtual void Depositar(double monto) => Saldo += monto;

        public virtual bool Extraer(double monto)
        {
            if (Saldo >= monto)
            {
                Saldo -= monto;
                return true;
            }
            return false;
        }

        public abstract void Pagar(double monto);
    }

    public class CuentaOro : Cuenta
    {
        public CuentaOro(string numero) : base(numero) { }

        public override void Pagar(double monto)
        {
            if (Extraer(monto))
            {
                double puntos = monto >= 1000 ? monto * 0.05 : monto * 0.03;
                Puntos += puntos;
            }
        }
    }

    public class CuentaPlata : Cuenta
    {
        public CuentaPlata(string numero) : base(numero) { }

        public override void Pagar(double monto)
        {
            if (Extraer(monto))
            {
                Puntos += monto * 0.02;
            }
        }
    }

    public class CuentaBronce : Cuenta
    {
        public CuentaBronce(string numero) : base(numero) { }

        public override void Pagar(double monto)
        {
            if (Extraer(monto))
            {
                Puntos += monto * 0.01;
            }
        }
    }

    public class Cliente
    {
        public string Nombre { get; private set; }
        public List<Cuenta> Cuentas { get; private set; }
        public List<Operacion> Historial { get; private set; }

        public Cliente(string nombre)
        {
            Nombre = nombre;
            Cuentas = new List<Cuenta>();
            Historial = new List<Operacion>();
        }

        public void AgregarCuenta(Cuenta cuenta)
        {
            Cuentas.Add(cuenta);
        }
    }

    public abstract class Operacion
    {
        public double Monto { get; protected set; }
        public string CuentaOrigen { get; protected set; }
        public string CuentaDestino { get; protected set; }

        public Operacion(double monto, string origen, string destino = null)
        {
            Monto = monto;
            CuentaOrigen = origen;
            CuentaDestino = destino;
        }

        public abstract string Descripcion();
    }

    public class Deposito : Operacion
    {
        public Deposito(double monto, string cuenta) : base(monto, cuenta) { }

        public override string Descripcion()
            => $"Depósito de ${Monto} en cuenta {CuentaOrigen}";
    }

    public class Retiro : Operacion
    {
        public Retiro(double monto, string cuenta) : base(monto, cuenta) { }

        public override string Descripcion()
            => $"Retiro de ${Monto} de cuenta {CuentaOrigen}";
    }

    public class Pago : Operacion
    {
        public Pago(double monto, string cuenta) : base(monto, cuenta) { }

        public override string Descripcion()
            => $"Pago de ${Monto} desde cuenta {CuentaOrigen}";
    }

    public class Transferencia : Operacion
    {
        public Transferencia(double monto, string origen, string destino) : base(monto, origen, destino) { }

        public override string Descripcion()
            => $"Transferencia de ${Monto} desde {CuentaOrigen} a {CuentaDestino}";
    }

    public class Banco
    {
        private List<Cliente> clientes = new();
        private List<Operacion> operaciones = new();
        private HashSet<string> numerosCuentas = new();

        public void AgregarCliente(Cliente cliente)
        {
            clientes.Add(cliente);
        }

        public Cuenta BuscarCuenta(string numero)
        {
            foreach (var cliente in clientes)
            {
                foreach (var cuenta in cliente.Cuentas)
                {
                    if (cuenta.Numero == numero)
                        return cuenta;
                }
            }
            return null;
        }

        public Cliente BuscarClientePorCuenta(string numero)
        {
            return clientes.FirstOrDefault(c => c.Cuentas.Any(cta => cta.Numero == numero));
        }

        public bool Depositar(string cuentaNum, double monto)
        {
            var cuenta = BuscarCuenta(cuentaNum);
            if (cuenta != null)
            {
                cuenta.Depositar(monto);
                var op = new Deposito(monto, cuentaNum);
                RegistrarOperacion(op, cuentaNum);
                return true;
            }
            return false;
        }

        public bool Retirar(string cuentaNum, double monto)
        {
            var cuenta = BuscarCuenta(cuentaNum);
            if (cuenta != null && cuenta.Extraer(monto))
            {
                var op = new Retiro(monto, cuentaNum);
                RegistrarOperacion(op, cuentaNum);
                return true;
            }
            return false;
        }

        public bool Pagar(string cuentaNum, double monto)
        {
            var cuenta = BuscarCuenta(cuentaNum);
            if (cuenta != null)
            {
                double saldoAntes = cuenta.Saldo;
                cuenta.Pagar(monto);
                if (cuenta.Saldo < saldoAntes)
                {
                    var op = new Pago(monto, cuentaNum);
                    RegistrarOperacion(op, cuentaNum);
                    return true;
                }
            }
            return false;
        }

        public bool Transferir(string origenNum, string destinoNum, double monto)
        {
            var origen = BuscarCuenta(origenNum);
            var destino = BuscarCuenta(destinoNum);
            if (origen != null && destino != null && origen.Extraer(monto))
            {
                destino.Depositar(monto);
                var op = new Transferencia(monto, origenNum, destinoNum);
                RegistrarOperacion(op, origenNum);
                return true;
            }
            return false;
        }

        private void RegistrarOperacion(Operacion op, string cuentaOrigen)
        {
            operaciones.Add(op);
            var cliente = BuscarClientePorCuenta(cuentaOrigen);
            cliente?.Historial.Add(op);
        }

        public void Reporte()
        {
            Console.WriteLine("=== HISTORIAL GLOBAL ===");
            foreach (var op in operaciones)
                Console.WriteLine(op.Descripcion());

            Console.WriteLine("\n=== ESTADO DE CUENTAS ===");
            foreach (var cliente in clientes)
            {
                foreach (var cuenta in cliente.Cuentas)
                {
                    Console.WriteLine($"Cuenta {cuenta.Numero} - Saldo: ${cuenta.Saldo}, Puntos: {cuenta.Puntos}");
                }
            }

            Console.WriteLine("\n=== HISTORIAL POR CLIENTE ===");
            foreach (var cliente in clientes)
            {
                Console.WriteLine($"\nCliente: {cliente.Nombre}");
                foreach (var op in cliente.Historial)
                    Console.WriteLine($"- {op.Descripcion()}");
            }
        }

        public string GenerarNumeroCuenta()
        {
            string num;
            Random rand = new();
            do
            {
                num = rand.Next(10000, 99999).ToString();
            } while (numerosCuentas.Contains(num));
            numerosCuentas.Add(num);
            return num;
        }
    }

    class Program
    {
        static void Main()
        {
            Banco banco = new();

            // Crear clientes
            Cliente ana = new("Ana");
            Cliente juan = new("Juan");

            // Crear cuentas
            var cuentaAna = new CuentaOro(banco.GenerarNumeroCuenta());
            var cuentaJuan = new CuentaPlata(banco.GenerarNumeroCuenta());

            ana.AgregarCuenta(cuentaAna);
            juan.AgregarCuenta(cuentaJuan);

            banco.AgregarCliente(ana);
            banco.AgregarCliente(juan);

            // Operaciones
            banco.Depositar(cuentaAna.Numero, 5000);
            banco.Depositar(cuentaJuan.Numero, 2000);

            banco.Pagar(cuentaAna.Numero, 1500);   // 5% puntos
            banco.Pagar(cuentaJuan.Numero, 1000);  // 2% puntos

            banco.Transferir(cuentaAna.Numero, cuentaJuan.Numero, 1000);

            banco.Retirar(cuentaJuan.Numero, 500);

            // Mostrar reporte
            banco.Reporte();
        }
    }
}

public class Deposito : Operacion
{
    public Deposito(double monto, string cuenta) : base(monto, cuenta) { }

    public override string Descripcion()
        => $"Depósito de ${Monto} en cuenta {CuentaOrigen}";
}

public class Retiro : Operacion
{
    public Retiro(double monto, string cuenta) : base(monto, cuenta) { }

    public override string Descripcion()
        => $"Retiro de ${Monto} de cuenta {CuentaOrigen}";
}

public class Pago : Operacion
{
    public Pago(double monto, string cuenta) : base(monto, cuenta) { }

    public override string Descripcion()
        => $"Pago de ${Monto} desde cuenta {CuentaOrigen}";
}

public class Transferencia : Operacion
{
    public Transferencia(double monto, string origen, string destino) : base(monto, origen, destino) { }

    public override string Descripcion()
        => $"Transferencia de ${Monto} desde {CuentaOrigen} a {CuentaDestino}";
}

public class Banco
{
    private List<Cliente> clientes = new();
    private List<Operacion> operaciones = new();
    private HashSet<string> numerosCuentas = new();

    public void AgregarCliente(Cliente cliente)
    {
        clientes.Add(cliente);
    }

    public Cuenta BuscarCuenta(string numero)
    {
        foreach (var cliente in clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                if (cuenta.Numero == numero)
                    return cuenta;
            }
        }
        return null;
    }

    public Cliente BuscarClientePorCuenta(string numero)
    {
        return clientes.FirstOrDefault(c => c.Cuentas.Any(cta => cta.Numero == numero));
    }

    public bool Depositar(string cuentaNum, double monto)
    {
        var cuenta = BuscarCuenta(cuentaNum);
        if (cuenta != null)
        {
            cuenta.Depositar(monto);
            var op = new Deposito(monto, cuentaNum);
            RegistrarOperacion(op, cuentaNum);
            return true;
        }
        return false;
    }

    public bool Retirar(string cuentaNum, double monto)
    {
        var cuenta = BuscarCuenta(cuentaNum);
        if (cuenta != null && cuenta.Extraer(monto))
        {
            var op = new Retiro(monto, cuentaNum);
            RegistrarOperacion(op, cuentaNum);
            return true;
        }
        return false;
    }

    public bool Pagar(string cuentaNum, double monto)
    {
        var cuenta = BuscarCuenta(cuentaNum);
        if (cuenta != null)
        {
            double saldoAntes = cuenta.Saldo;
            cuenta.Pagar(monto);
            if (cuenta.Saldo < saldoAntes)
            {
                var op = new Pago(monto, cuentaNum);
                RegistrarOperacion(op, cuentaNum);
                return true;
            }
        }
        return false;
    }

    public bool Transferir(string origenNum, string destinoNum, double monto)
    {
        var origen = BuscarCuenta(origenNum);
        var destino = BuscarCuenta(destinoNum);
        if (origen != null && destino != null && origen.Extraer(monto))
        {
            destino.Depositar(monto);
            var op = new Transferencia(monto, origenNum, destinoNum);
            RegistrarOperacion(op, origenNum);
            return true;
        }
        return false;
    }

    private void RegistrarOperacion(Operacion op, string cuentaOrigen)
    {
        operaciones.Add(op);
        var cliente = BuscarClientePorCuenta(cuentaOrigen);
        cliente?.Historial.Add(op);
    }

    public void Reporte()
    {
        Console.WriteLine("=== HISTORIAL GLOBAL ===");
        foreach (var op in operaciones)
            Console.WriteLine(op.Descripcion());

        Console.WriteLine("\n=== ESTADO DE CUENTAS ===");
        foreach (var cliente in clientes)
        {
            foreach (var cuenta in cliente.Cuentas)
            {
                Console.WriteLine($"Cuenta {cuenta.Numero} - Saldo: ${cuenta.Saldo}, Puntos: {cuenta.Puntos}");
            }
        }

        Console.WriteLine("\n=== HISTORIAL POR CLIENTE ===");
        foreach (var cliente in clientes)
        {
            Console.WriteLine($"\nCliente: {cliente.Nombre}");
            foreach (var op in cliente.Historial)
                Console.WriteLine($"- {op.Descripcion()}");
        }
    }

    public string GenerarNumeroCuenta()
    {
        string num;
        Random rand = new();
        do
        {
            num = rand.Next(10000, 99999).ToString();
        } while (numerosCuentas.Contains(num));
        numerosCuentas.Add(num);
        return num;
    }
}

// --- Código principal como script ---
Banco banco = new();

Cliente ana = new("Ana");
Cliente juan = new("Juan");

var cuentaAna = new CuentaOro(banco.GenerarNumeroCuenta());
var cuentaJuan = new CuentaPlata(banco.GenerarNumeroCuenta());

ana.AgregarCuenta(cuentaAna);
juan.AgregarCuenta(cuentaJuan);

banco.AgregarCliente(ana);
banco.AgregarCliente(juan);

banco.Depositar(cuentaAna.Numero, 5000);
banco.Depositar(cuentaJuan.Numero, 2000);

banco.Pagar(cuentaAna.Numero, 1500);  
banco.Pagar(cuentaJuan.Numero, 1000);  

banco.Transferir(cuentaAna.Numero, cuentaJuan.Numero, 1000);

banco.Retirar(cuentaJuan.Numero, 500);

banco.Reporte();
