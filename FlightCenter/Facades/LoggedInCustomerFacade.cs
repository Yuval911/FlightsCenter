using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This class is a part of set of classes that implements the Facade design pattern.
    /// It bridges between the Web API and the DAOs, and exposes all the available methods of the customer
    /// Calling these methods requires having a customer login token.
    /// </summary>
    public class LoggedInCustomerFacade : AnonymousUserFacade, ILoggedInCustomerFacade
    {
        private readonly string invalidTokenMessage = GlobalConfig.invalidTokenMessage;

        public void CancelTicket(LoginToken<Customer> token, int ticketId)
        {
            CheckToken(token);
            ticketDAO.Remove(ticketId);
            Logger.Log(LogLevel.Info, $"The customer '{token.User.GetFullName()}' has canceled the flight ticket {ticketId}.");
        }

        public IList<Flight> GetAllMyFlights(LoginToken<Customer> token)
        {
            CheckToken(token);
            Logger.Log(LogLevel.Info, $"A list of customer's flights was requested by '{token.User.GetFullName()}'.");
            return flightDAO.GetFlightsByCustomer(token.User);
        }

        public IList<JObject> GetAllMyFlightsJson(LoginToken<Customer> token)
        {
            CheckToken(token);
            Logger.Log(LogLevel.Info, $"A list of customer's flights was requested by '{token.User.GetFullName()}'.");
            return flightDAO.GetJsonFlightsByCustomer(token.User);
        }

        public void PurchaseTicket(LoginToken<Customer> token, int flightId)
        {
            CheckToken(token);
            ticketDAO.Add(new Ticket(flightId, token.User.Id));
            Logger.Log(LogLevel.Info, $"The customer '{token.User.GetFullName()}' has bought a ticket for flight {flightId}.");
        }

        public void ModifyCustomerDetails(LoginToken<Customer> token, Customer customer)
        {
            CheckToken(token);
            customerDAO.Update(customer);
            Logger.Log(LogLevel.Info, $"The customer '{token.User.GetFullName()}' has updated its details.");
        }

        public void ChangeMyPassword(LoginToken<Customer> token, string oldPassword, string newPassword)
        {
            CheckToken(token);
            if (oldPassword == token.User.Password)
            {
                token.User.Password = newPassword;
                customerDAO.Update(token.User);
                Logger.Log(LogLevel.Info, $"The customer '{token.User.GetFullName()}' has changed its password.");
            }
            else
                throw new WrongPasswordException("The old password is wrong.");
        }

        private void CheckToken(LoginToken<Customer> token)
        {
            if (token == null)
                throw new InvalidTokenException(invalidTokenMessage);
        }
    }
}
