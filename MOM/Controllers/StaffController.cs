using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult StaffList()
        {
            return View();
        }
        public IActionResult StaffAddEdit()
        {
            return View();
        }
    }
}
