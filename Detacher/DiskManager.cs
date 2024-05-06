namespace Detacher;

internal class DiskManager(string sid, Config config)
{
    public List<string> SearchDisk()
    {
        Logging.Log.Debug("Выполняется поиск дисков...");
        var diskList = new List<string>();
        if (config.DiskPaths == null) return diskList;
        foreach (var path in config.DiskPaths) SearchDiskInDirectory(path.Trim(), diskList);

        Logging.Log.Debug($"Список дисков для SID {sid}:\n{string.Join("\n", diskList)}");
        return diskList;
    }

    private void SearchDiskInDirectory(string dir, ICollection<string> diskList)
    {
        foreach (var diskName in Directory.GetFiles(dir))
        {
            if (!diskName.Contains(sid)) continue;
            diskList.Add(diskName);
        }

        foreach (var subDir in Directory.GetDirectories(dir))
            try
            {
                SearchDiskInDirectory(subDir, diskList);
            }
            catch (Exception ex)
            {
                Logging.Log.Error($"Не удалось просканировать подкаталог {subDir}: {ex.Message}");
            }
    }

    public void DismountDisk(List<string> diskList)
    {
        Logging.Log.Debug("Выполняется отсоединение дисков...");
        foreach (var disk in diskList)
        {
            var command = $"$CimSession = New-CimSession -ComputerName \"{config.ComputerName}\"; " +
                          $"Dismount-DiskImage -ImagePath \"{disk}\" -CimSession $CimSession";
            CommandExecutor.RunCommandLineProcess("powershell.exe", $"-Command \"{command}\"");
            Logging.Log.Debug($"Диск {disk} на целевом ресурсе {config.ComputerName} отсоединен");
        }
    }
}