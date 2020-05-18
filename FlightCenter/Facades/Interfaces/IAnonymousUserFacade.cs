using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    interface IAnonymousUserFacade
    {
        IList<AirlineCompany> GetAllAirlineCompanies();
        IList<Flight> GetAllFlights();
        Dictionary<Flight,int> GetAllFlightsVacancy();
        IList<Flight> GetFlightsByDepartureDate(DateTime date);
        IList<Flight> GetFlightsByDestinationCountry(int countryId);
        IList<Flight> GetFlightsByLandingDate(DateTime date);
        IList<Flight> GetFlightsByOriginCountry(int countryId);
        IList<Flight> GetFlightsByPriceRange(int min, int max);
        Flight GetFlightById(int id);

        JObject GetFlightByIdJson(int id);
        IList<JObject> GetOneWayFlightsByQuery(int originCountryId, int detinationCountryId, DateTime departureDate);
        IList<IList<JObject>> GetRoundtripFlightsByQuery(int originCountryId, int detinationCountryId, DateTime departureDate, DateTime returnDate);

        IList<JObject> GetArrivalFlights();
        IList<JObject> GetDepartureFlights();

        void SignUp(Customer customer);
        void AddAirlineToRegisterQueue(AirlineCompany airline);
    }
}
