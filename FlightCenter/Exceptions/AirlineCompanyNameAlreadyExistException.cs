using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class AirlineCompanyNameAlreadyExistException : FlightCenterException
    {
        public AirlineCompanyNameAlreadyExistException()
        {
        }

        public AirlineCompanyNameAlreadyExistException(string message) : base(message)
        {
        }

        public AirlineCompanyNameAlreadyExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AirlineCompanyNameAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}