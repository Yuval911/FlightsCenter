using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class InvalidFlightException : FlightCenterException
    {
        public InvalidFlightException()
        {
        }

        public InvalidFlightException(string message) : base(message)
        {
        }

        public InvalidFlightException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidFlightException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}