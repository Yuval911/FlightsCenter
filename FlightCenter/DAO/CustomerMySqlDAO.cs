using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This is the data access class of the customers.
    /// It conatins the basic CRUD methods and some extras.
    /// </summary>
    public class CustomerMySqlDAO : BasicMySqlDAO, ICustomerDAO
    {
        // Basic CRUD methods:

        public void Add(Customer t)
        {
            TryCatchDatabaseFunction((conn) => {

                if (!IsValidUser(t))
                    return;

                string query = $"INSERT INTO Customers (FIRST_NAME, LAST_NAME, USER_NAME, EMAIL, PASSWORD, ADDRESS, PHONE_NO, CREDIT_CARD_NO) " +
                               $"VALUES ('{t.FirstName}', '{t.LastName}', '{t.UserName}', '{t.Email}', '{t.Password}', '{t.Address}', " +
                               $"'{t.PhoneNo}', '{t.CreditCardNo}');";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        /// <summary>
        /// Adds a list of customers to the database. 
        /// When test mode is true there would be no constraints checking.
        /// </summary>
        public void AddRange(IList<Customer> list, bool testMode)
        {
            TryCatchDatabaseFunction((conn) => {

                if (!testMode)
                {
                    foreach (Customer customer in list)
                    {
                        if (!IsValidUser(customer))
                            return;
                    }
                }

                if (list.Count == 0)
                    return;

                string query = $"INSERT INTO Customers (FIRST_NAME, LAST_NAME, USER_NAME, EMAIL, PASSWORD, ADDRESS, PHONE_NO, CREDIT_CARD_NO) VALUES ";

                foreach (Customer customer in list)
                {
                    query += $"('{customer.FirstName}', '{customer.LastName}', '{customer.UserName}', '{customer.Email}', '{customer.Password}', " +
                               $"'{customer.Address}', '{customer.PhoneNo}', '{customer.CreditCardNo}'), ";
                }

                query = query.Remove(query.Count() - 2);

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        public Customer Get(int id)
        {
            Customer customer = new Customer();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Customers WHERE Customers.ID = {id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    customer.Id = Convert.ToInt32(reader[0]);
                    customer.FirstName = Convert.ToString(reader[1]);
                    customer.LastName = Convert.ToString(reader[2]);
                    customer.UserName = Convert.ToString(reader[3]);
                    customer.Email = Convert.ToString(reader[4]);
                    customer.Password = Convert.ToString(reader[5]);
                    customer.Address = Convert.ToString(reader[6]);
                    customer.PhoneNo = Convert.ToString(reader[7]);
                    customer.CreditCardNo = Convert.ToString(reader[8]);
                }

                reader.Close();

                if (customer.Id == 0)
                    throw new CustomerNotFoundException($"Customer with ID: {id} doesn't exist.");

            });

            return customer;
        }

        public IList<Customer> GetAll()
        {
            List<Customer> customers = new List<Customer>();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Customers";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Customer customer = new Customer();

                    customer.Id = Convert.ToInt32(reader[0]);
                    customer.FirstName = Convert.ToString(reader[1]);
                    customer.LastName = Convert.ToString(reader[2]);
                    customer.UserName = Convert.ToString(reader[3]);
                    customer.Email = Convert.ToString(reader[4]);
                    customer.Password = Convert.ToString(reader[5]);
                    customer.Address = Convert.ToString(reader[6]);
                    customer.PhoneNo = Convert.ToString(reader[7]);
                    customer.CreditCardNo = Convert.ToString(reader[8]);

                    customers.Add(customer);
                }

                reader.Close();

            });

            return customers;
        }

        public void Remove(int id)
        {
            TryCatchDatabaseFunction((conn) => {

                // Cheking if the customer exist:

                string query = $"SELECT * FROM Customers WHERE Customers.ID = {id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Customer customer = new Customer();

                while (reader.Read())
                {
                    customer.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (customer.Id == 0)
                    throw new CustomerNotFoundException($"Customer with ID: {id} doesn't exist.");

                // Deleting the tickets of the customer:

                query = $"DELETE FROM Tickets WHERE Tickets.CUSTOMER_ID = {id}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

                // Deleting the customer:

                query = $"DELETE FROM Customers WHERE Customers.ID = {id}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        public void Update(Customer t)
        {
            TryCatchDatabaseFunction((conn) => {

                if (!IsValidUser(t))
                    return;

                // Checks if a customer with that ID exists:

                string query = $"SELECT * FROM Customers WHERE Customers.ID = {t.Id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Customer customer = new Customer();

                while (reader.Read())
                {
                    customer.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (customer.Id == 0)
                    throw new CustomerNotFoundException($"Customer with ID: {t.Id} doesn't exist");

                // Updates the customer details:

                query = $"UPDATE Customers SET FIRST_NAME = '{t.FirstName}', LAST_NAME = '{t.LastName}', USER_NAME = '{t.UserName}', EMAIL = '{t.Email}', " +
                               $"PASSWORD = '{t.Password}', ADDRESS = '{t.Address}', PHONE_NO = '{t.PhoneNo}', CREDIT_CARD_NO = '{t.CreditCardNo}' " +
                               $"WHERE Customers.ID = {t.Id}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        // Extra methods:
        
        public Customer GetCustomerByUsername(string username)
        {
            Customer customer = new Customer();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Customers WHERE Customers.USER_NAME = '{username}'";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    customer.Id = Convert.ToInt32(reader[0]);
                    customer.FirstName = Convert.ToString(reader[1]);
                    customer.LastName = Convert.ToString(reader[2]);
                    customer.UserName = Convert.ToString(reader[3]);
                    customer.Email = Convert.ToString(reader[4]);
                    customer.Password = Convert.ToString(reader[5]);
                    customer.Address = Convert.ToString(reader[6]);
                    customer.PhoneNo = Convert.ToString(reader[7]);
                    customer.CreditCardNo = Convert.ToString(reader[8]);
                }

                reader.Close();

            });

            if (customer.Id == 0)
                return null;

            return customer;
        }

        /// <summary>
        /// This method checks if the given customer is valid and has all the required properties.
        /// </summary>
        public bool IsValidUser(Customer t)
        {
            // Validating the fields

            if (String.IsNullOrEmpty(t.FirstName))
                throw new InvalidCustomerException("A customer must have a first name");
            if (String.IsNullOrEmpty(t.LastName))
                throw new InvalidCustomerException("A customer must have a last name");
            if (String.IsNullOrEmpty(t.UserName))
                throw new InvalidCustomerException("A customer user must have a user name");
            if (String.IsNullOrEmpty(t.Email))
                throw new InvalidCustomerException("A customer user must have an email");
            if (String.IsNullOrEmpty(t.Password))
                throw new InvalidCustomerException("A customer user must have a password");
            if (String.IsNullOrEmpty(t.Address))
                throw new InvalidCustomerException("A customer must have an address");
            if (String.IsNullOrEmpty(t.PhoneNo))
                throw new InvalidCustomerException("A customer must have a phone number");
            if (String.IsNullOrEmpty(t.CreditCardNo))
                throw new InvalidCustomerException("A customer must have a credit card number");

            if (t.UserName == GlobalConfig.adminUserName)
                throw new UserNameAlreadyExistException("This user name is already taken.");

            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();

            // Checking if the user name is taken or not

            int customerId = 0;
            int airlineId = 0;

            string query = $"SELECT Customers.ID from Customers WHERE Customers.USER_NAME = '{t.UserName}'";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            MySqlDataReader reader = cmd.ExecuteReader();           

            while (reader.Read())
            {
                if (Convert.ToInt32(reader[0]) != t.Id)
                    customerId = Convert.ToInt32(reader[0]);
            }

            reader.Close();

            query = $"SELECT AirlineCompanies.ID from AirlineCompanies WHERE AirlineCompanies.USER_NAME = '{t.UserName}'";
            cmd = new MySqlCommand(query, conn);

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                airlineId = Convert.ToInt32(reader[0]);
            }

            reader.Close();

            if (airlineId != 0)
                throw new UserNameAlreadyExistException("This user name is already taken.");

            if (customerId != 0 && customerId != t.Id)
                    throw new UserNameAlreadyExistException("This user name is already taken.");

            conn.Close();

            return true;
        }
    }
}
