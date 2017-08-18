using System;

namespace TTMapSchemaGUID.Utility
{
    public class DSBindingException : Exception
    {
        public DSBindingException() : base() { }
        public DSBindingException(string message) : base(message) { }
        public DSBindingException(string message, Exception inner) : base(message, inner) { }
    }
}
