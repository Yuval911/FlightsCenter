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
    /// It bridges between the Web API and the DAOs, and exposes all the available methods of the airline company.
    /// Calling these methods requires having an airline company login token.
    /// </summary>
    public class LoggedInAirlineFacade : AnonymousUserFacade, ILoggedInAirlineFacade
    {
        private readonly string invalidTokenMessage = GlobalConfig.invalidTokenMessage;

        public void ChangeMyPassword(LoginToken<AirlineCompany> token, string oldPassword, string newPassword)
        {
            CheckToken(token);
            if (oldPassword == token.User.Password)
            {
                token.User.Password = newPassword;
                airlineDAO.Update(token.User);
                Logger.Log(LogLevel.Info, $"The airline company '{token.User.Name}' has changed its password.");
            }
            else
                throw new WrongPasswordException("The old password is wrong.");
        }

        public void CreateFlight(LoginToken<AirlineCompany> token, Flight flight)
        {
            CheckToken(token);
            flightDAO.Add(flight);
            Logger.Log(LogLevel.Info, $"The airline company '{token.User.Name}' created a new flight.");
        }

        public IList<Flight> GetAllMyFlights(LoginToken<AirlineCompany> token)
        {
            CheckToken(token);
            Logger.Log(LogLevel.Info, $"A list of all airline's flights was requested by '{token.User.Name}'.");
            return flightDAO.GetFlightsByAirlineCompanyId(token.User.Id);
        }

        public IList<Ticket> GetAllTickets(LoginToken<AirlineCompany> token)
        {
            CheckToken(token);
            Logger.Log(LogLevel.Info, $"A list of all airline's flight tickets was requested by '{token.User.Name}'.");
            return ticketDAO.GetTicketsByAirlineId(token.User.Id);
        }

        public void ModifyAirlineDetails(LoginToken<AirlineCompany> token, AirlineCompany airline)
        {
            CheckToken(token);
            airlineDAO.Update(airline);
            Logger.Log(LogLevel.Info, $"The airline company '{token.User.Name}' has updated its details.");
        }

        public void UpdateFlight(LoginToken<AirlineCompany> token, Flight flight)
        {
            CheckToken(token);
            flightDAO.Update(flight);
            Logger.Log(LogLevel.Info, $"The airline company '{token.User.Name}' has updated flight {flight.Id}.");
        }

        public void CancelFlight(LoginToken<AirlineCompany> token, int flightId)
        {
            CheckToken(token);
            flightDAO.Remove(flightId);
            Logger.Log(LogLevel.Info, $"The airline company '{token.User.Name}' has canceled flight {flightId}.");
        }

        private void CheckToken(LoginToken<AirlineCompany> token)
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
