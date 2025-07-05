using System;

public class SistemaBancario 
{
    public class Cliente {
        private string nombre;
        private Cuenta[] cuentas;
        private Operacion[] historialOperaciones;

        public string Nombre => nombre;

        public Cliente(string nombre) {
            this.nombre = nombre;
            cuentas = new Cuenta[0];
            historialOperaciones = new Operacion[0];
        }

        public Cuenta Agregar(Cuenta cuenta) {
            Array.Resize(ref cuentas, cuentas.Length + 1);
            cuenta.AsignarTitular(this);
            cuentas[cuentas.Length - 1] = cuenta;
            return cuenta;
        }

        public Cuenta? BuscarCuenta(string numero) {
            foreach (var cuenta in cuentas) {
                if (cuenta.Numero == numero) return cuenta;
            }
            return null;
        }

        public Cuenta? BuscarCuenta(Cuenta cuentaBuscada) {
            foreach (var cuenta in cuentas) {
                if (cuenta == cuentaBuscada) return cuenta;
            }
            return null;
        }

        public void RegistrarOperacion(Operacion operacion) {
            Array.Resize(ref historialOperaciones, historialOperaciones.Length + 1);
            historialOperaciones[historialOperaciones.Length - 1] = operacion;
        }
        
        public Cuenta[] GetCuentas() {
            return cuentas;
        }

        public void MostrarCuentas() {
            Console.WriteLine("Cuentas:");
            foreach (var cuenta in cuentas) {
                string tipoCuenta = cuenta switch {
                    CuentaOro => "Oro",
                    CuentaPlata => "Plata", 
                    CuentaBronce => "Bronce",
                    _ => "Desconocida"
                };
                Console.WriteLine($"  - Cuenta {cuenta.Numero} ({tipoCuenta}) - Saldo: {cuenta.Saldo:C} - Puntos: {cuenta.Puntos:C}");
            }
        }

        public void Informe() {
            double saldoTotal = 0.0;
            double puntosTotal = 0.0;

            foreach (var cuenta in cuentas) {
                saldoTotal += cuenta.Saldo;
                puntosTotal += cuenta.Puntos;
            }

            Console.WriteLine($"  Cliente: {nombre} | Saldo Total: {saldoTotal:C} | Puntos Total: {puntosTotal:C}");
            Console.WriteLine();
            foreach (var cuenta in cuentas) {
                cuenta.Informe();
            }
            
            if (historialOperaciones.Length > 0) {
                Console.WriteLine($"    Historial de Operaciones de {nombre}:");
                foreach (var operacion in historialOperaciones) {
                    var cuenta = BuscarCuenta(operacion.GetNumeroCuenta());
                    if (cuenta != null) {
                        operacion.Informe(cuenta);
                    }
                }
            }
        }
    }

    
    public abstract class Cuenta {
        protected string numero;
        private Cliente? titular; 
        private double saldo;
        private double puntos;
        private Operacion[] operaciones;

        public string Numero => numero;
        public Cliente? Titular => titular;
        public double Saldo => saldo;
        public double Puntos => puntos;

        public Cuenta(string numero, double saldoInicial = 0.0) {
            this.numero = numero;
            saldo = saldoInicial;
            puntos = 0;
            operaciones = new Operacion[0];
        }

        public void AsignarTitular(Cliente cliente) {
            titular = cliente;
        }

        public abstract double ObtenerPuntos(double monto);

        public double Depositar(double monto) {
            saldo += monto;
            return monto;
        }

        public double Retirar(double monto) {
            monto = Math.Min(monto, saldo);
            saldo -= monto;
            return monto;
        }

        public double Pagar(double monto) {
            monto = Retirar(monto);
            puntos += ObtenerPuntos(monto);
            return monto;
        }

        public Operacion RegistrarOperacion(Operacion operacion) {
            Array.Resize(ref operaciones, operaciones.Length + 1);
            operaciones[operaciones.Length - 1] = operacion;
            return operacion;
        }

        public void Informe() {
            Console.WriteLine($"    Cuenta: {numero} | Saldo: {saldo:C} | Puntos: {puntos:C}");
            foreach (var operacion in operaciones) {
                operacion.Informe(this);
            }
        }
    }

    public class CuentaOro : Cuenta {
        public CuentaOro(string numero, double saldoInicial = 0.0) : base(numero, saldoInicial) { }

        override public double ObtenerPuntos(double monto) {
            return monto * (monto >= 1000 ? 0.05 : 0.03);
        }
    }

    public class CuentaPlata : Cuenta {
        public CuentaPlata(string numero, double saldoInicial = 0.0) : base(numero, saldoInicial) { }

        override public double ObtenerPuntos(double monto) {
            return monto * 0.02;
        }
    }

    public class CuentaBronce : Cuenta {
        public CuentaBronce(string numero, double saldoInicial = 0.0) : base(numero, saldoInicial) { }

        override public double ObtenerPuntos(double monto) {
            return monto * 0.01;
        }
    }

    public abstract class Operacion {
        protected string numeroCuenta;
        protected double monto;

        protected Operacion(string numeroCuenta, double monto) {
            this.numeroCuenta = numeroCuenta;
            this.monto = monto;
        }

        public string GetNumeroCuenta() {
            return numeroCuenta;
        }

        public double GetMonto() {
            return monto;
        }

        public abstract void Operar(Banco banco);
        public abstract void Informe(Cuenta cuenta);
        public abstract string GetTipoOperacion();
    }

    public class Deposito : Operacion {
        public Deposito(string numeroCuenta, double monto) : base(numeroCuenta, monto) { }

        public override void Operar(Banco banco) {
            var cuenta = banco.BuscarCuenta(numeroCuenta);
            if (cuenta != null) {
                if (banco.BuscarCuenta(cuenta) != null) {
                    cuenta.Depositar(monto);
                    cuenta.RegistrarOperacion(this);
                    cuenta.Titular?.RegistrarOperacion(this);
                } else {
                    Console.WriteLine($"Error: La cuenta {numeroCuenta} no pertenece a este banco");
                }
            } else {
                Console.WriteLine($"Error: Cuenta {numeroCuenta} no encontrada");
            }
        }

        public override void Informe(Cuenta cuenta) {
            Console.WriteLine($"     -  Deposito {monto:C} a [{cuenta.Numero}/{cuenta.Titular?.Nombre}]");
        }

        public override string GetTipoOperacion() {
            return "Deposito";
        }
    }

    public class Retiro : Operacion {
        public Retiro(string numeroCuenta, double monto) : base(numeroCuenta, monto) { }

        public override void Operar(Banco banco) {
            var cuenta = banco.BuscarCuenta(numeroCuenta);
            if (cuenta != null) {
                if (banco.BuscarCuenta(cuenta) != null) {
                    if (cuenta.Saldo >= monto) {
                        cuenta.Retirar(monto);
                        cuenta.RegistrarOperacion(this);
                        cuenta.Titular?.RegistrarOperacion(this);
                    } else {
                        Console.WriteLine($"Error: Fondos insuficientes en cuenta {numeroCuenta}. Saldo disponible: {cuenta.Saldo:C}");
                    }
                } else {
                    Console.WriteLine($"Error: La cuenta {numeroCuenta} no pertenece a este banco");
                }
            } else {
                Console.WriteLine($"Error: Cuenta {numeroCuenta} no encontrada");
            }
        }

        public override void Informe(Cuenta cuenta) {
            Console.WriteLine($"     -  Retiro {monto:C} de [{cuenta.Numero}/{cuenta.Titular?.Nombre}]");
        }

        public override string GetTipoOperacion() {
            return "Retiro";
        }
    }

    public class Pago : Operacion {
        public Pago(string numeroCuenta, double monto) : base(numeroCuenta, monto) { }

        public override void Operar(Banco banco) {
            var cuenta = banco.BuscarCuenta(numeroCuenta);
            if (cuenta != null) {
                if (banco.BuscarCuenta(cuenta) != null) {
                    if (cuenta.Saldo >= monto) {
                        cuenta.Pagar(monto);
                        cuenta.RegistrarOperacion(this);
                        cuenta.Titular?.RegistrarOperacion(this);
                    } else {
                        Console.WriteLine($"Error: Fondos insuficientes para pago en cuenta {numeroCuenta}. Saldo disponible: {cuenta.Saldo:C}");
                    }
                } else {
                    Console.WriteLine($"Error: La cuenta {numeroCuenta} no pertenece a este banco");
                }
            } else {
                Console.WriteLine($"Error: Cuenta {numeroCuenta} no encontrada");
            }
        }

        public override void Informe(Cuenta cuenta) {
            Console.WriteLine($"     -  Pago {monto:C} con [{cuenta.Numero}/{cuenta.Titular?.Nombre}]");
        }

        public override string GetTipoOperacion() {
            return "Pago";
        }
    }

    public class Transferencia : Operacion {
        private string numeroDestino;
        private static Banco? bancoNac;
        private static Banco? bancoTup;

        public Transferencia(string numeroCuentaOrigen, string numeroCuentaDestino, double monto) : base(numeroCuentaOrigen, monto) {
            this.numeroDestino = numeroCuentaDestino;
        }

        public static void EstablecerBancos(Banco nac, Banco tup) {
            bancoNac = nac;
            bancoTup = tup;
        }

        public override void Operar(Banco banco) {
            var cuentaOrigen = banco.BuscarCuenta(numeroCuenta);
            
            if (cuentaOrigen == null) {
                Console.WriteLine($"Error: Cuenta origen {numeroCuenta} no encontrada");
                return;
            }
            
            if (banco.BuscarCuenta(cuentaOrigen) == null) {
                Console.WriteLine($"Error: La cuenta origen {numeroCuenta} no pertenece a este banco");
                return;
            }
            
            Cuenta? cuentaDestino = banco.BuscarCuenta(numeroDestino);
            
            if (cuentaDestino == null) {
                if (banco != bancoNac) cuentaDestino = bancoNac?.BuscarCuenta(numeroDestino);
                if (cuentaDestino == null && banco != bancoTup) cuentaDestino = bancoTup?.BuscarCuenta(numeroDestino);
            }
            
            if (cuentaDestino == null) {
                Console.WriteLine($"Error: Cuenta destino {numeroDestino} no encontrada");
                return;
            }
            
            if (cuentaOrigen.Saldo >= monto) {
                double montoRetirado = cuentaOrigen.Retirar(monto);
                cuentaDestino.Depositar(montoRetirado);
                
                cuentaOrigen.RegistrarOperacion(this);
                cuentaDestino.RegistrarOperacion(this);
                
                cuentaOrigen.Titular?.RegistrarOperacion(this);
                if (cuentaDestino.Titular != cuentaOrigen.Titular) {
                    cuentaDestino.Titular?.RegistrarOperacion(this);
                }
            } else {
                Console.WriteLine($"Error: Fondos insuficientes para transferencia desde cuenta {numeroCuenta}. Saldo disponible: {cuentaOrigen.Saldo:C}");
            }
        }

        public override void Informe(Cuenta cuenta) {
            if (cuenta.Numero == numeroCuenta) {
                var cuentaDestino = BuscarCuentaEnTodosBancos(numeroDestino);
                Console.WriteLine($"     -  Transferencia {monto:C} de [{cuenta.Numero}/{cuenta.Titular?.Nombre}] a [{numeroDestino}/{cuentaDestino?.Titular?.Nombre}]");
            } else {
                var cuentaOrigen = BuscarCuentaEnTodosBancos(numeroCuenta);
                Console.WriteLine($"     -  Transferencia {monto:C} de [{numeroCuenta}/{cuentaOrigen?.Titular?.Nombre}] a [{cuenta.Numero}/{cuenta.Titular?.Nombre}]");
            }
        }

        private Cuenta? BuscarCuentaEnTodosBancos(string numero) {
            return bancoNac?.BuscarCuenta(numero) ?? bancoTup?.BuscarCuenta(numero);
        }

        public override string GetTipoOperacion() {
            return "Transferencia";
        }
    }

    
    public class Banco {
        private string nombre;
        private Cliente[] clientes;
        private Operacion[] operacionesGlobales;

        public Banco(string nombre) {
            this.nombre = nombre;
            clientes = new Cliente[0];
            operacionesGlobales = new Operacion[0];
        }

        public Cliente Agregar(Cliente cliente) {
            Array.Resize(ref clientes, clientes.Length + 1);
            clientes[clientes.Length - 1] = cliente;
            return cliente;
        }

        public string GetNombre() {
            return nombre;
        }

        public Cliente[] GetClientes() {
            return clientes;
        }

        public void MostrarClientes() {
            Console.WriteLine("Clientes:");
            foreach (var cliente in clientes) {
                Console.WriteLine($"  - {cliente.Nombre}");
            }
        }

        public Cuenta? BuscarCuenta(string numero) {
            foreach (var cliente in clientes) {
                var cuenta = cliente.BuscarCuenta(numero);
                if (cuenta != null) return cuenta;
            }
            return null;
        }

        public Cuenta? BuscarCuenta(Cuenta cuenta) {
            foreach (var cliente in clientes) {
                var resultado = cliente.BuscarCuenta(cuenta);
                if (resultado != null) return resultado;
            }
            return null;
        }

        public Operacion Registrar(Operacion operacion) {
            Array.Resize(ref operacionesGlobales, operacionesGlobales.Length + 1);
            operacionesGlobales[operacionesGlobales.Length - 1] = operacion;
            
            operacion.Operar(this);
            return operacion;
        }

        public void Informe() {
            Console.WriteLine($"Banco: {nombre} | Clientes: {clientes.Length}");
            Console.WriteLine();
            foreach (var cliente in clientes) {
                cliente.Informe();
            }
            Console.WriteLine();
        }

        public void InformeGlobalOperaciones() {
            Console.WriteLine($"Informe Global de Operaciones - {nombre}");
            Console.WriteLine("Operación\tCuenta\t\tMonto\t\tTipo");
            Console.WriteLine("--------------------------------------------------------");
            foreach (var operacion in operacionesGlobales) {
                var cuenta = BuscarCuenta(operacion.GetNumeroCuenta());
                Console.WriteLine($"{operacion.GetTipoOperacion()}\t\t{operacion.GetNumeroCuenta()}\t\t{operacion.GetMonto():C}\t\t{cuenta?.Titular?.Nombre}");
            }
            Console.WriteLine();
        }
    }

    private Banco? bancoNacional;
    private Banco? bancoTup;

    public static void Main() {
        var sistema = new SistemaBancario();
        sistema.InicializarSistema();
        sistema.MenuPrincipal();
    }

    public void InicializarSistema() {
        var raul = new Cliente("Raul Perez");
        raul.Agregar(new CuentaOro("10001", 1000));
        raul.Agregar(new CuentaPlata("10002", 2000));

        var sara = new Cliente("Sara Lopez");
        sara.Agregar(new CuentaPlata("10003", 3000));
        sara.Agregar(new CuentaPlata("10004", 4000));

        var luis = new Cliente("Luis Gomez");
        luis.Agregar(new CuentaBronce("10005", 5000));

        bancoNacional = new Banco("Banco Nacional");
        bancoNacional.Agregar(raul);
        bancoNacional.Agregar(sara);

        bancoTup = new Banco("Banco TUP");
        bancoTup.Agregar(luis);

        Transferencia.EstablecerBancos(bancoNacional, bancoTup);
    }

    public void MenuPrincipal() {
        while (true) {
            Console.Clear();
            Console.WriteLine("=== SISTEMA BANCARIO ===");
            Console.WriteLine();
            Console.WriteLine("Seleccione un banco:");
            Console.WriteLine("1. Banco Nacional");
            Console.WriteLine("2. Banco TUP");
            Console.WriteLine("3. Ver informes completos");
            Console.WriteLine("4. Salir");
            Console.WriteLine();
            Console.Write("Opción: ");

            var opcion = Console.ReadLine();
            
            switch (opcion) {
                case "1":
                    MenuBanco(bancoNacional!);
                    break;
                case "2":
                    MenuBanco(bancoTup!);
                    break;
                case "3":
                    MostrarInformesCompletos();
                    break;
                case "4":
                    Console.WriteLine("¡Gracias por usar el sistema bancario!");
                    return;
                default:
                    Console.WriteLine("Opción inválida. Presione Enter para continuar...");
                    Console.ReadLine();
                    break;
            }
        }
    }

    public void MenuBanco(Banco banco) {
        while (true) {
            Console.Clear();
            Console.WriteLine($"=== {banco.GetNombre().ToUpper()} ===");
            Console.WriteLine();
            banco.MostrarClientes();
            Console.WriteLine();
            Console.WriteLine("Seleccione un cliente:");
            
            var clientes = banco.GetClientes();
            for (int i = 0; i < clientes.Length; i++) {
                Console.WriteLine($"{i + 1}. {clientes[i].Nombre}");
            }
            Console.WriteLine($"{clientes.Length + 1}. Volver al menú principal");
            Console.WriteLine();
            Console.Write("Opción: ");

            var opcion = Console.ReadLine();
            
            if (int.TryParse(opcion, out int indice)) {
                if (indice >= 1 && indice <= clientes.Length) {
                    MenuCliente(banco, clientes[indice - 1]);
                } else if (indice == clientes.Length + 1) {
                    return;
                }
            }
            
            if (opcion != (clientes.Length + 1).ToString()) {
                Console.WriteLine("Opción inválida. Presione Enter para continuar...");
                Console.ReadLine();
            }
        }
    }

    public void MenuCliente(Banco banco, Cliente cliente) {
        while (true) {
            Console.Clear();
            Console.WriteLine($"=== CLIENTE: {cliente.Nombre.ToUpper()} ===");
            Console.WriteLine($"Banco: {banco.GetNombre()}");
            Console.WriteLine();
            
            cliente.MostrarCuentas();
            
            Console.WriteLine();
            Console.WriteLine("Seleccione una operación:");
            Console.WriteLine("1. Depositar");
            Console.WriteLine("2. Extraer");
            Console.WriteLine("3. Pagar");
            Console.WriteLine("4. Transferir");
            Console.WriteLine("5. Ver historial de operaciones");
            Console.WriteLine("6. Volver");
            Console.WriteLine();
            Console.Write("Opción: ");

            var opcion = Console.ReadLine();
            
            switch (opcion) {
                case "1":
                    RealizarDeposito(banco, cliente);
                    break;
                case "2":
                    RealizarExtraccion(banco, cliente);
                    break;
                case "3":
                    RealizarPago(banco, cliente);
                    break;
                case "4":
                    RealizarTransferencia(banco, cliente);
                    break;
                case "5":
                    MostrarHistorialCliente(cliente);
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Presione Enter para continuar...");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private void RealizarDeposito(Banco banco, Cliente cliente) {
        Console.Clear();
        Console.WriteLine("=== DEPÓSITO ===");
        Console.WriteLine();
        
        var cuenta = SeleccionarCuenta(cliente);
        if (cuenta == null) return;

        Console.Write("Ingrese el monto a depositar: $");
        if (double.TryParse(Console.ReadLine(), out double monto) && monto > 0) {
            banco.Registrar(new Deposito(cuenta.Numero, monto));
            Console.WriteLine($"Depósito realizado exitosamente. Nuevo saldo: {cuenta.Saldo:C}");
        } else {
            Console.WriteLine("Monto inválido.");
        }
        
        Console.WriteLine("Presione Enter para continuar...");
        Console.ReadLine();
    }

    private void RealizarExtraccion(Banco banco, Cliente cliente) {
        Console.Clear();
        Console.WriteLine("=== EXTRACCIÓN ===");
        Console.WriteLine();
        
        var cuenta = SeleccionarCuenta(cliente);
        if (cuenta == null) return;

        Console.WriteLine($"Saldo disponible: {cuenta.Saldo:C}");
        Console.Write("Ingrese el monto a extraer: $");
        if (double.TryParse(Console.ReadLine(), out double monto) && monto > 0) {
            if (monto <= cuenta.Saldo) {
                banco.Registrar(new Retiro(cuenta.Numero, monto));
                Console.WriteLine($"Extracción realizada exitosamente. Nuevo saldo: {cuenta.Saldo:C}");
            } else {
                Console.WriteLine("Fondos insuficientes.");
            }
        } else {
            Console.WriteLine("Monto inválido.");
        }
        
        Console.WriteLine("Presione Enter para continuar...");
        Console.ReadLine();
    }

    private void RealizarPago(Banco banco, Cliente cliente) {
        Console.Clear();
        Console.WriteLine("=== PAGO ===");
        Console.WriteLine();
        
        var cuenta = SeleccionarCuenta(cliente);
        if (cuenta == null) return;

        Console.WriteLine($"Saldo disponible: {cuenta.Saldo:C}");
        Console.WriteLine($"Puntos actuales: {cuenta.Puntos:C}");
        Console.Write("Ingrese el monto a pagar: $");
        if (double.TryParse(Console.ReadLine(), out double monto) && monto > 0) {
            if (monto <= cuenta.Saldo) {
                banco.Registrar(new Pago(cuenta.Numero, monto));
                Console.WriteLine($"Pago realizado exitosamente.");
                Console.WriteLine($"Nuevo saldo: {cuenta.Saldo:C}");
                Console.WriteLine($"Nuevos puntos: {cuenta.Puntos:C}");
            } else {
                Console.WriteLine("Fondos insuficientes.");
            }
        } else {
            Console.WriteLine("Monto inválido.");
        }
        
        Console.WriteLine("Presione Enter para continuar...");
        Console.ReadLine();
    }

    private void RealizarTransferencia(Banco banco, Cliente cliente) {
        Console.Clear();
        Console.WriteLine("=== TRANSFERENCIA ===");
        Console.WriteLine();
        
        var cuentaOrigen = SeleccionarCuenta(cliente);
        if (cuentaOrigen == null) return;

        Console.WriteLine($"Saldo disponible: {cuentaOrigen.Saldo:C}");
        Console.Write("Ingrese el número de cuenta destino: ");
        var numeroDestino = Console.ReadLine();
        
        if (string.IsNullOrEmpty(numeroDestino)) {
            Console.WriteLine("Número de cuenta inválido.");
            Console.WriteLine("Presione Enter para continuar...");
            Console.ReadLine();
            return;
        }

        var cuentaDestino = bancoNacional?.BuscarCuenta(numeroDestino) ?? bancoTup?.BuscarCuenta(numeroDestino);
        
        if (cuentaDestino == null) {
            Console.WriteLine("Cuenta destino no encontrada.");
            Console.WriteLine("Presione Enter para continuar...");
            Console.ReadLine();
            return;
        }

        Console.WriteLine($"Cuenta destino: {numeroDestino} - {cuentaDestino.Titular?.Nombre}");
        Console.Write("Ingrese el monto a transferir: $");
        
        if (double.TryParse(Console.ReadLine(), out double monto) && monto > 0) {
            if (monto <= cuentaOrigen.Saldo) {
                banco.Registrar(new Transferencia(cuentaOrigen.Numero, numeroDestino, monto));
                Console.WriteLine($"Transferencia realizada exitosamente.");
                Console.WriteLine($"Nuevo saldo: {cuentaOrigen.Saldo:C}");
            } else {
                Console.WriteLine("Fondos insuficientes.");
            }
        } else {
            Console.WriteLine("Monto inválido.");
        }
        
        Console.WriteLine("Presione Enter para continuar...");
        Console.ReadLine();
    }

    private Cuenta? SeleccionarCuenta(Cliente cliente) {
        var cuentas = cliente.GetCuentas();
        if (cuentas.Length == 1) {
            return cuentas[0];
        }

        Console.WriteLine("Seleccione una cuenta:");
        for (int i = 0; i < cuentas.Length; i++) {
            Console.WriteLine($"{i + 1}. Cuenta {cuentas[i].Numero} - Saldo: {cuentas[i].Saldo:C}");
        }
        Console.Write("Opción: ");

        if (int.TryParse(Console.ReadLine(), out int indice) && indice >= 1 && indice <= cuentas.Length) {
            return cuentas[indice - 1];
        }

        Console.WriteLine("Selección inválida.");
        return null;
    }

    private void MostrarHistorialCliente(Cliente cliente) {
        Console.Clear();
        Console.WriteLine($"=== HISTORIAL DE {cliente.Nombre.ToUpper()} ===");
        Console.WriteLine();
        cliente.Informe();
        Console.WriteLine("Presione Enter para continuar...");
        Console.ReadLine();
    }

    private void MostrarInformesCompletos() {
        Console.Clear();
        Console.WriteLine("=== INFORMES COMPLETOS ===");
        Console.WriteLine();
        
        bancoNacional?.Informe();
        bancoTup?.Informe();
        
        Console.WriteLine("=== INFORMES GLOBALES DE OPERACIONES ===");
        Console.WriteLine();
        bancoNacional?.InformeGlobalOperaciones();
        bancoTup?.InformeGlobalOperaciones();
        
        Console.WriteLine("Presione Enter para continuar...");
        Console.ReadLine();
    }
}