USE MOM_Project;
GO

/* ===========================
   CLEAR EXISTING DATA
=========================== */
DELETE FROM MOM_MeetingMember;
DELETE FROM MOM_Meetings;
DELETE FROM MOM_Staff;
DELETE FROM MOM_MeetingVenue;
DELETE FROM MOM_MeetingType;
DELETE FROM MOM_Department;

DBCC CHECKIDENT ('MOM_MeetingMember', RESEED, 0);
DBCC CHECKIDENT ('MOM_Meetings', RESEED, 0);
DBCC CHECKIDENT ('MOM_Staff', RESEED, 0);
DBCC CHECKIDENT ('MOM_MeetingVenue', RESEED, 0);
DBCC CHECKIDENT ('MOM_MeetingType', RESEED, 0);
DBCC CHECKIDENT ('MOM_Department', RESEED, 0);

/* ===========================
   DEPARTMENTS
=========================== */
INSERT INTO MOM_Department (DepartmentName, Modified)
VALUES
('Computer Science Engineering', GETDATE()),
('Civil Engineering', GETDATE()),
('Mechanical Engineering', GETDATE()),
('Management Studies', GETDATE()),
('Diploma Engineering', GETDATE()),
('BCA', GETDATE()),
('BSc IT', GETDATE());

/* ===========================
   MEETING TYPES
=========================== */
INSERT INTO MOM_MeetingType (MeetingTypeName, Remarks, Modified)
VALUES
('Teaching Assistant Meeting', 'TA coordination and planning', GETDATE()),
('Evaluation Meeting', 'Internal / External evaluation', GETDATE()),
('Curriculum Review', 'Syllabus discussion', GETDATE()),
('Faculty Meeting', 'General faculty discussion', GETDATE()),
('Workshop', 'Academic workshop', GETDATE()),
('Research Review', 'Research & publication review', GETDATE());

/* ===========================
   MEETING VENUES
=========================== */
INSERT INTO MOM_MeetingVenue (MeetingVenueName, Modified)
VALUES
('Android Lab', GETDATE()),
('IoT Lab', GETDATE()),
('iOS Lab', GETDATE()),
('C-204', GETDATE()),
('C-205', GETDATE()),
('H-401', GETDATE()),
('H-402', GETDATE());

/* ===========================
   STAFF (FACULTY)
=========================== */
DECLARE 
@CSE INT, @Civil INT, @Mech INT, @Mgmt INT, @Diploma INT, @BCA INT, @BScIT INT;

SELECT @CSE = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Computer Science Engineering';
SELECT @Civil = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Civil Engineering';
SELECT @Mech = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Mechanical Engineering';
SELECT @Mgmt = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Management Studies';
SELECT @Diploma = DepartmentID FROM MOM_Department WHERE DepartmentName = 'Diploma Engineering';
SELECT @BCA = DepartmentID FROM MOM_Department WHERE DepartmentName = 'BCA';
SELECT @BScIT = DepartmentID FROM MOM_Department WHERE DepartmentName = 'BSc IT';

INSERT INTO MOM_Staff (DepartmentID, StaffName, MobileNo, EmailAddress, Remarks, Modified)
VALUES
(@CSE, 'Madhuresh Fichdiya', '999000001', 'madhuresh@college.edu', 'Faculty', GETDATE()),
(@CSE, 'Naimish Vadodariya', '999000002', 'naimish@college.edu', 'Faculty', GETDATE()),
(@Civil, 'Sejal Gupta', '999000003', 'sejal@college.edu', 'Faculty', GETDATE()),
(@Mech, 'Dharmik Vasiyani', '999000004', 'dharmik@college.edu', 'Faculty', GETDATE()),
(@Mgmt, 'Krunal Vyas', '999000005', 'krunal@college.edu', 'Faculty', GETDATE()),
(@Diploma, 'Rupesh Vaishnav', '999000006', 'rupesh@college.edu', 'Faculty', GETDATE()),
(@BCA, 'Gopi Sanghani', '999000007', 'gopi@college.edu', 'Faculty', GETDATE()),
(@BScIT, 'Nilesh Gambhva', '999000008', 'nilesh@college.edu', 'Faculty', GETDATE());

/* ===========================
   MEETINGS (PAST, FUTURE, CANCELLED)
=========================== */
DECLARE 
@AndroidLab INT, @IOTLab INT, @IOSLab INT, @C204 INT, @C205 INT, @H401 INT, @H402 INT,
@TA INT, @Eval INT, @Curriculum INT, @Faculty INT, @Workshop INT, @Research INT,
@Now DATETIME = GETDATE();

SELECT @AndroidLab = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'Android Lab';
SELECT @IOTLab = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'IoT Lab';
SELECT @IOSLab = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'iOS Lab';
SELECT @C204 = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'C-204';
SELECT @C205 = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'C-205';
SELECT @H401 = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'H-401';
SELECT @H402 = MeetingVenueID FROM MOM_MeetingVenue WHERE MeetingVenueName = 'H-402';

SELECT @TA = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Teaching Assistant Meeting';
SELECT @Eval = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Evaluation Meeting';
SELECT @Curriculum = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Curriculum Review';
SELECT @Faculty = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Faculty Meeting';
SELECT @Workshop = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Workshop';
SELECT @Research = MeetingTypeID FROM MOM_MeetingType WHERE MeetingTypeName = 'Research Review';

INSERT INTO MOM_Meetings
(MeetingDate, MeetingVenueID, MeetingTypeID, DepartmentID, MeetingDescription, DocumentPath, Modified, IsCancelled, CancellationDateTime, CancellationReason)
VALUES
(DATEADD(DAY,-40,@Now), @AndroidLab, @TA, @CSE, 'TA Allocation Discussion', '', @Now, 0, NULL, ''),
(DATEADD(DAY,-30,@Now), @IOTLab, @Eval, @CSE, 'Internal Practical Evaluation', '', @Now, 0, NULL, ''),
(DATEADD(DAY,-20,@Now), @C204, @Curriculum, @BCA, 'Syllabus Update Meeting', '', @Now, 0, NULL, ''),
(DATEADD(DAY,-10,@Now), @H401, @Faculty, @Mgmt, 'Monthly Faculty Meeting', '', @Now, 0, NULL, ''),
(DATEADD(DAY,5,@Now), @IOSLab, @Workshop, @BScIT, 'iOS Development Workshop', '', @Now, 0, NULL, ''),
(DATEADD(DAY,10,@Now), @H402, @Research, @CSE, 'Research Paper Review', '', @Now, 0, NULL, ''),
(DATEADD(DAY,-5,@Now), @C205, @Eval, @Diploma, 'External Viva Planning', '', @Now, 1, DATEADD(DAY,-6,@Now), 'Exam postponed');

/* ===========================
   MEETING MEMBERS
=========================== */
DECLARE 
@M1 INT, @M2 INT, @M3 INT, @M4 INT, @M5 INT, @M6 INT, @M7 INT,
@S1 INT, @S2 INT, @S3 INT, @S4 INT, @S5 INT, @S6 INT, @S7 INT, @S8 INT;

SELECT @M1 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'TA Allocation Discussion';
SELECT @M2 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Internal Practical Evaluation';
SELECT @M3 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Syllabus Update Meeting';
SELECT @M4 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Monthly Faculty Meeting';
SELECT @M5 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'iOS Development Workshop';
SELECT @M6 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'Research Paper Review';
SELECT @M7 = MeetingID FROM MOM_Meetings WHERE MeetingDescription = 'External Viva Planning';

SELECT @S1 = StaffID FROM MOM_Staff WHERE StaffName = 'Madhuresh Fichdiya';
SELECT @S2 = StaffID FROM MOM_Staff WHERE StaffName = 'Naimish Vadodariya';
SELECT @S3 = StaffID FROM MOM_Staff WHERE StaffName = 'Sejal Gupta';
SELECT @S4 = StaffID FROM MOM_Staff WHERE StaffName = 'Dharmik Vasiyani';
SELECT @S5 = StaffID FROM MOM_Staff WHERE StaffName = 'Krunal Vyas';
SELECT @S6 = StaffID FROM MOM_Staff WHERE StaffName = 'Rupesh Vaishnav';
SELECT @S7 = StaffID FROM MOM_Staff WHERE StaffName = 'Gopi Sanghani';
SELECT @S8 = StaffID FROM MOM_Staff WHERE StaffName = 'Nilesh Gambhva';

INSERT INTO MOM_MeetingMember (MeetingID, StaffID, IsPresent, Remarks, Modified)
VALUES
(@M1,@S1,1,'Coordinator',@Now),
(@M1,@S2,0,'On leave',@Now),
(@M2,@S3,1,'Evaluator',@Now),
(@M2,@S4,1,'Observer',@Now),
(@M3,@S7,1,'Syllabus drafting',@Now),
(@M4,@S5,1,'Chairperson',@Now),
(@M4,@S6,0,'Late arrival',@Now),
(@M5,@S8,1,'Trainer',@Now),
(@M6,@S1,1,'Reviewer',@Now),
(@M7,@S6,1,'Exam coordinator',@Now);

PRINT 'ACADEMIC STATIC DATA INSERTED SUCCESSFULLY ';


SELECT * FROM MOM_Department
SELECT * FROM MOM_MeetingType
SELECT * FROM MOM_MeetingVenue
SELECT * FROM MOM_Staff
SELECT *  FROM MOM_Meetings
SELECT * FROM MOM_MeetingMember
