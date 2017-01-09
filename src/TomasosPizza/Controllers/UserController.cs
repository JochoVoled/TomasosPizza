using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if (IsValidLogIn(user.AnvandarNamn,user.Losenord))
            {
                return RedirectToAction("MenuView","Navigation");
            }
            return RedirectToAction("LogInView","Navigation");
        }
        private bool IsValidLogIn(string UserName, string Password)
        {
            var matchingUserName = _context.Kund.Single(u => u.AnvandarNamn == UserName);
            if (matchingUserName == null) return false;
            if (matchingUserName.Losenord == Password) return true;
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