using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Configuration;

namespace FlightCenter
{
    /// <summary>
    /// This class handles the connection to the Redis cache, that stores all the 
    /// </summary>
    public class AirlineRedisDAO
    {
        private readonly string url = WebConfigurationManager.AppSettings["RedisHostURL"];
        private readonly string exceptionMessage = GlobalConfig.databaseExceptionMessage;
        private RedisClient redisClient;

        public AirlineRedisDAO()
        {
            redisClient = new RedisClient(url);
        }

        public void Add(AirlineCompany t)
        {
            TryCatchDatabaseFunction(() =>
            {
                string airlineJson = JsonConvert.SerializeObject(t);

                // Searching for the first available key to store the value:
                int key = 0;
                while (!String.IsNullOrEmpty(redisClient.Get<string>(key.ToString())))
                {
                    key++;
                }

                // Storing the value:
                redisClient.Set(key.ToString(), airlineJson);
            });
        }

        public AirlineCompany Get(int key)
        {
            AirlineCompany airline = new AirlineCompany();

            TryCatchDatabaseFunction(() =>
            {
                string airlineJson = redisClient.Get<string>(key.ToString());

                airline = JsonConvert.DeserializeObject<AirlineCompany>(airlineJson);
            });

            return airline;
        }

        public IList<AirlineCompany> GetAll()
        {
            List<AirlineCompany> airlines = new List<AirlineCompany>();

            TryCatchDatabaseFunction(() =>
            {
                List<string> keys = redisClient.GetAllKeys();

                foreach (string key in keys)
                {
                    string airlineJson = redisClient.Get<string>(key);
                    AirlineCompany airline = JsonConvert.DeserializeObject<AirlineCompany>(airlineJson);
                    airlines.Add(airline);
                }
            });

            return airlines;
        }

        public void Remove(AirlineCompany t)
        {
            TryCatchDatabaseFunction(() =>
            {
                string thisAirline = JsonConvert.SerializeObject(t);

                List<string> keys = redisClient.GetAllKeys();

                foreach (string key in keys)
                {
                    string airlineJson = redisClient.Get<string>(key);
                    if (airlineJson == thisAirline)
                    {
                        bool success = redisClient.Remove(key);
                        if (!success)
                            throw new DataConnectorException($"An error occurred while trying to remove key '{key}' from Redis cache.");
                    }
                }
            });
        }

        public void RemoveAll()
        {
            TryCatchDatabaseFunction(() =>
            {
                List<string> keys = redisClient.GetAllKeys();

                foreach (string key in keys)
                {
                    bool success = redisClient.Remove(key);
                    if (!success)
                        throw new DataConnectorException($"An error occurred while trying to remove key '{key}' from Redis cache.");
                }
            });
        }

        private void TryCatchDatabaseFunction(Action dbFunction)
        {
            using (redisClient)
            {
                try
                {
                    dbFunction.Invoke();
                }
                catch (DataConnectorException ex)
                {
                    Logger.Log(LogLevel.Error, ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex);
                    throw new DataConnectorException(exceptionMessage, ex);
                }
            }
        }
    }
}
