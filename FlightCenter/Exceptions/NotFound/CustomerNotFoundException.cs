using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class CustomerNotFoundException : FlightCenterException
    {
        public CustomerNotFoundException()
        {
        }

        public CustomerNotFoundException(string message) : base(message)
        {
        }

        public CustomerNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CustomerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}