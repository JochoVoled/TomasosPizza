using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TomasosPizza.IdentityModels;
using TomasosPizza.Models;
using TomasosPizza.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

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
                /*
                 * 	[I adminvyn] skall det gå att:
                 * 	   lägga till/uppdatera pizzor/maträtter och ingredienser,
                 * 	   ta bort ordrar eller ändra status på en order.
                 *  Det skall även gå att uppdatera en RegularUser till PremiumUser eller tvärtom.
                 */

        public IActionResult BeginUpdateMatratt()
        {
            // Redigera-knappen öppnar modal med formulär: Textfält för beskrivning, Dropdown för typ, lista med checkbox för ingrediens
            throw new NotImplementedException();
        }
        public IActionResult AddNewToMenu()
        {
            //Skapa Ny-knappen öppnar samma modal utan förifyllda värden
            var option = new Matratt
            {
                MatrattNamn = "",
                Beskrivning = "",
                Pris = 0,
                MatrattProdukt = new List<MatrattProdukt>()
            };
            ModalViewModel model = new ModalViewModel
            {
                Matratt = option,
                Produkter = _context.Produkt.ToList()
                //Produkter = _context.Produkt.Where(order => order.MatrattProdukt.Equals(option.MatrattProdukt)).ToList()
            };
            return PartialView("_ModalPartial", model);
            //throw new NotImplementedException();
        }

        public IActionResult AddNewToMenu(Matratt option)
        {
            //Skapa Ny-knappen öppnar samma modal utan förifyllda värden
            ModalViewModel model = new ModalViewModel
            {
                Matratt = option,
                Produkter = _context.Produkt.ToList()
            };
            return PartialView("_ModalPartial", model);
            //throw new NotImplementedException();
        }
        public IActionResult SaveMatratt()
        {
            // Spara Maträtt till databasen

            return RedirectToAction("AdminView", "Navigation");
        }
        public IActionResult UpdateUserRole(List<IdentityKund> identityUsers)
        {
            var users = _userManager.Users;
            foreach (var idUser in identityUsers)
            {
                var user = users.SingleOrDefault(x => x.UserName == idUser.UserName);

            }
            return RedirectToAction("AdminView", "Navigation");   
        }

        public IActionResult UpdateOrderDelivered(List<Bestallning> orders)
        {
            return RedirectToAction("AdminView", "Navigation");
            
        }

        
    }
}
