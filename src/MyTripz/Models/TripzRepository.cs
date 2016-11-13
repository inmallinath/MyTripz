using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyTripz.Models
{
    public class TripzRepository : ITripzRepository
    {
        private readonly TripzContext _context;

        public TripzRepository(TripzContext context)
        {
            _context = context;
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            return _context.Trips.OrderBy(t => t.Name).ToList();
        }

        public IEnumerable<Trip> GetTripsWithStops()
        {
            return _context.Trips
                .Include(t => t.Stops)
                .OrderBy(t => t.Name)
                .ToList();
        }
    }
}
