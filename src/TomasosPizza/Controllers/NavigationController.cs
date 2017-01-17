using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TomasosPizza.IdentityModels;
using TomasosPizza.Models;
using TomasosPizza.ViewModels;

namespace TomasosPizza.Controllers
{
    [Authorize]
    public class NavigationController : Controller
    {
        private TomasosContext _context;
        private readonly UserManager<IdentityKund> _userManager;
        private readonly SignInManager<IdentityKund> _signInManager;


        public NavigationController(TomasosContext context, UserManager<IdentityKund> userManager, SignInManager<IdentityKund> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("Menu")]
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
            var deliveryModel = new OrderViewModel
            {
                Gatuadress = user.Gatuadress,
                Postort = user.Postort,
                Postnr = user.Postnr
            };
            return View(deliveryModel);
        }

        public IActionResult ThankYou()
        {
            return View();
        }
        [Route(""), Route("Login"), Route("Navigation/LogIn")]
        [AllowAnonymous]
        public IActionResult LogInView()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult RegisterView()
        {
            return View();
        }

        
        //public IActionResult UserEdit()
        //{
        //    int userId = int.Parse(HttpContext.Session.GetString("User"));   
        //    Kund user = _context.Kund.First(u => u.KundId == userId);

        //    return View(user);
        //}

        [Route("EditProfile")]
        public async Task<IActionResult> UserEditAsync()
        {
            //var identity = await _userManager.GetUserAsync(User);
            //var kund = _context.Kund.SingleOrDefault(x => x.IdentityId == identity.Id.ToString());

            var identity = _userManager.GetUserAsync(User).Result;
            var kund = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == identity.UserName);

            return View("UserEdit",kund);
        }

        [Authorize(Roles = "Administrator")]
        [Route("Admin")]
        public IActionResult AdminView()
        {
            return View();
        }
    }
}