using FlightCenter;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace FlightWebAPI
{
    /// <summary>
    /// This class is managing the creation of the token at login.
    /// </summary>
    public static class JwtAuthManager
    {
        private static string SecretKey = Properties.AppResources.JWTSecurityKey;

        public static string GenerateJWTToken(ILoginToken loginToken, int expire_in_Minutes = 120)
        {
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var securitytokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = ClaimsIdentityBuilder(loginToken),

                Expires = now.AddMinutes(Convert.ToInt32(expire_in_Minutes)),

                SigningCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(securitytokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = jwtTokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = symmetricKey
                };

                SecurityToken securityToken;
                var principal = jwtTokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;
            }
            catch (Exception e)
            {
                e.GetType();
                return null;
            }
        }

        public static ClaimsIdentity ClaimsIdentityBuilder(ILoginToken loginToken)
        {
            LoginToken<Administrator> adminToken = loginToken as LoginToken<Administrator>;

            if (adminToken != null)
            {
                Claim[] claims =
                {
                    new Claim("LoginToken",JsonConvert.SerializeObject(adminToken)),
                    new Claim(ClaimTypes.Role, "Administrator")
                };

                return new ClaimsIdentity(claims);
            }

            LoginToken<AirlineCompany> airlineToken = loginToken as LoginToken<AirlineCompany>;

            if (airlineToken != null)
            {
                Claim[] claims =
                {
                    new Claim("LoginToken",JsonConvert.SerializeObject(airlineToken)),
                    new Claim(ClaimTypes.Role, "AirlineCompany")
                };

                return new ClaimsIdentity(claims);
            }

            LoginToken<Customer> customerToken = loginToken as LoginToken<Customer>;

            if (customerToken != null)
            {
                Claim[] claims =
{
                    new Claim("LoginToken",JsonConvert.SerializeObject(customerToken)),
                    new Claim(ClaimTypes.Role, "Customer")
                };

                return new ClaimsIdentity(claims);
            }

            return null;
        }
    }
}