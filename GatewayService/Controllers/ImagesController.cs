using GateWay.Controllers;
using GatewayAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
//using static GatewayAPI.Models.Images;

namespace GatewayAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(ILogger<ImagesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetImageTags")]
        public bool Get(string imageUrl)
        {

            // Create a WebClient instance
            var webClient = new WebClient();

            // Create a NetworkCredential object with the username and password
            var credentials = new NetworkCredential("acc_7a8b8b0d0c71225", "2b42cdf9360f30bbd960633e0a060f35");

            // Set the credentials for the WebClient
            webClient.Credentials = credentials;
            try
            {
                // Download the JSON response using the WebClient with authorization
                var response = webClient.DownloadString($"https://api.imagga.com/v2/tags?image_url={imageUrl}");
                RootImages myDeserializedClass = JsonConvert.DeserializeObject<RootImages>(response);
                if (myDeserializedClass?.result.tags != null) // Check if the image is ice cream
                {
                    foreach (var tagElement in myDeserializedClass.result.tags)
                    {
                        if (string.Equals(tagElement.tag.en, "ice cream", StringComparison.OrdinalIgnoreCase))
                            return true; // it's an ice cream!!!
                    }
                }
                return false; // not ice cream
            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }
}