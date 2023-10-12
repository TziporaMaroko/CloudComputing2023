using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using ZeldaWebsite.Data;
using ZeldaWebsite.Models;

namespace ZeldaWebsite.Controllers
{
    public class MLController : Controller
    {
        private readonly MLContext _mlContext;
        private readonly ZeldaWebsiteContext _context;
        public MLController(MLContext mlContext, ZeldaWebsiteContext context)
        {
            _mlContext = mlContext;
            _context = context;
        }

        [HttpPost("train")]
        public IActionResult TrainModel()
        {
            // Load and preprocess your historical data, define the pipeline, and train the model as shown in the previous code.

            // Return a success message or appropriate response.
            return Ok("Model trained successfully");
        }

        [HttpPost("predict")]
        public IActionResult Predict([FromBody] Order input)
        {
            if (input == null)
            {
                return BadRequest("Input data is missing.");
            }

            //var predictionEngine = _mlContext.Model.CreatePredictionEngine<Order, FlavourPrediction>(trainedModel);

            //var prediction = predictionEngine.Predict(input);

            //// Get the most wanted ice cream flavor
            //var mostWantedFlavor = prediction.Flavor;

            return Ok($"The most wanted ice cream flavor for the future date is: {/*mostWantedFlavor*/null}");
        }
    }
}

