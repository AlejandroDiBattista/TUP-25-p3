class Cliente
{
    public string Nombre { get; set; }

    private List<Cuenta> Cuentas { get; set; }

    private List<Operacion> operaciones = new List<Operacion>();
    public Cliente(string nombre)
    {
        Nombre = nombre;
        Cuentas = new List<Cuenta> { };
    }

    public void Agregar(Cuenta cuenta)
    {
        Cuentas.Add(cuenta);
        cuenta.Cliente = this;
        Banco.Registrar(cuenta);
    }
    public void RegistrarOperacion(Operacion operacion)
    {
        operaciones.Add(operacion);
    }
    public void Informe()
    {
        decimal saldoTotal = 0;
        int puntosTotal = 0;

        foreach (var cuenta in Cuentas)
        {
            saldoTotal += cuenta.Saldo;
            puntosTotal += cuenta.Puntaje;
        }

        Console.WriteLine($"  Cliente: {Nombre} | Saldo Total: $ {saldoTotal:F2} | Puntos Total: $ {puntosTotal:F2}\n");

        foreach (var cuenta in Cuentas)
        {
            Console.WriteLine($"    Cuenta: {cuenta.Numero} | Saldo: $ {cuenta.Saldo:F2} | Puntos: $ {cuenta.Puntaje:F2}");
            foreach (var operacion in operaciones)
            {
                if (operacion.Origen.Numero == cuenta.Numero)
                {
                    Console.WriteLine($"     -  {operacion.Descripcion}");
                }
            }
        }
    }

}
abstract class Cuenta
{
    public string Numero { get; set; }

    private decimal saldo;
    public int Puntaje { get; set; }

    public Cliente Cliente { get; set; }

    public Cuenta(string numero, decimal saldo)
    {
        Numero = numero;
        Saldo = saldo;

    }

    public decimal Saldo
    {
        get
        {
            return saldo;
        }
        set
        {
            if (value < 0)
            {
                Console.WriteLine("No puede ingresar saldo negativo");
                saldo = 0;
            }

            saldo = value;
        }
    }


    public virtual void Informacion(string numeroCuenta, decimal saldo)
    {
        Console.WriteLine($"Numero de cuenta: {numeroCuenta} | Saldo: {saldo} ");
    }

    public bool Depositar(decimal cantidad)
    {
        if (cantidad <= 0)
        {
            return false;
        }
        Saldo += cantidad;
        return true;
    }

    public bool Extraer(decimal cantidad)
    {
        if (cantidad <= 0) return false;
        if (cantidad > Saldo) return false;

        Saldo -= cantidad;
        return true;
    }
}
class Banco
{
    public string Nombre { get; set; }

    public List<Cliente> Clientes;

    public List<Operacion> Operaciones;

    public Banco(string nombre)
    {
        Nombre = nombre;
        Clientes = new List<Cliente>();
        this.Operaciones = new List<Operacion>();
    }

    public void Agregar(Cliente cliente)
    {
        Clientes.Add(cliente);
    }

    public void Registrar(Operacion operacion)
    {
        if (operacion.Ejecutar())
        {
            Operaciones.Add(operacion);
            operacion.Origen.Cliente.RegistrarOperacion(operacion);
        }

    }

    public static Dictionary<string, Cuenta> Cuentas = new Dictionary<string, Cuenta>();

    public static void Registrar(Cuenta cuenta)
    {
        if (Cuentas.ContainsKey(cuenta.Numero))
        {
            Console.WriteLine($"La cuenta {cuenta.Numero} ya existe");
        }
        else
        {
            Cuentas.Add(cuenta.Numero, cuenta);
        }
    }

    public static Cuenta Buscar(string numero)
    {
        if (Cuentas.ContainsKey(numero))
        {
            return Cuentas[numero];
        }
        else
        {
            throw new Exception($"La cuenta con el número {numero} no existe.");
        }
    }

    public void Informe()
    {
        Console.WriteLine($"Banco: {Nombre} | Clientes: {Clientes.Count}\n");
        foreach (var cliente in Clientes)
        {
            cliente.Informe();
        }
    }

}
abstract class Operacion
{
    public Cuenta Origen { get; set; }

    public decimal Monto { get; set; }


    public Operacion(string numero, decimal monto)
    {
        Origen = Banco.Buscar(numero);
        Monto = monto;
    }


    public abstract bool Ejecutar();
    public virtual string Descripcion { 
        get { return $"Deposito $ {Monto:F2} a [{Origen.Numero}/{Origen.Cliente.Nombre}]"; } 
    }
}

class CuentaOro : Cuenta
{
    public CuentaOro(string numero, decimal saldo) : base(numero, saldo) { }

    public override void Informacion(string numeroCuenta, decimal saldo)
    {
        Console.WriteLine($"Numero de cuenta: {numeroCuenta} | Saldo: {saldo} | Tipo: Oro");
    }

    public void AcumularPuntos(decimal monto)
    {
        if (monto > 1000)
        {
            Puntaje += (int)(monto * 0.05m);
        }
        else
        {
            Puntaje += (int)(monto * 0.03m);
        }
    }

}
class CuentaPlata : Cuenta
{
    public CuentaPlata(string numero, decimal saldo) : base(numero, saldo) { }
    public override void Informacion(string numeroCuenta, decimal saldo)
    {
        Console.WriteLine($"Numero de cuenta: {numeroCuenta} | Saldo: {saldo} | Tipo: Plata");
    }
    public void AcumularPuntos(decimal monto)
    {
        Puntaje += (int)(monto * 0.02m);
    }
}
class CuentaBronce : Cuenta
{
    public CuentaBronce(string numero, decimal saldo) : base(numero, saldo) { }
    public override void Informacion(string numeroCuenta, decimal saldo)
    {
        Console.WriteLine($"Numero de cuenta: {numeroCuenta} | Saldo: {saldo} | Tipo: Bronce");
    }
    public void AcumularPuntos(decimal monto)
    {
        Puntaje += (int)(monto * 0.01m);
    }
}

class Deposito : Operacion
{
    public Deposito(string numero, decimal monto) : base(numero, monto) { }
    public override bool Ejecutar()
    {
        return Origen.Depositar(Monto);
    }

    public override string Descripcion
    {
        get
        {
            return $"Deposito {Monto} a {Origen.Numero}";
        }
    }
}
class Retiro : Operacion
{
    public Retiro(string numero, decimal monto) : base(numero, monto) { }
    public override bool Ejecutar()
    {
        return Origen.Extraer(Monto);
    }
    public override string Descripcion
    {
        get
        {
            return $"Retiro {Monto} de {Origen.Numero}";
        }
    }
}
class Transferencia : Operacion
{
    public string Destino { get; set; }
    public Transferencia(string origen, string destino, decimal monto) : base(origen, monto)
    {
        Destino = destino;
    }
    public override bool Ejecutar()
    {
        if (Origen.Extraer(Monto))
        {
            var cuentaDestino = Banco.Buscar(Destino);
            return cuentaDestino.Depositar(Monto);
        }
        return false;
    }
    public override string Descripcion
    {
        get
        {
            return $"Transferencia {Monto} de {Origen.Numero} a {Destino}";
        }
    }
}
class Pago : Operacion
{
    public Pago(string numero, decimal monto) : base(numero, monto) { }
    public override bool Ejecutar()
    {
        return Origen.Extraer(Monto);
    }
    public override string Descripcion
    {
        get
        {
            return $"Pago {Monto} de {Origen.Numero}";
        }
    }
}



   var raul = new Cliente("Raul Perez");
   raul.Agregar(new CuentaOro("10001", 1000));
   raul.Agregar(new CuentaPlata("10002", 2000));


   // Definiciones 

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

   //Registrar Operaciones

   nac.Registrar(new Deposito("10001", 100));
   nac.Registrar(new Retiro("10002", 200));
   nac.Registrar(new Transferencia("10001", "10002", 300));
   nac.Registrar(new Transferencia("10003", "10004", 500));
   nac.Registrar(new Pago("10002", 400));

   tup.Registrar(new Deposito("10005", 100));
   tup.Registrar(new Retiro("10005", 200));
   tup.Registrar(new Transferencia("10005", "10002", 300));
   tup.Registrar(new Pago("10005", 400));

   //Informe final
   Console.Clear();
   nac.Informe();
   tup.Informe();