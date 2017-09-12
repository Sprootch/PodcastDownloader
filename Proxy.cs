using System.Net;

namespace Core
{
    public class Proxy
    {
        public IWebProxy WebProxy { get; private set; }

        public Proxy(string address, int port, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                port == default(int))
            {
                WebProxy = WebRequest.GetSystemWebProxy();
            }
            else
            {
                WebProxy = new WebProxy(address, port)
                {
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password)
                };
            }
        }
    }
}
