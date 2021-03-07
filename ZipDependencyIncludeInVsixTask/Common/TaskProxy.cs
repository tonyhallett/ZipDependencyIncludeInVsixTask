using System;

namespace ZipDependencyIncludeInVsixTask
{
    internal class TaskProxy : ITaskLogger, IZipIncludeAnnouncer, ITaskProxy
    {
        public const string WarningPrefix = "Warning";
        public const string MessagePrefix = "Message";
        public const string IncludeZipPrefix = "IncludeZip";

        public ZipInclusion LogOrReturnZip(string message, Action<string> logMessage, Action<string> logWarning)
        {
            var prefixSeparator = message.IndexOf(":");
            var prefix = message.Substring(0, prefixSeparator);
            message = message.Substring(prefixSeparator + 1);
            switch (prefix)
            {
                case MessagePrefix:
                    logMessage(message);
                    break;
                case WarningPrefix:
                    logWarning(message);
                    break;
                case IncludeZipPrefix:
                    return ZipInclusion.FromString(message);
            }
            return null;
        }
        public void LogError(string message)
        {
            Console.Error.WriteLine(message);
        }

        public void LogMessage(string message)
        {
            LogWithPrefix(MessagePrefix, message);
        }

        public void LogWarning(string warning)
        {
            LogWithPrefix(WarningPrefix, warning);
        }

        private void LogWithPrefix(string prefix, string message)
        {
            Console.WriteLine($"{prefix}:{message}");
        }

        
        public void IncludeZip(ZipInclusion zipInclusion)
        {
            LogWithPrefix(IncludeZipPrefix, zipInclusion.ToString());
        }

    }

}
