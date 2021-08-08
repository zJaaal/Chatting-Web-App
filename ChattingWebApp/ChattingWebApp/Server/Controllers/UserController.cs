using ChattingWebApp.Shared;
using ChattingWebApp.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChattingWebApp.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<AppDbContext> logger;
        private readonly AppDbContext context;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<AppDbContext> logger, AppDbContext context, IConfiguration configuration)
        {
            this.logger = logger;
            this.context = context;
            _configuration = configuration;
        }

        [HttpPost("registeruser")]
        public async Task<ActionResult> RegisterUser(User user)
        {
            try
            {
                user.Password = Shared.Utility.Encrypt(user.Password);
                user.RepeatPassword = Shared.Utility.Encrypt(user.RepeatPassword);
                user.Profile = new Profile()
                {
                    LastTimeConnected = DateTime.Now,
                    AboutMe = "Hi! I'm using this brand new app."
                };

                context.Add(user);
                await context.SaveChangesAsync();

                return new CreatedAtRouteResult("getuserbyid", new { UserID = user.UserID }, user);
            }
            catch(Exception)
            {
                return BadRequest();
            }
         }
        [HttpGet("{id}",Name = "getuserbyid")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.UserID == id);
        }
        [HttpGet("getuserbynickname/{nickname}")]
        public async Task<User> GetUserByNickname(string nickname)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Nickname == nickname);
            return user is null ? await Task.FromResult(new User { Nickname = ""}) : await Task.FromResult(new User() { Nickname = user.Nickname });
        }

        private string GenerateJwtToken(User user)
        {
            //getting the secret key
            string secretKey = _configuration["jwt:key"];
            var key = Encoding.ASCII.GetBytes(secretKey);

            //create claims
            var claimEmail = new Claim(ClaimTypes.Name, user.Nickname);
            var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString());

            //create claimsIdentity
            var claimsIdentity = new ClaimsIdentity(new[] { claimEmail, claimNameIdentifier }, "serverAuth");

            // generate token that is valid for 7 days
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //creating a token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //returning the token back
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("authenticatejwt")]
        public async Task<ActionResult<AuthenticationResponse>> AuthenticateJWT(AuthenticationRequest authenticationRequest)
        {
            string token = string.Empty;

            //checking if the user exists in the database
            authenticationRequest.Password = Shared.Utility.Encrypt(authenticationRequest.Password);
            User loggedInUser = await context.Users
                        .Where(u => u.Nickname == authenticationRequest.Nickname && u.Password == authenticationRequest.Password)
                        .FirstOrDefaultAsync();

            if (loggedInUser != null)
            {
                //generating the token
                token = GenerateJwtToken(loggedInUser);
            }
            return await Task.FromResult(new AuthenticationResponse() { Token = token });
        }
        [HttpPost("getuserbyjwt")]
        public async Task<ActionResult<User>> GetUserByJWT([FromBody] string jwtToken)
        {
            try
            {
                //getting the secret key
                string secretKey = _configuration["jwt:key"];
                var key = Encoding.ASCII.GetBytes(secretKey);

                //preparing the validation parameters
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;

                //validating the token
                var principle = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = (JwtSecurityToken)securityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    //returning the user if found
                    var userId = principle.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    return await context.Users.Where(u => u.UserID == Convert.ToInt64(userId)).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                //logging the error and returning null
                Console.WriteLine("Exception : " + ex.Message);
                return null;
            }
            //returning null if token is not validated
            return null;
        }
    }
}
