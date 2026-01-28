using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models
{
    public class StaffModel
    {
        [Key]
        public int StaffID { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Staff name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Staff name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s.'-]+$", ErrorMessage = "Staff name contains invalid characters")]
        [Display(Name = "Staff Name")]
        public string StaffName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Mobile number must be between 10 and 20 characters")]
        [RegularExpression(@"^[\+]?[0-9\s\-\(\)]+$", ErrorMessage = "Invalid mobile number format")]
        [Display(Name = "Mobile Number")]
        public string MobileNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(50, ErrorMessage = "Email address cannot exceed 50 characters")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Remarks cannot exceed 250 characters")]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;

        // Navigation properties
        [NotMapped]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; } = string.Empty;

        [NotMapped]
        public int MeetingCount { get; set; }
    }
}
