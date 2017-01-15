using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            model.Menu = _context.Matratt.Include(x => x.MatrattProdukt).ThenInclude(x => x.Produkt).ToList();
            foreach (var product in model.Menu)
            {
                product.MatrattTypNavigation =
                _context.MatrattTyp.FirstOrDefault(t => t.MatrattTyp1 == product.MatrattTyp);
            }
            if (HttpContext.Session.GetString("Order") != null)
            {
                string str = HttpContext.Session.GetString("Order");
                model.Order = JsonConvert.DeserializeObject<List<BestallningMatratt>>(str);
            }
            
            return View(model);
        }

        public IActionResult OrderView(Kund user)
        {
            return View(user);
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
            int userId = int.Parse(HttpContext.Session.GetString("User"));
            Kund user = _context.Kund.First(u => u.KundId == userId);
            return View(user);
        }
        [Authorize(Roles = "Administrator")]
        public IActionResult AdminView()
        {
            return View();
        }
    }
}