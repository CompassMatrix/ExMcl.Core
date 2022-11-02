using Mcl.Core.Network.Interface;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mcl.Core.Network
{
    [DebuggerDisplay("{DebuggerDisplay()}")]
    public class NetResponse : NetResponseBase, INetResponse
    {
        public NetResponse()
        {
            base.Headers = new List<Parameter>();
        }
    }
    [DebuggerDisplay("{DebuggerDisplay()}")]
    public class NetResponse<T> : NetResponseBase, INetResponse<T>, INetResponse
    {
        public T Data { get; set; }

        public static explicit operator NetResponse<T>(NetResponse response)
        {
            return new NetResponse<T>
            {
                ContentEncoding = response.ContentEncoding,
                ContentLength = response.ContentLength,
                ContentType = response.ContentType,
                Cookies = response.Cookies,
                ErrorMessage = response.ErrorMessage,
                ErrorException = response.ErrorException,
                Headers = response.Headers,
                RawBytes = response.RawBytes,
                ResponseStatus = response.ResponseStatus,
                ResponseUri = response.ResponseUri,
                Server = response.Server,
                StatusCode = response.StatusCode,
                StatusDescription = response.StatusDescription,
                Request = response.Request,
                ProtocolVersion = response.ProtocolVersion
            };
        }
    }
}
