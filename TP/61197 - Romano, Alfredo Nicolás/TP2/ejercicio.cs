namespace TP2
{
    abstract class Cuenta
    {
        public string Numero { get; private set; }        // Número de cuenta (único)
        public double Saldo { get; protected set; }        // Dinero en la cuenta
        public double Puntos { get; protected set; }       // Puntos acumulados
        public Cliente Titular { get; private set; }       // Dueño de la cuenta
        public List<Operacion> Historial { get; } = new(); // Lista de operaciones de esta cuenta


        public Cuenta (string numero, double saldo)
        {
            Numero = numero;
            Saldo = saldo;
            Puntos = 0;
        }

        public void AsignarTitular(Cliente c) => Titular = c;
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
        public void AgregarOperacion(Operacion op) => Historial.Add(op);
    }

    class CuentaOro : Cuenta
    {
        public CuentaOro(string numero, double saldo) : base(numero, saldo) { }

        public override void Pagar(double monto)
        {
            if (Extraer(monto))
            {
                if (monto > 1000) Puntos += monto * 0.05;
                else Puntos += monto * 0.03;
            }
            else throw new InvalidOperationException("Fondos insuficientes.");
        }
    }

    class CuentaPlata : Cuenta
    {
        public CuentaPlata(string numero, double saldo) : base(numero, saldo) { }

        public override void Pagar(double monto)
        {
            if (Extraer(monto)) Puntos += monto * 0.02;
            else throw new InvalidOperationException("Fondos insuficientes.");
        }
    }    

    class CuentaBronce : Cuenta
    {
        public CuentaBronce(string numero, double saldo) : base(numero, saldo) { }

        public override void Pagar(double monto)
        {
            if (Extraer(monto)) Puntos += monto * 0.01;
            else throw new InvalidOperationException("Fondos insuficientes.");
        }
    }

    // Cliente
    class Cliente
    {
        public string Nombre { get; }
        public List<Cuenta> Cuentas { get; } = new();

        public Cliente(string nombre) => Nombre = nombre;

        public void Agregar(Cuenta cuenta)
        {
            cuenta.AsignarTitular(this);
            Cuentas.Add(cuenta);
        }

        public double SaldoTotal() => Cuentas.Sum(c => c.Saldo);
        public double PuntosTotal() => Cuentas.Sum(c => c.Puntos);
    }

    // Operaciones
    abstract class Operacion
    {
        public double Monto { get; protected set; }
        public abstract void Ejecutar(Banco banco);
        public abstract string Detalle();
    }

    class Deposito : Operacion
    {
        string CuentaDestino;
        public Deposito(string destino, double monto)
        {
            CuentaDestino = destino;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var cuenta = banco.BuscarCuenta(CuentaDestino);
            cuenta.Depositar(Monto);
            cuenta.AgregarOperacion(this);
            banco.AgregarOperacion(this);
        }

        public override string Detalle() => $"Deposito $ {Monto:0.00} a [{CuentaDestino}/{Banco.NombreCliente(CuentaDestino)}]";
    }
    
    class Retiro : Operacion
    {
        string CuentaOrigen;
        public Retiro(string origen, double monto)
        {
            CuentaOrigen = origen;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var cuenta = banco.BuscarCuenta(CuentaOrigen);
            if (!cuenta.Extraer(Monto)) throw new InvalidOperationException("Fondos insuficientes.");
            cuenta.AgregarOperacion(this);
            banco.AgregarOperacion(this);
        }

        public override string Detalle() => $"Retiro $ {Monto:0.00} de [{CuentaOrigen}/{Banco.NombreCliente(CuentaOrigen)}]";
    }

    class Pago : Operacion
    {
        string CuentaOrigen;
        public Pago(string origen, double monto)
        {
            CuentaOrigen = origen;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var cuenta = banco.BuscarCuenta(CuentaOrigen);
            cuenta.Pagar(Monto);
            cuenta.AgregarOperacion(this);
            banco.AgregarOperacion(this);
        }

        public override string Detalle() => $"Pago $ {Monto:0.00} con [{CuentaOrigen}/{Banco.NombreCliente(CuentaOrigen)}]";
    }

    class Transferencia : Operacion
    {
        string CuentaOrigen, CuentaDestino;
        public Transferencia(string origen, string destino, double monto)
        {
            CuentaOrigen = origen;
            CuentaDestino = destino;
            Monto = monto;
        }

        public override void Ejecutar(Banco banco)
        {
            var origen = banco.BuscarCuenta(CuentaOrigen);
            var destino = banco.BuscarCuentaGlobal(CuentaDestino);
            if (!origen.Extraer(Monto)) throw new InvalidOperationException("Fondos insuficientes.");
            destino.Depositar(Monto);
            origen.AgregarOperacion(this);
            destino.AgregarOperacion(this);
            banco.AgregarOperacion(this);
        }

        public override string Detalle() => $"Transferencia $ {Monto:0.00} de [{CuentaOrigen}/{Banco.NombreCliente(CuentaOrigen)}] a [{CuentaDestino}/{Banco.NombreCliente(CuentaDestino)}]";
    }
    // Banco
    class Banco
    {
        string nombre;
        public static Dictionary<string, Cuenta> GlobalCuentas = new();
        List<Cliente> clientes = new();
        List<Operacion> historial = new();

        public Banco(string nombre) => this.nombre = nombre;

        public void Agregar(Cliente cliente)
        {
            clientes.Add(cliente);
            foreach (var c in cliente.Cuentas) GlobalCuentas[c.Numero] = c;
        }

        public void Registrar(Operacion op) => op.Ejecutar(this);

        public Cuenta BuscarCuenta(string numero)
        {
            if (GlobalCuentas.TryGetValue(numero, out var cuenta))
            {
                if (!clientes.Any(c => c.Cuentas.Contains(cuenta))) throw new InvalidOperationException("La cuenta no pertenece a este banco.");
                return cuenta;
            }
            throw new InvalidOperationException("Cuenta no encontrada.");
        }

        public Cuenta BuscarCuentaGlobal(string numero)
        {
            if (GlobalCuentas.TryGetValue(numero, out var cuenta)) return cuenta;
            throw new InvalidOperationException("Cuenta no encontrada.");
        }

        public void AgregarOperacion(Operacion op) => historial.Add(op);

        public static string NombreCliente(string numero) => GlobalCuentas.TryGetValue(numero, out var cuenta) ? cuenta.Titular.Nombre : "Desconocido";

        public void Informe()
        {
            Console.WriteLine($"\nBanco: {nombre} | Clientes: {clientes.Count}\n");

            foreach (var cliente in clientes)
            {
                Console.WriteLine($"  Cliente: {cliente.Nombre} | Saldo Total: $ {cliente.SaldoTotal():0.00} | Puntos Total: $ {cliente.PuntosTotal():0.00}\n");

                foreach (var cuenta in cliente.Cuentas)
                {
                    Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:0.00} | Puntos: $ {cuenta.Puntos:0.00}");
                    foreach (var op in cuenta.Historial)
                    {
                        Console.WriteLine($"     -  {op.Detalle()}");
                    }
                    Console.WriteLine();
                }
            }
        }
    }

    // Programa principal
    class Programa
    {
        static void Main()
        {
            // Definiciones 
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

            // Registrar Operaciones
            nac.Registrar(new Deposito("10001", 100));
            nac.Registrar(new Retiro("10002", 200));
            nac.Registrar(new Transferencia("10001", "10002", 300));
            nac.Registrar(new Transferencia("10003", "10004", 500));
            nac.Registrar(new Pago("10002", 400));

            tup.Registrar(new Deposito("10005", 100));
            tup.Registrar(new Retiro("10005", 200));
            tup.Registrar(new Transferencia("10005", "10002", 300));
            tup.Registrar(new Pago("10005", 400));

            // Informe final
            nac.Informe();
            tup.Informe();
        }
    }
}