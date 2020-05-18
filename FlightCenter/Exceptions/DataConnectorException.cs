using System;
using System.Runtime.Serialization;

namespace FlightCenter
{
    [Serializable]
    public class DataConnectorException : FlightCenterException
    {
        public DataConnectorException()
        {
        }

        public DataConnectorException(string message) : base(message)
        {
        }

        public DataConnectorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataConnectorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}