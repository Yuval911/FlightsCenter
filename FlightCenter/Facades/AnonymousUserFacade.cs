using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This class is a part of set of classes that implements the Facade design pattern.
    /// It bridges between the Web API and the DAOs, and exposes all the available methods of the anonymous user.
    /// </summary>
    public class AnonymousUserFacade : FacadeBase, IAnonymousUserFacade
    {
        public IList<AirlineCompany> GetAllAirlineCompanies()
        {
            Logger.Log(LogLevel.Info, "A List of all Airline Companies was requested.");
            return airlineDAO.GetAll();
        }

        public IList<Flight> GetAllFlights()
        {
            Logger.Log(LogLevel.Info, "A List of all flights was requested.");
            return flightDAO.GetAll();
        }

        public Dictionary<Flight, int> GetAllFlightsVacancy()
        {
            Logger.Log(LogLevel.Info, "A List of all flights vacancy was requested.");
            return flightDAO.GetAllFlightsVacancy();
        }

        public Flight GetFlightById(int id)
        {
            Logger.Log(LogLevel.Info, $"Flight {id} was requested.");
            return flightDAO.Get(id);
        }

        public JObject GetFlightByIdJson(int id)
        {
            Logger.Log(LogLevel.Info, $"Flight {id} was requested.");
            return flightDAO.GetJson(id);
        }

        public IList<Flight> GetFlightsByDepartureDate(DateTime date)
        {
            Logger.Log(LogLevel.Info, "A list of all flights by departure date was requested.");
            return flightDAO.GetFlightsByDepartureDate(date);
        }

        public IList<Flight> GetFlightsByDestinationCountry(int countryId)
        {
            Logger.Log(LogLevel.Info, "A list of all flights by destination country was requested.");
            return flightDAO.GetFlightsByDestinationCountryId(countryId);
        }

        public IList<Flight> GetFlightsByLandingDate(DateTime date)
        {
            Logger.Log(LogLevel.Info, "A list of all flights by landing date was requested.");
            return flightDAO.GetFlightsByLandingDate(date);
        }

        public IList<Flight> GetFlightsByOriginCountry(int countryId)
        {
            Logger.Log(LogLevel.Info, "A list of all flights by origin country was requested.");
            return flightDAO.GetFlightsByOriginCountryId(countryId);
        }

        public IList<Flight> GetFlightsByPriceRange(int min, int max)
        {
            Logger.Log(LogLevel.Info, "A list of all flights by price range was requested.");
            return flightDAO.GetFlightsByPriceRange(min, max);
        }

        public IList<Country> GetAllCountries()
        {
            Logger.Log(LogLevel.Info, "A list of all countries was requested.");
            return countryDAO.GetAll();
        }

        public IList<JObject> GetArrivalFlights()
        {
            Logger.Log(LogLevel.Info, "A list of all arrival flights was requested.");
            return flightDAO.GetArrivalFlights();
        }

        public IList<JObject> GetDepartureFlights()
        {
            Logger.Log(LogLevel.Info, "A list of all departure flights was requested.");
            return flightDAO.GetDepartureFlights();
        }

        public IList<JObject> GetOneWayFlightsByQuery(int originCountryId, int detinationCountryId, DateTime departureDate)
        {
            Logger.Log(LogLevel.Info, "A list of one-way flights by query was requested.");
            return flightDAO.GetOneWayFlightsByQuery(originCountryId, detinationCountryId, departureDate);
        }

        public IList<IList<JObject>> GetRoundtripFlightsByQuery(int originCountryId, int detinationCountryId, DateTime departureDate, DateTime returnDate)
        {
            Logger.Log(LogLevel.Info, "A list of roundtrip flights by query was requested.");
            return flightDAO.GetRoundtripFlightsByQuery(originCountryId, detinationCountryId, departureDate, returnDate);
        }

        public void SignUp(Customer customer)
        {
            Logger.Log(LogLevel.Info, $"A new customer account was created for '{customer.GetFullName()}'.");
            customerDAO.Add(customer);
        }

        public bool CheckUsernameAvailability(string username)
        {
            if (customerDAO.GetCustomerByUsername(username) == null &&
                    airlineDAO.GetAirlineByUserName(username) == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddAirlineToRegisterQueue(AirlineCompany airline)
        {
            Logger.Log(LogLevel.Info, $"A new airline company ('{airline.Name}') has completed the registration and added to the approval queue.");
            airlineRedisDAO.Add(airline);
        }
    }
}
