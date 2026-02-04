-- =============================================
-- Static Data Insert Script for MOM Project
-- This script inserts all the seed data from DataService into the database
-- Execute this script after creating the database and tables
-- All ID columns are auto-increment (IDENTITY), so we let SQL Server assign them
-- =============================================

USE MOM_Project;
GO

-- Clear existing data (in correct order due to foreign key constraints)
DELETE FROM MOM_MeetingMember;
DELETE FROM MOM_Meetings;
DELETE FROM MOM_Staff;
DELETE FROM MOM_MeetingVenue;
DELETE FROM MOM_MeetingType;
DELETE FROM MOM_Department;

-- Reset identity seeds to start from 1
DBCC CHECKIDENT ('MOM_MeetingMember', RESEED, 0);
DBCC CHECKIDENT ('MOM_Meetings', RESEED, 0);
DBCC CHECKIDENT ('MOM_Staff', RESEED, 0);
DBCC CHECKIDENT ('MOM_MeetingVenue', RESEED, 0);
DBCC CHECKIDENT ('MOM_MeetingType', RESEED, 0);
DBCC CHECKIDENT ('MOM_Department', RESEED, 0);

-- =============================================
-- Insert Departments (DepartmentID will auto-increment from 1)
-- =============================================
INSERT INTO MOM_Department (DepartmentName, Modified)
VALUES 
    ('Human Resources', GETDATE()),
    ('Information Technology', GETDATE()),
    ('Finance & Accounts', GETDATE()),
    ('Marketing', GETDATE()),
    ('Operations', GETDATE()),
    ('Sales', GETDATE()),
    ('Customer Service', GETDATE());

-- =============================================
-- Insert Meeting Types (MeetingTypeID will auto-increment from 1)
-- =============================================
INSERT INTO MOM_MeetingType (MeetingTypeName, Remarks, Modified)
VALUES 
    ('Board Meeting', 'Quarterly board discussions', GETDATE()),
    ('Client Meeting', 'Client requirement discussion', GETDATE()),
    ('Team Stand-up', 'Daily team sync', GETDATE()),
    ('Project Review', 'Milestone & progress review', GETDATE()),
    ('Training Session', 'Internal knowledge sharing', GETDATE()),
    ('Audit Meeting', 'Compliance and audit review', GETDATE()),
    ('Strategy Meeting', 'Business planning discussion', GETDATE());

-- =============================================
-- Insert Meeting Venues (MeetingVenueID will auto-increment from 1)
-- =============================================
INSERT INTO MOM_MeetingVenue (MeetingVenueName, Modified)
VALUES 
    ('Conference Room A', GETDATE()),
    ('Conference Room B', GETDATE()),
    ('Board Room', GETDATE()),
    ('Training Hall', GETDATE()),
    ('Virtual Meeting', GETDATE()),
    ('Executive Suite', GETDATE());

-- =============================================
-- Insert Staff (StaffID will auto-increment from 1)
-- Using variables to get the actual DepartmentIDs that were inserted
-- =============================================

-- Declare variables to store the actual DepartmentIDs
DECLARE @HRDeptID INT, @ITDeptID INT, @FinanceDeptID INT, @MarketingDeptID INT, @OperationsDeptID INT, @SalesDeptID INT, @CustomerServiceDeptID INT;

-- Get the actual DepartmentIDs that were inserted
SELECT @HRDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Human Resources';
SELECT @ITDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Information Technology';
SELECT @FinanceDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Finance & Accounts';
SELECT @MarketingDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Marketing';
SELECT @OperationsDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Operations';
SELECT @SalesDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Sales';
SELECT @CustomerServiceDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Customer Service';

INSERT INTO MOM_Staff (DepartmentID, StaffName, MobileNo, EmailAddress, Remarks, Modified)
VALUES 
    (@HRDeptID, 'John Smith', '+1-555-0101', 'john.smith@company.com', 'HR Manager', GETDATE()),
    (@ITDeptID, 'Sarah Johnson', '+1-555-0102', 'sarah.johnson@company.com', 'IT Director', GETDATE()),
    (@FinanceDeptID, 'Michael Brown', '+1-555-0103', 'michael.brown@company.com', 'Finance Manager', GETDATE()),
    (@MarketingDeptID, 'Emily Davis', '+1-555-0104', 'emily.davis@company.com', 'Marketing Lead', GETDATE()),
    (@OperationsDeptID, 'David Wilson', '+1-555-0105', 'david.wilson@company.com', 'Operations Manager', GETDATE()),
    (@ITDeptID, 'Lisa Anderson', '+1-555-0106', 'lisa.anderson@company.com', 'Senior Developer', GETDATE()),
    (@SalesDeptID, 'Robert Taylor', '+1-555-0107', 'robert.taylor@company.com', 'Sales Manager', GETDATE()),
    (@CustomerServiceDeptID, 'Jennifer Martinez', '+1-555-0108', 'jennifer.martinez@company.com', 'Customer Service Lead', GETDATE());

-- =============================================
-- Insert Meetings (MeetingID will auto-increment from 1)
-- Using variables to get the actual foreign key IDs
-- =============================================
-- Declare variables to store the actual DepartmentIDs
DECLARE @HRDeptID INT, @ITDeptID INT, @FinanceDeptID INT, @MarketingDeptID INT, @OperationsDeptID INT, @SalesDeptID INT, @CustomerServiceDeptID INT;

-- Get the actual DepartmentIDs that were inserted
SELECT @HRDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Human Resources';
SELECT @ITDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Information Technology';
SELECT @FinanceDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Finance & Accounts';
SELECT @MarketingDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Marketing';
SELECT @OperationsDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Operations';
SELECT @SalesDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Sales';
SELECT @CustomerServiceDeptID = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Customer Service';

DECLARE @BaseDate DATETIME = GETDATE();

-- Declare variables for foreign key references
DECLARE @VenueA INT, @VenueB INT, @VenueBoard INT, @VenueTraining INT, @VenueVirtual INT, @VenueExecutive INT;
DECLARE @TypeBoard INT, @TypeClient INT, @TypeStandup INT, @TypeReview INT, @TypeTraining INT, @TypeAudit INT, @TypeStrategy INT;

-- Get actual VenueIDs
SELECT @VenueA = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'Conference Room A';
SELECT @VenueB = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'Conference Room B';
SELECT @VenueBoard = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'Board Room';
SELECT @VenueTraining = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'Training Hall';
SELECT @VenueVirtual = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'Virtual Meeting';
SELECT @VenueExecutive = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'Executive Suite';

-- Get actual MeetingTypeIDs
SELECT @TypeBoard = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Board Meeting';
SELECT @TypeClient = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Client Meeting';
SELECT @TypeStandup = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Team Stand-up';
SELECT @TypeReview = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Project Review';
SELECT @TypeTraining = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Training Session';
SELECT @TypeAudit = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Audit Meeting';
SELECT @TypeStrategy = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Strategy Meeting';

INSERT INTO MOM_Meetings (MeetingDate, MeetingVenueID, MeetingTypeID, DepartmentID, MeetingDescription, DocumentPath, Modified, IsCancelled, CancellationDateTime, CancellationReason)
VALUES 
    -- Past meetings (last 6 months)
    (DATEADD(DAY, -150, @BaseDate), @VenueA, @TypeBoard, @HRDeptID, 'Q4 HR Strategy Meeting', '', DATEADD(DAY, -150, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -140, @BaseDate), @VenueB, @TypeClient, @ITDeptID, 'System Architecture Review', '', DATEADD(DAY, -140, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -130, @BaseDate), @VenueBoard, @TypeStandup, @FinanceDeptID, 'Budget Planning Session', '', DATEADD(DAY, -130, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -120, @BaseDate), @VenueA, @TypeReview, @MarketingDeptID, 'Marketing Campaign Launch', '', DATEADD(DAY, -120, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -110, @BaseDate), @VenueB, @TypeTraining, @OperationsDeptID, 'Operations Review', '', DATEADD(DAY, -110, @BaseDate), 0, NULL, ''),
    
    (DATEADD(DAY, -100, @BaseDate), @VenueBoard, @TypeBoard, @HRDeptID, 'HR Policy Update', '', DATEADD(DAY, -100, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -90, @BaseDate), @VenueA, @TypeClient, @ITDeptID, 'Client Onboarding Process', '', DATEADD(DAY, -90, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -80, @BaseDate), @VenueB, @TypeStandup, @FinanceDeptID, 'Financial Audit Preparation', '', DATEADD(DAY, -80, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -70, @BaseDate), @VenueTraining, @TypeReview, @MarketingDeptID, 'Product Launch Strategy', '', DATEADD(DAY, -70, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -60, @BaseDate), @VenueA, @TypeTraining, @OperationsDeptID, 'Process Improvement Workshop', '', DATEADD(DAY, -60, @BaseDate), 0, NULL, ''),
    
    (DATEADD(DAY, -50, @BaseDate), @VenueB, @TypeBoard, @HRDeptID, 'Team Building Session', '', DATEADD(DAY, -50, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -45, @BaseDate), @VenueBoard, @TypeClient, @ITDeptID, 'Technology Roadmap', '', DATEADD(DAY, -45, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -40, @BaseDate), @VenueA, @TypeStandup, @FinanceDeptID, 'Quarterly Financial Review', '', DATEADD(DAY, -40, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -35, @BaseDate), @VenueB, @TypeReview, @MarketingDeptID, 'Brand Strategy Meeting', '', DATEADD(DAY, -35, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -30, @BaseDate), @VenueTraining, @TypeTraining, @OperationsDeptID, 'Supply Chain Optimization', '', DATEADD(DAY, -30, @BaseDate), 0, NULL, ''),
    
    (DATEADD(DAY, -25, @BaseDate), @VenueA, @TypeBoard, @HRDeptID, 'Performance Review Cycle', '', DATEADD(DAY, -25, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -20, @BaseDate), @VenueBoard, @TypeClient, @ITDeptID, 'Security Assessment', '', DATEADD(DAY, -20, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -15, @BaseDate), @VenueB, @TypeStandup, @FinanceDeptID, 'Investment Planning', '', DATEADD(DAY, -15, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -10, @BaseDate), @VenueA, @TypeReview, @MarketingDeptID, 'Customer Feedback Analysis', '', DATEADD(DAY, -10, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -5, @BaseDate), @VenueTraining, @TypeTraining, @OperationsDeptID, 'Quality Assurance Review', '', DATEADD(DAY, -5, @BaseDate), 0, NULL, ''),
    
    -- Recent meetings
    (DATEADD(DAY, -2, @BaseDate), @VenueA, @TypeBoard, @HRDeptID, 'Weekly HR Standup', '', DATEADD(DAY, -2, @BaseDate), 0, NULL, ''),
    (DATEADD(DAY, -1, @BaseDate), @VenueB, @TypeClient, @ITDeptID, 'Sprint Planning', '', DATEADD(DAY, -1, @BaseDate), 0, NULL, ''),
    
    -- Upcoming meetings
    (DATEADD(DAY, 1, @BaseDate), @VenueA, @TypeBoard, @HRDeptID, 'Monthly HR Review', '', @BaseDate, 0, NULL, ''),
    (DATEADD(DAY, 2, @BaseDate), @VenueB, @TypeClient, @ITDeptID, 'Client Requirements Discussion', '', @BaseDate, 0, NULL, ''),
    (DATEADD(DAY, 3, @BaseDate), @VenueBoard, @TypeStandup, @ITDeptID, 'Daily Standup', '', @BaseDate, 0, NULL, ''),
    (DATEADD(DAY, 7, @BaseDate), @VenueTraining, @TypeTraining, @FinanceDeptID, 'Financial Training', '', @BaseDate, 0, NULL, ''),
    
    -- One cancelled meeting
    (DATEADD(DAY, -3, @BaseDate), @VenueA, @TypeReview, @MarketingDeptID, 'Marketing Campaign Review', '', DATEADD(DAY, -3, @BaseDate), 1, DATEADD(DAY, -4, @BaseDate), 'Budget constraints');

-- =============================================
-- Insert Meeting Members (MeetingMemberID will auto-increment from 1)
-- Using variables to get the actual MeetingID and StaffID values
-- =============================================

-- Declare variables for StaffIDs
DECLARE @JohnID INT, @SarahID INT, @MichaelID INT, @EmilyID INT, @DavidID INT, @LisaID INT, @RobertID INT, @JenniferID INT;
DECLARE @BaseDate DATETIME = GETDATE();
-- Get actual StaffIDs
SELECT @JohnID = StaffID FROM MOM_Staff WHERE StaffName = 'John Smith';
SELECT @SarahID = StaffID FROM MOM_Staff WHERE StaffName = 'Sarah Johnson';
SELECT @MichaelID = StaffID FROM MOM_Staff WHERE StaffName = 'Michael Brown';
SELECT @EmilyID = StaffID FROM MOM_Staff WHERE StaffName = 'Emily Davis';
SELECT @DavidID = StaffID FROM MOM_Staff WHERE StaffName = 'David Wilson';
SELECT @LisaID = StaffID FROM MOM_Staff WHERE StaffName = 'Lisa Anderson';
SELECT @RobertID = StaffID FROM MOM_Staff WHERE StaffName = 'Robert Taylor';
SELECT @JenniferID = StaffID FROM MOM_Staff WHERE StaffName = 'Jennifer Martinez';

-- Declare variables for MeetingIDs (we'll get the first few meetings)
DECLARE @Meeting1 INT, @Meeting2 INT, @Meeting3 INT, @Meeting4 INT, @Meeting5 INT;
DECLARE @Meeting6 INT, @Meeting7 INT, @Meeting8 INT, @Meeting9 INT, @Meeting10 INT;
DECLARE @Meeting11 INT, @Meeting12 INT, @Meeting13 INT, @Meeting14 INT, @Meeting15 INT;
DECLARE @Meeting21 INT, @Meeting22 INT;

-- Get actual MeetingIDs (ordered by MeetingID to get them in sequence)
SELECT @Meeting1 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Q4 HR Strategy Meeting';
SELECT @Meeting2 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'System Architecture Review';
SELECT @Meeting3 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Budget Planning Session';
SELECT @Meeting4 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Marketing Campaign Launch';
SELECT @Meeting5 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Operations Review';
SELECT @Meeting6 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'HR Policy Update';
SELECT @Meeting7 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Client Onboarding Process';
SELECT @Meeting8 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Financial Audit Preparation';
SELECT @Meeting9 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Product Launch Strategy';
SELECT @Meeting10 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Process Improvement Workshop';
SELECT @Meeting11 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Team Building Session';
SELECT @Meeting12 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Technology Roadmap';
SELECT @Meeting13 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Quarterly Financial Review';
SELECT @Meeting14 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Brand Strategy Meeting';
SELECT @Meeting15 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Supply Chain Optimization';
SELECT @Meeting21 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Weekly HR Standup';
SELECT @Meeting22 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Sprint Planning';

INSERT INTO MOM_MeetingMember (MeetingID, StaffID, IsPresent, Remarks, Modified)
VALUES 
    (@Meeting1, @JohnID, 1, 'Attended full meeting', DATEADD(DAY, -150, @BaseDate)),
    (@Meeting1, @MichaelID, 1, 'Provided financial insights', DATEADD(DAY, -150, @BaseDate)),
    (@Meeting2, @SarahID, 1, 'Led the discussion', DATEADD(DAY, -140, @BaseDate)),
    (@Meeting2, @LisaID, 1, 'Technical input provided', DATEADD(DAY, -140, @BaseDate)),
    (@Meeting3, @SarahID, 1, 'Daily update given', DATEADD(DAY, -130, @BaseDate)),
    (@Meeting3, @LisaID, 0, 'On leave', DATEADD(DAY, -130, @BaseDate)),
    (@Meeting4, @EmilyID, 1, 'Marketing presentation', DATEADD(DAY, -120, @BaseDate)),
    (@Meeting4, @JohnID, 1, 'HR coordination', DATEADD(DAY, -120, @BaseDate)),
    (@Meeting5, @DavidID, 1, 'Operations report', DATEADD(DAY, -110, @BaseDate)),
    (@Meeting5, @SarahID, 1, 'Technical support', DATEADD(DAY, -110, @BaseDate)),
    (@Meeting6, @JohnID, 1, 'Policy discussion', DATEADD(DAY, -100, @BaseDate)),
    (@Meeting7, @SarahID, 1, 'Client requirements', DATEADD(DAY, -90, @BaseDate)),
    (@Meeting8, @MichaelID, 1, 'Financial review', DATEADD(DAY, -80, @BaseDate)),
    (@Meeting9, @EmilyID, 1, 'Product strategy', DATEADD(DAY, -70, @BaseDate)),
    (@Meeting10, @DavidID, 1, 'Process improvement', DATEADD(DAY, -60, @BaseDate)),
    (@Meeting11, @JohnID, 1, 'Team building', DATEADD(DAY, -50, @BaseDate)),
    (@Meeting12, @SarahID, 1, 'Technology planning', DATEADD(DAY, -45, @BaseDate)),
    (@Meeting13, @MichaelID, 1, 'Financial analysis', DATEADD(DAY, -40, @BaseDate)),
    (@Meeting14, @EmilyID, 1, 'Brand strategy', DATEADD(DAY, -35, @BaseDate)),
    (@Meeting15, @DavidID, 1, 'Supply chain', DATEADD(DAY, -30, @BaseDate)),
    (@Meeting21, @JohnID, 1, 'Weekly standup', DATEADD(DAY, -2, @BaseDate)),
    (@Meeting22, @SarahID, 1, 'Sprint planning', DATEADD(DAY, -1, @BaseDate));

-- =============================================
-- Verification Queries
-- =============================================
PRINT 'Data insertion completed successfully!';
PRINT '';
PRINT 'Verification of inserted data:';
PRINT '==============================';

PRINT 'Departments: ' + CAST((SELECT COUNT(*) FROM MOM_Department) AS VARCHAR(10));
PRINT 'Meeting Types: ' + CAST((SELECT COUNT(*) FROM MOM_MeetingType) AS VARCHAR(10));
PRINT 'Meeting Venues: ' + CAST((SELECT COUNT(*) FROM MOM_MeetingVenue) AS VARCHAR(10));
PRINT 'Staff: ' + CAST((SELECT COUNT(*) FROM MOM_Staff) AS VARCHAR(10));
PRINT 'Meetings: ' + CAST((SELECT COUNT(*) FROM MOM_Meetings) AS VARCHAR(10));
PRINT 'Meeting Members: ' + CAST((SELECT COUNT(*) FROM MOM_MeetingMember) AS VARCHAR(10));

-- Optional: Display sample data
SELECT 'Departments' as TableName, DepartmentID, DepartmentName FROM MOM_Department
UNION ALL
SELECT 'Meeting Types', MeetingTypeID, MeetingTypeName FROM MOM_MeetingType
UNION ALL
SELECT 'Meeting Venues', MeetingVenueID, MeetingVenueName FROM MOM_MeetingVenue
ORDER BY TableName, DepartmentID;

PRINT '';
PRINT 'Static data insertion script completed successfully!';
PRINT 'You can now use your application with this sample data.';

