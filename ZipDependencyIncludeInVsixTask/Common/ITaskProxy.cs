using System;

namespace ZipDependencyIncludeInVsixTask
{
    internal interface ITaskProxy
    {
        ZipInclusion LogOrReturnZip(string message, Action<string> logMessage, Action<string> logWarning);
    }

}
