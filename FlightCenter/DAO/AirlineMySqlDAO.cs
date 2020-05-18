using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FlightCenter
{
    /// <summary>
    /// This is the data access class of the airline comapnies.
    /// It conatins the basic CRUD methods and some extras.
    /// </summary>
    public class AirlineMySqlDAO : BasicMySqlDAO, IAirlineDAO
    {
        // Basic CRUD methods:

        public void Add(AirlineCompany t)
        {
            TryCatchDatabaseFunction((conn) => {

                if (!IsValidUser(t))
                    return;

                string query = $"INSERT INTO AirlineCompanies (AIRLINE_NAME, USER_NAME, EMAIL, PASSWORD, COUNTRY_CODE) VALUES " +
                               $"('{t.Name}', '{t.UserName}', '{t.Email}', '{t.Password}', '{t.CountryCode}')";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        /// <summary>
        /// Adds a list of airlines to the database. 
        /// When test mode is true there would be no constraints checking.
        /// </summary>
        public void AddRange(IList<AirlineCompany> list, bool testMode)
        {
            TryCatchDatabaseFunction((conn) => {

                if (!testMode)
                {
                    foreach (AirlineCompany airline in list)
                    {
                        if (!IsValidUser(airline))
                            return;
                    }
                }

                if (list.Count == 0)
                    return;

                string query = "INSERT INTO AirlineCompanies (AIRLINE_NAME, USER_NAME, EMAIL, PASSWORD, COUNTRY_CODE) VALUES ";

                foreach (AirlineCompany airline in list)
                {
                    query += $"('{airline.Name}', '{airline.UserName}', '{airline.Email}', '{airline.Password}', '{airline.CountryCode}'), ";
                }

                query = query.Remove(query.Count() - 2);

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        public AirlineCompany Get(int id)
        {
            AirlineCompany airlineCompany = new AirlineCompany();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM AirlineCompanies WHERE AirlineCompanies.ID = {id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    airlineCompany.Id = Convert.ToInt32(reader[0]);
                    airlineCompany.Name = Convert.ToString(reader[1]);
                    airlineCompany.UserName = Convert.ToString(reader[2]);
                    airlineCompany.Email = Convert.ToString(reader[3]);
                    airlineCompany.Password = Convert.ToString(reader[4]);
                    airlineCompany.CountryCode = Convert.ToInt32(reader[5]);
                }

                reader.Close();

                if (airlineCompany.Id == 0)
                    throw new AirlineCompanyNotFoundException($"Airline Company with ID: {id} doesn't exist");      
            });

            return airlineCompany;
        }

        public IList<AirlineCompany> GetAll()
        {
            List<AirlineCompany> airlineCompanies = new List<AirlineCompany>();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM AirlineCompanies";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    AirlineCompany airlineCompany = new AirlineCompany();

                    airlineCompany.Id = Convert.ToInt32(reader[0]);
                    airlineCompany.Name = Convert.ToString(reader[1]);
                    airlineCompany.UserName = Convert.ToString(reader[2]);
                    airlineCompany.Email = Convert.ToString(reader[3]);
                    airlineCompany.Password = Convert.ToString(reader[4]);
                    airlineCompany.CountryCode = Convert.ToInt32(reader[5]);

                    airlineCompanies.Add(airlineCompany);
                }

                reader.Close();
            });

            return airlineCompanies;
        }

        public void Remove(int id)
        {
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
                    throw new AirlineCompanyNotFoundException($"Airline Company with ID: {id} doesn't exist");

                // Deleting all airline's flights:

                query = $"SELECT * FROM Flights WHERE Flights.AIRLINE_COMPANY_ID = {id}";
                cmd = new MySqlCommand(query, conn);
                reader = cmd.ExecuteReader();

                List<Flight> flights = new List<Flight>();

                while (reader.Read())
                {
                    Flight flight = new Flight();

                    flight.Id = Convert.ToInt32(reader[0]);

                    flights.Add(flight);
                }

                reader.Close();

                foreach (Flight flight in flights)
                {
                    query = $"DELETE FROM Tickets WHERE Tickets.FLIGHT_ID = {flight.Id}";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = $"DELETE FROM Flights WHERE Flights.ID = {flight.Id}";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }

                // Deleting the airline:

                query = $"DELETE FROM AirlineCompanies WHERE AirlineCompanies.ID = {id}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        public void Update(AirlineCompany t)
        {
            TryCatchDatabaseFunction((conn) => {

                if (!IsValidUser(t))
                    return;

                // Checks if an airline company with that ID exists:

                string query = $"SELECT * FROM AirlineCompanies WHERE AirlineCompanies.ID = {t.Id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                AirlineCompany airlineCompany = new AirlineCompany();

                while (reader.Read())
                {
                    airlineCompany.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (airlineCompany.Id == 0)
                    throw new AirlineCompanyNotFoundException($"Airline Company with ID: {t.Id} doesn't exist");

                // Updates the airline details:

                query = $"UPDATE AirlineCompanies SET AIRLINE_NAME = '{t.Name}', USER_NAME = '{t.UserName}', EMAIL = '{t.Email}', PASSWORD = '{t.Password}', " +
                        $"COUNTRY_CODE = '{t.CountryCode}' WHERE AirlineCompanies.ID = {t.Id}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        // Extra methods:

        public AirlineCompany GetAirlineByUserName(string username)
        {
            AirlineCompany airlineCompany = new AirlineCompany();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM AirlineCompanies WHERE AirlineCompanies.USER_NAME = '{username}'";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    airlineCompany.Id = Convert.ToInt32(reader[0]);
                    airlineCompany.Name = Convert.ToString(reader[1]);
                    airlineCompany.UserName = Convert.ToString(reader[2]);
                    airlineCompany.Email = Convert.ToString(reader[3]);
                    airlineCompany.Password = Convert.ToString(reader[4]);
                    airlineCompany.CountryCode = Convert.ToInt32(reader[5]);
                }

                reader.Close();
            });

            if (airlineCompany.Id == 0)
                return null;

            return airlineCompany;
        }

        public IList<AirlineCompany> GetAirlinesByCountryId(int countryId)
        {
            List<AirlineCompany> airlineCompanies = new List<AirlineCompany>();

            TryCatchDatabaseFunction((conn) => {

                // Checking if a country with that ID exist:

                string query = $"SELECT * FROM Countries WHERE Countries.ID = {countryId}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Country country = new Country();

                while (reader.Read())
                {
                    country.Id = Convert.ToInt32(reader[0]);
                    country.Name = Convert.ToString(reader[1]);
                }

                reader.Close();

                if (country.Id == 0)
                    throw new CountryNotFoundException("Country with ID: {id} doesn't exist");

                // Getting the airlines:

                query = $"SELECT * FROM AirlineCompanies WHERE AirlineCompanies.COUNTRY_CODE = {countryId}";
                cmd = new MySqlCommand(query, conn);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    AirlineCompany airlineCompany = new AirlineCompany();

                    airlineCompany.Id = Convert.ToInt32(reader[0]);
                    airlineCompany.Name = Convert.ToString(reader[1]);
                    airlineCompany.UserName = Convert.ToString(reader[2]);
                    airlineCompany.Email = Convert.ToString(reader[3]);
                    airlineCompany.Password = Convert.ToString(reader[4]);
                    airlineCompany.CountryCode = Convert.ToInt32(reader[5]);

                    airlineCompanies.Add(airlineCompany);
                }

                reader.Close();

            });

            return airlineCompanies;
        }

        /// <summary>
        /// This method checks if the given airline company is valid and has all the required properties.
        /// </summary>
        public bool IsValidUser(AirlineCompany t)
        {
            // Validating the fields:

            if (String.IsNullOrEmpty(t.Name))
                throw new InvalidAirlineCompanyException("Airline company must have a name.");
            if (String.IsNullOrEmpty(t.UserName))
                throw new InvalidAirlineCompanyException("Airline company user must have a user name.");
            if (String.IsNullOrEmpty(t.Email))
                throw new InvalidAirlineCompanyException("Airline company user must have an email");
            if (String.IsNullOrEmpty(t.Password))
                throw new InvalidAirlineCompanyException("Airline company user must have a password.");
            if (t.CountryCode == 0)
                throw new InvalidAirlineCompanyException("Airline company must have an origin country");

            if (t.UserName == GlobalConfig.adminUserName)
                throw new UserNameAlreadyExistException("This user name is already taken.");

            // Checking if the user name is taken or not:

            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();

            int customerId = 0;
            int airlineId = 0;

            string query = $"SELECT Customers.ID from Customers WHERE Customers.USER_NAME = '{t.UserName}'";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                customerId = Convert.ToInt32(reader[0]);
            }

            reader.Close();

            query = $"SELECT AirlineCompanies.ID from AirlineCompanies WHERE AirlineCompanies.USER_NAME = '{t.UserName}'";
            cmd = new MySqlCommand(query, conn);

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (Convert.ToInt32(reader[0]) != t.Id)
                    airlineId = Convert.ToInt32(reader[0]);
            }

            reader.Close();

            if (customerId != 0)
                throw new UserNameAlreadyExistException("This user name is already taken.");

            if (airlineId != 0 && airlineId != t.Id)
                throw new UserNameAlreadyExistException("This user name is already taken.");

            // Checking if the company name is taken or not:

            query = $"SELECT AirlineCompanies.ID from AirlineCompanies WHERE AirlineCompanies.AIRLINE_NAME = '{t.Name}'";
            cmd = new MySqlCommand(query, conn);

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if (Convert.ToInt32(reader[0]) != t.Id)
                    airlineId = Convert.ToInt32(reader[0]);
            }

            reader.Close();

            if (airlineId != 0)
                throw new AirlineCompanyNameAlreadyExistException("This airline comapny name already exist.");

            // Checking if the origin country exists:

            query = $"SELECT * FROM Countries WHERE Countries.ID = {t.CountryCode}";
            cmd = new MySqlCommand(query, conn);

            reader = cmd.ExecuteReader();

            Country country = new Country();

            while (reader.Read())
            {
                country.Id = Convert.ToInt32(reader[0]);
            }

            reader.Close();

            if (country.Id == 0)
                throw new CountryNotFoundException($"Country with ID: {t.CountryCode} doesn't exist");

            conn.Close();

            return true;
        }
    }
}
