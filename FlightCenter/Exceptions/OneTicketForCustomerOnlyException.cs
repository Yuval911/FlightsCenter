using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class OneTicketForCustomerOnlyException : FlightCenterException
    {
        public OneTicketForCustomerOnlyException()
        {
        }

        public OneTicketForCustomerOnlyException(string message) : base(message)
        {
        }

        public OneTicketForCustomerOnlyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OneTicketForCustomerOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}