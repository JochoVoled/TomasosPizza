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

            return RedirectToAction("CartPartial", "Navigation");
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
            return RedirectToAction("CartPartial", "Navigation");
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
            Bestallning model = new Bestallning();
            var str = HttpContext.Session.GetString("Order");
            var order = JsonConvert.DeserializeObject<List<BestallningMatratt>>(str);
            model.BestallningMatratt = order;

            var identity = _userManager.GetUserAsync(User).Result;
            var user = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == identity.UserName);

            model.Kund = user;
            model.Totalbelopp = CalculatePrice(order, user);
            model.BestallningDatum = DateTime.Now;
            model.Levererad = false;

            var tmp = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("FinalOrder", tmp);
            if (user.Gatuadress!=null && user.Postort!=null && user.Postnr!=null)
            {
                return RedirectToAction("CheckOut", "Order", user);
            }
            return RedirectToAction("OrderView","Navigation",user);
        }

        private int CalculatePrice(List<BestallningMatratt> order, Kund user)
        {
            if (User.IsInRole("PremiumUser"))
            {
                //Köper den tre pizzor/ maträtter eller mer får den 20 % rabatt på beställningen.
                decimal multiplier = 1m;
                if(order.Sum(p => p.Antal) >= 3)
                    multiplier = 0.8m;

                int discount = 0;
                //När den har 100 poäng ger det en gratis pizza vid en beställning.
                foreach (BestallningMatratt o in order)
                {
                    user.Poang += (10*o.Antal);
                    if (user.Poang < 100)
                        continue;
                    discount += order.Min(p => p.Matratt.Pris);
                    user.Poang -= 100;
                }
                          //Math.Round((Model.Order.Sum(p => p.Matratt.Pris * p.Antal) - discount) * multiplier))
                return (int)Math.Round((order.Sum(x => x.Matratt.Pris*x.Antal) - discount) * multiplier);
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

            if (string.IsNullOrWhiteSpace(updatedUser.Gatuadress) || string.IsNullOrWhiteSpace(updatedUser.Postort) || string.IsNullOrWhiteSpace(updatedUser.Postnr))
            {
                return RedirectToAction("OrderView", "Navigation");
            }

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
                Levererad = false,
                Totalbelopp = order.Totalbelopp,
                BestallningDatum = order.BestallningDatum,
            };
            _context.Add(b);

            foreach (var matratt in order.BestallningMatratt)
            {
                var m = new BestallningMatratt
                {
                    BestallningId = b.BestallningId,
                    MatrattId = matratt.MatrattId,
                    Antal = matratt.Antal
                };
                _context.Add(m);
                user.Poang = (user.Poang+ 10 * matratt.Antal)%100;
            }
            _context.SaveChanges();
            HttpContext.Session.Remove("FinalOrder");
            HttpContext.Session.Remove("Order");
            return RedirectToAction("ThankYou", "Navigation");
        }
    }
}
