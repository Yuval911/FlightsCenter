using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This is the data access class of the flights.
    /// It conatins the basic CRUD methods and many more.
    /// Some of the methods returns a JObject or a list of JObjects, for the client to use with convenience.
    /// </summary>
    public class FlightMySqlDAO : BasicMySqlDAO, IFlightDAO
    {
        // Basic CRUD methods:

        public void Add(Flight t)
        {
            TryCatchDatabaseFunction((conn) => {

                if (!IsValidFlight(t))
                    return;

                string query = $"INSERT INTO Flights (AIRLINE_COMPANY_ID, ORIGIN_COUNTRY_CODE, DESTINATION_COUNTRY_CODE, DEPARTURE_TIME, " +
                               $"LANDING_TIME, REMAINING_TICKETS, TICKET_PRICE) VALUES ('{t.AirlineCompanyId}', '{t.OriginCountryCode}', " +
                               $"'{t.DestinationCountryCode}', '{t.DepartureTime.ToString("yyyy-MM-dd HH:mm:ss")}', '{t.LandingTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
                               $"'{t.RemainingTickets}', '{t.TicketPrice}')";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        /// <summary>
        /// Adds a list of flights to the database. 
        /// When test mode is true there would be no constraints checking.
        /// </summary>
        public void AddRange(IList<Flight> list, bool testMode)
        {
            TryCatchDatabaseFunction((conn) => {

                if (!testMode)
                {
                    foreach (Flight flight in list)
                    {
                        if (!IsValidFlight(flight))
                            return;
                    }
                }

                if (list.Count == 0)
                    return;

                string query = "INSERT INTO Flights (AIRLINE_COMPANY_ID, ORIGIN_COUNTRY_CODE, DESTINATION_COUNTRY_CODE, DEPARTURE_TIME, " +
                               "LANDING_TIME, REMAINING_TICKETS, TICKET_PRICE) VALUES ";

                foreach (Flight flight in list)
                {
                    query += $"('{flight.AirlineCompanyId}', '{flight.OriginCountryCode}', " +
                             $"'{flight.DestinationCountryCode}', '{flight.DepartureTime.ToString("yyyy-MM-dd HH:mm:ss")}', '{flight.LandingTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
                             $"'{flight.RemainingTickets}', '{flight.TicketPrice}'), ";
                }

                query = query.Remove(query.Count() - 2);

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        public Flight Get(int id)
        {
            Flight flight = new Flight();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Flights WHERE Flights.ID = {id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    flight.Id = Convert.ToInt32(reader[0]);
                    flight.AirlineCompanyId = Convert.ToInt32(reader[1]);
                    flight.OriginCountryCode = Convert.ToInt32(reader[2]);
                    flight.DestinationCountryCode = Convert.ToInt32(reader[3]);
                    flight.DepartureTime = Convert.ToDateTime(reader[4]);
                    flight.LandingTime = Convert.ToDateTime(reader[5]);
                    flight.RemainingTickets = Convert.ToInt32(reader[6]);
                    flight.TicketPrice = Convert.ToInt32(reader[7]);
                }

                reader.Close();

                if (flight.Id == 0)
                    throw new FlightNotFoundException($"Flight with ID: {id} doesn't exist.");

            });

            return flight;
        }

        public JObject GetJson(int id)
        {
            JObject flight = new JObject();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT Flights.ID, AirlineCompanies.AIRLINE_NAME, c1.COUNTRY_NAME, c2.COUNTRY_NAME, " +
                               $"Flights.DEPARTURE_TIME, Flights.LANDING_TIME, " +
                               $"Flights.REMAINING_TICKETS, Flights.TICKET_PRICE FROM Flights " +
                               $"JOIN AirlineCompanies ON Flights.AIRLINE_COMPANY_ID = AirlineCompanies.ID " +
                               $"JOIN Countries c1 on Flights.ORIGIN_COUNTRY_CODE = c1.ID " +
                               $"JOIN Countries c2 on Flights.DESTINATION_COUNTRY_CODE = c2.ID " +
                               $"WHERE Flights.ID = {id}";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    flight.Add("Id", Convert.ToInt32(reader[0]));
                    flight.Add("AirlineCompany", Convert.ToString(reader[1]));
                    flight.Add("OriginCountry", Convert.ToString(reader[2]));
                    flight.Add("DestinationCountry", Convert.ToString(reader[3]));
                    flight.Add("DepartureTime", Convert.ToDateTime(reader[4]));
                    flight.Add("LandingTime", Convert.ToDateTime(reader[5]));
                    flight.Add("RemainingTickets", Convert.ToInt32(reader[6]));
                    flight.Add("TicketPrice", Convert.ToInt32(reader[7]));
                }

                reader.Close();

            });

            return flight;
        }

        public IList<Flight> GetAll()
        {
            List<Flight> flights = new List<Flight>();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Flights";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Flight flight = new Flight();

                    flight.Id = Convert.ToInt32(reader[0]);
                    flight.AirlineCompanyId = Convert.ToInt32(reader[1]);
                    flight.OriginCountryCode = Convert.ToInt32(reader[2]);
                    flight.DestinationCountryCode = Convert.ToInt32(reader[3]);
                    flight.DepartureTime = Convert.ToDateTime(reader[4]);
                    flight.LandingTime = Convert.ToDateTime(reader[5]);
                    flight.RemainingTickets = Convert.ToInt32(reader[6]);
                    flight.TicketPrice = Convert.ToInt32(reader[7]);

                    flights.Add(flight);
                }

                reader.Close();

            });

            return flights;
        }

        public void Remove(int id)
        {
            TryCatchDatabaseFunction((conn) => {

                // Checking if the flight exist:

                string query = $"SELECT * FROM Flights WHERE Flights.ID = {id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Flight flight = new Flight();

                while (reader.Read())
                {
                    flight.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (flight.Id == 0)
                    throw new FlightNotFoundException();

                // Deleting the tickets of the flight:

                query = $"DELETE FROM Tickets WHERE Tickets.FLIGHT_ID = {id}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

                // Deleting the flight:

                query = $"DELETE FROM Flights WHERE Flights.ID = {id}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        public void Update(Flight t)
        {
            TryCatchDatabaseFunction((conn) => {

                if (!IsValidFlight(t))
                    return;

                // Checks if a flight with that ID exists:

                string query = $"SELECT * FROM Flights WHERE Flights.ID = {t.Id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Flight flight = new Flight();

                while (reader.Read())
                {
                    flight.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (flight.Id == 0)
                    throw new FlightNotFoundException($"Flight with ID: {t.Id} doesn't exist");

                // Updates the flight details:

                query = $"UPDATE Flights SET AIRLINE_COMPANY_ID = '{t.AirlineCompanyId}', ORIGIN_COUNTRY_CODE = '{t.OriginCountryCode}', " +
                               $"DESTINATION_COUNTRY_CODE = '{t.DestinationCountryCode}', DEPARTURE_TIME = '{t.DepartureTime.ToString("yyyy-MM-dd HH:mm:ss")}', LANDING_TIME = '{t.LandingTime.ToString("yyyy-MM-dd HH:mm:ss")}', " +
                               $"REMAINING_TICKETS = '{t.RemainingTickets}', TICKET_PRICE = '{t.TicketPrice}' WHERE Flights.ID = {t.Id}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        // *** Extra methods: ***

        /// <summary>
        /// Returns a dictionary with flights as keys and remaining tickets as values.
        /// </summary>
        public Dictionary<Flight, int> GetAllFlightsVacancy()    
        {
            Dictionary<Flight, int> flightsVacancy = new Dictionary<Flight, int>();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Flights";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Flight flight = new Flight();

                    flight.Id = Convert.ToInt32(reader[0]);
                    flight.AirlineCompanyId = Convert.ToInt32(reader[1]);
                    flight.OriginCountryCode = Convert.ToInt32(reader[2]);
                    flight.DestinationCountryCode = Convert.ToInt32(reader[3]);
                    flight.DepartureTime = Convert.ToDateTime(reader[4]);
                    flight.LandingTime = Convert.ToDateTime(reader[5]);
                    flight.RemainingTickets = Convert.ToInt32(reader[6]);
                    flight.TicketPrice = Convert.ToInt32(reader[7]);

                    flightsVacancy.Add(flight, flight.RemainingTickets);
                }

                reader.Close();

            });

            return flightsVacancy;
        }

        // Getting flights by single parameter:

        public IList<Flight> GetFlightsByCustomer(Customer customer)
        {
            List<Flight> flights = new List<Flight>();

            TryCatchDatabaseFunction((conn) => {

                // Checks if a customer with that ID exists:

                string query = $"SELECT * FROM Customers WHERE Customers.ID = {customer.Id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Customer c = new Customer();

                while (reader.Read())
                {
                    c.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (c.Id == 0)
                    throw new CustomerNotFoundException($"Customer with ID: {customer.Id} doesn't exists.");

                // Gets the flights:

                query = $"SELECT Flights.ID, Flights.AIRLINE_COMPANY_ID, Flights.ORIGIN_COUNTRY_CODE, " +
                               $"Flights.DESTINATION_COUNTRY_CODE, Flights.DEPARTURE_TIME, Flights.LANDING_TIME, " +
                               $"Flights.REMAINING_TICKETS, Flights.TICKET_PRICE FROM Tickets " +
                               $"JOIN Flights ON Tickets.FLIGHT_ID = Flights.ID " +
                               $"WHERE Tickets.CUSTOMER_ID = {customer.Id}";
                cmd = new MySqlCommand(query, conn);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Flight flight = new Flight();

                    flight.Id = Convert.ToInt32(reader[0]);
                    flight.AirlineCompanyId = Convert.ToInt32(reader[1]);
                    flight.OriginCountryCode = Convert.ToInt32(reader[2]);
                    flight.DestinationCountryCode = Convert.ToInt32(reader[3]);
                    flight.DepartureTime = Convert.ToDateTime(reader[4]);
                    flight.LandingTime = Convert.ToDateTime(reader[5]);
                    flight.RemainingTickets = Convert.ToInt32(reader[6]);
                    flight.TicketPrice = Convert.ToInt32(reader[7]);

                    flights.Add(flight);
                }

                reader.Close();

            });

            return flights;
        }

        public IList<JObject> GetJsonFlightsByCustomer(Customer customer)
        {
            List<JObject> flights = new List<JObject>();

            TryCatchDatabaseFunction((conn) => {

                // Checks if a customer with that ID exists:

                string query = $"SELECT * FROM Customers WHERE Customers.ID = {customer.Id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Customer c = new Customer();

                while (reader.Read())
                {
                    c.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (c.Id == 0)
                    throw new CustomerNotFoundException($"Customer with ID: {customer.Id} doesn't exists.");

                // Gets the flights:

                query = $"SELECT Flights.ID, AirlineCompanies.AIRLINE_NAME, c1.COUNTRY_NAME, c2.COUNTRY_NAME, " +
                               $"Flights.DEPARTURE_TIME, Flights.LANDING_TIME, " +
                               $"Flights.REMAINING_TICKETS, Flights.TICKET_PRICE, Tickets.ID FROM Tickets " +
                               $"JOIN Flights ON Tickets.FLIGHT_ID = Flights.ID " +
                               $"JOIN AirlineCompanies ON Flights.AIRLINE_COMPANY_ID = AirlineCompanies.ID " +
                               $"JOIN Countries c1 on Flights.ORIGIN_COUNTRY_CODE = c1.ID " +
                               $"JOIN Countries c2 on Flights.DESTINATION_COUNTRY_CODE = c2.ID " +
                               $"WHERE Tickets.CUSTOMER_ID = {customer.Id}";
                cmd = new MySqlCommand(query, conn);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    JObject flight = new JObject();

                    flight.Add("Id", Convert.ToInt32(reader[0]));
                    flight.Add("AirlineCompany", Convert.ToString(reader[1]));
                    flight.Add("OriginCountry", Convert.ToString(reader[2]));
                    flight.Add("DestinationCountry", Convert.ToString(reader[3]));
                    flight.Add("DepartureTime", Convert.ToDateTime(reader[4]));
                    flight.Add("LandingTime", Convert.ToDateTime(reader[5]));
                    flight.Add("RemainingTickets", Convert.ToInt32(reader[6]));
                    flight.Add("TicketPrice", Convert.ToInt32(reader[7]));
                    flight.Add("TicketId", Convert.ToInt32(reader[8]));

                    flights.Add(flight);
                }

                reader.Close();

            });

            return flights;
        }

        public IList<Flight> GetFlightsByDepartureDate(DateTime departureDate)
        {
            List<Flight> flights = new List<Flight>();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Flights WHERE Flights.DEPARTURE_TIME BETWEEN '{departureDate.ToString("yyyy-MM-dd")} 00:00:00' AND " +
                               $"'{departureDate.ToString("yyyy-MM-dd")} 23:59:59'";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Flight flight = new Flight();

                    flight.Id = Convert.ToInt32(reader[0]);
                    flight.AirlineCompanyId = Convert.ToInt32(reader[1]);
                    flight.OriginCountryCode = Convert.ToInt32(reader[2]);
                    flight.DestinationCountryCode = Convert.ToInt32(reader[3]);
                    flight.DepartureTime = Convert.ToDateTime(reader[4]);
                    flight.LandingTime = Convert.ToDateTime(reader[5]);
                    flight.RemainingTickets = Convert.ToInt32(reader[6]);
                    flight.TicketPrice = Convert.ToInt32(reader[7]);

                    flights.Add(flight);
                }

                reader.Close();

            });

            return flights;
        }

        public IList<Flight> GetFlightsByDestinationCountryId(int countryCode)
        {
            List<Flight> flights = new List<Flight>();

            TryCatchDatabaseFunction((conn) => {

                // Checks if a country with that ID exists:

                string query = $"SELECT * FROM Countries WHERE Countries.ID = {countryCode}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Country country = new Country();

                while (reader.Read())
                {
                    country.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (country.Id == 0)
                    throw new CountryNotFoundException($"Country with ID: {countryCode} doesn't exists.");

                // Gets the flights:

                query = $"SELECT * FROM Flights WHERE Flights.DESTINATION_COUNTRY_CODE = {countryCode}";
                cmd = new MySqlCommand(query, conn);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Flight flight = new Flight();

                    flight.Id = Convert.ToInt32(reader[0]);
                    flight.AirlineCompanyId = Convert.ToInt32(reader[1]);
                    flight.OriginCountryCode = Convert.ToInt32(reader[2]);
                    flight.DestinationCountryCode = Convert.ToInt32(reader[3]);
                    flight.DepartureTime = Convert.ToDateTime(reader[4]);
                    flight.LandingTime = Convert.ToDateTime(reader[5]);
                    flight.RemainingTickets = Convert.ToInt32(reader[6]);
                    flight.TicketPrice = Convert.ToInt32(reader[7]);

                    flights.Add(flight);
                }

                reader.Close();

            });

            return flights;
        }

        public IList<Flight> GetFlightsByLandingDate(DateTime landingDate)
        {
            List<Flight> flights = new List<Flight>();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Flights WHERE Flights.LANDING_TIME BETWEEN '{landingDate.ToString("yyyy-MM-dd")} 00:00:00' AND " +
                               $"'{landingDate.ToString("yyyy-MM-dd")} 23:59:59'";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Flight flight = new Flight();

                    flight.Id = Convert.ToInt32(reader[0]);
                    flight.AirlineCompanyId = Convert.ToInt32(reader[1]);
                    flight.OriginCountryCode = Convert.ToInt32(reader[2]);
                    flight.DestinationCountryCode = Convert.ToInt32(reader[3]);
                    flight.DepartureTime = Convert.ToDateTime(reader[4]);
                    flight.LandingTime = Convert.ToDateTime(reader[5]);
                    flight.RemainingTickets = Convert.ToInt32(reader[6]);
                    flight.TicketPrice = Convert.ToInt32(reader[7]);

                    flights.Add(flight);
                }

                reader.Close();

            });

            return flights;
        }

        public IList<Flight> GetFlightsByOriginCountryId(int countryCode)
        {
            List<Flight> flights = new List<Flight>();

            TryCatchDatabaseFunction((conn) => {

                // Checks if a country with that ID exists:

                string query = $"SELECT * FROM Countries WHERE Countries.ID = {countryCode}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Country country = new Country();

                while (reader.Read())
                {
                    country.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (country.Id == 0)
                    throw new CountryNotFoundException($"Country with ID: {countryCode} doesn't exists.");

                // Gets the flights:

                query = $"SELECT * FROM Flights WHERE Flights.ORIGIN_COUNTRY_CODE = {countryCode}";
                cmd = new MySqlCommand(query, conn);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Flight flight = new Flight();

                    flight.Id = Convert.ToInt32(reader[0]);
                    flight.AirlineCompanyId = Convert.ToInt32(reader[1]);
                    flight.OriginCountryCode = Convert.ToInt32(reader[2]);
                    flight.DestinationCountryCode = Convert.ToInt32(reader[3]);
                    flight.DepartureTime = Convert.ToDateTime(reader[4]);
                    flight.LandingTime = Convert.ToDateTime(reader[5]);
                    flight.RemainingTickets = Convert.ToInt32(reader[6]);
                    flight.TicketPrice = Convert.ToInt32(reader[7]);

                    flights.Add(flight);
                }

                reader.Close();

            });

            return flights;
        }

        public IList<Flight> GetFlightsByAirlineCompanyId(int id)
        {
            List<Flight> flights = new List<Flight>();

            TryCatchDatabaseFunction((conn) => {

                // Checks if an airline company with that ID exists:

                string query = $"SELECT * FROM AirlineCompanies WHERE AirlineCompanies.ID = {id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                AirlineCompany airlineCompany = new AirlineCompany();

                while (reader.Read())
                {
                    airlineCompany.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (airlineCompany.Id == 0)
                    throw new AirlineCompanyNotFoundException($"Airline Company with ID: {id} doesn't exists.");

                // Gets the flights (including history, and up to 3 days ago):

                DateTime startDate = DateTime.Now.AddDays(-3);

                query = $"SELECT * FROM (SELECT * FROM FlightsHistory UNION ALL SELECT * FROM Flights) AS f " +
                        $"WHERE f.AIRLINE_COMPANY_ID = {id} AND " +
                        $"f.DEPARTURE_TIME > '{startDate.ToString("yyyy-MM-dd hh:mm")}'";
                cmd = new MySqlCommand(query, conn);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Flight flight = new Flight();

                    flight.Id = Convert.ToInt32(reader[0]);
                    flight.AirlineCompanyId = Convert.ToInt32(reader[1]);
                    flight.OriginCountryCode = Convert.ToInt32(reader[2]);
                    flight.DestinationCountryCode = Convert.ToInt32(reader[3]);
                    flight.DepartureTime = Convert.ToDateTime(reader[4]);
                    flight.LandingTime = Convert.ToDateTime(reader[5]);
                    flight.RemainingTickets = Convert.ToInt32(reader[6]);
                    flight.TicketPrice = Convert.ToInt32(reader[7]);

                    flights.Add(flight);
                }
                reader.Close();
            });

            return flights;
        }

        public IList<Flight> GetFlightsByPriceRange(int min, int max)
        {
            List<Flight> flights = new List<Flight>();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Flights WHERE Flights.TICKET_PRICE >= {min} AND Flights.TICKET_PRICE <= {max}";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Flight flight = new Flight();

                    flight.Id = Convert.ToInt32(reader[0]);
                    flight.AirlineCompanyId = Convert.ToInt32(reader[1]);
                    flight.OriginCountryCode = Convert.ToInt32(reader[2]);
                    flight.DestinationCountryCode = Convert.ToInt32(reader[3]);
                    flight.DepartureTime = Convert.ToDateTime(reader[4]);
                    flight.LandingTime = Convert.ToDateTime(reader[5]);
                    flight.RemainingTickets = Convert.ToInt32(reader[6]);
                    flight.TicketPrice = Convert.ToInt32(reader[7]);

                    flights.Add(flight);
                }
                reader.Close();
            });

            return flights;
        }

        // Getting flights by multiple parameters:

        /// <summary>
        /// Returns a list of single flights (one-way) by using multiple parameters.
        /// This method was created for the client's flights search.
        /// </summary>
        public IList<JObject> GetOneWayFlightsByQuery(int originCountryId, int detinationCountryId, DateTime departureDate)
        {
            List<JObject> flights = new List<JObject>();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT Flights.ID, AirlineCompanies.AIRLINE_NAME, c1.COUNTRY_NAME, c2.COUNTRY_NAME, Flights.DEPARTURE_TIME, " +
                               $"Flights.LANDING_TIME, Flights.REMAINING_TICKETS, Flights.TICKET_PRICE FROM Flights " +
                               $"JOIN AirlineCompanies on Flights.AIRLINE_COMPANY_ID = AirlineCompanies.ID " +
                               $"JOIN Countries c1 on Flights.ORIGIN_COUNTRY_CODE = c1.ID " +
                               $"JOIN Countries c2 on Flights.DESTINATION_COUNTRY_CODE = c2.ID " +
                               $"WHERE Flights.ORIGIN_COUNTRY_CODE = {originCountryId} AND " +
                               $"Flights.DESTINATION_COUNTRY_CODE = {detinationCountryId} AND " +
                               $"Flights.DEPARTURE_TIME BETWEEN '{departureDate.ToString("yyyy-MM-dd")} 00:00:00' AND " +
                               $"'{departureDate.ToString("yyyy-MM-dd")} 23:59:59' AND " +
                               $"Flights.REMAINING_TICKETS > 0";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    JObject flight = new JObject();

                    string departureTime = Convert.ToDateTime(reader[4]).ToString("yyyy/MM/dd HH:mm");
                    string landingTime = Convert.ToDateTime(reader[5]).ToString("yyyy/MM/dd HH:mm");

                    flight.Add("flightId", Convert.ToString(reader[0]));
                    flight.Add("airlineName", Convert.ToString(reader[1]));
                    flight.Add("originCountry", Convert.ToString(reader[2]));
                    flight.Add("destinationCountry", Convert.ToString(reader[3]));
                    flight.Add("departureDate", departureTime);
                    flight.Add("landingDate", landingTime);
                    flight.Add("remainingTickets", Convert.ToString(reader[6]));
                    flight.Add("ticketPrice", Convert.ToString(reader[7]));

                    flights.Add(flight);
                }

                reader.Close();
            });

            return flights;
        }

        /// <summary>
        /// Returns a list of pairs of flights (two-way) by using multiple parameters.
        /// This method was created for the cleint's flights search.
        /// </summary>
        public IList<IList<JObject>> GetRoundtripFlightsByQuery(int originCountryId, int detinationCountryId, DateTime departureDate, DateTime returnDate)
        {
            IList<IList<JObject>> flightsPairs = new List<IList<JObject>>();

            List<JObject> departureFlights = new List<JObject>();
            List<JObject> returnFlights = new List<JObject>();

            TryCatchDatabaseFunction((conn) => {

                // Getting the departure flights:

                string query = $"SELECT Flights.ID, AirlineCompanies.AIRLINE_NAME, c1.COUNTRY_NAME, c2.COUNTRY_NAME, Flights.DEPARTURE_TIME, " +
                               $"Flights.LANDING_TIME, Flights.REMAINING_TICKETS, Flights.TICKET_PRICE FROM Flights " +
                               $"JOIN AirlineCompanies on Flights.AIRLINE_COMPANY_ID = AirlineCompanies.ID " +
                               $"JOIN Countries c1 on Flights.ORIGIN_COUNTRY_CODE = c1.ID " +
                               $"JOIN Countries c2 on Flights.DESTINATION_COUNTRY_CODE = c2.ID " +
                               $"WHERE Flights.ORIGIN_COUNTRY_CODE = {originCountryId} AND " +
                               $"Flights.DESTINATION_COUNTRY_CODE = {detinationCountryId} AND " +
                               $"Flights.DEPARTURE_TIME BETWEEN '{departureDate.ToString("yyyy-MM-dd")} 00:00:00' AND " +
                               $"'{departureDate.ToString("yyyy-MM-dd")} 23:59:59' AND " +
                               $"Flights.REMAINING_TICKETS > 0";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    JObject flight = new JObject();

                    string departureTime = Convert.ToDateTime(reader[4]).ToString("yyyy/MM/dd HH:mm");
                    string landingTime = Convert.ToDateTime(reader[5]).ToString("yyyy/MM/dd HH:mm");

                    flight.Add("flightId", Convert.ToString(reader[0]));
                    flight.Add("airlineName", Convert.ToString(reader[1]));
                    flight.Add("originCountry", Convert.ToString(reader[2]));
                    flight.Add("destinationCountry", Convert.ToString(reader[3]));
                    flight.Add("departureDate", departureTime);
                    flight.Add("landingDate", landingTime);
                    flight.Add("remainingTickets", Convert.ToString(reader[6]));
                    flight.Add("ticketPrice", Convert.ToString(reader[7]));

                    departureFlights.Add(flight);
                }

                reader.Close();

                // Getting the return flights:

               query = $"SELECT Flights.ID, AirlineCompanies.AIRLINE_NAME, c1.COUNTRY_NAME, c2.COUNTRY_NAME, Flights.DEPARTURE_TIME, " +
                       $"Flights.LANDING_TIME, Flights.REMAINING_TICKETS, Flights.TICKET_PRICE FROM Flights " +
                       $"JOIN AirlineCompanies on Flights.AIRLINE_COMPANY_ID = AirlineCompanies.ID " +
                       $"JOIN Countries c1 on Flights.ORIGIN_COUNTRY_CODE = c1.ID " +
                       $"JOIN Countries c2 on Flights.DESTINATION_COUNTRY_CODE = c2.ID " +
                       $"WHERE Flights.ORIGIN_COUNTRY_CODE = {detinationCountryId} AND " +
                       $"Flights.DESTINATION_COUNTRY_CODE = {originCountryId} AND " +
                       $"Flights.DEPARTURE_TIME BETWEEN '{returnDate.ToString("yyyy-MM-dd")} 00:00:00' AND " +
                       $"'{returnDate.ToString("yyyy-MM-dd")} 23:59:59' AND " +
                       $"Flights.REMAINING_TICKETS > 0";

                cmd = new MySqlCommand(query, conn);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    JObject flight = new JObject();

                    string departureTime = Convert.ToDateTime(reader[4]).ToString("yyyy/MM/dd HH:mm");
                    string landingTime = Convert.ToDateTime(reader[5]).ToString("yyyy/MM/dd HH:mm");

                    flight.Add("flightId", Convert.ToString(reader[0]));
                    flight.Add("airlineName", Convert.ToString(reader[1]));
                    flight.Add("originCountry", Convert.ToString(reader[2]));
                    flight.Add("destinationCountry", Convert.ToString(reader[3]));
                    flight.Add("departureDate", departureTime);
                    flight.Add("landingDate", landingTime);
                    flight.Add("remainingTickets", Convert.ToString(reader[6]));
                    flight.Add("ticketPrice", Convert.ToString(reader[7]));

                    returnFlights.Add(flight);
                }

                reader.Close();

                flightsPairs = MergeFlightsToPairs(departureFlights, returnFlights);
                
            });

            return flightsPairs;
        }

        /// <summary>
        /// A Helper function for the GetRoundtripFlightsByQuery method. 
        /// It takes two lists of flights and merges them to a list of flights pairs.
        /// </summary>
        IList<IList<JObject>> MergeFlightsToPairs(IList<JObject> departureFlights, IList<JObject> returnFlights)
        {
            IList<IList<JObject>> flightsPairs = new List<IList<JObject>>();

            // Sorting the lists:
            departureFlights = departureFlights.OrderBy(f => f["airlineName"]).ToList();
            returnFlights = returnFlights.OrderBy(f => f["airlineName"]).ToList();

            // The size of flights pairs will be as of the smaller list.
            int numOfPairs = departureFlights.Count > returnFlights.Count ? returnFlights.Count : departureFlights.Count;

            // Merging the flights into pairs collection:
            for (int i = 0; i < numOfPairs; i++)
            {
                flightsPairs.Add(new List<JObject>() { departureFlights[i], returnFlights[i] });
            }

            return flightsPairs;
        }

        // Flights board's methods:

        /// <summary>
        /// Gets all the flights that will land in the next 12 hours, and all the flights that landed 4 hours ago or less.
        /// This method was created for the client's flights board.
        /// </summary>
        public IList<JObject> GetArrivalFlights()
        {
            IList<JObject> flights = new List<JObject>();

            TryCatchDatabaseFunction((conn) => {

                // Getting the start and end time of the query:

                DateTime startTime = DateTime.Now.AddHours(-4);
                DateTime endTime = DateTime.Now.AddHours(12);

                string query = 
                $"SELECT Flights.ID, AirlineCompanies.AIRLINE_NAME, c1.COUNTRY_NAME, c2.COUNTRY_NAME, Flights.LANDING_TIME FROM Flights " +
                $"JOIN AirlineCompanies on Flights.AIRLINE_COMPANY_ID = AirlineCompanies.ID " +
                $"JOIN Countries c1 on Flights.ORIGIN_COUNTRY_CODE = c1.ID " +
                $"JOIN Countries c2 on Flights.DESTINATION_COUNTRY_CODE = c2.ID " +
                $"WHERE Flights.LANDING_TIME >= '{startTime.ToString("yyyy-MM-dd HH:mm:ss")}' AND Flights.LANDING_TIME <= '{endTime.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                $"ORDER BY Flights.LANDING_TIME";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    JObject flight = new JObject();

                    string arrivalTime = Convert.ToDateTime(reader[4]).ToString("yyyy/MM/dd HH:mm");

                    flight.Add("flightId", Convert.ToString(reader[0]));
                    flight.Add("airlineName", Convert.ToString(reader[1]));
                    flight.Add("originCountry", Convert.ToString(reader[2]));
                    flight.Add("destinationCountry", Convert.ToString(reader[3]));
                    flight.Add("time", arrivalTime);

                    flights.Add(flight);
                }

                reader.Close();
            });

            return flights;
        }

        /// <summary>
        /// Gets all the flights that will departure in the next 12 hours.
        /// This method was created for the client's flights board.
        /// </summary>
        public IList<JObject> GetDepartureFlights()
        {
            IList<JObject> flights = new List<JObject>();

            TryCatchDatabaseFunction((conn) => {

                // Getting the start and end time of the query:

                DateTime startTime = DateTime.Now;
                DateTime endTime = DateTime.Now.AddHours(12);

                string query =
                $"SELECT Flights.ID, AirlineCompanies.AIRLINE_NAME, c1.COUNTRY_NAME, c2.COUNTRY_NAME, Flights.DEPARTURE_TIME FROM Flights " +
                $"JOIN AirlineCompanies on Flights.AIRLINE_COMPANY_ID = AirlineCompanies.ID " +
                $"JOIN Countries c1 on Flights.ORIGIN_COUNTRY_CODE = c1.ID " +
                $"JOIN Countries c2 on Flights.DESTINATION_COUNTRY_CODE = c2.ID " +
                $"WHERE Flights.DEPARTURE_TIME >= '{startTime.ToString("yyyy-MM-dd HH:mm:ss")}' AND Flights.DEPARTURE_TIME <= '{endTime.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                $"ORDER BY Flights.DEPARTURE_TIME";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    JObject flight = new JObject();

                    string departureTime = Convert.ToDateTime(reader[4]).ToString("yyyy/MM/dd HH:mm");

                    flight.Add("flightId", Convert.ToString(reader[0]));
                    flight.Add("airlineName", Convert.ToString(reader[1]));
                    flight.Add("originCountry", Convert.ToString(reader[2]));
                    flight.Add("destinationCountry", Convert.ToString(reader[3]));
                    flight.Add("time", departureTime);

                    flights.Add(flight);
                }

                reader.Close();
            });

            return flights;
        }

        /// <summary>
        /// Gets all the flights that landed from 24 hours ago until 3 hours ago and moves them
        /// to the flights history table, not after moving all the tickets of the flight to the tickets history table.
        /// </summary>
        public void MoveFlightsAndTicketsToHistory()
        {
            List<Flight> flights = new List<Flight>();
            DateTime startTime = DateTime.Now.AddDays(-1);
            DateTime endTime = DateTime.Now.AddHours(-3);

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Flights WHERE Flights.LANDING_TIME >= '{startTime.ToString("yyyy-MM-dd HH:mm:ss")}' " +
               $"AND Flights.LANDING_TIME <= '{endTime.ToString("yyyy-MM-dd HH:mm:ss")}' ";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Flight flight = new Flight();

                    flight.Id = Convert.ToInt32(reader[0]);
                    flight.AirlineCompanyId = Convert.ToInt32(reader[1]);
                    flight.OriginCountryCode = Convert.ToInt32(reader[2]);
                    flight.DestinationCountryCode = Convert.ToInt32(reader[3]);
                    flight.DepartureTime = Convert.ToDateTime(reader[4]);
                    flight.LandingTime = Convert.ToDateTime(reader[5]);
                    flight.RemainingTickets = Convert.ToInt32(reader[6]);
                    flight.TicketPrice = Convert.ToInt32(reader[7]);

                    flights.Add(flight);
                }

                reader.Close();

                foreach (Flight flight in flights)
                {
                    query = "INSERT INTO TicketsHistory ( " +
                           $"SELECT * FROM Tickets WHERE Tickets.FLIGHT_ID = {flight.Id} " +
                            "); " +
                           $"DELETE FROM Tickets WHERE Tickets.FLIGHT_ID = {flight.Id}; " + // Moves the tickets to history

                            "INSERT INTO FlightsHistory ( " +
                           $"SELECT * FROM Flights WHERE Flights.ID = {flight.Id}" +
                            "); " +
                           $"DELETE FROM Flights WHERE Flights.ID = {flight.Id}; "; // Moves the flight to history
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }
            });
        }

        /// <summary>
        /// This method checks if the given flight is valid and has all the required properties.
        /// </summary>
        public bool IsValidFlight(Flight t)
        {
            if (t.AirlineCompanyId == 0)
                throw new InvalidFlightException("A flight must have an airline company");
            if (t.OriginCountryCode == 0)
                throw new InvalidFlightException("A flight must have an origin country");
            if (t.DestinationCountryCode == 0)
                throw new InvalidFlightException("A flight must have a destination country");
            if (t.DepartureTime.Year == 1)
                throw new InvalidFlightException("A flight must have a departure time and date");
            if (t.LandingTime.Year == 1)
                throw new InvalidFlightException("A flight must have a landing time and date");

            // Checking if the airline company exists:

            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = $"SELECT * FROM AirlineCompanies WHERE AirlineCompanies.ID = {t.AirlineCompanyId}";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            MySqlDataReader reader = cmd.ExecuteReader();

            AirlineCompany airlineCompany = new AirlineCompany();

            while (reader.Read())
            {
                airlineCompany.Id = Convert.ToInt32(reader[0]);
            }

            reader.Close();

            if (airlineCompany.Id == 0)
                throw new AirlineCompanyNotFoundException($"Airline Company with ID: {t.AirlineCompanyId} doesn't exist");

            // Checking if the origin country exists:

            query = $"SELECT * FROM Countries WHERE Countries.ID = {t.OriginCountryCode}";
            cmd = new MySqlCommand(query, conn);

            reader = cmd.ExecuteReader();

            Country country = new Country();

            while (reader.Read())
            {
                country.Id = Convert.ToInt32(reader[0]);
            }

            reader.Close();

            if (country.Id == 0)
                throw new CountryNotFoundException($"Country with ID: {t.OriginCountryCode} doesn't exist");

            // Checking if the destination country exists:

            query = $"SELECT * FROM Countries WHERE Countries.ID = {t.DestinationCountryCode}";
            cmd = new MySqlCommand(query, conn);

            reader = cmd.ExecuteReader();

            country = new Country();

            while (reader.Read())
            {
                country.Id = Convert.ToInt32(reader[0]);
            }

            reader.Close();

            if (country.Id == 0)
                throw new CountryNotFoundException($"Country with ID: {t.DestinationCountryCode} doesn't exist");

            conn.Close();

            return true;
        }

    }
}
