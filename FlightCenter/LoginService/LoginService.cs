using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This class handles the login to the system for all users.
    /// It has 3 login methods for each and every user type: Administrator, Customer and Airline Company, and one method
    /// that calls them and tries each one until the login succeeds.
    /// </summary>
    public class LoginService : ILoginService
    {
        private readonly IAirlineDAO airlineDAO = new AirlineMySqlDAO();
        private readonly ICustomerDAO customerDAO = new CustomerMySqlDAO();

        private readonly string wrongPasswordMessage = GlobalConfig.wrongPasswordMessage;

        public ILoginToken TryLoginAllUsers(string username, string password)
        {
            LoginToken<Administrator> adminLoginToken = new LoginToken<Administrator>();
            LoginToken<AirlineCompany> airlineLoginToken = new LoginToken<AirlineCompany>();
            LoginToken<Customer> customerLoginToken = new LoginToken<Customer>();

            if (TryAdminLogin(username, password, out adminLoginToken))
            {
                return adminLoginToken;
            }
            if (TryAirlineLogin(username, password, out airlineLoginToken))
            {
                return airlineLoginToken;
            }
            if (TryCustomerLogin(username, password, out customerLoginToken))
            {
                return customerLoginToken;
            }

            return null;
        }

        public bool TryAdminLogin(string userName, string password, out LoginToken<Administrator> token)
        {
            Administrator admin = new Administrator();

            if (userName == GlobalConfig.adminUserName)
            {
                if (password == GlobalConfig.adminPassword)
                {
                    token = new LoginToken<Administrator>()
                    {
                        User = admin
                    };
                    Logger.Log(LogLevel.Info, "The administrator has logged in to the system.");
                    return true;
                }
                else
                {
                    Logger.Log(LogLevel.Info, "A failed login to the administrator account occurred (Wrong password inserted).");
                    throw new WrongPasswordException(wrongPasswordMessage);
                }     
            }
            else
            {
                token = null;
                return false;
            }
                
        }

        public bool TryAirlineLogin(string userName, string password, out LoginToken<AirlineCompany> token)
        {
            AirlineCompany airlineCompany = airlineDAO.GetAirlineByUserName(userName);

            if (airlineCompany != null)
            {
                if (airlineCompany.Password == password)
                {
                    token = new LoginToken<AirlineCompany>()
                    {
                        User = airlineCompany
                    };
                    Logger.Log(LogLevel.Info, $"The airline '{airlineCompany.Name}' has logged in to the system.");
                    return true;
                }
                else
                {
                    Logger.Log(LogLevel.Info, $"A failed login to the airline account of '{airlineCompany.Name}' occurred (Wrong password inserted).");
                    throw new WrongPasswordException(wrongPasswordMessage);
                }
                    
            }
            else
            {
                token = null;
                return false;
            }
        }

        public bool TryCustomerLogin(string userName, string password, out LoginToken<Customer> token)
        {
            Customer customer = customerDAO.GetCustomerByUsername(userName);

            if (customer != null)
            {
                if (customer.Password == password)
                {
                    token = new LoginToken<Customer>()
                    {
                        User = customer
                    };
                    Logger.Log(LogLevel.Info, $"The customer '{customer.GetFullName()}' has logged in to the system.");
                    return true;
                }
                else
                {
                    Logger.Log(LogLevel.Info, $"A failed login to the customer account of '{customer.GetFullName()}' occurred (Wrong password inserted).");
                    throw new WrongPasswordException(wrongPasswordMessage);
                }
            }
            else
            {
                token = null;
                return false;
            }
        }
    }
}
