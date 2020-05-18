using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    public abstract class FacadeBase
    {
        protected IAirlineDAO airlineDAO = new AirlineMySqlDAO();
        protected ICountryDAO countryDAO = new CountryMySqlDAO();
        protected ICustomerDAO customerDAO = new CustomerMySqlDAO();
        protected IFlightDAO flightDAO = new FlightMySqlDAO();
        protected ITicketDAO ticketDAO = new TicketMySqlDAO();
        protected ITestsDAO testsDAO = new TestsMySqlDAO();
        protected AirlineRedisDAO airlineRedisDAO = new AirlineRedisDAO();
    }
}
