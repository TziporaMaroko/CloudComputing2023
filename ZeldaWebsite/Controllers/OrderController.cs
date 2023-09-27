﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZeldaWebsite.Data;
using ZeldaWebsite.Models;
using System.Net.Http;
using GatewayAPI.Models;
using Newtonsoft.Json;
using Weather = ZeldaWebsite.Models.Weather;

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
            order.Day= (Models.DayOfWeek)DateTime.Now.DayOfWeek;
            Weather wez = FindWeather(order.City);
            order.FeelsLike= wez.FeelsLike;
            order.Humidity = wez.Humidity;
			bool isAddressValid = await CheckAddress(order.City, order.Street);

			if (isAddressValid)
			{
				_db.Add(order);
                await _db.SaveChangesAsync();
				TempData["OrderCompleted"] = true;
                //change all the cart items id to this order's id
                foreach (var item in order.Products)
                {
                    item.OrderId = order.Id;
					item.DateCreated= DateTime.Now; 
					
				}
                await _db.SaveChangesAsync();
    //            foreach (var item in order.Products)//deletes duplicates in the database where order id == null
				//{
    //                var toDelete = await _db.ShoppingCartItems.SingleOrDefaultAsync(
    //                c => c.CartId == item.CartId && c.FlavourId == item.FlavourId && item.OrderId == null);
    //                _db.ShoppingCartItems.Remove(toDelete);
    //            }
    //            await _db.SaveChangesAsync();
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
		public async Task<bool> IsItHoliday(string city, string street)
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

						return isValidAddress;
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
