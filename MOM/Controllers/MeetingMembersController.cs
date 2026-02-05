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
                filteredRows = filteredRows.Where(row => 
                {
                    var isPresentValue = row["IsPresent"];
                    if (isPresentValue == DBNull.Value) return false;
                    return Convert.ToBoolean(isPresentValue) == isPresent;
                });
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
            
            // Prepare data for JavaScript serialization
            var meetingsForJs = _dataService.Meetings.AsEnumerable()
                .Where(m => 
                {
                    var isCancelledValue = m["IsCancelled"];
                    if (isCancelledValue == DBNull.Value) return true; // Include if NULL
                    return !Convert.ToBoolean(isCancelledValue); // Only active meetings
                })
                .Select(m => new {
                    MeetingID = m.Field<int>("MeetingID"),
                    MeetingDescription = m.Field<string>("MeetingDescription"),
                    MeetingDate = m.Field<DateTime>("MeetingDate")
                }).OrderByDescending(m => m.MeetingDate).ToList();
            ViewBag.MeetingsForJs = meetingsForJs;
            
            var departmentsForJs = _dataService.Departments.AsEnumerable().Select(d => new {
                DepartmentID = d.Field<int>("DepartmentID"),
                DepartmentName = d.Field<string>("DepartmentName")
            }).OrderBy(d => d.DepartmentName).ToList();
            ViewBag.DepartmentsForJs = departmentsForJs;
            
            // Prepare staff data for JavaScript
            var staffForJs = _dataService.Staff.AsEnumerable().Select(s => new {
                StaffID = s.Field<int>("StaffID"),
                StaffName = s.Field<string>("StaffName"),
                DepartmentID = s.Field<int>("DepartmentID"),
                DepartmentName = _dataService.Departments.AsEnumerable()
                    .FirstOrDefault(d => d.Field<int>("DepartmentID") == s.Field<int>("DepartmentID"))
                    ?.Field<string>("DepartmentName") ?? "Unknown"
            }).OrderBy(s => s.StaffName).ToList();
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
                    
                    // Safe boolean conversion
                    var isPresentValue = row["IsPresent"];
                    model.IsPresent = isPresentValue == DBNull.Value ? false : Convert.ToBoolean(isPresentValue);
                    
                    model.Remarks = row.Field<string>("Remarks") ?? "";
                    model.Created = row.Field<DateTime>("Created");
                    model.Modified = row.Field<DateTime>("Modified");
                }
            }

            // Pass dropdown data to view
            ViewBag.Meetings = _dataService.Meetings.AsEnumerable()
                .Where(m => 
                {
                    var isCancelledValue = m["IsCancelled"];
                    if (isCancelledValue == DBNull.Value) return true; // Include if NULL
                    return !Convert.ToBoolean(isCancelledValue) && m.Field<DateTime>("MeetingDate") >= DateTime.Now.AddDays(-30);
                })
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
                if (meeting != null)
                {
                    var isCancelledValue = meeting["IsCancelled"];
                    bool isCancelled = isCancelledValue != DBNull.Value && Convert.ToBoolean(isCancelledValue);
                    if (isCancelled)
                    {
                        ModelState.AddModelError("MeetingID", "Cannot assign members to a cancelled meeting.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Meetings = _dataService.Meetings.AsEnumerable()
                    .Where(m => 
                    {
                        var isCancelledValue = m["IsCancelled"];
                        if (isCancelledValue == DBNull.Value) return true; // Include if NULL
                        return !Convert.ToBoolean(isCancelledValue) && m.Field<DateTime>("MeetingDate") >= DateTime.Now.AddDays(-30);
                    })
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
                    // Add new meeting member using stored procedure
                    if (_dataService.InsertMeetingMember(model.MeetingID, model.StaffID, model.IsPresent, model.Remarks ?? ""))
                    {
                        TempData["SuccessMessage"] = "Meeting member assigned successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to assign meeting member.";
                        return View(model);
                    }
                }
                else
                {
                    // Update existing meeting member using stored procedure
                    if (_dataService.UpdateMeetingMember(model.MeetingMemberID, model.MeetingID, model.StaffID, model.IsPresent, model.Remarks ?? ""))
                    {
                        TempData["SuccessMessage"] = "Meeting member updated successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Failed to update meeting member.";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                ViewBag.Meetings = _dataService.Meetings.AsEnumerable()
                    .Where(m => 
                    {
                        var isCancelledValue = m["IsCancelled"];
                        if (isCancelledValue == DBNull.Value) return true; // Include if NULL
                        return !Convert.ToBoolean(isCancelledValue) && m.Field<DateTime>("MeetingDate") >= DateTime.Now.AddDays(-30);
                    })
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
                if (_dataService.DeleteMeetingMember(id))
                {
                    TempData["SuccessMessage"] = "Meeting member removed successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete meeting member.";
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
                    return Json(new { success = false, message = "Please select at least one staff member." });
                }

                // Check if meeting exists and is not cancelled using new DataService method
                var meetingData = _dataService.GetMeetingById(meetingId);
                if (meetingData.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Meeting not found." });
                }
                
                var meetingRow = meetingData.Rows[0];
                var isCancelledValue = meetingRow["IsCancelled"];
                bool isCancelled = isCancelledValue != DBNull.Value && Convert.ToBoolean(isCancelledValue);
                if (isCancelled)
                {
                    return Json(new { success = false, message = "Cannot assign members to a cancelled meeting." });
                }

                // Use the new DataService bulk assign method
                var staffIdsList = staffIds.ToList();
                bool success = _dataService.BulkAssignStaffToMeeting(meetingId, staffIdsList, "Bulk assigned");
                
                if (success)
                {
                    return Json(new { success = true, message = $"Successfully assigned {staffIds.Length} members to the meeting." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to assign some members. Please check for duplicates." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                if (_dataService.DeleteMeetingMember(id))
                {
                    return Json(new { success = true, message = "Meeting member removed successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to delete meeting member." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult GetMeetingMemberDetails(int id)
        {
            try
            {
                var row = _dataService.MeetingMembers.AsEnumerable()
                    .FirstOrDefault(r => r.Field<int>("MeetingMemberID") == id);

                if (row == null)
                {
                    return Json(new { success = false, message = "Meeting member not found." });
                }

                // Get staff information
                var staff = _dataService.Staff.AsEnumerable()
                    .FirstOrDefault(s => s.Field<int>("StaffID") == row.Field<int>("StaffID"));

                // Get department information
                var dept = staff != null ? _dataService.Departments.AsEnumerable()
                    .FirstOrDefault(d => d.Field<int>("DepartmentID") == staff.Field<int>("DepartmentID")) : null;

                // Get meeting information
                var meeting = _dataService.Meetings.AsEnumerable()
                    .FirstOrDefault(m => m.Field<int>("MeetingID") == row.Field<int>("MeetingID"));

                // Get meeting type and venue
                var meetingType = meeting != null ? _dataService.MeetingTypes.AsEnumerable()
                    .FirstOrDefault(t => t.Field<int>("MeetingTypeID") == meeting.Field<int>("MeetingTypeID")) : null;

                var venue = meeting != null ? _dataService.MeetingVenues.AsEnumerable()
                    .FirstOrDefault(v => v.Field<int>("MeetingVenueID") == meeting.Field<int>("MeetingVenueID")) : null;

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        MeetingMemberID = row.Field<int>("MeetingMemberID"),
                        StaffName = staff?.Field<string>("StaffName") ?? "Unknown",
                        DepartmentName = dept?.Field<string>("DepartmentName") ?? "Unknown",
                        MeetingDescription = meeting?.Field<string>("MeetingDescription") ?? "Unknown",
                        MeetingDate = meeting?.Field<DateTime>("MeetingDate").ToString("dd/MM/yyyy hh:mm tt") ?? "Unknown",
                        MeetingTypeName = meetingType?.Field<string>("MeetingTypeName") ?? "Unknown",
                        VenueName = venue?.Field<string>("MeetingVenueName") ?? "Unknown",
                        IsPresent = row["IsPresent"] == DBNull.Value ? false : Convert.ToBoolean(row["IsPresent"]),
                        Remarks = row.Field<string>("Remarks") ?? "",
                        Created = row.Field<DateTime>("Created").ToString("dd/MM/yyyy hh:mm tt"),
                        Modified = row.Field<DateTime>("Modified").ToString("dd/MM/yyyy hh:mm tt")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error getting meeting member details: {ex.Message}" });
            }
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
        public IActionResult GetMeetingsForBulkAssign()
        {
            try
            {
                var meetings = _dataService.Meetings.AsEnumerable()
                    .Where(m => 
                    {
                        var isCancelledValue = m["IsCancelled"];
                        if (isCancelledValue == DBNull.Value) return true; // Include if NULL
                        return !Convert.ToBoolean(isCancelledValue);
                    })
                    .Select(m => new {
                        MeetingID = m.Field<int>("MeetingID"),
                        MeetingDescription = m.Field<string>("MeetingDescription"),
                        MeetingDate = m.Field<DateTime>("MeetingDate")
                    })
                    .OrderByDescending(m => m.MeetingDate)
                    .ToList();

                return Json(meetings);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public IActionResult GetDepartmentsForBulkAssign()
        {
            try
            {
                var departments = _dataService.Departments.AsEnumerable()
                    .Select(d => new {
                        DepartmentID = d.Field<int>("DepartmentID"),
                        DepartmentName = d.Field<string>("DepartmentName")
                    })
                    .OrderBy(d => d.DepartmentName)
                    .ToList();

                return Json(departments);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public IActionResult GetStaffForBulkAssign()
        {
            try
            {
                var staff = _dataService.Staff.AsEnumerable()
                    .Select(s => new {
                        StaffID = s.Field<int>("StaffID"),
                        StaffName = s.Field<string>("StaffName"),
                        DepartmentID = s.Field<int>("DepartmentID"),
                        DepartmentName = _dataService.Departments.AsEnumerable()
                            .FirstOrDefault(d => d.Field<int>("DepartmentID") == s.Field<int>("DepartmentID"))
                            ?.Field<string>("DepartmentName") ?? "Unknown"
                    })
                    .OrderBy(s => s.StaffName)
                    .ToList();

                return Json(staff);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
        }

        [HttpGet]
        public IActionResult GetAssignedMembers(int meetingId)
        {
            try
            {
                var assignedStaffIds = _dataService.MeetingMembers.AsEnumerable()
                    .Where(m => m.Field<int>("MeetingID") == meetingId)
                    .Select(m => m.Field<int>("StaffID"))
                    .ToList();

                return Json(assignedStaffIds);
            }
            catch (Exception ex)
            {
                return Json(new List<int>());
            }
        }

        [HttpPost]
        public IActionResult ToggleAttendance(int id)
        {
            try
            {
                // Get the meeting member data using new DataService method
                var memberData = _dataService.GetMeetingMemberById(id);
                if (memberData.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Meeting member not found." });
                }
                
                var row = memberData.Rows[0];
                bool currentStatus = row["IsPresent"] == DBNull.Value ? false : Convert.ToBoolean(row["IsPresent"]);
                bool newStatus = !currentStatus;
                
                int meetingId = Convert.ToInt32(row["MeetingID"]);
                int staffId = Convert.ToInt32(row["StaffID"]);
                string remarks = row["Remarks"]?.ToString() ?? "";
                
                // Update using new DataService method
                bool success = _dataService.UpdateMeetingMember(id, meetingId, staffId, newStatus, remarks);
                
                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = $"Attendance marked as {(newStatus ? "Present" : "Absent")}", 
                        isPresent = newStatus 
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update attendance." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult BulkUpdateAttendance(int meetingId, int[] presentIds, int[] absentIds)
        {
            try
            {
                var attendanceUpdates = new Dictionary<int, bool>();
                
                // Add present members
                if (presentIds != null && presentIds.Length > 0)
                {
                    foreach (int memberId in presentIds)
                    {
                        attendanceUpdates[memberId] = true;
                    }
                }
                
                // Add absent members
                if (absentIds != null && absentIds.Length > 0)
                {
                    foreach (int memberId in absentIds)
                    {
                        attendanceUpdates[memberId] = false;
                    }
                }
                
                if (attendanceUpdates.Count == 0)
                {
                    return Json(new { success = false, message = "No attendance updates specified." });
                }
                
                // Use the new DataService bulk update method
                bool success = _dataService.BulkUpdateAttendance(attendanceUpdates, "Bulk attendance update");
                
                if (success)
                {
                    return Json(new { 
                        success = true, 
                        message = $"Successfully updated attendance for {attendanceUpdates.Count} member(s)" 
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update some attendance records." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult BulkMarkAllPresent(int meetingId)
        {
            try
            {
                bool success = _dataService.BulkMarkAllPresent(meetingId, "Bulk marked all present");
                
                if (success)
                {
                    return Json(new { success = true, message = "Successfully marked all members as present." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update attendance for some members." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult BulkMarkAllAbsent(int meetingId)
        {
            try
            {
                bool success = _dataService.BulkMarkAllAbsent(meetingId, "Bulk marked all absent");
                
                if (success)
                {
                    return Json(new { success = true, message = "Successfully marked all members as absent." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update attendance for some members." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult GetMeetingMembersForAttendance(int meetingId)
        {
            try
            {
                var meetingMembers = _dataService.MeetingMembers.AsEnumerable()
                    .Where(m => m.Field<int>("MeetingID") == meetingId)
                    .Select(m => {
                        // Get staff information
                        var staff = _dataService.Staff.AsEnumerable()
                            .FirstOrDefault(s => s.Field<int>("StaffID") == m.Field<int>("StaffID"));
                        
                        // Get department information
                        var dept = staff != null ? _dataService.Departments.AsEnumerable()
                            .FirstOrDefault(d => d.Field<int>("DepartmentID") == staff.Field<int>("DepartmentID")) : null;
                        
                        return new {
                            meetingMemberID = m.Field<int>("MeetingMemberID"),
                            meetingID = m.Field<int>("MeetingID"),
                            staffID = m.Field<int>("StaffID"),
                            staffName = staff?.Field<string>("StaffName") ?? "Unknown Staff",
                            departmentName = dept?.Field<string>("DepartmentName") ?? "Unknown Dept",
                            isPresent = m["IsPresent"] == DBNull.Value ? false : Convert.ToBoolean(m["IsPresent"]),
                            remarks = m.Field<string>("Remarks") ?? ""
                        };
                    })
                    .OrderBy(m => m.staffName)
                    .ToList();

                return Json(meetingMembers);
            }
            catch (Exception ex)
            {
                return Json(new List<object>());
            }
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
                IsCancelled = meeting["IsCancelled"] == DBNull.Value ? false : Convert.ToBoolean(meeting["IsCancelled"])
            });
        }
    }
}
