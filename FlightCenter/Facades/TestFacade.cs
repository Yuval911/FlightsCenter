using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This class is a part of set of classes that implements the Facade design pattern.
    /// It bridges between the outside world and the DAOs, and exposes methods for the DB Generator to use.
    /// </summary>
    public class TestFacade : FacadeBase, ITestFacade
    {
        public IList<Customer> GetAllCustomers()
        {
            return customerDAO.GetAll();
        }
        public IList<AirlineCompany> GetAllAirlineCompanies()
        {
            return airlineDAO.GetAll();
        }
        public IList<Flight> GetAllFlights()
        {
            return flightDAO.GetAll();
        }
        public IList<Ticket> GetAllTickets()
        {
            return ticketDAO.GetAll();
        }

        public void AddRangeOfAirlineCompanies(IList<AirlineCompany> airlines)
        {
            airlineDAO.AddRange(airlines, true);
            if (airlines.Count > 0)
                Logger.Log(LogLevel.Info, $"{airlines.Count} airline companies were added using the DB Generator.");
        }

        public void AddRangeOfCustomers(IList<Customer> customers)
        {
            customerDAO.AddRange(customers, true);
            if (customers.Count > 0)
                Logger.Log(LogLevel.Info, $"{customers.Count} customers was added using the DB Generator.");
        }

        public void AddRangeOfFlights(IList<Flight> flights)
        {
            flightDAO.AddRange(flights, true);
            if (flights.Count > 0)
                Logger.Log(LogLevel.Info, $"{flights.Count} flights were added using the DB Generator.");
        }

        public void AddRangeOfTickets(IList<Ticket> tickets)
        {
            ticketDAO.AddRange(tickets, true);
            if (tickets.Count > 0)
                Logger.Log(LogLevel.Info, $"{tickets.Count} tickets were added using the DB Generator.");
        }

        public void AddTicket(Ticket ticket)
        {
            ticketDAO.Add(ticket);
            Logger.Log(LogLevel.Info, $"A ticket for flight {ticket.FlightId} was added using the DB Generator.");
        }

        public void DeleteAllTables()
        {
            testsDAO.DeleteAllTables();
            Logger.Log(LogLevel.Info, $"All the database tables were truncated using the DB Generator.");
        }
    }
}
