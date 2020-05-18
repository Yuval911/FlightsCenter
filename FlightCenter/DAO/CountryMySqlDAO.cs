using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This is the data access class of the airline comapnies.
    /// It conatins only get methods, by design.
    /// </summary>
    public class CountryMySqlDAO : BasicMySqlDAO, ICountryDAO
    {
        public Country Get(int id)
        {
            Country country = new Country();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Countries WHERE Countries.ID = {id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    country.Id = Convert.ToInt32(reader[0]);
                    country.Name = Convert.ToString(reader[1]);
                }

                reader.Close();

                if (country.Id == 0)
                    throw new CountryNotFoundException($"Country with ID: {id} doesn't exist");

            });

            return country;
        }

        public IList<Country> GetAll()
        {
            List<Country> countries = new List<Country>();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Countries";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Country country = new Country();

                    country.Id = Convert.ToInt32(reader[0]);
                    country.Name = Convert.ToString(reader[1]);

                    countries.Add(country);
                }

                reader.Close();

            });

            return countries;
        }

        /// <summary>
        /// This function is not implemented by design.
        /// This application does not allow to add countries.
        /// </summary>
        public void Add(Country t)
        {
            throw new CountriesCannotBeChangedException("It is not possible to add, update or remove a country");
        }

        /// <summary>
        /// This function is not implemented by design.
        /// This application does not allow to add countries.
        /// </summary>
        public void AddRange(IList<Country> list, bool testMode)
        {
            throw new CountriesCannotBeChangedException("It is not possible to add, update or remove a country");
        }

        /// <summary>
        /// This function is not implemented by design.
        /// This application does not allow to remove countries.
        /// </summary>
        public void Remove(int id)
        {
            throw new CountriesCannotBeChangedException("It is not possible to add, update or remove a country");
        }

        /// <summary>
        /// This function is not implemented by design.
        /// This application does not allow to update countries.
        /// </summary>
        public void Update(Country t)
        {
            throw new CountriesCannotBeChangedException("It is not possible to add, update or remove a country");
        }
    }
}
