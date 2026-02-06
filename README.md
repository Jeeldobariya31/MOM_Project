# 📝 MOM (Minutes of Meeting) Management System

> A comprehensive web-based application for managing organizational meetings, attendance tracking, and meeting documentation built with ASP.NET Core MVC.

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-purple.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red.svg)](https://www.microsoft.com/en-us/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-purple.svg)](https://getbootstrap.com/)

---

## 📋 Table of Contents

- [🎯 Overview](#-overview)
- [✨ Features](#-features)
- [🛠️ Technology Stack](#️-technology-stack)
- [📁 Project Structure](#-project-structure)
- [🗄️ Database Design](#️-database-design)
- [🚀 Installation & Setup](#-installation--setup)
- [📖 Usage Guide](#-usage-guide)
- [👨‍💻 Author Information](#-author-information)
- [🎓 Project Guidance](#-project-guidance)
- [🐛 Troubleshooting](#-troubleshooting)
- [📄 License](#-license)

---

## 🎯 Overview

The **MOM (Minutes of Meeting) Management System** is a full-featured web application designed to streamline the process of scheduling, managing, and tracking organizational meetings. The system provides comprehensive functionality for managing departments, staff members, meeting venues, meeting types, and attendance tracking with document management capabilities.

### 🌟 Key Highlights

✅ Complete CRUD Operations for all entities  
✅ Advanced Search & Filtering with pagination  
✅ Meeting Attendance Tracking with bulk operations  
✅ Document Upload & Management (PDF, DOC, DOCX, XLS, XLSX)  
✅ Meeting Cancellation & Reactivation workflow  
✅ Responsive Modern UI with Bootstrap 5  
✅ Database-Driven Architecture with stored procedures  
✅ Real-time Data Synchronization  
✅ Conflict Detection for venue bookings  

---

## ✨ Features

### 1️⃣ Department Management
- ➕ Create, read, update, and delete departments
- 📊 Track department-wise staff and meetings
- 🔍 Department-based filtering and reporting

### 2️⃣ Staff Management
- 👥 Comprehensive staff information management
- 🏢 Department-wise staff organization
- 📞 Contact details (email, mobile) management
- 🔒 Unique email validation

### 3️⃣ Meeting Type Management
- 📋 Define various meeting types (Board Meeting, Client Meeting, Team Stand-up, etc.)
- 💬 Custom remarks for each meeting type
- 🏷️ Meeting type-based categorization

### 4️⃣ Meeting Venue Management
- 🏛️ Manage multiple meeting venues
- 📅 Venue availability tracking
- ⚠️ Conflict detection for venue bookings

### 5️⃣ Meeting Management
- 📆 Schedule meetings with date, time, venue, type, and department
- 📎 Upload meeting documents (PDF, DOC, DOCX, XLS, XLSX - Max 10MB)
- 📝 Meeting description and notes
- ❌ Meeting cancellation with reason tracking
- ♻️ Meeting reactivation capability
- 🔍 Advanced filtering (by department, type, venue, status, date range)
- 📄 Pagination for large datasets
- 🏷️ Meeting status tracking (Today, Upcoming, Past, Cancelled)

### 6️⃣ Meeting Members & Attendance
- 👤 Assign staff members to meetings
- ✅ Track attendance (Present/Absent)
- 🔄 Bulk attendance operations
- 💬 Member-wise remarks
- 📊 Attendance statistics and reporting

---

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 10.0 MVC
- **Language**: C# 12.0
- **Database**: Microsoft SQL Server 2019+
- **Data Access**: ADO.NET with System.Data.SqlClient 4.8.6
- **Architecture**: MVC Pattern, Repository Pattern, Singleton Pattern

### Frontend
- **UI Framework**: Bootstrap 5.3
- **JavaScript**: jQuery 3.6+, Vanilla JavaScript ES6+
- **Icons**: Bootstrap Icons
- **Design**: Mobile-first responsive design

### Database
- **RDBMS**: Microsoft SQL Server
- **Design**: Stored Procedures for all CRUD operations
- **Normalization**: 3NF (Third Normal Form)
- **Constraints**: Foreign keys, Unique, Check, Default values
- **Audit**: Created and Modified timestamps on all tables

---

## 📁 Project Structure

```
MOM-Project/
│
├── 📂 MOM/                                    # Main application folder
│   ├── 📂 Controllers/                        # MVC Controllers (9 controllers)
│   │   ├── AccountController.cs
│   │   ├── AuthController.cs
│   │   ├── DepartmentController.cs
│   │   ├── HomeController.cs
│   │   ├── MeetingController.cs
│   │   ├── MeetingMembersController.cs
│   │   ├── MeetingTypeController.cs
│   │   ├── MeetingVenueController.cs
│   │   └── StaffController.cs
│   │
│   ├── 📂 Models/                            # Data models (7 models)
│   │   ├── DepartmentModel.cs
│   │   ├── MeetingModel.cs
│   │   ├── MeetingMembersModel.cs
│   │   ├── MeetingTypeModel.cs
│   │   ├── MeetingVenueModel.cs
│   │   ├── StaffModel.cs
│   │   └── ViewModels/
│   │
│   ├── 📂 Services/                          # Business logic
│   │   └── DataService.cs
│   │
│   ├── 📂 Views/                             # Razor views (30+ views)
│   │   ├── Account/, Auth/, Department/
│   │   ├── Home/, Meeting/, MeetingMembers/
│   │   ├── MeetingType/, MeetingVenue/, Staff/
│   │   └── Shared/
│   │
│   ├── 📂 wwwroot/                          # Static files
│   │   ├── assets/ (css, js, img)
│   │   ├── lib/ (bootstrap, jquery)
│   │   └── uploads/meetings/
│   │
│   ├── Program.cs
│   ├── appsettings.json
│   └── MOM.csproj
│
├── 📄 DB_Design.sql                         # Database schema
├── 📄 Stored_Procedures.sql                 # All stored procedures (36+)
├── 📄 Static_Data_Insert.sql                # Sample data
└── 📄 README.md                             # This file
```

---

## 🗄️ Database Design

### Entity Relationship Diagram

```
MOM_Department (1) ──────< (N) MOM_Staff
       │
       └──────< (N) MOM_Meetings >────── (1) MOM_MeetingType
                      │                        (1) MOM_MeetingVenue
                      │
                      └──────< (N) MOM_MeetingMember >────── (1) MOM_Staff
```

### Database Tables

#### 1. MOM_Department
Stores organizational departments

| Column | Type | Description |
|--------|------|-------------|
| DepartmentID | INT (PK) | Unique identifier |
| DepartmentName | NVARCHAR(100) | Department name (Unique) |
| Created | DATETIME | Creation timestamp |
| Modified | DATETIME | Last modification timestamp |

#### 2. MOM_MeetingType
Defines meeting categories

| Column | Type | Description |
|--------|------|-------------|
| MeetingTypeID | INT (PK) | Unique identifier |
| MeetingTypeName | NVARCHAR(100) | Type name (Unique) |
| Remarks | NVARCHAR(100) | Additional notes |
| Created | DATETIME | Creation timestamp |
| Modified | DATETIME | Last modification timestamp |

#### 3. MOM_MeetingVenue
Manages meeting locations

| Column | Type | Description |
|--------|------|-------------|
| MeetingVenueID | INT (PK) | Unique identifier |
| MeetingVenueName | NVARCHAR(100) | Venue name (Unique) |
| Created | DATETIME | Creation timestamp |
| Modified | DATETIME | Last modification timestamp |

#### 4. MOM_Staff
Employee information

| Column | Type | Description |
|--------|------|-------------|
| StaffID | INT (PK) | Unique identifier |
| DepartmentID | INT (FK) | Reference to department |
| StaffName | NVARCHAR(50) | Staff member name |
| MobileNo | NVARCHAR(20) | Contact number |
| EmailAddress | NVARCHAR(50) | Email (Unique) |
| Remarks | NVARCHAR(250) | Additional notes |
| Created | DATETIME | Creation timestamp |
| Modified | DATETIME | Last modification timestamp |

#### 5. MOM_Meetings
Core meeting information

| Column | Type | Description |
|--------|------|-------------|
| MeetingID | INT (PK) | Unique identifier |
| MeetingDate | DATETIME | Meeting date and time |
| MeetingVenueID | INT (FK) | Reference to venue |
| MeetingTypeID | INT (FK) | Reference to meeting type |
| DepartmentID | INT (FK) | Reference to department |
| MeetingDescription | NVARCHAR(250) | Meeting details |
| DocumentPath | NVARCHAR(250) | Uploaded document path |
| IsCancelled | BIT | Cancellation status |
| CancellationDateTime | DATETIME | When cancelled |
| CancellationReason | NVARCHAR(250) | Cancellation reason |
| Created | DATETIME | Creation timestamp |
| Modified | DATETIME | Last modification timestamp |

#### 6. MOM_MeetingMember
Tracks meeting attendance

| Column | Type | Description |
|--------|------|-------------|
| MeetingMemberID | INT (PK) | Unique identifier |
| MeetingID | INT (FK) | Reference to meeting |
| StaffID | INT (FK) | Reference to staff |
| IsPresent | BIT | Attendance status |
| Remarks | NVARCHAR(250) | Attendance notes |
| Created | DATETIME | Creation timestamp |
| Modified | DATETIME | Last modification timestamp |

**Unique Constraint**: (MeetingID, StaffID) - Prevents duplicate assignments

### Stored Procedures

The application uses **36+ stored procedures** following the naming pattern: `PR_[TableName]_[Operation]`

**Operations for each entity:**
- `SelectAll` - Retrieve all records
- `SelectByPK` - Get single record by ID
- `Insert` - Add new record
- `UpdateByPK` - Update existing record
- `DeleteByPK` - Delete record

---

## 🚀 Installation & Setup

### Prerequisites

| Software | Version | Download |
|----------|---------|----------|
| Visual Studio | 2022+ | [Download](https://visualstudio.microsoft.com/) |
| .NET SDK | 10.0+ | [Download](https://dotnet.microsoft.com/download) |
| SQL Server | 2019+ | [Download](https://www.microsoft.com/sql-server) |
| Git | Latest | [Download](https://git-scm.com/) |

### Step 1: Clone Repository

```bash
git clone https://github.com/Jeeldobaria31/MOM-Project.git
cd MOM-Project
```

### Step 2: Database Setup

Execute SQL scripts in order using SSMS:

1. **DB_Design.sql** - Creates database and tables
2. **Stored_Procedures.sql** - Creates all stored procedures
3. **Static_Data_Insert.sql** - Inserts sample data (optional)
4. **Update_Meeting_Procedures.sql** - Additional procedures

### Step 3: Configure Connection String

Update `MOM/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=MOM_Project;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

**Connection String Examples:**

```
Windows Auth: Server=localhost;Database=MOM_Project;Trusted_Connection=true;TrustServerCertificate=true
SQL Auth: Server=localhost;Database=MOM_Project;User Id=sa;Password=YourPass;TrustServerCertificate=true
Named Instance: Server=localhost\\SQLEXPRESS;Database=MOM_Project;Trusted_Connection=true;TrustServerCertificate=true
```

### Step 4: Build and Run

**Using Visual Studio:**
1. Open `MOM.sln`
2. Press `Ctrl + Shift + B` to build
3. Press `F5` to run

**Using Command Line:**
```bash
cd MOM
dotnet restore
dotnet build
dotnet run
```

Application will launch at: `https://localhost:5001`

---

## 📖 Usage Guide

### Creating a New Meeting

1. Navigate to **Meetings** → **Meeting List**
2. Click **➕ Add New Meeting**
3. Fill in the form:
   - Select meeting date & time
   - Choose department, type, and venue
   - Add description
   - Upload document (optional, max 10MB)
4. Click **💾 Save**

### Assigning Members to Meeting

1. Go to **Meeting Members** → **Member List**
2. Click **➕ Add New Member**
3. Select meeting and staff member
4. Set attendance status
5. Click **💾 Save**

### Tracking Attendance

1. Navigate to **Meeting Members** → **Member List**
2. Filter by meeting
3. Click **✏️ Edit** on member record
4. Update **Is Present** checkbox
5. Click **🔄 Update**

### Cancelling a Meeting

1. Go to **Meetings** → **Meeting List**
2. Find the meeting
3. Click **🚫 Cancel** button
4. Enter cancellation reason
5. Confirm

### Search & Filter

- Use search box for text search
- Apply filters: Department, Type, Venue, Status
- Select date range
- Click **🔍 Search**

---

## 👨‍💻 Author Information

**Jeel Dobariya**

- 🎓 **Program**: B.Tech in Computer Science and Engineering
- 📚 **Semester**: 4th Semester
- 🏛️ **University**: Darshan University, Rajkot, Gujarat, India
- 📅 **Academic Year**: 2024-2026

### 🌐 Connect With Me

- 📧 **Email**: [jeeldobariya33@gmail.com](mailto:jeeldobariya33@gmail.com)
- 🌐 **Portfolio**: [jeel-dobariya.vercel.app](https://jeel-dobariya.vercel.app)
- 💻 **GitHub**: [github.com/Jeeldobaria31](https://github.com/Jeeldobaria31)

**For more information about my skills and projects, visit my portfolio website.**

---

## 🎓 Project Guidance

This project was completed under the guidance of faculty members and teaching assistants from **Darshan University, Department of Computer Science and Engineering**.

### Faculty Supervisors

- **Prof. Naimish Vadodariya** - Project Guide & Supervisor
- **Prof. Madhuresh Fichdiya** - Project Evaluator
- **Prof. Sejal Gupta** - Project Evaluator

### Teaching Assistants

- **Dhairya Adroja** - Technical support and debugging
- **Shreyansh Ranpariya** - Code review and best practices
- **Harpalsinh Sindhav** - Database optimization and testing

### Acknowledgments

Special thanks to:
- 🏛️ **Darshan University** for infrastructure and resources
- 💻 **Department of CSE** for academic support
- 👨‍🏫 **Faculty & TAs** for continuous guidance
- 👥 **Fellow Students** for collaboration and peer learning

---

## 🐛 Troubleshooting

### Database Connection Failed

**Solutions:**
1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Enable TCP/IP protocol in SQL Configuration Manager
4. Check firewall settings

### Stored Procedure Not Found

**Solutions:**
1. Verify all SQL scripts were executed
2. Check database name in connection string
3. Refresh database connection

### File Upload Failed

**Solutions:**
1. Check `wwwroot/uploads/meetings/` folder exists
2. Verify folder write permissions
3. Check file size (max 10MB)
4. Verify file extension (PDF, DOC, DOCX, XLS, XLSX)

### Session Timeout

**Solutions:**
1. Increase session timeout in `Program.cs`
2. Enable cookies in browser
3. Clear browser cache

---

## 📄 License

This project is developed for **educational purposes** as part of the academic curriculum at **Darshan University**.

**Copyright © 2026 Jeel Dobariya**  
**Darshan University, Department of Computer Science and Engineering**

### Usage Terms

**Permitted:**
- ✅ Educational and learning purposes
- ✅ Academic reference with proper citation
- ✅ Personal learning and skill development
- ✅ Portfolio display with attribution

**Prohibited:**
- ❌ Commercial use without permission
- ❌ Redistribution without attribution
- ❌ Claiming as original work
- ❌ Academic dishonesty

### Citation

```
Dobariya, J. (2026). MOM (Minutes of Meeting) Management System. 
Darshan University, Department of Computer Science and Engineering.
https://github.com/Jeeldobaria31/MOM-Project
```

---

## 📊 Project Statistics

- 📝 **Lines of Code**: ~15,000+
- 🎮 **Controllers**: 9
- 📦 **Models**: 7
- 👁️ **Views**: 30+
- 🗄️ **Database Tables**: 6
- 🔧 **Stored Procedures**: 36+
- ⏱️ **Development Time**: 3 months

---

## 📞 Contact & Support

For questions or support:

- 📧 **Email**: [jeeldobariya33@gmail.com](mailto:jeeldobariya33@gmail.com)
- 🌐 **Portfolio**: [jeel-dobariya.vercel.app](https://jeel-dobariya.vercel.app)
- 💻 **GitHub**: [github.com/Jeeldobaria31](https://github.com/Jeeldobaria31)
- 🐛 **Issues**: [GitHub Issues](https://github.com/Jeeldobaria31/MOM-Project/issues)

---

<div align="center">

### 💻 Made with ❤️ by Jeel Dobariya

**Darshan University | Computer Science and Engineering | Semester 4**

**© 2026 All Rights Reserved**

---

[![Portfolio](https://img.shields.io/badge/Portfolio-Visit-blue?style=for-the-badge&logo=vercel)](https://jeel-dobariya.vercel.app)
[![GitHub](https://img.shields.io/badge/GitHub-Follow-black?style=for-the-badge&logo=github)](https://github.com/Jeeldobaria31)
[![Email](https://img.shields.io/badge/Email-Contact-red?style=for-the-badge&logo=gmail)](mailto:jeeldobariya33@gmail.com)

---


</div>
