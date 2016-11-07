using Microsoft.EntityFrameworkCore;

namespace MyTripz.Models
{
    public class TripzContext : DbContext
    {
        public TripzContext(DbContextOptions<TripzContext> options) : base(options)
        {

        }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Stop> Stops { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var connString = Startup.Configuration["ConnectionStrings:TripzContextConnection"];
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
