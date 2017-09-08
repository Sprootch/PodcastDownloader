using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public class Downloader : IDisposable
    {
        private readonly WebClient _client;

        public delegate void DownloadProgressEventHandler(object sender, DownloadProgressChangedEventArgs e);
        public event DownloadProgressEventHandler DownloadProgress;

        public Downloader()
        {
            _client = new WebClient
            {
                Encoding = Encoding.UTF8,
                Proxy = GetProxy()
            };
            _client.DownloadProgressChanged += (_, e) =>
            {
                DownloadProgress?.Invoke(this, e);
            };
        }

        private static IWebProxy GetProxy()
        {
            var proxyConfig = PodcastDownloaderConfiguration.Instance.Proxy;
            if (proxyConfig == null || string.IsNullOrWhiteSpace(proxyConfig.Address.Url)) return WebRequest.GetSystemWebProxy();

            return new WebProxy(proxyConfig.Address.Url, proxyConfig.Address.Port)
            {
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(proxyConfig.Security.UserName, proxyConfig.Security.Password)
            };
        }

        public string DownloadString(string url)
        {
            return _client.DownloadString(url);
        }

        public async Task<string> DownloadStringAsync(string url)
        {
            return await _client.DownloadStringTaskAsync(url);
        }

        public async Task<PodcastItem> DownloadPodcastAsync(PodcastItem podcast, string outputFolder, CancellationToken token, IProgress<int> progress = null)
        {
            // TODO: this line does not belong here.
            var outputFile = Path.Combine(outputFolder, podcast.Filename);

            token.Register(() => _client.CancelAsync());
            if (progress != null)
            {
                _client.DownloadProgressChanged += (_, e) => { progress.Report(e.ProgressPercentage); };
            }
            try
            {
                await _client.DownloadFileTaskAsync(podcast.DownloadUrl, outputFile);

                podcast.LocalPath = outputFile;
            }
            catch (WebException we)
            {
                TryDeleteFile(outputFile);
                if (we.Status != WebExceptionStatus.RequestCanceled)
                    throw;
            }

            return podcast;
        }

        private static void TryDeleteFile(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception)
            {
            }
        }

        //private async Task DownloadHttpClient(PodcastItem podcast, CancellationToken token)
        //{
        //    var httpClientHandler = new HttpClientHandler
        //    {
        //        Proxy = _proxy,
        //        PreAuthenticate = true,
        //        UseDefaultCredentials = false,
        //    };

        //    var client2 = new HttpClient(httpClientHandler);
        //    var r = client2.GetAsync(podcast.DownloadUrl, token);
        //    await r;
        //    bool b = r.IsCanceled;

        //}

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null) _client.Dispose();
            }
        }

        #endregion
    }
}
