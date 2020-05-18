using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class InvalidAirlineCompanyException : FlightCenterException
    {
        public InvalidAirlineCompanyException()
        {
        }

        public InvalidAirlineCompanyException(string message) : base(message)
        {
        }

        public InvalidAirlineCompanyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidAirlineCompanyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}