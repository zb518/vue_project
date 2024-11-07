using Microsoft.Extensions.Logging;

namespace PPE.WebCore;

public static class LoggerConfigExtensions
{
    public static void AddLog4netExt(this ILoggingBuilder logging)
    {
        logging.AddLog4Net("XmlConfig/log4net.config");
    }
}