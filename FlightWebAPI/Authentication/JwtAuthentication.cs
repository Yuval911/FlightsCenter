using FlightCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace FlightWebAPI
{
    /// <summary>
    /// This class is managing the authorization of a given token.
    /// </summary>
    public class JwtAuthentication : Attribute, IAuthenticationFilter
    {
        public string Role { get; set; }
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            // checking request header value having required scheme "Bearer" or not.
            if (authorization == null || authorization.Scheme != "Bearer" || string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthFailureResult("JWT Token is Missing", request);
                return;
            }

            // Getting Token value from header values.
            var token = authorization.Parameter;
            ClaimsIdentity claims;

            var principal = await AuthJwtToken(token, out claims);

            if (!ValidateRole(claims))
            {
                context.ErrorResult = new AuthFailureResult("Unathorized", request);
            }
            else if (principal == null)
            {
                context.ErrorResult = new AuthFailureResult("Invalid JWT Token", request);
            }
            else
            {
                context.Principal = principal;
            }           
        }

        private bool ValidateRole(ClaimsIdentity claims)
        {
            if (this.Role == "AnyRole")
                return true;

            string identityRole = "";
            try
            {
                identityRole = claims.Claims.FirstOrDefault(x => x.Type.Contains("claims/role")).Value;
            }
            catch (NullReferenceException)
            {
                return false;
            }

            if (identityRole == this.Role)
                return true;
            else
                return false;
        }

        private static bool ValidateToken(string token, out ClaimsIdentity identity)
        {
            identity = new ClaimsIdentity();

            var simplePrinciple = JwtAuthManager.GetPrincipal(token);

            if (simplePrinciple == null)
                return false;

            identity = simplePrinciple.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            return true;
        }

        protected Task<IPrincipal> AuthJwtToken(string token, out ClaimsIdentity claimsIdentity)
        {
            claimsIdentity = new ClaimsIdentity();

            if (ValidateToken(token, out claimsIdentity))
            {
                IPrincipal user = new ClaimsPrincipal(claimsIdentity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }
    }
}