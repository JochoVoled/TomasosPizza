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
        public async Task<IActionResult> UpdateUserRole(List<IdentityKund> identityUsers)
        {
            foreach (var idUser in identityUsers)
            {
                var user = _userManager.Users.First(o => o.Id == idUser.Id);
                // access the UsersRoles table (for the user)
                await _userManager.GetRolesAsync(user);
                // clear current roles (ideally just the one held previously, but this is easier)
                await _userManager.RemoveFromRolesAsync(user, new List<string> {"Administrator", "PremiumUser","RegularUser"});
                // find the one role checked in the list
                var newRole = _userManager.GetRolesAsync(user).Result.First();
                // assign that new role
                await _userManager.AddToRoleAsync(user, newRole);
            }
            return RedirectToAction("AdminView", "Navigation");   
        }

        [HttpPost]
        public IActionResult UpdateOrderDelivered(List<Bestallning> orders)
        {
            // todo debug, this looks to simple
            foreach (var order in orders)
            {
                // finn motsvarande i context (de bör mappa, så det vore effektivare, men detta har mindre risk att skriva fel)
                // skriv över dess levereradvärde till orders dito
                _context.Bestallning.First(o => o.BestallningId == order.BestallningId).Levererad = order.Levererad;
            }
            // save changes
            _context.SaveChanges();
            return RedirectToAction("AdminView", "Navigation");
            
        }

        
    }
}
