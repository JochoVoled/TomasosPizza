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

        // todo figure out what value to pass in order to get Kund, and how to pass that in from any view
        // Can I use the nav-bar as a partial view, whose menues are updated on LogIn, and holds LoggedInUser as model value?
        public IActionResult UserEdit(string userName)
        {
            Kund user = _context.Kund.SingleOrDefault(u => u.AnvandarNamn == userName);
            return View(user);
        }
    }
}