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
    /// The airline facade controller handles all the airline company's API calls. The airline can manage its flights, modify its details
    /// and change its password. A JWT Token with the airline company role is needed to gain access to these methods.
    /// </summary>
    [EnableCors("*", "*", "*")]
    [JwtAuthentication(Role = "AirlineCompany")]
    public class AirlineFacadeController : ApiController
    {
        static readonly LoggedInAirlineFacade facade;

        static AirlineFacadeController()
        {
            FlightCenterSystem fcs = FlightCenterSystem.GetInstance();

            facade = fcs.GetFacade(new LoginToken<AirlineCompany>()) as LoggedInAirlineFacade;
        }
        
        /// <summary>
        /// This method gets the JWT login token from the headers of the request and parse it.
        /// </summary>
        public LoginToken<AirlineCompany> GetLoginToken()
        {
            string jsonToken = ClaimsPrincipal.Current.Claims.FirstOrDefault(x => x.Type.Contains("LoginToken")).Value;
            LoginToken<AirlineCompany> token = JsonConvert.DeserializeObject<LoginToken<AirlineCompany>>(jsonToken);

            return token;
        }
        
        [HttpGet]
        [ResponseType(typeof(IList<Flight>))]
        [Route("api/airline/get-all-flights")]
        public IHttpActionResult GetAllFlights()
        {
            LoginToken<AirlineCompany> token = GetLoginToken();

            IList<Flight> flights = facade.GetAllMyFlights(token);

            return Ok(flights);
        }

        /// <summary>
        /// Gets all the tickets of flights that belong to this airline company.
        /// </summary>
        [HttpGet]
        [ResponseType(typeof(IList<Ticket>))]
        [Route("api/airline/get-all-tickets")]
        public IHttpActionResult GetAllTickets()
        {
            LoginToken<AirlineCompany> token = GetLoginToken();

            IList<Ticket> tickets = facade.GetAllTickets(token);

            return Ok(tickets);
        }

        [HttpPost]
        [Route("api/airline/create-flight")]
        public IHttpActionResult CreateFlight([FromBody]Flight flight)
        {
            LoginToken<AirlineCompany> token = GetLoginToken();

            facade.CreateFlight(token, flight);

            return Ok();
        }

        [HttpPut]
        [Route("api/airline/update-flight")]
        public IHttpActionResult UpdateFlight([FromBody]Flight flight)
        {
            LoginToken<AirlineCompany> token = GetLoginToken();

            facade.UpdateFlight(token, flight);

            return Ok();
        }

        [HttpDelete]
        [Route("api/airline/cancel-flight/{id}")]
        public IHttpActionResult CancelFlight([FromUri]int id)
        {
            LoginToken<AirlineCompany> token = GetLoginToken();

            facade.CancelFlight(token, id);

            return Ok();
        }

        [HttpPut]
        [Route("api/airline/change-my-password")]
        public IHttpActionResult ChangeMyPassword([FromBody]string[] passwords)
        {
            LoginToken<AirlineCompany> token = GetLoginToken();

            string oldPassword = passwords[0];
            string newPassword = passwords[1];
            facade.ChangeMyPassword(token, oldPassword, newPassword);

            return Ok();
        }

        [HttpPut]
        [Route("api/airline/modify-airline-details")]
        public IHttpActionResult ModifyAirlineDetails([FromBody]AirlineCompany airlineCompany)
        {
            LoginToken<AirlineCompany> token = GetLoginToken();

            airlineCompany.Id = token.User.Id;

            facade.ModifyAirlineDetails(token, airlineCompany);

            return Ok();
        }
    }
}
