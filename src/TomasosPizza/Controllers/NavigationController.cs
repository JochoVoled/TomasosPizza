using System;
using System.Collections.Generic;
using System.Linq;
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

        public IActionResult OrderView()
        {
            // get the order data
            Bestallning model = new Bestallning();
            var str = HttpContext.Session.GetString("Order");
            var order = JsonConvert.DeserializeObject<List<BestallningMatratt>>(str);
            model.BestallningMatratt = order;
            
            // if no customer is logged in, ask user to log in
            if (HttpContext.Session.GetString("User") == null) 
                return RedirectToAction("LogInView", "Navigation");
            
            // get the logged in customer
            var id = int.Parse(HttpContext.Session.GetString("User"));
            var user = _context.Kund.FirstOrDefault(u => u.KundId == id);
            
            // only save to database if all required fields have actual values
            if (string.IsNullOrWhiteSpace(user.Gatuadress) || string.IsNullOrWhiteSpace(user.Postort) || string.IsNullOrWhiteSpace(user.Postnr))
            {
                return RedirectToAction("OrderView", "Navigation");
            }

            model.Kund = user;
            
            // calculate price in method to make forward-compatible with discounts
            model.Totalbelopp = CalculatePrice(order);
            model.BestallningDatum = DateTime.Now;
            model.Levererad = false;

            // Save to Session, so it can be loaded at CheckOut.
            var tmp = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("FinalOrder",tmp);

            var deliveryModel = new OrderViewModel
            {
                Gatuadress = user.Gatuadress,
                Postort = user.Postort,
                Postnr = user.Postnr
            };

            return RedirectToAction("CheckOut", "Order",deliveryModel);
        }

        private int CalculatePrice(List<BestallningMatratt> order)
        {
            return order.Sum(x => x.Matratt.Pris);
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
    }
}