using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class InvalidCountryException : FlightCenterException
    {
        public InvalidCountryException()
        {
        }

        public InvalidCountryException(string message) : base(message)
        {
        }

        public InvalidCountryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidCountryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}