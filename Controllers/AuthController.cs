using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DemoJWTAuthenticate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config; // to get key, issuer from appsettings.json

        public AuthController(IConfiguration config) //Declare DI
        {
            _config = config;
        }

        [HttpPost("token")]
        public ActionResult getAccessToken(LoginModel model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // check username/password
            if(model.Username == "admin" && model.Password == "admin")
            {
                // generate token and return it
                // add claims
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, "ur username or fullname"));
                claims.Add(new Claim(ClaimTypes.Email, "ur email"));
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
                claims.Add(new Claim("custom", "I like chicken"));

                // create credentials
                var secret_key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]);
                var symmetricSecurityKey = new SymmetricSecurityKey(secret_key);
                var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                // create token by handler
                var access_token = new JwtSecurityToken(
                    issuer: _config["JwtSettings:Issuer"],
                    audience: _config["JwtSettings:Issuer"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(3),
                    signingCredentials: credentials
                    );

                return Ok(tokenHandler.WriteToken(access_token));
            }
            return BadRequest("Invalid");
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
