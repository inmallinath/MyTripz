using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyTripz.Models;
using MyTripz.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;

namespace MyTripz.Controllers.Api
{
    [Route("api/trips")]
    public class TripzController : Controller
    {
        private readonly ILogger<TripzController> _logger;
        private readonly ITripzRepository _repository;

        public TripzController(ITripzRepository repository, ILogger<TripzController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]
        public JsonResult Get()
        {
            var results = Mapper.Map<IEnumerable<TripViewModel>>(_repository.GetTripsWithStops());
            return Json(results);
        }

        [HttpPost("")]
        public JsonResult Post([FromBody]TripViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newTrip = Mapper.Map<Trip>(vm);

                    _logger.LogInformation("Attempting to save a new trip");
                    _repository.AddTrip(newTrip);

                    if (_repository.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;
                        return Json(Mapper.Map<TripViewModel>(newTrip)); 
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save new trip", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = ex.Message });
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(new { Message = "Failed", ModelState = ModelState });
        }
    }
}
