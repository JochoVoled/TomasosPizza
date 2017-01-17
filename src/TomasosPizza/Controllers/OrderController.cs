using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TomasosPizza.IdentityModels;
using TomasosPizza.Models;

namespace TomasosPizza.Controllers
{
    public class OrderController : Controller
    {
        private TomasosContext _context;
        private readonly UserManager<IdentityKund> _userManager;

        public OrderController(TomasosContext context, UserManager<IdentityKund> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult AddToOrder(int id)
        {
            Matratt option = _context.Matratt.First(mat => mat.MatrattId == id);
            List<BestallningMatratt> order = Deserialize();
            BestallningMatratt hasOrdered = order.SingleOrDefault(p => p.MatrattId == option.MatrattId);
            if (hasOrdered!=null)
            {
                hasOrdered.Antal += 1;
            }
            else
            {
                hasOrdered = new BestallningMatratt
                {
                    Matratt = option,
                    MatrattId = option.MatrattId,
                    Antal = 1,
                };
                order.Add(hasOrdered);
            }
            
            Reserialize(order);

            return RedirectToAction("MenuView", "Navigation");
        }

        public IActionResult RemoveFromOrder(int id)
        {
            Matratt option = _context.Matratt.First(mat => mat.MatrattId == id);
            List<BestallningMatratt> order = Deserialize();
            BestallningMatratt remove = order.Find(o => o.MatrattId == option.MatrattId);
            if (remove.Antal<=1)
            {
                order.Remove(remove);
            }
            else
            {
                remove.Antal -= 1;
            }
            
            Reserialize(order);
            return RedirectToAction("MenuView", "Navigation");
        }

        private void Reserialize(List<BestallningMatratt> order)
        {
            var serializedValue = JsonConvert.SerializeObject(order);
            HttpContext.Session.SetString("Order", serializedValue);
        }
        private List<BestallningMatratt> Deserialize()
        {
            List<BestallningMatratt> order;
            if (HttpContext.Session.GetString("Order") == null)
            {
                order = new List<BestallningMatratt>();
            }
            else
            {
                var str = HttpContext.Session.GetString("Order");
                order = JsonConvert.DeserializeObject<List<BestallningMatratt>>(str);
            }

            return order;
        }

        public IActionResult PrepareOrder()
        {
            // get the order data
            Bestallning model = new Bestallning();
            var str = HttpContext.Session.GetString("Order");
            var order = JsonConvert.DeserializeObject<List<BestallningMatratt>>(str);
            model.BestallningMatratt = order;

            var identity = _userManager.GetUserAsync(User).Result;
            var user = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == identity.UserName);

            //var id = int.Parse(HttpContext.Session.GetString("User"));
            //var user = _context.Kund.FirstOrDefault(u => u.KundId == id);

            model.Kund = user;
            // calculate price in method to make forward-compatible with discounts
            model.Totalbelopp = CalculatePrice(order, user);
            model.BestallningDatum = DateTime.Now;
            model.Levererad = false;

            // Save to Session, so it can be loaded at CheckOut.
            var tmp = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("FinalOrder", tmp);
            if (user.Gatuadress!=null && user.Postort!=null && user.Postnr!=null)
            {
                return RedirectToAction("CheckOut", "Order", user);
            }
            return RedirectToAction("OrderView","Navigation",user);
        }

        private int CalculatePrice(List<BestallningMatratt> order, Kund user) // todo user should not be null
        {
            if (User.IsInRole("PremiumUser"))
            {
                //Köper den tre pizzor/ maträtter eller mer får den 20 % rabatt på beställningen.
                decimal multiplier = order.Count >= 3 ? 0.8m : 1m;
                int discount = 0;
                //När den har 100 poäng ger det en gratis pizza vid en beställning.
                if (user.Poang >= 100)
                {
                    discount = order.Min(p => p.Matratt.Pris);
                    user.Poang -= 100;
                }
                return (int)Math.Round((order.Sum(x => x.Matratt.Pris) - discount) * multiplier,2);
            }
            else
            {
                return order.Sum(x => x.Matratt.Pris);
            }
            
        }

        public IActionResult CheckOut(Kund updatedUser)
        {
            var identity = _userManager.GetUserAsync(User).Result;
            var user = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == identity.UserName);

            //int userId = int.Parse(HttpContext.Session.GetString("User"));
            //Kund user = _context.Kund.First(u => u.KundId == userId);

            // only save to database if all required fields have actual values
            if (string.IsNullOrWhiteSpace(updatedUser.Gatuadress) || string.IsNullOrWhiteSpace(updatedUser.Postort) || string.IsNullOrWhiteSpace(updatedUser.Postnr))
            {
                return RedirectToAction("OrderView", "Navigation");
            }

            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction("OrderView", "Navigation");
            //}

            // only update if any value has been changed
            if (updatedUser.Gatuadress != user.Gatuadress || updatedUser.Postort != user.Postort || updatedUser.Postnr != user.Postnr)
            {
                user.Gatuadress = updatedUser.Gatuadress;
                user.Postort = updatedUser.Postort;
                user.Postnr = updatedUser.Postnr;
                _context.SaveChanges();
            }

            var serialized = HttpContext.Session.GetString("FinalOrder");
            Bestallning order = JsonConvert.DeserializeObject<Bestallning>(serialized);

            var b = new Bestallning
            {
                KundId = order.Kund.KundId,
                Levererad = true,
                Totalbelopp = order.Totalbelopp,
                BestallningDatum = order.BestallningDatum,
            };
            _context.Add(b);

            // At Checkout(), add 10 points to Kund foreach line in Order if points < 100

            foreach (var matratt in order.BestallningMatratt)
            {
                var m = new BestallningMatratt
                {
                    BestallningId = b.BestallningId,
                    MatrattId = matratt.MatrattId,
                    Antal = matratt.Antal
                };
                _context.Add(m);
                user.Poang += 10;
            }
            _context.SaveChanges();
            return RedirectToAction("ThankYou", "Navigation");
        }
    }
}
