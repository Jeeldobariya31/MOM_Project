-- 1. SelectAll Procedure for MOM_Users
CREATE OR ALTER PROCEDURE [dbo].[PR_Users_SelectAll]
AS
BEGIN
    SELECT UserID,
           Username,
           Password,
           FullName,
           Email,
           IsActive,
           LastLogin,
           Created,
           Modified
    FROM [dbo].[MOM_Users]
    ORDER BY FullName
END

GO

-- 2. SelectByPK Procedure for MOM_Users
CREATE OR ALTER PROCEDURE [dbo].[PR_Users_SelectByPK]
@UserID INT
AS
BEGIN
    SELECT UserID,
           Username,
           Password,
           FullName,
           Email,
           IsActive,
           LastLogin,
           Created,
           Modified
    FROM [dbo].[MOM_Users]
    WHERE UserID = @UserID
END
GO

-- 3. Login Procedure - Validate user credentials
CREATE OR ALTER PROCEDURE [dbo].[PR_Users_Login]
@Username NVARCHAR(50),
@Password NVARCHAR(255)
AS
BEGIN
    SELECT UserID,
           Username,
           FullName,
           Email,
           IsActive,
           LastLogin
    FROM [dbo].[MOM_Users]
    WHERE Username = @Username 
      AND Password = @Password 
      AND IsActive = 1
END
GO

-- 4. Update Last Login Procedure
CREATE OR ALTER PROCEDURE [dbo].[PR_Users_UpdateLastLogin]
@UserID INT
AS
BEGIN
    UPDATE [dbo].[MOM_Users]
    SET LastLogin = GETDATE(),
        Modified = GETDATE()
    WHERE UserID = @UserID
END
GO

-- 5. Insert Procedure for MOM_Users
CREATE OR ALTER PROCEDURE [dbo].[PR_Users_Insert]
@Username NVARCHAR(50),
@Password NVARCHAR(255),
@FullName NVARCHAR(100),
@Email NVARCHAR(100),
@IsActive BIT = 1
AS
BEGIN
    INSERT INTO [dbo].[MOM_Users]
    (Username, Password, FullName, Email, IsActive, Modified)
    VALUES
    (@Username, @Password, @FullName, @Email, @IsActive, GETDATE())
END
GO

-- 6. Update Procedure for MOM_Users
CREATE OR ALTER PROCEDURE [dbo].[PR_Users_UpdateByPK]
@UserID INT,
@Username NVARCHAR(50),
@Password NVARCHAR(255),
@FullName NVARCHAR(100),
@Email NVARCHAR(100),
@IsActive BIT
AS
BEGIN
    UPDATE [dbo].[MOM_Users]
    SET Username = @Username,
        Password = @Password,
        FullName = @FullName,
        Email = @Email,
        IsActive = @IsActive,
        Modified = GETDATE()
    WHERE UserID = @UserID
END
GO

-- 7. Delete Procedure for MOM_Users
CREATE OR ALTER PROCEDURE [dbo].[PR_Users_DeleteByPK]
@UserID INT
AS
BEGIN
    DELETE FROM [dbo].[MOM_Users]
    WHERE UserID = @UserID
END
GO