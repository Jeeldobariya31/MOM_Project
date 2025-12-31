using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingController : Controller
    {
        public IActionResult MeetingList()
        {
            DataTable dt_meeting = new DataTable("MOM_Meetings");

            dt_meeting.Columns.Add("MeetingID", typeof(int));
            dt_meeting.Columns.Add("MeetingDate", typeof(DateTime));
            dt_meeting.Columns.Add("VenueName", typeof(string));
            dt_meeting.Columns.Add("MeetingTypeName", typeof(string));
            dt_meeting.Columns.Add("DepartmentName", typeof(string));
            dt_meeting.Columns.Add("IsCancelled", typeof(bool));
         

            // Sample data
            dt_meeting.Rows.Add(1, new DateTime(2024, 03, 01, 10, 00, 0), "Block A – Room 101", "Board Meeting", "Administration", false);
            dt_meeting.Rows.Add(2, new DateTime(2024, 03, 02, 11, 30, 0), "Block B – Room 201", "Client Meeting", "Sales", false);
            dt_meeting.Rows.Add(3, new DateTime(2024, 03, 03, 09, 00, 0), "Block C – Room 102", "Team Stand-up", "IT", false);
            dt_meeting.Rows.Add(4, new DateTime(2024, 03, 04, 14, 00, 0), "Block D – Room 202", "Project Review", "Operations", true);
            dt_meeting.Rows.Add(5, new DateTime(2024, 03, 05, 16, 00, 0), "Block E – Auditorium", "Training Session", "HR", false);
            dt_meeting.Rows.Add(6, new DateTime(2024, 03, 06, 10, 30, 0), "Block F – Room 301", "Audit Meeting", "Finance", false);
            dt_meeting.Rows.Add(7, new DateTime(2024, 03, 07, 15, 00, 0), "Block G – Room 203", "Vendor Meeting", "Procurement", false);
            dt_meeting.Rows.Add(8, new DateTime(2024, 03, 08, 11, 00, 0), "Block H – Room 302", "Strategy Meeting", "Management", false);
            dt_meeting.Rows.Add(9, new DateTime(2024, 03, 09, 13, 00, 0), "Block A – Room 102", "Internal Review", "Quality Assurance", true);
            dt_meeting.Rows.Add(10, new DateTime(2024, 03, 10, 17, 00, 0), "Block C – Auditorium", "Emergency Meeting", "Administration", true);

            return View(dt_meeting);
        }

        public IActionResult MeetingAddEdit(int? id)
        {
            return View();
        }
    }
}
