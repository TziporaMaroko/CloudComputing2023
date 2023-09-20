using Microsoft.AspNetCore.Mvc;

namespace ZeldaWebsite.Controllers
{
	public class PayPalController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
