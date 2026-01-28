using System.Data;
using MOM.Models;

namespace MOM.Services
{
    public class DataService
    {
        private static DataService? _instance;
        private static readonly object _lock = new object();

        // DataTables for all entities
        public DataTable Departments { get; private set; }
        public DataTable MeetingTypes { get; private set; }
        public DataTable MeetingVenues { get; private set; }
        public DataTable Staff { get; private set; }
        public DataTable Meetings { get; private set; }
        public DataTable MeetingMembers { get; private set; }
        public DataTable Users { get; private set; }

        private DataService()
        {
            InitializeDataTables();
            SeedData();
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
                            _instance = new DataService();
                    }
                }
                return _instance;
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

        private void SeedData()
        {
            // Seed Departments
            Departments.Rows.Add(1, "Human Resources", DateTime.Now, DateTime.Now);
            Departments.Rows.Add(2, "Information Technology", DateTime.Now, DateTime.Now);
            Departments.Rows.Add(3, "Finance & Accounts", DateTime.Now, DateTime.Now);
            Departments.Rows.Add(4, "Marketing", DateTime.Now, DateTime.Now);
            Departments.Rows.Add(5, "Operations", DateTime.Now, DateTime.Now);
            Departments.Rows.Add(6, "Sales", DateTime.Now, DateTime.Now);
            Departments.Rows.Add(7, "Customer Service", DateTime.Now, DateTime.Now);

            // Seed Meeting Types
            MeetingTypes.Rows.Add(1, "Board Meeting", "Quarterly board discussions", DateTime.Now, DateTime.Now);
            MeetingTypes.Rows.Add(2, "Client Meeting", "Client requirement discussion", DateTime.Now, DateTime.Now);
            MeetingTypes.Rows.Add(3, "Team Stand-up", "Daily team sync", DateTime.Now, DateTime.Now);
            MeetingTypes.Rows.Add(4, "Project Review", "Milestone & progress review", DateTime.Now, DateTime.Now);
            MeetingTypes.Rows.Add(5, "Training Session", "Internal knowledge sharing", DateTime.Now, DateTime.Now);
            MeetingTypes.Rows.Add(6, "Audit Meeting", "Compliance and audit review", DateTime.Now, DateTime.Now);
            MeetingTypes.Rows.Add(7, "Strategy Meeting", "Business planning discussion", DateTime.Now, DateTime.Now);

            // Seed Meeting Venues
            MeetingVenues.Rows.Add(1, "Conference Room A", DateTime.Now, DateTime.Now);
            MeetingVenues.Rows.Add(2, "Conference Room B", DateTime.Now, DateTime.Now);
            MeetingVenues.Rows.Add(3, "Board Room", DateTime.Now, DateTime.Now);
            MeetingVenues.Rows.Add(4, "Training Hall", DateTime.Now, DateTime.Now);
            MeetingVenues.Rows.Add(5, "Virtual Meeting", DateTime.Now, DateTime.Now);
            MeetingVenues.Rows.Add(6, "Executive Suite", DateTime.Now, DateTime.Now);

            // Seed Staff
            Staff.Rows.Add(1, 1, "John Smith", "+1-555-0101", "john.smith@company.com", "HR Manager", DateTime.Now, DateTime.Now);
            Staff.Rows.Add(2, 2, "Sarah Johnson", "+1-555-0102", "sarah.johnson@company.com", "IT Director", DateTime.Now, DateTime.Now);
            Staff.Rows.Add(3, 3, "Michael Brown", "+1-555-0103", "michael.brown@company.com", "Finance Manager", DateTime.Now, DateTime.Now);
            Staff.Rows.Add(4, 4, "Emily Davis", "+1-555-0104", "emily.davis@company.com", "Marketing Lead", DateTime.Now, DateTime.Now);
            Staff.Rows.Add(5, 5, "David Wilson", "+1-555-0105", "david.wilson@company.com", "Operations Manager", DateTime.Now, DateTime.Now);
            Staff.Rows.Add(6, 2, "Lisa Anderson", "+1-555-0106", "lisa.anderson@company.com", "Senior Developer", DateTime.Now, DateTime.Now);
            Staff.Rows.Add(7, 6, "Robert Taylor", "+1-555-0107", "robert.taylor@company.com", "Sales Manager", DateTime.Now, DateTime.Now);
            Staff.Rows.Add(8, 7, "Jennifer Martinez", "+1-555-0108", "jennifer.martinez@company.com", "Customer Service Lead", DateTime.Now, DateTime.Now);

            // Seed Meetings
            Meetings.Rows.Add(1, DateTime.Now.AddDays(1), 1, 1, 1, "Monthly HR Review", "", DateTime.Now, DateTime.Now, false, DBNull.Value, "");
            Meetings.Rows.Add(2, DateTime.Now.AddDays(2), 2, 2, 2, "Client Requirements Discussion", "", DateTime.Now, DateTime.Now, false, DBNull.Value, "");
            Meetings.Rows.Add(3, DateTime.Now.AddDays(3), 3, 3, 2, "Daily Standup", "", DateTime.Now, DateTime.Now, false, DBNull.Value, "");
            Meetings.Rows.Add(4, DateTime.Now.AddDays(7), 4, 5, 3, "Financial Training", "", DateTime.Now, DateTime.Now, false, DBNull.Value, "");
            Meetings.Rows.Add(5, DateTime.Now.AddDays(-2), 1, 4, 4, "Marketing Campaign Review", "", DateTime.Now, DateTime.Now, true, DateTime.Now.AddDays(-3), "Budget constraints");

            // Seed Meeting Members
            MeetingMembers.Rows.Add(1, 1, 1, true, "Attended full meeting", DateTime.Now, DateTime.Now);
            MeetingMembers.Rows.Add(2, 1, 3, true, "Provided financial insights", DateTime.Now, DateTime.Now);
            MeetingMembers.Rows.Add(3, 2, 2, true, "Led the discussion", DateTime.Now, DateTime.Now);
            MeetingMembers.Rows.Add(4, 2, 6, true, "Technical input provided", DateTime.Now, DateTime.Now);
            MeetingMembers.Rows.Add(5, 3, 2, true, "Daily update given", DateTime.Now, DateTime.Now);
            MeetingMembers.Rows.Add(6, 3, 6, false, "On leave", DateTime.Now, DateTime.Now);

            // Seed Users
            Users.Rows.Add(1, "admin", "admin123", "System Administrator", "admin@company.com", true, DBNull.Value, DateTime.Now, DateTime.Now);
            Users.Rows.Add(2, "manager", "manager123", "Department Manager", "manager@company.com", true, DBNull.Value, DateTime.Now, DateTime.Now);
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