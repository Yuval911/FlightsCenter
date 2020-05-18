using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class TicketNotFoundException : FlightCenterException
    {
        public TicketNotFoundException()
        {
        }

        public TicketNotFoundException(string message) : base(message)
        {
        }

        public TicketNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TicketNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}