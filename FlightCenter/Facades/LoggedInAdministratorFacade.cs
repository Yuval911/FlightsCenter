using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This class is a part of set of classes that implements the Facade design pattern.
    /// It bridges between the Web API and the DAOs, and exposes all the available methods of the administrator.
    /// /// Calling these methods requires having an administrator login token.
    /// </summary>
    public class LoggedInAdministratorFacade : AnonymousUserFacade, ILoggedInAdministratorFacade
    {
        private readonly string invalidTokenMessage = GlobalConfig.invalidTokenMessage;

        public void CreateNewAirline(LoginToken<Administrator> token, AirlineCompany airline)
        {
            CheckToken(token);
            airlineDAO.Add(airline);
            Logger.Log(LogLevel.Info, $"The airline company '{airline.Name}' was aprroved by the administrator and an account was created for it");
        }

        public void CreateNewCustomer(LoginToken<Administrator> token, Customer customer)
        {
            CheckToken(token);
            customerDAO.Add(customer);
            Logger.Log(LogLevel.Info, $"The administrator created a new customer account for '{customer.GetFullName()}'.");
        }

        public void RemoveAirline(LoginToken<Administrator> token, AirlineCompany airline)
        {
            CheckToken(token);
            airlineDAO.Remove(airline.Id);
            Logger.Log(LogLevel.Info, $"The administrator removed the airline company account of '{airline.Name}'.");
        }

        public void RemoveCustomer(LoginToken<Administrator> token, Customer customer)
        {
            CheckToken(token);
            customerDAO.Remove(customer.Id);
            Logger.Log(LogLevel.Info, $"The administrator removed the customer account of '{customer.GetFullName()}'.");
        }

        public void UpdateAirlineDetails(LoginToken<Administrator> token, AirlineCompany airline)
        {
            CheckToken(token);
            airlineDAO.Update(airline);
            Logger.Log(LogLevel.Info, $"The administrator updated the airline account of '{airline.Name}'.");
        }

        public void UpdateCustomerDetails(LoginToken<Administrator> token, Customer customer)
        {
            CheckToken(token);
            customerDAO.Update(customer);
            Logger.Log(LogLevel.Info, $"The administrator updated the customer account of '{customer.GetFullName()}'.");
        }

        public IList<Customer> GetAllCustomers(LoginToken<Administrator> token)
        {
            CheckToken(token);
            Logger.Log(LogLevel.Info, "A list of all customers was requested by the administrator");
            return customerDAO.GetAll();
        }

        public IList<Ticket> GetAllTickets(LoginToken<Administrator> token)
        {
            CheckToken(token);
            Logger.Log(LogLevel.Info, "A list of all flight tickets was requested by the administrator");
            return ticketDAO.GetAll();
        }

        public IList<AirlineCompany> GetAllAirlinesFromRegisterQueue(LoginToken<Administrator> token)
        {
            CheckToken(token);
            Logger.Log(LogLevel.Info, "A list of all airline companies was requested by the administrator");
            return airlineRedisDAO.GetAll();
        }

        public void RemoveAirlineFromRegisterQueue(LoginToken<Administrator> token, AirlineCompany airline)
        {
            CheckToken(token);
            airlineRedisDAO.Remove(airline);
        }

        private void CheckToken(LoginToken<Administrator> token)
        {
            if (token == null)
            {
                InvalidTokenException ex = new InvalidTokenException(invalidTokenMessage);
                Logger.Log(LogLevel.Error, ex);
                throw ex;
            }
        }
    }
}
