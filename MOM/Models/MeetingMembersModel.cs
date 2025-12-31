using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingMembersModel
    {
        [Key]
        public int MeetingMemberID { get; set; }

        [Required(ErrorMessage = "Meeting ID is required.")]
        public int MeetingID { get; set; }

        [Required(ErrorMessage = "Staff ID is required.")]
        public int StaffID { get; set; }

        [Required(ErrorMessage = "Attendance status is required.")]
        [Display(Name = "Is Present")]
        public bool IsPresent { get; set; }

        [StringLength(250, ErrorMessage = "Remarks cannot exceed 250 characters.")]
        public string? Remarks { get; set; }

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Modified date is required.")]
        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; }
    }
}
