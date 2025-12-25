using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class MeetingVenueController : Controller
    {
        public IActionResult MeetingVenueList()
        {
            return View();
        }
        public IActionResult MeetingVenueAddEdit()
        {
            return View();
        }

    }
}
