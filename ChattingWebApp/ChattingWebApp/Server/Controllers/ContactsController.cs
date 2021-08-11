using ChattingWebApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChattingWebApp.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly AppDbContext context;
        public ContactsController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("getcontacts")]
        public async Task<List<Profile>> GetProfilesAsync()
        {
            return await context.Profiles.OrderBy(p => p.Nickname).ToListAsync();
        }
        [HttpGet("getcontacts/{filter}")]
        public async Task<List<Profile>> GetProfilesByFilterAsync(string filter)
        {
            return await context.Profiles.Where( p => p.Nickname.Contains(filter)).OrderBy(p => p.Nickname).ToListAsync();
        }
        [HttpGet("getprofile/{id}")]
        public async Task<ActionResult<Profile>> GetProfile(int id)
        {
            var profile = await context.Profiles.FirstOrDefaultAsync(x => x.UserID == id);
            if (profile == null)
                return await Task.FromResult(new Profile() { Nickname = ""});

            return await Task.FromResult(profile);
        }
        [HttpGet("getcurrentprofile")]
        public async Task<ActionResult<Profile>> GetCurrentProfile()
        {
            Profile currentProfile = new();
            if (User.Identity.IsAuthenticated)
            {
                var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                currentProfile = await context.Profiles.Where(u => u.UserID.ToString() == id).FirstOrDefaultAsync();
            }
            return await Task.FromResult(currentProfile);
        }
        [HttpPut("updateprofile/{id}")]
        public async Task<ActionResult<Profile>> UpdateProfile(int id,[FromBody] Profile profile)
        {
            var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if ( currentId == id.ToString())
            {
                var currentUser = await context.Profiles.FirstOrDefaultAsync(x => x.UserID == id);
                currentUser.Nickname = profile.Nickname;
                currentUser.ProfilePhoto = profile.ProfilePhoto;
                currentUser.AboutMe = profile.AboutMe;

                await context.SaveChangesAsync();
                return await Task.FromResult(profile);
            }
            return await Task.FromResult(BadRequest());
        }
    }
}
