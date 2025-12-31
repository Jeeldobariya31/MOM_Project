using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingType
    {
        [Key]
        public int MeetingTypeID { get; set; }

        [Required(ErrorMessage = "Meeting type name is required.")]
        [StringLength(100, ErrorMessage = "Meeting type name cannot exceed 100 characters.")]
        [Display(Name = "Meeting Type Name")]
        public string MeetingTypeName { get; set; }

        [Required(ErrorMessage = "Remarks are required.")]
        [StringLength(100, ErrorMessage = "Remarks cannot exceed 100 characters.")]
        public string Remarks { get; set; }

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Modified date is required.")]
        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; }
    }
}
