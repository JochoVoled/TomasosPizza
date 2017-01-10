using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                    HttpContext.Session.SetString("User", user.KundId.ToString());
                    return RedirectToAction("MenuView", "Navigation");
                }
            }
            return RedirectToAction("LogInView","Navigation");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.SetString("User", "");
            return RedirectToAction("MenuView", "Navigation");
        }

        private bool IsValidLogIn(string userName, string password)
        {
            var matchingUserName = _context.Kund.Single(u => u.AnvandarNamn == userName);
            if (matchingUserName == null) return false;
            if (matchingUserName.Losenord == password) return true;
            return false;
        }

        public bool CreateUser(Kund newUser)
        {
            if (_context.Kund.Any(u => u.AnvandarNamn == newUser.AnvandarNamn)) return false;
            // todo enter further validation here
            // todo complete data here
            _context.Kund.Add(newUser);
            _context.SaveChanges();
            return true;
        }
        public bool UpdateUser(Kund updatedUser)
        {
            // todo remember how to update data with EF
            return true;
        }
    }
}