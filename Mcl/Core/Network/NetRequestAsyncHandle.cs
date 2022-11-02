using System.Net;

namespace Mcl.Core.Network
{
    public class NetRequestAsyncHandle
    {
        public HttpWebRequest WebRequest;

        public NetRequestAsyncHandle()
        {
        }

        public NetRequestAsyncHandle(HttpWebRequest webRequest)
        {
            WebRequest = webRequest;
        }

        public void Abort()
        {
            WebRequest?.Abort();
        }
    }
}
