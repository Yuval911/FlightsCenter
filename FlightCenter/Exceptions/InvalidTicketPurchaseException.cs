using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class InvalidTicketPurchaseException : FlightCenterException
    {
        public InvalidTicketPurchaseException()
        {
        }

        public InvalidTicketPurchaseException(string message) : base(message)
        {
        }

        public InvalidTicketPurchaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidTicketPurchaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}