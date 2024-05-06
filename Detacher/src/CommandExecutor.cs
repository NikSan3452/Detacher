using System.Diagnostics;

namespace Detacher;

internal static class CommandExecutor
{
    public static string? RunCommandLineProcess(string fileName, string arguments)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            Logging.Log.Debug($"Запущен процесс {fileName} с аргументами {arguments}");

            var output = process?.StandardOutput.ReadToEnd();
            process?.WaitForExit();

            Logging.Log.Debug($"Процесс {fileName} завершен");
            if (!string.IsNullOrEmpty(output)) Logging.Log.Debug($"\nПроцесс вернул: {output}");

            return output;
        }
        catch (Exception ex)
        {
            Logging.Log.Error($"Произошла ошибка в методе RunCommandLineProcess: {ex.Message}");
            return null;
        }
    }
}