using System;
using System.Collections.Generic;

public class Banco {
    public string Nombre { get; set; }
    private List<Cliente> Clientes = new List<Cliente>();
    private List<Operacion> Operaciones = new List<Operacion>();

    private const string ArchivoClientes = "clientes.txt";
    private static int contadorCuentas = 10000; // Contador global para números de cuenta

    public Banco(string nombre) {
        Nombre = nombre;
        CargarClientes();
    }

    public static string GenerarNumeroCuenta() {
        return (contadorCuentas++).ToString();
    }

    public void GuardarClientes() {
        var lineas = new List<string>();
        foreach (var cliente in Clientes) {
            lineas.Add($"Cliente|{cliente.Nombre}");
            foreach (var cuenta in cliente.Cuentas) {
                lineas.Add($"Cuenta|{cuenta.NumeroCuenta}|{cuenta.Saldo}|{cuenta.Puntos}");
            }
        }
        System.IO.File.WriteAllLines(ArchivoClientes, lineas);
    }

    public void CargarClientes() {
        Clientes.Clear();
        if (!System.IO.File.Exists(ArchivoClientes)) return;

        Cliente clienteActual = null;
        foreach (var linea in System.IO.File.ReadAllLines(ArchivoClientes)) {
            var partes = linea.Split('|');
            if (partes[0] == "Cliente") {
                clienteActual = new Cliente(partes[1]);
                Clientes.Add(clienteActual);
            } else if (partes[0] == "Cuenta" && clienteActual != null) {
                var cuenta = new CuentaOro(partes[1], decimal.Parse(partes[2])) {
                    Puntos = decimal.Parse(partes[3])
                };
                clienteActual.Agregar(cuenta);

                // Actualizar el contador global según el número de cuenta más alto
                int numeroCuenta = int.Parse(partes[1]);
                if (numeroCuenta >= contadorCuentas) {
                    contadorCuentas = numeroCuenta + 1;
                }
            }
        }
    }

    public void Agregar(Cliente cliente) {
        Clientes.Add(cliente);
        GuardarClientes();
    }

    public IEnumerable<Cliente> ObtenerClientes() {
        return Clientes;
    }

    public void Registrar(Operacion operacion) {
        if (Operaciones.Exists(op => op == operacion)) {
            return;
        }

        try {
            operacion.Ejecutar();
            // Solo asignar un mensaje genérico si no hay un mensaje personalizado
            if (string.IsNullOrEmpty(operacion.Error) && string.IsNullOrEmpty(operacion.Mensaje)) {
                operacion.Mensaje = $"Operación ({operacion.GetType().Name}) exitosa.";
            }
        } catch (ArgumentException ex) {
            operacion.Error = $"ERROR: Operación ({operacion.GetType().Name}) fallida. {ex.Message} en la cuenta {operacion.Cuenta?.NumeroCuenta} del cliente {operacion.Cuenta?.Cliente?.Nombre}.";
        } catch (NullReferenceException) {
            operacion.Error = $"ERROR: Operación ({operacion.GetType().Name}) fallida. El cliente o la cuenta no existe.";
        } finally {
            Operaciones.Add(operacion);
        }
    }

    public void Informe() {
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"\nBanco: {Nombre} | Clientes: {Clientes.Count}");
        Console.WriteLine(new string('=', 70));
        foreach (var cliente in Clientes) {
            cliente.Resumen();
        }
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("\nDetalle global de operaciones:");
        Console.WriteLine(new string('=', 70));
        foreach (var operacion in Operaciones) {
            // Pasar el parámetro esDetalleGlobal = true
            Console.WriteLine($"- {operacion.Descripcion(esDetalleGlobal: true)}");
        }
        Console.WriteLine(new string('=', 70));
    }

    public void EliminarCliente(string nombreCliente) {
        var cliente = Clientes.Find(c => c.Nombre == nombreCliente);
        if (cliente != null) {
            Clientes.Remove(cliente);
            GuardarClientes(); // Guardar cambios permanentemente
            Console.WriteLine(new string('=', 70));
            Console.WriteLine($"Operación (Eliminar cliente) exitosa. Cliente '{nombreCliente}' eliminado.");

            // Registrar la eliminación como una operación
            Operaciones.Add(new EliminacionCliente(cliente));
        } else {
            Console.WriteLine($"Cliente '{nombreCliente}' no encontrado.");
        }
    }

    public void EliminarCuenta(string numeroCuenta) {
        foreach (var cliente in Clientes) {
            var cuenta = cliente.Cuentas.Find(c => c.NumeroCuenta == numeroCuenta);
            if (cuenta != null) {
                cliente.Cuentas.Remove(cuenta);
                GuardarClientes(); // Guardar cambios permanentemente
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"Operación (Eliminar cuenta) exitosa. Cuenta '{numeroCuenta}' eliminada.");

                // Registrar la eliminación como una operación
                Operaciones.Add(new EliminacionCuenta(cuenta, cliente));
                return;
            }
        }
        Console.WriteLine($"Cuenta '{numeroCuenta}' no encontrada.");
    }
}

public class Cliente {
    public string Nombre { get; set; }
    public List<Cuenta> Cuentas { get; set; } = new List<Cuenta>();

    public Cliente(string nombre) {
        Nombre = nombre;
    }

    public void Agregar(Cuenta cuenta) {
        if (!Cuentas.Contains(cuenta)) {
            Cuentas.Add(cuenta);
            cuenta.Cliente = this;
        }
    }

    public Cuenta ObtenerCuentaPorIndice(int indice) {
        if (indice < 0 || indice >= Cuentas.Count) {
            throw new ArgumentOutOfRangeException(nameof(indice), $"El cliente {Nombre} no tiene una cuenta en el índice {indice}.");
        }
        return Cuentas[indice];
    }

    public IEnumerable<Cuenta> ObtenerCuentas() {
        return Cuentas;
    }

    public void Resumen() {
        Console.WriteLine($"\nCliente : {Nombre}");
        Console.WriteLine(new string('=', 70));
        decimal totalSaldo = 0;
        decimal totalPuntos = 0;
        int totalOperaciones = 0;

        foreach (var cuenta in Cuentas) {
            totalSaldo += cuenta.Saldo;
            totalPuntos += cuenta.Puntos;
            totalOperaciones += cuenta.Historia.Count;
            Console.WriteLine($"Cuenta : {cuenta.NumeroCuenta}  | Tipo : {cuenta.TipoCuenta}  | Saldo : {cuenta.Saldo:C}  |  Puntos : {cuenta.Puntos:C}");
            foreach (var operacion in cuenta.Historia) {
                if (!string.IsNullOrEmpty(operacion.Error)) {
                    Console.WriteLine($"- {operacion.Error}");
                } else {
                    Console.WriteLine($"- {operacion.Descripcion()}");
                }
            }
            Console.WriteLine(new string('=', 70));
        }

        Console.WriteLine($"Total Saldo: {totalSaldo:C} | Total Puntos: {totalPuntos:C} | Total Operaciones: {totalOperaciones}");
    }
}

public abstract class Cuenta {
    public string NumeroCuenta { get; set; }
    public decimal Saldo { get; set; }
    public decimal Puntos { get; set; }
    public Cliente Cliente { get; set; }
    public List<Operacion> Historia { get; set; } = new List<Operacion>();

    public Cuenta(string numeroCuenta, decimal saldo) {
        NumeroCuenta = numeroCuenta;
        Saldo = saldo;
    }

    public virtual void Depositar(decimal monto) {
        Saldo += monto;
    }

    public virtual bool Retirar(decimal monto) {
        
        if (Saldo >= monto) {
            Saldo -= monto;
            
            return true;
        }
        Console.WriteLine($"[DEBUG] Retiro fallido. Fondos insuficientes en la cuenta {NumeroCuenta}");
        return false;
    }

    public abstract void AcumularPuntos(decimal monto);

    public void RegistrarOperacion(Operacion operacion) {
        if (!Historia.Contains(operacion)) {
            Historia.Add(operacion);
        }
    }

    public virtual void Resumen() {
        Console.WriteLine($"Cuenta : {NumeroCuenta}  | Saldo : {Saldo:C}  |  Puntos : {Puntos:C}");
        foreach (var operacion in Historia) {
            if (!string.IsNullOrEmpty(operacion.Error)) {
                Console.WriteLine($"- {operacion.Error}");
            } else {
                Console.WriteLine($"- {operacion.Descripcion()}");
            }
        }
    }

    public virtual string TipoCuenta => "Desconocido";
}

class CuentaOro : Cuenta {
    public CuentaOro(string numeroCuenta, decimal saldo) : base(numeroCuenta, saldo) { }

    public override string TipoCuenta => "Oro";

    public override void AcumularPuntos(decimal monto) {
        Puntos += monto > 1000 ? monto * 0.05m : monto * 0.03m;
    }
}

class CuentaPlata : Cuenta {
    public CuentaPlata(string numeroCuenta, decimal saldo) : base(numeroCuenta, saldo) { }

    public override string TipoCuenta => "Plata";

    public override void AcumularPuntos(decimal monto) {
        Puntos += monto * 0.02m;
    }
}

class CuentaBronce : Cuenta {
    public CuentaBronce(string numeroCuenta, decimal saldo) : base(numeroCuenta, saldo) { }

    public override string TipoCuenta => "Bronce";

    public override void AcumularPuntos(decimal monto) {
        Puntos += monto * 0.01m;
    }
}

public abstract class Operacion {
    public decimal Monto { get; set; }
    public Cuenta Cuenta { get; set; }
    public string Error { get; set; } // Exclusivo para errores
    public string Mensaje { get; set; } // Nuevo campo para mensajes de éxito

    public Operacion(decimal monto, Cuenta cuenta) {
        Monto = monto;
        Cuenta = cuenta;
    }

    public virtual void Ejecutar() {
        if (Monto <= 0) {
            Error = $"ERROR: Operación ({this.GetType().Name}) fallida. El monto debe ser mayor a 0 en la cuenta {Cuenta?.NumeroCuenta}.";
            Cuenta?.RegistrarOperacion(this);
            return;
        }
    }

    // Método abstracto con el parámetro opcional
    public abstract string Descripcion(bool esDetalleGlobal = false);
}

public class Deposito : Operacion {
    public Deposito(decimal monto, Cuenta cuenta) : base(monto, cuenta) { }

    public override void Ejecutar() {
        base.Ejecutar();
        if (!string.IsNullOrEmpty(Error)) return;

        Cuenta.Depositar(Monto);
        Cuenta.RegistrarOperacion(this);

        // Registrar el mensaje de éxito
        Mensaje = $"Operacion (Deposito) exitosa.  | Monto: {Monto:C}  |  Cuenta: {Cuenta.NumeroCuenta}.";
    }

    public override string Descripcion(bool esDetalleGlobal = false) {
        if (!string.IsNullOrEmpty(Error)) {
            return Error;
        }

        // Formato para el detalle global de operaciones
        if (esDetalleGlobal) {
            return $"Operacion (Deposito) exitosa.  |  Monto: {Monto:C}.  |  Cliente: {Cuenta.Cliente.Nombre}.  |  Cuenta: {Cuenta.NumeroCuenta}.";
        }

        // Formato para el detalle del cliente
        return $"Operacion (Deposito) exitosa.  |  Monto: {Monto:C}.";
    }
}

public class Retiro : Operacion {
    public Retiro(decimal monto, Cuenta cuenta) : base(monto, cuenta) { }

    public override void Ejecutar() {
        if (!Cuenta.Retirar(Monto)) {
            Error = $"ERROR: Operación (Retiro) fallida. Fondos insuficientes en la cuenta {Cuenta.NumeroCuenta}.";
            Cuenta.RegistrarOperacion(this);
            return;
        }
        Cuenta.RegistrarOperacion(this);
    }

    public override string Descripcion(bool esDetalleGlobal = false) {
        if (!string.IsNullOrEmpty(Error)) {
            return Error;
        }

        if (esDetalleGlobal) {
            return $"Operacion (Retiro) exitosa.  |  Monto: {Monto:C}.  |  Cliente: {Cuenta.Cliente.Nombre}.  |  Cuenta: {Cuenta.NumeroCuenta}.";
        }

        return $"Operacion (Retiro) exitosa.  |  Monto: {Monto:C}.";
    }
}

public class Pago : Operacion {
    public Pago(decimal monto, Cuenta cuenta) : base(monto, cuenta) { }

    public override void Ejecutar() {
        if (!Cuenta.Retirar(Monto)) {
            Error = $"ERROR: Operación (Pago) fallida. Fondos insuficientes en la cuenta {Cuenta.NumeroCuenta}.";
            Cuenta.RegistrarOperacion(this);
            return;
        }
        Cuenta.AcumularPuntos(Monto);
        Cuenta.RegistrarOperacion(this);
    }

    public override string Descripcion(bool esDetalleGlobal = false) {
        if (!string.IsNullOrEmpty(Error)) {
            return Error;
        }

        if (esDetalleGlobal) {
            return $"Operacion (Pago) exitosa.  |  Monto: {Monto:C}.  |  Cliente: {Cuenta.Cliente.Nombre}.  |  Cuenta: {Cuenta.NumeroCuenta}.";
        }

        return $"Operacion (Pago) exitosa.  |  Monto: {Monto:C}.";
    }
}

public class Transferencia : Operacion {
    public Cuenta CuentaDestino { get; set; }

    public Transferencia(decimal monto, Cuenta cuentaOrigen, Cuenta cuentaDestino) : base(monto, cuentaOrigen) {
        CuentaDestino = cuentaDestino;
    }

    public override void Ejecutar() {
        if (!Cuenta.Retirar(Monto)) {
            Error = $"ERROR: Operación (Transferencia) fallida. Fondos insuficientes en la cuenta {Cuenta.NumeroCuenta}.";
            Cuenta.RegistrarOperacion(this);
            return;
        }
        CuentaDestino.Depositar(Monto);
        Cuenta.RegistrarOperacion(this);
        CuentaDestino.RegistrarOperacion(this);
    }

    public override string Descripcion(bool esDetalleGlobal = false) {
        if (!string.IsNullOrEmpty(Error)) {
            return Error;
        }

        if (esDetalleGlobal) {
            return $"Operacion (Transferencia) exitosa.  |  Monto: {Monto:C}.  |  Cliente: {Cuenta.Cliente.Nombre}.  |  Origen: {Cuenta.NumeroCuenta}.  |  Destino: {CuentaDestino.NumeroCuenta}.";
        }

        return $"Transferencia de {Monto:C} a la cuenta {CuentaDestino.NumeroCuenta}.";
    }
}

// Nueva clase para registrar la eliminación de un cliente
public class EliminacionCliente : Operacion {
    public EliminacionCliente(Cliente cliente) : base(0, null) {
        Mensaje = $"Cliente '{cliente.Nombre}' eliminado del sistema.";
    }

    public override string Descripcion(bool esDetalleGlobal = false) {
        return Mensaje;
    }
}

// Nueva clase para registrar la eliminación de una cuenta
public class EliminacionCuenta : Operacion {
    public EliminacionCuenta(Cuenta cuenta, Cliente cliente) : base(0, cuenta) {
        Mensaje = $"Cuenta '{cuenta.NumeroCuenta}' del cliente '{cliente.Nombre}' eliminada del sistema.";
    }

    public override string Descripcion(bool esDetalleGlobal = false) {
        return Mensaje;
    }
}

// Clase para registrar la creación de un cliente
public class CreacionCliente : Operacion {
    public CreacionCliente(Cliente cliente) : base(0, null) {
        if (cliente == null) {
            Error = "ERROR: Operación (CreacionCliente) fallida. El cliente no puede ser nulo.";
        } else {
            Mensaje = $"Operación (CreacionCliente) exitosa.  |  Cliente: {cliente.Nombre}";
        }
    }

    public override void Ejecutar() {
        // No se realiza ninguna validación de monto aquí
    }

    public override string Descripcion(bool esDetalleGlobal = false) {
        return !string.IsNullOrEmpty(Error) ? Error : Mensaje;
    }
}

// Clase para registrar la creación de una cuenta
public class CreacionCuenta : Operacion {
    public CreacionCuenta(Cuenta cuenta, Cliente cliente) : base(0, cuenta) {
        if (cuenta == null || cliente == null) {
            Error = "ERROR: Operación (CreacionCuenta) fallida. Intente nuevamente.";
        } else {
            Mensaje = $"Operación (CreacionCuenta) exitosa.  |  Cliente: {cliente.Nombre}  |  Tipo: {cuenta.TipoCuenta}  |  Cuenta: {cuenta.NumeroCuenta}";
        }
    }

    public override void Ejecutar() {
        // No se realiza ninguna validación de monto aquí
    }

    public override string Descripcion(bool esDetalleGlobal = false) {
        return !string.IsNullOrEmpty(Error) ? Error : Mensaje;
    }
}

Console.WriteLine(new string('=', 70));
Console.WriteLine("Iniciando el programa...");
Console.WriteLine("Presione cualquier tecla para continuar...");
Console.ReadKey();

Banco banco = new Banco("Banco Central");

while (true) {
    Console.Clear();
    Console.WriteLine("==========================");
    Console.WriteLine("    BANCO CENTRAL");
    Console.WriteLine("==========================\n");
    Console.WriteLine("Seleccione una opción:");
    Console.WriteLine("1. Agregar cliente");
    Console.WriteLine("2. Crear nueva cuenta");
    Console.WriteLine("3. Realizar depósito");
    Console.WriteLine("4. Realizar retiro");
    Console.WriteLine("5. Realizar pago");
    Console.WriteLine("6. Realizar transferencia");
    Console.WriteLine("7. Mostrar informe del banco");
    Console.WriteLine("8. Eliminar cliente o cuenta");
    Console.WriteLine("9. Salir\n");
    Console.Write("Ingrese su opción: ");

    string opcion = Console.ReadLine();
    switch (opcion) {
        case "1":
            Console.Clear();
            string nombreCliente;

            // Validar el nombre del cliente
            while (true) {
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el nombre del cliente: ");
                nombreCliente = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nombreCliente) && System.Text.RegularExpressions.Regex.IsMatch(nombreCliente, @"^[a-zA-Z\s]+$")) {
                    break; // Nombre válido, salir del bucle
                }

                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Error: El nombre solo puede contener letras y no puede estar vacío. Intente nuevamente.");
            }

            // Seleccionar el tipo de cuenta
            string tipoCuentaSeleccionada;
            while (true) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Seleccione el tipo de cuenta para el cliente:");
                Console.WriteLine("1. Cuenta Oro");
                Console.WriteLine("2. Cuenta Plata");
                Console.WriteLine("3. Cuenta Bronce");
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese su opción: ");
                tipoCuentaSeleccionada = Console.ReadLine();

                if (tipoCuentaSeleccionada == "1" || tipoCuentaSeleccionada == "2" || tipoCuentaSeleccionada == "3") {
                    break; // Opción válida, salir del bucle
                }

                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Error: Opción inválida. Intente nuevamente.");
            }

            // Crear la cuenta según la opción seleccionada
            string numeroCuenta = Banco.GenerarNumeroCuenta();
            Cuenta cuentaNueva = tipoCuentaSeleccionada switch {
                "1" => new CuentaOro(numeroCuenta, 0),
                "2" => new CuentaPlata(numeroCuenta, 0),
                "3" => new CuentaBronce(numeroCuenta, 0),
                _ => null
            };

            // Crear el cliente y asociar la cuenta
            Cliente nuevoCliente = new Cliente(nombreCliente);
            nuevoCliente.Agregar(cuentaNueva);
            banco.Agregar(nuevoCliente);

            // Registrar la creación del cliente y la cuenta
            banco.Registrar(new CreacionCliente(nuevoCliente));
            banco.Registrar(new CreacionCuenta(cuentaNueva, nuevoCliente));

            // Mostrar el mensaje de éxito
            Console.WriteLine(new string('=', 70));
            Console.WriteLine($"Cliente {nombreCliente} creado con éxito.  |  Tipo: {cuentaNueva.TipoCuenta}  |  Cuenta: {numeroCuenta}");
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
        case "2":
            Console.Clear();
            if (!banco.ObtenerClientes().GetEnumerator().MoveNext()) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("No hay clientes registrados. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }

            // Seleccionar cliente
            Cliente clienteSeleccionadoCuenta;
            while (true) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Seleccione el cliente para crear una nueva cuenta:");
                var clientesParaCuenta = new List<Cliente>(banco.ObtenerClientes());
                for (int i = 0; i < clientesParaCuenta.Count; i++) {
                    Console.WriteLine($"{i + 1}. {clientesParaCuenta[i].Nombre}");
                }
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el número del cliente: ");
                if (int.TryParse(Console.ReadLine(), out int clienteIndexCuenta) && clienteIndexCuenta >= 1 && clienteIndexCuenta <= clientesParaCuenta.Count) {
                    clienteSeleccionadoCuenta = clientesParaCuenta[clienteIndexCuenta - 1];
                    break; // Cliente válido, salir del bucle
                }

                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Error: Selección inválida. Intente nuevamente.");
            }

            // Seleccionar tipo de cuenta
            string tipoCuentaNueva;
            while (true) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Seleccione el tipo de cuenta:");
                Console.WriteLine("1. Cuenta Oro");
                Console.WriteLine("2. Cuenta Plata");
                Console.WriteLine("3. Cuenta Bronce");
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese su opción: ");
                tipoCuentaNueva = Console.ReadLine();

                if (tipoCuentaNueva == "1" || tipoCuentaNueva == "2" || tipoCuentaNueva == "3") {
                    break; // Opción válida, salir del bucle
                }

                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Error: Opción inválida. Intente nuevamente.");
            }

            // Crear la cuenta según la opción seleccionada
            string nuevoNumeroCuenta = Banco.GenerarNumeroCuenta();
            Cuenta cuentaAdicional = tipoCuentaNueva switch { // Cambiar nombre de variable
                "1" => new CuentaOro(nuevoNumeroCuenta, 0),
                "2" => new CuentaPlata(nuevoNumeroCuenta, 0),
                "3" => new CuentaBronce(nuevoNumeroCuenta, 0),
                _ => null
            };

            // Asociar la cuenta al cliente
            clienteSeleccionadoCuenta.Agregar(cuentaAdicional); // Usar el nuevo nombre
            banco.GuardarClientes();

            // Registrar la creación de la nueva cuenta
            banco.Registrar(new CreacionCuenta(cuentaAdicional, clienteSeleccionadoCuenta)); // Usar el nuevo nombre

            // Mostrar el mensaje de éxito
            Console.WriteLine(new string('=', 70));
            Console.WriteLine($"Nueva cuenta creada exitosamente. Tipo: {cuentaAdicional.TipoCuenta} | Número de cuenta: {nuevoNumeroCuenta}"); // Usar el nuevo nombre
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
        case "3":
            Console.Clear();
            if (!banco.ObtenerClientes().GetEnumerator().MoveNext()) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("No hay clientes registrados. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }
            
            // 1. Seleccionar cliente con reintentos
            Cliente clienteSeleccionado = null;
            while (clienteSeleccionado == null) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Seleccione el cliente para realizar el depósito:");
                var clientes = new List<Cliente>(banco.ObtenerClientes());
                for (int i = 0; i < clientes.Count; i++) {
                    Console.WriteLine($"{i + 1}. {clientes[i].Nombre}");
                }
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el número del cliente: ");
                if (int.TryParse(Console.ReadLine(), out int clienteIndex) && clienteIndex >= 1 && clienteIndex <= clientes.Count) {
                    clienteSeleccionado = clientes[clienteIndex - 1];
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                }
            }
            
            // Verificar si el cliente tiene cuentas
            var cuentas = new List<Cuenta>(clienteSeleccionado.ObtenerCuentas());
            if (cuentas.Count == 0) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"El cliente {clienteSeleccionado.Nombre} no tiene cuentas registradas. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }
            
            // 2. Seleccionar cuenta con reintentos
            Cuenta cuentaSeleccionada = null;
            while (cuentaSeleccionada == null) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"Seleccione la cuenta del cliente {clienteSeleccionado.Nombre}:");
                for (int i = 0; i < cuentas.Count; i++) {
                    Console.WriteLine($"{i + 1}. Cuenta {cuentas[i].NumeroCuenta} | Saldo: {cuentas[i].Saldo:C}");
                }
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el número de la cuenta: ");
                if (int.TryParse(Console.ReadLine(), out int cuentaIndex) && cuentaIndex >= 1 && cuentaIndex <= cuentas.Count) {
                    cuentaSeleccionada = cuentas[cuentaIndex - 1];
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                }
            }
            
            // Resto del código para el depósito
            decimal montoDeposito;
            while (true) {
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el monto a depositar: ");
                if (!decimal.TryParse(Console.ReadLine(), out montoDeposito) || montoDeposito <= 0) {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Operación (Depósito) fallida. No se puede operar con valores negativos o indebidos. Intente nuevamente.");
                } else {
                    break; // Salir del bucle si el monto es válido
                }
            }
            
            // Realizar el depósito
            Deposito deposito = new Deposito(montoDeposito, cuentaSeleccionada);
            banco.Registrar(deposito);
            banco.GuardarClientes();
            Console.WriteLine(new string('=', 70));
            Console.WriteLine($"Depósito realizado exitosamente. Nuevo saldo: {cuentaSeleccionada.Saldo:C}");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
        case "4":
            Console.Clear();
            if (!banco.ObtenerClientes().GetEnumerator().MoveNext()) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("No hay clientes registrados. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }
            
            // 1. Seleccionar cliente con reintentos
            Cliente clienteSeleccionadoRetiro = null;
            while (clienteSeleccionadoRetiro == null) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Seleccione el cliente para realizar el retiro:");
                var clientesRetiro = new List<Cliente>(banco.ObtenerClientes());
                for (int i = 0; i < clientesRetiro.Count; i++) {
                    Console.WriteLine($"{i + 1}. {clientesRetiro[i].Nombre}");
                }
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el número del cliente: ");
                if (int.TryParse(Console.ReadLine(), out int clienteIndexRetiro) && clienteIndexRetiro >= 1 && clienteIndexRetiro <= clientesRetiro.Count) {
                    clienteSeleccionadoRetiro = clientesRetiro[clienteIndexRetiro - 1];
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                }
            }
            
            // Verificar si el cliente tiene cuentas
            var cuentasRetiro = new List<Cuenta>(clienteSeleccionadoRetiro.ObtenerCuentas());
            if (cuentasRetiro.Count == 0) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"El cliente {clienteSeleccionadoRetiro.Nombre} no tiene cuentas registradas. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }
            
            // 2. Seleccionar cuenta con reintentos
            Cuenta cuentaSeleccionadaRetiro = null;
            while (cuentaSeleccionadaRetiro == null) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"Seleccione la cuenta del cliente {clienteSeleccionadoRetiro.Nombre}:");
                for (int i = 0; i < cuentasRetiro.Count; i++) {
                    Console.WriteLine($"{i + 1}. Cuenta {cuentasRetiro[i].NumeroCuenta} | Saldo: {cuentasRetiro[i].Saldo:C}");
                }
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el número de la cuenta: ");
                if (int.TryParse(Console.ReadLine(), out int cuentaIndexRetiro) && cuentaIndexRetiro >= 1 && cuentaIndexRetiro <= cuentasRetiro.Count) {
                    cuentaSeleccionadaRetiro = cuentasRetiro[cuentaIndexRetiro - 1];
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                }
            }
            
            // Solicitar monto de retiro con validación
            decimal montoRetiro;
            bool retiroExitoso = false;
            
            while (!retiroExitoso) {
                while (true) {
                    Console.WriteLine(new string('=', 70));
                    Console.Write("Ingrese el monto a retirar: ");
                    if (!decimal.TryParse(Console.ReadLine(), out montoRetiro) || montoRetiro <= 0) {
                        Console.WriteLine(new string('=', 70));
                        Console.WriteLine("Operación (Retiro) fallida. No se puede operar con valores negativos o indebidos. Intente nuevamente.");
                    } else {
                        break; // Salir del bucle si el monto es válido
                    }
                }
                
                // Verificar si hay fondos suficientes
                if (montoRetiro > cuentaSeleccionadaRetiro.Saldo) {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine($"Operación (Retiro) fallida. Fondos insuficientes en la cuenta {cuentaSeleccionadaRetiro.NumeroCuenta}.");
                    Console.WriteLine($"Saldo disponible: {cuentaSeleccionadaRetiro.Saldo:C}");
                    Console.WriteLine("Intente con un monto menor.");
                } else {
                    // Realizar el retiro si hay fondos suficientes
                    Retiro retiro = new Retiro(montoRetiro, cuentaSeleccionadaRetiro);
                    banco.Registrar(retiro);
                    banco.GuardarClientes();
                    retiroExitoso = true;
                    
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine($"Retiro realizado exitosamente. Nuevo saldo: {cuentaSeleccionadaRetiro.Saldo:C}");
                }
            }
            
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
        case "5":
            Console.Clear();
            if (!banco.ObtenerClientes().GetEnumerator().MoveNext()) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("No hay clientes registrados. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }
            
            // 1. Seleccionar cliente con reintentos
            Cliente clienteSeleccionadoPago = null;
            while (clienteSeleccionadoPago == null) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Seleccione el cliente para realizar el pago:");
                var clientesPago = new List<Cliente>(banco.ObtenerClientes());
                for (int i = 0; i < clientesPago.Count; i++) {
                    Console.WriteLine($"{i + 1}. {clientesPago[i].Nombre}");
                }
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el número del cliente: ");
                if (int.TryParse(Console.ReadLine(), out int clienteIndexPago) && clienteIndexPago >= 1 && clienteIndexPago <= clientesPago.Count) {
                    clienteSeleccionadoPago = clientesPago[clienteIndexPago - 1];
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                }
            }
            
            // Verificar si el cliente tiene cuentas
            var cuentasPago = new List<Cuenta>(clienteSeleccionadoPago.ObtenerCuentas());
            if (cuentasPago.Count == 0) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"El cliente {clienteSeleccionadoPago.Nombre} no tiene cuentas registradas. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }
            
            // 2. Seleccionar cuenta con reintentos
            Cuenta cuentaSeleccionadaPago = null;
            while (cuentaSeleccionadaPago == null) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"Seleccione la cuenta del cliente {clienteSeleccionadoPago.Nombre}:");
                for (int i = 0; i < cuentasPago.Count; i++) {
                    Console.WriteLine($"{i + 1}. Cuenta {cuentasPago[i].NumeroCuenta}  |  Tipo de Cuenta: {cuentasPago[i].TipoCuenta}  |  Saldo: {cuentasPago[i].Saldo:C}");
                }
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el número de la cuenta: ");
                if (int.TryParse(Console.ReadLine(), out int cuentaIndexPago) && cuentaIndexPago >= 1 && cuentaIndexPago <= cuentasPago.Count) {
                    cuentaSeleccionadaPago = cuentasPago[cuentaIndexPago - 1];
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                }
            }
            
            // Solicitar y validar el monto con verificación de fondos
            decimal montoPago;
            bool pagoExitoso = false;
            
            while (!pagoExitoso) {
                while (true) {
                    Console.WriteLine(new string('=', 70));
                    Console.Write("Ingrese el monto a pagar: ");
                    if (!decimal.TryParse(Console.ReadLine(), out montoPago) || montoPago <= 0) {
                        Console.WriteLine(new string('=', 70));
                        Console.WriteLine("Operación (Pago) fallida. No se puede operar con valores negativos o indebidos. Intente nuevamente.");
                    } else {
                        break; // Salir del bucle si el monto es válido
                    }
                }
                
                // Verificar si hay fondos suficientes
                if (montoPago > cuentaSeleccionadaPago.Saldo) {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine($"Operación (Pago) fallida. Fondos insuficientes en la cuenta {cuentaSeleccionadaPago.NumeroCuenta}.");
                    Console.WriteLine($"Saldo disponible: {cuentaSeleccionadaPago.Saldo:C}");
                    
                    // Verificar si el cliente tiene más cuentas para ofrecer cambiar
                    if (cuentasPago.Count > 1) {
                        Console.WriteLine(new string('=', 70));
                        Console.WriteLine("¿Qué desea hacer?");
                        Console.WriteLine("1. Intentar con un monto menor");
                        Console.WriteLine("2. Elegir otra cuenta");
                        Console.WriteLine(new string('=', 70));
                        Console.Write("Ingrese su opción: ");
                        
                        string opcionFondos = Console.ReadLine();
                        if (opcionFondos == "2") {
                            // Permitir seleccionar otra cuenta
                            cuentaSeleccionadaPago = null;
                            while (cuentaSeleccionadaPago == null) {
                                Console.WriteLine(new string('=', 70));
                                Console.WriteLine($"Seleccione otra cuenta del cliente {clienteSeleccionadoPago.Nombre}:");
                                for (int i = 0; i < cuentasPago.Count; i++) {
                                    Console.WriteLine($"{i + 1}. Cuenta {cuentasPago[i].NumeroCuenta}  |  Tipo de Cuenta: {cuentasPago[i].TipoCuenta}  |  Saldo: {cuentasPago[i].Saldo:C}");
                                }
                                Console.WriteLine(new string('=', 70));
                                Console.Write("Ingrese el número de la cuenta: ");
                                if (int.TryParse(Console.ReadLine(), out int cuentaIndexPago) && cuentaIndexPago >= 1 && cuentaIndexPago <= cuentasPago.Count) {
                                    cuentaSeleccionadaPago = cuentasPago[cuentaIndexPago - 1];
                                } else {
                                    Console.WriteLine(new string('=', 70));
                                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                                }
                            }
                            // Después de elegir otra cuenta, se vuelve a solicitar el monto
                            continue;
                        }
                        // Si se seleccionó opción 1 o cualquier otra, se intentará con un monto menor
                    } else {
                        Console.WriteLine("Intente con un monto menor.");
                    }
                } else {
                    // Realizar el pago si hay fondos suficientes
                    Pago pago = new Pago(montoPago, cuentaSeleccionadaPago);
                    banco.Registrar(pago);
                    banco.GuardarClientes();
                    pagoExitoso = true;
                    
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine($"Pago realizado exitosamente. Nuevo saldo: {cuentaSeleccionadaPago.Saldo:C} | Puntos acumulados: {cuentaSeleccionadaPago.Puntos:C}");
                }
            }
            
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
        case "6":
            Console.Clear();
            if (!banco.ObtenerClientes().GetEnumerator().MoveNext()) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("No hay clientes registrados. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }
            
            // 1. Seleccionar cliente origen con reintentos
            Cliente clienteOrigen = null;
            while (clienteOrigen == null) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Seleccione el cliente origen para realizar la transferencia:");
                var clientesOrigen = new List<Cliente>(banco.ObtenerClientes());
                for (int i = 0; i < clientesOrigen.Count; i++) {
                    Console.WriteLine($"{i + 1}. {clientesOrigen[i].Nombre}");
                }
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el número del cliente origen: ");
                if (int.TryParse(Console.ReadLine(), out int clienteIndexOrigen) && clienteIndexOrigen >= 1 && clienteIndexOrigen <= clientesOrigen.Count) {
                    clienteOrigen = clientesOrigen[clienteIndexOrigen - 1];
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                }
            }
            
            // Verificar si el cliente origen tiene cuentas
            var cuentasOrigen = new List<Cuenta>(clienteOrigen.ObtenerCuentas());
            if (cuentasOrigen.Count == 0) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"El cliente {clienteOrigen.Nombre} no tiene cuentas registradas. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }
            
            // 2. Seleccionar cuenta origen con reintentos
            Cuenta cuentaOrigen = null;
            while (cuentaOrigen == null) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"Seleccione la cuenta del cliente {clienteOrigen.Nombre}:");
                for (int i = 0; i < cuentasOrigen.Count; i++) {
                    Console.WriteLine($"{i + 1}. Cuenta {cuentasOrigen[i].NumeroCuenta} | Saldo: {cuentasOrigen[i].Saldo:C}");
                }
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el número de la cuenta origen: ");
                if (int.TryParse(Console.ReadLine(), out int cuentaIndexOrigen) && cuentaIndexOrigen >= 1 && cuentaIndexOrigen <= cuentasOrigen.Count) {
                    cuentaOrigen = cuentasOrigen[cuentaIndexOrigen - 1];
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                }
            }
            
            // 3. Seleccionar cliente destino con reintentos
            Cliente clienteDestino = null;
            while (clienteDestino == null) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("Seleccione el cliente destino para realizar la transferencia:");
                var clientesDestino = new List<Cliente>(banco.ObtenerClientes());
                for (int i = 0; i < clientesDestino.Count; i++) {
                    Console.WriteLine($"{i + 1}. {clientesDestino[i].Nombre}");
                }
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el número del cliente destino: ");
                if (int.TryParse(Console.ReadLine(), out int clienteIndexDestino) && clienteIndexDestino >= 1 && clienteIndexDestino <= clientesDestino.Count) {
                    clienteDestino = clientesDestino[clienteIndexDestino - 1];
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                }
            }
            
            // Verificar si el cliente destino tiene cuentas
            var cuentasDestino = new List<Cuenta>(clienteDestino.ObtenerCuentas());
            if (cuentasDestino.Count == 0) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"El cliente {clienteDestino.Nombre} no tiene cuentas registradas. Presione cualquier tecla para continuar...");
                Console.ReadKey();
                break;
            }
            
            // 4. Seleccionar cuenta destino con reintentos
            Cuenta cuentaDestino = null;
            while (cuentaDestino == null) {
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"Seleccione la cuenta del cliente {clienteDestino.Nombre}:");
                for (int i = 0; i < cuentasDestino.Count; i++) {
                    Console.WriteLine($"{i + 1}. Cuenta {cuentasDestino[i].NumeroCuenta} | Saldo: {cuentasDestino[i].Saldo:C}");
                }
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese el número de la cuenta destino: ");
                if (int.TryParse(Console.ReadLine(), out int cuentaIndexDestino) && cuentaIndexDestino >= 1 && cuentaIndexDestino <= cuentasDestino.Count) {
                    cuentaDestino = cuentasDestino[cuentaIndexDestino - 1];
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                }
            }
            
            // Solicitar monto de la transferencia y verificar fondos
            decimal montoTransferencia;
            bool transferenciaExitosa = false;
            
            while (!transferenciaExitosa) {
                while (true) {
                    Console.WriteLine(new string('=', 70));
                    Console.Write("Ingrese el monto a transferir: ");
                    if (!decimal.TryParse(Console.ReadLine(), out montoTransferencia) || montoTransferencia <= 0) {
                        Console.WriteLine(new string('=', 70));
                        Console.WriteLine("Operación (Transferencia) fallida. No se puede operar con valores negativos o indebidos. Intente nuevamente.");
                    } else {
                        break; // Salir del bucle si el monto es válido
                    }
                }
                
                // Verificar si hay fondos suficientes
                if (montoTransferencia > cuentaOrigen.Saldo) {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine($"Operación (Transferencia) fallida. Fondos insuficientes en la cuenta {cuentaOrigen.NumeroCuenta}.");
                    Console.WriteLine($"Saldo disponible: {cuentaOrigen.Saldo:C}");
                    
                    // Verificar si el cliente origen tiene más cuentas para ofrecer cambiar
                    if (cuentasOrigen.Count > 1) {
                        Console.WriteLine(new string('=', 70));
                        Console.WriteLine("¿Qué desea hacer?");
                        Console.WriteLine("1. Intentar con un monto menor");
                        Console.WriteLine("2. Elegir otra cuenta origen");
                        Console.WriteLine(new string('=', 70));
                        Console.Write("Ingrese su opción: ");
                        
                        string opcionFondos = Console.ReadLine();
                        if (opcionFondos == "2") {
                            // Permitir seleccionar otra cuenta origen
                            cuentaOrigen = null;
                            while (cuentaOrigen == null) {
                                Console.WriteLine(new string('=', 70));
                                Console.WriteLine($"Seleccione otra cuenta del cliente {clienteOrigen.Nombre}:");
                                for (int i = 0; i < cuentasOrigen.Count; i++) {
                                    Console.WriteLine($"{i + 1}. Cuenta {cuentasOrigen[i].NumeroCuenta} | Saldo: {cuentasOrigen[i].Saldo:C}");
                                }
                                Console.WriteLine(new string('=', 70));
                                Console.Write("Ingrese el número de la cuenta origen: ");
                                if (int.TryParse(Console.ReadLine(), out int nuevaCuentaIndex) && nuevaCuentaIndex >= 1 && nuevaCuentaIndex <= cuentasOrigen.Count) {
                                    cuentaOrigen = cuentasOrigen[nuevaCuentaIndex - 1];
                                } else {
                                    Console.WriteLine(new string('=', 70));
                                    Console.WriteLine("Selección inválida. Intente nuevamente.");
                                }
                            }
                            continue; // Volver a pedir el monto después de cambiar la cuenta
                        }
                        // Si eligió opción 1 o cualquier otra, intentará con un monto menor
                    } else {
                        Console.WriteLine("Intente con un monto menor.");
                    }
                } else {
                    // Realizar la transferencia si hay fondos suficientes
                    Transferencia transferencia = new Transferencia(montoTransferencia, cuentaOrigen, cuentaDestino);
                    banco.Registrar(transferencia);
                    banco.GuardarClientes();
                    transferenciaExitosa = true;
                    
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine($"Transferencia realizada exitosamente. Monto: {montoTransferencia:C} | Origen: {cuentaOrigen.NumeroCuenta} | Destino: {cuentaDestino.NumeroCuenta}");
                }
            }
            
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
        case "7":
            banco.Informe();
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
        case "8":
            Console.Clear();
            // Preguntar si se desea eliminar un cliente o una cuenta con validación
            string opcionEliminar = "";
            while (opcionEliminar != "1" && opcionEliminar != "2") {
                Console.WriteLine("Seleccione una opción:");
                Console.WriteLine(new string('=', 70));
                Console.WriteLine("1. Eliminar cliente");
                Console.WriteLine("2. Eliminar cuenta");
                Console.WriteLine(new string('=', 70));
                Console.Write("Ingrese su opción: ");
                opcionEliminar = Console.ReadLine();
                
                if (opcionEliminar != "1" && opcionEliminar != "2") {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Opción inválida. Intente nuevamente.");
                }
            }
            
            if (opcionEliminar == "1") {
                Console.Clear();
                if (!banco.ObtenerClientes().GetEnumerator().MoveNext()) {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("No hay clientes registrados. Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
                }
                
                // Seleccionar cliente a eliminar con reintentos
                Cliente clienteAEliminar = null;
                while (clienteAEliminar == null) {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Clientes existentes:");
                    var clientesEliminar = new List<Cliente>(banco.ObtenerClientes());
                    for (int i = 0; i < clientesEliminar.Count; i++) {
                        Console.WriteLine($"{i + 1}. {clientesEliminar[i].Nombre}");
                        foreach (var cuenta in clientesEliminar[i].Cuentas) {
                            Console.WriteLine($"   - Cuenta: {cuenta.NumeroCuenta} | Saldo: {cuenta.Saldo:C}");
                        }
                    }
                    Console.WriteLine(new string('=', 70));
                    Console.Write("Ingrese el número del cliente a eliminar: ");
                    if (int.TryParse(Console.ReadLine(), out int clienteIndexEliminar) && clienteIndexEliminar >= 1 && clienteIndexEliminar <= clientesEliminar.Count) {
                        clienteAEliminar = clientesEliminar[clienteIndexEliminar - 1];
                    } else {
                        Console.WriteLine(new string('=', 70));
                        Console.WriteLine("Selección inválida. Intente nuevamente.");
                    }
                }
                
                // Confirmar eliminación con validación
                Console.WriteLine(new string('=', 70));
                Console.WriteLine($"¿Está seguro que desea eliminar al cliente '{clienteAEliminar.Nombre}'? Esta acción no se puede deshacer.");
                Console.WriteLine("1. Sí, eliminar");
                Console.WriteLine("2. No, cancelar");
                Console.WriteLine(new string('=', 70));

                string confirmarEliminar = "";
                while (confirmarEliminar != "1" && confirmarEliminar != "2") {
                    Console.Write("Ingrese su opción (1 o 2): ");
                    confirmarEliminar = Console.ReadLine();
                    
                    if (confirmarEliminar != "1" && confirmarEliminar != "2") {
                        Console.WriteLine(new string('=', 70));
                        Console.WriteLine("Opción inválida. Por favor, ingrese 1 para confirmar o 2 para cancelar.");
                    }
                }

                if (confirmarEliminar == "1") {
                    banco.EliminarCliente(clienteAEliminar.Nombre); // El método ya imprime el mensaje
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Operación cancelada.");
                }
            } else if (opcionEliminar == "2") {
                Console.Clear();
                if (!banco.ObtenerClientes().GetEnumerator().MoveNext()) {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("No hay clientes ni cuentas registradas. Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
                }
                
                // Verificar si hay cuentas
                var todasLasCuentas = new List<(string NumeroCuenta, string ClienteNombre)>();
                foreach (var cliente in banco.ObtenerClientes()) {
                    foreach (var cuenta in cliente.Cuentas) {
                        todasLasCuentas.Add((cuenta.NumeroCuenta, cliente.Nombre));
                    }
                }
                
                if (todasLasCuentas.Count == 0) {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("No hay cuentas registradas. Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
                }
                
                // Seleccionar cuenta a eliminar con reintentos
                string cuentaAEliminar = null;
                while (cuentaAEliminar == null) {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Cuentas existentes:");
                    for (int i = 0; i < todasLasCuentas.Count; i++) {
                        Console.WriteLine($"{i + 1}. Cuenta: {todasLasCuentas[i].NumeroCuenta} | Cliente: {todasLasCuentas[i].ClienteNombre}");
                    }
                    Console.WriteLine(new string('=', 70));
                    Console.Write("Ingrese el número de la cuenta a eliminar: ");
                    
                    if (int.TryParse(Console.ReadLine(), out int cuentaIndexEliminar) && cuentaIndexEliminar >= 1 && cuentaIndexEliminar <= todasLasCuentas.Count) {
                        cuentaAEliminar = todasLasCuentas[cuentaIndexEliminar - 1].NumeroCuenta;
                    } else {
                        Console.WriteLine(new string('=', 70));
                        Console.WriteLine("Selección inválida. Intente nuevamente.");
                    }
                }
                
                // Confirmar eliminación de cuenta con validación
                Console.WriteLine(new string('=', 70));
                var infoEliminar = todasLasCuentas.Find(c => c.NumeroCuenta == cuentaAEliminar);
                Console.WriteLine($"¿Está seguro que desea eliminar la cuenta '{cuentaAEliminar}' del cliente '{infoEliminar.ClienteNombre}'? Esta acción no se puede deshacer.");
                Console.WriteLine("1. Sí, eliminar");
                Console.WriteLine("2. No, cancelar");
                Console.WriteLine(new string('=', 70));

                string confirmarEliminarCuenta = "";
                while (confirmarEliminarCuenta != "1" && confirmarEliminarCuenta != "2") {
                    Console.Write("Ingrese su opción (1 o 2): ");
                    confirmarEliminarCuenta = Console.ReadLine();
                    
                    if (confirmarEliminarCuenta != "1" && confirmarEliminarCuenta != "2") {
                        Console.WriteLine(new string('=', 70));
                        Console.WriteLine("Opción inválida. Por favor, ingrese 1 para confirmar o 2 para cancelar.");
                    }
                }

                if (confirmarEliminarCuenta == "1") {
                    banco.EliminarCuenta(cuentaAEliminar); // El método ya imprime el mensaje
                } else {
                    Console.WriteLine(new string('=', 70));
                    Console.WriteLine("Operación cancelada.");
                }
            }

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
        case "9":
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("Programa finalizado. Presione cualquier tecla para salir...");
            Console.ReadKey();
            return;
        default:
        Console.WriteLine(new string('=', 70));
            Console.WriteLine("Opción no válida. Intente nuevamente.");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            break;
    }
}