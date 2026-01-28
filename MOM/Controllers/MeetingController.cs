using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Services;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingController : Controller
    {
        private readonly DataService _dataService;

        public MeetingController()
        {
            _dataService = DataService.Instance;
        }

        public IActionResult MeetingList(string search = "", int departmentFilter = 0, int typeFilter = 0, int venueFilter = 0, string statusFilter = "", DateTime? dateFrom = null, DateTime? dateTo = null, int page = 1, int pageSize = 10)
        {
            var meetings = _dataService.Meetings.Clone();
            
            // Add columns for navigation properties if they don't exist
            if (!meetings.Columns.Contains("DepartmentName"))
                meetings.Columns.Add("DepartmentName", typeof(string));
            if (!meetings.Columns.Contains("MeetingTypeName"))
                meetings.Columns.Add("MeetingTypeName", typeof(string));
            if (!meetings.Columns.Contains("VenueName"))
                meetings.Columns.Add("VenueName", typeof(string));
            if (!meetings.Columns.Contains("MemberCount"))
                meetings.Columns.Add("MemberCount", typeof(int));
            if (!meetings.Columns.Contains("PresentCount"))
                meetings.Columns.Add("PresentCount", typeof(int));

            // Clear existing rows and rebuild with navigation properties
            meetings.Rows.Clear();
            
            foreach (DataRow row in _dataService.Meetings.Rows)
            {
                var newRow = meetings.NewRow();
                newRow.ItemArray = row.ItemArray;
                
                // Add department name
                var dept = _dataService.Departments.AsEnumerable()
                    .FirstOrDefault(d => d.Field<int>("DepartmentID") == row.Field<int>("DepartmentID"));
                newRow["DepartmentName"] = dept?["DepartmentName"]?.ToString() ?? "";
                
                // Add meeting type name
                var type = _dataService.MeetingTypes.AsEnumerable()
                    .FirstOrDefault(t => t.Field<int>("MeetingTypeID") == row.Field<int>("MeetingTypeID"));
                newRow["MeetingTypeName"] = type?["MeetingTypeName"]?.ToString() ?? "";
                
                // Add venue name
                var venue = _dataService.MeetingVenues.AsEnumerable()
                    .FirstOrDefault(v => v.Field<int>("MeetingVenueID") == row.Field<int>("MeetingVenueID"));
                newRow["VenueName"] = venue?["MeetingVenueName"]?.ToString() ?? "";
                
                // Add member counts
                int meetingId = row.Field<int>("MeetingID");
                var members = _dataService.MeetingMembers.AsEnumerable()
                    .Where(m => m.Field<int>("MeetingID") == meetingId);
                newRow["MemberCount"] = members.Count();
                newRow["PresentCount"] = members.Count(m => m.Field<bool>("IsPresent"));
                
                meetings.Rows.Add(newRow);
            }

            // Apply filters
            var filteredRows = meetings.AsEnumerable().Where(row => true);

            if (!string.IsNullOrEmpty(search))
            {
                filteredRows = filteredRows.Where(row =>
                    row.Field<string>("MeetingDescription")?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                    row.Field<string>("DepartmentName")?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                    row.Field<string>("MeetingTypeName")?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                    row.Field<string>("VenueName")?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);
            }

            if (departmentFilter > 0)
                filteredRows = filteredRows.Where(row => row.Field<int>("DepartmentID") == departmentFilter);

            if (typeFilter > 0)
                filteredRows = filteredRows.Where(row => row.Field<int>("MeetingTypeID") == typeFilter);

            if (venueFilter > 0)
                filteredRows = filteredRows.Where(row => row.Field<int>("MeetingVenueID") == venueFilter);

            if (dateFrom.HasValue)
                filteredRows = filteredRows.Where(row => row.Field<DateTime>("MeetingDate").Date >= dateFrom.Value.Date);

            if (dateTo.HasValue)
                filteredRows = filteredRows.Where(row => row.Field<DateTime>("MeetingDate").Date <= dateTo.Value.Date);

            if (!string.IsNullOrEmpty(statusFilter))
            {
                filteredRows = filteredRows.Where(row =>
                {
                    bool isCancelled = row.Field<bool>("IsCancelled");
                    DateTime meetingDate = row.Field<DateTime>("MeetingDate");
                    
                    return statusFilter switch
                    {
                        "Cancelled" => isCancelled,
                        "Today" => !isCancelled && meetingDate.Date == DateTime.Today,
                        "Upcoming" => !isCancelled && meetingDate > DateTime.Now,
                        "Past" => !isCancelled && meetingDate < DateTime.Now,
                        _ => true
                    };
                });
            }

            // Order by meeting date descending
            filteredRows = filteredRows.OrderByDescending(row => row.Field<DateTime>("MeetingDate"));

            // Pagination
            var totalRecords = filteredRows.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var pagedRows = filteredRows.Skip((page - 1) * pageSize).Take(pageSize);

            var filteredTable = meetings.Clone();
            foreach (var row in pagedRows)
            {
                filteredTable.ImportRow(row);
            }

            // Pass filter data and pagination info to view
            ViewBag.Departments = _dataService.Departments;
            ViewBag.MeetingTypes = _dataService.MeetingTypes;
            ViewBag.MeetingVenues = _dataService.MeetingVenues;
            ViewBag.Search = search;
            ViewBag.DepartmentFilter = departmentFilter;
            ViewBag.TypeFilter = typeFilter;
            ViewBag.VenueFilter = venueFilter;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;

            return View(filteredTable);
        }

        public IActionResult MeetingAddEdit(int? id)
        {
            MeetingModel model = new MeetingModel();

            if (id.HasValue && id > 0)
            {
                var row = _dataService.Meetings.AsEnumerable()
                    .FirstOrDefault(x => x.Field<int>("MeetingID") == id);
                
                if (row != null)
                {
                    model.MeetingID = row.Field<int>("MeetingID");
                    model.MeetingDate = row.Field<DateTime>("MeetingDate");
                    model.DepartmentID = row.Field<int>("DepartmentID");
                    model.MeetingTypeID = row.Field<int>("MeetingTypeID");
                    model.MeetingVenueID = row.Field<int>("MeetingVenueID");
                    model.MeetingDescription = row.Field<string>("MeetingDescription") ?? "";
                    model.DocumentPath = row.Field<string>("DocumentPath") ?? "";
                    model.IsCancelled = row.Field<bool>("IsCancelled");
                    model.CancellationDateTime = row.Field<DateTime?>("CancellationDateTime");
                    model.CancellationReason = row.Field<string>("CancellationReason") ?? "";
                    model.Created = row.Field<DateTime>("Created");
                    model.Modified = row.Field<DateTime>("Modified");
                }
            }

            // Pass dropdown data to view
            ViewBag.Departments = _dataService.Departments;
            ViewBag.MeetingTypes = _dataService.MeetingTypes;
            ViewBag.MeetingVenues = _dataService.MeetingVenues;

            return View(model);
        }

        [HttpPost]
        public IActionResult MeetingAddEdit(MeetingModel model, IFormFile? upload)
        {
            // Custom validation
            if (model.MeetingDate <= DateTime.Now && model.MeetingID == 0)
            {
                ModelState.AddModelError("MeetingDate", "Meeting date must be in the future for new meetings.");
            }

            if (model.IsCancelled && string.IsNullOrWhiteSpace(model.CancellationReason))
            {
                ModelState.AddModelError("CancellationReason", "Cancellation reason is required when meeting is cancelled.");
            }

            // Check for duplicate meetings
            if (model.MeetingID == 0)
            {
                var existingMeeting = _dataService.Meetings.AsEnumerable()
                    .Any(m => m.Field<int>("MeetingVenueID") == model.MeetingVenueID &&
                             m.Field<DateTime>("MeetingDate").Date == model.MeetingDate.Date &&
                             Math.Abs((m.Field<DateTime>("MeetingDate") - model.MeetingDate).TotalHours) < 2 &&
                             !m.Field<bool>("IsCancelled"));

                if (existingMeeting)
                {
                    ModelState.AddModelError("", "A meeting is already scheduled at this venue within 2 hours of the selected time.");
                }
            }

            // Handle file upload
            if (upload != null && upload.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".doc", ".xls" };
                var extension = Path.GetExtension(upload.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("upload", "Only PDF, DOC, DOCX, XLS, and XLSX files are allowed.");
                }
                else if (upload.Length > 10 * 1024 * 1024) // 10MB limit
                {
                    ModelState.AddModelError("upload", "File size cannot exceed 10MB.");
                }
                else
                {
                    try
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "meetings");
                        Directory.CreateDirectory(uploadsFolder);
                        
                        var fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N")[..8]}{extension}";
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            upload.CopyTo(stream);
                        }
                        
                        model.DocumentPath = $"/uploads/meetings/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("upload", $"File upload failed: {ex.Message}");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _dataService.Departments;
                ViewBag.MeetingTypes = _dataService.MeetingTypes;
                ViewBag.MeetingVenues = _dataService.MeetingVenues;
                return View(model);
            }

            try
            {
                if (model.MeetingID == 0)
                {
                    // Add new meeting
                    int newId = _dataService.GetNextId(_dataService.Meetings, "MeetingID");
                    _dataService.Meetings.Rows.Add(
                        newId,
                        model.MeetingDate,
                        model.MeetingVenueID,
                        model.MeetingTypeID,
                        model.DepartmentID,
                        model.MeetingDescription ?? "",
                        model.DocumentPath ?? "",
                        DateTime.Now,
                        DateTime.Now,
                        model.IsCancelled,
                        model.IsCancelled ? (model.CancellationDateTime ?? DateTime.Now) : (object)DBNull.Value,
                        model.IsCancelled ? (model.CancellationReason ?? "") : ""
                    );
                    
                    TempData["SuccessMessage"] = "Meeting scheduled successfully!";
                }
                else
                {
                    // Update existing meeting
                    var row = _dataService.Meetings.AsEnumerable()
                        .FirstOrDefault(x => x.Field<int>("MeetingID") == model.MeetingID);
                    
                    if (row != null)
                    {
                        row["MeetingDate"] = model.MeetingDate;
                        row["MeetingVenueID"] = model.MeetingVenueID;
                        row["MeetingTypeID"] = model.MeetingTypeID;
                        row["DepartmentID"] = model.DepartmentID;
                        row["MeetingDescription"] = model.MeetingDescription ?? "";
                        if (!string.IsNullOrEmpty(model.DocumentPath))
                            row["DocumentPath"] = model.DocumentPath;
                        row["Modified"] = DateTime.Now;
                        row["IsCancelled"] = model.IsCancelled;
                        row["CancellationDateTime"] = model.IsCancelled ? 
                            (model.CancellationDateTime ?? DateTime.Now) : (object)DBNull.Value;
                        row["CancellationReason"] = model.IsCancelled ? (model.CancellationReason ?? "") : "";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Meeting not found.";
                        return RedirectToAction("MeetingList");
                    }
                    
                    TempData["SuccessMessage"] = "Meeting updated successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                ViewBag.Departments = _dataService.Departments;
                ViewBag.MeetingTypes = _dataService.MeetingTypes;
                ViewBag.MeetingVenues = _dataService.MeetingVenues;
                return View(model);
            }

            return RedirectToAction("MeetingList");
        }

        public IActionResult DeleteMeeting(int id)
        {
            var row = _dataService.Meetings.AsEnumerable()
                .FirstOrDefault(x => x.Field<int>("MeetingID") == id);
            
            if (row != null)
            {
                // Check if meeting has members
                var hasMembers = _dataService.MeetingMembers.AsEnumerable()
                    .Any(m => m.Field<int>("MeetingID") == id);
                
                if (hasMembers)
                {
                    TempData["ErrorMessage"] = "Cannot delete meeting with assigned members. Please remove members first.";
                }
                else
                {
                    _dataService.Meetings.Rows.Remove(row);
                    TempData["SuccessMessage"] = "Meeting deleted successfully!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Meeting not found.";
            }

            return RedirectToAction("MeetingList");
        }

        [HttpPost]
        public IActionResult CancelMeeting(int id, string reason)
        {
            var row = _dataService.Meetings.AsEnumerable()
                .FirstOrDefault(x => x.Field<int>("MeetingID") == id);
            
            if (row != null)
            {
                row["IsCancelled"] = true;
                row["CancellationDateTime"] = DateTime.Now;
                row["CancellationReason"] = reason ?? "";
                row["Modified"] = DateTime.Now;
                
                TempData["SuccessMessage"] = "Meeting cancelled successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Meeting not found.";
            }

            return RedirectToAction("MeetingList");
        }
    }
}
