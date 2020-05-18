using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This class holds the members that are essential to connect to the MySql Database. 
    /// All MySql data access classes inherit from it.
    /// </summary>
    public class BasicMySqlDAO
    {
        protected readonly string connectionString = GlobalConfig.connString;
        protected readonly string exceptionMessage = GlobalConfig.databaseExceptionMessage;

        /// <summary>
        /// This method using a MySql Connection object to and invokes the function that was sent as an argument.
        /// It uses a try catch to handle exceptions.
        /// </summary>
        protected void TryCatchDatabaseFunction(Action<MySqlConnection> dbFunction)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    dbFunction.Invoke(connection);
                }
                catch (FlightCenterException ex)
                {
                    Logger.Log(LogLevel.Error, ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex);
                    throw new DataConnectorException(exceptionMessage, ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
