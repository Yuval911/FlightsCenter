using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class CountriesCannotBeChangedException : FlightCenterException
    {
        public CountriesCannotBeChangedException()
        {
        }

        public CountriesCannotBeChangedException(string message) : base(message)
        {
        }

        public CountriesCannotBeChangedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CountriesCannotBeChangedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}