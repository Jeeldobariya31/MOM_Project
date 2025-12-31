using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingMembersController : Controller
    {
        public IActionResult MeetingMemberList()
        {
            // Create DataTable
            DataTable dt_meeting_member = new DataTable("MOM_MeetingMember");
            dt_meeting_member.Columns.Add("MeetingMemberID", typeof(int));
            dt_meeting_member.Columns.Add("MeetingTitle", typeof(string));
            dt_meeting_member.Columns.Add("StaffName", typeof(string));
            dt_meeting_member.Columns.Add("DepartmentName", typeof(string));
            dt_meeting_member.Columns.Add("IsPresent", typeof(bool));
            dt_meeting_member.Columns.Add("Remarks", typeof(string));

            // Static data (sample)
            dt_meeting_member.Rows.Add(1, "Board Meeting – 01 Mar 2024", "Raj Patel", "Administration", true, "Attended full meeting");
            dt_meeting_member.Rows.Add(2, "Client Meeting – 02 Mar 2024", "Neha Sharma", "IT", false, "On leave");
            dt_meeting_member.Rows.Add(3, "Team Stand-up – 03 Mar 2024", "Amit Joshi", "Finance", true, "");
            dt_meeting_member.Rows.Add(4, "Project Review – 04 Mar 2024", "Pooja Mehta", "Marketing", true, "Presented update");
            dt_meeting_member.Rows.Add(5, "Training Session – 05 Mar 2024", "Ravi Desai", "Operations", true, "Completed training");
            dt_meeting_member.Rows.Add(6, "Audit Meeting – 06 Mar 2024", "Khushi Shah", "Administration", false, "Medical emergency");
            dt_meeting_member.Rows.Add(7, "Vendor Meeting – 07 Mar 2024", "Suresh Kumar", "Quality Assurance", true, "");
            dt_meeting_member.Rows.Add(8, "Strategy Meeting – 08 Mar 2024", "Priya Verma", "Sales", true, "Shared strategy inputs");
            dt_meeting_member.Rows.Add(9, "Internal Review – 09 Mar 2024", "Arjun Singh", "Procurement", false, "Client visit");
            dt_meeting_member.Rows.Add(10, "Emergency Meeting – 10 Mar 2024", "Anjali Trivedi", "Management", true, "Approved emergency actions");

            return View(dt_meeting_member);
        }
        public IActionResult MeetingMemberAddEdit()
        {
            return View();
        }
    }
}
