using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    public class Flight : IPoco
    {
        public int Id { get; set; }
        public int AirlineCompanyId { get; set; }
        public int OriginCountryCode { get; set; }
        public int DestinationCountryCode { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime LandingTime { get; set; }
        public int RemainingTickets { get; set; }
        public int TicketPrice { get; set; }

        public Flight()
        {

        }

        public Flight(int airlineCompanyId, int originCountryCode, int destinationCountryCode, DateTime departureTime, DateTime landingTime, 
            int remainingTickets, int ticketPrice)
        {
            AirlineCompanyId = airlineCompanyId;
            OriginCountryCode = originCountryCode;
            DestinationCountryCode = destinationCountryCode;
            DepartureTime = departureTime;
            LandingTime = landingTime;
            RemainingTickets = remainingTickets;
            TicketPrice = ticketPrice;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            Flight otherFlight = obj as Flight;
            return this.Id == otherFlight.Id;
        }

        public static bool operator ==(Flight a, Flight b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            return a.Id == b.Id;
        }

        public static bool operator !=(Flight a, Flight b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.Id;
        }

        public override string ToString()
        {
            return $"Flight {Id} of {AirlineCompanyId}, from {OriginCountryCode} to {DestinationCountryCode}";
        }

        public string GetAllDetalies()
        {
            return $"{Id}, {AirlineCompanyId}, {OriginCountryCode}, {DestinationCountryCode}, {DepartureTime}, {LandingTime}, {RemainingTickets}, {TicketPrice}";
        }
    }
}
