using System.Security.Principal;
using NLog;

namespace Detacher
{
    internal class User
    {
        public string Username { get; private set; }
        public string Sid { get; private set; }

        public User()
        {
            Username = GetUsername();
            Sid = GetSidFromUsername(Username);
        }

        private static string GetUsername()
        {
            Console.Write("Введите имя учетной записи пользователя: ");
            var sid = Console.ReadLine();
            if (string.IsNullOrEmpty(sid) || sid.Length < 2 || sid.Length > 100)
            {
                Logging.Log.Warn("Имя пользователя не может содержать менее 2 и более 100 символов");
            }
            sid = sid.Trim().ToLower();
            return sid;
        }

        private static string GetSidFromUsername(string username)
        {
            if (!OperatingSystem.IsWindows())
            {
                Logging.Log.Error("Неподдерживаемая операционная система");
                return "";
            }
                
            var output = string.Empty;
            try
            {
                Logging.Log.Debug($"Получение SID для пользователя {username}...");
                var account = new NTAccount(username);
                var sid = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
                output = sid.ToString();
                Logging.Log.Debug($"SID для пользователя {username}: {output}");
            }
            catch (Exception ex)
            {
                Logging.Log.Error($"Произошла ошибка в метода GetSidFromUsername: {ex.Message}");
            }
            return output;
        }
    }
}