using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingVenueController : Controller
    {
        public IActionResult MeetingVenueList()
        {
            // Create DataTable
            DataTable dt_meeting_venue = new DataTable("MOM_MeetingVenue");
            dt_meeting_venue.Columns.Add("MeetingVenueID", typeof(int));
            dt_meeting_venue.Columns.Add("MeetingVenueName", typeof(string));
            dt_meeting_venue.Columns.Add("Created", typeof(DateTime));
            dt_meeting_venue.Columns.Add("Modified", typeof(DateTime));

            // Static data (sample)
            dt_meeting_venue.Rows.Add(1, "Block A - Ground Floor - Room 101", new DateTime(2024, 01, 05), new DateTime(2024, 02, 01));
            dt_meeting_venue.Rows.Add(2, "Block A - First Floor - Room 201", new DateTime(2024, 01, 06), new DateTime(2024, 02, 02));
            dt_meeting_venue.Rows.Add(3, "Block B - Ground Floor - Room 102", new DateTime(2024, 01, 07), new DateTime(2024, 02, 03));
            dt_meeting_venue.Rows.Add(4, "Block B - First Floor - Room 202", new DateTime(2024, 01, 08), new DateTime(2024, 02, 04));
            dt_meeting_venue.Rows.Add(5, "Block C - Second Floor - Room 301", new DateTime(2024, 01, 10), new DateTime(2024, 02, 06));
            dt_meeting_venue.Rows.Add(6, "Block D - Ground Floor - Room 103", new DateTime(2024, 01, 12), new DateTime(2024, 02, 08));
            dt_meeting_venue.Rows.Add(7, "Block E - First Floor - Room 203", new DateTime(2024, 01, 14), new DateTime(2024, 02, 10));
            dt_meeting_venue.Rows.Add(8, "Block G - Second Floor - Room 302", new DateTime(2024, 01, 16), new DateTime(2024, 02, 12));
            dt_meeting_venue.Rows.Add(9, "Block H - Second Floor - Room 401", new DateTime(2024, 01, 18), new DateTime(2024, 02, 14));
            dt_meeting_venue.Rows.Add(10, "Block H - Third Floor - Auditorium", new DateTime(2024, 01, 20), new DateTime(2024, 02, 16));

            return View(dt_meeting_venue);
        }

        public IActionResult MeetingVenueAddEdit(int? id)
        {
            return View();
        }
    }
}
