using System.Text.RegularExpressions;

namespace Detacher;

internal class SessionManager(Config config)
{
    public List<string> GetSessions(string username)
    {
        var sessionList = new List<string>();

        if (config.RDSList != null)
        {
            Logging.Log.Debug($"Запущен поиск сеансов для пользователя {username}...");
            foreach (var rds in config.RDSList)
            {
                var output =
                    CommandExecutor.RunCommandLineProcess("cmd.exe", $"/c query session {username} /server:{rds}");
                var outputLines = output?.Split('\n');

                if (outputLines == null) continue;

                var session = outputLines
                    .Where(line => line.Contains(username))
                    .Select(line => ParseSessionId(line, username))
                    .Where(sessionId => !string.IsNullOrEmpty(sessionId))
                    .Select(sessionId => $"{sessionId}@{rds}");

                sessionList.AddRange(session);
            }
        }

        if (sessionList.Count == 0)
            Logging.Log.Debug("Для данного пользователя не найдено ни одного сеанса");
        else
            Logging.Log.Debug($"Сеансы для пользователя {username}:{string.Join("\n",
                sessionList.Select(s => $"ID = {s.Split('@')[0]} RDS = {s.Split('@')[1]}"))}");
        ;

        return sessionList;
    }

    private static string? ParseSessionId(string line, string username)
    {
        var match = Regex.Match(line, @"(?<=" + username + @"\s+)\b\d+\b");

        return match.Success ? match.Value : null;
    }


    public static void ResetSessionsById(List<string> sessionList)
    {
        if (sessionList.Count == 0) return;

        foreach (var sessionId in sessionList)
        {
            var parts = sessionId.Split('@');
            CommandExecutor.RunCommandLineProcess("cmd.exe", $"/c reset session {parts[0]} /server:{parts[1]}");
            Logging.Log.Debug($"Сеанс {parts[0]} завершен");
        }
    }
}