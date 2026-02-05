using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Services;
using System.Data;
using System.Data.SqlClient;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {
        private readonly DataService _dataService;
        private readonly IConfiguration _configuration;

        public StaffController(IConfiguration configuration)
        {
            _dataService = DataService.Instance;
            _configuration = configuration;
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

                // Get departments for dropdown - with better error handling
                var departmentsList = new List<dynamic>();
                
                if (_dataService.Departments != null && _dataService.Departments.Rows.Count > 0)
                {
                    departmentsList = _dataService.Departments.AsEnumerable()
                        .Select(d => new { 
                            Value = d.Field<int>("DepartmentID"), 
                            Text = d.Field<string>("DepartmentName") ?? "Unknown Department" 
                        })
                        .OrderBy(d => d.Text)
                        .ToList<dynamic>();
                }
                else
                {
                    // Fallback: Load departments directly from database if DataService failed
                    try
                    {
                        var fallbackDepts = GetDepartmentsFromDatabase();
                        departmentsList = fallbackDepts.Select(d => new { Value = d.Value, Text = d.Text }).ToList<dynamic>();
                    }
                    catch (Exception dbEx)
                    {
                        Console.WriteLine($"Error loading departments from database: {dbEx.Message}");
                        // Last resort: Add some default departments
                        departmentsList = new List<dynamic>
                        {
                            new { Value = 1, Text = "Human Resources" },
                            new { Value = 2, Text = "Information Technology" },
                            new { Value = 3, Text = "Finance & Accounts" }
                        };
                    }
                }

                ViewBag.Departments = departmentsList;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading staff member: {ex.Message}";
                
                // Still try to provide departments for the view
                ViewBag.Departments = new List<dynamic>
                {
                    new { Value = 1, Text = "Human Resources" },
                    new { Value = 2, Text = "Information Technology" },
                    new { Value = 3, Text = "Finance & Accounts" }
                };
                
                return View(new StaffModel());
            }
        }

        private List<dynamic> GetDepartmentsFromDatabase()
        {
            var departments = new List<dynamic>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("PR_Department_SelectAll", connection);
                command.CommandType = CommandType.StoredProcedure;
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        departments.Add(new { 
                            Value = reader.GetInt32("DepartmentID"), 
                            Text = reader.GetString("DepartmentName") 
                        });
                    }
                }
            }
            
            return departments;
        }

        private List<dynamic> LoadDepartmentsForDropdown()
        {
            var departmentsList = new List<dynamic>();
            
            if (_dataService.Departments != null && _dataService.Departments.Rows.Count > 0)
            {
                departmentsList = _dataService.Departments.AsEnumerable()
                    .Select(d => new { 
                        Value = d.Field<int>("DepartmentID"), 
                        Text = d.Field<string>("DepartmentName") ?? "Unknown Department" 
                    })
                    .OrderBy(d => d.Text)
                    .ToList<dynamic>();
            }
            else
            {
                // Fallback: Load departments directly from database
                try
                {
                    var fallbackDepts = GetDepartmentsFromDatabase();
                    departmentsList = fallbackDepts.Select(d => new { Value = d.Value, Text = d.Text }).ToList<dynamic>();
                }
                catch (Exception)
                {
                    // Last resort: Add some default departments
                    departmentsList = new List<dynamic>
                    {
                        new { Value = 1, Text = "Human Resources" },
                        new { Value = 2, Text = "Information Technology" },
                        new { Value = 3, Text = "Finance & Accounts" }
                    };
                }
            }
            
            return departmentsList;
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

                    // Reload departments for dropdown - with better error handling
                    var departmentsList = new List<dynamic>();
                    
                    if (_dataService.Departments != null && _dataService.Departments.Rows.Count > 0)
                    {
                        departmentsList = _dataService.Departments.AsEnumerable()
                            .Select(d => new { 
                                Value = d.Field<int>("DepartmentID"), 
                                Text = d.Field<string>("DepartmentName") ?? "Unknown Department" 
                            })
                            .OrderBy(d => d.Text)
                            .ToList<dynamic>();
                    }
                    else
                    {
                        // Fallback: Load departments directly from database
                        try
                        {
                            var fallbackDepts = GetDepartmentsFromDatabase();
                            departmentsList = fallbackDepts.Select(d => new { Value = d.Value, Text = d.Text }).ToList<dynamic>();
                        }
                        catch (Exception)
                        {
                            // Last resort: Add some default departments
                            departmentsList = new List<dynamic>
                            {
                                new { Value = 1, Text = "Human Resources" },
                                new { Value = 2, Text = "Information Technology" },
                                new { Value = 3, Text = "Finance & Accounts" }
                            };
                        }
                    }

                    ViewBag.Departments = departmentsList;
                    return View(model);
                }

                if (model.StaffID == 0)
                {
                    // Add new staff using stored procedure
                    if (_dataService.InsertStaff(model.DepartmentID, model.StaffName.Trim(), model.MobileNo.Trim(), 
                                               model.EmailAddress.Trim().ToLower(), model.Remarks?.Trim() ?? ""))
                    {
                        TempData["SuccessMessage"] = "Staff member added successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to save staff member to database.";
                        
                        // Reload departments for dropdown with error handling
                        ViewBag.Departments = LoadDepartmentsForDropdown();
                        return View(model);
                    }
                }
                else
                {
                    // Update existing staff using stored procedure
                    if (_dataService.UpdateStaff(model.StaffID, model.DepartmentID, model.StaffName.Trim(), 
                                               model.MobileNo.Trim(), model.EmailAddress.Trim().ToLower(), model.Remarks?.Trim() ?? ""))
                    {
                        TempData["SuccessMessage"] = "Staff member updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update staff member in database.";
                        
                        // Reload departments for dropdown with error handling
                        ViewBag.Departments = LoadDepartmentsForDropdown();
                        return View(model);
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

                // Delete using stored procedure
                if (_dataService.DeleteStaff(id))
                {
                    return Json(new { success = true, message = "Staff member deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete staff member." });
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
