using FlightCenter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace FlightWebAPI.Controllers
{
    /// <summary>
    /// The customer facade controller handles all the customer's API calls. The customer can buy flight tickets, manage his purchased tickets 
    /// modify his details and change his password. A JWT Token with the customer role is needed to gain access to these methods.
    /// </summary>
    [EnableCors("*", "*", "*")]
    [JwtAuthentication(Role = "Customer")]
    public class CustomerFacadeController : ApiController
    {
        static readonly LoggedInCustomerFacade facade;

        static CustomerFacadeController()
        {
            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            facade = fcs.GetFacade(new LoginToken<Customer>()) as LoggedInCustomerFacade;
        }

        /// <summary>
        /// This method gets the JWT login token from the headers of the request and parse it.
        /// </summary>
        public LoginToken<Customer> GetLoginToken()
        {
            string jsonToken = ClaimsPrincipal.Current.Claims.FirstOrDefault(x => x.Type.Contains("LoginToken")).Value;
            LoginToken<Customer> token = JsonConvert.DeserializeObject<LoginToken<Customer>>(jsonToken);

            return token;
        }

        /// <summary>
        /// Gets all the flights which the customer owns ticket to.
        /// </summary>
        [HttpGet]
        [ResponseType(typeof(IList<JObject>))]
        [Route("api/customer/get-all-my-flights")]
        public IHttpActionResult GetAllMyFlights()
        {
            LoginToken<Customer> token = GetLoginToken();

            IList<JObject> flights = facade.GetAllMyFlightsJson(token);

            return Ok(flights);
        }

        [HttpPost]
        [Route("api/customer/purchase-ticket/{flightId}")]
        public IHttpActionResult PurchaseTicket([FromUri]int flightId)
        {
            LoginToken<Customer> token = GetLoginToken();

            facade.PurchaseTicket(token, flightId);

            return Ok();
        }

        [HttpDelete]
        [Route("api/customer/cancel-ticket/{ticketId}")]
        public IHttpActionResult CancelTicket([FromUri]int ticketId)
        {
            LoginToken<Customer> token = GetLoginToken();

            facade.CancelTicket(token, ticketId);

            return Ok();
        }

        [HttpPut]
        [Route("api/customer/modify-customer-details")]
        public IHttpActionResult ModifyCustomerDetails([FromBody]Customer customer)
        {
            LoginToken<Customer> token = GetLoginToken();

            customer.Id = token.User.Id;

            facade.ModifyCustomerDetails(token, customer);

            return Ok();
        }

        [HttpPut]
        [Route("api/customer/change-my-password")]
        public IHttpActionResult ChangeMyPassword([FromBody]string[] passwords)
        {
            LoginToken<Customer> token = GetLoginToken();

            string oldPassword = passwords[0];
            string newPassword = passwords[1];
            facade.ChangeMyPassword(token, oldPassword, newPassword);

            return Ok();
        }
    }
}
