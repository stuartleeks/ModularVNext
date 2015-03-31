using System;
using Microsoft.AspNet.Mvc;

namespace Module2.Controllers
{
    [Area("Module2")]
    public class WeatherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
