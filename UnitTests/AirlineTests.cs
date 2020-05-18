using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightCenter;
using System.Collections.Generic;

namespace UnitTests
{
    /// <summary>
    /// This class contains all tests of the airline company.
    /// </summary>
    [TestClass]
    public class AirlineTests
    {
        /// <summary>
        /// Testing the login procedure.
        /// </summary>
        [TestMethod]
        public void AirlineLoginTest()
        {
            new TestFacade().DeleteAllTables();

            LoginToken<Administrator> token = new LoginToken<Administrator>() { User = new Administrator() };

            new LoggedInAdministratorFacade().CreateNewAirline(token, new AirlineCompany("Air One", "airone", "a@o.com", "555", 1));

            // Login
            ILoginService loginService = new LoginService();
            LoginToken<AirlineCompany> loginToken = new LoginToken<AirlineCompany>();

            // Inserting wrong credntials should return a null token
            loginService.TryAirlineLogin("WrongUserName", "WrongPassword", out loginToken); 

            Assert.AreEqual(loginToken, null);

            // Valid login, should return an airline token
            loginService.TryAirlineLogin("airone", "555", out loginToken); 

            Assert.AreEqual(loginToken.User.GetType(), new AirlineCompany().GetType());

            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            // Getting the facade
            LoggedInAirlineFacade facade = fcs.GetFacade<AirlineCompany>(loginToken) as LoggedInAirlineFacade;

            Assert.AreNotEqual(facade.GetType(), null);
        }

        /// <summary>
        /// Testing airline facade's methods.
        /// </summary>
        [TestMethod]
        public void AirlineFacadeTest()
        {
            new TestFacade().DeleteAllTables();

            AirlineCompany testAirline = new AirlineCompany("Air One", "airone", "a@o.com", "555", 1);

            LoginToken<Administrator> token = new LoginToken<Administrator>() { User = new Administrator() };

            new LoggedInAdministratorFacade().CreateNewAirline(token, testAirline);

            testAirline.Id = new AnonymousUserFacade().GetAllAirlineCompanies()[0].Id;

            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            LoginToken<AirlineCompany> loginToken = new LoginToken<AirlineCompany>()
            {
                User = testAirline
            };

            LoggedInAirlineFacade facade = fcs.GetFacade<AirlineCompany>(loginToken) as LoggedInAirlineFacade;

            #region Create flight

            IList<AirlineCompany> airlines = new AnonymousUserFacade().GetAllAirlineCompanies();

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.CreateFlight(null, new Flight(airlines[0].Id, 1, 2, DateTime.Now, DateTime.Now.AddHours(3), 5, 99));
                // Null token, should cause an exception to be thrown
            });

            // Airline company constraints:

            Assert.ThrowsException<InvalidFlightException>(() =>
            {
                facade.CreateFlight(loginToken, new Flight(0, 1, 2, new DateTime(2011, 11, 11), new DateTime(2011, 11, 11), 5, 99));
                // Airline company Id is 0, should cause an exception to be thrown
            });

            Assert.ThrowsException<AirlineCompanyNotFoundException>(() =>
            {
                facade.CreateFlight(loginToken, new Flight(9999, 1, 2, new DateTime(2011, 11, 11), new DateTime(2011, 11, 11), 5, 99));
                // Airline company Id doesn't exist, should cause an exception to be thrown from the sql
            });

            // Origin country constraints:

            Assert.ThrowsException<InvalidFlightException>(() =>
            {
                facade.CreateFlight(loginToken, new Flight(airlines[0].Id, 0, 2, new DateTime(2011, 11, 11), new DateTime(2011, 11, 11), 5, 99));
                // Origin country Id is 0, should cause an exception to be thrown
            });

            Assert.ThrowsException<CountryNotFoundException>(() =>
            {
                facade.CreateFlight(loginToken, new Flight(airlines[0].Id, 9999, 2, new DateTime(2011, 11, 11), new DateTime(2011, 11, 11), 5, 99));
                // Origin country Id doesn't exist, should cause an exception to be thrown from the sql
            });

            // Destination country constraints:

            Assert.ThrowsException<InvalidFlightException>(() =>
            {
                facade.CreateFlight(loginToken, new Flight(airlines[0].Id, 1, 0, new DateTime(2011, 11, 11), new DateTime(2011, 11, 11), 5, 99));
                // Destination country Id is 0, should cause an exception to be thrown
            });

            Assert.ThrowsException<CountryNotFoundException>(() =>
            {
                facade.CreateFlight(loginToken, new Flight(airlines[0].Id, 1, 9999, new DateTime(2011, 11, 11), new DateTime(2011, 11, 11), 5, 99));
                // Destination country Id doesn't exist, should cause an exception to be thrown from the sql
            });

            // Flight time constraints:

            Assert.ThrowsException<InvalidFlightException>(() =>
            {
                facade.CreateFlight(loginToken, new Flight(airlines[0].Id, 1, 2, new DateTime(), new DateTime(2011, 11, 11), 5, 99));
                // No departure time, should cause an exception to be thrown
            });

            Assert.ThrowsException<InvalidFlightException>(() =>
            {
                facade.CreateFlight(loginToken, new Flight(airlines[0].Id, 1, 2, new DateTime(2011, 11, 11), new DateTime(), 5, 99));
                // No landing time, should cause an exception to be thrown
            });

            int yearNow = DateTime.Now.Year;
            int monthNow = DateTime.Now.Month;
            int dayNow = DateTime.Now.Day;

            facade.CreateFlight(loginToken, new Flight(airlines[0].Id, 1, 2, new DateTime(2012, 12, 12), new DateTime(2012, 12, 12), 5, 120));
            IList<Flight> flights = facade.GetAllFlights();

            Assert.AreEqual(flights[0].AirlineCompanyId, airlines[0].Id);
            Assert.AreEqual(flights[0].OriginCountryCode, 1);
            Assert.AreEqual(flights[0].DestinationCountryCode, 2);
            Assert.AreEqual(flights[0].DepartureTime, new DateTime(2012, 12, 12));
            Assert.AreEqual(flights[0].LandingTime, new DateTime(2012, 12, 12));
            Assert.AreEqual(flights[0].RemainingTickets, 5);
            Assert.AreEqual(flights[0].TicketPrice, 120);

            #endregion

            #region Update flight

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.UpdateFlight(null, new Flight(airlines[0].Id, 3, 4, new DateTime(yearNow, monthNow, dayNow), new DateTime(yearNow, monthNow, dayNow), 10, 99));
                // Null token, should cause an exception to be thrown
            });

            flights = facade.GetAllFlights();
            facade.UpdateFlight(loginToken, new Flight(airlines[0].Id, 2, 3, new DateTime(yearNow, monthNow, dayNow), new DateTime(yearNow, monthNow, dayNow), 10, 99)
            {
                Id = flights[0].Id
            });

            flights = facade.GetAllFlights();

            Assert.AreEqual(flights[0].AirlineCompanyId, airlines[0].Id);
            Assert.AreEqual(flights[0].OriginCountryCode, 2);
            Assert.AreEqual(flights[0].DestinationCountryCode, 3);
            Assert.AreEqual(flights[0].DepartureTime, new DateTime(yearNow, monthNow, dayNow));
            Assert.AreEqual(flights[0].LandingTime, new DateTime(yearNow, monthNow, dayNow));
            Assert.AreEqual(flights[0].RemainingTickets, 10);
            Assert.AreEqual(flights[0].TicketPrice, 99);

            #endregion

            #region Change my password

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.ChangeMyPassword(null, "555", "666");
                // Null token, should cause an exception to be thrown
            });

            Assert.ThrowsException<WrongPasswordException>(() =>
            {
                facade.ChangeMyPassword(loginToken, "444", "666");
                // wrong password, should cause an exception to be thrown
            });

            facade.ChangeMyPassword(loginToken, "555", "666");

            airlines = facade.GetAllAirlineCompanies();

            Assert.AreEqual(airlines[0].Password, "666");

            #endregion

            #region Modify airline details

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.ModifyAirlineDetails(null, testAirline);
                // Null token, should cause an exception to be thrown
            });

            testAirline.Name = "Best Pilots";

            facade.ModifyAirlineDetails(loginToken, testAirline);
            airlines = facade.GetAllAirlineCompanies();

            Assert.AreEqual(airlines[0].Name, "Best Pilots");

            #endregion

            #region Get all my flights

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.CreateFlight(null, new Flight(airlines[0].Id, 3, 4, new DateTime(2009, 9, 9), new DateTime(2009, 9, 9), 10, 99));
                // Null token, should cause an exception to be thrown
            });

            facade.CreateFlight(loginToken, new Flight(airlines[0].Id, 3, 4, new DateTime(yearNow, monthNow, dayNow), new DateTime(yearNow, monthNow, dayNow), 10, 99));

            flights = facade.GetAllMyFlights(loginToken);

            Assert.AreEqual(flights[0].AirlineCompanyId, airlines[0].Id);
            Assert.AreEqual(flights[0].OriginCountryCode, 2);
            Assert.AreEqual(flights[0].DestinationCountryCode, 3);
            Assert.AreEqual(flights[0].DepartureTime, new DateTime(yearNow, monthNow, dayNow));
            Assert.AreEqual(flights[0].LandingTime, new DateTime(yearNow, monthNow, dayNow));
            Assert.AreEqual(flights[0].RemainingTickets, 10);
            Assert.AreEqual(flights[0].TicketPrice, 99);

            Assert.AreEqual(flights[1].AirlineCompanyId, airlines[0].Id);
            Assert.AreEqual(flights[1].OriginCountryCode, 3);
            Assert.AreEqual(flights[1].DestinationCountryCode, 4);
            Assert.AreEqual(flights[1].DepartureTime, new DateTime(yearNow, monthNow, dayNow));
            Assert.AreEqual(flights[1].LandingTime, new DateTime(yearNow, monthNow, dayNow));
            Assert.AreEqual(flights[1].RemainingTickets, 10);
            Assert.AreEqual(flights[1].TicketPrice, 99);

            #endregion
        }

        [TestMethod]
        public void AirlineGetAllTicketsTest()
        {
            new TestFacade().DeleteAllTables();

            // Creating the data for the test:

            LoginToken<Administrator> token = new LoginToken<Administrator>() { User = new Administrator() };

            LoginToken<AirlineCompany> airlineToken = new LoginToken<AirlineCompany>() { User = new AirlineCompany() };

            Customer testCustomer = new Customer("Joe", "Fin", "jf", "Joe@Fin.com", "111", "China", "100", "4580");
            new LoggedInAdministratorFacade().CreateNewCustomer(token, testCustomer);
            testCustomer.Id = new LoggedInAdministratorFacade().GetAllCustomers(token)[0].Id;

            new LoggedInAdministratorFacade().CreateNewAirline(token, new AirlineCompany("Air One", "airone", "a@o.com", "555", 1));

            IList<AirlineCompany> airlines = new LoggedInAdministratorFacade().GetAllAirlineCompanies();

            new LoggedInAirlineFacade().CreateFlight(airlineToken, new Flight(airlines[0].Id, 1, 2, new DateTime(2011, 11, 11), new DateTime(2010, 10, 10), 5, 99));
            new LoggedInAirlineFacade().CreateFlight(airlineToken, new Flight(airlines[0].Id, 2, 3, new DateTime(2012, 12, 12), new DateTime(2011, 11, 11), 0, 99));
            new LoggedInAirlineFacade().CreateFlight(airlineToken, new Flight(airlines[0].Id, 2, 3, new DateTime(2012, 12, 12), new DateTime(2012, 12, 12), 15, 99));
            new LoggedInAirlineFacade().CreateFlight(airlineToken, new Flight(airlines[0].Id, 2, 3, new DateTime(2010, 10, 10), new DateTime(2010, 10, 10), 10, 99));

            IList<Flight> flights = new LoggedInAdministratorFacade().GetAllFlights();

            LoginToken<Customer> customerToken = new LoginToken<Customer>() { User = new Customer() };

            new TestFacade().AddTicket(new Ticket(flights[0].Id, testCustomer.Id));
            new TestFacade().AddTicket(new Ticket(flights[3].Id, testCustomer.Id));

            // Testing the GetAllTickets method:

            IList<Ticket> tickets = new LoggedInAdministratorFacade().GetAllTickets(token);

            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            LoginToken<AirlineCompany> loginToken = new LoginToken<AirlineCompany>()
            {
                User = new LoggedInAdministratorFacade().GetAllAirlineCompanies()[0]
            };

            LoggedInAirlineFacade facade = fcs.GetFacade<AirlineCompany>(loginToken) as LoggedInAirlineFacade;

            Assert.AreEqual(tickets[0].CustomerId, testCustomer.Id);
            Assert.AreEqual(tickets[0].FlightId, flights[0].Id);

            Assert.AreEqual(tickets[1].CustomerId, testCustomer.Id);
            Assert.AreEqual(tickets[1].FlightId, flights[3].Id);
        }
    }
}
