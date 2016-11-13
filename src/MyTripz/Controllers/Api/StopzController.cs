using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyTripz.Models;
using MyTripz.Services;
using MyTripz.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MyTripz.Controllers.Api
{
    [Route("api/trips/{tripName}/stops")]
    public class StopzController : Controller
    {
        private readonly CoordService _coordService;
        private readonly ILogger<StopzController> _logger;
        private readonly ITripzRepository _repository;

        public StopzController(ITripzRepository repository, ILogger<StopzController> logger, CoordService coordService)
        {
            _repository = repository;
            _logger = logger;
            _coordService = coordService;
        }

        [HttpGet("")]
        public JsonResult Get(string tripName)
        {
            try
            {
                var results = _repository.GetTripByName(tripName);
                if (results == null)
                {
                    return Json(null);
                }

                return Json(Mapper.Map<IEnumerable<StopViewModel>>(results.Stops.OrderBy(s=>s.Order)));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get stops for trip {tripName}", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Error occured finding trip name");
            }
        }

        public JsonResult Post(string tripName, [FromBody]StopViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Map to the Entity
                    var newStop = Mapper.Map<Stop>(vm);

                    //Looking up Geocoordinates
                    var coordResult = _coordService.Lookup(newStop.Name);

                    if (!coordResult.Success)
                    {
                        Response.StatusCode = (int) HttpStatusCode.BadRequest;
                        Json(coordResult.Message);
                    }

                    newStop.Latitude = coordResult.Latitude;
                    newStop.Longitude = coordResult.Longitude;

                    //Save to the db
                    _repository.AddStop(tripName, newStop);
                    if (_repository.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;
                        return Json(Mapper.Map<StopViewModel>(newStop));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save new stop", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Failed to save new stop");
            }
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json("Validation Failed on new stop");
        }
    }
}
