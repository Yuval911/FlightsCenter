using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    public class Country : IPoco
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Country()
        {

        }

        public Country(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            Country otherCountry = obj as Country;
            return this.Id == otherCountry.Id;
        }

        public static bool operator ==(Country a, Country b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            return a.Id == b.Id;
        }

        public static bool operator !=(Country a, Country b)
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
            return $"{Id}, {Name}";
        }
    }
}
