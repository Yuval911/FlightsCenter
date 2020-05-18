using FlightCenter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DBGenerator
{
    /// <summary>
    /// This class handles the data generating.
    /// </summary>
    public class DataGenerator
    {
        private static Dictionary<int, int[]> countryCoordinates = new Dictionary<int, int[]>();

        static DataGenerator()
        {
            InitializeCountryCoordinates();
        }

        /// <summary>
        /// Initializing the locations of the countries (Read the comment in the bottom of the file to understand its purpose).
        /// </summary>
        public static void InitializeCountryCoordinates()
        {
            countryCoordinates.Add(1, new int[2] { 10, 2 }); // Israel
            countryCoordinates.Add(2, new int[2] { 9, 2 }); // Jordan
            countryCoordinates.Add(3, new int[2] { 11, 1 }); // Egypt
            countryCoordinates.Add(4, new int[2] { 11, 3 }); // Turkey
            countryCoordinates.Add(5, new int[2] { 12, 2 }); // Greece
            countryCoordinates.Add(6, new int[2] { 13, 4 }); // Italy
            countryCoordinates.Add(7, new int[2] { 13, 5 }); // Germany
            countryCoordinates.Add(8, new int[2] { 14, 5 }); // France
            countryCoordinates.Add(9, new int[2] { 15, 6 }); // England
            countryCoordinates.Add(10, new int[2] { 7, 0 }); // India
            countryCoordinates.Add(11, new int[2] { 8, 7 }); // Russia
            countryCoordinates.Add(12, new int[2] { 4, 3 }); // China
            countryCoordinates.Add(13, new int[2] { 0, 3 }); // Japan
            countryCoordinates.Add(14, new int[2] { 22, 2 }); // USA
        }

        /// <summary>
        /// Gets a list of 1000 random customers from a dedicated text file, deserializes them 
        /// and returns a list of "n" customers.
        /// </summary>
        public static IList<Customer> GetRandomCustomersList(int n)
        {
            // This file contains 1000 customers.
            string jsonString = Properties.Resources.customersJson;

            List<Customer> randomCustomers = JsonConvert.DeserializeObject<List<Customer>>(jsonString);

            List<Customer> customers = new List<Customer>();

            for (int i = 0; i < n; i++)
            {
                customers.Add(randomCustomers[i]);
            }

            return customers;
        }

        /// <summary>
        /// Gets a list of 1000 random companies from a dedicated text file, deserializes them 
        /// and returns a list of "n" airline companies.
        /// </summary>
        public static IList<AirlineCompany> GetRandomAirlineCompaniesList(int n)
        {
            // This file contains 500 companies.
            string jsonString = Properties.Resources.airlinesJson;

            List<AirlineCompany> randomAirlines = JsonConvert.DeserializeObject<List<AirlineCompany>>(jsonString);

            List<AirlineCompany> airlines = new List<AirlineCompany>();

            for (int i=0; i < n; i++)
            {
                airlines.Add(randomAirlines[i]);
            }

            return airlines;
        }

        /// <summary>
        /// Generates random flights for each airline company.
        /// Read the comment in the bottom of the file to understand its logics.
        /// </summary>
        public static IList<Flight> GetRandomFlights(int flightsPerAirline)
        {
            TestFacade facade = new TestFacade();

            // Getting all airline companies.
            IList<AirlineCompany> airlines = facade.GetAllAirlineCompanies();

            IList<Flight> flights = new List<Flight>();

            Random rnd = new Random();

            DateTime startDate;

            // Defining the initial time of the flights generation.
            DateTime sDate = DateTime.Now.AddDays(-1);
            startDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, sDate.Hour, 0, 0);

            foreach (AirlineCompany airline in airlines)
            {
                for (int i=0; i < flightsPerAirline; i++)
                {       
                    Flight flight = new Flight();
                    flight.AirlineCompanyId = airline.Id;

                    // Choosing random countries
                    if (i%2 == 0)
                    {
                        flight.OriginCountryCode = airline.CountryCode;
                        do
                        {
                            flight.DestinationCountryCode = rnd.Next(1, 15);
                        }
                        while (flight.DestinationCountryCode == flight.OriginCountryCode);
                    }
                    else
                    {
                        flight.DestinationCountryCode = airline.CountryCode;
                        do
                        {
                            flight.OriginCountryCode = rnd.Next(1, 15);
                        }
                        while (flight.DestinationCountryCode == flight.OriginCountryCode); 
                    }

                    // Generating flight schedule:

                    flight.DepartureTime = startDate.AddMinutes(i * 60);

                    int flightTime = GetFlightDuration(flight.OriginCountryCode, flight.DestinationCountryCode);

                    flight.LandingTime = flight.DepartureTime.AddHours(flightTime);

                    if (rnd.Next(1, 3) == 1)
                    {
                        if (rnd.Next(1, 3) == 1)
                            flight.LandingTime = flight.LandingTime.AddMinutes(30);
                        else
                            flight.LandingTime = flight.LandingTime.AddMinutes(-30);
                    }

                    // Generating tickets number and ticket price:

                    flight.RemainingTickets = rnd.Next(10, 150);

                    flight.TicketPrice = flightTime * 50 - 1;

                    flights.Add(flight);
                }
            }

            return flights;
        }

        /// <summary>
        /// Generates flight tickets randomly for each customer.
        /// </summary>
        public static IList<Ticket> GetRandomTickets(int ticketsPerCustomer)
        {
            TestFacade facade = new TestFacade();

            IList<Customer> customers = facade.GetAllCustomers();
            IList<Flight> flights = facade.GetAllFlights();

            IList<Ticket> tickets = new List<Ticket>();

            foreach (Customer customer in customers)
            {
                for (int i = 0; i < ticketsPerCustomer; i++)
                {
                    if (flights[i].DepartureTime < DateTime.Now.AddDays(-1))
                        continue;

                    Ticket ticket = new Ticket
                    {
                        CustomerId = customer.Id,
                        FlightId = flights[i].Id
                    };
                    tickets.Add(ticket);
                }
            }

            return tickets;
        }

        /// <summary>
        /// This is a helper function for the GetRandomFlights method.
        /// It calculates the flight duration using the coordinates of the origin and destination countries.
        /// </summary>
        private static int GetFlightDuration(int originCountryId, int destinationCountryId)
        {
            int[] originLocation = countryCoordinates[originCountryId];
            int[] destinationLocation = countryCoordinates[destinationCountryId];

            double distancePow2 = Math.Pow(originLocation[0] - destinationLocation[0], 2) + Math.Pow(originLocation[1] - destinationLocation[1], 2);
            double distance = Math.Sqrt(distancePow2);
            int distanceRounded = Convert.ToInt32(Math.Floor(distance));

            return distanceRounded;
        }
        

        /*  Flights generating logics:
         * 
         *  ()  The flights generating "period" will always start one day ago, at 00:00. The length of that period depend on
         *      The number of the generated flights.
         *  
         *  ()  Each airline company will have an equal number of generated flights.
         * 
         *  ()  Every airline will fly TO or FROM their origin country only.
         *  
         *  ()  The gap between each generated airline flight is 30 minutes.
         * 
         *  ()  Flight duration will be calculated using the following logic: Every country has its coordinates in a defined
         *      cartesian coordinate system. The 'GetFlightDuration' method will calculate the distance between these two coordinates
         *      and determine the flight duration.
         *   
         *  ()  The ticket price will depend on the flight duration. The longer the flight, the higher the price.
         * 
        */

    }

}
