using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Services;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingMembersController : Controller
    {
        private readonly DataService _dataService;

        public MeetingMembersController()
        {
            _dataService = DataService.Instance;
        }

        public IActionResult MeetingMemberList(string search = "", int meetingFilter = 0, int departmentFilter = 0, string attendanceFilter = "", int page = 1, int pageSize = 10)
        {
            var meetingMembers = _dataService.MeetingMembers.Clone();
            
            // Add columns for navigation properties if they don't exist
            if (!meetingMembers.Columns.Contains("StaffName"))
                meetingMembers.Columns.Add("StaffName", typeof(string));
            if (!meetingMembers.Columns.Contains("DepartmentName"))
                meetingMembers.Columns.Add("DepartmentName", typeof(string));
            if (!meetingMembers.Columns.Contains("MeetingDescription"))
                meetingMembers.Columns.Add("MeetingDescription", typeof(string));
            if (!meetingMembers.Columns.Contains("MeetingDate"))
                meetingMembers.Columns.Add("MeetingDate", typeof(DateTime));
            if (!meetingMembers.Columns.Contains("MeetingTypeName"))
                meetingMembers.Columns.Add("MeetingTypeName", typeof(string));
            if (!meetingMembers.Columns.Contains("VenueName"))
                meetingMembers.Columns.Add("VenueName", typeof(string));

            // Clear existing rows and rebuild with navigation properties
            meetingMembers.Rows.Clear();
            
            foreach (DataRow row in _dataService.MeetingMembers.Rows)
            {
                var newRow = meetingMembers.NewRow();
                newRow.ItemArray = row.ItemArray;
                
                // Get staff information
                var staff = _dataService.Staff.AsEnumerable()
                    .FirstOrDefault(s => s.Field<int>("StaffID") == row.Field<int>("StaffID"));
                newRow["StaffName"] = staff?["StaffName"]?.ToString() ?? "";
                
                // Get department information
                if (staff != null)
                {
                    var dept = _dataService.Departments.AsEnumerable()
                        .FirstOrDefault(d => d.Field<int>("DepartmentID") == staff.Field<int>("DepartmentID"));
                    newRow["DepartmentName"] = dept?["DepartmentName"]?.ToString() ?? "";
                }
                
                // Get meeting information
                var meeting = _dataService.Meetings.AsEnumerable()
                    .FirstOrDefault(m => m.Field<int>("MeetingID") == row.Field<int>("MeetingID"));
                if (meeting != null)
                {
                    newRow["MeetingDescription"] = meeting["MeetingDescription"]?.ToString() ?? "";
                    newRow["MeetingDate"] = meeting.Field<DateTime>("MeetingDate");
                    
                    // Get meeting type
                    var meetingType = _dataService.MeetingTypes.AsEnumerable()
                        .FirstOrDefault(t => t.Field<int>("MeetingTypeID") == meeting.Field<int>("MeetingTypeID"));
                    newRow["MeetingTypeName"] = meetingType?["MeetingTypeName"]?.ToString() ?? "";
                    
                    // Get venue
                    var venue = _dataService.MeetingVenues.AsEnumerable()
                        .FirstOrDefault(v => v.Field<int>("MeetingVenueID") == meeting.Field<int>("MeetingVenueID"));
                    newRow["VenueName"] = venue?["MeetingVenueName"]?.ToString() ?? "";
                }
                
                meetingMembers.Rows.Add(newRow);
            }

            // Apply filters
            var filteredRows = meetingMembers.AsEnumerable().Where(row => true);

            if (!string.IsNullOrEmpty(search))
            {
                filteredRows = filteredRows.Where(row =>
                    row.Field<string>("StaffName")?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                    row.Field<string>("DepartmentName")?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                    row.Field<string>("MeetingDescription")?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                    row.Field<string>("MeetingTypeName")?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                    row.Field<string>("VenueName")?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                    row.Field<string>("Remarks")?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);
            }

            if (meetingFilter > 0)
                filteredRows = filteredRows.Where(row => row.Field<int>("MeetingID") == meetingFilter);

            if (departmentFilter > 0)
            {
                filteredRows = filteredRows.Where(row =>
                {
                    var staff = _dataService.Staff.AsEnumerable()
                        .FirstOrDefault(s => s.Field<int>("StaffID") == row.Field<int>("StaffID"));
                    return staff != null && staff.Field<int>("DepartmentID") == departmentFilter;
                });
            }

            if (!string.IsNullOrEmpty(attendanceFilter))
            {
                bool isPresent = attendanceFilter == "Present";
                filteredRows = filteredRows.Where(row => row.Field<bool>("IsPresent") == isPresent);
            }

            // Order by meeting date descending, then by staff name
            filteredRows = filteredRows.OrderByDescending(row => row.Field<DateTime>("MeetingDate"))
                                     .ThenBy(row => row.Field<string>("StaffName"));

            // Pagination
            var totalRecords = filteredRows.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var pagedRows = filteredRows.Skip((page - 1) * pageSize).Take(pageSize);

            var filteredTable = meetingMembers.Clone();
            foreach (var row in pagedRows)
            {
                filteredTable.ImportRow(row);
            }

            // Pass filter data and pagination info to view
            ViewBag.Meetings = _dataService.Meetings;
            ViewBag.Departments = _dataService.Departments;
            ViewBag.Staff = _dataService.Staff;
            
            // Prepare staff data for JavaScript
            var staffForJs = _dataService.Staff.AsEnumerable().Select(s => new {
                StaffID = s.Field<int>("StaffID"),
                StaffName = s.Field<string>("StaffName"),
                DepartmentID = s.Field<int>("DepartmentID"),
                DepartmentName = _dataService.Departments.AsEnumerable()
                    .FirstOrDefault(d => d.Field<int>("DepartmentID") == s.Field<int>("DepartmentID"))
                    ?.Field<string>("DepartmentName") ?? "Unknown"
            }).ToList();
            ViewBag.StaffForJs = staffForJs;
            
            ViewBag.Search = search;
            ViewBag.MeetingFilter = meetingFilter;
            ViewBag.DepartmentFilter = departmentFilter;
            ViewBag.AttendanceFilter = attendanceFilter;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;

            return View(filteredTable);
        }

        public IActionResult MeetingMemberAddEdit(int? id)
        {
            MeetingMembersModel model = new MeetingMembersModel();

            if (id.HasValue && id > 0)
            {
                var row = _dataService.MeetingMembers.AsEnumerable()
                    .FirstOrDefault(x => x.Field<int>("MeetingMemberID") == id);
                
                if (row != null)
                {
                    model.MeetingMemberID = row.Field<int>("MeetingMemberID");
                    model.MeetingID = row.Field<int>("MeetingID");
                    model.StaffID = row.Field<int>("StaffID");
                    model.IsPresent = row.Field<bool>("IsPresent");
                    model.Remarks = row.Field<string>("Remarks") ?? "";
                    model.Created = row.Field<DateTime>("Created");
                    model.Modified = row.Field<DateTime>("Modified");
                }
            }

            // Pass dropdown data to view
            ViewBag.Meetings = _dataService.Meetings.AsEnumerable()
                .Where(m => !m.Field<bool>("IsCancelled") && m.Field<DateTime>("MeetingDate") >= DateTime.Now.AddDays(-30))
                .OrderBy(m => m.Field<DateTime>("MeetingDate"));
            
            // Create staff data with department names
            var staffWithDept = _dataService.Staff.Clone();
            if (!staffWithDept.Columns.Contains("DepartmentName"))
                staffWithDept.Columns.Add("DepartmentName", typeof(string));
            
            staffWithDept.Rows.Clear();
            foreach (DataRow staff in _dataService.Staff.Rows)
            {
                var newRow = staffWithDept.NewRow();
                newRow.ItemArray = staff.ItemArray;
                
                var dept = _dataService.Departments.AsEnumerable()
                    .FirstOrDefault(d => d.Field<int>("DepartmentID") == staff.Field<int>("DepartmentID"));
                newRow["DepartmentName"] = dept?["DepartmentName"]?.ToString() ?? "Unknown Dept";
                
                staffWithDept.Rows.Add(newRow);
            }
            
            ViewBag.Staff = staffWithDept;
            ViewBag.Departments = _dataService.Departments;

            return View(model);
        }

        [HttpPost]
        public IActionResult MeetingMemberAddEdit(MeetingMembersModel model)
        {
            // Custom validation
            if (model.MeetingID > 0 && model.StaffID > 0)
            {
                // Check for duplicate assignment
                var existingMember = _dataService.MeetingMembers.AsEnumerable()
                    .FirstOrDefault(m => m.Field<int>("MeetingID") == model.MeetingID &&
                                        m.Field<int>("StaffID") == model.StaffID &&
                                        m.Field<int>("MeetingMemberID") != model.MeetingMemberID);

                if (existingMember != null)
                {
                    ModelState.AddModelError("", "This staff member is already assigned to the selected meeting.");
                }

                // Check if meeting is cancelled
                var meeting = _dataService.Meetings.AsEnumerable()
                    .FirstOrDefault(m => m.Field<int>("MeetingID") == model.MeetingID);
                if (meeting != null && meeting.Field<bool>("IsCancelled"))
                {
                    ModelState.AddModelError("MeetingID", "Cannot assign members to a cancelled meeting.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Meetings = _dataService.Meetings.AsEnumerable()
                    .Where(m => !m.Field<bool>("IsCancelled") && m.Field<DateTime>("MeetingDate") >= DateTime.Now.AddDays(-30))
                    .OrderBy(m => m.Field<DateTime>("MeetingDate"));
                
                // Create staff data with department names
                var staffWithDept = _dataService.Staff.Clone();
                if (!staffWithDept.Columns.Contains("DepartmentName"))
                    staffWithDept.Columns.Add("DepartmentName", typeof(string));
                
                staffWithDept.Rows.Clear();
                foreach (DataRow staff in _dataService.Staff.Rows)
                {
                    var newRow = staffWithDept.NewRow();
                    newRow.ItemArray = staff.ItemArray;
                    
                    var dept = _dataService.Departments.AsEnumerable()
                        .FirstOrDefault(d => d.Field<int>("DepartmentID") == staff.Field<int>("DepartmentID"));
                    newRow["DepartmentName"] = dept?["DepartmentName"]?.ToString() ?? "Unknown Dept";
                    
                    staffWithDept.Rows.Add(newRow);
                }
                
                ViewBag.Staff = staffWithDept;
                ViewBag.Departments = _dataService.Departments;
                return View(model);
            }

            try
            {
                if (model.MeetingMemberID == 0)
                {
                    // Add new meeting member
                    int newId = _dataService.GetNextId(_dataService.MeetingMembers, "MeetingMemberID");
                    _dataService.MeetingMembers.Rows.Add(
                        newId,
                        model.MeetingID,
                        model.StaffID,
                        model.IsPresent,
                        model.Remarks ?? "",
                        DateTime.Now,
                        DateTime.Now
                    );
                    
                    TempData["SuccessMessage"] = "Meeting member assigned successfully!";
                }
                else
                {
                    // Update existing meeting member
                    var row = _dataService.MeetingMembers.AsEnumerable()
                        .FirstOrDefault(x => x.Field<int>("MeetingMemberID") == model.MeetingMemberID);
                    
                    if (row != null)
                    {
                        row["MeetingID"] = model.MeetingID;
                        row["StaffID"] = model.StaffID;
                        row["IsPresent"] = model.IsPresent;
                        row["Remarks"] = model.Remarks ?? "";
                        row["Modified"] = DateTime.Now;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Meeting member not found.";
                        return RedirectToAction("MeetingMemberList");
                    }
                    
                    TempData["SuccessMessage"] = "Meeting member updated successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                ViewBag.Meetings = _dataService.Meetings.AsEnumerable()
                    .Where(m => !m.Field<bool>("IsCancelled") && m.Field<DateTime>("MeetingDate") >= DateTime.Now.AddDays(-30))
                    .OrderBy(m => m.Field<DateTime>("MeetingDate"));
                
                // Create staff data with department names
                var staffWithDept = _dataService.Staff.Clone();
                if (!staffWithDept.Columns.Contains("DepartmentName"))
                    staffWithDept.Columns.Add("DepartmentName", typeof(string));
                
                staffWithDept.Rows.Clear();
                foreach (DataRow staff in _dataService.Staff.Rows)
                {
                    var newRow = staffWithDept.NewRow();
                    newRow.ItemArray = staff.ItemArray;
                    
                    var dept = _dataService.Departments.AsEnumerable()
                        .FirstOrDefault(d => d.Field<int>("DepartmentID") == staff.Field<int>("DepartmentID"));
                    newRow["DepartmentName"] = dept?["DepartmentName"]?.ToString() ?? "Unknown Dept";
                    
                    staffWithDept.Rows.Add(newRow);
                }
                
                ViewBag.Staff = staffWithDept;
                ViewBag.Departments = _dataService.Departments;
                return View(model);
            }

            return RedirectToAction("MeetingMemberList");
        }

        public IActionResult DeleteMeetingMember(int id)
        {
            try
            {
                var row = _dataService.MeetingMembers.AsEnumerable()
                    .FirstOrDefault(x => x.Field<int>("MeetingMemberID") == id);
                
                if (row != null)
                {
                    _dataService.MeetingMembers.Rows.Remove(row);
                    TempData["SuccessMessage"] = "Meeting member removed successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Meeting member not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("MeetingMemberList");
        }

        [HttpPost]
        public IActionResult BulkAssignMembers(int meetingId, int[] staffIds)
        {
            try
            {
                if (staffIds == null || staffIds.Length == 0)
                {
                    TempData["ErrorMessage"] = "Please select at least one staff member.";
                    return RedirectToAction("MeetingMemberList");
                }

                // Check if meeting exists and is not cancelled
                var meeting = _dataService.Meetings.AsEnumerable()
                    .FirstOrDefault(m => m.Field<int>("MeetingID") == meetingId);
                if (meeting == null)
                {
                    TempData["ErrorMessage"] = "Meeting not found.";
                    return RedirectToAction("MeetingMemberList");
                }
                if (meeting.Field<bool>("IsCancelled"))
                {
                    TempData["ErrorMessage"] = "Cannot assign members to a cancelled meeting.";
                    return RedirectToAction("MeetingMemberList");
                }

                int assignedCount = 0;
                int skippedCount = 0;

                foreach (int staffId in staffIds)
                {
                    // Check if already assigned
                    var existingMember = _dataService.MeetingMembers.AsEnumerable()
                        .Any(m => m.Field<int>("MeetingID") == meetingId && m.Field<int>("StaffID") == staffId);

                    if (!existingMember)
                    {
                        int newId = _dataService.GetNextId(_dataService.MeetingMembers, "MeetingMemberID");
                        _dataService.MeetingMembers.Rows.Add(
                            newId,
                            meetingId,
                            staffId,
                            false, // Default to absent
                            "",
                            DateTime.Now,
                            DateTime.Now
                        );
                        assignedCount++;
                    }
                    else
                    {
                        skippedCount++;
                    }
                }

                string message = $"Successfully assigned {assignedCount} members.";
                if (skippedCount > 0)
                    message += $" {skippedCount} members were already assigned.";

                TempData["SuccessMessage"] = message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("MeetingMemberList");
        }

        [HttpGet]
        public IActionResult GetStaffByDepartment(int departmentId)
        {
            var staff = _dataService.Staff.AsEnumerable()
                .Where(s => s.Field<int>("DepartmentID") == departmentId)
                .Select(s => new
                {
                    StaffID = s.Field<int>("StaffID"),
                    StaffName = s.Field<string>("StaffName")
                })
                .OrderBy(s => s.StaffName);

            return Json(staff);
        }

        [HttpGet]
        public IActionResult GetMeetingDetails(int meetingId)
        {
            var meeting = _dataService.Meetings.AsEnumerable()
                .FirstOrDefault(m => m.Field<int>("MeetingID") == meetingId);

            if (meeting == null)
                return Json(null);

            var department = _dataService.Departments.AsEnumerable()
                .FirstOrDefault(d => d.Field<int>("DepartmentID") == meeting.Field<int>("DepartmentID"));

            var meetingType = _dataService.MeetingTypes.AsEnumerable()
                .FirstOrDefault(t => t.Field<int>("MeetingTypeID") == meeting.Field<int>("MeetingTypeID"));

            var venue = _dataService.MeetingVenues.AsEnumerable()
                .FirstOrDefault(v => v.Field<int>("MeetingVenueID") == meeting.Field<int>("MeetingVenueID"));

            return Json(new
            {
                MeetingID = meeting.Field<int>("MeetingID"),
                MeetingDate = meeting.Field<DateTime>("MeetingDate").ToString("yyyy-MM-dd HH:mm"),
                MeetingDescription = meeting.Field<string>("MeetingDescription"),
                DepartmentName = department?.Field<string>("DepartmentName"),
                MeetingTypeName = meetingType?.Field<string>("MeetingTypeName"),
                VenueName = venue?.Field<string>("MeetingVenueName"),
                IsCancelled = meeting.Field<bool>("IsCancelled")
            });
        }
    }
}
