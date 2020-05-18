using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    public interface IAirlineDAO : IBasicDB<AirlineCompany>
    {
        AirlineCompany GetAirlineByUserName(string username);
        IList<AirlineCompany> GetAirlinesByCountryId(int countryId);
        bool IsValidUser(AirlineCompany t);
    }
}
