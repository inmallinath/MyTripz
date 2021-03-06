﻿using System.Collections.Generic;

namespace MyTripz.Models
{
    public interface ITripzRepository
    {
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetTripsWithStops();
        void AddTrip(Trip newTrip);
        bool SaveAll();
        Trip GetTripByName(string tripName);
        void AddStop(string tripName, Stop newStop);
    }
}