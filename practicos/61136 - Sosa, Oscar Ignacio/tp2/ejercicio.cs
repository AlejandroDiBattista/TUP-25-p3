using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaBancario
{
    
    public abstract class Operacion
    {
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public abstract void Ejecutar();

        public Operacion(decimal monto)
        {
            Monto = monto;
            Fecha = DateTime.Now;
        }
    }

    
    public abstract class Cuenta
    {
        public string NumeroCuenta { get; set; }
        public decimal Saldo { get; protected set; }
        public List<Operacion> HistorialOperaciones { get; protected set; }
        public int PuntosAcumulados { get; protected set; }

        public Cuenta(string numeroCuenta)
        {
            NumeroCuenta = numeroCuenta;
            Saldo = 0;
            HistorialOperaciones = new List<Operacion>();
            PuntosAcumulados = 0;
        }

        public abstract void EjecutarOperacion(Operacion operacion);

        public void AgregarOperacion(Operacion operacion)
        {
            HistorialOperaciones.Add(operacion);
        }

        public void MostrarEstado()
        {
            Console.WriteLine($"Cuenta: {NumeroCuenta}, Saldo: {Saldo:C}, Puntos: {PuntosAcumulados}");
        }
    }

    public class CuentaOro : Cuenta
    {
        public CuentaOro(string numeroCuenta) : base(numeroCuenta) { }

        public override void EjecutarOperacion(Operacion operacion)
        {
            if (operacion is Deposito)
            {
                Saldo += operacion.Monto;
            }
            else if (operacion is Retiro)
            {
                if (Saldo >= operacion.Monto)
                    Saldo -= operacion.Monto;
                else
                    throw new InvalidOperationException("Fondos insuficientes");
            }
            else if (operacion is Pago)
            {
                if (Saldo >= operacion.Monto)
                {
                    Saldo -= operacion.Monto;
                
                    PuntosAcumulados += operacion.Monto > 1000 ? (int)(operacion.Monto * 0.05m) : (int)(operacion.Monto * 0.03m);
                }
                else
                    throw new InvalidOperationException("Fondos insuficientes");
            }
            else if (operacion is Transferencia)
            {
                Transferencia trans = operacion as Transferencia;
                if (Saldo >= trans.Monto)
                {
                    Saldo -= trans.Monto;
                    trans.CuentaDestino.Saldo += trans.Monto;
                }
                else
                    throw new InvalidOperationException("Fondos insuficientes");
            }

            AgregarOperacion(operacion);
        }
    }

    public class CuentaPlata : Cuenta
    {
        public CuentaPlata(string numeroCuenta) : base(numeroCuenta) { }

        public override void EjecutarOperacion(Operacion operacion)
        {
            if (operacion is Deposito)
            {
                Saldo += operacion.Monto;
            }
            else if (operacion is Retiro)
            {
                if (Saldo >= operacion.Monto)
                    Saldo -= operacion.Monto;
                else
                    throw new InvalidOperationException("Fondos insuficientes");
            }
            else if (operacion is Pago)
            {
                if (Saldo >= operacion.Monto)
                {
                    Saldo -= operacion.Monto;
                    
                    PuntosAcumulados += (int)(operacion.Monto * 0.02m);
                }
                else
                    throw new InvalidOperationException("Fondos insuficientes");
            }
            else if (operacion is Transferencia)
            {
                Transferencia trans = operacion as Transferencia;
                if (Saldo >= trans.Monto)
                {
                    Saldo -= trans.Monto;
                    trans.CuentaDestino.Saldo += trans.Monto;
                }
                else
                    throw new InvalidOperationException("Fondos insuficientes");
            }

            AgregarOperacion(operacion);
        }
    }

    public class CuentaBronce : Cuenta
    {
        public CuentaBronce(string numeroCuenta) : base(numeroCuenta) { }

        public override void EjecutarOperacion(Operacion operacion)
        {
            if (operacion is Deposito)
            {
                Saldo += operacion.Monto;
            }
            else if (operacion is Retiro)
            {
                if (Saldo >= operacion.Monto)
                    Saldo -= operacion.Monto;
                else
                    throw new InvalidOperationException("Fondos insuficientes");
            }
            else if (operacion is Pago)
            {
                if (Saldo >= operacion.Monto)
                {
                    Saldo -= operacion.Monto;
                
                    PuntosAcumulados += (int)(operacion.Monto * 0.01m);
                }
                else
                    throw new InvalidOperationException("Fondos insuficientes");
            }
            else if (operacion is Transferencia)
            {
                Transferencia trans = operacion as Transferencia;
                if (Saldo >= trans.Monto)
                {
                    Saldo -= trans.Monto;
                    trans.CuentaDestino.Saldo += trans.Monto;
                }
                else
                    throw new InvalidOperationException("Fondos insuficientes");
            }

            AgregarOperacion(operacion);
        }
    }

    public class Deposito : Operacion
    {
        public Cuenta CuentaDestino { get; set; }

        public Deposito(decimal monto, Cuenta cuentaDestino) : base(monto)
        {
            CuentaDestino = cuentaDestino;
        }

        public override void Ejecutar()
        {
            CuentaDestino.Saldo += Monto;
        }
    }

    public class Retiro : Operacion
    {
        public Cuenta CuentaOrigen { get; set; }

        public Retiro(decimal monto, Cuenta cuentaOrigen) : base(monto)
        {
            CuentaOrigen = cuentaOrigen;
        }

        public override void Ejecutar()
        {
            if (CuentaOrigen.Saldo >= Monto)
                CuentaOrigen.Saldo -= Monto;
            else
                throw new InvalidOperationException("Fondos insuficientes");
        }
    }

    public class Pago : Operacion
    {
        public Cuenta CuentaDestino { get; set; }

        public Pago(decimal monto, Cuenta cuentaDestino) : base(monto)
        {
            CuentaDestino = cuentaDestino;
        }

        public override void Ejecutar()
        {
            CuentaDestino.Saldo -= Monto;
        }
    }

    public class Transferencia : Operacion
    {
        public Cuenta CuentaOrigen { get; set; }
        public Cuenta CuentaDestino { get; set; }

        public Transferencia(decimal monto, Cuenta cuentaOrigen, Cuenta cuentaDestino) : base(monto)
        {
            CuentaOrigen = cuentaOrigen;
            CuentaDestino = cuentaDestino;
        }

        public override void Ejecutar()
        {
            if (CuentaOrigen.Saldo >= Monto)
            {
                CuentaOrigen.Saldo -= Monto;
                CuentaDestino.Saldo += Monto;
            }
            else
                throw new InvalidOperationException("Fondos insuficientes");
        }
    }

    
    public class Cliente
    {
        public string Nombre { get; set; }
        public List<Cuenta> Cuentas { get; set; }

        public Cliente(string nombre)
        {
            Nombre = nombre;
            Cuentas = new List<Cuenta>();
        }

        public void AgregarCuenta(Cuenta cuenta)
        {
            Cuentas.Add(cuenta);
        }

        public void MostrarHistorial()
        {
            Console.WriteLine($"Historial de operaciones de {Nombre}:");
            foreach (var cuenta in Cuentas)
            {
                Console.WriteLine($"Historial de la cuenta {cuenta.NumeroCuenta}:");
                foreach (var operacion in cuenta.HistorialOperaciones)
                {
                    Console.WriteLine($"Tipo: {operacion.GetType().Name}, Monto: {operacion.Monto:C}, Fecha: {operacion.Fecha}");
                }
            }
        }
    }

    
    public class Banco
    {
        public List<Cliente> Clientes { get; set; }
        public List<Operacion> HistorialGlobal { get; set; }

        public Banco()
        {
            Clientes = new List<Cliente>();
            HistorialGlobal = new List<Operacion>();
        }

        public void AgregarCliente(Cliente cliente)
        {
            Clientes.Add(cliente);
        }

        public void EjecutarOperacion(Operacion operacion)
        {
            operacion.Ejecutar();
            HistorialGlobal.Add(operacion);
        }

        public void MostrarInforme()
        {
            Console.WriteLine("Informe Global:");
            foreach (var operacion in HistorialGlobal)
            {
                Console.WriteLine($"Tipo: {operacion.GetType().Name}, Monto: {operacion.Monto:C}, Fecha: {operacion.Fecha}");
            }

            foreach (var cliente in Clientes)
            {
                cliente.MostrarHistorial();
                foreach (var cuenta in cliente.Cuentas)
                {
                    cuenta.MostrarEstado();
                }
            }
        }
    }

    
    class Program
    {
        static void Main(string[] args)
        {
            Banco banco = new Banco();

            
            Cliente cliente1 = new Cliente("Juan Pérez");
            Cliente cliente2 = new Cliente("María López");

        
            Cuenta cuenta1 = new CuentaOro("12345");
            Cuenta cuenta2 = new CuentaPlata("67890");
            Cuenta cuenta3 = new CuentaBronce("11223");

        
            cliente1.AgregarCuenta(cuenta1);
            cliente1.AgregarCuenta(cuenta2);
            cliente2.AgregarCuenta(cuenta3);

            banco.AgregarCliente(cliente1);
            banco.AgregarCliente(cliente2);

        
            banco.EjecutarOperacion(new Deposito(2000, cuenta1));
            banco.EjecutarOperacion(new Pago(1500, cuenta1));
            banco.EjecutarOperacion(new Transferencia(1000, cuenta1, cuenta3));
            banco.EjecutarOperacion(new Retiro(500, cuenta2));

        
            banco.MostrarInforme();
        }
    }
}
