using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class DepartmentModel
    {
        [Key]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
        public string DepartmentName { get; set; }

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Modified date is required.")]
        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; }
    }
}
