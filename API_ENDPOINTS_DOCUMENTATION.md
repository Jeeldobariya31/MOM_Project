# API Endpoints Documentation - MOM System

## Table of Contents
1. [API Architecture](#api-architecture)
2. [Meeting Endpoints](#meeting-endpoints)
3. [Staff Endpoints](#staff-endpoints)
4. [Department Endpoints](#department-endpoints)
5. [Meeting Type Endpoints](#meeting-type-endpoints)
6. [Meeting Venue Endpoints](#meeting-venue-endpoints)
7. [Meeting Members Endpoints](#meeting-members-endpoints)
8. [Authentication Endpoints](#authentication-endpoints)
9. [Common Response Patterns](#common-response-patterns)
10. [Error Handling](#error-handling)

## API Architecture

### RESTful Design Principles
The MOM system follows RESTful API design principles with the following conventions:

- **GET**: Retrieve data (List views, Details)
- **POST**: Create new resources or perform actions
- **PUT**: Update existing resources (not implemented in current version)
- **DELETE**: Remove resources

### Response Format
All API endpoints return JSON responses with a consistent structure:

```json
{
    "success": true/false,
    "message": "Status message",
    "data": { /* Response data */ }
}
```

### Authentication & Security
- **CSRF Protection**: Anti-forgery tokens on all POST requests
- **Model Validation**: Server-side validation with detailed error messages
- **Input Sanitization**: XSS protection and data validation

## Meeting Endpoints

### 1. Get Meeting List
**Endpoint:** `GET /Meeting/MeetingList`

**Parameters:**
- `search` (string, optional): Search term for meeting description
- `departmentFilter` (int, optional): Filter by department ID
- `typeFilter` (int, optional): Filter by meeting type ID
- `venueFilter` (int, optional): Filter by venue ID
- `statusFilter` (string, optional): Filter by status (Upcoming, Today, Past, Cancelled)
- `dateFrom` (DateTime, optional): Start date filter
- `dateTo` (DateTime, optional): End date filter
- `page` (int, default: 1): Page number for pagination
- `pageSize` (int, default: 10): Number of items per page

**Response:** HTML view with filtered and paginated meeting list

**Example Request:**
```
GET /Meeting/MeetingList?search=review&departmentFilter=1&page=1&pageSize=10
```

### 2. Get Meeting Details (AJAX)
**Endpoint:** `GET /Meeting/GetMeetingDetails`

**Parameters:**
- `id` (int, required): Meeting ID

**Response:**
```json
{
    "success": true,
    "data": {
        "MeetingID": 1,
        "MeetingDescription": "Quarterly Business Review",
        "MeetingDate": "15/03/2024 10:00 AM",
        "DepartmentName": "IT Department",
        "MeetingTypeName": "Board Meeting",
        "MeetingVenueName": "Conference Room A",
        "Status": "Upcoming",
        "StatusClass": "success",
        "MemberCount": 5,
        "PresentCount": 0,
        "IsCancelled": false,
        "CancellationReason": null,
        "DocumentPath": "/uploads/meetings/20240315_100000_abc123.pdf",
        "Created": "10/03/2024 09:00 AM",
        "Modified": "12/03/2024 02:30 PM"
    }
}
```

**Error Response:**
```json
{
    "success": false,
    "message": "Meeting not found."
}
```

### 3. Create/Update Meeting
**Endpoint:** `GET /Meeting/MeetingAddEdit` (Form Display)
**Endpoint:** `POST /Meeting/MeetingAddEdit` (Form Submission)

**GET Parameters:**
- `id` (int, optional): Meeting ID for editing (omit for new meeting)

**POST Parameters:**
- `MeetingID` (int): Meeting ID (0 for new meeting)
- `MeetingDate` (DateTime, required): Meeting date and time
- `DepartmentID` (int, required): Department ID
- `MeetingTypeID` (int, required): Meeting type ID
- `MeetingVenueID` (int, required): Meeting venue ID
- `MeetingDescription` (string, optional): Meeting description (max 250 chars)
- `IsCancelled` (bool): Cancellation status
- `CancellationDateTime` (DateTime, optional): Cancellation date/time
- `CancellationReason` (string, optional): Cancellation reason (max 250 chars)
- `upload` (IFormFile, optional): Document file upload

**File Upload Constraints:**
- **Allowed Extensions**: .pdf, .doc, .docx, .xls, .xlsx
- **Maximum Size**: 10MB
- **Storage Location**: `/wwwroot/uploads/meetings/`

**Validation Rules:**
- Meeting date must be in the future (for new meetings)
- No overlapping meetings at same venue within 2 hours
- All required fields must be provided
- File type and size validation

**Success Response:** Redirect to `/Meeting/MeetingList` with success message

**Error Response:** Return to form with validation errors

### 4. Cancel Meeting (AJAX)
**Endpoint:** `POST /Meeting/CancelMeeting`

**Parameters:**
- `id` (int, required): Meeting ID
- `reason` (string, optional): Cancellation reason (max 250 chars)

**Request Example:**
```javascript
const formData = new FormData();
formData.append('id', 123);
formData.append('reason', 'Schedule conflict with client meeting');

fetch('/Meeting/CancelMeeting', {
    method: 'POST',
    headers: {
        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
    },
    body: formData
})
```

**Success Response:**
```json
{
    "success": true,
    "message": "Meeting cancelled successfully!"
}
```

**Error Response:**
```json
{
    "success": false,
    "message": "Meeting not found."
}
```

### 5. Delete Meeting (AJAX)
**Endpoint:** `POST /Meeting/Delete`

**Parameters:**
- `id` (int, required): Meeting ID

**Business Rules:**
- Cannot delete meetings with assigned members
- Must remove all members before deletion

**Success Response:**
```json
{
    "success": true,
    "message": "Meeting deleted successfully!"
}
```

**Error Response:**
```json
{
    "success": false,
    "message": "Cannot delete meeting with assigned members. Please remove members first."
}
```

## Staff Endpoints

### 1. Get Staff List
**Endpoint:** `GET /Staff/StaffList`

**Parameters:**
- `search` (string, optional): Search term for staff name or email
- `departmentFilter` (int, optional): Filter by department ID
- `statusFilter` (string, optional): Filter by status (Active, Inactive)
- `page` (int, default: 1): Page number for pagination

**Features:**
- Advanced search and filtering
- Department-wise grouping
- Active/Inactive status filtering
- Pagination support

### 2. Create/Update Staff
**Endpoint:** `GET /Staff/StaffAddEdit` (Form Display)
**Endpoint:** `POST /Staff/StaffAddEdit` (Form Submission)

**POST Parameters:**
- `StaffID` (int): Staff ID (0 for new staff)
- `StaffName` (string, required): Staff name (2-50 chars)
- `EmailAddress` (string, required): Email address (max 50 chars)
- `MobileNo` (string, required): Mobile number (10-20 chars)
- `DepartmentID` (int, required): Department ID
- `IsActive` (bool): Active status

**Validation Rules:**
- Staff name: 2-50 characters, letters and spaces only
- Email: Valid email format, unique within system
- Mobile: 10-20 digits, unique within system
- Department must exist

### 3. Delete Staff (AJAX)
**Endpoint:** `POST /Staff/Delete`

**Parameters:**
- `id` (int, required): Staff ID

**Business Rules:**
- Cannot delete staff assigned to meetings
- Must remove from all meetings before deletion

### 4. Toggle Staff Status (AJAX)
**Endpoint:** `POST /Staff/ToggleStatus`

**Parameters:**
- `id` (int, required): Staff ID

**Response:**
```json
{
    "success": true,
    "message": "Staff status updated successfully!",
    "newStatus": "Active"
}
```

## Department Endpoints

### 1. Get Department List
**Endpoint:** `GET /Department/DepartmentList`

**Parameters:**
- `search` (string, optional): Search term for department name
- `page` (int, default: 1): Page number for pagination

**Features:**
- Search functionality
- Staff count per department
- Meeting count statistics
- Creation and modification timestamps

### 2. Create/Update Department
**Endpoint:** `GET /Department/DepartmentAddEdit` (Form Display)
**Endpoint:** `POST /Department/DepartmentAddEdit` (Form Submission)

**POST Parameters:**
- `DepartmentID` (int): Department ID (0 for new department)
- `DepartmentName` (string, required): Department name (2-100 chars)
- `Remarks` (string, required): Department remarks (5-100 chars)

**Validation Rules:**
- Department name: 2-100 characters, unique within system
- Remarks: 5-100 characters, descriptive text

### 3. Delete Department (AJAX)
**Endpoint:** `POST /Department/Delete`

**Parameters:**
- `id` (int, required): Department ID

**Business Rules:**
- Cannot delete departments with active staff
- Cannot delete departments with scheduled meetings
- Must be empty before deletion

## Meeting Type Endpoints

### 1. Get Meeting Type List
**Endpoint:** `GET /MeetingType/MeetingTypeList`

**Parameters:**
- `search` (string, optional): Search term for meeting type name
- `page` (int, default: 1): Page number for pagination

**Features:**
- Search functionality
- Usage statistics (number of meetings per type)
- Creation and modification tracking

### 2. Create/Update Meeting Type
**Endpoint:** `GET /MeetingType/MeetingTypeAddEdit` (Form Display)
**Endpoint:** `POST /MeetingType/MeetingTypeAddEdit` (Form Submission)

**POST Parameters:**
- `MeetingTypeID` (int): Meeting type ID (0 for new type)
- `MeetingTypeName` (string, required): Meeting type name (2-100 chars)
- `Remarks` (string, required): Meeting type remarks (5-100 chars)

**Validation Rules:**
- Meeting type name: 2-100 characters, unique within system
- Remarks: 5-100 characters, descriptive text

### 3. Delete Meeting Type (AJAX)
**Endpoint:** `POST /MeetingType/Delete`

**Parameters:**
- `id` (int, required): Meeting type ID

**Business Rules:**
- Cannot delete meeting types used in scheduled meetings
- Must not be referenced by any meetings

## Meeting Venue Endpoints

### 1. Get Meeting Venue List
**Endpoint:** `GET /MeetingVenue/MeetingVenueList`

**Parameters:**
- `search` (string, optional): Search term for venue name or location
- `page` (int, default: 1): Page number for pagination

**Features:**
- Search by name or location
- Capacity information
- Utilization statistics
- Location details

### 2. Create/Update Meeting Venue
**Endpoint:** `GET /MeetingVenue/MeetingVenueAddEdit` (Form Display)
**Endpoint:** `POST /MeetingVenue/MeetingVenueAddEdit` (Form Submission)

**POST Parameters:**
- `MeetingVenueID` (int): Meeting venue ID (0 for new venue)
- `MeetingVenueName` (string, required): Venue name (2-100 chars)
- `Capacity` (int, required): Venue capacity (1-1000)
- `Location` (string, required): Venue location (5-200 chars)

**Validation Rules:**
- Venue name: 2-100 characters, unique within system
- Capacity: 1-1000 people
- Location: 5-200 characters, descriptive address

### 3. Delete Meeting Venue (AJAX)
**Endpoint:** `POST /MeetingVenue/Delete`

**Parameters:**
- `id` (int, required): Meeting venue ID

**Business Rules:**
- Cannot delete venues with scheduled meetings
- Must not be referenced by any meetings

## Meeting Members Endpoints

### 1. Get Meeting Members List
**Endpoint:** `GET /MeetingMembers/MeetingMemberList`

**Parameters:**
- `meetingId` (int, optional): Filter by specific meeting
- `staffId` (int, optional): Filter by specific staff member
- `page` (int, default: 1): Page number for pagination

**Features:**
- Meeting-specific member listing
- Staff-specific meeting history
- Attendance tracking
- Join time recording

### 2. Create/Update Meeting Member
**Endpoint:** `GET /MeetingMembers/MeetingMemberAddEdit` (Form Display)
**Endpoint:** `POST /MeetingMembers/MeetingMemberAddEdit` (Form Submission)

**POST Parameters:**
- `MeetingMemberID` (int): Meeting member ID (0 for new assignment)
- `MeetingID` (int, required): Meeting ID
- `StaffID` (int, required): Staff ID
- `IsPresent` (bool): Attendance status
- `JoinedAt` (DateTime, optional): Join time

**Validation Rules:**
- Meeting must exist and not be cancelled
- Staff must be active
- No duplicate assignments (same staff to same meeting)

### 3. Delete Meeting Member (AJAX)
**Endpoint:** `POST /MeetingMembers/Delete`

**Parameters:**
- `id` (int, required): Meeting member ID

**Response:**
```json
{
    "success": true,
    "message": "Member removed from meeting successfully!"
}
```

### 4. Toggle Attendance (AJAX)
**Endpoint:** `POST /MeetingMembers/ToggleAttendance`

**Parameters:**
- `id` (int, required): Meeting member ID

**Response:**
```json
{
    "success": true,
    "message": "Attendance status updated successfully!",
    "newStatus": "Present"
}
```

## Authentication Endpoints

### 1. Login
**Endpoint:** `GET /Auth/Login` (Login Form)
**Endpoint:** `POST /Auth/Login` (Login Submission)

**POST Parameters:**
- `Username` (string, required): User username
- `Password` (string, required): User password
- `RememberMe` (bool, optional): Remember login session

**Success Response:** Redirect to dashboard
**Error Response:** Return to login form with error message

### 2. Logout
**Endpoint:** `POST /Auth/Logout`

**Response:** Redirect to login page with session cleared

## Common Response Patterns

### 1. Success Response Pattern
```json
{
    "success": true,
    "message": "Operation completed successfully!",
    "data": {
        // Response data object
    }
}
```

### 2. Error Response Pattern
```json
{
    "success": false,
    "message": "Error description",
    "errors": [
        "Specific error 1",
        "Specific error 2"
    ]
}
```

### 3. Validation Error Response
```json
{
    "success": false,
    "message": "Validation failed",
    "errors": {
        "FieldName": ["Field-specific error message"],
        "AnotherField": ["Another field error"]
    }
}
```

### 4. Pagination Response Pattern
```json
{
    "success": true,
    "data": {
        "items": [ /* Array of items */ ],
        "pagination": {
            "currentPage": 1,
            "totalPages": 5,
            "totalRecords": 47,
            "pageSize": 10,
            "hasNextPage": true,
            "hasPreviousPage": false
        }
    }
}
```

## Error Handling

### 1. HTTP Status Codes
- **200 OK**: Successful GET requests
- **302 Found**: Successful POST requests (redirect)
- **400 Bad Request**: Validation errors
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server-side errors

### 2. Client-Side Error Handling
```javascript
fetch('/Meeting/Delete', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
        'RequestVerificationToken': getAntiForgeryToken()
    },
    body: `id=${meetingId}`
})
.then(response => {
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    }
    return response.json();
})
.then(data => {
    if (data.success) {
        modernUI.showToast(data.message, 'success');
        // Handle success
    } else {
        modernUI.showToast(data.message, 'error');
        // Handle business logic error
    }
})
.catch(error => {
    console.error('Network error:', error);
    modernUI.showToast('Network error occurred', 'error');
});
```

### 3. Server-Side Error Handling
```csharp
[HttpPost]
public IActionResult Delete(int id)
{
    try
    {
        var result = _dataService.SafeDeleteMeeting(id);
        
        return Json(new { 
            success = result.Success, 
            message = result.Message 
        });
    }
    catch (Exception ex)
    {
        // Log error
        Console.WriteLine($"Error deleting meeting {id}: {ex.Message}");
        
        return Json(new { 
            success = false, 
            message = "An unexpected error occurred. Please try again." 
        });
    }
}
```

### 4. CSRF Token Handling
```javascript
function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
}

// Usage in AJAX requests
fetch('/Meeting/CancelMeeting', {
    method: 'POST',
    headers: {
        'RequestVerificationToken': getAntiForgeryToken()
    },
    body: formData
})
```

This comprehensive API documentation covers all endpoints in the MOM system, including request/response formats, validation rules, business logic constraints, and error handling patterns.