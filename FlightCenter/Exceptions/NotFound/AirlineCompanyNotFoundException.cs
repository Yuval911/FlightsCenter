using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class AirlineCompanyNotFoundException : FlightCenterException
    {
        public AirlineCompanyNotFoundException()
        {

        }

        public AirlineCompanyNotFoundException(string message) : base(message)
        {
        }

        public AirlineCompanyNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AirlineCompanyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}