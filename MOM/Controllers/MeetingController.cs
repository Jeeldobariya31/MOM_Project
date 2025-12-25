using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class MeetingController : Controller
    {
        public IActionResult MeetingList()
        {
            return View();
        }
        public IActionResult MeetingAddEdit()
        {
            return View();
        }
    }
}
