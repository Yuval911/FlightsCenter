using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Configuration;

namespace FlightCenter
{
    /// <summary>
    /// This is a singelton class that provides the user facades to the Web API.
    /// It's also responsible of moving past flights to the history table.
    /// </summary>
    public class FlightCenterSystem
    {
        private static FlightCenterSystem instance = null;
        private readonly static object key = new object();

        IFlightDAO flightDAO = new FlightMySqlDAO();

        private FlightCenterSystem()
        {
            InitializeFCS();
        }

        private void InitializeFCS()
        {
            MoveFlightsToHistory();

            Task.Run(() =>
            {
                while (true)
                {
                    if (DateTime.Now.Hour == GlobalConfig.flightsArchivingTime.Hour)
                    {
                        MoveFlightsToHistory();
                    }
                    Thread.Sleep(Convert.ToInt32(TimeSpan.FromHours(24).TotalMilliseconds)); // Sleeps for 24 hours
                }
            });
        }

        /// <summary>
        /// Runs once when the program excecutes, and then enters the routine of running
        /// every 24 hours at the defined time. This is being managed by the task below.
        /// </summary>    
        private void MoveFlightsToHistory()
        {
            Logger.Log(LogLevel.Info, "The Flights Center System moved all the past flights and tickets to history.");
            flightDAO.MoveFlightsAndTicketsToHistory();
        }

        /// <summary>
        /// Returns the matching facade according to the login token that was given.
        /// </summary>
        public FacadeBase GetFacade<T>(LoginToken<T> token) where T : IUser
        {
            if (typeof(T) == typeof(Customer))
                return new LoggedInCustomerFacade();
            if (typeof(T) == typeof(AirlineCompany))
                return new LoggedInAirlineFacade();
            if (typeof(T) == typeof(Administrator))
                return new LoggedInAdministratorFacade();

            return new AnonymousUserFacade();
        }

        /// <summary>
        /// Returns an instance of this class using the singleton design pattern.
        /// </summary>
        public static FlightCenterSystem GetInstance()
        {
            if (instance == null)
            {
                lock(key)
                {
                    if (instance == null)
                    {
                        instance = new FlightCenterSystem();
                    }
                }
            }
            return instance;
        }
    }
}
