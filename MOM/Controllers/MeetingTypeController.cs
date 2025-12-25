using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class MeetingTypeController : Controller
    {
        public IActionResult MeetingTypeList()
        {
            return View();
        }

        public IActionResult MeetingTypeAddEdit()
        {
            return View();
        }

    }
}
