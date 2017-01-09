using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TomasosPizza.Models;
using TomasosPizza.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TomasosPizza.Controllers
{
    public class NavigationController : Controller
    {
        private TomasosContext _context;

        public NavigationController(TomasosContext context)
        {
            _context = context;
        }

        public IActionResult MenuView()
        {
            
            MenuViewModel model = new MenuViewModel();
            foreach (var product in _context.Matratt)
            {
                model.Menu.Add(product);
            }
            if (HttpContext.Session.GetString("Order") != null)
            {
                string str = HttpContext.Session.GetString("Order");
                model.Order = JsonConvert.DeserializeObject<Dictionary<Matratt, int>>(str);
            }
            
            return View(model);
        }

        //[HttpPost]
        //public IActionResult MenuView(MenuViewModel.MenuOption option)
        //{

        //    return View(model);
        //}

        public IActionResult OrderView()
        {
            // get the order data
            // get the logged in customer
            // if no customer is logged in, ask user to log in
            return View();
        }
        public IActionResult ThankYou()
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