using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class InvalidCustomerException : FlightCenterException
    {
        public InvalidCustomerException()
        {
        }

        public InvalidCustomerException(string message) : base(message)
        {
        }

        public InvalidCustomerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidCustomerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}