using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Services;
using System.Data;

namespace MOM.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly DataService _dataService;

        public DepartmentController()
        {
            _dataService = DataService.Instance;
        }

        public IActionResult DepartmentList(string searchTerm = "", string sortBy = "DepartmentName", string sortOrder = "asc")
        {
            try
            {
                var departments = _dataService.GetFilteredData(_dataService.Departments, searchTerm, null);
                
                // Apply sorting
                var sortedRows = departments.AsEnumerable();
                switch (sortBy.ToLower())
                {
                    case "departmentname":
                        sortedRows = sortOrder == "desc" 
                            ? sortedRows.OrderByDescending(r => r.Field<string>("DepartmentName"))
                            : sortedRows.OrderBy(r => r.Field<string>("DepartmentName"));
                        break;
                    case "created":
                        sortedRows = sortOrder == "desc"
                            ? sortedRows.OrderByDescending(r => r.Field<DateTime>("Created"))
                            : sortedRows.OrderBy(r => r.Field<DateTime>("Created"));
                        break;
                    default:
                        sortedRows = sortedRows.OrderBy(r => r.Field<string>("DepartmentName"));
                        break;
                }

                var sortedTable = departments.Clone();
                foreach (var row in sortedRows)
                {
                    sortedTable.ImportRow(row);
                }

                ViewBag.SearchTerm = searchTerm;
                ViewBag.SortBy = sortBy;
                ViewBag.SortOrder = sortOrder;
                ViewBag.TotalRecords = _dataService.Departments.Rows.Count;
                ViewBag.FilteredRecords = sortedTable.Rows.Count;

                return View(sortedTable);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading departments: {ex.Message}";
                return View(new DataTable());
            }
        }

        public IActionResult DepartmentAddEdit(int? id)
        {
            try
            {
                var model = new DepartmentModel();

                if (id.HasValue && id.Value > 0)
                {
                    var row = _dataService.Departments.AsEnumerable()
                                .FirstOrDefault(r => r.Field<int>("DepartmentID") == id.Value);

                    if (row == null)
                    {
                        TempData["ErrorMessage"] = "Department not found.";
                        return RedirectToAction("DepartmentList");
                    }

                    model.DepartmentID = row.Field<int>("DepartmentID");
                    model.DepartmentName = row.Field<string>("DepartmentName") ?? "";
                    model.Created = row.Field<DateTime>("Created");
                    model.Modified = row.Field<DateTime>("Modified");

                    // Get staff count for this department
                    model.StaffCount = _dataService.Staff.AsEnumerable()
                        .Count(r => r.Field<int>("DepartmentID") == id.Value);

                    // Get meeting count for this department
                    model.MeetingCount = _dataService.Meetings.AsEnumerable()
                        .Count(r => r.Field<int>("DepartmentID") == id.Value);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading department: {ex.Message}";
                return RedirectToAction("DepartmentList");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DepartmentAddEdit(DepartmentModel model)
        {
            try
            {
                // Custom validation for duplicate names
                var existingDept = _dataService.Departments.AsEnumerable()
                    .FirstOrDefault(r => r.Field<string>("DepartmentName")?.Equals(model.DepartmentName, StringComparison.OrdinalIgnoreCase) == true
                                    && r.Field<int>("DepartmentID") != model.DepartmentID);

                if (existingDept != null)
                {
                    ModelState.AddModelError("DepartmentName", "A department with this name already exists.");
                }

                if (!ModelState.IsValid)
                {
                    // Reload counts for edit mode
                    if (model.DepartmentID > 0)
                    {
                        model.StaffCount = _dataService.Staff.AsEnumerable()
                            .Count(r => r.Field<int>("DepartmentID") == model.DepartmentID);
                        model.MeetingCount = _dataService.Meetings.AsEnumerable()
                            .Count(r => r.Field<int>("DepartmentID") == model.DepartmentID);
                    }
                    return View(model);
                }

                if (model.DepartmentID == 0)
                {
                    // Add new department
                    var newId = _dataService.GetNextId(_dataService.Departments, "DepartmentID");
                    _dataService.Departments.Rows.Add(
                        newId,
                        model.DepartmentName.Trim(),
                        DateTime.Now,
                        DateTime.Now
                    );
                    TempData["SuccessMessage"] = "Department added successfully.";
                }
                else
                {
                    // Update existing department
                    var row = _dataService.Departments.AsEnumerable()
                                .FirstOrDefault(r => r.Field<int>("DepartmentID") == model.DepartmentID);

                    if (row != null)
                    {
                        row["DepartmentName"] = model.DepartmentName.Trim();
                        row["Modified"] = DateTime.Now;
                        TempData["SuccessMessage"] = "Department updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Department not found for update.";
                    }
                }

                return RedirectToAction("DepartmentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error saving department: {ex.Message}";
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                // Check if department has staff
                var hasStaff = _dataService.Staff.AsEnumerable()
                    .Any(r => r.Field<int>("DepartmentID") == id);

                if (hasStaff)
                {
                    return Json(new { success = false, message = "Cannot delete department. It has associated staff members." });
                }

                // Check if department has meetings
                var hasMeetings = _dataService.Meetings.AsEnumerable()
                    .Any(r => r.Field<int>("DepartmentID") == id);

                if (hasMeetings)
                {
                    return Json(new { success = false, message = "Cannot delete department. It has associated meetings." });
                }

                var row = _dataService.Departments.AsEnumerable()
                            .FirstOrDefault(r => r.Field<int>("DepartmentID") == id);

                if (row != null)
                {
                    _dataService.Departments.Rows.Remove(row);
                    return Json(new { success = true, message = "Department deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Department not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting department: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult GetDepartmentDetails(int id)
        {
            try
            {
                var row = _dataService.Departments.AsEnumerable()
                            .FirstOrDefault(r => r.Field<int>("DepartmentID") == id);

                if (row == null)
                {
                    return Json(new { success = false, message = "Department not found." });
                }

                var staffCount = _dataService.Staff.AsEnumerable()
                    .Count(r => r.Field<int>("DepartmentID") == id);

                var meetingCount = _dataService.Meetings.AsEnumerable()
                    .Count(r => r.Field<int>("DepartmentID") == id);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        DepartmentID = row.Field<int>("DepartmentID"),
                        DepartmentName = row.Field<string>("DepartmentName"),
                        Created = row.Field<DateTime>("Created").ToString("dd/MM/yyyy hh:mm tt"),
                        Modified = row.Field<DateTime>("Modified").ToString("dd/MM/yyyy hh:mm tt"),
                        StaffCount = staffCount,
                        MeetingCount = meetingCount
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error getting department details: {ex.Message}" });
            }
        }

        [HttpGet]
        public JsonResult GetDepartments()
        {
            try
            {
                var departments = _dataService.Departments.AsEnumerable()
                    .Select(row => new
                    {
                        Value = row.Field<int>("DepartmentID"),
                        Text = row.Field<string>("DepartmentName")
                    })
                    .OrderBy(d => d.Text)
                    .ToList();

                return Json(departments);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
