using FlightCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;

namespace FlightWebAPI.Controllers
{
    /// <summary>
    /// The anonymous facade contoroller handles all the API calls that regarding to "public" information like flights, countries, 
    /// airline companies, etc. Every user can access these methods.
    /// </summary>
    [EnableCors("*","*", "*")]
    public class AnonymousFacadeController : ApiController
    {
        static readonly AnonymousUserFacade facade;

        static AnonymousFacadeController()
        {
            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            facade = fcs.GetFacade(new LoginToken<Anonymous>()) as AnonymousUserFacade;
        }

        [HttpGet]
        [ResponseType(typeof(IList<Flight>))]
        [Route("api/guest/get-all-flights")]
        public IHttpActionResult GetAllFlights()
        {
            IList<Flight> flights = facade.GetAllFlights();

            return Ok(flights);
        }
        
        [HttpGet]
        [ResponseType(typeof(IList<AirlineCompany>))]
        [Route("api/guest/get-all-airlines")]
        public IHttpActionResult GetAllAirlineCompanies()
        {
            IList<AirlineCompany> airlineCompanies = facade.GetAllAirlineCompanies();

            return Ok(airlineCompanies);
        }

        [HttpGet]
        [ResponseType(typeof(IDictionary<Flight,int>))]
        [Route("api/guest/get-all-flights-vacancy")]
        public IHttpActionResult GetAllFlightsVacancy()
        {
            IDictionary<Flight, int> flightsVacancy = facade.GetAllFlightsVacancy();

            return Ok(flightsVacancy);
        }

        [HttpGet]
        [ResponseType(typeof(Flight))]
        [Route("api/guest/get-flight/{id}")]
        public IHttpActionResult GetFlightById([FromUri] int id)
        {
            Flight flight = facade.GetFlightById(id);

            return Ok(flight);
        }

        [HttpGet]
        [ResponseType(typeof(JObject))]
        [Route("api/guest/get-flight-json/{id}")]
        public IHttpActionResult GetFlightByIdJson([FromUri] int id)
        {
            JObject flight = facade.GetFlightByIdJson(id);

            return Ok(flight);
        }

        [HttpGet]
        [ResponseType(typeof(IList<Flight>))]
        [Route("api/guest/get-flights-by-origin-country/search")]
        public IHttpActionResult GetFlightsByOriginCountry(int id = 0)
        {
            IList<Flight> flights = facade.GetFlightsByOriginCountry(id);

            return Ok(flights);
        }

        [HttpGet]
        [ResponseType(typeof(IList<Flight>))]
        [Route("api/guest/get-flights-by-destination-country/search")]
        public IHttpActionResult GetFlightsByDestinationCountry(int id = 0)
        {
            IList<Flight> flights = facade.GetFlightsByDestinationCountry(id);

            return Ok(flights);
        }

        [HttpGet]
        [ResponseType(typeof(IList<Flight>))]
        [Route("api/guest/get-flights-by-departure-date/{date}")]
        public IHttpActionResult GetFlightsByDepartureDate([FromUri] DateTime date)
        {
            IList<Flight> flights = facade.GetFlightsByDepartureDate(date);

            return Ok(flights);
        }

        [HttpGet]
        [ResponseType(typeof(IList<Flight>))]
        [Route("api/guest/get-flights-by-landing-date/{date}")]
        public IHttpActionResult GetFlightsByLadnigDate([FromUri] DateTime date)
        {
            IList<Flight> flights = facade.GetFlightsByLandingDate(date);

            return Ok(flights);
        }

        [HttpGet]
        [ResponseType(typeof(IList<Flight>))]
        [Route("api/guest/get-flights-by-price-range/price")]
        public IHttpActionResult GetFlightsByPriceRange(int min = 0, int max = 0)
        {
            IList<Flight> flights = facade.GetFlightsByPriceRange(min, max);

            return Ok(flights);
        }

        [HttpGet]
        [ResponseType(typeof(IList<Country>))]
        [Route("api/guest/get-all-countries")]
        public IHttpActionResult GetAllCountries()
        {
            IList<Country> countries = facade.GetAllCountries();

            return Ok(countries);
        }

        [HttpGet]
        [ResponseType(typeof(IList<JObject>))]
        [Route("api/guest/get-arrival-flights")]
        public IHttpActionResult GetArrivalFlights()
        {
            IList<JObject> flights = facade.GetArrivalFlights();

            return Ok(flights);
        }

        [HttpGet]
        [ResponseType(typeof(IList<JObject>))]
        [Route("api/guest/get-departure-flights")]
        public IHttpActionResult GetDepartureFlights()
        {
            IList<JObject> flights = facade.GetDepartureFlights();

            return Ok(flights);
        }

        [HttpGet]
        [ResponseType(typeof(IList<JObject>))]
        [Route("api/guest/get-oneway-flights/query")]
        public IHttpActionResult GetOneWayFlightsByQuery(DateTime departureDate, int originCountryId = 0, int destinationCountryId = 0)
        {
            IList<JObject> flights = facade.GetOneWayFlightsByQuery(originCountryId, destinationCountryId, departureDate);

            return Ok(flights);
        }

        [HttpGet]
        [ResponseType(typeof(IList<IList<JObject>>))]
        [Route("api/guest/get-roundtrip-flights/query")]
        public IHttpActionResult GetRoundtripFlights(DateTime departureDate, DateTime returnDate, int originCountryId = 0, int destinationCountryId = 0)
        {
            IList<IList<JObject>> flights = facade.GetRoundtripFlightsByQuery(originCountryId, destinationCountryId, departureDate, returnDate);

            return Ok(flights);
        }

        [HttpPost]
        [Route("api/guest/sign-up")]
        public async Task<IHttpActionResult> SignUp([FromBody] Customer customer)
        {
            facade.SignUp(customer);

            await SendCustomerRegistrationEmail(customer);

            return Ok();
        }

        /// <summary>
        /// This method was created for the sign up form. It checks if the given username already exists in the database.
        /// </summary>
        [HttpGet]
        [ResponseType(typeof(bool))]
        [Route("api/guest/check-username/{username}")]
        public IHttpActionResult CheckUsernameAvailability([FromUri] string username)
        {
            bool isAvailable = facade.CheckUsernameAvailability(username);

            return Ok(isAvailable);
        }

        /// <summary>
        /// When a new airline company completes its registration, this method adds it to the register queue, where it will wait
        /// untill the administrator aproves it.
        /// </summary>
        [HttpPost]
        [Route("api/guest/add-airline-to-register-queue")]
        public IHttpActionResult AddAirlineToRegisterQueue([FromBody] AirlineCompany airline)
        {
            facade.AddAirlineToRegisterQueue(airline);

            return Ok();
        }

        /// <summary>
        /// When a new customer signs up, this methods will send him an email.
        /// </summary>
        private async Task<HttpStatusCode> SendCustomerRegistrationEmail(Customer customer)
        {
            string apiKey = Properties.AppResources.SendGridAPIKey;

            SendGridClient client = new SendGridClient(apiKey);
            EmailAddress from = new EmailAddress("admin@flightscenter.com", "Flights Center");
            var subject = "Welcome to Flights Center!";
            var to = new EmailAddress(customer.Email, $"{customer.GetFullName()}");
            var plainTextContent = $"Welcome aboard, {customer.FirstName}!\n\n"
                                  + "Your account is now created. We hope you will enjoy our website!\n\n"
                                  + "If you have any questions, be sure to contact us.\n\n"
                                  + "Sincerely, The website manager.\n\n"
                                  + "(This is an automatic e-mail. Please do not reply.)";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, "");
            var response = await client.SendEmailAsync(msg);
            
            return response.StatusCode;
        }
    }
}
