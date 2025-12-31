using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingTypeController : Controller
    {
        public IActionResult MeetingTypeList()
        {
            // Create DataTable
            DataTable dt_meeting_type = new DataTable("MOM_MeetingType");
            dt_meeting_type.Columns.Add("MeetingTypeID", typeof(int));
            dt_meeting_type.Columns.Add("MeetingTypeName", typeof(string));
            dt_meeting_type.Columns.Add("Remarks", typeof(string));
            dt_meeting_type.Columns.Add("Created", typeof(DateTime));
            dt_meeting_type.Columns.Add("Modified", typeof(DateTime));

            // Static data (sample)
            dt_meeting_type.Rows.Add(1, "Board Meeting", "Quarterly board discussions", DateTime.Now, DateTime.Now);
            dt_meeting_type.Rows.Add(2, "Client Meeting", "Client requirement discussion", DateTime.Now, DateTime.Now);
            dt_meeting_type.Rows.Add(3, "Team Stand-up", "Daily team sync", DateTime.Now, DateTime.Now);
            dt_meeting_type.Rows.Add(4, "Project Review", "Milestone & progress review", DateTime.Now, DateTime.Now);
            dt_meeting_type.Rows.Add(5, "Training Session", "Internal knowledge sharing", DateTime.Now, DateTime.Now);
            dt_meeting_type.Rows.Add(6, "Audit Meeting", "Compliance and audit review", DateTime.Now, DateTime.Now);
            dt_meeting_type.Rows.Add(7, "Vendor Meeting", "External vendor coordination", DateTime.Now, DateTime.Now);
            dt_meeting_type.Rows.Add(8, "Strategy Meeting", "Business planning and strategy discussion", DateTime.Now, DateTime.Now);
            dt_meeting_type.Rows.Add(9, "Internal Review", "Department performance review", DateTime.Now, DateTime.Now);
            dt_meeting_type.Rows.Add(10, "Emergency Meeting", "Urgent decision making", DateTime.Now, DateTime.Now);

            return View(dt_meeting_type);
        }

        public IActionResult MeetingTypeAddEdit(int? id)
        {
            // Later you can load data by id
            return View();
        }
    }
}
