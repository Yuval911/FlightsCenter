using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    public interface ICustomerDAO : IBasicDB<Customer>
    {
        Customer GetCustomerByUsername(string username);
        bool IsValidUser(Customer t);
    }
}
