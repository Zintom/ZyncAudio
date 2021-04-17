namespace ZyncAudio
{
    public interface ILogger
    {
        void Log(string message, LogLevel severity = LogLevel.Information);

        public enum LogLevel
        {
            Information,
            Warning,
            Error
        }
    }
}
