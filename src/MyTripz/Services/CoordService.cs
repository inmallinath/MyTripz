using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MyTripz.Services
{
    public class CoordService
    {
        private readonly ILogger<CoordService> _logger;

        public CoordService(ILogger<CoordService> logger)
        {
            _logger = logger;
        }

        public CoordServiceResult Lookup(string location)
        {
            var result = new CoordServiceResult()
            {
                Success = false,
                Message = "Undetermined failure while looking up coordinates"
            };

            var bingKey = Startup.Configuration["AppSettings:Bingkey"];
            var encodedName = WebUtility.UrlEncode(location);
            var url = $"http://dev.virtualearth.net/REST/v1/Location?q={encodedName}&key={bingKey}";

            return result;
        }
    }
}
