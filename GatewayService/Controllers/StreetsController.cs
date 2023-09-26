using GatewayAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace GateWay.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StreetsController : ControllerBase
    {

        private readonly ILogger<StreetsController> _logger;

        public StreetsController(ILogger<StreetsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetAddress")]
        public bool Get(string city, string street)
        {

            // Deserialize the JSON response into an instance of the MyData class
            var response = new WebClient().DownloadString("https://data.gov.il/api/3/action/datastore_search?resource_id=bf185c7f-1a4e-4662-88c5-fa118a244bda&limit=130000");
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(response);

            // Check if the given city and street exist in the Addresses list
            if (myDeserializedClass?.result.records != null)
            {
                foreach (var address in myDeserializedClass.result.records)
                {
                    if (string.Equals(address.city_name,city+" ", StringComparison.OrdinalIgnoreCase) && string.Equals(address.street_name,street+" ", StringComparison.OrdinalIgnoreCase))
                        return true; // Address exists
                }
            }

            return false; // Address not found

        }


    }
}