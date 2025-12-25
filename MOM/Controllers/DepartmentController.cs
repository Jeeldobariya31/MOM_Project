using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class DepartmentController : Controller
    {
        public IActionResult DepartmentList()
        {
            return View();
        }
        public IActionResult DepartmentAddEdit()
        {
            return View();
        }
    }
}
