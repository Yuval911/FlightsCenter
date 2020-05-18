using FlightCenter;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace FlightWebAPI.Controllers
{
    [EnableCors("*", "*", "*")]
    public class AuthController : ApiController
    {
        /// <summary>
        /// This method handles the login to the system for all accounts. 
        /// If the login succeeded it returns the generated JWT token.
        /// </summary>
        [HttpPost]
        [Route("api/authentication/get-token")]
        public IHttpActionResult GetToken([FromBody]string[] credentials)
        {
            string username = credentials[0];
            string password = credentials[1];

            LoginService loginService = new LoginService();
            ILoginToken loginToken;
            try
            {
                loginToken = loginService.TryLoginAllUsers(username, password);
            }
            catch (WrongPasswordException ex)
            {
                ex.GetType();
                return Unauthorized();
            }

            if (loginToken != null)
                return Ok(JwtAuthManager.GenerateJWTToken(loginToken));
            else
                return Unauthorized();
        }
    }
}
