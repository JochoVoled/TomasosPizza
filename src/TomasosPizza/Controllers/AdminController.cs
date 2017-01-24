using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            model.Matratt.MatrattProdukt = _context.MatrattProdukt.Where(x => x.MatrattId == model.Matratt.MatrattId).ToList();
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
            };
            return View("_ModalPartial", model);
        }

        [HttpPost]
        public IActionResult SaveMatratt(Matratt option)
        {
            // todo this method doesn't send input values, either
            // Update the food without any products in mind
            var currentOption = _context.Matratt.FirstOrDefault(x => x.MatrattId == option.MatrattId);
            currentOption.Beskrivning = option.Beskrivning;
            currentOption.Pris = option.Pris;
            currentOption.Beskrivning = option.Beskrivning;

            // If the Matratt ingredients have been changed, update them
            var serialized = HttpContext.Session.GetString("productList");
            if (!string.IsNullOrEmpty(serialized))
            {
                // Empty MatrattProdukt of all rows for selected Matratt
                var oldProducts = _context.MatrattProdukt.Where(x => x.MatrattId == option.MatrattId).ToList();
                _context.MatrattProdukt.RemoveRange(oldProducts);

                // Add selected Products to Matratt (thus, hopefully, negating previous removals)
                var modalModel = JsonConvert.DeserializeObject<ModalViewModel>(serialized);
                foreach (var product in modalModel.Produkter)
                {
                    var newProduct = new MatrattProdukt
                    {
                        Matratt = option,
                        Produkt = product,
                    };
                    _context.MatrattProdukt.Add(newProduct);
                }
            }
            _context.SaveChanges();
            HttpContext.Session.Remove("productList");

            return RedirectToAction("AdminView", "Navigation");
        }


        public async Task<IActionResult> SetUserRole(string userName, string newRole)
        {
            var user = _userManager.Users.First(o => o.UserName == userName);
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
        public IActionResult RemoveUndeliveredOrder(int id)
        {
            var order = _context.Bestallning.First(o => o.BestallningId == id);
            _context.Bestallning.Remove(order);
            var cascade = _context.BestallningMatratt.Where(x => x.BestallningId == order.BestallningId);
            _context.RemoveRange(cascade);
            _context.SaveChanges();
            return RedirectToAction("AdminView", "Navigation");
        }

        public IActionResult SetProductName(int productId, string newName)
        {
            // todo ProduktNamn not sent from view's text input
            var product = _context.Produkt.First(x => x.ProduktId == productId);
            product.ProduktNamn = newName;
            _context.SaveChanges();

            return RedirectToAction("AdminView", "Navigation");
        }
        public IActionResult AddProductToMatratt(int productId, int matrattId)
        {
            var model = new List<Produkt>();
            var product = _context.Produkt.First(x => x.ProduktId == productId);

            model = LoadOrInitializeModalModel(matrattId, model);
            model.Add(product);
            ReserializeProductList(model);

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
            var model = new List<Produkt>();
            var product = _context.Produkt.First(x => x.ProduktId == productId);

            model = LoadOrInitializeModalModel(matrattId, model);
            model.Remove(product);
            ReserializeProductList(model);

            var modalModel = new ModalViewModel
            {
                Matratt = _context.Matratt.FirstOrDefault(x => x.MatrattId == matrattId),
                //Typer = _context.MatrattTyp.ToList(),
                Produkter = _context.Produkt.ToList(),
                //Title = "Redigera maträtt",
            };

            return PartialView("_ModalProductListPartial", modalModel);
        }

        private void ReserializeProductList(List<Produkt> model)
        {
            // todo solve self-referencial loop
            var str = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("productList", str);
        }

        private List<Produkt> LoadOrInitializeModalModel(int matrattId, List<Produkt> model)
        {
            if (HttpContext.Session.GetString("productList") != null)
            {
                // todo evaluate if Working as Intended
                var serialized = HttpContext.Session.GetString("productList");
                model = JsonConvert.DeserializeObject<List<Produkt>>(serialized);
            }
            else
            {
                var matratt = _context.Matratt.FirstOrDefault(x => x.MatrattId == matrattId);
                if (matratt == null) return model;
                
                // get all products linked to Matratt in MatrattProdukt
                var relationRows = _context.MatrattProdukt.Where(x => x.MatrattId == matrattId).ToList();

                foreach (MatrattProdukt produkt in relationRows)
                {
                    var p = _context.Produkt.Find(produkt.ProduktId);
                    model.Add(p);
                }                
            }
            return model;
        }
    }
}
