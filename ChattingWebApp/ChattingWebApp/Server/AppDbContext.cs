using ChattingWebApp.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace ChattingWebApp.Server
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Message> Messages { get; set; }

    }
}
