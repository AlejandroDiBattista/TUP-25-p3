# TP2: Sistema Bancario 

> El pull request tambien debe llamar "TP2 - <Legajo> <NombreCopleto>"

## Objetivo:
Implementar un sistema bancario en C# utilizando programación orientada a objetos que permita gestionar clientes, cuentas y operaciones bancarias. 

El sistema deberá registrar de forma individual cada operación (depósito, extracción, pago y transferencia) en clases separadas y mantener un historial global en el banco, así como un historial personal en cada cliente.

## Requerimientos:
1. Clientes y Cuentas:
    * El banco debe poder gestionar múltiples clientes.
    * Cada cliente tiene un nombre y puede poseer varias cuentas.
    * Cada cuenta cuenta con un número (formato XXXXX) y un saldo.
    * El número de cuenta debe ser único.
    * Se deben definir tres tipos de cuentas:
        * Oro: Acumula un 5% de Puntos sobre los pagos realizados para montos mayores a 1000 y un 3% para montos menores.
        * Plata: Acumula un 2% de Puntos sobre los pagos realizados.
        * Bronce: Acumula un 1% de Puntos sobre los pagos realizados.

2. Operaciones Bancarias:
    * El sistema debe soportar las siguientes operaciones:
        * Depositar: Incrementa el saldo de la cuenta.
        * Extraer: Disminuye el saldo de la cuenta si hay fondos suficientes.
        * Pagar: Disminuye el saldo de la cuenta y acumula Puntos según el porcentaje definido para el tipo de cuenta.
        * Transferir: Permite mover fondos de una cuenta a otra.
    * Cada operación se debe implementar en su propia clase derivada de una clase abstracta base llamada Operacion.
    * Cada operación debe registrar el monto y las cuentas involucradas (para transferencias se deben registrar ambas: origen y destino).
    * El banco debe verificar que la cuenta origen de la operacion es suya.

3. Registro y Reportes:
    * El banco debe mantener un registro global de todas las operaciones realizadas.
    * Además, cada cliente debe mantener un historial (lista) de las operaciones realizadas en cualquiera de sus cuentas.
    * Al finalizar las operaciones, el sistema deberá generar un informe completo que muestre:
        * El detalle global de las operaciones (tipo, monto y cuentas involucradas).
        * El estado final de cada cuenta (saldo y Puntos acumulado).
        * El historial individual de operaciones para cada cliente.

## Clases:
El programa debe incluir las siguientes clases:

* `Banco`: Gestiona clientes, cuentas y operaciones.
* `Cliente`: Representa a un cliente con un nombre y una lista de cuentas.
* `Cuenta` (abstracta): Clase base para las cuentas bancarias.
    * `CuentaOro`: Implementa la acumulación de puntos específica para cuentas Oro.
    * `CuentaPlata`: Implementa la acumulación de puntos específica para cuentas Plata.
    * `CuentaBronce`: Implementa la acumulación de puntos específica para cuentas Bronce.
* `Operacion` (abstracta): Clase base para las operaciones bancarias.
    * `Deposito`: Representa una operación de depósito.
    * `Retiro`: Representa una operación de extracción.
    * `Pago`: Representa una operación de pago.
    * `Transferencia`: Representa una operación de transferencia entre cuentas.

## Consideraciones Adicionales:
* Utiliza principios de programación orientada a objetos (herencia, encapsulamiento y polimorfismo).
* Asegúrate de manejar adecuadamente los posibles errores (por ejemplo, fondos insuficientes para extracción o pago).

## Ejemplo de salida 
Dato el siguiente ejemplo de uso:
```csharp
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
```

Debe producir la siguiente salida
```bash
Banco: Banco Nac | Clientes: 2

  Cliente: Raul Perez | Saldo Total: $ 2.800,00 | Puntos Total: $ 8,00

    Cuenta: 10001 | Saldo: $ 800,00 | Puntos: $ 0,00
     -  Deposito $ 100,00 a [10001/Raul Perez]
     -  Transferencia $ 300,00 de [10001/Raul Perez] a [10002/Raul Perez]

    Cuenta: 10002 | Saldo: $ 2.000,00 | Puntos: $ 8,00
     -  Retiro $ 200,00 de [10002/Raul Perez]
     -  Transferencia $ 300,00 de [10001/Raul Perez] a [10002/Raul Perez]
     -  Pago $ 400,00 con [10002/Raul Perez]
     -  Transferencia $ 300,00 de [10005/Luis Gomez] a [10002/Raul Perez]

  Cliente: Sara Lopez | Saldo Total: $ 7.000,00 | Puntos Total: $ 0,00

    Cuenta: 10003 | Saldo: $ 2.500,00 | Puntos: $ 0,00
     -  Transferencia $ 500,00 de [10003/Sara Lopez] a [10004/Sara Lopez]

    Cuenta: 10004 | Saldo: $ 4.500,00 | Puntos: $ 0,00
     -  Transferencia $ 500,00 de [10003/Sara Lopez] a [10004/Sara Lopez]


Banco : Banco TUP | Clientes: 1

  Cliente: Luis Gomez | Saldo Total: $ 4.200,00 | Puntos Total: $ 4,00

    Cuenta: 10005 | Saldo: $ 4.200,00 | Puntos: $ 4,00
     -  Deposito $ 100,00 a [10005/Luis Gomez]
     -  Retiro $ 200,00 de [10005/Luis Gomez]
     -  Transferencia $ 300,00 de [10005/Luis Gomez] a [10002/Raul Perez]
     -  Pago $ 400,00 con [10005/Luis Gomez]
```

## Entrega:
Desarrolla y documenta el código en C#. El programa debe compilar y ejecutarse mostrando en consola el informe global de operaciones y el historial individual de cada cliente, junto con el estado final de sus cuentas.