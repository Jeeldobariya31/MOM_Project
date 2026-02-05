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

        // ========== GENERALIZED CRUD METHODS ==========

        /// <summary>
        /// Generic method to execute any stored procedure with parameters
        /// </summary>
        public DataTable ExecuteStoredProcedure(string procedureName, Dictionary<string, object>? parameters = null)
        {
            var resultTable = new DataTable();
            
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("No database connection available.");
                return resultTable;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(procedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);
                            }
                        }
                        
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(resultTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing stored procedure {procedureName}: {ex.Message}");
            }
            
            return resultTable;
        }

        /// <summary>
        /// Generic method to execute non-query stored procedures (Insert, Update, Delete)
        /// </summary>
        public bool ExecuteNonQueryStoredProcedure(string procedureName, Dictionary<string, object>? parameters = null)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("No database connection available.");
                return false;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(procedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);
                            }
                        }
                        
                        command.ExecuteNonQuery();
                    }
                }
                
                // Refresh data after modification
                RefreshData();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing stored procedure {procedureName}: {ex.Message}");
                return false;
            }
        }

        // ========== ENTITY-SPECIFIC CRUD METHODS ==========

        // Department CRUD Methods
        public DataTable GetAllDepartments() => ExecuteStoredProcedure("PR_Department_SelectAll");
        public DataTable GetDepartmentById(int departmentId) => ExecuteStoredProcedure("PR_Department_SelectByPK", new Dictionary<string, object> { { "DepartmentID", departmentId } });
        public bool InsertDepartment(string departmentName) => ExecuteNonQueryStoredProcedure("PR_Department_Insert", new Dictionary<string, object> { { "DepartmentName", departmentName } });
        public bool UpdateDepartment(int departmentId, string departmentName) => ExecuteNonQueryStoredProcedure("PR_Department_UpdateByPK", new Dictionary<string, object> { { "DepartmentID", departmentId }, { "DepartmentName", departmentName } });
        public bool DeleteDepartment(int departmentId) => ExecuteNonQueryStoredProcedure("PR_Department_DeleteByPK", new Dictionary<string, object> { { "DepartmentID", departmentId } });

        // MeetingType CRUD Methods
        public DataTable GetAllMeetingTypes() => ExecuteStoredProcedure("PR_MeetingType_SelectAll");
        public DataTable GetMeetingTypeById(int meetingTypeId) => ExecuteStoredProcedure("PR_MeetingType_SelectByPK", new Dictionary<string, object> { { "MeetingTypeID", meetingTypeId } });
        public bool InsertMeetingType(string meetingTypeName, string remarks) => ExecuteNonQueryStoredProcedure("PR_MeetingType_Insert", new Dictionary<string, object> { { "MeetingTypeName", meetingTypeName }, { "Remarks", remarks } });
        public bool UpdateMeetingType(int meetingTypeId, string meetingTypeName, string remarks) => ExecuteNonQueryStoredProcedure("PR_MeetingType_UpdateByPK", new Dictionary<string, object> { { "MeetingTypeID", meetingTypeId }, { "MeetingTypeName", meetingTypeName }, { "Remarks", remarks } });
        public bool DeleteMeetingType(int meetingTypeId) => ExecuteNonQueryStoredProcedure("PR_MeetingType_DeleteByPK", new Dictionary<string, object> { { "MeetingTypeID", meetingTypeId } });

        // MeetingVenue CRUD Methods
        public DataTable GetAllMeetingVenues() => ExecuteStoredProcedure("PR_MeetingVenue_SelectAll");
        public DataTable GetMeetingVenueById(int meetingVenueId) => ExecuteStoredProcedure("PR_MeetingVenue_SelectByPK", new Dictionary<string, object> { { "MeetingVenueID", meetingVenueId } });
        public bool InsertMeetingVenue(string meetingVenueName) => ExecuteNonQueryStoredProcedure("PR_MeetingVenue_Insert", new Dictionary<string, object> { { "MeetingVenueName", meetingVenueName } });
        public bool UpdateMeetingVenue(int meetingVenueId, string meetingVenueName) => ExecuteNonQueryStoredProcedure("PR_MeetingVenue_UpdateByPK", new Dictionary<string, object> { { "MeetingVenueID", meetingVenueId }, { "MeetingVenueName", meetingVenueName } });
        public bool DeleteMeetingVenue(int meetingVenueId) => ExecuteNonQueryStoredProcedure("PR_MeetingVenue_DeleteByPK", new Dictionary<string, object> { { "MeetingVenueID", meetingVenueId } });

        // Staff CRUD Methods
        public DataTable GetAllStaff() => ExecuteStoredProcedure("PR_Staff_SelectAll");
        public DataTable GetStaffById(int staffId) => ExecuteStoredProcedure("PR_Staff_SelectByPK", new Dictionary<string, object> { { "StaffID", staffId } });
        public bool InsertStaff(int departmentId, string staffName, string mobileNo, string emailAddress, string remarks) => ExecuteNonQueryStoredProcedure("PR_Staff_Insert", new Dictionary<string, object> { { "DepartmentID", departmentId }, { "StaffName", staffName }, { "MobileNo", mobileNo }, { "EmailAddress", emailAddress }, { "Remarks", remarks } });
        public bool UpdateStaff(int staffId, int departmentId, string staffName, string mobileNo, string emailAddress, string remarks) => ExecuteNonQueryStoredProcedure("PR_Staff_UpdateByPK", new Dictionary<string, object> { { "StaffID", staffId }, { "DepartmentID", departmentId }, { "StaffName", staffName }, { "MobileNo", mobileNo }, { "EmailAddress", emailAddress }, { "Remarks", remarks } });
        public bool DeleteStaff(int staffId) => ExecuteNonQueryStoredProcedure("PR_Staff_DeleteByPK", new Dictionary<string, object> { { "StaffID", staffId } });

        // Meeting CRUD Methods
        public DataTable GetAllMeetings() => ExecuteStoredProcedure("PR_Meetings_SelectAll");
        public DataTable GetMeetingById(int meetingId) => ExecuteStoredProcedure("PR_Meetings_SelectByPK", new Dictionary<string, object> { { "MeetingID", meetingId } });
        public bool InsertMeeting(DateTime meetingDate, int meetingVenueId, int meetingTypeId, int departmentId, string meetingDescription, string documentPath, bool isCancelled = false, DateTime? cancellationDateTime = null, string cancellationReason = null) => ExecuteNonQueryStoredProcedure("PR_Meetings_Insert", new Dictionary<string, object> { { "MeetingDate", meetingDate }, { "MeetingVenueID", meetingVenueId }, { "MeetingTypeID", meetingTypeId }, { "DepartmentID", departmentId }, { "MeetingDescription", meetingDescription }, { "DocumentPath", documentPath ?? "" }, { "IsCancelled", isCancelled }, { "CancellationDateTime", cancellationDateTime }, { "CancellationReason", cancellationReason } });
        public bool UpdateMeeting(int meetingId, DateTime meetingDate, int meetingVenueId, int meetingTypeId, int departmentId, string meetingDescription, string documentPath, bool isCancelled = false, DateTime? cancellationDateTime = null, string cancellationReason = null) => ExecuteNonQueryStoredProcedure("PR_Meetings_UpdateByPK", new Dictionary<string, object> { { "MeetingID", meetingId }, { "MeetingDate", meetingDate }, { "MeetingVenueID", meetingVenueId }, { "MeetingTypeID", meetingTypeId }, { "DepartmentID", departmentId }, { "MeetingDescription", meetingDescription }, { "DocumentPath", documentPath ?? "" }, { "IsCancelled", isCancelled }, { "CancellationDateTime", cancellationDateTime }, { "CancellationReason", cancellationReason } });
        public bool DeleteMeeting(int meetingId) => ExecuteNonQueryStoredProcedure("PR_Meetings_DeleteByPK", new Dictionary<string, object> { { "MeetingID", meetingId } });

        // MeetingMember CRUD Methods
        public DataTable GetAllMeetingMembers() => ExecuteStoredProcedure("PR_MeetingMember_SelectAll");
        public DataTable GetMeetingMemberById(int meetingMemberId) => ExecuteStoredProcedure("PR_MeetingMember_SelectByPK", new Dictionary<string, object> { { "MeetingMemberID", meetingMemberId } });
        public DataTable GetMeetingMembersByMeetingId(int meetingId) => ExecuteStoredProcedure("PR_MeetingMember_SelectByMeetingID", new Dictionary<string, object> { { "MeetingID", meetingId } });
        public bool InsertMeetingMember(int meetingId, int staffId, bool isPresent, string remarks) => ExecuteNonQueryStoredProcedure("PR_MeetingMember_Insert", new Dictionary<string, object> { { "MeetingID", meetingId }, { "StaffID", staffId }, { "IsPresent", isPresent }, { "Remarks", remarks } });
        public bool UpdateMeetingMember(int meetingMemberId, int meetingId, int staffId, bool isPresent, string remarks) => ExecuteNonQueryStoredProcedure("PR_MeetingMember_UpdateByPK", new Dictionary<string, object> { { "MeetingMemberID", meetingMemberId }, { "MeetingID", meetingId }, { "StaffID", staffId }, { "IsPresent", isPresent }, { "Remarks", remarks } });
        public bool DeleteMeetingMember(int meetingMemberId) => ExecuteNonQueryStoredProcedure("PR_MeetingMember_DeleteByPK", new Dictionary<string, object> { { "MeetingMemberID", meetingMemberId } });

        // ========== BULK OPERATIONS ==========

        /// <summary>
        /// Bulk assign staff members to a meeting
        /// </summary>
        public bool BulkAssignStaffToMeeting(int meetingId, List<int> staffIds, string remarks = "")
        {
            bool success = true;
            foreach (int staffId in staffIds)
            {
                if (!InsertMeetingMember(meetingId, staffId, false, remarks))
                {
                    success = false;
                    Console.WriteLine($"Failed to assign staff {staffId} to meeting {meetingId}");
                }
            }
            return success;
        }

        /// <summary>
        /// Bulk update attendance for multiple meeting members
        /// </summary>
        public bool BulkUpdateAttendance(Dictionary<int, bool> memberAttendance, string remarks = "")
        {
            bool success = true;
            foreach (var attendance in memberAttendance)
            {
                int meetingMemberId = attendance.Key;
                bool isPresent = attendance.Value;
                
                // Get existing meeting member data
                var memberData = GetMeetingMemberById(meetingMemberId);
                if (memberData.Rows.Count > 0)
                {
                    var row = memberData.Rows[0];
                    int meetingId = Convert.ToInt32(row["MeetingID"]);
                    int staffId = Convert.ToInt32(row["StaffID"]);
                    
                    if (!UpdateMeetingMember(meetingMemberId, meetingId, staffId, isPresent, remarks))
                    {
                        success = false;
                        Console.WriteLine($"Failed to update attendance for meeting member {meetingMemberId}");
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Mark all assigned members as present for a meeting
        /// </summary>
        public bool BulkMarkAllPresent(int meetingId, string remarks = "Bulk marked present")
        {
            var members = GetMeetingMembersByMeetingId(meetingId);
            bool success = true;
            
            foreach (DataRow row in members.Rows)
            {
                int meetingMemberId = Convert.ToInt32(row["MeetingMemberID"]);
                int staffId = Convert.ToInt32(row["StaffID"]);
                
                if (!UpdateMeetingMember(meetingMemberId, meetingId, staffId, true, remarks))
                {
                    success = false;
                    Console.WriteLine($"Failed to mark member {meetingMemberId} as present");
                }
            }
            return success;
        }

        /// <summary>
        /// Mark all assigned members as absent for a meeting
        /// </summary>
        public bool BulkMarkAllAbsent(int meetingId, string remarks = "Bulk marked absent")
        {
            var members = GetMeetingMembersByMeetingId(meetingId);
            bool success = true;
            
            foreach (DataRow row in members.Rows)
            {
                int meetingMemberId = Convert.ToInt32(row["MeetingMemberID"]);
                int staffId = Convert.ToInt32(row["StaffID"]);
                
                if (!UpdateMeetingMember(meetingMemberId, meetingId, staffId, false, remarks))
                {
                    success = false;
                    Console.WriteLine($"Failed to mark member {meetingMemberId} as absent");
                }
            }
            return success;
        }

        // ========== MEETING MANAGEMENT ==========

        /// <summary>
        /// Cancel a meeting with reason
        /// </summary>
        public bool CancelMeeting(int meetingId, string cancellationReason)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("No database connection available.");
                return false;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Update meeting to set as cancelled
                    string sql = @"UPDATE MOM_Meetings 
                                  SET IsCancelled = 1, 
                                      CancellationDateTime = GETDATE(), 
                                      CancellationReason = @CancellationReason,
                                      Modified = GETDATE()
                                  WHERE MeetingID = @MeetingID";
                    
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@MeetingID", meetingId);
                        command.Parameters.AddWithValue("@CancellationReason", cancellationReason);
                        command.ExecuteNonQuery();
                    }
                }
                
                RefreshData();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling meeting: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reactivate a cancelled meeting
        /// </summary>
        public bool ReactivateMeeting(int meetingId)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("No database connection available.");
                return false;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    string sql = @"UPDATE MOM_Meetings 
                                  SET IsCancelled = 0, 
                                      CancellationDateTime = NULL, 
                                      CancellationReason = NULL,
                                      Modified = GETDATE()
                                  WHERE MeetingID = @MeetingID";
                    
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@MeetingID", meetingId);
                        command.ExecuteNonQuery();
                    }
                }
                
                RefreshData();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reactivating meeting: {ex.Message}");
                return false;
            }
        }

        // ========== DASHBOARD METHODS ==========

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        public Dictionary<string, int> GetDashboardStats()
        {
            var stats = new Dictionary<string, int>();
            
            try
            {
                stats["TotalDepartments"] = Departments.Rows.Count;
                stats["TotalStaff"] = Staff.Rows.Count;
                stats["TotalMeetings"] = Meetings.Rows.Count;
                stats["TotalMeetingTypes"] = MeetingTypes.Rows.Count;
                stats["TotalMeetingVenues"] = MeetingVenues.Rows.Count;
                
                // Count cancelled meetings
                int cancelledMeetings = 0;
                foreach (DataRow row in Meetings.Rows)
                {
                    if (Convert.ToBoolean(row["IsCancelled"]))
                        cancelledMeetings++;
                }
                stats["CancelledMeetings"] = cancelledMeetings;
                stats["ActiveMeetings"] = stats["TotalMeetings"] - cancelledMeetings;
                
                // Count present/absent members
                int presentMembers = 0;
                int absentMembers = 0;
                foreach (DataRow row in MeetingMembers.Rows)
                {
                    if (Convert.ToBoolean(row["IsPresent"]))
                        presentMembers++;
                    else
                        absentMembers++;
                }
                stats["PresentMembers"] = presentMembers;
                stats["AbsentMembers"] = absentMembers;
                stats["TotalMeetingMembers"] = MeetingMembers.Rows.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating dashboard stats: {ex.Message}");
            }
            
            return stats;
        }

        // ========== LEGACY METHODS (for backward compatibility) ==========

        // Method to save a new record to database using stored procedures
        public bool SaveToDatabase(string tableName, Dictionary<string, object> values)
        {
            string procedureName = GetInsertProcedureName(tableName);
            if (string.IsNullOrEmpty(procedureName))
            {
                Console.WriteLine($"No insert procedure found for table: {tableName}");
                return false;
            }
            return ExecuteNonQueryStoredProcedure(procedureName, values);
        }

        // Method to update a record in database using stored procedures
        public bool UpdateInDatabase(string tableName, Dictionary<string, object> values, string whereClause, Dictionary<string, object> whereParameters)
        {
            string procedureName = GetUpdateProcedureName(tableName);
            if (string.IsNullOrEmpty(procedureName))
            {
                Console.WriteLine($"No update procedure found for table: {tableName}");
                return false;
            }
            
            // Combine where parameters and values
            var allParameters = new Dictionary<string, object>(whereParameters);
            foreach (var kvp in values)
            {
                allParameters[kvp.Key] = kvp.Value;
            }
            
            return ExecuteNonQueryStoredProcedure(procedureName, allParameters);
        }

        // Method to delete a record from database using stored procedures
        public bool DeleteFromDatabase(string tableName, Dictionary<string, object> whereParameters)
        {
            string procedureName = GetDeleteProcedureName(tableName);
            if (string.IsNullOrEmpty(procedureName))
            {
                Console.WriteLine($"No delete procedure found for table: {tableName}");
                return false;
            }
            return ExecuteNonQueryStoredProcedure(procedureName, whereParameters);
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