namespace FtpFileManager
{
    public sealed class FtpConnectionSettings
    {
        public string Url { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int RetryAttempts { get; set; } = 3;

        public int TimeoutRetryAttempts { get; set; } = 10000;
    }
}
