using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    interface ITestFacade
    {
        void DeleteAllTables();
        IList<Customer> GetAllCustomers();
        IList<AirlineCompany> GetAllAirlineCompanies();
        IList<Flight> GetAllFlights();
        IList<Ticket> GetAllTickets();
        void AddRangeOfCustomers(IList<Customer> customers);
        void AddRangeOfAirlineCompanies(IList<AirlineCompany> airlines);
        void AddRangeOfFlights(IList<Flight> flights);
        void AddRangeOfTickets(IList<Ticket> tickets);
        void AddTicket(Ticket ticket);
    }
}
