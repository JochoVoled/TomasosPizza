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

        #region NewMatratt
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
            return View("NewMatrattView", model);
        }
        private void ReserializeProductList(List<Produkt> model)
        {

            var str = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("productList", str);
        }

        private List<Produkt> LoadOrInitializeProductList(int productId, List<Produkt> model)
        {
            if (HttpContext.Session.GetString("productList") != null)
            {
                // todo evaluate if Working as Intended
                var serialized = HttpContext.Session.GetString("productList");
                model = JsonConvert.DeserializeObject<List<Produkt>>(serialized);
            }
            return model;
        }
        [HttpPost]
        public IActionResult AddMatratt(string newName, string newDesc, int newPrice, string newCat)
        {
            var newMatratt = new Matratt
            {
                MatrattNamn = newName,
                Beskrivning = newDesc,
                Pris = newPrice,
                MatrattTyp = int.Parse(newCat)
            };
            _context.Matratt.Add(newMatratt);
            _context.SaveChanges();
            var currentOption = _context.Matratt.OrderByDescending(x => x.MatrattId).FirstOrDefault();

            // Only add Product-Matratt relations if the Matratt ingredients have been set
            var serialized = HttpContext.Session.GetString("productList");
            if (!string.IsNullOrEmpty(serialized))
            {
                // Add selected Products to new Matratt
                var modalModel = JsonConvert.DeserializeObject<List<Produkt>>(serialized);
                foreach (var product in modalModel)
                {
                    var newProduct = new MatrattProdukt
                    {
                        MatrattId = currentOption.MatrattId,
                        ProduktId = product.ProduktId,
                    };
                    _context.MatrattProdukt.Add(newProduct);
                }
            }

            _context.SaveChanges();
            HttpContext.Session.Remove("productList");

            return RedirectToAction("AdminView", "Navigation");
        }
        public IActionResult AddProductToNewMatratt(int productId)
        {
            var model = new List<Produkt>();

            model = LoadOrInitializeProductList(productId, model);
            // I figure a user may buttonmash the 'Add' link due to lacking feedback
            if (model.All(x => x.ProduktId != productId))
            {
                model.Add(_context.Produkt.First(x => x.ProduktId == productId));
            }
            ReserializeProductList(model);

            return RedirectToAction("AddNewToMenu");
        }
        public IActionResult RemoveProductFromNewMatratt(int productId)
        {
            var model = new List<Produkt>();

            model = LoadOrInitializeProductList(productId, model);
            if (model.Any(x => x.ProduktId == productId))
            {
                model.Remove(_context.Produkt.First(x => x.ProduktId == productId));
            }
            ReserializeProductList(model);

            return RedirectToAction("AddNewToMenu");
        }
        #endregion
        #region ExistingMatratt
        public IActionResult BeginUpdateMatratt(int id)
        {
            var matratt = _context.Matratt.FirstOrDefault(x => x.MatrattId == id);

            var model = new ModalViewModel
            {
                Matratt = matratt,
                Typer = _context.MatrattTyp.ToList(),
                Produkter = _context.Produkt.ToList(),
                Title = "Redigera maträtt",
            };
            model.Matratt.MatrattProdukt = _context.MatrattProdukt.Where(x => x.MatrattId == matratt.MatrattId).ToList();
            return View("UpdateMatrattView", model);
        }
        public IActionResult UpdateMatratt(int id, string newName, string newDesc, int newPrice, string newCat)
        {
            var currentOption = _context.Matratt.FirstOrDefault(x => x.MatrattId == id);
            // todo evaluate both adding new and editing existing

            // redigera befintlig
            currentOption.MatrattNamn = newName;
            currentOption.Beskrivning = newDesc;
            currentOption.Pris = newPrice;
            currentOption.MatrattTyp = _context.MatrattTyp.First(x => x.MatrattTyp1 == int.Parse(newCat)).MatrattTyp1;

            _context.SaveChanges();

            return RedirectToAction("AdminView", "Navigation");
        }
        public IActionResult AddProductToExistingMatratt(int productId, int matrattId)
        {
            // if the Interface sends the wrong data, check to prevent double links
            if (!_context.MatrattProdukt.Any(x => x.ProduktId == productId && x.MatrattId == matrattId))
            {
                var matrattProdukt = new MatrattProdukt
                {
                    MatrattId = matrattId,
                    ProduktId = productId
                };
                _context.MatrattProdukt.Add(matrattProdukt);
                _context.SaveChanges();
            }

            return RedirectToAction("BeginUpdateMatratt", new { id = matrattId});
        }
        public IActionResult RemoveProductFromExistingMatratt(int productId, int matrattId)
        {
            var matrattProdukt =
                _context.MatrattProdukt.First(x => x.ProduktId == productId && x.MatrattId == matrattId);
            _context.MatrattProdukt.Remove(matrattProdukt);
            _context.SaveChanges();

            return RedirectToAction("BeginUpdateMatratt", new { id = matrattId });
        }
        #endregion

        #region ProductManagement

        public IActionResult SetProductName(int id, string newName)
        {
            var product = _context.Produkt.First(x => x.ProduktId == id);
            product.ProduktNamn = newName;
            _context.SaveChanges();

            return RedirectToAction("AdminView", "Navigation");
        }
        public IActionResult NewProduct(string newProductName)
        {
            var product = new Produkt
            {
                ProduktNamn = newProductName
            };
            _context.Produkt.Add(product);
            _context.SaveChanges();
            return RedirectToAction("AdminView", "Navigation");
        }
        #endregion
        #region OrderManagement
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
        #endregion
        #region UserManagement
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
        #endregion
        public IActionResult CloseMatratt()
        {
            HttpContext.Session.Remove("productList");
            return RedirectToAction("AdminView", "Navigation");
        }
    }
}
