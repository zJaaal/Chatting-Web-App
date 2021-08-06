using ChattingWebApp.Shared;
using ChattingWebApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ChattingWebApp.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<AppDbContext> logger;
        private readonly AppDbContext context;

        public UserController(ILogger<AppDbContext> logger, AppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [HttpPost("registeruser")]
        public async Task<ActionResult> RegisterUser(User user)
        {
            try
            {
                user.Password = Utility.Encrypt(user.Password);
                user.RepeatPassword = Utility.Encrypt(user.RepeatPassword);
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
    }
}
