namespace TP4_61450;

public static class Utilidades
{
    public static void Esperar()
    {
        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey(true);
    }

    public static string ObtenerEntrada(string mensaje)
    {
        Console.Write(mensaje);
        string entrada;
        do
        {
            entrada = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(entrada))
            {
                Console.WriteLine("Entrada inválida. Por favor, intente nuevamente.");
            }
        } while (string.IsNullOrWhiteSpace(entrada));
        return entrada;
    }
}
