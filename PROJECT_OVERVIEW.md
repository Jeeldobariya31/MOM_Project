# MOM (Minutes of Meeting) System - Project Overview

## Table of Contents
1. [Project Introduction](#project-introduction)
2. [System Architecture](#system-architecture)
3. [Technology Stack](#technology-stack)
4. [Project Structure](#project-structure)
5. [Key Features](#key-features)
6. [Database Design](#database-design)
7. [Security Features](#security-features)
8. [User Interface Design](#user-interface-design)

## Project Introduction

The **MOM (Minutes of Meeting) System** is a comprehensive web application built using ASP.NET Core MVC that manages organizational meetings, participants, and related documentation. The system provides a complete solution for scheduling, managing, and tracking meetings across different departments.

### Project Objectives
- **Meeting Management**: Schedule, edit, cancel, and track meetings
- **Participant Management**: Manage staff members and meeting attendees
- **Department Organization**: Organize meetings by departments
- **Document Management**: Upload and manage meeting-related documents
- **Reporting & Analytics**: Generate insights and statistics about meetings
- **User-Friendly Interface**: Modern, responsive UI with interactive components

## System Architecture

### Architecture Pattern: MVC (Model-View-Controller)
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│     VIEW        │    │   CONTROLLER    │    │     MODEL       │
│                 │    │                 │    │                 │
│ - Razor Pages   │◄──►│ - HTTP Requests │◄──►│ - Data Models   │
│ - HTML/CSS/JS   │    │ - Business Logic│    │ - Validation    │
│ - User Interface│    │ - Data Flow     │    │ - Data Access   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │
                                ▼
                       ┌─────────────────┐
                       │    SERVICES     │
                       │                 │
                       │ - DataService   │
                       │ - Business Logic│
                       │ - Data Storage  │
                       └─────────────────┘
```

### Layer Architecture
1. **Presentation Layer** (Views)
   - Razor Views (.cshtml files)
   - JavaScript (modern-ui.js, main.js)
   - CSS Styling (Bootstrap + Custom CSS)

2. **Business Logic Layer** (Controllers)
   - MVC Controllers
   - Action Methods
   - Request/Response Handling

3. **Data Access Layer** (Services)
   - DataService (Singleton Pattern)
   - In-Memory Data Storage
   - CRUD Operations

4. **Model Layer**
   - Data Models with Validation
   - ViewModels for UI
   - Business Entities

## Technology Stack

### Backend Technologies
- **Framework**: ASP.NET Core 10.0 MVC
- **Language**: C# 12.0
- **Architecture**: Model-View-Controller (MVC)
- **Data Storage**: In-Memory DataTables (Singleton Pattern)
- **Validation**: Data Annotations + Custom Validators

### Frontend Technologies
- **UI Framework**: Bootstrap 5.3
- **JavaScript**: Vanilla JS + Custom ModernUI Framework
- **CSS**: Custom CSS + Bootstrap
- **Icons**: Bootstrap Icons
- **Charts**: ApexCharts, ECharts
- **Responsive Design**: Mobile-First Approach

### Development Tools
- **IDE**: Visual Studio / VS Code
- **Version Control**: Git
- **Package Manager**: NuGet
- **Build System**: .NET CLI

## Project Structure

```
MOM/
├── Controllers/           # MVC Controllers
│   ├── HomeController.cs
│   ├── MeetingController.cs
│   ├── StaffController.cs
│   ├── DepartmentController.cs
│   ├── MeetingTypeController.cs
│   ├── MeetingVenueController.cs
│   ├── MeetingMembersController.cs
│   ├── AccountController.cs
│   └── AuthController.cs
├── Models/               # Data Models
│   ├── MeetingModel.cs
│   ├── StaffModel.cs
│   ├── DepartmentModel.cs
│   ├── MeetingTypeModel.cs
│   ├── MeetingVenueModel.cs
│   ├── MeetingMembersModel.cs
│   └── ViewModels/
├── Views/                # Razor Views
│   ├── Home/
│   ├── Meeting/
│   ├── Staff/
│   ├── Department/
│   ├── MeetingType/
│   ├── MeetingVenue/
│   ├── MeetingMembers/
│   ├── Account/
│   ├── Auth/
│   └── Shared/
├── Services/             # Business Services
│   └── DataService.cs
├── wwwroot/             # Static Files
│   ├── assets/
│   │   ├── css/
│   │   ├── js/
│   │   ├── img/
│   │   └── docs/
│   ├── uploads/
│   └── lib/
├── Properties/
├── Program.cs           # Application Entry Point
└── MOM.csproj          # Project Configuration
```

## Key Features

### 1. Meeting Management
- **Schedule Meetings**: Create new meetings with date, time, venue, type
- **Edit Meetings**: Modify meeting details, participants, documents
- **Cancel Meetings**: Cancel meetings with optional reasons
- **Meeting Status**: Track upcoming, ongoing, past, and cancelled meetings
- **Document Upload**: Attach files (PDF, DOC, DOCX, XLS, XLSX)

### 2. Participant Management
- **Staff Management**: Add, edit, delete staff members
- **Meeting Members**: Assign staff to meetings
- **Attendance Tracking**: Mark present/absent for meetings
- **Department-wise Organization**: Group staff by departments

### 3. Master Data Management
- **Departments**: Manage organizational departments
- **Meeting Types**: Define different types of meetings
- **Meeting Venues**: Manage meeting locations and capacities

### 4. Dashboard & Analytics
- **Interactive Dashboard**: Real-time statistics and charts
- **Meeting Analytics**: Attendance rates, department-wise meetings
- **Visual Reports**: Charts showing meeting trends and patterns
- **Quick Stats**: Total meetings, upcoming meetings, attendance rates

### 5. Advanced Features
- **Search & Filter**: Advanced filtering by date, department, type, status
- **Pagination**: Efficient data loading with pagination
- **Export Functionality**: Export data to CSV/JSON formats
- **Responsive Design**: Works on desktop, tablet, and mobile devices
- **Modern UI**: Interactive components with animations and transitions

## Database Design

### Entity Relationship Overview
```
┌─────────────┐    ┌─────────────────┐    ┌─────────────┐
│ Department  │    │    Meeting      │    │ MeetingType │
│             │◄──►│                 │◄──►│             │
│ - ID        │    │ - ID            │    │ - ID        │
│ - Name      │    │ - Date          │    │ - Name      │
│ - Remarks   │    │ - Description   │    │ - Remarks   │
└─────────────┘    │ - DocumentPath  │    └─────────────┘
                   │ - IsCancelled   │
┌─────────────┐    │ - CancelReason  │    ┌─────────────┐
│MeetingVenue │    │ - Created       │    │    Staff    │
│             │◄──►│ - Modified      │    │             │
│ - ID        │    └─────────────────┘    │ - ID        │
│ - Name      │           │               │ - Name      │
│ - Capacity  │           │               │ - Email     │
│ - Location  │           ▼               │ - Mobile    │
└─────────────┘    ┌─────────────────┐    │ - Department│
                   │ MeetingMembers  │◄──►│ - IsActive  │
                   │                 │    └─────────────┘
                   │ - MeetingID     │
                   │ - StaffID       │
                   │ - IsPresent     │
                   │ - JoinedAt      │
                   └─────────────────┘
```

### Data Storage Implementation
- **In-Memory DataTables**: Using System.Data.DataTable for data storage
- **Singleton Pattern**: DataService ensures single instance across application
- **CRUD Operations**: Full Create, Read, Update, Delete functionality
- **Data Relationships**: Foreign key relationships maintained programmatically

## Security Features

### 1. Input Validation
- **Server-Side Validation**: Data Annotations on models
- **Client-Side Validation**: JavaScript validation with modern-ui.js
- **XSS Protection**: HTML encoding and sanitization
- **File Upload Security**: File type and size validation

### 2. CSRF Protection
- **Anti-Forgery Tokens**: Implemented on all POST requests
- **Request Verification**: Automatic token validation

### 3. Data Validation
- **Model Validation**: Comprehensive validation rules
- **Custom Validators**: Business-specific validation logic
- **Error Handling**: Graceful error handling and user feedback

## User Interface Design

### Design Principles
1. **Modern & Clean**: Contemporary design with clean layouts
2. **Responsive**: Mobile-first responsive design
3. **Intuitive**: User-friendly navigation and interactions
4. **Accessible**: WCAG compliance considerations
5. **Performance**: Optimized loading and interactions

### UI Components
- **Custom Modal System**: Interactive modals for actions
- **Toast Notifications**: User feedback system
- **Data Tables**: Advanced tables with sorting, filtering, pagination
- **Form Validation**: Real-time validation with visual feedback
- **Charts & Graphs**: Interactive data visualization
- **Loading States**: Visual feedback for async operations

### Color Scheme & Branding
- **Primary Colors**: Professional blue and white theme
- **Status Colors**: Green (success), Red (danger), Yellow (warning), Blue (info)
- **Typography**: Clean, readable fonts with proper hierarchy
- **Icons**: Bootstrap Icons for consistent iconography

This overview provides the foundation for understanding the MOM system architecture and design decisions. Each component will be detailed in separate documentation files.