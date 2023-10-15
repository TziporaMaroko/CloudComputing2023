using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PayPal.Api;
using ZeldaWebsite.Data;
using ZeldaWebsite.Models;

namespace ZeldaWebsite.Controllers
{
	public class ManagerController : Controller
	{
		private readonly ZeldaWebsiteContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ManagerController(ZeldaWebsiteContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Flavours1
        public async Task<IActionResult> Index()
		{
            if (StaticFields.IsUser)
            {
                return _context.Flavour != null ? View(await _context.Flavour.ToListAsync()) :
                        Problem("Entity set 'ZeldaWebsiteContext.Flavour'  is null.");
            }
            else
            {
                // Redirect to the "404NotFound" action in the controller
                return RedirectToAction("NotFound404");
            }
            
		}

        public async Task<IActionResult> NotFound404()
        {
			return View();
        }
        public async Task<IActionResult> Dashboard()
        {
			if (StaticFields.IsUser)
			{
				var dailyOrders = _context.Orders
				.GroupBy(o => new { Year = o.Date.Year, Month = o.Date.Month, Day = o.Date.Day })
				.OrderBy(g => g.Key.Year)
				.ThenBy(g => g.Key.Month)
				.ThenBy(g => g.Key.Day)
				.Select(g => new
				{
					Period = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
					OrdersCount = g.Count(),
					OrdersTotal = g.Sum(o => o.Total)
				})
				.ToList();

				ViewBag.DailyOrdersJson = JsonConvert.SerializeObject(dailyOrders.Select(g => new
				{
					period = g.Period,
					ordersCount = g.OrdersCount,
					ordersTotal = g.OrdersTotal.ToString("F2")
				}));

				return _context.Orders != null ?
					View(await _context.Orders.ToListAsync()) :
					Problem("Entity set 'ZeldaWebsiteContext.Orders' is null.");
			}
            else
            {
                // Redirect to the "404NotFound" action in the controller
                return RedirectToAction("NotFound404");
            }
        }




		// GET: Flavours1/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (StaticFields.IsUser) { 
				if (id == null || _context.Flavour == null)
				{
					return NotFound();
				}

			var flavour = await _context.Flavour
				.FirstOrDefaultAsync(m => m.Id == id);
			if (flavour == null)
			{
				return NotFound();
			}

			return View(flavour);
			}
            else
            {
                // Redirect to the "404NotFound" action in the controller
                return RedirectToAction("NotFound404");
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> OrderDetails(int? id)
        {
			if (StaticFields.IsUser)
			{
				if (id == null || _context.Orders == null)
				{
					return NotFound();
				}

				var order = await _context.Orders
					.FirstOrDefaultAsync(m => m.Id == id);
				if (order == null)
				{
					return NotFound();
				}
				var orderItems = await _context.ShoppingCartItems.Where(item => item.OrderId == id).ToListAsync();
				//Bind order's products to the entity order
				order.Products = orderItems;

				return View(order);
			}
            else
            {
                // Redirect to the "404NotFound" action in the controller
                return RedirectToAction("NotFound404");
            }
        }

        // GET: Flavours1/Create
        public IActionResult Create()
		{
			if (StaticFields.IsUser)
			{
				return View();
			}
            else
            {
                // Redirect to the "404NotFound" action in the controller
                return RedirectToAction("NotFound404");
            }
        }

		// POST: Flavours1/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImageURL,Description,Price")] Flavour flavour, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Handle the case where the user uploads an image from the file explorer
                // Generate a unique filename for the image
                string uniqueFileName = $"{Guid.NewGuid().ToString()}_{DateTime.Now.Ticks}{Path.GetExtension(ImageFile.FileName)}";

				// Define the path where you want to save the image inside the wwwroot / images folder
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", uniqueFileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                // Update the Image_URL property with the new path (relative to wwwroot)
                flavour.ImageURL = $"{uniqueFileName}";

                _context.Add(flavour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else if (!string.IsNullOrEmpty(flavour.ImageURL))
            {
                // Handle the case where the user provides an image URL
                var imageUrl = flavour.ImageURL;
                var isImageValid = await CheckImage(imageUrl);

                if (!isImageValid)
                {
                    ModelState.AddModelError(string.Empty, "The provided image URL is not valid or does not contain a recognized ice cream image.");
                    return View(flavour);
                }

                // Generate a unique filename for the image
                string uniqueFileName = $"{Guid.NewGuid().ToString()}_{DateTime.Now.Ticks}.jpg";

				// Define the path where you want to save the image inside the wwwroot / images folder
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", uniqueFileName);

                // Download and save the image to the specified path
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(imageUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var contentStream = await response.Content.ReadAsStreamAsync();

                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await contentStream.CopyToAsync(fileStream);
                        }

                        // Update the Image_URL property with the new path (relative to wwwroot)
                        flavour.ImageURL = $"{uniqueFileName}";

                        _context.Add(flavour);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please provide either an image URL or upload an image.");
                return View(flavour);
            }

            // If ModelState is not valid, return the view with validation errors
            return View(flavour);
        }


        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<bool> CheckImage(string imageURL)
		{
			// Construct the URL of the other project's endpoint
			string apiUrl = $"http://localhost:5186/Images?imageUrl={imageURL}";

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

		// GET: Flavours1/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (StaticFields.IsUser)
			{
				if (id == null || _context.Flavour == null)
				{
					return NotFound();
				}

				var flavour = await _context.Flavour.FindAsync(id);
				if (flavour == null)
				{
					return NotFound();
				}
				return View(flavour);
			}
            else
            {
                return RedirectToAction("NotFound404");
            }
		}

		// POST: Flavours1/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageURL")] Flavour flavour)
		{
			if (StaticFields.IsUser)
			{
				if (id != flavour.Id)
				{
					return NotFound();
				}

				if (ModelState.IsValid)
				{
					try
					{
						_context.Update(flavour);
						await _context.SaveChangesAsync();
					}
					catch (DbUpdateConcurrencyException)
					{
						if (!FlavourExists(flavour.Id))
						{
							return NotFound();
						}
						else
						{
							throw;
						}
					}
					return RedirectToAction(nameof(Index));
				}
				return View(flavour);
			}
            else
            {
                return RedirectToAction("NotFound404");
            }
		}

		// GET: Flavours1/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{

			if (StaticFields.IsUser)
			{
				if (id == null || _context.Flavour == null)
				{
					return NotFound();
				}

				var flavour = await _context.Flavour
					.FirstOrDefaultAsync(m => m.Id == id);
				if (flavour == null)
				{
					return NotFound();
				}

				return View(flavour);
			}
            else
            {
                return RedirectToAction("NotFound404");
            }
		}

		// POST: Flavours1/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Flavour == null)
			{
				return Problem("Entity set 'ZeldaWebsiteContext.Flavour'  is null.");
			}
			var flavour = await _context.Flavour.FindAsync(id);
			if (flavour != null)
			{
				_context.Flavour.Remove(flavour);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool FlavourExists(int id)
		{
			return (_context.Flavour?.Any(e => e.Id == id)).GetValueOrDefault();
		}


        [HttpGet]
        public IActionResult GetDataForDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Query the database to get the relevant data for the graph
                var graphData = _context.Orders
                    .Where(o => o.Date >= startDate && o.Date <= endDate)
                    .GroupBy(o => new { Year = o.Date.Year, Month = o.Date.Month, Day = o.Date.Day })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .ThenBy(g => g.Key.Day)
                    .Select(g => new
                    {
                        Period = new DateTime(g.Key.Year,g.Key.Month,g.Key.Day),
                        OrdersCount = g.Count(),
                        OrdersTotal = g.Sum(o => o.Total).ToString("F2")
                    })
                    .ToList();

                return Json(graphData);
            }
            catch (Exception ex)
            {
                // Handle any errors and return an error response if needed
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public List<ZeldaWebsite.Models.Order> GetOrdersForDateRange(DateTime startDate, DateTime endDate)
		{
			try
			{
				// Query the database to get the relevant data for the graph
				var graphData = _context.Orders
					.Where(o => o.Date >= startDate && o.Date <= endDate)
					.OrderBy(g => g.Date.Year)
					.ThenBy(g => g.Date.Month)
					.ThenBy(g => g.Date.Day)
					.ToList();

				return graphData;
			}
			catch (Exception ex)
			{
				// Handle any errors and return an error response if needed
				return null;
			}
		
        }

    }
}
