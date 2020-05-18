using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class CountryNotFoundException : FlightCenterException
    {
        public CountryNotFoundException()
        {
        }

        public CountryNotFoundException(string message) : base(message)
        {
        }

        public CountryNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CountryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}