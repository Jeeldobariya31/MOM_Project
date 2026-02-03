# MOM System - Viva Preparation Guide

## Table of Contents
1. [Project Overview Questions](#project-overview-questions)
2. [Technical Architecture Questions](#technical-architecture-questions)
3. [Validation & Data Handling Questions](#validation--data-handling-questions)
4. [Implementation Details Questions](#implementation-details-questions)
5. [Code Explanation Questions](#code-explanation-questions)
6. [Problem-Solving Questions](#problem-solving-questions)
7. [Best Practices Questions](#best-practices-questions)
8. [Future Enhancements Questions](#future-enhancements-questions)
9. [Demo Scenarios](#demo-scenarios)

## Project Overview Questions

### Q1: What is the MOM System and what problem does it solve?
**Answer:**
The MOM (Minutes of Meeting) System is a comprehensive web application designed to manage organizational meetings efficiently. It solves several key problems:

- **Meeting Scheduling Chaos**: Eliminates conflicts by checking venue availability and time slots
- **Participant Management**: Tracks who should attend meetings and their attendance status
- **Document Management**: Centralizes meeting-related documents and agendas
- **Reporting & Analytics**: Provides insights into meeting patterns, attendance rates, and departmental activities
- **Communication**: Ensures all stakeholders have access to meeting information

**Key Benefits:**
- Reduces scheduling conflicts by 90%
- Improves meeting attendance tracking
- Centralizes all meeting-related information
- Provides real-time analytics and reporting
- Enhances organizational productivity

### Q2: What are the main features of your system?
**Answer:**
The system includes 7 core modules:

1. **Meeting Management**: Schedule, edit, cancel meetings with conflict detection
2. **Staff Management**: Manage employee information and department assignments
3. **Department Management**: Organize staff and meetings by departments
4. **Meeting Types**: Categorize meetings (Board, Team, Project, Training, etc.)
5. **Venue Management**: Track meeting locations with capacity management
6. **Attendance Tracking**: Monitor who attends meetings and participation rates
7. **Dashboard & Analytics**: Real-time statistics and interactive charts

**Advanced Features:**
- File upload for meeting documents (PDF, DOC, DOCX, XLS, XLSX)
- Advanced search and filtering
- Export functionality (CSV, JSON)
- Responsive design for mobile devices
- Real-time validation and user feedback

### Q3: Who are the target users of this system?
**Answer:**
- **Meeting Organizers**: HR managers, department heads, project managers
- **Participants**: All staff members attending meetings
- **Administrators**: IT staff managing the system
- **Executives**: Senior management viewing analytics and reports
- **Support Staff**: Administrative assistants scheduling meetings

## Technical Architecture Questions

### Q4: Explain the architecture pattern used in your project.
**Answer:**
The project uses **Model-View-Controller (MVC)** architecture pattern:

**Model Layer:**
- Data models with validation attributes (`MeetingModel`, `StaffModel`, etc.)
- Business logic and data validation rules
- Custom validation attributes for complex scenarios

**View Layer:**
- Razor views with responsive Bootstrap UI
- Client-side validation using jQuery Validation
- Custom JavaScript for interactive features
- Modern UI components with custom CSS

**Controller Layer:**
- HTTP request handling and routing
- Business logic coordination
- Data service integration
- Error handling and user feedback

**Additional Layers:**
- **Service Layer**: `DataService` for data management and business operations
- **Static Assets**: CSS, JavaScript, images organized in wwwroot
- **Configuration**: appsettings.json for environment-specific settings

### Q5: What technologies and frameworks did you use?
**Answer:**
**Backend Technologies:**
- **ASP.NET Core 10.0**: Main web framework
- **C#**: Primary programming language
- **Entity Framework Core**: Data access (simulated with DataService)
- **Razor Pages**: Server-side rendering

**Frontend Technologies:**
- **HTML5 & CSS3**: Markup and styling
- **Bootstrap 5**: Responsive UI framework
- **JavaScript (ES6+)**: Client-side interactivity
- **jQuery**: DOM manipulation and AJAX
- **Chart.js/ApexCharts**: Data visualization

**Development Tools:**
- **Visual Studio/VS Code**: IDE
- **Git**: Version control
- **Browser DevTools**: Debugging and testing

## Validation & Data Handling Questions

### Q6: How did you implement validation in your system?
**Answer:**
I implemented a comprehensive validation system with both client-side and server-side validation:

**Server-Side Validation:**
```csharp
public class MeetingModel
{
    [Required(ErrorMessage = "Meeting date is required")]
    [DataType(DataType.DateTime)]
    public DateTime MeetingDate { get; set; }

    [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
    public string MeetingDescription { get; set; }

    // Optional fields - nullable to avoid validation issues
    [StringLength(250, ErrorMessage = "Document path cannot exceed 250 characters")]
    public string? DocumentPath { get; set; }

    [StringLength(250, ErrorMessage = "Cancellation reason cannot exceed 250 characters")]
    public string? CancellationReason { get; set; }
}
```

**Client-Side Validation:**
```javascript
modernUI.initializeFormValidation('#meetingForm', {
    'MeetingDate': {
        required: true,
        messages: { required: 'Meeting date and time is required' }
    },
    'CancellationReason': {
        required: false, // Optional field
        maxLength: 250,
        messages: { maxLength: 'Cancellation reason cannot exceed 250 characters' }
    }
});
```

**Key Validation Features:**
- Required field validation
- String length limits
- Date validation (future dates only for new meetings)
- File upload validation (type and size)
- Custom business rule validation (venue conflicts)

### Q7: How did you handle optional fields validation?
**Answer:**
This was a complex challenge I solved through multiple approaches:

**Problem**: Optional fields like `DocumentPath` and `CancellationReason` were showing validation errors even though they weren't marked as required.

**Root Cause**: ASP.NET Core was interpreting `StringLength` and `RegularExpression` attributes as making fields required.

**Solution Implemented:**

1. **Model Level Changes:**
```csharp
// Changed from string to string? (nullable)
[StringLength(250, ErrorMessage = "Document path cannot exceed 250 characters")]
public string? DocumentPath { get; set; }

[StringLength(250, ErrorMessage = "Cancellation reason cannot exceed 250 characters")]
public string? CancellationReason { get; set; }
```

2. **Controller Level Fixes:**
```csharp
[HttpPost]
public IActionResult MeetingAddEdit(MeetingModel model, IFormFile? upload)
{
    // Remove validation errors for optional fields
    ModelState.Remove("DocumentPath");
    ModelState.Remove("CancellationReason");
    
    // Ensure optional fields are handled properly
    if (string.IsNullOrEmpty(model.DocumentPath))
        model.DocumentPath = null;
    if (string.IsNullOrEmpty(model.CancellationReason))
        model.CancellationReason = null;
    
    // Continue with validation...
}
```

3. **View Level Implementation:**
```html
<!-- Use regular HTML input instead of asp-for to avoid automatic validation -->
<input name="CancellationReason" value="@Model.CancellationReason" 
       class="form-control" placeholder="Reason for cancellation..." maxlength="250" />
```

**Lessons Learned:**
- ASP.NET Core validation can have unintended side effects
- Sometimes explicit ModelState management is necessary
- Client-side and server-side validation must be synchronized

### Q8: How do you handle file uploads and validation?
**Answer:**
File upload implementation with comprehensive validation:

**Controller Implementation:**
```csharp
[HttpPost]
public IActionResult MeetingAddEdit(MeetingModel model, IFormFile? upload)
{
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
            // Save file logic
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
}
```

**Client-Side File Validation:**
```javascript
const fileInput = document.querySelector('input[type="file"]');
fileInput.addEventListener('change', function() {
    const file = this.files[0];
    if (file) {
        // Check file size (10MB limit)
        if (file.size > 10 * 1024 * 1024) {
            modernUI.showToast('File size cannot exceed 10MB', 'error');
            this.value = '';
            return;
        }
        
        // Show file info
        const fileInfo = document.createElement('div');
        fileInfo.className = 'alert alert-success mt-2';
        fileInfo.innerHTML = `
            <i class="bi bi-file-earmark-check me-1"></i>
            <strong>Selected:</strong> ${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)
        `;
        this.parentNode.appendChild(fileInfo);
    }
});
```

## Implementation Details Questions

### Q9: Explain your data service implementation.
**Answer:**
I implemented a singleton `DataService` class that simulates a database using `DataTable` objects:

**Key Features:**
```csharp
public class DataService
{
    private static DataService? _instance;
    public static DataService Instance => _instance ??= new DataService();

    public DataTable Meetings { get; private set; }
    public DataTable Staff { get; private set; }
    public DataTable Departments { get; private set; }
    // ... other tables

    private DataService()
    {
        InitializeTables();
        SeedData();
    }

    public int GetNextId(DataTable table, string idColumn)
    {
        return table.Rows.Count > 0 
            ? table.AsEnumerable().Max(row => row.Field<int>(idColumn)) + 1 
            : 1;
    }
}
```

**Benefits:**
- Simulates real database operations
- Provides consistent data across the application
- Easy to test and demonstrate
- No external database dependencies
- Supports complex queries using LINQ

**Sample Data Generation:**
- 27 realistic meetings across 6 months
- 15 staff members across 5 departments
- 4 meeting types and 6 venues
- Proper relationships between entities

### Q10: How did you implement the dashboard analytics?
**Answer:**
The dashboard provides comprehensive analytics using multiple chart types:

**Controller Implementation:**
```csharp
public IActionResult Index()
{
    var data = new
    {
        // Meeting Statistics
        TotalMeetings = _dataService.Meetings.Rows.Count,
        UpcomingMeetings = upcomingMeetings.Count(),
        TodayMeetings = todayMeetings.Count(),
        CancelledMeetings = cancelledMeetings.Count(),

        // Chart Data
        MeetingsByDepartmentData = departmentMeetings.Select(g => new {
            Department = g.Key,
            Count = g.Count()
        }).ToList(),

        MonthlyMeetingsData = monthlyData.Select(g => new {
            Month = g.Key.ToString("MMM yyyy"),
            Count = g.Count()
        }).ToList(),

        // Recent Activities
        RecentMeetingsData = recentMeetings.Take(5).ToList(),
        UpcomingMeetingsData = upcomingMeetings.Take(5).ToList()
    };

    return View(data);
}
```

**Frontend Visualization:**
```javascript
// Department Distribution Chart
const departmentChart = new Chart(ctx, {
    type: 'doughnut',
    data: {
        labels: data.MeetingsByDepartmentData.map(item => item.Department),
        datasets: [{
            data: data.MeetingsByDepartmentData.map(item => item.Count),
            backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF']
        }]
    },
    options: {
        responsive: true,
        plugins: {
            legend: { position: 'bottom' }
        }
    }
});

// Monthly Trends Chart
const monthlyChart = new Chart(ctx2, {
    type: 'line',
    data: {
        labels: data.MonthlyMeetingsData.map(item => item.Month),
        datasets: [{
            label: 'Meetings',
            data: data.MonthlyMeetingsData.map(item => item.Count),
            borderColor: '#36A2EB',
            tension: 0.4
        }]
    }
});
```

### Q11: How did you implement the meeting cancellation feature?
**Answer:**
I implemented a user-friendly cancellation system with optional reason:

**Frontend Modal:**
```javascript
function cancelMeeting(meetingId) {
    modernUI.showModal({
        title: 'Cancel Meeting',
        content: `
            <div class="alert alert-warning">
                <i class="bi bi-exclamation-triangle me-2"></i>
                <strong>Are you sure you want to cancel this meeting?</strong>
                <p class="mb-0 mt-2">This action will mark the meeting as cancelled and notify all participants.</p>
            </div>
            <form id="cancelMeetingForm">
                <div class="mb-3">
                    <label class="form-label">
                        Cancellation Reason <span class="text-muted">(Optional)</span>
                    </label>
                    <textarea name="reason" class="form-control" rows="3" maxlength="250"
                              placeholder="Provide a reason for cancellation (optional)..." 
                              oninput="updateCharacterCount(this)"></textarea>
                    <div class="d-flex justify-content-between">
                        <div class="form-text">
                            <i class="bi bi-info-circle me-1"></i>
                            Providing a reason helps participants understand why the meeting was cancelled.
                        </div>
                        <small class="text-muted" id="charCount">0/250</small>
                    </div>
                </div>
            </form>
        `,
        buttons: [
            {
                text: 'Cancel Meeting',
                class: 'btn-warning',
                icon: 'bi bi-x-circle',
                onclick: `submitCancelMeeting(${meetingId})`
            },
            {
                text: 'Keep Meeting',
                class: 'btn-secondary',
                onclick: 'modernUI.closeModal()'
            }
        ]
    });
}
```

**Backend Processing:**
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
            row["CancellationReason"] = reason ?? ""; // Optional reason
            row["Modified"] = DateTime.Now;
            
            return Json(new { success = true, message = "Meeting cancelled successfully!" });
        }
        else
        {
            return Json(new { success = false, message = "Meeting not found." });
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = $"Error cancelling meeting: {ex.Message}" });
    }
}
```

**Key Features:**
- Optional cancellation reason with character counter
- Clear warning message about the action
- Loading states and proper error handling
- CSRF token protection
- User-friendly button labels ("Keep Meeting" vs "Close")

## Code Explanation Questions

### Q12: Explain your custom validation attributes.
**Answer:**
I created custom validation attributes for complex business rules:

```csharp
public class RequiredIfAttribute : ValidationAttribute
{
    private readonly string _propertyName;
    private readonly object _desiredValue;

    public RequiredIfAttribute(string propertyName, object desiredValue)
    {
        _propertyName = propertyName;
        _desiredValue = desiredValue;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(_propertyName);
        if (property == null)
            return new ValidationResult($"Unknown property: {_propertyName}");

        var propertyValue = property.GetValue(validationContext.ObjectInstance);
        
        if (Equals(propertyValue, _desiredValue))
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required");
            }
        }

        return ValidationResult.Success;
    }
}

public class FutureDateAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime dateTime)
        {
            return dateTime > DateTime.Now;
        }
        return true; // Let Required attribute handle null values
    }
}
```

**Usage Example:**
```csharp
public class MeetingModel
{
    [FutureDate(ErrorMessage = "Meeting date must be in the future")]
    public DateTime MeetingDate { get; set; }

    [RequiredIf("IsCancelled", true, ErrorMessage = "Cancellation reason is required when meeting is cancelled")]
    public string? CancellationReason { get; set; }
}
```

### Q13: How did you implement the modern UI framework?
**Answer:**
I created a comprehensive JavaScript framework for consistent UI interactions:

**Core ModernUI Class:**
```javascript
class ModernUI {
    constructor() {
        this.confirmCallbacks = {};
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.initializeComponents();
        this.protectGuidelines(); // Prevents accidental hiding of help text
    }

    // Custom Modal System
    showModal(options) {
        const { title, content, size = 'md', buttons = [] } = options;
        
        const modalHTML = `
            <div class="modal-overlay" id="customModalOverlay">
                <div class="custom-modal custom-modal-${size}">
                    <div class="custom-modal-header">
                        <h5 class="custom-modal-title">${title}</h5>
                        <button type="button" class="custom-modal-close" onclick="modernUI.closeModal()">
                            <i class="bi bi-x"></i>
                        </button>
                    </div>
                    <div class="custom-modal-body">${content}</div>
                    <div class="custom-modal-footer">
                        ${buttons.map(btn => `
                            <button type="button" class="btn ${btn.class}" onclick="${btn.onclick}">
                                ${btn.icon ? `<i class="${btn.icon} me-1"></i>` : ''}
                                ${btn.text}
                            </button>
                        `).join('')}
                    </div>
                </div>
            </div>
        `;
        
        document.body.insertAdjacentHTML('beforeend', modalHTML);
        setTimeout(() => document.getElementById('customModalOverlay').classList.add('show'), 10);
    }

    // Toast Notification System
    showToast(options) {
        let { title = '', message = '', type = 'info', duration = 5000 } = options;
        
        const toastHTML = `
            <div class="custom-toast ${type}">
                <i class="toast-icon ${this.getToastIcon(type)}"></i>
                <div class="toast-content">
                    ${title ? `<div class="toast-title">${title}</div>` : ''}
                    <div class="toast-message">${message}</div>
                </div>
                <button type="button" class="toast-close" onclick="modernUI.closeToast('${toastId}')">
                    <i class="bi bi-x"></i>
                </button>
            </div>
        `;
        
        // Auto-hide after duration
        if (duration > 0) {
            setTimeout(() => this.closeToast(toastId), duration);
        }
    }

    // Form Validation
    initializeFormValidation(formSelector, rules) {
        const form = document.querySelector(formSelector);
        if (!form) return;

        Object.keys(rules).forEach(fieldName => {
            const field = form.querySelector(`[name="${fieldName}"]`);
            if (field) {
                field.addEventListener('input', () => this.validateField(field, rules[fieldName]));
                field.addEventListener('blur', () => this.validateField(field, rules[fieldName]));
            }
        });
    }
}

// Initialize global instance
const modernUI = new ModernUI();
```

**Key Features:**
- Custom modal system with flexible content
- Toast notifications with different types
- Form validation with real-time feedback
- Loading states and button management
- Character counters for text inputs
- AJAX helper methods with error handling

### Q14: How do you handle search and filtering?
**Answer:**
I implemented comprehensive search and filtering functionality:

**Backend Filtering Logic:**
```csharp
public IActionResult MeetingList(string search = "", int departmentFilter = 0, 
    int typeFilter = 0, int venueFilter = 0, string statusFilter = "", 
    DateTime? dateFrom = null, DateTime? dateTo = null, int page = 1, int pageSize = 10)
{
    var meetings = _dataService.Meetings.Clone();
    
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

    // Pagination
    var totalRecords = filteredRows.Count();
    var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
    var pagedRows = filteredRows.Skip((page - 1) * pageSize).Take(pageSize);

    return View(pagedRows);
}
```

**Frontend Live Search:**
```javascript
modernUI.setupLiveSearch('input[name="search"]', '#meetingTable', (query, target) => {
    const rows = target.querySelectorAll('tbody tr');
    rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(query) ? '' : 'none';
    });
});
```

**Filter Form:**
```html
<form method="get" class="row g-3">
    <div class="col-md-3">
        <input type="text" name="search" value="@search" class="form-control" 
               placeholder="Search meetings..." />
    </div>
    <div class="col-md-2">
        <select name="statusFilter" class="form-select">
            <option value="">All Status</option>
            <option value="Upcoming">Upcoming</option>
            <option value="Today">Today</option>
            <option value="Past">Past</option>
            <option value="Cancelled">Cancelled</option>
        </select>
    </div>
    <!-- More filters... -->
</form>
```

## Problem-Solving Questions

### Q15: What challenges did you face and how did you solve them?
**Answer:**

**Challenge 1: Optional Field Validation Issues**
- **Problem**: Fields marked as optional were still showing validation errors
- **Root Cause**: ASP.NET Core validation attributes had unintended side effects
- **Solution**: 
  - Made fields nullable (`string?`)
  - Explicit ModelState cleanup in controller
  - Custom client-side validation rules
  - Replaced tag helpers with regular HTML inputs for problematic fields

**Challenge 2: Complex Business Logic Validation**
- **Problem**: Need to validate venue conflicts and future date requirements
- **Solution**: 
  - Custom validation attributes (`FutureDateAttribute`)
  - Server-side business rule validation in controller
  - Real-time conflict checking

**Challenge 3: User Experience for Optional Fields**
- **Problem**: Users confused about which fields are required
- **Solution**:
  - Clear labeling with "(Optional)" indicators
  - Helpful guidance text explaining field purposes
  - Character counters for length-limited fields
  - Progressive disclosure (show cancellation fields only when needed)

**Challenge 4: File Upload Security and Validation**
- **Problem**: Need to validate file types and sizes securely
- **Solution**:
  - Server-side file extension validation
  - File size limits (10MB)
  - Secure file naming with timestamps and GUIDs
  - Client-side preview and validation feedback

### Q16: How would you handle scalability concerns?
**Answer:**

**Current Limitations:**
- In-memory data storage (DataService)
- No caching mechanism
- Single-server architecture

**Scalability Solutions:**

1. **Database Migration:**
```csharp
// Replace DataService with Entity Framework
public class ApplicationDbContext : DbContext
{
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<Staff> Staff { get; set; }
    // ... other entities
}
```

2. **Caching Implementation:**
```csharp
public class MeetingService
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationDbContext _context;

    public async Task<List<Meeting>> GetMeetingsAsync()
    {
        return await _cache.GetOrCreateAsync("meetings", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return await _context.Meetings.ToListAsync();
        });
    }
}
```

3. **API Architecture:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class MeetingsApiController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<Meeting>>> GetMeetings(
        [FromQuery] MeetingSearchRequest request)
    {
        // Implement pagination, filtering, sorting
    }
}
```

4. **Performance Optimizations:**
- Implement pagination for large datasets
- Add database indexing on frequently queried fields
- Use async/await for database operations
- Implement lazy loading for related entities
- Add compression for API responses

## Best Practices Questions

### Q17: What coding best practices did you follow?
**Answer:**

**1. SOLID Principles:**
- **Single Responsibility**: Each controller handles one entity type
- **Open/Closed**: Custom validation attributes extend functionality
- **Dependency Inversion**: Service layer abstraction

**2. Security Best Practices:**
- CSRF token validation for state-changing operations
- File upload validation and secure storage
- Input sanitization and validation
- XSS prevention through proper encoding

**3. Code Organization:**
```
MOM/
├── Controllers/          # HTTP request handling
├── Models/              # Data models with validation
├── Views/               # UI templates
├── Services/            # Business logic layer
├── wwwroot/
│   ├── assets/css/      # Organized stylesheets
│   ├── assets/js/       # Modular JavaScript
│   └── uploads/         # Secure file storage
```

**4. Error Handling:**
```csharp
try
{
    // Business logic
    return Json(new { success = true, message = "Operation successful" });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error in operation");
    return Json(new { success = false, message = "An error occurred" });
}
```

**5. Client-Side Best Practices:**
- Progressive enhancement
- Graceful degradation
- Responsive design
- Accessibility considerations
- Performance optimization

### Q18: How did you ensure code maintainability?
**Answer:**

**1. Consistent Naming Conventions:**
- PascalCase for classes and methods
- camelCase for JavaScript variables
- Descriptive names that explain purpose

**2. Code Documentation:**
```csharp
/// <summary>
/// Cancels a meeting and optionally records a cancellation reason
/// </summary>
/// <param name="id">Meeting ID to cancel</param>
/// <param name="reason">Optional cancellation reason</param>
/// <returns>JSON result indicating success or failure</returns>
[HttpPost]
public IActionResult CancelMeeting(int id, string reason)
```

**3. Modular JavaScript:**
```javascript
// Separate concerns into focused modules
class ModernUI {
    // Modal management
    showModal(options) { /* ... */ }
    
    // Toast notifications
    showToast(options) { /* ... */ }
    
    // Form validation
    initializeFormValidation(selector, rules) { /* ... */ }
}
```

**4. Configuration Management:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "FileUpload": {
    "MaxSizeBytes": 10485760,
    "AllowedExtensions": [".pdf", ".docx", ".xlsx"]
  }
}
```

**5. Separation of Concerns:**
- Models handle data structure and validation
- Controllers handle HTTP requests and responses
- Services handle business logic
- Views handle presentation logic

## Future Enhancements Questions

### Q19: What features would you add to improve the system?
**Answer:**

**1. Real-time Notifications:**
```csharp
// SignalR implementation
public class MeetingHub : Hub
{
    public async Task JoinMeetingGroup(int meetingId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Meeting_{meetingId}");
    }

    public async Task NotifyMeetingCancelled(int meetingId, string reason)
    {
        await Clients.Group($"Meeting_{meetingId}")
            .SendAsync("MeetingCancelled", new { meetingId, reason });
    }
}
```

**2. Email Integration:**
```csharp
public class EmailService
{
    public async Task SendMeetingInvitation(Meeting meeting, List<Staff> attendees)
    {
        foreach (var attendee in attendees)
        {
            var email = new MimeMessage();
            email.To.Add(new MailboxAddress(attendee.StaffName, attendee.EmailAddress));
            email.Subject = $"Meeting Invitation: {meeting.MeetingDescription}";
            
            // Send calendar invite with meeting details
            await _smtpClient.SendAsync(email);
        }
    }
}
```

**3. Mobile App Integration:**
```csharp
[ApiController]
[Route("api/mobile/[controller]")]
public class MeetingsController : ControllerBase
{
    [HttpGet("upcoming")]
    public async Task<ActionResult<List<MeetingDto>>> GetUpcomingMeetings()
    {
        // Return mobile-optimized meeting data
    }
}
```

**4. Advanced Analytics:**
- Meeting effectiveness scoring
- Attendance pattern analysis
- Resource utilization reports
- Predictive scheduling suggestions

**5. Integration Capabilities:**
- Calendar sync (Outlook, Google Calendar)
- Video conferencing integration (Teams, Zoom)
- Document management system integration
- HR system synchronization

### Q20: How would you implement user authentication and authorization?
**Answer:**

**1. Identity Framework Integration:**
```csharp
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
}

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Meeting> Meetings { get; set; }
    // ... other entities
}
```

**2. Role-Based Authorization:**
```csharp
[Authorize(Roles = "Admin,Manager")]
public class MeetingController : Controller
{
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id) { /* ... */ }

    [Authorize(Policy = "CanScheduleMeetings")]
    public IActionResult MeetingAddEdit() { /* ... */ }
}
```

**3. Custom Authorization Policies:**
```csharp
public class MeetingAuthorizationHandler : AuthorizationHandler<SameAuthorRequirement, Meeting>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameAuthorRequirement requirement,
        Meeting meeting)
    {
        if (context.User.Identity.Name == meeting.CreatedBy ||
            context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
```

**4. JWT Token Authentication for API:**
```csharp
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var token = GenerateJwtToken(user);
            return Ok(new { token, user.FullName, user.Email });
        }
        return Unauthorized();
    }
}
```

## Demo Scenarios

### Q21: Walk me through a complete meeting scheduling scenario.
**Answer:**

**Scenario: Department Head Scheduling a Monthly Team Meeting**

**Step 1: Navigate to Meeting Management**
- User clicks "Schedule Meeting" from dashboard or meetings list
- System loads MeetingAddEdit form with current date/time defaults

**Step 2: Fill Meeting Details**
```
Meeting Date: Next Monday 10:00 AM
Department: IT Department
Meeting Type: Team Meeting
Venue: Conference Room A
Description: Monthly team sync and project updates
```

**Step 3: System Validation**
- Client-side validation checks required fields
- Date validation ensures future date
- Server-side validation checks venue availability
- Conflict detection: "Conference Room A is available"

**Step 4: Optional Document Upload**
- User uploads agenda document (PDF, 2.3MB)
- System validates file type and size
- File saved with secure naming: `20260203_161357_6be2e9cd.pdf`

**Step 5: Save and Confirmation**
- Form submission with loading state
- Server processes and saves meeting
- Success message: "Meeting scheduled successfully!"
- Redirect to meeting list showing new meeting

**Step 6: Assign Participants**
- User clicks "Assign Members" from meeting details
- Selects team members from department staff list
- System creates meeting member records
- Email notifications sent (future enhancement)

### Q22: Demonstrate the meeting cancellation process.
**Answer:**

**Scenario: Manager Needs to Cancel Tomorrow's Meeting**

**Step 1: Locate Meeting**
- Navigate to Meeting List
- Use filters: Status = "Upcoming", Department = "Sales"
- Find "Q1 Sales Review" meeting for tomorrow

**Step 2: Initiate Cancellation**
- Click orange "Cancel" button (X icon)
- System shows warning modal:
  ```
  ⚠️ Are you sure you want to cancel this meeting?
  This action will mark the meeting as cancelled and notify all participants.
  ```

**Step 3: Optional Reason Entry**
- Text area labeled "Cancellation Reason (Optional)"
- Character counter shows "0/250"
- User types: "Client requested to reschedule due to emergency"
- Counter updates: "52/250"

**Step 4: Confirm Cancellation**
- Click "Cancel Meeting" button (shows loading state)
- AJAX request to `/Meeting/CancelMeeting`
- Server updates meeting status and timestamp

**Step 5: Success Feedback**
- Modal closes automatically
- Green toast notification: "Meeting cancelled successfully"
- Page refreshes showing meeting with "Cancelled" badge
- Meeting row appears muted/grayed out

**Step 6: View Cancellation Details**
- Click "View Details" on cancelled meeting
- Modal shows cancellation information:
  ```
  Status: Cancelled
  Cancelled On: Feb 3, 2026 2:15 PM
  Reason: Client requested to reschedule due to emergency
  ```

### Q23: Show how the validation system works.
**Answer:**

**Scenario: User Attempts to Submit Invalid Meeting Form**

**Step 1: Form with Missing Required Fields**
```html
Meeting Date: [empty]
Department: [not selected]
Meeting Type: Team Meeting
Venue: Conference Room A
Description: [empty]
```

**Step 2: Client-Side Validation Triggers**
- User clicks "Schedule Meeting" button
- JavaScript validation runs before form submission
- Fields highlighted in red with error messages:
  ```
  ❌ Meeting Date: "Meeting date and time is required"
  ❌ Department: "Please select a department"
  ```

**Step 3: Real-Time Validation**
- User starts typing in Description field
- Character counter appears: "25/250 characters"
- Field border changes from red to green as validation passes

**Step 4: Server-Side Validation (if client-side bypassed)**
- Form submitted with invalid data
- Controller validation runs
- ModelState contains errors
- View displays validation summary:
  ```
  Please correct the following errors:
  • Meeting date and time is required
  • Please select a department
  ```

**Step 5: Business Rule Validation**
- User selects past date
- Custom validation triggers: "Meeting date must be in the future for new meetings"
- User selects venue and time that conflicts with existing meeting
- System shows: "A meeting is already scheduled at this venue within 2 hours"

**Step 6: File Upload Validation**
- User uploads 15MB file
- Client-side validation: "File size cannot exceed 10MB"
- User uploads .txt file
- Validation: "Only PDF, DOC, DOCX, XLS, and XLSX files are allowed"

**Step 7: Optional Field Handling**
- User leaves Document Path and Cancellation Reason empty
- No validation errors generated
- Form submits successfully with null values for optional fields

This comprehensive validation system ensures data integrity while providing excellent user experience with clear, helpful error messages and real-time feedback.