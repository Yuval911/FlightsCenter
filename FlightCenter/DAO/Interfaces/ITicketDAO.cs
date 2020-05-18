using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    public interface ITicketDAO : IBasicDB<Ticket>
    {
        void RemoveTicketsByFlightId(int id);
        void RemoveTicketsByCustomerId(int id);
        IList<Ticket> GetTicketsByAirlineId(int id);
        bool IsValidTicket(Ticket t);
    }
}
