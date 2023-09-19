using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZeldaWebsite.Models;

namespace ZeldaWebsite.Controllers
{
    public class HebcalController : Controller
    {
        // GET: HebcalController
        public ActionResult Index()
        {
            Wadapter adapter= new Wadapter();
            var data=adapter.GetWeather();
            return View();
        }

        // GET: HebcalController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HebcalController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HebcalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HebcalController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HebcalController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HebcalController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HebcalController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
