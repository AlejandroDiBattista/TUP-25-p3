using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TUP;

public enum ResultadoEjecucion {
    Ok,
    FallaServidor,
    FallaCliente,
    FallaNavegador,
    NoPresentado
}

public class Corredor
{
    public static async Task<ResultadoEjecucion> CorrerSistema(string carpetaBase)
    {
        Console.WriteLine($"Iniciando prueba de sistema en: {carpetaBase}");
        string servidorPath = Path.Combine(carpetaBase, "servidor");
        string clientePath = Path.Combine(carpetaBase, "cliente");

        Consola.Escribir($"=== Lanzando servidor en: {servidorPath} ===", ConsoleColor.Red);
        var (direccionServidor, servidorOk) = await LanzarYDetectarAsync(servidorPath, true);
        if (!servidorOk || string.IsNullOrEmpty(direccionServidor))
        {
            Console.WriteLine("No se pudo iniciar el servidor correctamente.");
            return ResultadoEjecucion.FallaServidor;
        }

        Console.WriteLine($"=== Lanzando cliente en: {clientePath} ===");
        var (direccionCliente, clienteOk) = await LanzarYDetectarAsync(clientePath, true);
        if (!clienteOk || string.IsNullOrEmpty(direccionCliente))
        {
            Console.WriteLine("No se pudo iniciar el cliente correctamente.");
            return ResultadoEjecucion.FallaCliente;
        }

        if (!string.IsNullOrEmpty(direccionCliente))
        {
            Console.WriteLine($"Abriendo navegador en: {direccionCliente}");
            try
            {
                var openInfo = new ProcessStartInfo("open", $"-u {direccionCliente}")
                {
                    UseShellExecute = false
                };
                Process.Start(openInfo);
                return ResultadoEjecucion.Ok;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No se pudo abrir el navegador automáticamente: {ex.Message}");
                return ResultadoEjecucion.FallaNavegador;
            }
        }
        else
        {
            Console.WriteLine("No se pudo abrir el navegador porque no se detectó la dirección del cliente.");
            return ResultadoEjecucion.FallaNavegador;
        }
    }

    private static async Task<(string direccion, bool ok)> LanzarYDetectarAsync(string workingDir, bool buscarDireccion)
    {
        Console.WriteLine($"Ejecutando 'dotnet run' en: {workingDir}");
        var info = new ProcessStartInfo("dotnet", "run")
        {
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Process proc = null;
        string direccion = null;
        try
        {
            proc = Process.Start(info);
            if (proc == null)
            {
                Console.WriteLine($"No se pudo iniciar el proceso en {workingDir}.");
                return (null, false);
            }
            // Leer la salida en tiempo real y buscar la URL
            var urlRegex = new Regex(@"http://(?:localhost|127\.0\.0\.1):\d+");
            bool urlDetectada = false;
            while (!proc.HasExited && !urlDetectada)
            {
                string? linea = await proc.StandardOutput.ReadLineAsync();
                if (linea == null) break;
                Console.WriteLine($"[servidor] {linea}");
                var match = urlRegex.Match(linea);
                if (buscarDireccion && match.Success)
                {
                    direccion = match.Value;
                    Console.WriteLine($"Sitio iniciado en: {direccion}");
                    urlDetectada = true;
                }
                if (linea.Contains("address already in use"))
                {
                    Console.WriteLine($"El puerto en {workingDir} está ocupado. Intentando liberar...");
                    await LiberarPuertoAsync(linea);
                    // Reintentar
                    Console.WriteLine($"Reintentando 'dotnet run' en: {workingDir}");
                    proc.Kill();
                    return await LanzarYDetectarAsync(workingDir, buscarDireccion);
                }
            }
            // También leer errores
            while (!proc.HasExited && !urlDetectada)
            {
                string? linea = await proc.StandardError.ReadLineAsync();
                if (linea == null) break;
                Console.WriteLine($"[servidor][err] {linea}");
                var match = urlRegex.Match(linea);
                if (buscarDireccion && match.Success)
                {
                    direccion = match.Value;
                    Console.WriteLine($"Sitio iniciado en: {direccion}");
                    urlDetectada = true;
                }
                if (linea.Contains("address already in use"))
                {
                    Console.WriteLine($"El puerto en {workingDir} está ocupado. Intentando liberar...");
                    await LiberarPuertoAsync(linea);
                    // Reintentar
                    Console.WriteLine($"Reintentando 'dotnet run' en: {workingDir}");
                    proc.Kill();
                    return await LanzarYDetectarAsync(workingDir, buscarDireccion);
                }
            }
            return (direccion, urlDetectada);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al iniciar el proceso en {workingDir}: {ex.Message}");
            return (direccion, false);
        }
    }

    private static async Task LiberarPuertoAsync(string salida)
    {
        // Buscar puerto en localhost:XXXX o 127.0.0.1:XXXX
        var match = Regex.Match(salida, @"(?:localhost|127\.0\.0\.1):(\d+)");
        if (match.Success)
        {
            string puerto = match.Groups[1].Value;
            Console.WriteLine($"Buscando proceso en el puerto {puerto}...");
            var lsof = new ProcessStartInfo("lsof", $"-ti tcp:{puerto}")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using var lsofProc = Process.Start(lsof);
            string pidsOutput = await lsofProc.StandardOutput.ReadToEndAsync();
            lsofProc.WaitForExit();

            // Dividir los PIDs por líneas y procesar cada uno
            var pids = pidsOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pidStr in pids)
            {
                var pid = pidStr.Trim();
                if (!string.IsNullOrEmpty(pid) && int.TryParse(pid, out int pidInt))
                {
                    try
                    {
                        Process.GetProcessById(pidInt).Kill();
                        Console.WriteLine($"PID {pid} terminado.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"No se pudo cerrar PID {pid}: {ex.Message}");
                    }
                }
            }
            Console.WriteLine($"Puerto {puerto} liberado.");
        }
    }
}