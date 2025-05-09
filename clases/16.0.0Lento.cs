using System.Threading;
using System.Threading.Tasks;

async Task FuncionLenta(){
    var suma = 0.0;
    for(int i = 0; i < 50; i++){
        // Simulando una operación lenta
        suma += Math.Sqrt(i);
        WriteLine($"Calculando... {i} - {suma}");
        await Task.Delay(100);
    }
    WriteLine($"Resultado de la función lenta: {suma} {DateTime.Now}");
}

async Task Probar(){
    WriteLine($"Inicio de la función lenta {DateTime.Now}");
    await FuncionLenta();
    WriteLine($"Fin de la función lenta {DateTime.Now} \n Las suma es");
}

Clear();
await Probar();
await Task.Delay(10_000);
