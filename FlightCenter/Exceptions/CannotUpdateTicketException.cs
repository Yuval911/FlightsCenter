using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    internal class CannotUpdateTicketException : FlightCenterException
    {
        public CannotUpdateTicketException()
        {
        }

        public CannotUpdateTicketException(string message) : base(message)
        {
        }

        public CannotUpdateTicketException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotUpdateTicketException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}