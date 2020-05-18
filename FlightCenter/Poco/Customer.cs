using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    public class Customer : IUser, IPoco
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; } // 
        public string Email { get; set; }
        public string Password { get; set; } //
        public string Address { get; set; } //
        public string PhoneNo { get; set; } //
        public string CreditCardNo { get; set; }

        public Customer()
        {

        }

        public Customer(string firstName, string lastName, string userName, string email, string password, string address, string phoneNo, string creditCardNo)
        {
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Email = email;
            Password = password;
            Address = address;
            PhoneNo = phoneNo;
            CreditCardNo = creditCardNo;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            Customer otherCustomer = obj as Customer;
            return this.Id == otherCustomer.Id;
        }

        public static bool operator ==(Customer a, Customer b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;
            return a.Id == b.Id;
        }

        public static bool operator !=(Customer a, Customer b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.Id;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }

        public string GetAllDetalies()
        {
            return $"{Id}, {FirstName}, {LastName}, {UserName}, {Email}, {Password}, {Address}, {PhoneNo}, {CreditCardNo}";
        }
    }
}
