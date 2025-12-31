using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult StaffList()
        {
            // Create DataTable
            DataTable dt_staff = new DataTable("MOM_Staff");
            dt_staff.Columns.Add("StaffID", typeof(int));
            dt_staff.Columns.Add("StaffName", typeof(string));
            dt_staff.Columns.Add("DepartmentName", typeof(string));
            dt_staff.Columns.Add("MobileNo", typeof(string));
            dt_staff.Columns.Add("EmailAddress", typeof(string));

            // Static sample data
            dt_staff.Rows.Add(1, "Raj Patel", "Human Resources", "9876543210", "raj.patel@momify.com");
            dt_staff.Rows.Add(2, "Neha Sharma", "Information Technology", "9898989898", "neha.sharma@momify.com");
            dt_staff.Rows.Add(3, "Amit Joshi", "Finance", "9123456789", "amit.joshi@momify.com");
            dt_staff.Rows.Add(4, "Pooja Mehta", "Marketing", "9012345678", "pooja.mehta@momify.com");
            dt_staff.Rows.Add(5, "Ravi Desai", "Operations", "9988776655", "ravi.desai@momify.com");
            dt_staff.Rows.Add(6, "Khushi Shah", "Administration", "9090909090", "khushi.shah@momify.com");
            dt_staff.Rows.Add(7, "Suresh Kumar", "Quality Assurance", "9345678123", "suresh.kumar@momify.com");
            dt_staff.Rows.Add(8, "Priya Verma", "Sales", "9765432109", "priya.verma@momify.com");
            dt_staff.Rows.Add(9, "Arjun Singh", "Procurement", "9654321876", "arjun.singh@momify.com");
            dt_staff.Rows.Add(10, "Anjali Trivedi", "Management", "9876501234", "anjali.trivedi@momify.com");

            return View(dt_staff);
        }

        public IActionResult StaffAddEdit(int? id)
        {
            return View();
        }
    }
}
