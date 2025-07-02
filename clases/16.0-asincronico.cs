using System;
using System.Threading;
using System.Threading.Tasks;

async Task CalculoLento(string nombre = "") {
    WriteLine($"Inicio del calculo lento: {nombre}");
    for(int i = 0; i < 3; i++) {
        WriteLine($" - Calculando {nombre}... {i}");
        await Task.Delay(1000);
    }
    WriteLine($"Fin del calculo lento: {nombre}");
}

async Task ProbarSimple(){
    WriteLine("Inicio Prueba ");
    CalculoLento("Uno");
    WriteLine("Fin Prueba ");
}

async Task ProbarSinAwait() {
    WriteLine($"\n=== Prueba SIN await === {DateTime.Now}");
    WriteLine("* Inicio Prueba *");
    CalculoLento("UNO");  // Sin await - no espera que termine
    WriteLine($"* Fin Prueba SIN await * {DateTime.Now}");
}

async Task ProbarConAwait() {
    WriteLine($"\n=== Prueba CON await === {DateTime.Now}");
    WriteLine("* Inicio Prueba *");
    await CalculoLento("DOS");  // Con await - espera que termine
    WriteLine($"* Fin Prueba CON await* {DateTime.Now}");
}

// Probar Simultaneo 
async Task ProbarSimultaneo() {
    WriteLine($"\n=== Prueba Simultanea === {DateTime.Now}");
    WriteLine("* Inicio Prueba *");
    var tarea3 = CalculoLento("TRES");
    var tarea4 = CalculoLento("CUATRO");
    await Task.WhenAll(tarea3, tarea4);  // Espera a que ambas terminen
    WriteLine($"* Fin Prueba Simultanea * {DateTime.Now}");
}

// Probar en Secuencia
async Task ProbarSecuencia() {
    WriteLine($"\n=== Prueba Secuencial === {DateTime.Now}");
    WriteLine("* Inicio Prueba *");
    await CalculoLento("UNO");
    await CalculoLento("DOS");
    WriteLine($"* Fin Prueba Secuencial * {DateTime.Now}");
}

// Ejecutamos ambas pruebas
Clear();
await ProbarSinAwait();
await Task.Delay(5000); // Pausa para separar las pruebas
await ProbarConAwait();
await Task.Delay(5000);
await ProbarSimultaneo(); // Pausa para separar las pruebas
// await Task.Delay(5000);
// await ProbarSecuencia(); // Pausa para separar las pruebas