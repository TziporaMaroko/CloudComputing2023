using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZeldaWebsite.Data;
using ZeldaWebsite.Models;
using System.Net.Http;

namespace ZeldaWebsite.Controllers
{
    
    public class OrderController : Controller
    {
        public ZeldaWebsiteContext _db = new ZeldaWebsiteContext();

        public OrderController(ZeldaWebsiteContext context)
        {
            _db = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder([Bind("Id,FirstName,LastName,PhoneNumber,Email,Street,City,HouseNumber,Total,Products,Date,FeelsLike,Humidity,IsItHoliday,Day")] Order order)
        {
            order.Date = DateTime.Now;
			bool isAddressValid = await CheckAddress(order.City, order.Street);

			if (isAddressValid)
			{
				_db.Add(order);
				await _db.SaveChangesAsync();
				TempData["OrderCompleted"] = true;
				return RedirectToAction("ThankYou");
			}
			else
			{
				// If the address is not valid, add a model error
				ModelState.AddModelError(string.Empty, "Invalid address. Please check your city and street.");
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


        public IActionResult ThankYou()
        {
            return View();
        }

     
    }

}
