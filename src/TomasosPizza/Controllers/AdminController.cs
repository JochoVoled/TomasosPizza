using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            currentOption.MatrattProdukt = model.Matratt.MatrattProdukt; // todo Where do the checkbox info save before being sent here? This looks wrong
            // todo solve data structure above then uncomment below
            //_context.SaveChanges();

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
            return RedirectToAction("AdminView", "Navigation");
        }
        public IActionResult SetOrderDelivered(int id)
        {
            _context.Bestallning.First(o => o.BestallningId == id).Levererad = true;
            _context.SaveChanges();
            return RedirectToAction("AdminView", "Navigation");
        }
    }
}
