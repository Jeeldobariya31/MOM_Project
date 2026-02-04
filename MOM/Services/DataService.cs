using System.Data;
using System.Data.SqlClient;
using MOM.Models;

namespace MOM.Services
{
    public class DataService
    {
        private static DataService? _instance;
        private static readonly object _lock = new object();
        private readonly string _connectionString;

        // DataTables for all entities
        public DataTable Departments { get; private set; }
        public DataTable MeetingTypes { get; private set; }
        public DataTable MeetingVenues { get; private set; }
        public DataTable Staff { get; private set; }
        public DataTable Meetings { get; private set; }
        public DataTable MeetingMembers { get; private set; }
        public DataTable Users { get; private set; }

        private DataService(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDataTables();
            LoadDataFromDatabase();
        }

        public static DataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            // This will be injected from Program.cs or controller
                            throw new InvalidOperationException("DataService must be initialized with connection string first. Call Initialize() method.");
                        }
                    }
                }
                return _instance;
            }
        }

        public static void Initialize(string connectionString)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new DataService(connectionString);
                }
            }
        }

        public static void InitializeWithFallback()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new DataService("");
                }
            }
        }

        private void InitializeDataTables()
        {
            // Initialize Departments table
            Departments = new DataTable("Departments");
            Departments.Columns.Add("DepartmentID", typeof(int));
            Departments.Columns.Add("DepartmentName", typeof(string));
            Departments.Columns.Add("Created", typeof(DateTime));
            Departments.Columns.Add("Modified", typeof(DateTime));
            Departments.PrimaryKey = new DataColumn[] { Departments.Columns["DepartmentID"]! };

            // Initialize MeetingTypes table
            MeetingTypes = new DataTable("MeetingTypes");
            MeetingTypes.Columns.Add("MeetingTypeID", typeof(int));
            MeetingTypes.Columns.Add("MeetingTypeName", typeof(string));
            MeetingTypes.Columns.Add("Remarks", typeof(string));
            MeetingTypes.Columns.Add("Created", typeof(DateTime));
            MeetingTypes.Columns.Add("Modified", typeof(DateTime));
            MeetingTypes.PrimaryKey = new DataColumn[] { MeetingTypes.Columns["MeetingTypeID"]! };

            // Initialize MeetingVenues table
            MeetingVenues = new DataTable("MeetingVenues");
            MeetingVenues.Columns.Add("MeetingVenueID", typeof(int));
            MeetingVenues.Columns.Add("MeetingVenueName", typeof(string));
            MeetingVenues.Columns.Add("Created", typeof(DateTime));
            MeetingVenues.Columns.Add("Modified", typeof(DateTime));
            MeetingVenues.PrimaryKey = new DataColumn[] { MeetingVenues.Columns["MeetingVenueID"]! };

            // Initialize Staff table
            Staff = new DataTable("Staff");
            Staff.Columns.Add("StaffID", typeof(int));
            Staff.Columns.Add("DepartmentID", typeof(int));
            Staff.Columns.Add("StaffName", typeof(string));
            Staff.Columns.Add("MobileNo", typeof(string));
            Staff.Columns.Add("EmailAddress", typeof(string));
            Staff.Columns.Add("Remarks", typeof(string));
            Staff.Columns.Add("Created", typeof(DateTime));
            Staff.Columns.Add("Modified", typeof(DateTime));
            Staff.PrimaryKey = new DataColumn[] { Staff.Columns["StaffID"]! };

            // Initialize Meetings table
            Meetings = new DataTable("Meetings");
            Meetings.Columns.Add("MeetingID", typeof(int));
            Meetings.Columns.Add("MeetingDate", typeof(DateTime));
            Meetings.Columns.Add("MeetingVenueID", typeof(int));
            Meetings.Columns.Add("MeetingTypeID", typeof(int));
            Meetings.Columns.Add("DepartmentID", typeof(int));
            Meetings.Columns.Add("MeetingDescription", typeof(string));
            Meetings.Columns.Add("DocumentPath", typeof(string));
            Meetings.Columns.Add("Created", typeof(DateTime));
            Meetings.Columns.Add("Modified", typeof(DateTime));
            Meetings.Columns.Add("IsCancelled", typeof(bool));
            Meetings.Columns.Add("CancellationDateTime", typeof(DateTime));
            Meetings.Columns.Add("CancellationReason", typeof(string));
            Meetings.PrimaryKey = new DataColumn[] { Meetings.Columns["MeetingID"]! };

            // Initialize MeetingMembers table
            MeetingMembers = new DataTable("MeetingMembers");
            MeetingMembers.Columns.Add("MeetingMemberID", typeof(int));
            MeetingMembers.Columns.Add("MeetingID", typeof(int));
            MeetingMembers.Columns.Add("StaffID", typeof(int));
            MeetingMembers.Columns.Add("IsPresent", typeof(bool));
            MeetingMembers.Columns.Add("Remarks", typeof(string));
            MeetingMembers.Columns.Add("Created", typeof(DateTime));
            MeetingMembers.Columns.Add("Modified", typeof(DateTime));
            MeetingMembers.PrimaryKey = new DataColumn[] { MeetingMembers.Columns["MeetingMemberID"]! };

            // Initialize Users table
            Users = new DataTable("Users");
            Users.Columns.Add("UserID", typeof(int));
            Users.Columns.Add("Username", typeof(string));
            Users.Columns.Add("Password", typeof(string));
            Users.Columns.Add("FullName", typeof(string));
            Users.Columns.Add("Email", typeof(string));
            Users.Columns.Add("IsActive", typeof(bool));
            Users.Columns.Add("LastLogin", typeof(DateTime));
            Users.Columns.Add("Created", typeof(DateTime));
            Users.Columns.Add("Modified", typeof(DateTime));
            Users.PrimaryKey = new DataColumn[] { Users.Columns["UserID"]! };
        }

        private void LoadDataFromDatabase()
        {
            // If no connection string provided, use static data only
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("No connection string provided. Loading static data only.");
                SeedData();
                return;
            }

            try
            {
                LoadDepartments();
                LoadMeetingTypes();
                LoadMeetingVenues();
                LoadStaff();
                LoadMeetings();
                LoadMeetingMembers();
                LoadUsers(); // This will load static data since Users table doesn't exist in DB
            }
            catch (Exception ex)
            {
                // Log error and fall back to seed data if database is not available
                Console.WriteLine($"Error loading data from database: {ex.Message}");
                SeedData(); // Fallback to static data
            }
        }

        private void LoadDepartments()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("PR_Department_SelectAll", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = Departments.NewRow();
                            row["DepartmentID"] = reader["DepartmentID"];
                            row["DepartmentName"] = reader["DepartmentName"];
                            row["Created"] = reader["Created"];
                            row["Modified"] = reader["Modified"];
                            Departments.Rows.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Departments: {ex.Message}");
                // Load some default departments if database fails
                Departments.Rows.Add(1, "Human Resources", DateTime.Now, DateTime.Now);
                Departments.Rows.Add(2, "Information Technology", DateTime.Now, DateTime.Now);
                Departments.Rows.Add(3, "Finance & Accounts", DateTime.Now, DateTime.Now);
            }
        }

        private void LoadMeetingTypes()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("PR_MeetingType_SelectAll", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = MeetingTypes.NewRow();
                            row["MeetingTypeID"] = reader["MeetingTypeID"];
                            row["MeetingTypeName"] = reader["MeetingTypeName"];
                            row["Remarks"] = reader["Remarks"] ?? "";
                            row["Created"] = reader["Created"];
                            row["Modified"] = reader["Modified"];
                            MeetingTypes.Rows.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading MeetingTypes: {ex.Message}");
                // Load some default meeting types if database fails
                MeetingTypes.Rows.Add(1, "Board Meeting", "Quarterly board discussions", DateTime.Now, DateTime.Now);
                MeetingTypes.Rows.Add(2, "Client Meeting", "Client requirement discussion", DateTime.Now, DateTime.Now);
                MeetingTypes.Rows.Add(3, "Team Stand-up", "Daily team sync", DateTime.Now, DateTime.Now);
            }
        }

        private void LoadMeetingVenues()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("PR_MeetingVenue_SelectAll", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = MeetingVenues.NewRow();
                            row["MeetingVenueID"] = reader["MeetingVenueID"];
                            row["MeetingVenueName"] = reader["MeetingVenueName"];
                            row["Created"] = reader["Created"];
                            row["Modified"] = reader["Modified"];
                            MeetingVenues.Rows.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading MeetingVenues: {ex.Message}");
                // Load some default meeting venues if database fails
                MeetingVenues.Rows.Add(1, "Conference Room A", DateTime.Now, DateTime.Now);
                MeetingVenues.Rows.Add(2, "Conference Room B", DateTime.Now, DateTime.Now);
                MeetingVenues.Rows.Add(3, "Board Room", DateTime.Now, DateTime.Now);
            }
        }

        private void LoadStaff()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("PR_Staff_SelectAll", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = Staff.NewRow();
                            row["StaffID"] = reader["StaffID"];
                            row["DepartmentID"] = reader["DepartmentID"];
                            row["StaffName"] = reader["StaffName"];
                            row["MobileNo"] = reader["MobileNo"] ?? "";
                            row["EmailAddress"] = reader["EmailAddress"] ?? "";
                            row["Remarks"] = reader["Remarks"] ?? "";
                            row["Created"] = reader["Created"];
                            row["Modified"] = reader["Modified"];
                            Staff.Rows.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Staff: {ex.Message}");
                // Load some default staff if database fails
                Staff.Rows.Add(1, 1, "John Smith", "+1-555-0101", "john.smith@company.com", "HR Manager", DateTime.Now, DateTime.Now);
                Staff.Rows.Add(2, 2, "Sarah Johnson", "+1-555-0102", "sarah.johnson@company.com", "IT Director", DateTime.Now, DateTime.Now);
            }
        }

        private void LoadMeetings()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("PR_Meetings_SelectAll", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = Meetings.NewRow();
                            row["MeetingID"] = reader["MeetingID"];
                            row["MeetingDate"] = reader["MeetingDate"];
                            row["MeetingVenueID"] = reader["MeetingVenueID"];
                            row["MeetingTypeID"] = reader["MeetingTypeID"];
                            row["DepartmentID"] = reader["DepartmentID"];
                            row["MeetingDescription"] = reader["MeetingDescription"] ?? "";
                            row["DocumentPath"] = reader["DocumentPath"] ?? "";
                            row["Created"] = reader["Created"];
                            row["Modified"] = reader["Modified"];
                            row["IsCancelled"] = reader["IsCancelled"];
                            row["CancellationDateTime"] = reader["CancellationDateTime"] == DBNull.Value ? DBNull.Value : reader["CancellationDateTime"];
                            row["CancellationReason"] = reader["CancellationReason"] ?? "";
                            Meetings.Rows.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading Meetings: {ex.Message}");
                // Load some default meetings if database fails
                var now = DateTime.Now;
                Meetings.Rows.Add(1, now.AddDays(1), 1, 1, 1, "Sample Meeting", "", now, now, false, DBNull.Value, "");
            }
        }

        private void LoadMeetingMembers()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("PR_MeetingMember_SelectAll", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = MeetingMembers.NewRow();
                            row["MeetingMemberID"] = reader["MeetingMemberID"];
                            row["MeetingID"] = reader["MeetingID"];
                            row["StaffID"] = reader["StaffID"];
                            row["IsPresent"] = reader["IsPresent"];
                            row["Remarks"] = reader["Remarks"] ?? "";
                            row["Created"] = reader["Created"];
                            row["Modified"] = reader["Modified"];
                            MeetingMembers.Rows.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading MeetingMembers: {ex.Message}");
                // Load some default meeting members if database fails
                var now = DateTime.Now;
                MeetingMembers.Rows.Add(1, 1, 1, true, "Sample attendance", now, now);
            }
        }

        private void LoadUsers()
        {
            // Users table doesn't exist in database yet, use static data
            LoadStaticUsers();
        }

        private void LoadStaticUsers()
        {
            // Static user data since Users table is not in database
            Users.Rows.Add(1, "admin", "admin123", "System Administrator", "admin@company.com", true, DBNull.Value, DateTime.Now, DateTime.Now);
            Users.Rows.Add(2, "manager", "manager123", "Department Manager", "manager@company.com", true, DBNull.Value, DateTime.Now, DateTime.Now);
            Users.Rows.Add(3, "user", "user123", "Regular User", "user@company.com", true, DBNull.Value, DateTime.Now, DateTime.Now);
            Users.Rows.Add(4, "hr_admin", "hr123", "HR Administrator", "hr@company.com", true, DBNull.Value, DateTime.Now, DateTime.Now);
            Users.Rows.Add(5, "it_admin", "it123", "IT Administrator", "it@company.com", true, DBNull.Value, DateTime.Now, DateTime.Now);
        }

        // Fallback method for when database is not available
        private void SeedData()
        {
            // Seed Departments
            Departments.Rows.Add(1, "Human Resources", DateTime.Now, DateTime.Now);
            Departments.Rows.Add(2, "Information Technology", DateTime.Now, DateTime.Now);
            Departments.Rows.Add(3, "Finance & Accounts", DateTime.Now, DateTime.Now);
            Departments.Rows.Add(4, "Marketing", DateTime.Now, DateTime.Now);
            Departments.Rows.Add(5, "Operations", DateTime.Now, DateTime.Now);

            // Seed Meeting Types
            MeetingTypes.Rows.Add(1, "Board Meeting", "Quarterly board discussions", DateTime.Now, DateTime.Now);
            MeetingTypes.Rows.Add(2, "Client Meeting", "Client requirement discussion", DateTime.Now, DateTime.Now);
            MeetingTypes.Rows.Add(3, "Team Stand-up", "Daily team sync", DateTime.Now, DateTime.Now);

            // Seed Meeting Venues
            MeetingVenues.Rows.Add(1, "Conference Room A", DateTime.Now, DateTime.Now);
            MeetingVenues.Rows.Add(2, "Conference Room B", DateTime.Now, DateTime.Now);
            MeetingVenues.Rows.Add(3, "Board Room", DateTime.Now, DateTime.Now);

            // Seed Staff
            Staff.Rows.Add(1, 1, "John Smith", "+1-555-0101", "john.smith@company.com", "HR Manager", DateTime.Now, DateTime.Now);
            Staff.Rows.Add(2, 2, "Sarah Johnson", "+1-555-0102", "sarah.johnson@company.com", "IT Director", DateTime.Now, DateTime.Now);

            // Seed Users
            Users.Rows.Add(1, "admin", "admin123", "System Administrator", "admin@company.com", true, DBNull.Value, DateTime.Now, DateTime.Now);
        }

        // Method to refresh data from database
        public void RefreshData()
        {
            // Clear existing data
            Departments.Clear();
            MeetingTypes.Clear();
            MeetingVenues.Clear();
            Staff.Clear();
            Meetings.Clear();
            MeetingMembers.Clear();
            Users.Clear();

            // Reload from database (Users will load static data)
            LoadDataFromDatabase();
        }

        // Method to save a new record to database using stored procedures
        public bool SaveToDatabase(string tableName, Dictionary<string, object> values)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("No database connection available. Cannot save to database.");
                return false;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    string procedureName = GetInsertProcedureName(tableName);
                    if (string.IsNullOrEmpty(procedureName))
                    {
                        Console.WriteLine($"No insert procedure found for table: {tableName}");
                        return false;
                    }

                    using (var command = new SqlCommand(procedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        
                        foreach (var kvp in values)
                        {
                            command.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        
                        command.ExecuteNonQuery();
                    }
                }
                
                // Refresh the specific table data
                RefreshData();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to database: {ex.Message}");
                return false;
            }
        }

        // Method to update a record in database using stored procedures
        public bool UpdateInDatabase(string tableName, Dictionary<string, object> values, string whereClause, Dictionary<string, object> whereParameters)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("No database connection available. Cannot update database.");
                return false;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    string procedureName = GetUpdateProcedureName(tableName);
                    if (string.IsNullOrEmpty(procedureName))
                    {
                        Console.WriteLine($"No update procedure found for table: {tableName}");
                        return false;
                    }

                    using (var command = new SqlCommand(procedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        
                        // Add WHERE parameters first (usually the ID)
                        foreach (var kvp in whereParameters)
                        {
                            command.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        
                        // Add SET parameters
                        foreach (var kvp in values)
                        {
                            command.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        
                        command.ExecuteNonQuery();
                    }
                }
                
                // Refresh the specific table data
                RefreshData();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating database: {ex.Message}");
                return false;
            }
        }

        // Method to delete a record from database using stored procedures
        public bool DeleteFromDatabase(string tableName, Dictionary<string, object> whereParameters)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("No database connection available. Cannot delete from database.");
                return false;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    string procedureName = GetDeleteProcedureName(tableName);
                    if (string.IsNullOrEmpty(procedureName))
                    {
                        Console.WriteLine($"No delete procedure found for table: {tableName}");
                        return false;
                    }

                    using (var command = new SqlCommand(procedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        
                        foreach (var kvp in whereParameters)
                        {
                            command.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                        }
                        
                        command.ExecuteNonQuery();
                    }
                }
                
                // Refresh the specific table data
                RefreshData();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting from database: {ex.Message}");
                return false;
            }
        }

        private string GetInsertProcedureName(string tableName)
        {
            return tableName switch
            {
                "Department" => "PR_Department_Insert",
                "MeetingType" => "PR_MeetingType_Insert",
                "MeetingVenue" => "PR_MeetingVenue_Insert",
                "Staff" => "PR_Staff_Insert",
                "Meetings" => "PR_Meetings_Insert",
                "MeetingMember" => "PR_MeetingMember_Insert",
                _ => ""
            };
        }

        private string GetUpdateProcedureName(string tableName)
        {
            return tableName switch
            {
                "Department" => "PR_Department_UpdateByPK",
                "MeetingType" => "PR_MeetingType_UpdateByPK",
                "MeetingVenue" => "PR_MeetingVenue_UpdateByPK",
                "Staff" => "PR_Staff_UpdateByPK",
                "Meetings" => "PR_Meetings_UpdateByPK",
                "MeetingMember" => "PR_MeetingMember_UpdateByPK",
                _ => ""
            };
        }

        private string GetDeleteProcedureName(string tableName)
        {
            return tableName switch
            {
                "Department" => "PR_Department_DeleteByPK",
                "MeetingType" => "PR_MeetingType_DeleteByPK",
                "MeetingVenue" => "PR_MeetingVenue_DeleteByPK",
                "Staff" => "PR_Staff_DeleteByPK",
                "Meetings" => "PR_Meetings_DeleteByPK",
                "MeetingMember" => "PR_MeetingMember_DeleteByPK",
                _ => ""
            };
        }

        public int GetNextId(DataTable table, string idColumn)
        {
            if (table.Rows.Count == 0) return 1;
            return table.AsEnumerable().Max(row => row.Field<int>(idColumn)) + 1;
        }

        public DataTable GetFilteredData(DataTable sourceTable, string searchTerm = "", Dictionary<string, object>? filters = null)
        {
            var filteredTable = sourceTable.Clone();
            
            foreach (DataRow row in sourceTable.Rows)
            {
                bool includeRow = true;

                // Apply search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    bool matchFound = false;
                    foreach (DataColumn column in sourceTable.Columns)
                    {
                        if (column.DataType == typeof(string))
                        {
                            var value = row[column].ToString();
                            if (!string.IsNullOrEmpty(value) && value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                            {
                                matchFound = true;
                                break;
                            }
                        }
                    }
                    if (!matchFound) includeRow = false;
                }

                // Apply additional filters
                if (includeRow && filters != null)
                {
                    foreach (var filter in filters)
                    {
                        if (sourceTable.Columns.Contains(filter.Key))
                        {
                            var columnValue = row[filter.Key];
                            if (!columnValue.Equals(filter.Value))
                            {
                                includeRow = false;
                                break;
                            }
                        }
                    }
                }

                if (includeRow)
                {
                    filteredTable.ImportRow(row);
                }
            }

            return filteredTable;
        }
    }
}