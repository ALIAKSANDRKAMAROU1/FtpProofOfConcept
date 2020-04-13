using FluentFTP;
using Polly;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FtpFileManager
{
    public sealed class FileManager
    {
        private readonly FtpConnectionSettings _ftpConnectionSettings;

        public FileManager(FtpConnectionSettings ftpConnectionSettings)
        {
            _ftpConnectionSettings = ftpConnectionSettings ?? throw new ArgumentNullException("FtpConnectionSettings cannot be null.");
        }

        public async Task SendFile(string localPath, string remotePath, CancellationToken cancellationToken)
        {
            using (var client = new FtpClient(_ftpConnectionSettings.Url))
            {
                client.Credentials = new NetworkCredential(_ftpConnectionSettings.UserName, _ftpConnectionSettings.Password);

                var polly = Policy.Handle<FtpException>()
                    .Or<FtpCommandException>()
                    .WaitAndRetryAsync(_ftpConnectionSettings.RetryAttempts, retryAttempt => TimeSpan.FromMilliseconds(_ftpConnectionSettings.TimeoutRetryAttempts));

                await polly.ExecuteAsync(async () =>
                {
                    await client.ConnectAsync(cancellationToken);

                    if (await client.FileExistsAsync(remotePath, cancellationToken))
                    {
                        await client.UploadFileAsync(localPath, remotePath, FtpRemoteExists.Overwrite, token: cancellationToken);
                    }
                    else
                    {
                        await client.UploadFileAsync(localPath, remotePath, token: cancellationToken);
                    }
                });
            }
        }
    }
}
