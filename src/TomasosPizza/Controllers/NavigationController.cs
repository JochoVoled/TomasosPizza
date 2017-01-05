using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizza.Controllers
{
    public class NavigationController : Controller
    {
        
        public IActionResult MenuView()
        {
            return View();
        }
        public IActionResult OrderView()
        {
            return View();
        }
        public IActionResult LogInView()
        {
            return View();
        }
        public IActionResult RegisterView()
        {
            return View();
        }
        public IActionResult UserEdit()
        {
            return View();
        }
    }
}