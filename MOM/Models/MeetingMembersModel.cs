using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models
{
    public class MeetingMembersModel
    {
        [Key]
        public int MeetingMemberID { get; set; }

        [Required(ErrorMessage = "Meeting is required")]
        [Display(Name = "Meeting")]
        public int MeetingID { get; set; }

        [Required(ErrorMessage = "Staff member is required")]
        [Display(Name = "Staff Member")]
        public int StaffID { get; set; }

        [Required(ErrorMessage = "Attendance status is required")]
        [Display(Name = "Is Present")]
        public bool IsPresent { get; set; }

        [StringLength(250, ErrorMessage = "Remarks cannot exceed 250 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,'-]*$", ErrorMessage = "Invalid characters in remarks")]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;

        // Navigation properties
        [NotMapped]
        [Display(Name = "Staff Name")]
        public string StaffName { get; set; } = string.Empty;

        [NotMapped]
        [Display(Name = "Department")]
        public string DepartmentName { get; set; } = string.Empty;

        [NotMapped]
        [Display(Name = "Meeting Description")]
        public string MeetingDescription { get; set; } = string.Empty;

        [NotMapped]
        [Display(Name = "Meeting Date")]
        public DateTime MeetingDate { get; set; }
    }
}
