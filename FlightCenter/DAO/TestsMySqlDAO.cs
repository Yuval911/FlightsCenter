using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This class being used by the unit tests and the data generator.
    /// It has one method that wipes all the tables (except the countries table).
    /// </summary>
    public class TestsMySqlDAO : ITestsDAO
    {
        private readonly string connectionString = GlobalConfig.connString;
        private readonly string exceptionMessage = GlobalConfig.databaseExceptionMessage;

        public void DeleteAllTables()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    string query = $"DELETE FROM Tickets; DELETE FROM TicketsHistory; " +
                                   $"DELETE FROM Flights; DELETE FROM FlightsHistory;" +
                                   $"DELETE FROM Customers; DELETE FROM AirlineCompanies;";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new DataConnectorException(exceptionMessage, ex);
                }

                connection.Close();
            }
        }
    }
}
