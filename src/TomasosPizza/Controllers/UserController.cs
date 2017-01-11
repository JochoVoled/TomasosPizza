using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TomasosPizza.Models;

namespace TomasosPizza.Controllers
{
    public class UserController : Controller
    {
        private TomasosContext _context;

        public UserController(TomasosContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult LogIn(Kund user)
        {
            if (ModelState.IsValid)
            {
                if (IsValidLogIn(user.AnvandarNamn, user.Losenord))
                {
                    var matchingUserId = _context.Kund.Single(u => u.AnvandarNamn == user.AnvandarNamn).KundId;
                    HttpContext.Session.SetString("User", matchingUserId.ToString());
                    return RedirectToAction("MenuView", "Navigation");
                }
            }
            return RedirectToAction("LogInView","Navigation");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("User");
            return RedirectToAction("MenuView", "Navigation");
        }

        private bool IsValidLogIn(string userName, string password)
        {
            var matchingUserName = _context.Kund.Single(u => u.AnvandarNamn == userName);
            if (matchingUserName == null) return false;
            if (matchingUserName.Losenord == password) return true;
            return false;
        }

        public IActionResult CreateUser(Kund newUser, string confirm)
        {
            if (_context.Kund.Any(u => u.AnvandarNamn == newUser.AnvandarNamn))
                return RedirectToAction("RegisterView", "Navigation");
            if (newUser.Losenord != confirm)
            {
                return RedirectToAction("RegisterView", "Navigation");
            }
            if (ModelState.IsValid)
            {
                //newUser.KundId = _context.Kund.Max(id => id.KundId) + 1;
                if (newUser.Gatuadress == null) { newUser.Gatuadress = ""; }
                if (newUser.Postnr == null) { newUser.Postnr = ""; }
                if (newUser.Postort == null) { newUser.Postort = ""; }
                _context.Kund.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetString("User",newUser.KundId.ToString());
                return RedirectToAction("MenuView", "Navigation");
            }
            return RedirectToAction("RegisterView","Navigation");
        }
        public IActionResult UpdateUser(Kund updatedUser, string confirm)
        {
            try
            {
                int userId = int.Parse(HttpContext.Session.GetString("User"));
                Kund user = _context.Kund.First(u => u.KundId == userId);
                user.AnvandarNamn = updatedUser.AnvandarNamn;
                user.Namn = updatedUser.Namn;

                user.Gatuadress = updatedUser.Gatuadress;
                user.Postort = updatedUser.Postort;
                user.Postnr = updatedUser.Postnr;

                user.Email = updatedUser.Gatuadress;
                user.Telefon = updatedUser.Telefon;

                if (confirm == user.Losenord)
                {
                    user.Losenord = updatedUser.Losenord;
                }
                else if (confirm == user.Losenord && confirm!=null)
                {
                    // fail user attempt to change password due to invalid password provided
                    return RedirectToAction("UserEdit", "Navigation");
                }

                _context.SaveChanges();
                return RedirectToAction("UserEdit","Navigation");
                // todo send "success" message
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("UserEdit", "Navigation");
                // todo send error message
            }

        }

        public IActionResult AddAdress(Kund updatedUser)
        {
            int userId = int.Parse(HttpContext.Session.GetString("User"));
            Kund user = _context.Kund.First(u => u.KundId == userId);
            user.Gatuadress = updatedUser.Gatuadress;
            user.Postort = updatedUser.Postort;
            user.Postnr = updatedUser.Postnr;
            // todo Get SQL update exception on update without changed values. Add more validation or solve why values are null if not changed
            _context.SaveChanges();
            return RedirectToAction("OrderView", "Navigation",user);
        }
    }
}