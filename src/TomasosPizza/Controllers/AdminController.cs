using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class AdminController : Controller
    {
        private readonly TomasosContext _context;
        private readonly UserManager<IdentityKund> _userManager;

        public AdminController(TomasosContext context, UserManager<IdentityKund> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult BeginUpdateMatratt(int id)
        {
            var model = new ModalViewModel
            {
                Matratt = _context.Matratt.FirstOrDefault(x => x.MatrattId == id),
                Typer = _context.MatrattTyp.ToList(),
                Produkter = _context.Produkt.ToList(),
                Title = "Redigera maträtt",
            };
            //model.Title = "Skapa ny maträtt";
            //model.Matratt = _context.Matratt.FirstOrDefault(x => x.MatrattId == id);
            model.Matratt.MatrattProdukt = _context.MatrattProdukt.Where(x => x.MatrattId == model.Matratt.MatrattId).ToList();
            //model.Typer = _context.MatrattTyp.ToList();
            //model.Produkter = _context.Produkt.ToList();
            return View("_ModalPartial", model);
        }
        public IActionResult AddNewToMenu()
        {
            var option = new Matratt
            {
                MatrattNamn = "",
                Beskrivning = "",
                Pris = 0,
                MatrattProdukt = new List<MatrattProdukt>()
            };
            var model = new ModalViewModel
            {
                Title = "Skapa ny maträtt",
                Matratt = option,
                Typer = _context.MatrattTyp.ToList(),
                Produkter = _context.Produkt.ToList(),
            //Produkter = _context.Produkt.Where(order => order.MatrattProdukt.Equals(option.MatrattProdukt)).ToList()
        };
            return View("_ModalPartial", model);
        }

        [HttpPost]
        public IActionResult SaveMatratt(ModalViewModel model)
        {
            var option = model.Matratt;
            // Spara Maträtt/ändringar till databasen
            var currentOption = _context.Matratt.FirstOrDefault(x => x.MatrattId == option.MatrattId);
            currentOption.Beskrivning = option.Beskrivning;
            currentOption.Pris = option.Pris;
            currentOption.Beskrivning = option.Beskrivning;

            // todo load session for productList, loop through and add to MatrattProdukt table

            //currentOption.MatrattProdukt = model.Matratt.MatrattProdukt;
            // todo solve data structure above then uncomment below
            //_context.SaveChanges();
            //HttpContext.Session.Remove("productList");

            return RedirectToAction("AdminView", "Navigation");
        }


        public async Task<IActionResult> SetUserRole(string userName, string newRole)
        {
            var user = _userManager.Users.First(o => o.UserName == userName);
            // todo I find roles on HQ, but not on Laptop. How do the DBs deffer?
            var roles = await _userManager.GetRolesAsync(user);
            
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, newRole);
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("AdminView", "Navigation");
        }

        public IActionResult CloseMatratt()
        {
            HttpContext.Session.Remove("productList");
            return RedirectToAction("AdminView", "Navigation");
        }
        public IActionResult SetOrderDelivered(int id)
        {
            _context.Bestallning.First(o => o.BestallningId == id).Levererad = true;
            _context.SaveChanges();
            return RedirectToAction("AdminView", "Navigation");
        }

        public IActionResult AddProductToMatratt(int productId, int matrattId)
        {
            ModalViewModel model = new ModalViewModel();
            var product = _context.Produkt.First(x => x.ProduktId == productId);

            if (HttpContext.Session.GetString("productList") != null)
            {
                var serialized = HttpContext.Session.GetString("productList");
                model = JsonConvert.DeserializeObject<ModalViewModel>(serialized);
            }
            else
            {
                model.Matratt = _context.Matratt.First(x => x.MatrattId == matrattId);
                model.Produkter = new List<Produkt>();
            }
            model.Produkter.Add(product);

            var str = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("productList",str);

            var modalModel = new ModalViewModel
            {
                Matratt = _context.Matratt.FirstOrDefault(x => x.MatrattId == matrattId),
                //Typer = _context.MatrattTyp.ToList(),
                Produkter = _context.Produkt.ToList(),
                //Title = "",
            };

            return PartialView("_ModalProductListPartial",modalModel);
        }

        public IActionResult RemoveProductFromMatratt(int productId, int matrattId)
        {
            // todo solve known issue where you remove product already in food.
            ModalViewModel model = new ModalViewModel();
            var product = _context.Produkt.First(x => x.ProduktId == productId);

            if (HttpContext.Session.GetString("productList") != null)
            {
                var serialized = HttpContext.Session.GetString("productList");
                model = JsonConvert.DeserializeObject<ModalViewModel>(serialized);
            }
            else
            {
                model.Matratt = _context.Matratt.First(x => x.MatrattId == matrattId);
                model.Produkter = new List<Produkt>();
            }
            model.Produkter.Remove(product);

            var str = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("productList", str);

            var modalModel = new ModalViewModel
            {
                Matratt = _context.Matratt.FirstOrDefault(x => x.MatrattId == matrattId),
                //Typer = _context.MatrattTyp.ToList(),
                Produkter = _context.Produkt.ToList(),
                //Title = "Redigera maträtt",
            };

            return PartialView("_ModalProductListPartial", modalModel);
        }


    }
}
