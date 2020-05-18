using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This is the data access class of the tickets.
    /// It conatins the basic CRUD methods and some extras.
    /// </summary>
    public class TicketMySqlDAO : BasicMySqlDAO, ITicketDAO
    {
        // Basic DAO functions:

        public void Add(Ticket t)
        {
            TryCatchDatabaseFunction((conn) => {

                if (!IsValidTicket(t))
                    return;

                // 1. Checking if there are tickets available for the flight, and if the flight is not departured, landed or canceled.

                string query = $"SELECT * FROM Flights WHERE ID = {t.FlightId}";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                int remainingTickets = 0;

                while (reader.Read())
                {
                    remainingTickets = Convert.ToInt32(reader[6]);
                }

                reader.Close();

                if (remainingTickets == 0)
                    throw new NoTicketsRemainingException("This flight is fully booked and there are no tickets left.");

                // 2. Removing 1 from the flight's remaining tickets

                remainingTickets--;

                query = $"UPDATE Flights SET REMAINING_TICKETS = '{remainingTickets}' WHERE Flights.ID = {t.FlightId}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

                // 3. Creating new ticket

                query = $"INSERT INTO Tickets (FLIGHT_ID, CUSTOMER_ID) VALUES ('{t.FlightId}', '{t.CustomerId}')";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        /// <summary>
        /// Adds a list of tickets to the database. 
        /// When test mode is true there would be no constraints checking.
        /// </summary>
        public void AddRange(IList<Ticket> list, bool testMode)
        {
            TryCatchDatabaseFunction((conn) => {

                if (!testMode)
                {
                    foreach (Ticket ticket in list)
                    {
                        if (!IsValidTicket(ticket))
                            return;
                    }
                }

                if (list.Count == 0)
                    return;

                string query;
                MySqlCommand cmd;
                MySqlDataReader reader;

                if (!testMode)
                {
                    foreach (Ticket ticket in list)
                    {
                        // 1. Checking if there are tickets available for the flight, and if the flight is not departured, landed or canceled.

                        query = $"SELECT * FROM Flights WHERE ID = {ticket.FlightId}";
                        cmd = new MySqlCommand(query, conn);
                        reader = cmd.ExecuteReader();

                        int remainingTickets = 0;
                        string flightStatus = "";

                        while (reader.Read())
                        {
                            remainingTickets = Convert.ToInt32(reader[6]);
                            flightStatus = Convert.ToString(reader[7]);
                        }

                        reader.Close();

                        if (remainingTickets == 0)
                            throw new NoTicketsRemainingException("This flight is fully booked and there is no tickets left.");

                        // 2. Removing 1 from the flight's remaining tickets

                        remainingTickets--;

                        query = $"UPDATE Flights SET REMAINING_TICKETS = '{remainingTickets}' WHERE Flights.ID = {ticket.FlightId}";
                        cmd = new MySqlCommand(query, conn);
                        cmd.ExecuteNonQuery();

                        // 3. Creating new ticket

                        query = $"INSERT INTO Tickets (FLIGHT_ID, CUSTOMER_ID) VALUES ('{ticket.FlightId}', '{ticket.CustomerId}')";
                        cmd = new MySqlCommand(query, conn);
                        cmd.ExecuteNonQuery();
                    }
                }

                // if it's test mode (DB Generator) the tickets will be created without any validation:
                else
                {
                    query = "INSERT INTO Tickets (FLIGHT_ID, CUSTOMER_ID) VALUES ";

                    foreach (Ticket ticket in list)
                    {
                        query += $"('{ticket.FlightId}', '{ticket.CustomerId}'), ";
                    }

                    query = query.Remove(query.Count() - 2);

                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                }
            });
        }

        public Ticket Get(int id)
        {
            Ticket ticket = new Ticket();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Tickets WHERE Tickets.ID = {id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ticket.Id = Convert.ToInt32(reader[0]);
                    ticket.FlightId = Convert.ToInt32(reader[1]);
                    ticket.CustomerId = Convert.ToInt32(reader[2]);
                }

                reader.Close();

                if (ticket.Id == 0)
                    throw new TicketNotFoundException($"Ticket with ID: {id} doesn't exist");

            });

            return ticket;
        }

        public IList<Ticket> GetAll()
        {
            List<Ticket> tickets = new List<Ticket>();

            TryCatchDatabaseFunction((conn) => {

                string query = $"SELECT * FROM Tickets";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Ticket ticket = new Ticket();

                    ticket.Id = Convert.ToInt32(reader[0]);
                    ticket.FlightId = Convert.ToInt32(reader[1]);
                    ticket.CustomerId = Convert.ToInt32(reader[2]);

                    tickets.Add(ticket);
                }

                reader.Close();
            });

            return tickets;
        }

        public void Remove(int id)
        {
            TryCatchDatabaseFunction((conn) => {

                // Checking if the ticket exist:

                string query = $"SELECT * FROM Tickets WHERE Tickets.ID = {id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Ticket ticket = new Ticket();

                while (reader.Read())
                {
                    ticket.Id = Convert.ToInt32(reader[0]);
                    ticket.FlightId = Convert.ToInt32(reader[1]);
                }

                reader.Close();

                if (ticket.Id == 0)
                    throw new TicketNotFoundException($"Ticket with ID: {id} doesn't exist");

                // Deleting the ticket:

                query = $"DELETE FROM Tickets WHERE Tickets.ID = {id}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

                // Adding one ticket to the flight's vacancy:

                query = $"SELECT * FROM Flights WHERE ID = {ticket.FlightId}";
                cmd = new MySqlCommand(query, conn);
                reader = cmd.ExecuteReader();

                int remainingTickets = 0;

                while (reader.Read())
                {
                    remainingTickets = Convert.ToInt32(reader[6]);
                }

                reader.Close();

                remainingTickets++;

                query = $"UPDATE Flights SET REMAINING_TICKETS = '{remainingTickets}' WHERE Flights.ID = {ticket.FlightId}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        public void Update(Ticket t)
        {
            throw new CannotUpdateTicketException("It is not possible to update a ticket. Please cancel it and buy a new one.");
        }

        // Extra methods:

        public void RemoveTicketsByCustomerId(int id)
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

                // Deleting the tickets:

                query = $"DELETE FROM Tickets WHERE Tickets.CUSTOMER_ID = {id}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        public void RemoveTicketsByFlightId(int id)
        {
            TryCatchDatabaseFunction((conn) => {

                // Checking if the flight exist:

                string query = $"SELECT * FROM Flights WHERE Flights.ID = {id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                Flight flight = new Flight();

                while (reader.Read())
                {
                    flight.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (flight.Id == 0)
                    throw new FlightNotFoundException($"Flight with ID: {id} doesn't exist.");

                // Deleting the tickets:

                query = $"DELETE FROM Tickets WHERE Tickets.FLIGHT_ID = {id}";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

            });
        }

        public IList<Ticket> GetTicketsByAirlineId(int id)
        {
            List<Ticket> tickets = new List<Ticket>();

            TryCatchDatabaseFunction((conn) => {

                // Checking if the airline exist:

                string query = $"SELECT * FROM AirlineCompanies WHERE AirlineCompanies.ID = {id}";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                AirlineCompany airlineCompany = new AirlineCompany();

                while (reader.Read())
                {
                    airlineCompany.Id = Convert.ToInt32(reader[0]);
                }

                reader.Close();

                if (airlineCompany.Id == 0)
                    throw new AirlineCompanyNotFoundException($"Airline Company with ID: {id} doesn't exist");

                // Getting the tickets:

                query = $"SELECT Tickets.ID, Tickets.FLIGHT_ID, Tickets.CUSTOMER_ID FROM Tickets " +
                               $"JOIN Flights ON Tickets.FLIGHT_ID = Flights.ID " +
                               $"WHERE Flights.AIRLINE_COMPANY_ID = {id}";
                cmd = new MySqlCommand(query, conn);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Ticket ticket = new Ticket();

                    ticket.Id = Convert.ToInt32(reader[0]);
                    ticket.FlightId = Convert.ToInt32(reader[1]);
                    ticket.CustomerId = Convert.ToInt32(reader[2]);

                    tickets.Add(ticket);
                }

                reader.Close();
            });

            return tickets;
        }

        /// <summary>
        /// This method checks if the given ticket is valid and has all the required properties.
        /// </summary>
        public bool IsValidTicket(Ticket t)
        {
            // Validating the fields:

            if (t.FlightId == 0)
                throw new InvalidTicketException("A ticket must have a flight.");
            if (t.CustomerId == 0)
                throw new InvalidTicketException("A ticket must have a customer.");

            // Checking if a flight with that ID exists:

            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = $"SELECT * FROM Flights WHERE Flights.ID = {t.FlightId}";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            MySqlDataReader reader = cmd.ExecuteReader();

            Flight flight = new Flight();

            while (reader.Read())
            {
                flight.Id = Convert.ToInt32(reader[0]);
            }

            reader.Close();

            if (flight.Id == 0)
                throw new FlightNotFoundException($"Flight with ID: {t.FlightId} doesn't exist.");

            // Checking if a ticket with the same customer and flight id already exists:

            int ticketId = 0;

            query = $"SELECT * FROM Tickets WHERE Tickets.FLIGHT_ID = {t.FlightId} AND Tickets.CUSTOMER_ID = {t.CustomerId}";
            cmd = new MySqlCommand(query, conn);
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ticketId = Convert.ToInt32(reader[0]);
            }

            reader.Close();

            if (ticketId != 0)
                throw new OneTicketForCustomerOnlyException("A customer cannot buy two tickets for the same flight.");

            conn.Close();

            return true;
        }
    }
}
