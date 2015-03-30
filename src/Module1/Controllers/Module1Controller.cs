using System;
using Microsoft.AspNet.Mvc;

namespace Module1.Controllers
{
    public class Module1Controller : Controller
    {
        public IActionResult Index()
        {
            //return Content("Hello from Module1!!!");
            return View();
        }
    }
}