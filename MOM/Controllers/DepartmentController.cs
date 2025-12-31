using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MOM.Controllers
{
    public class DepartmentController : Controller
    {
        public IActionResult DepartmentList()
        {
            // Create DataTable
            DataTable dt_department = new DataTable("MOM_Department");
            dt_department.Columns.Add("DepartmentID", typeof(int));
            dt_department.Columns.Add("DepartmentName", typeof(string));
            dt_department.Columns.Add("Created", typeof(DateTime));
            dt_department.Columns.Add("Modified", typeof(DateTime));

            // Sample data
            dt_department.Rows.Add(1, "Human Resources", new DateTime(2024, 01, 10), new DateTime(2024, 02, 01));
            dt_department.Rows.Add(2, "Information Technology", new DateTime(2024, 01, 12), new DateTime(2024, 02, 05));
            dt_department.Rows.Add(3, "Finance", new DateTime(2024, 01, 15), new DateTime(2024, 02, 07));
            dt_department.Rows.Add(4, "Marketing", new DateTime(2024, 01, 18), new DateTime(2024, 02, 10));
            dt_department.Rows.Add(5, "Operations", new DateTime(2024, 01, 20), new DateTime(2024, 02, 12));
            dt_department.Rows.Add(6, "Administration", new DateTime(2024, 01, 22), new DateTime(2024, 02, 14));
            dt_department.Rows.Add(7, "Research & Development", new DateTime(2024, 01, 25), new DateTime(2024, 02, 16));
            dt_department.Rows.Add(8, "Customer Support", new DateTime(2024, 01, 28), new DateTime(2024, 02, 18));
            dt_department.Rows.Add(9, "Sales", new DateTime(2024, 01, 30), new DateTime(2024, 02, 20));
            dt_department.Rows.Add(10, "Quality Assurance", new DateTime(2024, 02, 01), new DateTime(2024, 02, 22));

            return View(dt_department);
        }

        public IActionResult DepartmentAddEdit(int? id)
        {
            return View();
        }
    }
}
