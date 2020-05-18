using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class NoTicketsRemainingException : FlightCenterException
    {
        public NoTicketsRemainingException()
        {
        }

        public NoTicketsRemainingException(string message) : base(message)
        {
        }

        public NoTicketsRemainingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoTicketsRemainingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}