# Services Documentation - MOM System

## Table of Contents
1. [Service Architecture](#service-architecture)
2. [DataService Overview](#dataservice-overview)
3. [Data Storage Implementation](#data-storage-implementation)
4. [CRUD Operations](#crud-operations)
5. [Data Relationships](#data-relationships)
6. [Business Logic](#business-logic)
7. [Performance Considerations](#performance-considerations)
8. [Error Handling](#error-handling)

## Service Architecture

### Design Pattern: Singleton
The MOM system uses a **Singleton Pattern** for the DataService to ensure a single instance manages all data operations throughout the application lifecycle.

```csharp
public class DataService
{
    private static DataService? _instance;
    private static readonly object _lock = new object();

    public static DataService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new DataService();
                }
            }
            return _instance;
        }
    }

    private DataService()
    {
        InitializeData();
    }
}
```

### Service Layer Benefits
1. **Centralized Data Management**: Single point of data access
2. **Business Logic Encapsulation**: Data operations with business rules
3. **Consistency**: Uniform data access patterns
4. **Testability**: Isolated business logic for unit testing
5. **Maintainability**: Changes in one place affect entire application

## DataService Overview

### Core Responsibilities
- **Data Storage**: In-memory DataTable management
- **CRUD Operations**: Create, Read, Update, Delete functionality
- **Data Relationships**: Foreign key constraint management
- **Business Rules**: Data validation and business logic
- **Sample Data**: Initial data population for demonstration

### Data Tables Structure
```csharp
public class DataService
{
    // Core Data Tables
    public DataTable Departments { get; private set; }
    public DataTable Staff { get; private set; }
    public DataTable MeetingTypes { get; private set; }
    public DataTable MeetingVenues { get; private set; }
    public DataTable Meetings { get; private set; }
    public DataTable MeetingMembers { get; private set; }

    // Utility Methods
    public int GetNextId(DataTable table, string idColumn)
    public void InitializeData()
    private void CreateTables()
    private void PopulateSampleData()
}
```

## Data Storage Implementation

### 1. Departments Table
```csharp
private void CreateDepartmentsTable()
{
    Departments = new DataTable("Departments");
    Departments.Columns.Add("DepartmentID", typeof(int));
    Departments.Columns.Add("DepartmentName", typeof(string));
    Departments.Columns.Add("Remarks", typeof(string));
    Departments.Columns.Add("Created", typeof(DateTime));
    Departments.Columns.Add("Modified", typeof(DateTime));
    
    // Set primary key
    Departments.PrimaryKey = new DataColumn[] { Departments.Columns["DepartmentID"] };
}
```

**Sample Data:**
- IT Department
- Human Resources
- Finance
- Marketing
- Operations

### 2. Staff Table
```csharp
private void CreateStaffTable()
{
    Staff = new DataTable("Staff");
    Staff.Columns.Add("StaffID", typeof(int));
    Staff.Columns.Add("StaffName", typeof(string));
    Staff.Columns.Add("EmailAddress", typeof(string));
    Staff.Columns.Add("MobileNo", typeof(string));
    Staff.Columns.Add("DepartmentID", typeof(int));
    Staff.Columns.Add("IsActive", typeof(bool));
    Staff.Columns.Add("Created", typeof(DateTime));
    Staff.Columns.Add("Modified", typeof(DateTime));
    
    Staff.PrimaryKey = new DataColumn[] { Staff.Columns["StaffID"] };
}
```

**Features:**
- Employee information management
- Department association
- Active/Inactive status tracking
- Contact information storage

### 3. Meeting Types Table
```csharp
private void CreateMeetingTypesTable()
{
    MeetingTypes = new DataTable("MeetingTypes");
    MeetingTypes.Columns.Add("MeetingTypeID", typeof(int));
    MeetingTypes.Columns.Add("MeetingTypeName", typeof(string));
    MeetingTypes.Columns.Add("Remarks", typeof(string));
    MeetingTypes.Columns.Add("Created", typeof(DateTime));
    MeetingTypes.Columns.Add("Modified", typeof(DateTime));
    
    MeetingTypes.PrimaryKey = new DataColumn[] { MeetingTypes.Columns["MeetingTypeID"] };
}
```

**Sample Types:**
- Board Meeting
- Team Meeting
- Project Review
- Training Session
- Client Meeting

### 4. Meeting Venues Table
```csharp
private void CreateMeetingVenuesTable()
{
    MeetingVenues = new DataTable("MeetingVenues");
    MeetingVenues.Columns.Add("MeetingVenueID", typeof(int));
    MeetingVenues.Columns.Add("MeetingVenueName", typeof(string));
    MeetingVenues.Columns.Add("Capacity", typeof(int));
    MeetingVenues.Columns.Add("Location", typeof(string));
    MeetingVenues.Columns.Add("Created", typeof(DateTime));
    MeetingVenues.Columns.Add("Modified", typeof(DateTime));
    
    MeetingVenues.PrimaryKey = new DataColumn[] { MeetingVenues.Columns["MeetingVenueID"] };
}
```

**Features:**
- Venue capacity management
- Location tracking
- Availability checking

### 5. Meetings Table (Core Entity)
```csharp
private void CreateMeetingsTable()
{
    Meetings = new DataTable("Meetings");
    Meetings.Columns.Add("MeetingID", typeof(int));
    Meetings.Columns.Add("MeetingDate", typeof(DateTime));
    Meetings.Columns.Add("MeetingVenueID", typeof(int));
    Meetings.Columns.Add("MeetingTypeID", typeof(int));
    Meetings.Columns.Add("DepartmentID", typeof(int));
    Meetings.Columns.Add("MeetingDescription", typeof(string));
    Meetings.Columns.Add("DocumentPath", typeof(string));
    Meetings.Columns.Add("Created", typeof(DateTime));
    Meetings.Columns.Add("Modified", typeof(DateTime));
    Meetings.Columns.Add("IsCancelled", typeof(bool));
    Meetings.Columns.Add("CancellationDateTime", typeof(DateTime));
    Meetings.Columns.Add("CancellationReason", typeof(string));
    
    Meetings.PrimaryKey = new DataColumn[] { Meetings.Columns["MeetingID"] };
}
```

**Features:**
- Complete meeting lifecycle management
- Document attachment support
- Cancellation tracking with reasons
- Audit trail (Created/Modified timestamps)

### 6. Meeting Members Table (Junction Table)
```csharp
private void CreateMeetingMembersTable()
{
    MeetingMembers = new DataTable("MeetingMembers");
    MeetingMembers.Columns.Add("MeetingMemberID", typeof(int));
    MeetingMembers.Columns.Add("MeetingID", typeof(int));
    MeetingMembers.Columns.Add("StaffID", typeof(int));
    MeetingMembers.Columns.Add("IsPresent", typeof(bool));
    MeetingMembers.Columns.Add("JoinedAt", typeof(DateTime));
    MeetingMembers.Columns.Add("Created", typeof(DateTime));
    MeetingMembers.Columns.Add("Modified", typeof(DateTime));
    
    MeetingMembers.PrimaryKey = new DataColumn[] { MeetingMembers.Columns["MeetingMemberID"] };
}
```

**Features:**
- Many-to-many relationship between Meetings and Staff
- Attendance tracking
- Join time recording
- Meeting participation history

## CRUD Operations

### 1. Create Operations
```csharp
// Example: Adding a new meeting
public int AddMeeting(MeetingModel meeting)
{
    int newId = GetNextId(Meetings, "MeetingID");
    
    Meetings.Rows.Add(
        newId,
        meeting.MeetingDate,
        meeting.MeetingVenueID,
        meeting.MeetingTypeID,
        meeting.DepartmentID,
        meeting.MeetingDescription ?? "",
        meeting.DocumentPath ?? "",
        DateTime.Now,
        DateTime.Now,
        meeting.IsCancelled,
        meeting.IsCancelled ? (meeting.CancellationDateTime ?? DateTime.Now) : (object)DBNull.Value,
        meeting.IsCancelled ? (meeting.CancellationReason ?? "") : ""
    );
    
    return newId;
}
```

### 2. Read Operations
```csharp
// Example: Get meeting with related data
public DataRow GetMeetingWithDetails(int meetingId)
{
    var meeting = Meetings.AsEnumerable()
        .FirstOrDefault(m => m.Field<int>("MeetingID") == meetingId);
    
    if (meeting != null)
    {
        // Add navigation properties
        var dept = Departments.AsEnumerable()
            .FirstOrDefault(d => d.Field<int>("DepartmentID") == meeting.Field<int>("DepartmentID"));
        
        var type = MeetingTypes.AsEnumerable()
            .FirstOrDefault(t => t.Field<int>("MeetingTypeID") == meeting.Field<int>("MeetingTypeID"));
        
        var venue = MeetingVenues.AsEnumerable()
            .FirstOrDefault(v => v.Field<int>("MeetingVenueID") == meeting.Field<int>("MeetingVenueID"));
        
        // Calculate member statistics
        var members = MeetingMembers.AsEnumerable()
            .Where(m => m.Field<int>("MeetingID") == meetingId);
        
        // Return enriched data
        return meeting;
    }
    
    return null;
}
```

### 3. Update Operations
```csharp
// Example: Update meeting
public bool UpdateMeeting(MeetingModel meeting)
{
    var row = Meetings.AsEnumerable()
        .FirstOrDefault(m => m.Field<int>("MeetingID") == meeting.MeetingID);
    
    if (row != null)
    {
        row["MeetingDate"] = meeting.MeetingDate;
        row["MeetingVenueID"] = meeting.MeetingVenueID;
        row["MeetingTypeID"] = meeting.MeetingTypeID;
        row["DepartmentID"] = meeting.DepartmentID;
        row["MeetingDescription"] = meeting.MeetingDescription ?? "";
        row["Modified"] = DateTime.Now;
        
        if (!string.IsNullOrEmpty(meeting.DocumentPath))
            row["DocumentPath"] = meeting.DocumentPath;
        
        row["IsCancelled"] = meeting.IsCancelled;
        row["CancellationDateTime"] = meeting.IsCancelled ? 
            (meeting.CancellationDateTime ?? DateTime.Now) : (object)DBNull.Value;
        row["CancellationReason"] = meeting.IsCancelled ? 
            (meeting.CancellationReason ?? "") : "";
        
        return true;
    }
    
    return false;
}
```

### 4. Delete Operations
```csharp
// Example: Delete meeting with cascade check
public bool DeleteMeeting(int meetingId)
{
    // Check for dependent records
    var hasMembers = MeetingMembers.AsEnumerable()
        .Any(m => m.Field<int>("MeetingID") == meetingId);
    
    if (hasMembers)
    {
        throw new InvalidOperationException("Cannot delete meeting with assigned members.");
    }
    
    var row = Meetings.AsEnumerable()
        .FirstOrDefault(m => m.Field<int>("MeetingID") == meetingId);
    
    if (row != null)
    {
        Meetings.Rows.Remove(row);
        return true;
    }
    
    return false;
}
```

## Data Relationships

### 1. Foreign Key Relationships
```
Meetings
├── DepartmentID → Departments.DepartmentID
├── MeetingTypeID → MeetingTypes.MeetingTypeID
└── MeetingVenueID → MeetingVenues.MeetingVenueID

MeetingMembers
├── MeetingID → Meetings.MeetingID
└── StaffID → Staff.StaffID

Staff
└── DepartmentID → Departments.DepartmentID
```

### 2. Relationship Validation
```csharp
public bool ValidateForeignKeys(MeetingModel meeting)
{
    // Check if Department exists
    var deptExists = Departments.AsEnumerable()
        .Any(d => d.Field<int>("DepartmentID") == meeting.DepartmentID);
    
    // Check if MeetingType exists
    var typeExists = MeetingTypes.AsEnumerable()
        .Any(t => t.Field<int>("MeetingTypeID") == meeting.MeetingTypeID);
    
    // Check if MeetingVenue exists
    var venueExists = MeetingVenues.AsEnumerable()
        .Any(v => v.Field<int>("MeetingVenueID") == meeting.MeetingVenueID);
    
    return deptExists && typeExists && venueExists;
}
```

### 3. Cascade Operations
```csharp
// Example: Get all meetings for a department
public IEnumerable<DataRow> GetMeetingsByDepartment(int departmentId)
{
    return Meetings.AsEnumerable()
        .Where(m => m.Field<int>("DepartmentID") == departmentId)
        .OrderByDescending(m => m.Field<DateTime>("MeetingDate"));
}

// Example: Get meeting attendance statistics
public (int Total, int Present) GetMeetingAttendance(int meetingId)
{
    var members = MeetingMembers.AsEnumerable()
        .Where(m => m.Field<int>("MeetingID") == meetingId);
    
    var total = members.Count();
    var present = members.Count(m => m.Field<bool>("IsPresent"));
    
    return (total, present);
}
```

## Business Logic

### 1. Meeting Scheduling Rules
```csharp
public class MeetingBusinessRules
{
    public static bool CanScheduleMeeting(DateTime meetingDate, int venueId, DataService dataService)
    {
        // Rule 1: Cannot schedule meetings in the past
        if (meetingDate <= DateTime.Now)
            return false;
        
        // Rule 2: Check venue availability (no overlapping meetings within 2 hours)
        var conflictingMeetings = dataService.Meetings.AsEnumerable()
            .Where(m => m.Field<int>("MeetingVenueID") == venueId &&
                       !m.Field<bool>("IsCancelled") &&
                       Math.Abs((m.Field<DateTime>("MeetingDate") - meetingDate).TotalHours) < 2);
        
        return !conflictingMeetings.Any();
    }
    
    public static bool CanCancelMeeting(int meetingId, DataService dataService)
    {
        var meeting = dataService.Meetings.AsEnumerable()
            .FirstOrDefault(m => m.Field<int>("MeetingID") == meetingId);
        
        if (meeting == null) return false;
        
        // Rule: Can only cancel future meetings
        var meetingDate = meeting.Field<DateTime>("MeetingDate");
        return meetingDate > DateTime.Now && !meeting.Field<bool>("IsCancelled");
    }
}
```

### 2. Data Validation Rules
```csharp
public class DataValidationRules
{
    public static ValidationResult ValidateMeeting(MeetingModel meeting, DataService dataService)
    {
        var errors = new List<string>();
        
        // Required field validation
        if (meeting.MeetingDate == default)
            errors.Add("Meeting date is required");
        
        if (meeting.DepartmentID <= 0)
            errors.Add("Department selection is required");
        
        if (meeting.MeetingTypeID <= 0)
            errors.Add("Meeting type selection is required");
        
        if (meeting.MeetingVenueID <= 0)
            errors.Add("Meeting venue selection is required");
        
        // Business rule validation
        if (!MeetingBusinessRules.CanScheduleMeeting(meeting.MeetingDate, meeting.MeetingVenueID, dataService))
            errors.Add("Selected venue is not available at the specified time");
        
        // String length validation
        if (!string.IsNullOrEmpty(meeting.MeetingDescription) && meeting.MeetingDescription.Length > 250)
            errors.Add("Meeting description cannot exceed 250 characters");
        
        if (!string.IsNullOrEmpty(meeting.CancellationReason) && meeting.CancellationReason.Length > 250)
            errors.Add("Cancellation reason cannot exceed 250 characters");
        
        return new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}
```

### 3. Sample Data Population
```csharp
private void PopulateSampleData()
{
    // Populate Departments
    PopulateDepartments();
    
    // Populate Staff
    PopulateStaff();
    
    // Populate Meeting Types
    PopulateMeetingTypes();
    
    // Populate Meeting Venues
    PopulateMeetingVenues();
    
    // Populate Meetings with realistic data
    PopulateMeetings();
    
    // Populate Meeting Members
    PopulateMeetingMembers();
}

private void PopulateMeetings()
{
    var random = new Random();
    var meetingDescriptions = new[]
    {
        "Quarterly Business Review",
        "Project Planning Session",
        "Team Standup Meeting",
        "Client Presentation",
        "Budget Planning Meeting",
        "Performance Review Discussion",
        "Strategic Planning Workshop",
        "Product Launch Meeting",
        "Training Session",
        "Board Meeting"
    };
    
    // Generate 27 meetings over the past 6 months
    for (int i = 0; i < 27; i++)
    {
        var daysAgo = random.Next(0, 180); // 0 to 6 months ago
        var meetingDate = DateTime.Now.AddDays(-daysAgo).AddHours(random.Next(9, 17));
        
        var departmentId = random.Next(1, 6);
        var typeId = random.Next(1, 6);
        var venueId = random.Next(1, 6);
        var description = meetingDescriptions[random.Next(meetingDescriptions.Length)];
        
        var isCancelled = random.Next(1, 10) == 1; // 10% chance of cancellation
        
        Meetings.Rows.Add(
            i + 1,
            meetingDate,
            venueId,
            typeId,
            departmentId,
            description,
            "", // DocumentPath
            DateTime.Now.AddDays(-daysAgo - 1),
            DateTime.Now.AddDays(-daysAgo - 1),
            isCancelled,
            isCancelled ? (object)meetingDate.AddHours(-2) : DBNull.Value,
            isCancelled ? "Schedule conflict" : ""
        );
    }
}
```

## Performance Considerations

### 1. Memory Management
- **In-Memory Storage**: Fast access but limited by available RAM
- **DataTable Optimization**: Efficient for small to medium datasets
- **Indexing**: Primary keys provide O(1) lookup performance
- **Lazy Loading**: Data loaded only when needed

### 2. Query Optimization
```csharp
// Efficient filtering using LINQ
public IEnumerable<DataRow> GetFilteredMeetings(
    string searchTerm = null,
    int? departmentId = null,
    DateTime? fromDate = null,
    DateTime? toDate = null)
{
    var query = Meetings.AsEnumerable();
    
    // Apply filters progressively
    if (!string.IsNullOrEmpty(searchTerm))
    {
        query = query.Where(m => 
            m.Field<string>("MeetingDescription")?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true);
    }
    
    if (departmentId.HasValue)
    {
        query = query.Where(m => m.Field<int>("DepartmentID") == departmentId.Value);
    }
    
    if (fromDate.HasValue)
    {
        query = query.Where(m => m.Field<DateTime>("MeetingDate").Date >= fromDate.Value.Date);
    }
    
    if (toDate.HasValue)
    {
        query = query.Where(m => m.Field<DateTime>("MeetingDate").Date <= toDate.Value.Date);
    }
    
    return query.OrderByDescending(m => m.Field<DateTime>("MeetingDate"));
}
```

### 3. Caching Strategy
```csharp
// Simple caching for frequently accessed data
private Dictionary<string, object> _cache = new Dictionary<string, object>();

public IEnumerable<DataRow> GetActiveDepartments()
{
    const string cacheKey = "active_departments";
    
    if (!_cache.ContainsKey(cacheKey))
    {
        var activeDepts = Departments.AsEnumerable()
            .Where(d => HasActiveStaff(d.Field<int>("DepartmentID")))
            .ToList();
        
        _cache[cacheKey] = activeDepts;
    }
    
    return (IEnumerable<DataRow>)_cache[cacheKey];
}
```

## Error Handling

### 1. Exception Management
```csharp
public class DataServiceException : Exception
{
    public DataServiceException(string message) : base(message) { }
    public DataServiceException(string message, Exception innerException) : base(message, innerException) { }
}

public class BusinessRuleException : DataServiceException
{
    public BusinessRuleException(string message) : base(message) { }
}
```

### 2. Safe Operations
```csharp
public (bool Success, string Message) SafeDeleteMeeting(int meetingId)
{
    try
    {
        // Check business rules
        var hasMembers = MeetingMembers.AsEnumerable()
            .Any(m => m.Field<int>("MeetingID") == meetingId);
        
        if (hasMembers)
        {
            return (false, "Cannot delete meeting with assigned members. Please remove members first.");
        }
        
        var meeting = Meetings.AsEnumerable()
            .FirstOrDefault(m => m.Field<int>("MeetingID") == meetingId);
        
        if (meeting == null)
        {
            return (false, "Meeting not found.");
        }
        
        Meetings.Rows.Remove(meeting);
        return (true, "Meeting deleted successfully.");
    }
    catch (Exception ex)
    {
        return (false, $"Error deleting meeting: {ex.Message}");
    }
}
```

### 3. Data Integrity Checks
```csharp
public bool ValidateDataIntegrity()
{
    try
    {
        // Check for orphaned records
        var orphanedMeetings = Meetings.AsEnumerable()
            .Where(m => !Departments.AsEnumerable()
                .Any(d => d.Field<int>("DepartmentID") == m.Field<int>("DepartmentID")));
        
        var orphanedMembers = MeetingMembers.AsEnumerable()
            .Where(mm => !Meetings.AsEnumerable()
                .Any(m => m.Field<int>("MeetingID") == mm.Field<int>("MeetingID")) ||
                        !Staff.AsEnumerable()
                .Any(s => s.Field<int>("StaffID") == mm.Field<int>("StaffID")));
        
        return !orphanedMeetings.Any() && !orphanedMembers.Any();
    }
    catch (Exception)
    {
        return false;
    }
}
```

This comprehensive documentation covers all aspects of the DataService implementation, including architecture, data storage, CRUD operations, business logic, performance considerations, and error handling strategies used in the MOM system.