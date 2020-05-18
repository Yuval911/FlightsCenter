using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightCenter;
using System.Collections.Generic;

namespace UnitTests
{
    /// <summary>
    /// This class contains all tests of the customer.
    /// </summary>
    [TestClass]
    public class CustomerTests
    {
        /// <summary>
        /// Testing the login procedure.
        /// </summary>
        [TestMethod]
        public void F_CustomerLoginTest()
        {
            new TestFacade().DeleteAllTables();

            LoginToken<Administrator> token = new LoginToken<Administrator>() { User = new Administrator() };

            new LoggedInAdministratorFacade().CreateNewCustomer(token, new Customer("Joe", "Fin", "jf", "Joe@Fin.com", "111", "China", "100", "4580"));

            // Login

            ILoginService loginService = new LoginService();
            LoginToken<Customer> loginToken = new LoginToken<Customer>();

            // Inserting wrong credntials should return a null token
            loginService.TryCustomerLogin("WrongUserName", "WrongPassword", out loginToken); 

            Assert.AreEqual(loginToken, null);

            // Valid login, should return an airline token
            loginService.TryCustomerLogin("jf", "111", out loginToken); 

            Assert.AreEqual(loginToken.User.GetType(), new Customer().GetType());

            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            // Getting the facade
            LoggedInCustomerFacade facade = fcs.GetFacade<Customer>(loginToken) as LoggedInCustomerFacade;

            Assert.AreNotEqual(facade.GetType(), null);
        }

        /// <summary>
        /// Testing customer facade's methods.
        /// </summary>
        [TestMethod]
        public void G_CustomerFacadeTest()
        {
            new TestFacade().DeleteAllTables();

            Customer testCustomer = new Customer("Joe", "Fin", "jf", "Joe@Fin.com", "111", "China", "100", "4580");

            LoginToken<Administrator> token = new LoginToken<Administrator>() { User = new Administrator() };

            new LoggedInAdministratorFacade().CreateNewCustomer(token, testCustomer);

            testCustomer.Id = new LoggedInAdministratorFacade().GetAllCustomers(token)[0].Id;

            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            LoginToken<Customer> loginToken = new LoginToken<Customer>()
            {
                User = testCustomer
            };

            LoggedInCustomerFacade facade = fcs.GetFacade<Customer>(loginToken) as LoggedInCustomerFacade;

            // Adding some data for testing:

            new LoggedInAdministratorFacade().CreateNewAirline(token, new AirlineCompany("Air One", "airone", "a@o.com", "555", 1));

            IList<AirlineCompany> airlines = facade.GetAllAirlineCompanies();

            LoginToken<AirlineCompany> airlineToken = new LoginToken<AirlineCompany>() { User = new AirlineCompany() };

            new LoggedInAirlineFacade().CreateFlight(airlineToken, new Flight(airlines[0].Id, 1, 2, new DateTime(2011, 11, 11), new DateTime(2010, 10, 10), 5, 99));
            new LoggedInAirlineFacade().CreateFlight(airlineToken, new Flight(airlines[0].Id, 2, 3, new DateTime(2012, 12, 12), new DateTime(2011, 11, 11), 0, 99));
            new LoggedInAirlineFacade().CreateFlight(airlineToken, new Flight(airlines[0].Id, 2, 3, new DateTime(2012, 12, 12), new DateTime(2012, 12, 12), 1, 99));
            new LoggedInAirlineFacade().CreateFlight(airlineToken, new Flight(airlines[0].Id, 2, 3, new DateTime(2010, 10, 10), new DateTime(2010, 10, 10), 10, 99));

            IList<Flight> flights = facade.GetAllFlights();

            #region Purchase ticket

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.PurchaseTicket(null, flights[0].Id);
                // Null token, should cause an exception to be thrown
            });

            Assert.ThrowsException<NoTicketsRemainingException>(() =>
            {
                facade.PurchaseTicket(loginToken, flights[1].Id);
                // A flight with no tickets left, should cause an exception to be thrown
            });

            facade.PurchaseTicket(loginToken, flights[0].Id);

            IList<Ticket> tickets = new LoggedInAdministratorFacade().GetAllTickets(token);
            flights = facade.GetAllFlights();

            Assert.AreEqual(tickets[0].CustomerId, testCustomer.Id);
            Assert.AreEqual(tickets[0].FlightId, flights[0].Id);

            Assert.AreEqual(flights[0].RemainingTickets, 4);

            Assert.ThrowsException<OneTicketForCustomerOnlyException>(() =>
            {
                facade.PurchaseTicket(loginToken, flights[0].Id);
                // A customer can buy only one ticket per flight, should cause an exception to be thrown
            });

            #endregion

            #region Cancel ticket

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.CancelTicket(null, tickets[0].Id);
                // Null token, should cause an exception to be thrown
            });

            facade.CancelTicket(loginToken, tickets[0].Id);

            tickets = new LoggedInAdministratorFacade().GetAllTickets(token);

            flights = facade.GetAllFlights();

            Assert.AreEqual(tickets.Count, 0);

            Assert.AreEqual(flights[0].RemainingTickets, 5);

            #endregion

            #region Get all my flights

            facade.PurchaseTicket(loginToken, flights[0].Id);
            facade.PurchaseTicket(loginToken, flights[3].Id);

            flights = facade.GetAllMyFlights(loginToken);

            Assert.AreEqual(flights[0].AirlineCompanyId, airlines[0].Id);
            Assert.AreEqual(flights[0].OriginCountryCode, 1);
            Assert.AreEqual(flights[0].DestinationCountryCode, 2);
            Assert.AreEqual(flights[0].DepartureTime, new DateTime(2011, 11, 11));
            Assert.AreEqual(flights[0].LandingTime, new DateTime(2010, 10, 10));
            Assert.AreEqual(flights[0].RemainingTickets, 4);

            Assert.AreEqual(flights[1].AirlineCompanyId, airlines[0].Id);
            Assert.AreEqual(flights[1].OriginCountryCode, 2);
            Assert.AreEqual(flights[1].DestinationCountryCode, 3);
            Assert.AreEqual(flights[1].DepartureTime, new DateTime(2010, 10, 10));
            Assert.AreEqual(flights[1].LandingTime, new DateTime(2010, 10, 10));
            Assert.AreEqual(flights[1].RemainingTickets, 9);

            #endregion
        }
    }
}
