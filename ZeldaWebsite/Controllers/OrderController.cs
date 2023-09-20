using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZeldaWebsite.Data;
using ZeldaWebsite.Models;

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
			order.Date= DateTime.Now;

			/*if (ModelState.IsValid)
			{*/
				_db.Add(order);
				await _db.SaveChangesAsync();
				return RedirectToAction("ThankYou");
			
			//return View(order);
		}

		public IActionResult ThankYou()
        {
            return View();
        }

     
    }

}
