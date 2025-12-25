-- 1. SelectAll Procedure for MOM_MeetingType (for List Page)
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingType_SelectAll]
AS
BEGIN
    SELECT MeetingTypeID,
           MeetingTypeName,
           Remarks,
           Created,
           Modified
    FROM [dbo].[MOM_MeetingType]
    ORDER BY MeetingTypeName
END
GO

-- 2. SelectByPK Procedure for MOM_MeetingType (for Insert/Update/Delete & Fill Control)
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingType_SelectByPK]
@MeetingTypeID INT
AS
BEGIN
    SELECT MeetingTypeID,
           MeetingTypeName,
           Remarks,
           Created,
           Modified
    FROM [dbo].[MOM_MeetingType]
    WHERE MeetingTypeID = @MeetingTypeID
END
GO

-- 3. Insert Procedure for MOM_MeetingType (To add any new record)
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingType_Insert]
@MeetingTypeName    NVARCHAR(100),
@Remarks           NVARCHAR(100)
AS
BEGIN
    INSERT INTO [dbo].[MOM_MeetingType]
    (
        MeetingTypeName,
        Remarks,
        Modified
    )
    VALUES
    (
        @MeetingTypeName,
        @Remarks,
        GETDATE()
    )
END
GO

-- 4. Update/Modify Procedure for MOM_MeetingType (To update/modify existing record)
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingType_UpdateByPK]
@MeetingTypeID     INT,
@MeetingTypeName   NVARCHAR(100),
@Remarks          NVARCHAR(100)
AS
BEGIN
    UPDATE [dbo].[MOM_MeetingType]
    SET
        MeetingTypeName = @MeetingTypeName,
        Remarks = @Remarks,
        Modified = GETDATE()
    WHERE MeetingTypeID = @MeetingTypeID
END
GO

-- 5. Delete/PK Procedure for MOM_MeetingType (To Delete record)
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingType_DeleteByPK]
@MeetingTypeID INT
AS
BEGIN
    DELETE FROM [dbo].[MOM_MeetingType]
    WHERE MeetingTypeID = @MeetingTypeID
END
GO


-- Stored Procedures for MOM_Department


-- 1. SelectAll Procedure for MOM_Department
CREATE OR ALTER PROCEDURE [dbo].[PR_Department_SelectAll]
AS
BEGIN
    SELECT DepartmentID,
           DepartmentName,
           Created,
           Modified
    FROM [dbo].[MOM_Department]
    ORDER BY DepartmentName
END
GO

-- 2. SelectByPK Procedure for MOM_Department
CREATE OR ALTER PROCEDURE [dbo].[PR_Department_SelectByPK]
@DepartmentID INT
AS
BEGIN
    SELECT DepartmentID,
           DepartmentName,
           Created,
           Modified
    FROM [dbo].[MOM_Department]
    WHERE DepartmentID = @DepartmentID
END
GO

-- 3. Insert Procedure for MOM_Department
CREATE OR ALTER PROCEDURE [dbo].[PR_Department_Insert]
@DepartmentName NVARCHAR(100)
AS
BEGIN
    INSERT INTO [dbo].[MOM_Department]
    (
        DepartmentName,
        Modified
    )
    VALUES
    (
        @DepartmentName,
        GETDATE()
    )
END
GO

-- 4. Update Procedure for MOM_Department
CREATE OR ALTER PROCEDURE [dbo].[PR_Department_UpdateByPK]
@DepartmentID   INT,
@DepartmentName NVARCHAR(100)
AS
BEGIN
    UPDATE [dbo].[MOM_Department]
    SET
        DepartmentName = @DepartmentName,
        Modified = GETDATE()
    WHERE DepartmentID = @DepartmentID
END
GO

-- 5. Delete Procedure for MOM_Department
CREATE OR ALTER PROCEDURE [dbo].[PR_Department_DeleteByPK]
@DepartmentID INT
AS
BEGIN
    DELETE FROM [dbo].[MOM_Department]
    WHERE DepartmentID = @DepartmentID
END
GO


-- Stored Procedures for MOM_MeetingVenue


-- 1. SelectAll Procedure for MOM_MeetingVenue
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingVenue_SelectAll]
AS
BEGIN
    SELECT MeetingVenueID,
           MeetingVenueName,
           Created,
           Modified
    FROM [dbo].[MOM_MeetingVenue]
    ORDER BY MeetingVenueName
END
GO

-- 2. SelectByPK Procedure for MOM_MeetingVenue
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingVenue_SelectByPK]
@MeetingVenueID INT
AS
BEGIN
    SELECT MeetingVenueID,
           MeetingVenueName,
           Created,
           Modified
    FROM [dbo].[MOM_MeetingVenue]
    WHERE MeetingVenueID = @MeetingVenueID
END
GO

-- 3. Insert Procedure for MOM_MeetingVenue
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingVenue_Insert]
@MeetingVenueName NVARCHAR(100)
AS
BEGIN
    INSERT INTO [dbo].[MOM_MeetingVenue]
    (
        MeetingVenueName,
        Modified
    )
    VALUES
    (
        @MeetingVenueName,
        GETDATE()
    )
END
GO

-- 4. Update Procedure for MOM_MeetingVenue
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingVenue_UpdateByPK]
@MeetingVenueID   INT,
@MeetingVenueName NVARCHAR(100)
AS
BEGIN
    UPDATE [dbo].[MOM_MeetingVenue]
    SET
        MeetingVenueName = @MeetingVenueName,
        Modified = GETDATE()
    WHERE MeetingVenueID = @MeetingVenueID
END
GO

-- 5. Delete Procedure for MOM_MeetingVenue
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingVenue_DeleteByPK]
@MeetingVenueID INT
AS
BEGIN
    DELETE FROM [dbo].[MOM_MeetingVenue]
    WHERE MeetingVenueID = @MeetingVenueID
END
GO

-- Stored Procedures for MOM_Staff


-- 1. SelectAll Procedure for MOM_Staff
CREATE OR ALTER PROCEDURE [dbo].[PR_Staff_SelectAll]
AS
BEGIN
    SELECT s.StaffID,
           s.DepartmentID,
           d.DepartmentName,
           s.StaffName,
           s.MobileNo,
           s.EmailAddress,
           s.Remarks,
           s.Created,
           s.Modified
    FROM [dbo].[MOM_Staff] s
    INNER JOIN [dbo].[MOM_Department] d ON s.DepartmentID = d.DepartmentID
    ORDER BY s.StaffName
END
GO

-- 2. SelectByPK Procedure for MOM_Staff
CREATE OR ALTER PROCEDURE [dbo].[PR_Staff_SelectByPK]
@StaffID INT
AS
BEGIN
    SELECT s.StaffID,
           s.DepartmentID,
           d.DepartmentName,
           s.StaffName,
           s.MobileNo,
           s.EmailAddress,
           s.Remarks,
           s.Created,
           s.Modified
    FROM [dbo].[MOM_Staff] s
    INNER JOIN [dbo].[MOM_Department] d ON s.DepartmentID = d.DepartmentID
    WHERE s.StaffID = @StaffID
END
GO

-- 3. Insert Procedure for MOM_Staff
CREATE OR ALTER PROCEDURE [dbo].[PR_Staff_Insert]
@DepartmentID INT,
@StaffName    NVARCHAR(50),
@MobileNo     NVARCHAR(20),
@EmailAddress NVARCHAR(50),
@Remarks      NVARCHAR(250)
AS
BEGIN
    INSERT INTO [dbo].[MOM_Staff]
    (
        DepartmentID,
        StaffName,
        MobileNo,
        EmailAddress,
        Remarks,
        Modified
    )
    VALUES
    (
        @DepartmentID,
        @StaffName,
        @MobileNo,
        @EmailAddress,
        @Remarks,
        GETDATE()
    )
END
GO

-- 4. Update Procedure for MOM_Staff
CREATE OR ALTER PROCEDURE [dbo].[PR_Staff_UpdateByPK]
@StaffID      INT,
@DepartmentID INT,
@StaffName    NVARCHAR(50),
@MobileNo     NVARCHAR(20),
@EmailAddress NVARCHAR(50),
@Remarks      NVARCHAR(250)
AS
BEGIN
    UPDATE [dbo].[MOM_Staff]
    SET
        DepartmentID = @DepartmentID,
        StaffName = @StaffName,
        MobileNo = @MobileNo,
        EmailAddress = @EmailAddress,
        Remarks = @Remarks,
        Modified = GETDATE()
    WHERE StaffID = @StaffID
END
GO

-- 5. Delete Procedure for MOM_Staff
CREATE OR ALTER PROCEDURE [dbo].[PR_Staff_DeleteByPK]
@StaffID INT
AS
BEGIN
    DELETE FROM [dbo].[MOM_Staff]
    WHERE StaffID = @StaffID
END
GO


-- Stored Procedures for MOM_Meetings


-- 1. SelectAll Procedure for MOM_Meetings
CREATE OR ALTER PROCEDURE [dbo].[PR_Meetings_SelectAll]
AS
BEGIN
    SELECT m.MeetingID,
           m.MeetingDate,
           m.MeetingVenueID,
           mv.MeetingVenueName,
           m.MeetingTypeID,
           mt.MeetingTypeName,
           m.DepartmentID,
           d.DepartmentName,
           m.MeetingDescription,
           m.DocumentPath,
           m.Created,
           m.Modified,
           m.IsCancelled,
           m.CancellationDateTime,
           m.CancellationReason
    FROM [dbo].[MOM_Meetings] m
    INNER JOIN [dbo].[MOM_MeetingVenue] mv ON m.MeetingVenueID = mv.MeetingVenueID
    INNER JOIN [dbo].[MOM_MeetingType] mt ON m.MeetingTypeID = mt.MeetingTypeID
    INNER JOIN [dbo].[MOM_Department] d ON m.DepartmentID = d.DepartmentID
    ORDER BY m.MeetingDate DESC
END
GO

-- 2. SelectByPK Procedure for MOM_Meetings
CREATE OR ALTER PROCEDURE [dbo].[PR_Meetings_SelectByPK]
@MeetingID INT
AS
BEGIN
    SELECT m.MeetingID,
           m.MeetingDate,
           m.MeetingVenueID,
           mv.MeetingVenueName,
           m.MeetingTypeID,
           mt.MeetingTypeName,
           m.DepartmentID,
           d.DepartmentName,
           m.MeetingDescription,
           m.DocumentPath,
           m.Created,
           m.Modified,
           m.IsCancelled,
           m.CancellationDateTime,
           m.CancellationReason
    FROM [dbo].[MOM_Meetings] m
    INNER JOIN [dbo].[MOM_MeetingVenue] mv ON m.MeetingVenueID = mv.MeetingVenueID
    INNER JOIN [dbo].[MOM_MeetingType] mt ON m.MeetingTypeID = mt.MeetingTypeID
    INNER JOIN [dbo].[MOM_Department] d ON m.DepartmentID = d.DepartmentID
    WHERE m.MeetingID = @MeetingID
END
GO

-- 3. Insert Procedure for MOM_Meetings
CREATE OR ALTER PROCEDURE [dbo].[PR_Meetings_Insert]
@MeetingDate        DATETIME,
@MeetingVenueID     INT,
@MeetingTypeID      INT,
@DepartmentID       INT,
@MeetingDescription NVARCHAR(250),
@DocumentPath       NVARCHAR(250)
AS
BEGIN
    INSERT INTO [dbo].[MOM_Meetings]
    (
        MeetingDate,
        MeetingVenueID,
        MeetingTypeID,
        DepartmentID,
        MeetingDescription,
        DocumentPath,
        Modified
    )
    VALUES
    (
        @MeetingDate,
        @MeetingVenueID,
        @MeetingTypeID,
        @DepartmentID,
        @MeetingDescription,
        @DocumentPath,
        GETDATE()
    )
END
GO

-- 4. Update Procedure for MOM_Meetings
CREATE OR ALTER PROCEDURE [dbo].[PR_Meetings_UpdateByPK]
@MeetingID          INT,
@MeetingDate        DATETIME,
@MeetingVenueID     INT,
@MeetingTypeID      INT,
@DepartmentID       INT,
@MeetingDescription NVARCHAR(250),
@DocumentPath       NVARCHAR(250)
AS
BEGIN
    UPDATE [dbo].[MOM_Meetings]
    SET
        MeetingDate = @MeetingDate,
        MeetingVenueID = @MeetingVenueID,
        MeetingTypeID = @MeetingTypeID,
        DepartmentID = @DepartmentID,
        MeetingDescription = @MeetingDescription,
        DocumentPath = @DocumentPath,
        Modified = GETDATE()
    WHERE MeetingID = @MeetingID
END
GO

-- 5. Delete Procedure for MOM_Meetings
CREATE OR ALTER PROCEDURE [dbo].[PR_Meetings_DeleteByPK]
@MeetingID INT
AS
BEGIN
    DELETE FROM [dbo].[MOM_Meetings]
    WHERE MeetingID = @MeetingID
END
GO

-- Stored Procedures for MOM_MeetingMember


-- 1. SelectAll Procedure for MOM_MeetingMember
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingMember_SelectAll]
AS
BEGIN
    SELECT mm.MeetingMemberID,
           mm.MeetingID,
           m.MeetingDate,
           mm.StaffID,
           s.StaffName,
           s.EmailAddress,
           mm.IsPresent,
           mm.Remarks,
           mm.Created,
           mm.Modified
    FROM [dbo].[MOM_MeetingMember] mm
    INNER JOIN [dbo].[MOM_Meetings] m ON mm.MeetingID = m.MeetingID
    INNER JOIN [dbo].[MOM_Staff] s ON mm.StaffID = s.StaffID
    ORDER BY m.MeetingDate DESC, s.StaffName
END
GO

-- 2. SelectByPK Procedure for MOM_MeetingMember
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingMember_SelectByPK]
@MeetingMemberID INT
AS
BEGIN
    SELECT mm.MeetingMemberID,
           mm.MeetingID,
           m.MeetingDate,
           mm.StaffID,
           s.StaffName,
           s.EmailAddress,
           mm.IsPresent,
           mm.Remarks,
           mm.Created,
           mm.Modified
    FROM [dbo].[MOM_MeetingMember] mm
    INNER JOIN [dbo].[MOM_Meetings] m ON mm.MeetingID = m.MeetingID
    INNER JOIN [dbo].[MOM_Staff] s ON mm.StaffID = s.StaffID
    WHERE mm.MeetingMemberID = @MeetingMemberID
END
GO

-- 3. Insert Procedure for MOM_MeetingMember
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingMember_Insert]
@MeetingID INT,
@StaffID   INT,
@IsPresent BIT,
@Remarks   NVARCHAR(250)
AS
BEGIN
    INSERT INTO [dbo].[MOM_MeetingMember]
    (
        MeetingID,
        StaffID,
        IsPresent,
        Remarks,
        Modified
    )
    VALUES
    (
        @MeetingID,
        @StaffID,
        @IsPresent,
        @Remarks,
        GETDATE()
    )
END
GO

-- 4. Update Procedure for MOM_MeetingMember
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingMember_UpdateByPK]
@MeetingMemberID INT,
@MeetingID       INT,
@StaffID         INT,
@IsPresent       BIT,
@Remarks         NVARCHAR(250)
AS
BEGIN
    UPDATE [dbo].[MOM_MeetingMember]
    SET
        MeetingID = @MeetingID,
        StaffID = @StaffID,
        IsPresent = @IsPresent,
        Remarks = @Remarks,
        Modified = GETDATE()
    WHERE MeetingMemberID = @MeetingMemberID
END
GO

-- 5. Delete Procedure for MOM_MeetingMember
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingMember_DeleteByPK]
@MeetingMemberID INT
AS
BEGIN
    DELETE FROM [dbo].[MOM_MeetingMember]
    WHERE MeetingMemberID = @MeetingMemberID
END
GO


-- Additional Utility Procedures


-- Get Meeting Members by Meeting ID
CREATE OR ALTER PROCEDURE [dbo].[PR_MeetingMember_SelectByMeetingID]
@MeetingID INT
AS
BEGIN
    SELECT mm.MeetingMemberID,
           mm.MeetingID,
           mm.StaffID,
           s.StaffName,
           s.EmailAddress,
           mm.IsPresent,
           mm.Remarks,
           mm.Created,
           mm.Modified
    FROM [dbo].[MOM_MeetingMember] mm
    INNER JOIN [dbo].[MOM_Staff] s ON mm.StaffID = s.StaffID
    WHERE mm.MeetingID = @MeetingID
    ORDER BY s.StaffName
END
GO
