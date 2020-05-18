using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This is the login token. It being obtained by the login service and contains the user's Poco.
    /// </summary>
    public class LoginToken<T> : ILoginToken where T : IUser
    {
        public T User { get; set; }    
    }
}
