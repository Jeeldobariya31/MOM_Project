using System.Diagnostics;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Services;

namespace MOM.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataService _dataService;

        public HomeController()
        {
            _dataService = DataService.Instance;
        }

        public IActionResult Index()
        {
            try
            {
                // Use the new DataService dashboard method
                var stats = _dataService.GetDashboardStats();
                
                var now = DateTime.Now;
                var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
                var startOfMonth = new DateTime(now.Year, now.Month, 1);

                // Get basic statistics from the new method
                var totalMeetings = stats.GetValueOrDefault("TotalMeetings", 0);
                var totalDepartments = stats.GetValueOrDefault("TotalDepartments", 0);
                var totalStaff = stats.GetValueOrDefault("TotalStaff", 0);
                var totalVenues = stats.GetValueOrDefault("TotalMeetingVenues", 0);
                var cancelledMeetings = stats.GetValueOrDefault("CancelledMeetings", 0);
                var activeMeetings = stats.GetValueOrDefault("ActiveMeetings", 0);
                var presentMembers = stats.GetValueOrDefault("PresentMembers", 0);
                var absentMembers = stats.GetValueOrDefault("AbsentMembers", 0);
                var totalMeetingMembers = stats.GetValueOrDefault("TotalMeetingMembers", 0);

                // Calculate attendance rate
                var attendanceRate = totalMeetingMembers > 0 ? (double)presentMembers / totalMeetingMembers * 100 : 0;

                // Meeting Analytics using DataService data
                var meetings = _dataService.Meetings?.AsEnumerable() ?? Enumerable.Empty<System.Data.DataRow>();
                
                var upcomingMeetings = meetings
                    .Where(m => 
                    {
                        var isCancelledValue = m["IsCancelled"];
                        bool isCancelled = isCancelledValue != DBNull.Value && Convert.ToBoolean(isCancelledValue);
                        return m.Field<DateTime>("MeetingDate") > now && !isCancelled;
                    })
                    .Count();

                var recentMeetings = meetings
                    .Where(m => m.Field<DateTime>("MeetingDate") >= startOfWeek && m.Field<DateTime>("MeetingDate") <= now)
                    .Count();

                var thisMonthMeetings = meetings
                    .Where(m => m.Field<DateTime>("MeetingDate") >= startOfMonth && m.Field<DateTime>("MeetingDate") <= now)
                    .Count();

                // Department-wise meeting distribution
                var departmentMeetings = meetings
                    .Where(m => 
                    {
                        var isCancelledValue = m["IsCancelled"];
                        return isCancelledValue == DBNull.Value || !Convert.ToBoolean(isCancelledValue);
                    })
                    .GroupBy(m => m.Field<int>("DepartmentID"))
                    .Select(g => new {
                        DepartmentID = g.Key,
                        Count = g.Count(),
                        DepartmentName = _dataService.Departments?.AsEnumerable()
                            .FirstOrDefault(d => d.Field<int>("DepartmentID") == g.Key)
                            ?.Field<string>("DepartmentName") ?? "Unknown"
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                // Meeting type distribution
                var meetingTypeDistribution = meetings
                    .Where(m => 
                    {
                        var isCancelledValue = m["IsCancelled"];
                        return isCancelledValue == DBNull.Value || !Convert.ToBoolean(isCancelledValue);
                    })
                    .GroupBy(m => m.Field<int>("MeetingTypeID"))
                    .Select(g => new {
                        MeetingTypeID = g.Key,
                        Count = g.Count(),
                        MeetingTypeName = _dataService.MeetingTypes?.AsEnumerable()
                            .FirstOrDefault(mt => mt.Field<int>("MeetingTypeID") == g.Key)
                            ?.Field<string>("MeetingTypeName") ?? "Unknown"
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                // Monthly meeting trends (last 6 months)
                var monthlyTrends = new List<object>();
                for (int i = 5; i >= 0; i--)
                {
                    var monthStart = now.AddMonths(-i).AddDays(-(now.AddMonths(-i).Day - 1));
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                    
                    var monthMeetings = meetings
                        .Where(m => 
                        {
                            var isCancelledValue = m["IsCancelled"];
                            bool isCancelled = isCancelledValue != DBNull.Value && Convert.ToBoolean(isCancelledValue);
                            return m.Field<DateTime>("MeetingDate") >= monthStart && 
                                   m.Field<DateTime>("MeetingDate") <= monthEnd &&
                                   !isCancelled;
                        })
                        .Count();

                    monthlyTrends.Add(new {
                        Month = monthStart.ToString("MMM yyyy"),
                        Count = monthMeetings
                    });
                }

                // Venue utilization
                var venueUtilization = meetings
                    .Where(m => 
                    {
                        var isCancelledValue = m["IsCancelled"];
                        return isCancelledValue == DBNull.Value || !Convert.ToBoolean(isCancelledValue);
                    })
                    .GroupBy(m => m.Field<int>("MeetingVenueID"))
                    .Select(g => new {
                        VenueID = g.Key,
                        Count = g.Count(),
                        VenueName = _dataService.MeetingVenues?.AsEnumerable()
                            .FirstOrDefault(v => v.Field<int>("MeetingVenueID") == g.Key)
                            ?.Field<string>("MeetingVenueName") ?? "Unknown"
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                // Active members count
                var activeMembers = _dataService.MeetingMembers?.AsEnumerable()
                    ?.Select(m => m.Field<int>("StaffID"))
                    ?.Distinct()
                    ?.Count() ?? 0;

                // Recent meetings for display
                var recentMeetingsData = meetings
                    .Where(m => m.Field<DateTime>("MeetingDate") >= now.AddDays(-30))
                    .OrderByDescending(m => m.Field<DateTime>("MeetingDate"))
                    .Take(5)
                    .Select(m => new {
                        MeetingID = m.Field<int>("MeetingID"),
                        MeetingDate = m.Field<DateTime>("MeetingDate"),
                        Description = m.Field<string>("MeetingDescription") ?? "",
                        DepartmentName = _dataService.Departments?.AsEnumerable()
                            .FirstOrDefault(d => d.Field<int>("DepartmentID") == m.Field<int>("DepartmentID"))
                            ?.Field<string>("DepartmentName") ?? "Unknown",
                        VenueName = _dataService.MeetingVenues?.AsEnumerable()
                            .FirstOrDefault(v => v.Field<int>("MeetingVenueID") == m.Field<int>("MeetingVenueID"))
                            ?.Field<string>("MeetingVenueName") ?? "Unknown",
                        MeetingTypeName = _dataService.MeetingTypes?.AsEnumerable()
                            .FirstOrDefault(mt => mt.Field<int>("MeetingTypeID") == m.Field<int>("MeetingTypeID"))
                            ?.Field<string>("MeetingTypeName") ?? "Unknown",
                        IsCancelled = m["IsCancelled"] != DBNull.Value && Convert.ToBoolean(m["IsCancelled"])
                    })
                    .ToArray();

                // Upcoming meetings for display
                var upcomingMeetingsData = meetings
                    .Where(m => 
                    {
                        var isCancelledValue = m["IsCancelled"];
                        bool isCancelled = isCancelledValue != DBNull.Value && Convert.ToBoolean(isCancelledValue);
                        return m.Field<DateTime>("MeetingDate") > now && !isCancelled;
                    })
                    .OrderBy(m => m.Field<DateTime>("MeetingDate"))
                    .Take(5)
                    .Select(m => new {
                        MeetingID = m.Field<int>("MeetingID"),
                        MeetingDate = m.Field<DateTime>("MeetingDate"),
                        Description = m.Field<string>("MeetingDescription") ?? "",
                        DepartmentName = _dataService.Departments?.AsEnumerable()
                            .FirstOrDefault(d => d.Field<int>("DepartmentID") == m.Field<int>("DepartmentID"))
                            ?.Field<string>("DepartmentName") ?? "Unknown",
                        VenueName = _dataService.MeetingVenues?.AsEnumerable()
                            .FirstOrDefault(v => v.Field<int>("MeetingVenueID") == m.Field<int>("MeetingVenueID"))
                            ?.Field<string>("MeetingVenueName") ?? "Unknown",
                        MeetingTypeName = _dataService.MeetingTypes?.AsEnumerable()
                            .FirstOrDefault(mt => mt.Field<int>("MeetingTypeID") == m.Field<int>("MeetingTypeID"))
                            ?.Field<string>("MeetingTypeName") ?? "Unknown"
                    })
                    .ToArray();

                // Create dashboard data using the new DataService stats
                var dashboardData = new
                {
                    // Basic Stats from DataService
                    TotalMeetings = totalMeetings,
                    TotalDepartments = totalDepartments,
                    TotalStaff = totalStaff,
                    TotalVenues = totalVenues,
                    UpcomingMeetings = upcomingMeetings,
                    RecentMeetings = recentMeetings,
                    CancelledMeetings = cancelledMeetings,
                    ActiveMeetings = activeMeetings,
                    ThisMonthMeetings = thisMonthMeetings,
                    ActiveMembers = activeMembers,
                    AttendanceRate = Math.Round(attendanceRate, 1),
                    PresentMembers = presentMembers,
                    AbsentMembers = absentMembers,
                    TotalMeetingMembers = totalMeetingMembers,

                    // Chart Data
                    DepartmentMeetings = departmentMeetings.ToArray(),
                    MeetingTypeDistribution = meetingTypeDistribution.ToArray(),
                    MonthlyTrends = monthlyTrends.ToArray(),
                    VenueUtilization = venueUtilization.ToArray(),

                    // Recent Data
                    RecentMeetingsData = recentMeetingsData,
                    UpcomingMeetingsData = upcomingMeetingsData
                };

                // Pass data using ViewData and ViewBag for compatibility
                ViewData["DashboardData"] = dashboardData;
                ViewBag.DashboardData = dashboardData;
                
                // Add debug information
                ViewBag.DebugInfo = $"Data loaded: Meetings={totalMeetings}, Departments={totalDepartments}, Staff={totalStaff}, Present={presentMembers}, Absent={absentMembers}";
                
                return View();
            }
            catch (Exception ex)
            {
                // Log error and provide fallback data
                var fallbackData = new
                {
                    TotalMeetings = 0,
                    TotalDepartments = 0,
                    TotalStaff = 0,
                    TotalVenues = 0,
                    UpcomingMeetings = 0,
                    RecentMeetings = 0,
                    CancelledMeetings = 0,
                    ActiveMeetings = 0,
                    ThisMonthMeetings = 0,
                    ActiveMembers = 0,
                    AttendanceRate = 0.0,
                    PresentMembers = 0,
                    AbsentMembers = 0,
                    TotalMeetingMembers = 0,
                    DepartmentMeetings = new object[0],
                    MeetingTypeDistribution = new object[0],
                    MonthlyTrends = new object[0],
                    VenueUtilization = new object[0],
                    RecentMeetingsData = new object[0],
                    UpcomingMeetingsData = new object[0]
                };
                
                ViewBag.DashboardData = fallbackData;
                ViewBag.ErrorMessage = $"Unable to load dashboard statistics: {ex.Message}";
                ViewBag.DebugInfo = $"Exception occurred: {ex.GetType().Name} - {ex.Message}";
                return View();
            }
        }

        public IActionResult About()
        {
            ViewData["Title"] = "About MOM System";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Title"] = "Contact Us";
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = "Privacy Policy";
            return View();
        }

        public IActionResult Terms()
        {
            ViewData["Title"] = "Terms & Conditions";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}