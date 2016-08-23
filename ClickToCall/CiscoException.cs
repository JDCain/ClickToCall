using System;
using System.Runtime.Serialization;

namespace ClickToCall
{
    [Serializable]
    internal class CiscoException : Exception
    {
        private int v;
        private string value;

        public CiscoException()
        {
        }

        public CiscoException(string message) : base(message)
        {
        }

        public CiscoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CiscoException(string value, int v)
        {
            this.value = value;
            this.v = v;
        }

        protected CiscoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}