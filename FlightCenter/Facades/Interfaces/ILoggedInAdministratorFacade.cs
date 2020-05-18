using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    interface ILoggedInAdministratorFacade
    {
        void CreateNewAirline(LoginToken<Administrator> token, AirlineCompany airline);
        void CreateNewCustomer(LoginToken<Administrator> token, Customer customer);
        void RemoveAirline(LoginToken<Administrator> token, AirlineCompany airline);
        void RemoveCustomer(LoginToken<Administrator> token, Customer customer);
        void UpdateAirlineDetails(LoginToken<Administrator> token, AirlineCompany airline);
        void UpdateCustomerDetails(LoginToken<Administrator> token, Customer customer);
        IList<Customer> GetAllCustomers(LoginToken<Administrator> token);
        IList<Ticket> GetAllTickets(LoginToken<Administrator> token);

        IList<AirlineCompany> GetAllAirlinesFromRegisterQueue(LoginToken<Administrator> token);
        void RemoveAirlineFromRegisterQueue(LoginToken<Administrator> token, AirlineCompany airline);
    }
}
