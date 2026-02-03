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

            // Seed Meetings with more realistic historical data
            var now = DateTime.Now;
            
            // Past meetings (last 6 months)
            Meetings.Rows.Add(1, now.AddDays(-150), 1, 1, 1, "Q4 HR Strategy Meeting", "", now.AddDays(-150), now.AddDays(-150), false, DBNull.Value, "");
            Meetings.Rows.Add(2, now.AddDays(-140), 2, 2, 2, "System Architecture Review", "", now.AddDays(-140), now.AddDays(-140), false, DBNull.Value, "");
            Meetings.Rows.Add(3, now.AddDays(-130), 3, 3, 3, "Budget Planning Session", "", now.AddDays(-130), now.AddDays(-130), false, DBNull.Value, "");
            Meetings.Rows.Add(4, now.AddDays(-120), 1, 4, 4, "Marketing Campaign Launch", "", now.AddDays(-120), now.AddDays(-120), false, DBNull.Value, "");
            Meetings.Rows.Add(5, now.AddDays(-110), 2, 5, 5, "Operations Review", "", now.AddDays(-110), now.AddDays(-110), false, DBNull.Value, "");
            
            Meetings.Rows.Add(6, now.AddDays(-100), 3, 1, 1, "HR Policy Update", "", now.AddDays(-100), now.AddDays(-100), false, DBNull.Value, "");
            Meetings.Rows.Add(7, now.AddDays(-90), 1, 2, 2, "Client Onboarding Process", "", now.AddDays(-90), now.AddDays(-90), false, DBNull.Value, "");
            Meetings.Rows.Add(8, now.AddDays(-80), 2, 3, 3, "Financial Audit Preparation", "", now.AddDays(-80), now.AddDays(-80), false, DBNull.Value, "");
            Meetings.Rows.Add(9, now.AddDays(-70), 4, 4, 4, "Product Launch Strategy", "", now.AddDays(-70), now.AddDays(-70), false, DBNull.Value, "");
            Meetings.Rows.Add(10, now.AddDays(-60), 1, 5, 5, "Process Improvement Workshop", "", now.AddDays(-60), now.AddDays(-60), false, DBNull.Value, "");
            
            Meetings.Rows.Add(11, now.AddDays(-50), 2, 1, 1, "Team Building Session", "", now.AddDays(-50), now.AddDays(-50), false, DBNull.Value, "");
            Meetings.Rows.Add(12, now.AddDays(-45), 3, 2, 2, "Technology Roadmap", "", now.AddDays(-45), now.AddDays(-45), false, DBNull.Value, "");
            Meetings.Rows.Add(13, now.AddDays(-40), 1, 3, 3, "Quarterly Financial Review", "", now.AddDays(-40), now.AddDays(-40), false, DBNull.Value, "");
            Meetings.Rows.Add(14, now.AddDays(-35), 2, 4, 4, "Brand Strategy Meeting", "", now.AddDays(-35), now.AddDays(-35), false, DBNull.Value, "");
            Meetings.Rows.Add(15, now.AddDays(-30), 4, 5, 5, "Supply Chain Optimization", "", now.AddDays(-30), now.AddDays(-30), false, DBNull.Value, "");
            
            Meetings.Rows.Add(16, now.AddDays(-25), 1, 1, 1, "Performance Review Cycle", "", now.AddDays(-25), now.AddDays(-25), false, DBNull.Value, "");
            Meetings.Rows.Add(17, now.AddDays(-20), 3, 2, 2, "Security Assessment", "", now.AddDays(-20), now.AddDays(-20), false, DBNull.Value, "");
            Meetings.Rows.Add(18, now.AddDays(-15), 2, 3, 3, "Investment Planning", "", now.AddDays(-15), now.AddDays(-15), false, DBNull.Value, "");
            Meetings.Rows.Add(19, now.AddDays(-10), 1, 4, 4, "Customer Feedback Analysis", "", now.AddDays(-10), now.AddDays(-10), false, DBNull.Value, "");
            Meetings.Rows.Add(20, now.AddDays(-5), 4, 5, 5, "Quality Assurance Review", "", now.AddDays(-5), now.AddDays(-5), false, DBNull.Value, "");
            
            // Recent meetings
            Meetings.Rows.Add(21, now.AddDays(-2), 1, 1, 1, "Weekly HR Standup", "", now.AddDays(-2), now.AddDays(-2), false, DBNull.Value, "");
            Meetings.Rows.Add(22, now.AddDays(-1), 2, 2, 2, "Sprint Planning", "", now.AddDays(-1), now.AddDays(-1), false, DBNull.Value, "");
            
            // Upcoming meetings
            Meetings.Rows.Add(23, now.AddDays(1), 1, 1, 1, "Monthly HR Review", "", now, now, false, DBNull.Value, "");
            Meetings.Rows.Add(24, now.AddDays(2), 2, 2, 2, "Client Requirements Discussion", "", now, now, false, DBNull.Value, "");
            Meetings.Rows.Add(25, now.AddDays(3), 3, 3, 2, "Daily Standup", "", now, now, false, DBNull.Value, "");
            Meetings.Rows.Add(26, now.AddDays(7), 4, 5, 3, "Financial Training", "", now, now, false, DBNull.Value, "");
            
            // One cancelled meeting
            Meetings.Rows.Add(27, now.AddDays(-3), 1, 4, 4, "Marketing Campaign Review", "", now.AddDays(-3), now.AddDays(-3), true, now.AddDays(-4), "Budget constraints");

            // Seed Meeting Members with more data
            MeetingMembers.Rows.Add(1, 1, 1, true, "Attended full meeting", now.AddDays(-150), now.AddDays(-150));
            MeetingMembers.Rows.Add(2, 1, 3, true, "Provided financial insights", now.AddDays(-150), now.AddDays(-150));
            MeetingMembers.Rows.Add(3, 2, 2, true, "Led the discussion", now.AddDays(-140), now.AddDays(-140));
            MeetingMembers.Rows.Add(4, 2, 6, true, "Technical input provided", now.AddDays(-140), now.AddDays(-140));
            MeetingMembers.Rows.Add(5, 3, 2, true, "Daily update given", now.AddDays(-130), now.AddDays(-130));
            MeetingMembers.Rows.Add(6, 3, 6, false, "On leave", now.AddDays(-130), now.AddDays(-130));
            MeetingMembers.Rows.Add(7, 4, 4, true, "Marketing presentation", now.AddDays(-120), now.AddDays(-120));
            MeetingMembers.Rows.Add(8, 4, 1, true, "HR coordination", now.AddDays(-120), now.AddDays(-120));
            MeetingMembers.Rows.Add(9, 5, 5, true, "Operations report", now.AddDays(-110), now.AddDays(-110));
            MeetingMembers.Rows.Add(10, 5, 2, true, "Technical support", now.AddDays(-110), now.AddDays(-110));
            MeetingMembers.Rows.Add(11, 6, 1, true, "Policy discussion", now.AddDays(-100), now.AddDays(-100));
            MeetingMembers.Rows.Add(12, 7, 2, true, "Client requirements", now.AddDays(-90), now.AddDays(-90));
            MeetingMembers.Rows.Add(13, 8, 3, true, "Financial review", now.AddDays(-80), now.AddDays(-80));
            MeetingMembers.Rows.Add(14, 9, 4, true, "Product strategy", now.AddDays(-70), now.AddDays(-70));
            MeetingMembers.Rows.Add(15, 10, 5, true, "Process improvement", now.AddDays(-60), now.AddDays(-60));
            MeetingMembers.Rows.Add(16, 11, 1, true, "Team building", now.AddDays(-50), now.AddDays(-50));
            MeetingMembers.Rows.Add(17, 12, 2, true, "Technology planning", now.AddDays(-45), now.AddDays(-45));
            MeetingMembers.Rows.Add(18, 13, 3, true, "Financial analysis", now.AddDays(-40), now.AddDays(-40));
            MeetingMembers.Rows.Add(19, 14, 4, true, "Brand strategy", now.AddDays(-35), now.AddDays(-35));
            MeetingMembers.Rows.Add(20, 15, 5, true, "Supply chain", now.AddDays(-30), now.AddDays(-30));
            MeetingMembers.Rows.Add(21, 21, 1, true, "Weekly standup", now.AddDays(-2), now.AddDays(-2));
            MeetingMembers.Rows.Add(22, 22, 2, true, "Sprint planning", now.AddDays(-1), now.AddDays(-1));

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