using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        // GET: /Account/Profile
        public IActionResult Profile()
        {
            return View();
        }

        // GET: /Account/Settings
        public IActionResult Settings()
        {
            return View();
        }
    }
}
