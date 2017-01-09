using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TomasosPizza.Models;
using TomasosPizza.ViewModels;

namespace TomasosPizza.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult AddToOrder(Matratt option)
        {
            // todo Are Linq-queries the reason for these Exceptions? If so, how do I work with my session data?
            Dictionary<Matratt,int> order = Deserialize();
            Matratt hasOrdered;
            try
            {
                hasOrdered = order.Keys.Single(p => p.MatrattId == option.MatrattId);
            }
            catch
            {
                hasOrdered = null;
            }
            
            if (hasOrdered!=null)
            {
                int numOrder = order.First(p => p.Key == hasOrdered).Value;
                order[hasOrdered] = numOrder + 1;
            }
            else
            {
                order.Add(option, 0);
            }
            
            Reserialize(order);

            return RedirectToAction("MenuView", "Navigation");
        }

        public IActionResult RemoveFromOrder(Matratt option)
        {
            Dictionary<Matratt,int> order = Deserialize();
            order.Remove(option);
            Reserialize(order);
            return RedirectToAction("MenuView", "Navigation");
        }

        private void Reserialize(Dictionary<Matratt,int> order)
        {
            var serializedValue = JsonConvert.SerializeObject(order);
            HttpContext.Session.SetString("Order", serializedValue);
        }
        private Dictionary<Matratt,int> Deserialize()
        {
            Dictionary<Matratt,int> order;
            if (HttpContext.Session.GetString("Order") == null)
            {
                order = new Dictionary<Matratt, int>();
            }
            else
            {
                var str = HttpContext.Session.GetString("Order");
                order = JsonConvert.DeserializeObject<Dictionary<Matratt,int>>(str);
            }

            return order;
        }

        public IActionResult CheckOut()
        {
            var order = Deserialize();
            // todo Add order to Beställning in DB, possibly link in BestallningMatratt
            return RedirectToAction("ThankYou","Navigation");
        }
    }
}
