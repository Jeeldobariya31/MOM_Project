CREATE DATABASE MOM_Project ;


CREATE TABLE MOM_MeetingType (
    MeetingTypeID INT PRIMARY KEY IDENTITY(1,1),
    MeetingTypeName NVARCHAR(100) NOT NULL UNIQUE,
    Remarks NVARCHAR(100) NOT NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL
);

CREATE TABLE MOM_Department (
    DepartmentID INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL
);

CREATE TABLE MOM_MeetingVenue (
    MeetingVenueID INT PRIMARY KEY IDENTITY(1,1),
    MeetingVenueName NVARCHAR(100) NOT NULL UNIQUE,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL
);

CREATE TABLE MOM_Meetings (
    MeetingID INT PRIMARY KEY IDENTITY(1,1),
    MeetingDate DATETIME NOT NULL,
    MeetingVenueID INT NOT NULL,
    MeetingTypeID INT NOT NULL,
    DepartmentID INT NOT NULL,
    MeetingDescription NVARCHAR(250) NULL,
    DocumentPath NVARCHAR(250) NULL,
    Created DATETIME DEFAULT GETDATE(),
    Modified DATETIME NOT NULL,
    IsCancelled BIT NULL,
    CancellationDateTime DATETIME NULL,
    CancellationReason NVARCHAR(250) NULL,

    CONSTRAINT FK_Meeting_Venue 
        FOREIGN KEY (MeetingVenueID) REFERENCES MOM_MeetingVenue(MeetingVenueID),

    CONSTRAINT FK_Meeting_Type 
        FOREIGN KEY (MeetingTypeID) REFERENCES MOM_MeetingType(MeetingTypeID),

    CONSTRAINT FK_Meeting_Department 
        FOREIGN KEY (DepartmentID) REFERENCES MOM_Department(DepartmentID)
);

CREATE TABLE MOM_Staff (
    StaffID INT PRIMARY KEY IDENTITY(1,1),
    DepartmentID INT NOT NULL,
    StaffName NVARCHAR(50) NOT NULL,
    MobileNo NVARCHAR(20) NOT NULL,
    EmailAddress NVARCHAR(50) NOT NULL UNIQUE,
    Remarks NVARCHAR(250) NULL,
    Created DATETIME NOT NULL DEFAULT GETDATE(),
    Modified DATETIME NOT NULL,

    CONSTRAINT FK_Staff_Department 
        FOREIGN KEY (DepartmentID) REFERENCES MOM_Department(DepartmentID)
);

CREATE TABLE MOM_MeetingMember (
    MeetingMemberID INT PRIMARY KEY IDENTITY(1,1),
    MeetingID INT NOT NULL,
    StaffID INT NOT NULL,
    IsPresent BIT NOT NULL,
    Remarks NVARCHAR(250) NULL,
    Created DATETIME NOT NULL DEFAULT GETDATE(),
    Modified DATETIME NOT NULL,

    CONSTRAINT FK_MeetingMember_Meeting 
        FOREIGN KEY (MeetingID) REFERENCES MOM_Meetings(MeetingID),

    CONSTRAINT FK_MeetingMember_Staff 
        FOREIGN KEY (StaffID) REFERENCES MOM_Staff(StaffID),

    CONSTRAINT UQ_Meeting_Staff UNIQUE (MeetingID, StaffID)
);


-- View all meeting types
SELECT *
FROM MOM_MeetingType;

-- View all departments
SELECT *
FROM MOM_Department;

-- View all meeting venues
SELECT *
FROM MOM_MeetingVenue;

-- View all meetings
SELECT *
FROM MOM_Meetings;

-- View all staff
SELECT *
FROM MOM_Staff;

-- View all meeting members (attendance)
SELECT*
FROM MOM_MeetingMember;

