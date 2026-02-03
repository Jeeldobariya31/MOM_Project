# Controllers Documentation - MOM System

## Table of Contents
1. [Controller Architecture](#controller-architecture)
2. [HomeController](#homecontroller)
3. [MeetingController](#meetingcontroller)
4. [StaffController](#staffcontroller)
5. [DepartmentController](#departmentcontroller)
6. [MeetingTypeController](#meetingtypecontroller)
7. [MeetingVenueController](#meetingvenuecontroller)
8. [MeetingMembersController](#meetingmemberscontroller)
9. [AuthController](#authcontroller)
10. [AccountController](#accountcontroller)
11. [Common Patterns](#common-patterns)

## Controller Architecture

### MVC Pattern Implementation
Controllers in the MOM system follow the standard ASP.NET Core MVC pattern:

```csharp
[Controller] → [Action Method] → [Business Logic] → [DataService] → [View/JSON Response]
```

### Base Controller Structure
```csharp
public class BaseController : Controller
{
    protected readonly DataService _dataService;
    
    public BaseController()
    {
        _dataService = DataService.Instance; // Singleton pattern
    }
}
```

### Common Controller Features
- **Dependency Injection**: DataService singleton injection
- **Action Filters**: Validation and error handling
- **HTTP Methods**: GET, POST, PUT, DELETE support
- **JSON Responses**: AJAX-friendly API endpoints
- **Model Validation**: Server-side validation with ModelState
- **TempData**: Success/error message passing

## HomeController

### Purpose
Manages the main dashboard and static pages of the application.

### Key Actions

#### 1. Index (Dashboard)
```csharp
public IActionResult Index()
{
    // Comprehensive dashboard data preparation
    var dashboardData = new
    {
        // Meeting Statistics
        TotalMeetings = _dataService.Meetings.Rows.Count,
        UpcomingMeetings = upcomingMeetings.Count(),
        TodayMeetings = todayMeetings.Count(),
        CancelledMeetings = cancelledMeetings.Count(),
        
        // Department Statistics
        TotalDepartments = _dataService.Departments.Rows.Count,
        ActiveDepartments = activeDepartments.Count(),
        
        // Staff Statistics
        TotalStaff = _dataService.Staff.Rows.Count,
        ActiveStaff = activeStaff.Count(),
        
        // Chart Data
        MeetingsByDepartment = departmentMeetings,
        MeetingsByMonth = monthlyMeetings,
        AttendanceRates = attendanceData,
        MeetingsByType = typeMeetings,
        
        // Recent Activities
        RecentMeetings = recentMeetings.Take(5),
        UpcomingMeetingsData = upcomingMeetings.Take(5)
    };
    
    return View(dashboardData);
}
```

**Features:**
- Real-time statistics calculation
- Interactive charts data preparation
- Recent activities tracking
- Performance metrics

#### 2. Static Pages
```csharp
public IActionResult About() => View();
public IActionResult Privacy() => View();
public IActionResult Contact() => View();
public IActionResult Terms() => View();
```

**Content:**
- Company information and mission
- Privacy policy and data handling
- Contact information and support
- Terms of service and usage

## MeetingController

### Purpose
Core controller managing all meeting-related operations including CRUD operations, file uploads, and meeting status management.

### Key Actions

#### 1. MeetingList (GET)
```csharp
public IActionResult MeetingList(
    string search = "", 
    int departmentFilter = 0, 
    int typeFilter = 0, 
    int venueFilter = 0, 
    string statusFilter = "", 
    DateTime? dateFrom = null, 
    DateTime? dateTo = null, 
    int page = 1, 
    int pageSize = 10)
{
    // Complex filtering and pagination logic
    var meetings = _dataService.Meetings.Clone();
    
    // Add navigation properties
    foreach (DataRow row in _dataService.Meetings.Rows)
    {
        // Join with related tables
        var dept = _dataService.Departments.AsEnumerable()
            .FirstOrDefault(d => d.Field<int>("DepartmentID") == row.Field<int>("DepartmentID"));
        newRow["DepartmentName"] = dept?["DepartmentName"]?.ToString() ?? "";
        
        // Calculate member counts
        var members = _dataService.MeetingMembers.AsEnumerable()
            .Where(m => m.Field<int>("MeetingID") == meetingId);
        newRow["MemberCount"] = members.Count();
        newRow["PresentCount"] = members.Count(m => m.Field<bool>("IsPresent"));
    }
    
    // Apply filters
    var filteredRows = meetings.AsEnumerable().Where(row => true);
    
    if (!string.IsNullOrEmpty(search))
        filteredRows = filteredRows.Where(row => /* search logic */);
    
    // Pagination
    var totalRecords = filteredRows.Count();
    var pagedRows = filteredRows.Skip((page - 1) * pageSize).Take(pageSize);
    
    return View(filteredTable);
}
```

**Features:**
- Advanced search and filtering
- Multi-column sorting
- Pagination with page size options
- Status-based filtering (Upcoming, Today, Past, Cancelled)
- Export functionality

#### 2. MeetingAddEdit (GET/POST)
```csharp
[HttpGet]
public IActionResult MeetingAddEdit(int? id)
{
    MeetingModel model = new MeetingModel();
    
    if (id.HasValue && id > 0)
    {
        // Load existing meeting data
        var row = _dataService.Meetings.AsEnumerable()
            .FirstOrDefault(x => x.Field<int>("MeetingID") == id);
        
        if (row != null)
        {
            // Map DataRow to Model
            model.MeetingID = row.Field<int>("MeetingID");
            model.MeetingDate = row.Field<DateTime>("MeetingDate");
            // ... other properties
        }
    }
    
    // Populate dropdown data
    ViewBag.Departments = _dataService.Departments;
    ViewBag.MeetingTypes = _dataService.MeetingTypes;
    ViewBag.MeetingVenues = _dataService.MeetingVenues;
    
    return View(model);
}

[HttpPost]
public IActionResult MeetingAddEdit(MeetingModel model, IFormFile? upload)
{
    // Remove validation errors for optional fields
    ModelState.Remove("DocumentPath");
    ModelState.Remove("CancellationReason");
    
    // Custom validation
    if (model.MeetingDate <= DateTime.Now && model.MeetingID == 0)
    {
        ModelState.AddModelError("MeetingDate", "Meeting date must be in the future for new meetings.");
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
            // Save file
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
    }
    
    if (!ModelState.IsValid)
    {
        // Repopulate dropdown data
        ViewBag.Departments = _dataService.Departments;
        ViewBag.MeetingTypes = _dataService.MeetingTypes;
        ViewBag.MeetingVenues = _dataService.MeetingVenues;
        return View(model);
    }
    
    // Save or update meeting
    if (model.MeetingID == 0)
    {
        // Add new meeting
        int newId = _dataService.GetNextId(_dataService.Meetings, "MeetingID");
        _dataService.Meetings.Rows.Add(/* meeting data */);
        TempData["SuccessMessage"] = "Meeting scheduled successfully!";
    }
    else
    {
        // Update existing meeting
        var row = _dataService.Meetings.AsEnumerable()
            .FirstOrDefault(x => x.Field<int>("MeetingID") == model.MeetingID);
        
        if (row != null)
        {
            // Update row data
            row["MeetingDate"] = model.MeetingDate;
            // ... other updates
            TempData["SuccessMessage"] = "Meeting updated successfully!";
        }
    }
    
    return RedirectToAction("MeetingList");
}
```

**Features:**
- Add/Edit meeting functionality
- File upload with validation
- Custom business validation
- Dropdown population
- Success/error messaging

#### 3. AJAX API Endpoints

##### Delete Meeting
```csharp
[HttpPost]
public IActionResult Delete(int id)
{
    try
    {
        var row = _dataService.Meetings.AsEnumerable()
            .FirstOrDefault(x => x.Field<int>("MeetingID") == id);
        
        if (row == null)
            return Json(new { success = false, message = "Meeting not found." });

        // Check if meeting has members
        var hasMembers = _dataService.MeetingMembers.AsEnumerable()
            .Any(m => m.Field<int>("MeetingID") == id);
        
        if (hasMembers)
            return Json(new { success = false, message = "Cannot delete meeting with assigned members." });

        _dataService.Meetings.Rows.Remove(row);
        return Json(new { success = true, message = "Meeting deleted successfully!" });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = $"Error deleting meeting: {ex.Message}" });
    }
}
```

##### Cancel Meeting
```csharp
[HttpPost]
public IActionResult CancelMeeting(int id, string reason)
{
    try
    {
        var row = _dataService.Meetings.AsEnumerable()
            .FirstOrDefault(x => x.Field<int>("MeetingID") == id);
        
        if (row != null)
        {
            row["IsCancelled"] = true;
            row["CancellationDateTime"] = DateTime.Now;
            row["CancellationReason"] = reason ?? "";
            row["Modified"] = DateTime.Now;
            
            return Json(new { success = true, message = "Meeting cancelled successfully!" });
        }
        
        return Json(new { success = false, message = "Meeting not found." });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = $"Error cancelling meeting: {ex.Message}" });
    }
}
```

##### Get Meeting Details
```csharp
[HttpGet]
public IActionResult GetMeetingDetails(int id)
{
    try
    {
        var row = _dataService.Meetings.AsEnumerable()
            .FirstOrDefault(r => r.Field<int>("MeetingID") == id);

        if (row == null)
            return Json(new { success = false, message = "Meeting not found." });

        // Get related data
        var dept = _dataService.Departments.AsEnumerable()
            .FirstOrDefault(d => d.Field<int>("DepartmentID") == row.Field<int>("DepartmentID"));
        
        // Calculate statistics
        var members = _dataService.MeetingMembers.AsEnumerable()
            .Where(m => m.Field<int>("MeetingID") == id);
        
        return Json(new
        {
            success = true,
            data = new
            {
                MeetingID = row.Field<int>("MeetingID"),
                MeetingDescription = row.Field<string>("MeetingDescription"),
                MeetingDate = row.Field<DateTime>("MeetingDate").ToString("dd/MM/yyyy hh:mm tt"),
                DepartmentName = dept?.Field<string>("DepartmentName") ?? "Unknown",
                // ... other properties
                MemberCount = members.Count(),
                PresentCount = members.Count(m => m.Field<bool>("IsPresent"))
            }
        });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = $"Error getting meeting details: {ex.Message}" });
    }
}
```

## StaffController

### Purpose
Manages staff members including CRUD operations, department assignments, and status management.

### Key Features
- Staff listing with search and filtering
- Add/Edit staff with validation
- Department assignment
- Active/Inactive status management
- Email and mobile validation
- Bulk operations support

### Key Actions
```csharp
public IActionResult StaffList(string search = "", int departmentFilter = 0, string statusFilter = "", int page = 1)
public IActionResult StaffAddEdit(int? id)
[HttpPost] public IActionResult StaffAddEdit(StaffModel model)
[HttpPost] public IActionResult Delete(int id)
[HttpPost] public IActionResult ToggleStatus(int id)
```

## DepartmentController

### Purpose
Manages organizational departments with hierarchical structure support.

### Key Features
- Department CRUD operations
- Remarks and description management
- Staff count tracking
- Meeting count statistics
- Department-wise reporting

## MeetingTypeController

### Purpose
Manages different types of meetings (Board Meeting, Team Meeting, etc.).

### Key Features
- Meeting type definitions
- Category management
- Usage statistics
- Type-based filtering support

## MeetingVenueController

### Purpose
Manages meeting venues with capacity and location information.

### Key Features
- Venue CRUD operations
- Capacity management
- Location tracking
- Availability checking
- Venue utilization reports

## MeetingMembersController

### Purpose
Manages the relationship between meetings and staff members (attendees).

### Key Features
- Member assignment to meetings
- Attendance tracking
- Present/Absent status
- Join time tracking
- Bulk member operations

### Key Actions
```csharp
public IActionResult MeetingMemberList(int? meetingId, int? staffId)
public IActionResult MeetingMemberAddEdit(int? id, int? meetingId)
[HttpPost] public IActionResult MeetingMemberAddEdit(MeetingMembersModel model)
[HttpPost] public IActionResult Delete(int id)
[HttpPost] public IActionResult ToggleAttendance(int id)
```

## AuthController

### Purpose
Handles user authentication and authorization.

### Key Features
- Login/Logout functionality
- Session management
- User validation
- Security token handling

## AccountController

### Purpose
Manages user account operations and profile management.

### Key Features
- Profile management
- Settings configuration
- Password management
- Account preferences

## Common Patterns

### 1. Error Handling Pattern
```csharp
try
{
    // Business logic
    return Json(new { success = true, message = "Operation successful" });
}
catch (Exception ex)
{
    return Json(new { success = false, message = $"Error: {ex.Message}" });
}
```

### 2. Validation Pattern
```csharp
if (!ModelState.IsValid)
{
    // Repopulate ViewBag data
    ViewBag.Departments = _dataService.Departments;
    return View(model);
}
```

### 3. AJAX Response Pattern
```csharp
return Json(new { 
    success = true/false, 
    message = "Status message",
    data = responseData 
});
```

### 4. Pagination Pattern
```csharp
var totalRecords = filteredRows.Count();
var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
var pagedRows = filteredRows.Skip((page - 1) * pageSize).Take(pageSize);
```

### 5. Search and Filter Pattern
```csharp
var filteredRows = dataTable.AsEnumerable().Where(row => true);

if (!string.IsNullOrEmpty(search))
    filteredRows = filteredRows.Where(row => 
        row.Field<string>("ColumnName")?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);

if (filter > 0)
    filteredRows = filteredRows.Where(row => row.Field<int>("FilterColumn") == filter);
```

This documentation provides comprehensive coverage of all controllers in the MOM system, their purposes, key features, and implementation patterns.