using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
