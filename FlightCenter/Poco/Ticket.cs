using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    public class Ticket : IPoco
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int CustomerId { get; set; }

        public Ticket()
        {

        }

        public Ticket(int flightId, int customerId)
        {
            FlightId = flightId;
            CustomerId = customerId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            Ticket otherTicket = obj as Ticket;
            return this.Id == otherTicket.Id;
        }

        public static bool operator ==(Ticket a, Ticket b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            return a.Id == b.Id;
        }

        public static bool operator !=(Ticket a, Ticket b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.Id;
        }

        public override string ToString()
        {
            return $"Ticket {Id} of flight {FlightId}, of {CustomerId}";
        }
    }
}
