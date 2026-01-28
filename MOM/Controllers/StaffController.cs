using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Services;
using System.Data;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {
        private readonly DataService _dataService;

        public StaffController()
        {
            _dataService = DataService.Instance;
        }

        public IActionResult StaffList(string searchTerm = "", string departmentFilter = "", string sortBy = "StaffName", string sortOrder = "asc")
        {
            try
            {
                var staff = _dataService.GetFilteredData(_dataService.Staff, searchTerm, null);
                
                // Apply department filter
                if (!string.IsNullOrEmpty(departmentFilter) && int.TryParse(departmentFilter, out int deptId))
                {
                    var filteredRows = staff.AsEnumerable()
                        .Where(r => r.Field<int>("DepartmentID") == deptId);
                    
                    var filteredTable = staff.Clone();
                    foreach (var row in filteredRows)
                    {
                        filteredTable.ImportRow(row);
                    }
                    staff = filteredTable;
                }

                // Apply sorting
                var sortedRows = staff.AsEnumerable();
                switch (sortBy.ToLower())
                {
                    case "staffname":
                        sortedRows = sortOrder == "desc" 
                            ? sortedRows.OrderByDescending(r => r.Field<string>("StaffName"))
                            : sortedRows.OrderBy(r => r.Field<string>("StaffName"));
                        break;
                    case "emailaddress":
                        sortedRows = sortOrder == "desc"
                            ? sortedRows.OrderByDescending(r => r.Field<string>("EmailAddress"))
                            : sortedRows.OrderBy(r => r.Field<string>("EmailAddress"));
                        break;
                    case "created":
                        sortedRows = sortOrder == "desc"
                            ? sortedRows.OrderByDescending(r => r.Field<DateTime>("Created"))
                            : sortedRows.OrderBy(r => r.Field<DateTime>("Created"));
                        break;
                    default:
                        sortedRows = sortedRows.OrderBy(r => r.Field<string>("StaffName"));
                        break;
                }

                var sortedTable = staff.Clone();
                foreach (var row in sortedRows)
                {
                    sortedTable.ImportRow(row);
                }

                // Add department names to the table
                foreach (DataRow row in sortedTable.Rows)
                {
                    var departmentId = row.Field<int>("DepartmentID");
                    var deptRow = _dataService.Departments.AsEnumerable()
                        .FirstOrDefault(d => d.Field<int>("DepartmentID") == departmentId);
                    
                    if (deptRow != null)
                    {
                        // Add department name as a computed column
                        if (!sortedTable.Columns.Contains("DepartmentName"))
                        {
                            sortedTable.Columns.Add("DepartmentName", typeof(string));
                        }
                        row["DepartmentName"] = deptRow.Field<string>("DepartmentName");
                    }
                }

                ViewBag.SearchTerm = searchTerm;
                ViewBag.DepartmentFilter = departmentFilter;
                ViewBag.SortBy = sortBy;
                ViewBag.SortOrder = sortOrder;
                ViewBag.TotalRecords = _dataService.Staff.Rows.Count;
                ViewBag.FilteredRecords = sortedTable.Rows.Count;
                ViewBag.Departments = _dataService.Departments.AsEnumerable()
                    .Select(d => new { Value = d.Field<int>("DepartmentID"), Text = d.Field<string>("DepartmentName") })
                    .ToList();

                return View(sortedTable);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading staff: {ex.Message}";
                return View(new DataTable());
            }
        }

        public IActionResult StaffAddEdit(int? id)
        {
            try
            {
                var model = new StaffModel();

                if (id.HasValue && id.Value > 0)
                {
                    var row = _dataService.Staff.AsEnumerable()
                                .FirstOrDefault(r => r.Field<int>("StaffID") == id.Value);

                    if (row == null)
                    {
                        TempData["ErrorMessage"] = "Staff member not found.";
                        return RedirectToAction("StaffList");
                    }

                    model.StaffID = row.Field<int>("StaffID");
                    model.DepartmentID = row.Field<int>("DepartmentID");
                    model.StaffName = row.Field<string>("StaffName") ?? "";
                    model.MobileNo = row.Field<string>("MobileNo") ?? "";
                    model.EmailAddress = row.Field<string>("EmailAddress") ?? "";
                    model.Remarks = row.Field<string>("Remarks") ?? "";
                    model.Created = row.Field<DateTime>("Created");
                    model.Modified = row.Field<DateTime>("Modified");

                    // Get department name
                    var deptRow = _dataService.Departments.AsEnumerable()
                        .FirstOrDefault(d => d.Field<int>("DepartmentID") == model.DepartmentID);
                    model.DepartmentName = deptRow?.Field<string>("DepartmentName") ?? "";

                    // Get meeting count for this staff
                    model.MeetingCount = _dataService.MeetingMembers.AsEnumerable()
                        .Count(r => r.Field<int>("StaffID") == id.Value);
                }

                // Get departments for dropdown
                ViewBag.Departments = _dataService.Departments.AsEnumerable()
                    .Select(d => new { Value = d.Field<int>("DepartmentID"), Text = d.Field<string>("DepartmentName") })
                    .OrderBy(d => d.Text)
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading staff member: {ex.Message}";
                return RedirectToAction("StaffList");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StaffAddEdit(StaffModel model)
        {
            try
            {
                // Custom validation for duplicate email
                var existingStaff = _dataService.Staff.AsEnumerable()
                    .FirstOrDefault(r => r.Field<string>("EmailAddress")?.Equals(model.EmailAddress, StringComparison.OrdinalIgnoreCase) == true
                                    && r.Field<int>("StaffID") != model.StaffID);

                if (existingStaff != null)
                {
                    ModelState.AddModelError("EmailAddress", "A staff member with this email address already exists.");
                }

                // Validate department exists
                var deptExists = _dataService.Departments.AsEnumerable()
                    .Any(d => d.Field<int>("DepartmentID") == model.DepartmentID);

                if (!deptExists)
                {
                    ModelState.AddModelError("DepartmentID", "Selected department does not exist.");
                }

                if (!ModelState.IsValid)
                {
                    // Reload data for edit mode
                    if (model.StaffID > 0)
                    {
                        var deptRow = _dataService.Departments.AsEnumerable()
                            .FirstOrDefault(d => d.Field<int>("DepartmentID") == model.DepartmentID);
                        model.DepartmentName = deptRow?.Field<string>("DepartmentName") ?? "";
                        
                        model.MeetingCount = _dataService.MeetingMembers.AsEnumerable()
                            .Count(r => r.Field<int>("StaffID") == model.StaffID);
                    }

                    // Reload departments for dropdown
                    ViewBag.Departments = _dataService.Departments.AsEnumerable()
                        .Select(d => new { Value = d.Field<int>("DepartmentID"), Text = d.Field<string>("DepartmentName") })
                        .OrderBy(d => d.Text)
                        .ToList();

                    return View(model);
                }

                if (model.StaffID == 0)
                {
                    // Add new staff
                    var newId = _dataService.GetNextId(_dataService.Staff, "StaffID");
                    _dataService.Staff.Rows.Add(
                        newId,
                        model.DepartmentID,
                        model.StaffName.Trim(),
                        model.MobileNo.Trim(),
                        model.EmailAddress.Trim().ToLower(),
                        model.Remarks?.Trim() ?? "",
                        DateTime.Now,
                        DateTime.Now
                    );
                    TempData["SuccessMessage"] = "Staff member added successfully.";
                }
                else
                {
                    // Update existing staff
                    var row = _dataService.Staff.AsEnumerable()
                                .FirstOrDefault(r => r.Field<int>("StaffID") == model.StaffID);

                    if (row != null)
                    {
                        row["DepartmentID"] = model.DepartmentID;
                        row["StaffName"] = model.StaffName.Trim();
                        row["MobileNo"] = model.MobileNo.Trim();
                        row["EmailAddress"] = model.EmailAddress.Trim().ToLower();
                        row["Remarks"] = model.Remarks?.Trim() ?? "";
                        row["Modified"] = DateTime.Now;
                        TempData["SuccessMessage"] = "Staff member updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Staff member not found for update.";
                    }
                }

                return RedirectToAction("StaffList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error saving staff member: {ex.Message}";
                
                // Reload departments for dropdown
                ViewBag.Departments = _dataService.Departments.AsEnumerable()
                    .Select(d => new { Value = d.Field<int>("DepartmentID"), Text = d.Field<string>("DepartmentName") })
                    .OrderBy(d => d.Text)
                    .ToList();

                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                // Check if staff has meeting memberships
                var hasMeetings = _dataService.MeetingMembers.AsEnumerable()
                    .Any(r => r.Field<int>("StaffID") == id);

                if (hasMeetings)
                {
                    return Json(new { success = false, message = "Cannot delete staff member. They have associated meeting memberships." });
                }

                var row = _dataService.Staff.AsEnumerable()
                            .FirstOrDefault(r => r.Field<int>("StaffID") == id);

                if (row != null)
                {
                    _dataService.Staff.Rows.Remove(row);
                    return Json(new { success = true, message = "Staff member deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Staff member not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting staff member: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult GetStaffDetails(int id)
        {
            try
            {
                var row = _dataService.Staff.AsEnumerable()
                            .FirstOrDefault(r => r.Field<int>("StaffID") == id);

                if (row == null)
                {
                    return Json(new { success = false, message = "Staff member not found." });
                }

                var deptRow = _dataService.Departments.AsEnumerable()
                    .FirstOrDefault(d => d.Field<int>("DepartmentID") == row.Field<int>("DepartmentID"));

                var meetingCount = _dataService.MeetingMembers.AsEnumerable()
                    .Count(r => r.Field<int>("StaffID") == id);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        StaffID = row.Field<int>("StaffID"),
                        StaffName = row.Field<string>("StaffName"),
                        DepartmentName = deptRow?.Field<string>("DepartmentName") ?? "Unknown",
                        MobileNo = row.Field<string>("MobileNo"),
                        EmailAddress = row.Field<string>("EmailAddress"),
                        Remarks = row.Field<string>("Remarks") ?? "",
                        Created = row.Field<DateTime>("Created").ToString("dd/MM/yyyy hh:mm tt"),
                        Modified = row.Field<DateTime>("Modified").ToString("dd/MM/yyyy hh:mm tt"),
                        MeetingCount = meetingCount
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error getting staff details: {ex.Message}" });
            }
        }

        [HttpGet]
        public JsonResult GetStaffByDepartment(int departmentId)
        {
            try
            {
                var staff = _dataService.Staff.AsEnumerable()
                    .Where(s => s.Field<int>("DepartmentID") == departmentId)
                    .Select(row => new
                    {
                        Value = row.Field<int>("StaffID"),
                        Text = row.Field<string>("StaffName")
                    })
                    .OrderBy(s => s.Text)
                    .ToList();

                return Json(staff);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetAllStaff()
        {
            try
            {
                var staff = _dataService.Staff.AsEnumerable()
                    .Select(row => new
                    {
                        Value = row.Field<int>("StaffID"),
                        Text = row.Field<string>("StaffName"),
                        DepartmentID = row.Field<int>("DepartmentID")
                    })
                    .OrderBy(s => s.Text)
                    .ToList();

                return Json(staff);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
