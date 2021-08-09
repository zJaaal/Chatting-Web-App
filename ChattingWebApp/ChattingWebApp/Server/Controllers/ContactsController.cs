using ChattingWebApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return await context.Profiles.ToListAsync();
        }
        [HttpGet("getcontacts/{filter}")]
        public async Task<List<Profile>> GetProfilesByFilterAsync(string filter)
        {
            return await context.Profiles.Where( p => p.Nickname.Contains(filter)).ToListAsync();
        }
    }
}
