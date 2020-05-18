using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    interface ILoggedInCustomerFacade
    {
        void CancelTicket(LoginToken<Customer> token, int ticketId);
        void PurchaseTicket(LoginToken<Customer> token, int flightId);
        IList<Flight> GetAllMyFlights(LoginToken<Customer> token);
        IList<JObject> GetAllMyFlightsJson(LoginToken<Customer> token);
    }
}
