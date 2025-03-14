using System;

namespace APIAPP.Exceptions
{
    public class AuthException : Exception
    {
        public int StatusCode { get; }

        public AuthException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public AuthException(string message, int statusCode, Exception inner) : base(message, inner)
        {
            StatusCode = statusCode;
        }
    }
}
