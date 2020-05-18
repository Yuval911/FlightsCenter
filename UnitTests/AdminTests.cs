using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightCenter;
using System.Collections.Generic;

namespace UnitTests
{
    /// <summary>
    /// This class contains all tests of the administrator.
    /// </summary>
    [TestClass]
    public class AdminTests
    {
        /// <summary>
        /// Testing the login procedure.
        /// </summary>
        [TestMethod]
        public void AdministratorLoginTest()
        {
            new TestFacade().DeleteAllTables();

            // Login
            ILoginService loginService = new LoginService();
            LoginToken<Administrator> loginToken = new LoginToken<Administrator>();

            // Inserting wrong credntials should return a null token
            loginService.TryAdminLogin("WrongUserName", "WrongPassword", out loginToken); 

            Assert.AreEqual(loginToken, null);

            // Valid login, should return administrator token
            loginService.TryAdminLogin(GlobalConfig.adminUserName, GlobalConfig.adminPassword, out loginToken); 

            Assert.AreEqual(loginToken.User.GetType(), new Administrator().GetType());

            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            // Getting the facade
            LoggedInAdministratorFacade facade = fcs.GetFacade<Administrator>(loginToken) as LoggedInAdministratorFacade;

            Assert.AreNotEqual(facade.GetType(), null);
        }

        /// <summary>
        /// Testing administrator facade's methods.
        /// </summary>
        [TestMethod]
        public void AdministratorFacadeTest()
        {
            new TestFacade().DeleteAllTables();

            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            LoginToken<Administrator> loginToken = new LoginToken<Administrator>()
            {
                User = new Administrator()
            };

            LoggedInAdministratorFacade facade = fcs.GetFacade<Administrator>(loginToken) as LoggedInAdministratorFacade;

            #region Create New Airline

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.CreateNewAirline(null, new AirlineCompany("name", "username", "n@n.com", "123", 1));
                // Token is null, should cause an exception to be thrown
            });

            Assert.ThrowsException<InvalidAirlineCompanyException>(() =>
            {
                facade.CreateNewAirline(loginToken, new AirlineCompany("", "username", "n@n.com", "123", 1));
                // No name, should cause an exception to be thrown
            });

            Assert.ThrowsException<InvalidAirlineCompanyException>(() =>
            {
                facade.CreateNewAirline(loginToken, new AirlineCompany("name", "", "n@n.com", "123", 1));
                // No user name, should cause an exception to be thrown
            });

            Assert.ThrowsException<InvalidAirlineCompanyException>(() =>
            {
                facade.CreateNewAirline(loginToken, new AirlineCompany("name", "username", "", "123", 1));
                // No email, should cause an exception to be thrown
            });

            Assert.ThrowsException<InvalidAirlineCompanyException>(() =>
            {
                facade.CreateNewAirline(loginToken, new AirlineCompany("name", "username", "n@n.com", "", 1));
                // No password, should cause an exception to be thrown
            });

            Assert.ThrowsException<InvalidAirlineCompanyException>(() =>
            {
                facade.CreateNewAirline(loginToken, new AirlineCompany("name", "username", "n@n.com", "123", 0));
                // Origin country ID cannot be zero (null), should cause an exception to be thrown
            });

            Assert.ThrowsException<CountryNotFoundException>(() =>
            {
                facade.CreateNewAirline(loginToken, new AirlineCompany("name", "username", "n@n.com", "123", 500));
                // Origin country ID does not exist, should cause an exception to be thrown
            });


            Assert.ThrowsException<UserNameAlreadyExistException>(() =>
            {
                facade.CreateNewAirline(loginToken, new AirlineCompany("name", "admin", "n@n.com", "123", 1));
                // User name is taken by the admin, should cause an exception to be thrown
            });

            Assert.ThrowsException<UserNameAlreadyExistException>(() =>
            {
                facade.CreateNewAirline(loginToken, new AirlineCompany("Sky Team", "skt", "s@t.com", "666", 1));
                facade.CreateNewAirline(loginToken, new AirlineCompany("Sky Tour", "skt", "s@t.com", "777", 1));
                // User name is taken, should cause an exception to be thrown
            });

            Assert.ThrowsException<AirlineCompanyNameAlreadyExistException>(() =>
            {
                facade.CreateNewAirline(loginToken, new AirlineCompany("Air One", "airone", "a@o.com", "555", 1));
                facade.CreateNewAirline(loginToken, new AirlineCompany("Air One", "airy", "a@o.com", "333", 1));
                // Company name already exist, should cause an exception to be thrown
            });

            new TestFacade().DeleteAllTables();

            facade.CreateNewAirline(loginToken, new AirlineCompany("One World", "one", "o@w.com", "777", 1));
            IList<AirlineCompany> airlines = facade.GetAllAirlineCompanies();

            Assert.AreEqual(airlines[0].Name, "One World");
            Assert.AreEqual(airlines[0].UserName, "one");
            Assert.AreEqual(airlines[0].Email, "o@w.com");
            Assert.AreEqual(airlines[0].Password, "777");
            Assert.AreEqual(airlines[0].CountryCode, 1);

            #endregion

            #region Create New Customer

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.CreateNewCustomer(null, new Customer("John", "Doe", "jd", "john@doe.com","j123", "unknown", "555", "4580"));
                // Token is null, should cause an exception to be thrown
            });

            Assert.ThrowsException<InvalidCustomerException>(() =>
            {
                facade.CreateNewCustomer(loginToken, new Customer("", "Doe", "jd", "john@doe.com", "j123", "unknown", "555", "4580"));
                // No first name, should cause an exception to be thrown
            });

            Assert.ThrowsException<InvalidCustomerException>(() =>
            {
                facade.CreateNewCustomer(loginToken, new Customer("John", "", "jd", "john@doe.com", "j123", "unknown", "555", "4580"));
                // No last name, should cause an exception to be thrown
            });

            Assert.ThrowsException<InvalidCustomerException>(() =>
            {
                facade.CreateNewCustomer(loginToken, new Customer("John", "Doe", "", "john@doe.com", "j123", "unknown", "555", "4580"));
                // No user name, should cause an exception to be thrown
            });

            Assert.ThrowsException<InvalidCustomerException>(() =>
            {
                facade.CreateNewCustomer(loginToken, new Customer("John", "Doe", "jd", "", "j123", "unknown", "555", "4580"));
                // No email, should cause an exception to be thrown
            });

            Assert.ThrowsException<InvalidCustomerException>(() =>
            {
                facade.CreateNewCustomer(loginToken, new Customer("John", "Doe", "jd", "john@doe.com", "", "unknown", "555", "4580"));
                // No password, should cause an exception to be thrown
            });

            Assert.ThrowsException<UserNameAlreadyExistException>(() =>
            {
                facade.CreateNewCustomer(loginToken, new Customer("John", "Doe", "jd", "john@doe.com", "j123", "unknown", "555", "4580"));
                facade.CreateNewCustomer(loginToken, new Customer("Jane", "Dean", "jd", "john@doe.com", "j999", "NY", "458", "4580"));
                // User name is already taken, should cause an exception to be thrown
            });

            new TestFacade().DeleteAllTables();

            facade.CreateNewCustomer(loginToken, new Customer("John", "Doe", "jd", "john@doe.com", "j123", "unknown", "555", "4580"));
            IList<Customer> customers = facade.GetAllCustomers(loginToken);

            Assert.AreEqual(customers[0].FirstName, "John");
            Assert.AreEqual(customers[0].LastName, "Doe");
            Assert.AreEqual(customers[0].UserName, "jd");
            Assert.AreEqual(customers[0].Email, "john@doe.com");
            Assert.AreEqual(customers[0].Password, "j123");
            Assert.AreEqual(customers[0].Address, "unknown");
            Assert.AreEqual(customers[0].PhoneNo, "555");
            Assert.AreEqual(customers[0].CreditCardNo, "4580");

            #endregion

            #region Update airline details

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.CreateNewAirline(loginToken, new AirlineCompany("Air One", "airone", "a@o.com", "555", 1));
                airlines = facade.GetAllAirlineCompanies();

                facade.UpdateAirlineDetails(null, new AirlineCompany("Air two", "airtwo", "a@t.com", "555", 1) { Id = airlines[0].Id });
                // Token is null, should cause an exception to be thrown
            });

            facade.UpdateAirlineDetails(loginToken, new AirlineCompany("Air two", "airtwo", "a@t.com", "555", 1) { Id = airlines[0].Id });
            airlines = facade.GetAllAirlineCompanies();

            Assert.AreEqual(airlines[0].Name, "Air two");
            Assert.AreEqual(airlines[0].UserName, "airtwo");
            Assert.AreEqual(airlines[0].Email, "a@t.com");
            Assert.AreEqual(airlines[0].Password, "555");
            Assert.AreEqual(airlines[0].CountryCode, 1);

            new TestFacade().DeleteAllTables();

            #endregion

            #region Update customer details

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.CreateNewCustomer(loginToken, new Customer("John", "Doe", "jd", "john@doe.com", "j123", "unknown", "555", "4580"));
                customers = facade.GetAllCustomers(loginToken);

                facade.UpdateCustomerDetails(null, new Customer("Jane", "Darwin", "jd", "john@doe.com", "j123", "CA", "255", "4580") { Id = customers[0].Id });
                // Token is null, should cause an exception to be thrown
            });

            facade.UpdateCustomerDetails(loginToken, new Customer("Jane", "Darwin", "jd", "john@doe.com", "j123", "CA", "255", "4580") { Id = customers[0].Id });
            customers = facade.GetAllCustomers(loginToken);

            Assert.AreEqual(customers[0].FirstName, "Jane");
            Assert.AreEqual(customers[0].LastName, "Darwin");
            Assert.AreEqual(customers[0].UserName, "jd");
            Assert.AreEqual(customers[0].Email, "john@doe.com");
            Assert.AreEqual(customers[0].Password, "j123");
            Assert.AreEqual(customers[0].Address, "CA");
            Assert.AreEqual(customers[0].PhoneNo, "255");
            Assert.AreEqual(customers[0].CreditCardNo, "4580");

            #endregion

            new TestFacade().DeleteAllTables();

            #region Remove airline

            facade.CreateNewAirline(loginToken, new AirlineCompany("Air One", "airone", "a@o.com", "555", 1));
            airlines = facade.GetAllAirlineCompanies();

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.RemoveAirline(null, airlines[0]);
                // Token is null, should cause an exception to be thrown
            });

            facade.RemoveAirline(loginToken, airlines[0]);
            airlines = facade.GetAllAirlineCompanies();

            Assert.AreEqual(airlines.Count, 0);

            #endregion

            #region Remove customer

            facade.CreateNewCustomer(loginToken, new Customer("John", "Doe", "jd", "john@doe.com", "j123", "unknown", "555", "4580"));
            customers = facade.GetAllCustomers(loginToken);

            Assert.ThrowsException<InvalidTokenException>(() =>
            {
                facade.RemoveCustomer(null, customers[0]);
                // Token is null, should cause an exception to be thrown
            });

            facade.RemoveCustomer(loginToken, customers[0]);
            customers = facade.GetAllCustomers(loginToken);

            Assert.AreEqual(customers.Count, 0);

            #endregion

            #region Get all customers

            facade.CreateNewAirline(loginToken, new AirlineCompany("Air One", "airone", "a@o.com", "555", 1));
            facade.CreateNewAirline(loginToken, new AirlineCompany("Fly Far", "flyfar", "f@f.com", "931", 1));

            airlines = facade.GetAllAirlineCompanies();

            Assert.AreEqual(airlines[0].Name, "Air One");
            Assert.AreEqual(airlines[0].UserName, "airone");
            Assert.AreEqual(airlines[0].Email, "a@o.com");
            Assert.AreEqual(airlines[0].Password, "555");
            Assert.AreEqual(airlines[0].CountryCode, 1);

            Assert.AreEqual(airlines[1].Name, "Fly Far");
            Assert.AreEqual(airlines[1].UserName, "flyfar");
            Assert.AreEqual(airlines[1].Email, "f@f.com");
            Assert.AreEqual(airlines[1].Password, "931");
            Assert.AreEqual(airlines[1].CountryCode, 1);

            #endregion
        }
    }
}
