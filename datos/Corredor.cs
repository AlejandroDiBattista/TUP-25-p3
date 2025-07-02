using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using TUP;

public enum ResultadoEjecucion {
    Ok,
    FallaCapeta,
    FallaServidor,
    FallaCliente,
    FallaNavegador,
    FallaBaseDatos,
    FallaGeneral
}

public class Corredor {
    public static async Task<ResultadoEjecucion> CorrerSistema(string origen) {
        var carpetaServidor = Path.Combine(origen, "servidor");
        var carpetaCliente = Path.Combine(origen, "cliente");

        try {
            // Matar procesos dotnet escuchando en puertos TCP (equivalente a: lsof -tiTCP -sTCP:LISTEN -c dotnet | xargs -r kill -9)
            await MatarProcesoPuerto("5184");
            await MatarProcesoPuerto("5177");

            var resultadoServidor = await EjecutarProyecto(carpetaServidor);
            if (!resultadoServidor.StartsWith("ok")) {
                return resultadoServidor.Contains("base de datos")
                    ? ResultadoEjecucion.FallaBaseDatos
                    : ResultadoEjecucion.FallaServidor;
            }
            Consola.Escribir($"[Auditoría] Servidor iniciado correctamente en {resultadoServidor}", ConsoleColor.Yellow);
            // Esperar un poco para que el servidor inicie
            await Task.Delay(1000);

            // Ejecutar cliente
            var resultadoCliente = await EjecutarProyecto(carpetaCliente);
            if (!resultadoCliente.StartsWith("ok")) {

                return resultadoCliente.Contains("base de datos")
                    ? ResultadoEjecucion.FallaBaseDatos
                    : ResultadoEjecucion.FallaCliente;
            }

            // Esperar un poco para que el cliente inicie
            Consola.Escribir($"[Auditoría] Cliente iniciado correctamente en {resultadoCliente}", ConsoleColor.Yellow);
            await Task.Delay(1000);

            // Abrir el navegador en localhost:5177
            Trace.WriteLine($"[Auditoría] Intentando abrir navegador en http://localhost:5177");
            var matchPuerto = Regex.Match(resultadoCliente, @"ok:(\d+)");
            var puerto = matchPuerto.Success ? matchPuerto.Groups[1].Value : string.Empty;
            if (string.IsNullOrEmpty(puerto))
            {
                puerto = "5177"; // Valor por defecto si no se pudo extraer el puerto
            }
            puerto = $"http://localhost:{puerto}";
            try
            {
                Trace.WriteLine($"[Auditoría] Intentando abrir navegador en {puerto} y corren en {resultadoCliente}");
                var psi = new ProcessStartInfo
                {
                    FileName = "open",
                    Arguments = puerto,
                    UseShellExecute = true
                };
                Process.Start(psi);
                Consola.Escribir($"[Auditoría] Navegador abierto en {puerto}", ConsoleColor.Yellow);
                return ResultadoEjecucion.Ok;
            }
            catch
                {
                    return ResultadoEjecucion.FallaNavegador;
                }
        } catch {
            return ResultadoEjecucion.FallaGeneral;
        }
    }

    private static async Task<string> EjecutarProyecto(string carpetaProyecto) {
        for (int intento = 1; intento <= 2; intento++) {
            Trace.WriteLine($"[Auditoría] EjecutarProyecto: Iniciando proyecto '{carpetaProyecto}', intento {intento}");
            try {
                var psi = new ProcessStartInfo {
                    FileName = "dotnet",
                    Arguments = "run",
                    WorkingDirectory = carpetaProyecto,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                var proceso = new Process { StartInfo = psi };
                proceso.Start();

                // Leer algunas líneas del output sin agotar el stream
                string puerto = string.Empty;
                for (int i = 0; i < 10; i++) {
                    if (proceso.HasExited) break;
                    if (proceso.StandardOutput.Peek() > -1) {
                        string linea = await proceso.StandardOutput.ReadLineAsync() ?? string.Empty;
                        puerto = ExtraerPuerto(linea);
                        if (!string.IsNullOrEmpty(puerto)) break;
                    } else {
                        await Task.Delay(100); // Esperar un poco si no hay línea disponible
                    }
                }

                if (!proceso.HasExited) {
                    return !string.IsNullOrEmpty(puerto) ? $"ok:{puerto}" : "ok:desconocido";
                }
                var output = await proceso.StandardOutput.ReadToEndAsync();
                var errorCompleto = output + await proceso.StandardError.ReadToEndAsync();
                if (proceso.ExitCode != 0 &&
                    (errorCompleto.Contains("address already in use") || errorCompleto.Contains("puerto") || errorCompleto.Contains("port"))) {
                    // Extraer puerto del mensaje de error
                    var match = Regex.Match(errorCompleto, @":(\d+)");
                    var puertoBloqueado = match.Success ? match.Groups[1].Value : "desconocido";
                    Trace.WriteLine($"[Auditoría] Puerto ocupado detectado: {puertoBloqueado}");
                    if (intento == 1) {
                        await MatarProcesoPuerto(puertoBloqueado);
                        continue; // Reintentar
                    } else {
                        return $"error: Puerto {puertoBloqueado} ocupado después de limpiar procesos";
                    }
                } else if (errorCompleto.Contains("no such table") || errorCompleto.Contains("Migrations") || errorCompleto.Contains("SqliteException") || errorCompleto.Contains("Unhandled exception")) {
                    return $"error: Error de base de datos - falta crear migraciones o aplicar schema";
                } else {
                    return $"error: {errorCompleto}";
                }
            } catch (Exception ex) {
                if (intento == 2) {
                    return $"error: {ex.Message}";
                }
                await MatarProcesosPuertos();
            }
        }
        return $"error: Falló después de 2 intentos";
    }

    // Extrae el puerto de una línea de output típica de dotnet run
    private static string ExtraerPuerto(string linea) {
        // Busca patrones como: Now listening on: http://localhost:5184
        if (linea.Contains("Now listening on")) {
            var match = Regex.Match(linea, @"http://localhost:(\d+)");
            if (match.Success && match.Groups.Count > 1) {
                return match.Groups[1].Value;
            }
        }
        return string.Empty;
    }

    // Nuevo método para matar procesos en puerto específico
    private static async Task MatarProcesoPuerto(string puerto) {
        try {
            Trace.WriteLine($"[Auditoría] MatarProcesoPuerto: Puerto {puerto}");
            var psi = new ProcessStartInfo { FileName = "lsof", Arguments = $"-ti:{puerto}", UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true };
            using var proceso = Process.Start(psi);
            if (proceso != null) {
                var salida = await proceso.StandardOutput.ReadToEndAsync();
                await proceso.WaitForExitAsync();
                if (!string.IsNullOrWhiteSpace(salida)) {
                    foreach (var pid in salida.Trim().Split('\n')) {
                        if (int.TryParse(pid.Trim(), out int pidNumero)) {
                            try {
                                var kill = Process.Start("kill", $"-9 {pidNumero}");
                                if (kill != null) {
                                    await kill.WaitForExitAsync();
                                }
                            } catch { }
                        }
                    }
                }
            }
        } catch { }
    }

    private static async Task MatarProcesosPuertos() {
        try {
            var psi = new ProcessStartInfo {
                FileName = "lsof",
                Arguments = "-tiTCP -sTCP:LISTEN -c dotnet",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            using var proceso = Process.Start(psi);
            if (proceso != null) {
                var salida = await proceso.StandardOutput.ReadToEndAsync();
                await proceso.WaitForExitAsync();
                if (!string.IsNullOrWhiteSpace(salida)) {
                    foreach (var pid in salida.Trim().Split('\n')) {
                        if (int.TryParse(pid.Trim(), out int pidNumero)) {
                            try {
                                var kill = Process.Start("kill", $"-9 {pidNumero}");
                                if (kill != null) {
                                    await kill.WaitForExitAsync();
                                }
                            } catch { }
                        }
                    }
                }
            }
        } catch {
            // Manejo de errores opcional o logging
        }
    }

}