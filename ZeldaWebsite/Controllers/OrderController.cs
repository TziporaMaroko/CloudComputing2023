using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZeldaWebsite.Data;
using ZeldaWebsite.Models;
using System.Net.Http;
using GatewayAPI.Models;
using Newtonsoft.Json;
using Weather = ZeldaWebsite.Models.Weather;
using System.Text;
using System.Net;

namespace ZeldaWebsite.Controllers
{
    
    public class OrderController : Controller
    {
        public ZeldaWebsiteContext _db = new ZeldaWebsiteContext();
		string AccessToken = "ATaR0ncvoOm38jdm-Jg3Nloh1NzbJ_YeBXy5B3-_2Jo0yQQQizWVQdIfuiOCxwCHulyCZzY2Ytk46J1U";
		public OrderController(ZeldaWebsiteContext context)
        {
            _db = context;
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateOrder([Bind("Id,FirstName,LastName,PhoneNumber,Email,Street,City,HouseNumber,Total,Products,Date,FeelsLike,Humidity,IsItHoliday,Day")] Order order)
		{
			order.Date = DateTime.Now;
			order.Day = (Models.DayOfWeek)DateTime.Now.DayOfWeek;
			Weather wez = FindWeather(order.City);
			order.FeelsLike = wez.FeelsLike;
			order.Humidity = wez.Humidity;
			bool isAddressValid = await CheckAddress(order.City, order.Street);
			order.IsItHoliday = await IsItHoliday(order.Date.Year.ToString(), order.Date.Month.ToString(), order.Date.Day.ToString());
			if (!isAddressValid )
			{
				ModelState.AddModelError(string.Empty, "Invalid address. Please check your city and street.");
			}
			if (ModelState.IsValid && isAddressValid)
			{
				_db.Add(order);
				await _db.SaveChangesAsync();
				//change all the cart items id to this order's id
				foreach (var item in order.Products)
				{
					item.OrderId = order.Id;
					item.DateCreated = DateTime.Now;
				}
				await _db.SaveChangesAsync();
				return RedirectToAction("ThankYou");
			}
			else
			{
				TempData["OrderPayed"] = true;
				// If there's a validation error, return to the Checkout view
				return View("~/Views/Order/Checkout.cshtml", order);
			}
		}


		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> CheckAddress(string city, string street)
        {
            // Construct the URL of the other project's endpoint
            string apiUrl = $"http://localhost:5186/Streets?city={city}&street={street}"; 

            try
            {
                // Create an instance of HttpClient
                using (var httpClient = new HttpClient())
                {
                    // Send a GET request to the other project's endpoint
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response content as a boolean value
                        bool isValidAddress = bool.Parse(await response.Content.ReadAsStringAsync());

                        return  isValidAddress;
                    }
                    else
                    {
                        return false ;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Weather FindWeather(string city)
		{
			// Construct the URL to the API Gateway's GetWeather endpoint
			string apiUrl = $"http://localhost:5186/Weather?city={city}"; 

			try
			{
				// Create an instance of HttpClient
				using (var httpClient = new HttpClient())
				{
					// Send a GET request to the API Gateway's GetWeather endpoint
					HttpResponseMessage response = httpClient.GetAsync(apiUrl).Result;

					// Check if the request was successful
					if (response.IsSuccessStatusCode)
					{
						// Deserialize the JSON response into a WeatherToReturn object
						var jsonContent = response.Content.ReadAsStringAsync().Result;
						var weather = JsonConvert.DeserializeObject<Weather>(jsonContent);
						return weather;
					}
					else
					{
						// Handle errors here
						return new Weather(); ; 
					}
				}
			}
			catch (Exception ex)
			{
				// Handle exceptions here
				return new Weather(); 
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<bool> IsItHoliday(string y, string m, string d)
		{
			// Construct the URL of the other project's endpoint
			string apiUrl = $"http://localhost:5186/HebCal?y={y}&m={m}&d={d}";

			try
			{
				// Create an instance of HttpClient
				using (var httpClient = new HttpClient())
				{
					// Send a GET request to the other project's endpoint
					HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

					// Check if the request was successful
					if (response.IsSuccessStatusCode)
					{
						// Parse the response content as a boolean value
						bool isHoliday = bool.Parse(await response.Content.ReadAsStringAsync());

						return isHoliday;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public IActionResult ThankYou()
        {
            return View();
        }

		

	}

}
