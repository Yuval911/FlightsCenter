using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightCenter;
using System.Collections.Generic;

namespace UnitTests
{
    /// <summary>
    /// This class contains all tests of the anonymous user.
    /// </summary>
    [TestClass]
    public class AnonymousTests
    {
        [TestMethod]
        public void I_AnonymousFacadeTest()
        {
            new TestFacade().DeleteAllTables();

            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            AnonymousUserFacade facade = fcs.GetFacade<Anonymous>(null) as AnonymousUserFacade;

            #region Sign up

            facade.SignUp(new Customer("Joe", "Fin", "jf", "Joe@Fin.com", "111", "China", "100", "4580"));

            LoginToken<Administrator> token = new LoginToken<Administrator>() { User = new Administrator() };

            Customer testCustomer = new LoggedInAdministratorFacade().GetAllCustomers(token)[0];

            Assert.AreEqual(testCustomer.FirstName, "Joe");
            Assert.AreEqual(testCustomer.LastName, "Fin");
            Assert.AreEqual(testCustomer.UserName, "jf");
            Assert.AreEqual(testCustomer.Password, "111");
            Assert.AreEqual(testCustomer.Address, "China");
            Assert.AreEqual(testCustomer.PhoneNo, "100");
            Assert.AreEqual(testCustomer.CreditCardNo, "4580");

            #endregion

            #region Get all airlines

            new LoggedInAdministratorFacade().CreateNewAirline(token, new AirlineCompany("Air One", "airone", "a@o.com", "555", 1));
            new LoggedInAdministratorFacade().CreateNewAirline(token, new AirlineCompany("Travel Air", "travelair", "t@a.com", "222", 1));

            IList<AirlineCompany> airlines = facade.GetAllAirlineCompanies();

            Assert.AreEqual(airlines[0].Name, "Air One");
            Assert.AreEqual(airlines[0].UserName, "airone");
            Assert.AreEqual(airlines[0].Password, "555");
            Assert.AreEqual(airlines[0].CountryCode, 1);

            Assert.AreEqual(airlines[1].Name, "Travel Air");
            Assert.AreEqual(airlines[1].UserName, "travelair");
            Assert.AreEqual(airlines[1].Password, "222");
            Assert.AreEqual(airlines[1].CountryCode, 1);

            #endregion

            #region Get all flights

            LoginToken<AirlineCompany> airlineToken = new LoginToken<AirlineCompany>() { User = new AirlineCompany() };

            new LoggedInAirlineFacade().CreateFlight(airlineToken, new Flight(airlines[0].Id, 1, 2, new DateTime(2010, 10, 10), new DateTime(2010, 10, 10), 5, 99));
            new LoggedInAirlineFacade().CreateFlight(airlineToken, new Flight(airlines[1].Id, 2, 3, new DateTime(2012, 12, 12), new DateTime(2012, 12, 12), 0, 99));

            IList<Flight> flights = facade.GetAllFlights();

            Assert.AreEqual(flights[0].AirlineCompanyId, airlines[0].Id);
            Assert.AreEqual(flights[0].OriginCountryCode, 1);
            Assert.AreEqual(flights[0].DestinationCountryCode, 2);
            Assert.AreEqual(flights[0].DepartureTime, new DateTime(2010, 10, 10));
            Assert.AreEqual(flights[0].LandingTime, new DateTime(2010, 10, 10));
            Assert.AreEqual(flights[0].RemainingTickets, 5);
            Assert.AreEqual(flights[0].TicketPrice, 99);

            Assert.AreEqual(flights[1].AirlineCompanyId, airlines[1].Id);
            Assert.AreEqual(flights[1].OriginCountryCode, 2);
            Assert.AreEqual(flights[1].DestinationCountryCode, 3);
            Assert.AreEqual(flights[1].DepartureTime, new DateTime(2012, 12, 12));
            Assert.AreEqual(flights[1].LandingTime, new DateTime(2012, 12, 12));
            Assert.AreEqual(flights[1].RemainingTickets, 0);
            Assert.AreEqual(flights[1].TicketPrice, 99);

            #endregion

            #region Get all flights vacancy

            Dictionary<Flight, int> flightsVacancy = facade.GetAllFlightsVacancy();

            Flight[] flightsArray = new Flight[2];
            int[] vacancyArray = new int[2];

            flightsVacancy.Keys.CopyTo(flightsArray, 0);
            flightsVacancy.Values.CopyTo(vacancyArray, 0);

            Assert.AreEqual(flightsArray[0].Id, flights[0].Id);
            Assert.AreEqual(vacancyArray[0], flights[0].RemainingTickets);

            Assert.AreEqual(flightsArray[1].Id, flights[1].Id);
            Assert.AreEqual(vacancyArray[1], flights[1].RemainingTickets);

            #endregion

            #region Get flight by id

            Flight flight = facade.GetFlightById(flights[0].Id);

            Assert.AreEqual(flight.AirlineCompanyId, airlines[0].Id);
            Assert.AreEqual(flight.OriginCountryCode, 1);
            Assert.AreEqual(flight.DestinationCountryCode, 2);
            Assert.AreEqual(flight.DepartureTime, new DateTime(2010, 10, 10));
            Assert.AreEqual(flight.LandingTime, new DateTime(2010, 10, 10));
            Assert.AreEqual(flight.RemainingTickets, 5);

            #endregion

            #region Get flights by departure date

            IList<Flight> selectedFlights = facade.GetFlightsByDepartureDate(new DateTime(2010, 10, 10));

            Assert.AreEqual(selectedFlights[0].AirlineCompanyId, airlines[0].Id);
            Assert.AreEqual(selectedFlights[0].OriginCountryCode, 1);
            Assert.AreEqual(selectedFlights[0].DestinationCountryCode, 2);
            Assert.AreEqual(selectedFlights[0].DepartureTime, new DateTime(2010, 10, 10));
            Assert.AreEqual(selectedFlights[0].LandingTime, new DateTime(2010, 10, 10));
            Assert.AreEqual(selectedFlights[0].RemainingTickets, 5);

            #endregion

            #region Get flights by landing date

            selectedFlights = facade.GetFlightsByLandingDate(new DateTime(2012, 12, 12));

            Assert.AreEqual(selectedFlights[0].AirlineCompanyId, airlines[1].Id);
            Assert.AreEqual(selectedFlights[0].OriginCountryCode, 2);
            Assert.AreEqual(selectedFlights[0].DestinationCountryCode, 3);
            Assert.AreEqual(selectedFlights[0].DepartureTime, new DateTime(2012, 12, 12));
            Assert.AreEqual(selectedFlights[0].LandingTime, new DateTime(2012, 12, 12));
            Assert.AreEqual(selectedFlights[0].RemainingTickets, 0);

            #endregion

            #region Get flights by origin country

            selectedFlights = facade.GetFlightsByOriginCountry(1);

            Assert.AreEqual(selectedFlights[0].AirlineCompanyId, airlines[0].Id);
            Assert.AreEqual(selectedFlights[0].OriginCountryCode, 1);
            Assert.AreEqual(selectedFlights[0].DestinationCountryCode, 2);
            Assert.AreEqual(selectedFlights[0].DepartureTime, new DateTime(2010, 10, 10));
            Assert.AreEqual(selectedFlights[0].LandingTime, new DateTime(2010, 10, 10));
            Assert.AreEqual(selectedFlights[0].RemainingTickets, 5);

            #endregion

            #region Get flights by destination country

            selectedFlights = facade.GetFlightsByDestinationCountry(3);

            Assert.AreEqual(selectedFlights[0].AirlineCompanyId, airlines[1].Id);
            Assert.AreEqual(selectedFlights[0].OriginCountryCode, 2);
            Assert.AreEqual(selectedFlights[0].DestinationCountryCode, 3);
            Assert.AreEqual(selectedFlights[0].DepartureTime, new DateTime(2012, 12, 12));
            Assert.AreEqual(selectedFlights[0].LandingTime, new DateTime(2012, 12, 12));
            Assert.AreEqual(selectedFlights[0].RemainingTickets, 0);

            #endregion
        }
    }
}
