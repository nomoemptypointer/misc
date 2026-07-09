namespace Trax.Utils
{
    public enum LogType
    {
        Error,
        Info,
        Warning,
        Debug,
        Custom,
        Unknown
    }

    public class Debug
    {
        private static string logFilePath;

        public static void Log(LogType type, string message, string codeContext = null)
        {
            string logMessage = $"[{DateTime.Now}] [{type.ToString().ToUpper()}]";
            if (!string.IsNullOrEmpty(codeContext)) logMessage += $" [{codeContext}]";
            logMessage += $" {message}";
            Console.WriteLine(logMessage);
            WriteLogToFile(logMessage);
        }

        private static void WriteLogToFile(string logMessage)
        {
            string logsDirectory = "logs";
            if (!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }
            string dateStamp = DateTime.Now.ToString("yyyyMMdd");
            logFilePath = Path.Combine(logsDirectory, $"{dateStamp}.txt");

            try
            {
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}
