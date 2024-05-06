namespace Detacher
{
    public static class Program
    {
        public static void Main()
        {
            if (!OperatingSystem.IsWindows()) return;

            try
            {
                var user = new User();
                var config = new Config();
                config.Init();
                var diskManager = new DiskManager(user.Sid, config);
                var diskList = diskManager.SearchDisk();
                var sessionManager = new SessionManager(config);
                var sessionList = sessionManager.GetSessions(user.Username);
                SessionManager.ResetSessionsById(sessionList);
                diskManager.DismountDisk(diskList);
            }
            catch (Exception ex)
            {
                Logging.Log.Error($"Произошла ошибка: {ex.Message}");
            }

            Console.ReadLine();

            Environment.Exit(0);
        }
    }
}