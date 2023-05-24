using System;
using System.Net;

namespace Laberis.com.lairon.laberis.server.exception
{
    public class ServerErrorException : Exception
    {
        public HttpStatusCode code { get; }

        public ServerErrorException(HttpStatusCode code)
        {
            this.code = code;
        }
    }
}