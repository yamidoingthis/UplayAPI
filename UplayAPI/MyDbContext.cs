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
        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }
		public DbSet<Vendor> Vendors { get; set; }
		public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
    }
}
