using System;
using System.Runtime.Serialization;

namespace plugins
{
    [Serializable]
    internal class FaultException<T> : Exception
    {
        public FaultException()
        {
        }

        public FaultException(string message) : base(message)
        {
        }

        public FaultException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FaultException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}