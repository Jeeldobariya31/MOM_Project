-- Insert static user records
INSERT INTO [dbo].[MOM_Users] (Username, Password, FullName, Email, IsActive, Created, Modified)
VALUES 
    ('admin', 'admin123', 'System Administrator', 'admin@company.com', 1, GETDATE(), GETDATE()),
    ('manager', 'manager123', 'Department Manager', 'manager@company.com', 1, GETDATE(), GETDATE()),
    ('user', 'user123', 'Regular User', 'user@company.com', 1, GETDATE(), GETDATE());