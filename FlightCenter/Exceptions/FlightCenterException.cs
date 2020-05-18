using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class FlightCenterException : ApplicationException
    {
        public FlightCenterException()
        {

        }

        public FlightCenterException(string message) : base(message)
        {

        }

        public FlightCenterException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected FlightCenterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}