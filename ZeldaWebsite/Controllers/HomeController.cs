using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ZeldaWebsite.Data;
using ZeldaWebsite.Models;

namespace ZeldaWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ZeldaWebsiteContext _context;
        public HomeController(ILogger<HomeController> logger, ZeldaWebsiteContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Flavour != null ?
                         View(await _context.Flavour.ToListAsync()) :
                         Problem("Entity set 'ZeldaWebsiteContext.Flavour'  is null.");
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public async Task<IActionResult> Flavour(int? id)
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
        public async Task<IActionResult> Flavours()
        {
            return _context.Flavour != null ?
                          View(await _context.Flavour.ToListAsync()) :
                          Problem("Entity set 'ZeldaWebsiteContext.Flavour'  is null.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult LogIn()
        {
            return View();
        }

        //async Task<IActionResult>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult LogIn([Bind("UserName,Password")] User users)
        {
            bool flag = false;
            if (ModelState.IsValid)
            {
                foreach (var user in _context.Users)
                {
                    if (user.UserName == users.UserName && user.Password == users.Password)
                    {
                        ViewBag.text = "true";
                        flag = true;
                        StaticFields.UserName = user.UserName;
                        StaticFields.IsUser = true;
                        return View("~/Views/Manager/Index.cshtml",_context.Flavour);
                    }
                }
                if (flag == false)
                {
                    ViewBag.text = "false";
                }
            }
            return View();
        }

        public IActionResult LogOut()
        {
            StaticFields.IsUser = false;
            return View("~/Views/Home/Index.cshtml");
        }
    }

    
}