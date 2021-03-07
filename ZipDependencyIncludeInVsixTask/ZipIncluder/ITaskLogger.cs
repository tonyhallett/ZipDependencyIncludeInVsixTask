namespace ZipDependencyIncludeInVsixTask
{
    public interface ITaskLogger
    {
        void LogMessage(string message);
        void LogError(string errorMessage);
        void LogWarning(string warningMessage);
    }


}
