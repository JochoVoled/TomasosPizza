using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TomasosPizza.Models;
using TomasosPizza.ViewModels;

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
        public IActionResult LogIn(LoginViewModel user)
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
            HttpContext.Session.Clear();
            return RedirectToAction("MenuView", "Navigation");
        }

        private bool IsValidLogIn(string userName, string password)
        {
            var matchingUserName = _context.Kund.FirstOrDefault(u => u.AnvandarNamn == userName);
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

                // Tips: Raden ska ersätta user med uppdateringar från updatedUser
                //_context.Entry(user).CurrentValues.SetValues(updatedUser);

                if (user.AnvandarNamn == updatedUser.AnvandarNamn && updatedUser.AnvandarNamn!=null)
                {
                    user.AnvandarNamn = updatedUser.AnvandarNamn;
                }
                if (user.Namn == updatedUser.Namn && updatedUser.Namn != null)
                {
                    user.Namn = updatedUser.Namn;
                }
                if (user.Gatuadress == updatedUser.Gatuadress && updatedUser.Gatuadress != null)
                {
                    user.Gatuadress = updatedUser.Gatuadress;
                }
                if (user.Postort == updatedUser.Postort && updatedUser.Postort != null)
                {
                    user.Postort = updatedUser.Postort;
                }
                if (user.Postnr == updatedUser.Postnr && updatedUser.Postnr != null)
                {
                    user.Postnr = updatedUser.Postnr;
                }
                if (user.Email == updatedUser.Email && updatedUser.Email != null)
                {
                    user.Email = updatedUser.Email;
                }
                if (user.Telefon == updatedUser.Telefon && updatedUser.Telefon != null)
                {
                    user.Telefon = updatedUser.Telefon;
                }

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
                // todo (low prio) send "success" message
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return RedirectToAction("UserEdit", "Navigation");
                // todo (low prio) send error message
            }

        }
        [HttpPost]
        public IActionResult AddAdress(Kund updatedUser)
        {
            //Kund updatedUser = bestallning.Kund;
            int userId = int.Parse(HttpContext.Session.GetString("User"));
            Kund user = _context.Kund.First(u => u.KundId == userId);

            // only save to database if all required fields have actual values
            if (string.IsNullOrWhiteSpace(updatedUser.Gatuadress) || string.IsNullOrWhiteSpace(updatedUser.Postort) || string.IsNullOrWhiteSpace(updatedUser.Postnr))
            {
                return RedirectToAction("OrderView", "Navigation", user);
            }

            // only update if any value has been changed
            if (updatedUser.Gatuadress != user.Gatuadress || updatedUser.Postort != user.Postort || updatedUser.Postnr != user.Postnr)
            {
                user.Gatuadress = updatedUser.Gatuadress;
                user.Postort = updatedUser.Postort;
                user.Postnr = updatedUser.Postnr;
                _context.SaveChanges();
            }
            
            //return RedirectToAction("OrderView", "Navigation",user);
            return RedirectToAction("CheckOut","Order",false); // I know CheckOut takes two parameters, so this shouldn't work, yet IDE sais it would? Pay attention, and learn something
        }
    }
}