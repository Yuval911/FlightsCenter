using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    public class AirlineCompany : IUser, IPoco
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int CountryCode { get; set; }

        public AirlineCompany()
        {
            
        }

        public AirlineCompany(string name, string userName, string email, string password, int countryCode)
        {
            Name = name;
            UserName = userName;
            Email = email;
            Password = password;
            CountryCode = countryCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            AirlineCompany otherAirlineComapny = obj as AirlineCompany;
            return this.Id == otherAirlineComapny.Id;
        }

        public static bool operator ==(AirlineCompany a, AirlineCompany b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            return a.Id == b.Id;
        }

        public static bool operator !=(AirlineCompany a, AirlineCompany b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.Id;
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        public string GetAllDetalies()
        {
            return $"{Id}, {Name}, {UserName}, {Email}, {Password}, {CountryCode}";
        }
    }
}
