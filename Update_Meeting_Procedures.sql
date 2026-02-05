-- =============================================
-- Update Meeting Stored Procedures
-- This script updates the Insert and Update procedures for MOM_Meetings
-- to include IsCancelled, CancellationDateTime, and CancellationReason parameters
-- =============================================

USE MOM_Project;
GO

-- 3. Insert Procedure for MOM_Meetings
CREATE OR ALTER PROCEDURE [dbo].[PR_Meetings_Insert]
@MeetingDate        DATETIME,
@MeetingVenueID     INT,
@MeetingTypeID      INT,
@DepartmentID       INT,
@MeetingDescription NVARCHAR(250),
@DocumentPath       NVARCHAR(250),
@IsCancelled        BIT = 0,
@CancellationDateTime DATETIME = NULL,
@CancellationReason NVARCHAR(250) = NULL
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
        IsCancelled,
        CancellationDateTime,
        CancellationReason,
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
        @IsCancelled,
        @CancellationDateTime,
        @CancellationReason,
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
@DocumentPath       NVARCHAR(250),
@IsCancelled        BIT = 0,
@CancellationDateTime DATETIME = NULL,
@CancellationReason NVARCHAR(250) = NULL
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
        IsCancelled = @IsCancelled,
        CancellationDateTime = @CancellationDateTime,
        CancellationReason = @CancellationReason,
        Modified = GETDATE()
    WHERE MeetingID = @MeetingID
END
GO

PRINT 'Meeting stored procedures updated successfully!';