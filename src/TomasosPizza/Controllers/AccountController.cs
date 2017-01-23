using Microsoft.AspNetCore.Mvc;

namespace TomasosPizza.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return RedirectToAction("LogInView", "Navigation");
        }
    }
}
