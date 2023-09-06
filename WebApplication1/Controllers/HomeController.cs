using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebApplication1Context _context;
        public HomeController(ILogger<HomeController> logger, WebApplication1Context context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Flavour != null ?
                         View(await _context.Flavour.ToListAsync()) :
                         Problem("Entity set 'WebApplication1Context.Flavour'  is null.");
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
                          Problem("Entity set 'WebApplication1Context.Flavour'  is null.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}