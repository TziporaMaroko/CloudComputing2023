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
        public async Task<IActionResult> Checkout([FromBody] CartView cart)
        {
            Order model = new Order();
            model.products = cart;
            return View(model);
        }

        public IActionResult ThankYou()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PlaceOrder(Order order)
        {

            if (ModelState.IsValid)
            {
                // Save the order to the database
               // _db.Orders.Add(order);
                _db.SaveChanges();

                // You can redirect the user to a thank you page or perform other actions as needed.
                // For now, we'll redirect to a thank you action.
                return RedirectToAction("ThankYou");
            }

            // If the model is not valid, return to the checkout page with validation errors.
            return View("Checkout", order); // You may want to pass the 'order' object back to the view to display validation errors.
        }

    }

}
