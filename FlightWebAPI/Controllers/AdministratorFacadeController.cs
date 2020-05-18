using FlightCenter;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace FlightWebAPI.Controllers
{
    /// <summary>
    /// The administrator facade controller handles all the administrator's API calls. The administrator can perform actions that no
    /// other user can, like getting customers information, updating and deleting accounts, etc.
    /// A JWT Token with the administrator role is needed to gain access to these methods.
    /// </summary>
    [EnableCors("*", "*", "*")]
    [JwtAuthentication(Role="Administrator")]
    public class AdministratorFacadeController : ApiController
    {
        static readonly LoggedInAdministratorFacade facade;

        static readonly LoginToken<Administrator> token = new LoginToken<Administrator>();

        public AdministratorFacadeController()
        {
            
        }
        
        static AdministratorFacadeController()
        {
            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            facade = fcs.GetFacade(new LoginToken<Administrator>()) as LoggedInAdministratorFacade;
        }
        
        [HttpGet]
        [ResponseType(typeof(IList<Customer>))]
        [Route("api/admin/get-all-customers")]
        public IHttpActionResult GetAllCustomers()
        {
            IList<Customer> customers = facade.GetAllCustomers(token);

            return Ok(customers);
        }

        [HttpPost]
        [Route("api/admin/create-new-customer")]
        public IHttpActionResult CreateNewCustomer([FromBody]Customer customer)
        {
            facade.CreateNewCustomer(token, customer);

            return Ok();
        }

        [HttpPut]
        [Route("api/admin/update-customer-details")]
        public IHttpActionResult UpdateCustomerDetails([FromBody]Customer customer)
        {
            facade.UpdateCustomerDetails(token, customer);

            return Ok();
        }

        [HttpDelete]
        [Route("api/admin/remove-customer")]
        public IHttpActionResult RemoveCustomer([FromBody]Customer customer)
        {
            facade.RemoveCustomer(token, customer);

            return Ok();
        }

        [HttpPut]
        [Route("api/admin/update-airline-details")]
        public IHttpActionResult UpdateAirlineDetails([FromBody]AirlineCompany airline)
        {
            facade.UpdateAirlineDetails(token, airline);

            return Ok();
        }

        [HttpDelete]
        [Route("api/admin/remove-airline")]
        public IHttpActionResult RemoveAirline([FromBody]AirlineCompany airline)
        {
            facade.RemoveAirline(token, airline);
            
            return Ok();
        }

        /// <summary>
        /// Gets all the airline companies from the register queue.
        /// </summary>
        [HttpGet]
        [ResponseType(typeof(IList<AirlineCompany>))]
        [Route("api/admin/get-all-airlines-from-register-queue")]
        public IHttpActionResult GetAllAirlinesFromRegisterQueue()
        {
            IList<AirlineCompany> airlines = facade.GetAllAirlinesFromRegisterQueue(token);

            return Ok(airlines);
        }

        /// <summary>
        /// Every new airline company has to wait for administrator to aprove its registration and create the account for it.
        /// This method handles the account creation and also sends an email to the airline.
        /// </summary>
        [HttpPost]
        [Route("api/admin/create-new-airline")]
        public async Task<IHttpActionResult> CreateNewAirline([FromBody]AirlineCompany airline)
        {
            facade.CreateNewAirline(token, airline);

            facade.RemoveAirlineFromRegisterQueue(token, airline);

            await SendAirlineRegistrationEmail(airline);

            return Ok();
        }

        /// <summary>
        /// When the administrator decides not to aprove a new airline company, This method removes it from the queue and 
        /// sends an email to inform it.
        /// </summary>
        [HttpDelete]
        [Route("api/admin/reject-airline-in-register-queue")]
        public async Task<IHttpActionResult> RejectAirlineInRegisterQueue([FromBody]AirlineCompany airline)
        {
            facade.RemoveAirlineFromRegisterQueue(token, airline);

            await SendAirlineRejectEmail(airline);

            return Ok();
        }

        /// <summary>
        /// Handles the airline registration email sending.
        /// </summary>
        private async Task<HttpStatusCode> SendAirlineRegistrationEmail(AirlineCompany airline)
        {
            string apiKey = Properties.AppResources.SendGridAPIKey;

            SendGridClient client = new SendGridClient(apiKey);
            EmailAddress from = new EmailAddress("admin@flightscenter.com", "Flights Center");
            var subject = "Welcome to Flights Center!";
            var to = new EmailAddress(airline.Email, $"{airline.Name}");
            var plainTextContent = $"Welcome aboard, {airline.Name}!\n\n"
                                  + "Your account is now created. We hope you will make the best use of our website and increase your sales!\n\n"
                                  + "You may now start adding all your flights to the system.\n\n"
                                  + "If you have any questions, be sure to contact us.\n\n"
                                  + "Sincerely, The website manager.\n\n"
                                  + "(This is an automatic e-mail. Please do not reply.)";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, "");
            var response = await client.SendEmailAsync(msg);

            return response.StatusCode;
        }

        /// <summary>
        /// Handles the airline rejection email sending.
        /// </summary>
        private async Task<HttpStatusCode> SendAirlineRejectEmail(AirlineCompany airline)
        {
            string apiKey = Properties.AppResources.SendGridAPIKey;

            SendGridClient client = new SendGridClient(apiKey);
            EmailAddress from = new EmailAddress("admin@flightscenter.com", "Flights Center");
            var subject = "Your recent registration to Flights Center";
            var to = new EmailAddress(airline.Email, $"{airline.Name}");
            var plainTextContent = $"Hello, {airline.Name}.\n\n"
                                  + "After all your details was reviewed, the management decided not to aprove your account creation.\n\n"
                                  + "We are deeply sorry for the inconvenience.\n\n"
                                  + "If you have any questions, be sure to contact us.\n\n"
                                  + "Sincerely, The website manager.\n\n"
                                  + "(This is an automatic e-mail. Please do not reply.)";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, "");
            var response = await client.SendEmailAsync(msg);

            return response.StatusCode;
        }
    }
}
