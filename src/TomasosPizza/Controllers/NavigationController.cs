using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TomasosPizza.Models;
using TomasosPizza.ViewModels;

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
                model.Order = JsonConvert.DeserializeObject<List<BestallningMatratt>>(str);
            }
            
            return View(model);
        }

        public IActionResult OrderView(List<BestallningMatratt> order)
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
            string userName = HttpContext.Session.GetString("User");
            Kund user = _context.Kund.First(u => u.AnvandarNamn == userName);
            return View(user);
        }
    }
}