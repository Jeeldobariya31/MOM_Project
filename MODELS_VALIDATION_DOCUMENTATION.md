# Models & Validation Documentation - MOM System

## Table of Contents
1. [Model Architecture](#model-architecture)
2. [Data Models](#data-models)
3. [Validation Framework](#validation-framework)
4. [Custom Validation Attributes](#custom-validation-attributes)
5. [ViewModels](#viewmodels)
6. [Model Binding](#model-binding)
7. [Validation Patterns](#validation-patterns)
8. [Error Handling](#error-handling)

## Model Architecture

### Design Principles
The MOM system follows these model design principles:

1. **Single Responsibility**: Each model represents a single business entity
2. **Data Annotations**: Declarative validation using attributes
3. **Separation of Concerns**: Models focus on data structure and validation
4. **Immutable Properties**: Read-only properties for calculated fields
5. **Nullable Reference Types**: Explicit handling of optional fields

### Model Structure
```
Models/
├── Core Models/
│   ├── MeetingModel.cs
│   ├── StaffModel.cs
│   ├── DepartmentModel.cs
│   ├── MeetingTypeModel.cs
│   ├── MeetingVenueModel.cs
│   └── MeetingMembersModel.cs
├── ViewModels/
│   └── MeetingViewModel.cs
└── Base Classes/
    └── BaseModel.cs (if implemented)
```

## Data Models

### 1. MeetingModel
```csharp
public class MeetingModel
{
    [Key]
    public int MeetingID { get; set; }

    [Required(ErrorMessage = "Meeting date is required")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Meeting Date & Time")]
    public DateTime MeetingDate { get; set; }

    [Required(ErrorMessage = "Venue is required")]
    [Display(Name = "Meeting Venue")]
    public int MeetingVenueID { get; set; }

    [Required(ErrorMessage = "Meeting type is required")]
    [Display(Name = "Meeting Type")]
    public int MeetingTypeID { get; set; }

    [Required(ErrorMessage = "Department is required")]
    [Display(Name = "Department")]
    public int DepartmentID { get; set; }

    [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
    [Display(Name = "Meeting Description")]
    public string MeetingDescription { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "Document path cannot exceed 250 characters")]
    [Display(Name = "Document Path")]
    public string? DocumentPath { get; set; }

    [Display(Name = "Created Date")]
    public DateTime Created { get; set; } = DateTime.Now;

    [Display(Name = "Modified Date")]
    public DateTime Modified { get; set; } = DateTime.Now;

    [Display(Name = "Is Cancelled")]
    public bool IsCancelled { get; set; }

    [Display(Name = "Cancellation Date & Time")]
    public DateTime? CancellationDateTime { get; set; }

    [StringLength(250, ErrorMessage = "Cancellation reason cannot exceed 250 characters")]
    [Display(Name = "Cancellation Reason")]
    public string? CancellationReason { get; set; }

    // Navigation properties (NotMapped for in-memory storage)
    [NotMapped]
    [Display(Name = "Department Name")]
    public string DepartmentName { get; set; } = string.Empty;

    [NotMapped]
    [Display(Name = "Meeting Type Name")]
    public string MeetingTypeName { get; set; } = string.Empty;

    [NotMapped]
    [Display(Name = "Meeting Venue Name")]
    public string MeetingVenueName { get; set; } = string.Empty;

    [NotMapped]
    public int MemberCount { get; set; }

    [NotMapped]
    public int PresentCount { get; set; }
}
```

**Key Features:**
- **Primary Key**: MeetingID with [Key] attribute
- **Required Fields**: Date, Venue, Type, Department
- **Optional Fields**: Description, DocumentPath, CancellationReason
- **Audit Fields**: Created, Modified timestamps
- **Navigation Properties**: Calculated fields for display
- **Nullable Types**: Optional fields use nullable reference types

### 2. StaffModel
```csharp
public class StaffModel
{
    [Key]
    public int StaffID { get; set; }

    [Required(ErrorMessage = "Staff name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Staff name must be between 2 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Staff name can only contain letters and spaces")]
    [Display(Name = "Staff Name")]
    public string StaffName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required")]
    [StringLength(50, ErrorMessage = "Email address cannot exceed 50 characters")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Display(Name = "Email Address")]
    public string EmailAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile number is required")]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "Mobile number must be between 10 and 20 characters")]
    [RegularExpression(@"^[0-9+\-\s()]+$", ErrorMessage = "Please enter a valid mobile number")]
    [Display(Name = "Mobile Number")]
    public string MobileNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required")]
    [Display(Name = "Department")]
    public int DepartmentID { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Created Date")]
    public DateTime Created { get; set; } = DateTime.Now;

    [Display(Name = "Modified Date")]
    public DateTime Modified { get; set; } = DateTime.Now;

    // Navigation properties
    [NotMapped]
    [Display(Name = "Department Name")]
    public string DepartmentName { get; set; } = string.Empty;

    [NotMapped]
    public int MeetingCount { get; set; }
}
```

**Validation Features:**
- **Name Validation**: Letters and spaces only, 2-50 characters
- **Email Validation**: Built-in EmailAddress attribute
- **Mobile Validation**: Flexible phone number format
- **Department Reference**: Required foreign key
- **Status Tracking**: Active/Inactive boolean flag

### 3. DepartmentModel
```csharp
public class DepartmentModel
{
    [Key]
    public int DepartmentID { get; set; }

    [Required(ErrorMessage = "Department name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Department name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s&.-]+$", ErrorMessage = "Department name contains invalid characters")]
    [Display(Name = "Department Name")]
    public string DepartmentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Remarks are required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Remarks must be between 5 and 100 characters")]
    [Display(Name = "Remarks")]
    public string Remarks { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    public DateTime Created { get; set; } = DateTime.Now;

    [Display(Name = "Modified Date")]
    public DateTime Modified { get; set; } = DateTime.Now;

    // Calculated properties
    [NotMapped]
    public int StaffCount { get; set; }

    [NotMapped]
    public int MeetingCount { get; set; }

    [NotMapped]
    public int ActiveStaffCount { get; set; }
}
```

### 4. MeetingTypeModel
```csharp
public class MeetingTypeModel
{
    [Key]
    public int MeetingTypeID { get; set; }

    [Required(ErrorMessage = "Meeting type name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Meeting type name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s&.-]+$", ErrorMessage = "Meeting type name contains invalid characters")]
    [Display(Name = "Meeting Type Name")]
    public string MeetingTypeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Remarks are required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Remarks must be between 5 and 100 characters")]
    [Display(Name = "Remarks")]
    public string Remarks { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    public DateTime Created { get; set; } = DateTime.Now;

    [Display(Name = "Modified Date")]
    public DateTime Modified { get; set; } = DateTime.Now;

    // Usage statistics
    [NotMapped]
    public int UsageCount { get; set; }
}
```

### 5. MeetingVenueModel
```csharp
public class MeetingVenueModel
{
    [Key]
    public int MeetingVenueID { get; set; }

    [Required(ErrorMessage = "Meeting venue name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Meeting venue name must be between 2 and 100 characters")]
    [Display(Name = "Meeting Venue Name")]
    public string MeetingVenueName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
    [Display(Name = "Capacity")]
    public int Capacity { get; set; }

    [Required(ErrorMessage = "Location is required")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Location must be between 5 and 200 characters")]
    [Display(Name = "Location")]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Created Date")]
    public DateTime Created { get; set; } = DateTime.Now;

    [Display(Name = "Modified Date")]
    public DateTime Modified { get; set; } = DateTime.Now;

    // Utilization statistics
    [NotMapped]
    public int BookingCount { get; set; }

    [NotMapped]
    public double UtilizationRate { get; set; }
}
```

### 6. MeetingMembersModel
```csharp
public class MeetingMembersModel
{
    [Key]
    public int MeetingMemberID { get; set; }

    [Required(ErrorMessage = "Meeting is required")]
    [Display(Name = "Meeting")]
    public int MeetingID { get; set; }

    [Required(ErrorMessage = "Staff member is required")]
    [Display(Name = "Staff Member")]
    public int StaffID { get; set; }

    [Display(Name = "Is Present")]
    public bool IsPresent { get; set; }

    [Display(Name = "Joined At")]
    public DateTime? JoinedAt { get; set; }

    [Display(Name = "Created Date")]
    public DateTime Created { get; set; } = DateTime.Now;

    [Display(Name = "Modified Date")]
    public DateTime Modified { get; set; } = DateTime.Now;

    // Navigation properties
    [NotMapped]
    [Display(Name = "Meeting Description")]
    public string MeetingDescription { get; set; } = string.Empty;

    [NotMapped]
    [Display(Name = "Meeting Date")]
    public DateTime MeetingDate { get; set; }

    [NotMapped]
    [Display(Name = "Staff Name")]
    public string StaffName { get; set; } = string.Empty;

    [NotMapped]
    [Display(Name = "Department Name")]
    public string DepartmentName { get; set; } = string.Empty;
}
```

## Validation Framework

### 1. Built-in Validation Attributes

#### Required Validation
```csharp
[Required(ErrorMessage = "Field is required")]
public string FieldName { get; set; }
```

#### String Length Validation
```csharp
[StringLength(100, MinimumLength = 2, ErrorMessage = "Field must be between 2 and 100 characters")]
public string FieldName { get; set; }
```

#### Range Validation
```csharp
[Range(1, 1000, ErrorMessage = "Value must be between 1 and 1000")]
public int NumericField { get; set; }
```

#### Email Validation
```csharp
[EmailAddress(ErrorMessage = "Please enter a valid email address")]
public string Email { get; set; }
```

#### Regular Expression Validation
```csharp
[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters and spaces allowed")]
public string Name { get; set; }
```

### 2. Data Type Validation
```csharp
[DataType(DataType.DateTime)]
public DateTime DateField { get; set; }

[DataType(DataType.EmailAddress)]
public string Email { get; set; }

[DataType(DataType.PhoneNumber)]
public string Phone { get; set; }

[DataType(DataType.Url)]
public string Website { get; set; }
```

### 3. Display Attributes
```csharp
[Display(Name = "Display Name")]
[Display(Name = "Field Name", Description = "Field description")]
[Display(Name = "Field", Prompt = "Enter value...")]
public string Field { get; set; }
```

## Custom Validation Attributes

### 1. FutureDateAttribute
```csharp
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

// Usage
[FutureDate(ErrorMessage = "Meeting date must be in the future")]
public DateTime MeetingDate { get; set; }
```

### 2. RequiredIfAttribute
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

// Usage
[RequiredIf("IsCancelled", true, ErrorMessage = "Cancellation reason is required when meeting is cancelled")]
public string? CancellationReason { get; set; }
```

### 3. UniqueValueAttribute (Custom Implementation)
```csharp
public class UniqueValueAttribute : ValidationAttribute
{
    private readonly string _tableName;
    private readonly string _columnName;

    public UniqueValueAttribute(string tableName, string columnName)
    {
        _tableName = tableName;
        _columnName = columnName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        var dataService = DataService.Instance;
        var table = GetTable(dataService, _tableName);
        
        if (table != null)
        {
            var existingValue = table.AsEnumerable()
                .Any(row => row.Field<string>(_columnName)?.Equals(value.ToString(), StringComparison.OrdinalIgnoreCase) == true);
            
            if (existingValue)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be unique");
            }
        }

        return ValidationResult.Success;
    }

    private DataTable? GetTable(DataService dataService, string tableName)
    {
        return tableName switch
        {
            "Departments" => dataService.Departments,
            "Staff" => dataService.Staff,
            "MeetingTypes" => dataService.MeetingTypes,
            "MeetingVenues" => dataService.MeetingVenues,
            _ => null
        };
    }
}

// Usage
[UniqueValue("Staff", "EmailAddress", ErrorMessage = "Email address must be unique")]
public string EmailAddress { get; set; }
```

## ViewModels

### 1. MeetingViewModel
```csharp
public class MeetingViewModel
{
    public MeetingModel Meeting { get; set; } = new MeetingModel();
    
    // Dropdown data
    public IEnumerable<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> MeetingTypes { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> MeetingVenues { get; set; } = new List<SelectListItem>();
    
    // Additional properties
    public bool IsEditMode => Meeting.MeetingID > 0;
    public string PageTitle => IsEditMode ? "Edit Meeting" : "Schedule Meeting";
    public string SubmitButtonText => IsEditMode ? "Update Meeting" : "Schedule Meeting";
    
    // File upload
    public IFormFile? DocumentUpload { get; set; }
    
    // Validation summary
    public List<string> ValidationErrors { get; set; } = new List<string>();
}
```

### 2. Dashboard ViewModel
```csharp
public class DashboardViewModel
{
    // Statistics
    public int TotalMeetings { get; set; }
    public int UpcomingMeetings { get; set; }
    public int TodayMeetings { get; set; }
    public int CancelledMeetings { get; set; }
    public int TotalStaff { get; set; }
    public int ActiveStaff { get; set; }
    public int TotalDepartments { get; set; }
    
    // Chart data
    public Dictionary<string, int> MeetingsByDepartment { get; set; } = new();
    public Dictionary<string, int> MeetingsByMonth { get; set; } = new();
    public Dictionary<string, double> AttendanceRates { get; set; } = new();
    
    // Recent activities
    public List<MeetingModel> RecentMeetings { get; set; } = new();
    public List<MeetingModel> UpcomingMeetingsData { get; set; } = new();
}
```

## Model Binding

### 1. Controller Model Binding
```csharp
[HttpPost]
public IActionResult MeetingAddEdit(MeetingModel model, IFormFile? upload)
{
    // Model binding automatically maps form data to model properties
    // Validation attributes are automatically applied
    
    if (!ModelState.IsValid)
    {
        // Return view with validation errors
        ViewBag.Departments = _dataService.Departments;
        ViewBag.MeetingTypes = _dataService.MeetingTypes;
        ViewBag.MeetingVenues = _dataService.MeetingVenues;
        return View(model);
    }
    
    // Process valid model
    // ...
}
```

### 2. Custom Model Binding
```csharp
public class MeetingModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType != typeof(MeetingModel))
        {
            return Task.CompletedTask;
        }

        var model = new MeetingModel();
        
        // Custom binding logic
        var meetingIdValue = bindingContext.ValueProvider.GetValue("MeetingID");
        if (meetingIdValue != ValueProviderResult.None)
        {
            if (int.TryParse(meetingIdValue.FirstValue, out int meetingId))
            {
                model.MeetingID = meetingId;
            }
        }

        // Set additional properties
        model.Modified = DateTime.Now;
        
        bindingContext.Result = ModelBindingResult.Successful(model);
        return Task.CompletedTask;
    }
}
```

## Validation Patterns

### 1. Server-Side Validation Pattern
```csharp
[HttpPost]
public IActionResult Create(MeetingModel model)
{
    // Remove validation for optional fields
    ModelState.Remove("DocumentPath");
    ModelState.Remove("CancellationReason");
    
    // Custom business validation
    if (model.MeetingDate <= DateTime.Now && model.MeetingID == 0)
    {
        ModelState.AddModelError("MeetingDate", "Meeting date must be in the future for new meetings.");
    }
    
    // Check for conflicts
    var hasConflict = CheckVenueConflict(model.MeetingVenueID, model.MeetingDate, model.MeetingID);
    if (hasConflict)
    {
        ModelState.AddModelError("", "A meeting is already scheduled at this venue within 2 hours.");
    }
    
    if (!ModelState.IsValid)
    {
        // Repopulate dropdown data
        PopulateViewBagData();
        return View(model);
    }
    
    // Process valid model
    SaveMeeting(model);
    TempData["SuccessMessage"] = "Meeting saved successfully!";
    return RedirectToAction("Index");
}
```

### 2. Client-Side Validation Pattern
```javascript
// Form validation rules
const validationRules = {
    'MeetingDate': {
        required: true,
        messages: {
            required: 'Meeting date and time is required'
        }
    },
    'DepartmentID': {
        required: true,
        messages: {
            required: 'Please select a department'
        }
    },
    'MeetingDescription': {
        maxLength: 250,
        messages: {
            maxLength: 'Description cannot exceed 250 characters'
        }
    },
    'CancellationReason': {
        required: false, // Optional field
        maxLength: 250,
        messages: {
            maxLength: 'Cancellation reason cannot exceed 250 characters'
        }
    }
};

// Initialize validation
modernUI.initializeFormValidation('#meetingForm', validationRules);
```

### 3. Conditional Validation Pattern
```csharp
public class ConditionalValidationModel
{
    public bool IsCancelled { get; set; }
    
    [RequiredIf("IsCancelled", true, ErrorMessage = "Cancellation date is required when meeting is cancelled")]
    public DateTime? CancellationDateTime { get; set; }
    
    // Optional even when cancelled
    public string? CancellationReason { get; set; }
}
```

## Error Handling

### 1. Validation Error Display
```html
<!-- Validation Summary -->
@if (!ViewData.ModelState.IsValid)
{
    <div class="alert alert-danger">
        <h6><i class="bi bi-exclamation-triangle me-1"></i>Please correct the following errors:</h6>
        <ul class="mb-0">
            @foreach (var error in ViewData.ModelState.Values.SelectMany(v => v.Errors))
            {
                <li>@error.ErrorMessage</li>
            }
        </ul>
    </div>
}

<!-- Field-specific validation -->
<input asp-for="MeetingDescription" class="form-control" />
<span asp-validation-for="MeetingDescription" class="text-danger"></span>
```

### 2. Custom Error Messages
```csharp
public class CustomErrorMessages
{
    public static class Meeting
    {
        public const string DateRequired = "Please select a meeting date and time";
        public const string DateInPast = "Meeting date cannot be in the past";
        public const string VenueRequired = "Please select a meeting venue";
        public const string VenueConflict = "Selected venue is not available at the specified time";
        public const string DescriptionTooLong = "Meeting description cannot exceed 250 characters";
    }
    
    public static class Staff
    {
        public const string NameRequired = "Staff name is required";
        public const string NameInvalid = "Staff name can only contain letters and spaces";
        public const string EmailRequired = "Email address is required";
        public const string EmailInvalid = "Please enter a valid email address";
        public const string EmailDuplicate = "This email address is already registered";
    }
}

// Usage
[Required(ErrorMessage = CustomErrorMessages.Meeting.DateRequired)]
public DateTime MeetingDate { get; set; }
```

### 3. Validation Result Pattern
```csharp
public class ValidationResult<T>
{
    public bool IsValid { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public Dictionary<string, List<string>> FieldErrors { get; set; } = new Dictionary<string, List<string>>();
    
    public static ValidationResult<T> Success(T data)
    {
        return new ValidationResult<T>
        {
            IsValid = true,
            Data = data
        };
    }
    
    public static ValidationResult<T> Failure(params string[] errors)
    {
        return new ValidationResult<T>
        {
            IsValid = false,
            Errors = errors.ToList()
        };
    }
    
    public void AddFieldError(string fieldName, string error)
    {
        if (!FieldErrors.ContainsKey(fieldName))
        {
            FieldErrors[fieldName] = new List<string>();
        }
        FieldErrors[fieldName].Add(error);
        IsValid = false;
    }
}

// Usage
public ValidationResult<MeetingModel> ValidateMeeting(MeetingModel model)
{
    var result = new ValidationResult<MeetingModel> { Data = model };
    
    if (model.MeetingDate <= DateTime.Now)
    {
        result.AddFieldError(nameof(model.MeetingDate), "Meeting date must be in the future");
    }
    
    if (string.IsNullOrEmpty(model.MeetingDescription))
    {
        result.AddFieldError(nameof(model.MeetingDescription), "Meeting description is required");
    }
    
    return result;
}
```

This comprehensive documentation covers all aspects of models and validation in the MOM system, including data annotations, custom validation attributes, ViewModels, model binding, validation patterns, and error handling strategies.