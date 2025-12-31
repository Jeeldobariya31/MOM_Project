using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class StaffModel
    {
        [Key]
        public int StaffID { get; set; }

        [Required(ErrorMessage = "Department ID is required.")]
        [Display(Name = "Department")]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Staff name is required.")]
        [StringLength(50, ErrorMessage = "Staff name cannot exceed 50 characters.")]
        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [StringLength(20, ErrorMessage = "Mobile number cannot exceed 20 characters.")]
        [Display(Name = "Mobile Number")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(50, ErrorMessage = "Email address cannot exceed 50 characters.")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [StringLength(250, ErrorMessage = "Remarks cannot exceed 250 characters.")]
        public string? Remarks { get; set; }

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Modified date is required.")]
        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; }
    }
}
