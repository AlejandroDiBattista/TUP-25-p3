// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.


using System;
using System.Collections.Generic;

namespace TP2_SistemaBancario
{
    public class SistemaBancario
    {
    
        Cliente raul = new Cliente("Raul Perez");
        Cliente sara = new Cliente("Sara Lopez");
        Cliente luis = new Cliente("Luis Gomez");

        Banco bancoNac = new Banco("Banco Nac");
        Banco bancoTup = new Banco("Banco TUP");

        public SistemaBancario()
        {
            raul.Agregar(new CuentaOro("10001", 1000));
            raul.Agregar(new CuentaPlata("10002", 2000));

            sara.Agregar(new CuentaPlata("10003", 3000));
            sara.Agregar(new CuentaPlata("10004", 4000));

            luis.Agregar(new CuentaBronce("10005", 5000));

            bancoNac.Agregar(raul);
            bancoNac.Agregar(sara);

            bancoTup.Agregar(luis);

            bancoNac.Registrar(new Deposito("10001", 100));
            bancoNac.Registrar(new Retiro("10002", 200));
            bancoNac.Registrar(new Transferencia("10001", "10002", 300));
            bancoNac.Registrar(new Transferencia("10003", "10004", 500));
            bancoNac.Registrar(new Pago("10002", 400));

            bancoTup.Registrar(new Deposito("10005", 100));
            bancoTup.Registrar(new Retiro("10005", 200));
            bancoTup.Registrar(new Transferencia("10005", "10002", 300));
            bancoTup.Registrar(new Pago("10005", 400));

            bancoNac.Informe();
            bancoTup.Informe();
        }
    }

    public abstract class Cuenta
    {
        public string Numero;
        public double Saldo;
        public double Puntos;

        public Cuenta(string numero, double saldo)
        {
            Numero = numero;
            Saldo = saldo;
            Puntos = 0;
        }

        public void Depositar(double monto)
        {
            Saldo += monto;
        }

        public bool Extraer(double monto)
        {
            if (Saldo >= monto)
            {
                Saldo -= monto;
                return true;
            }
            return false;
        }

        public bool Pagar(double monto)
        {
            if (Saldo >= monto)
            {
                Saldo -= monto;
                AcumularPuntos(monto);
                return true;
            }
            return false;
        }

        public abstract void AcumularPuntos(double monto);
    }

    public class CuentaOro : Cuenta
    {
        public CuentaOro(string numero, double saldo) : base(numero, saldo) { }

        public override void AcumularPuntos(double monto)
        {
            if (monto > 1000)
                Puntos += monto * 0.05;
            else
                Puntos += monto * 0.03;
        }
    }

    public class CuentaPlata : Cuenta
    {
        public CuentaPlata(string numero, double saldo) : base(numero, saldo) { }

        public override void AcumularPuntos(double monto)
        {
            Puntos += monto * 0.02;
        }
    }

    public class CuentaBronce : Cuenta
    {
        public CuentaBronce(string numero, double saldo) : base(numero, saldo) { }

        public override void AcumularPuntos(double monto)
        {
            Puntos += monto * 0.01;
        }
    }

    public class Cliente
    {
        public string Nombre;
        List<Cuenta> cuentas = new List<Cuenta>();
        List<Operacion> historial = new List<Operacion>();

        public Cliente(string nombre)
        {
            Nombre = nombre;
        }

        public void Agregar(Cuenta cuenta)
        {
            cuentas.Add(cuenta);
            Banco.RegistraCuenta(cuenta.Numero, this);
        }

        public Cuenta ObtenerCuenta(string numero)
        {
            for (int i = 0; i < cuentas.Count; i++)
            {
                if (cuentas[i].Numero == numero)
                    return cuentas[i];
            }
            return null;
        }

        public void AgregarOperacion(Operacion op)
        {
            historial.Add(op);
        }

        public double TotalSaldo()
        {
            double total = 0;
            for (int i = 0; i < cuentas.Count; i++)
            {
                total += cuentas[i].Saldo;
            }
            return total;
        }

        public double TotalPuntos()
        {
            double total = 0;
            for (int i = 0; i < cuentas.Count; i++)
            {
                total += cuentas[i].Puntos;
            }
            return total;
        }

        public string Informe()
        {
            string texto = "  Cliente: " + Nombre + " | Saldo Total: $" + TotalSaldo().ToString("0.00") + " | Puntos: " + TotalPuntos().ToString("0.00") + "\n";
            for (int i = 0; i < cuentas.Count; i++)
            {
                var cuenta = cuentas[i];
                texto += "\n    Cuenta: " + cuenta.Numero + " | Saldo: $" + cuenta.Saldo.ToString("0.00") + " | Puntos: " + cuenta.Puntos.ToString("0.00") + "\n";
                for (int j = 0; j < historial.Count; j++)
                {
                    if (historial[j].InvolucraCuenta(cuenta.Numero))
                    {
                        texto += "     -  " + historial[j].Descripcion() + "\n";
                    }
                }
            }
            return texto;
        }

        public bool TieneCuenta(string numero)
        {
            for (int i = 0; i < cuentas.Count; i++)
            {
                if (cuentas[i].Numero == numero)
                    return true;
            }
            return false;
        }
    }

    public abstract class Operacion
    {
        public double Monto;
        public abstract void Ejecutar(Banco banco);
        public abstract string Descripcion();
        public abstract bool InvolucraCuenta(string numero);
    }

    public class Deposito : Operacion
    {
        string cuenta;

        public Deposito(string cuenta, double monto)
        {
            this.cuenta = cuenta;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var cli = banco.BuscarClientePorCuenta(cuenta);
            var cta = cli?.ObtenerCuenta(cuenta);
            if (cta != null)
            {
                cta.Depositar(Monto);
                cli.AgregarOperacion(this);
            }
        }

        public override string Descripcion()
        {
            return "Deposito $" + Monto.ToString("0.00") + " a [" + cuenta + "/" + Banco.ObtenerNombreCliente(cuenta) + "]";
        }

        public override bool InvolucraCuenta(string numero)
        {
            return numero == cuenta;
        }
    }

    public class Retiro : Operacion
    {
        string cuenta;

        public Retiro(string cuenta, double monto)
        {
            this.cuenta = cuenta;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var cli = banco.BuscarClientePorCuenta(cuenta);
            var cta = cli?.ObtenerCuenta(cuenta);
            if (cta != null && cta.Extraer(Monto))
            {
                cli.AgregarOperacion(this);
            }
        }

        public override string Descripcion()
        {
            return "Retiro $" + Monto.ToString("0.00") + " de [" + cuenta + "/" + Banco.ObtenerNombreCliente(cuenta) + "]";
        }

        public override bool InvolucraCuenta(string numero)
        {
            return numero == cuenta;
        }
    }

    public class Pago : Operacion
    {
        string cuenta;

        public Pago(string cuenta, double monto)
        {
            this.cuenta = cuenta;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var cli = banco.BuscarClientePorCuenta(cuenta);
            var cta = cli?.ObtenerCuenta(cuenta);
            if (cta != null && cta.Pagar(Monto))
            {
                cli.AgregarOperacion(this);
            }
        }

        public override string Descripcion()
        {
            return "Pago $" + Monto.ToString("0.00") + " con [" + cuenta + "/" + Banco.ObtenerNombreCliente(cuenta) + "]";
        }

        public override bool InvolucraCuenta(string numero)
        {
            return numero == cuenta;
        }
    }

    public class Transferencia : Operacion
    {
        string origen, destino;

        public Transferencia(string origen, string destino, double monto)
        {
            this.origen = origen;
            this.destino = destino;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var cliO = banco.BuscarClientePorCuenta(origen);
            var cliD = banco.BuscarClientePorCuenta(destino);
            var ctaO = cliO?.ObtenerCuenta(origen);
            var ctaD = cliD?.ObtenerCuenta(destino);
            if (ctaO != null && ctaD != null && ctaO.Extraer(Monto))
            {
                ctaD.Depositar(Monto);
                cliO.AgregarOperacion(this);
                cliD.AgregarOperacion(this);
            }
        }

        public override string Descripcion()
        {
            return "Transferencia $" + Monto.ToString("0.00") + " de [" + origen + "/" + Banco.ObtenerNombreCliente(origen) + "] a [" + destino + "/" + Banco.ObtenerNombreCliente(destino) + "]";
        }

        public override bool InvolucraCuenta(string numero)
        {
            return numero == origen || numero == destino;
        }
    }

    public class Banco
    {
        string nombre;
        List<Cliente> clientes = new List<Cliente>();
        static Dictionary<string, string> cuentaCliente = new Dictionary<string, string>();

        public Banco(string nombre)
        {
            this.nombre = nombre;
        }

        public void Agregar(Cliente cli)
        {
            clientes.Add(cli);
        }

        public Cliente BuscarClientePorCuenta(string numero)
        {
            for (int i = 0; i < clientes.Count; i++)
            {
                if (clientes[i].TieneCuenta(numero))
                    return clientes[i];
            }
            return null;
        }

        public void Registrar(Operacion op)
        {
            op.Ejecutar(this);
        }

        public void Informe()
        {
            Console.WriteLine("\nBanco: " + nombre + " | Clientes: " + clientes.Count + "\n");
            for (int i = 0; i < clientes.Count; i++)
            {
                Console.WriteLine(clientes[i].Informe());
            }
        }

        public static void RegistraCuenta(string cuenta, Cliente cli)
        {
            cuentaCliente[cuenta] = cli.Nombre;
        }

        public static string ObtenerNombreCliente(string cuenta)
        {
            return cuentaCliente.ContainsKey(cuenta) ? cuentaCliente[cuenta] : "Desconocido";
        }
    }
}


