using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ZeldaWebsite.Data;
using ZeldaWebsite.Models;

namespace ZeldaWebsite.Controllers
{
	public class ManagerController : Controller
	{
		private readonly ZeldaWebsiteContext _context;

		public ManagerController(ZeldaWebsiteContext context)
		{
			_context = context;
		}

		// GET: Flavours1
		public async Task<IActionResult> Index()
		{
			return _context.Flavour != null ? View(await _context.Flavour.ToListAsync()) :
						Problem("Entity set 'ZeldaWebsiteContext.Flavour'  is null.");
		}
        public async Task<IActionResult> Dashboard()
        {
            var dailyOrders = _context.Orders
                .GroupBy(o => new { Year = o.Date.Year, Month = o.Date.Month, Day = o.Date.Day })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .ThenBy(g => g.Key.Day)
                .Select(g => new
                {
                    Period = $"{g.Key.Day:D2}/{g.Key.Month:D2}/{g.Key.Year}",
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




        // GET: Flavours1/Details/5
        public async Task<IActionResult> Details(int? id)
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

		// GET: Flavours1/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Flavours1/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,ImageURL")] Flavour flavour)
		{
			if (ModelState.IsValid)
			{
				_context.Add(flavour);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(flavour);
		}

		// GET: Flavours1/Edit/5
		public async Task<IActionResult> Edit(int? id)
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

		// POST: Flavours1/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageURL")] Flavour flavour)
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

		// GET: Flavours1/Delete/5
		public async Task<IActionResult> Delete(int? id)
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
        [HttpGet]
        public IActionResult UpdateGraph(DateTime startDate, DateTime endDate)
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
                        Period = $"{g.Key.Day:D2}/{g.Key.Month:D2}/{g.Key.Year}",
                        OrdersCount = g.Count(),
                        OrdersTotal = g.Sum(o => o.Total)
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

    }
}
