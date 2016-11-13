using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTripz.Models
{
    public class TripzRepository : ITripzRepository
    {
        private readonly TripzContext _context;
        private readonly ILogger<TripzRepository> _logger;

        public TripzRepository(TripzContext context, ILogger<TripzRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            try
            {
                return _context.Trips.OrderBy(t => t.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get Trips from Database", ex);
                return null;
            }
        }

        public IEnumerable<Trip> GetTripsWithStops()
        {
            try
            {
                return _context.Trips
                        .Include(t => t.Stops)
                        .OrderBy(t => t.Name)
                        .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not get Trips with Stops from Database", ex);
                return null;
            }
        }
    }
}
