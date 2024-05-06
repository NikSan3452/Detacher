using NLog;

namespace Detacher;

public static class Logging
{
    public static Logger Log { get; } = LogManager.GetCurrentClassLogger();
}