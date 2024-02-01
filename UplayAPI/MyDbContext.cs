using UplayAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace UplayAPI
{
    public class MyDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public MyDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder
        optionsBuilder)
        {
            string? connectionString = _configuration.GetConnectionString(
            "MyConnection");
            if (connectionString != null)
            {
                optionsBuilder.UseMySQL(connectionString);
            }
        }
		public DbSet<Activity> Activities { get; set; }
		public DbSet<Vendor> Vendors { get; set; }
		public DbSet<Booking> Bookings { get; set; }
    }
}
