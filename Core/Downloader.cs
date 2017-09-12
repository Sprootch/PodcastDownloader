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
                Encoding = Encoding.UTF8
            };
            _client.DownloadProgressChanged += (_, e) =>
            {
                DownloadProgress?.Invoke(this, e);
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

        public async Task<PodcastItem> DownloadPodcastAsync(PodcastItem podcast, CancellationToken token, IProgress<int> progress = null)
        {
            token.Register(() => _client.CancelAsync());
            if (progress != null)
            {
                _client.DownloadProgressChanged += (_, e) => { progress.Report(e.ProgressPercentage); };
            }
            try
            {
                await _client.DownloadFileTaskAsync(podcast.DownloadUrl, podcast.Path);
            }
            catch (WebException we)
            {
                TryDeleteFile(podcast.Path);
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
