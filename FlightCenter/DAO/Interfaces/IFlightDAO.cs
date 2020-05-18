using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    public interface IFlightDAO : IBasicDB<Flight>
    {
        Dictionary<Flight, int> GetAllFlightsVacancy();
        IList<Flight> GetFlightsByOriginCountryId(int countryCode);
        IList<Flight> GetFlightsByDestinationCountryId(int countryCode);
        IList<Flight> GetFlightsByDepartureDate(DateTime departureDate);
        IList<Flight> GetFlightsByLandingDate(DateTime landingDate);
        IList<Flight> GetFlightsByCustomer(Customer customer);
        IList<Flight> GetFlightsByAirlineCompanyId(int id);
        IList<Flight> GetFlightsByPriceRange(int min, int max);

        JObject GetJson(int id);
        IList<JObject> GetOneWayFlightsByQuery(int originCountryId, int detinationCountryId, DateTime departureDate);
        IList<IList<JObject>> GetRoundtripFlightsByQuery(int originCountryId, int detinationCountryId, DateTime departureDate, DateTime returnDate);
        IList<JObject> GetArrivalFlights();
        IList<JObject> GetDepartureFlights();
        IList<JObject> GetJsonFlightsByCustomer(Customer customer);

        void MoveFlightsAndTicketsToHistory();
        bool IsValidFlight(Flight t);
    }
}
