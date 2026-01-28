using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models.ViewModels
{
    public class MeetingViewModel
    {
        public MeetingModel Meeting { get; set; } = new MeetingModel();
        
        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> MeetingTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> MeetingVenues { get; set; } = new List<SelectListItem>();
        
        [Display(Name = "Upload Document")]
        public IFormFile? DocumentFile { get; set; }
    }

    public class MeetingMemberViewModel
    {
        public MeetingMembersModel MeetingMember { get; set; } = new MeetingMembersModel();
        
        public List<SelectListItem> Meetings { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Staff { get; set; } = new List<SelectListItem>();
        
        // For bulk add
        public List<int> SelectedStaffIds { get; set; } = new List<int>();
        public bool SetAllPresent { get; set; } = true;
        public string BulkRemarks { get; set; } = string.Empty;
    }

    public class StaffViewModel
    {
        public StaffModel Staff { get; set; } = new StaffModel();
        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
    }

    public class DashboardViewModel
    {
        public int TotalMeetings { get; set; }
        public int UpcomingMeetings { get; set; }
        public int TotalStaff { get; set; }
        public int TotalDepartments { get; set; }
        public int CancelledMeetings { get; set; }
        
        public List<MeetingModel> RecentMeetings { get; set; } = new List<MeetingModel>();
        public List<MeetingModel> TodaysMeetings { get; set; } = new List<MeetingModel>();
        
        // Chart data
        public Dictionary<string, int> MeetingsByDepartment { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> MeetingsByType { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> MeetingsByMonth { get; set; } = new Dictionary<string, int>();
    }
}