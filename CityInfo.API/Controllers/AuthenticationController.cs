using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using CityInfo.API.Models.Authentication;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        /*
            Token consists of a header (contains used key algorithm for signing the token),
            payload contains some data, and signature to sign or hash the payload with a key.
            Secret ==> Key ==> Use the key for signing.
         
            Token is encoded not encrypted, the encryption rely on https connection.
            
            Most web servers don't log body request, this is why we are sending user, and 
            password in the request's body.
         */

        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? 
                throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("authenticate")]
        public ActionResult<AuthenticationDto> Authenticate
            ([FromBody] AuthenticationCreationDto authenticationCreationDto)
        {
            var user = ValidateUserCredential(
                authenticationCreationDto.UserName, 
                authenticationCreationDto.Password
            );

            if (user == null)
                return Unauthorized();

            // Create a key from a secret, then set the signing algorithm, payload, and then the token.
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["Authentication:SecretKey"]));
            
            var signingCredentials = 
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.Now,
                DateTime.Now.AddHours(1),       // in seconds from 1/1/1970
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return Ok(
                new AuthenticationDto()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    City = user.City,
                    Token = tokenToReturn
                });
        }

        private CityInfoUser ValidateUserCredential(string? userName, string? password)
        {
            // There is no user DB to check with, so it is assumed that the credentials are valid

            return new CityInfoUser(
                1, 
                userName ?? ""
                , "Kevin", 
                "Dockx", 
                "Antwerp");
        }
    }
}
