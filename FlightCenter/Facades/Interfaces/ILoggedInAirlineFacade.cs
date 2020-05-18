using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    interface ILoggedInAirlineFacade
    {
        IList<Flight> GetAllMyFlights(LoginToken<AirlineCompany> token);
        IList<Ticket> GetAllTickets(LoginToken<AirlineCompany> token);
        void ChangeMyPassword(LoginToken<AirlineCompany> token, string oldPassword, string newPassword);
        void CreateFlight(LoginToken<AirlineCompany> token, Flight flight);
        void ModifyAirlineDetails(LoginToken<AirlineCompany> token, AirlineCompany airline);
        void UpdateFlight(LoginToken<AirlineCompany> token, Flight flight);
    }
}
