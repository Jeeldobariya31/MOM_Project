using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models
{
    public class MeetingTypeModel
    {
        [Key]
        public int MeetingTypeID { get; set; }

        [Required(ErrorMessage = "Meeting type name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Meeting type name must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s&.-]+$", ErrorMessage = "Meeting type name contains invalid characters")]
        [Display(Name = "Meeting Type Name")]
        public string MeetingTypeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Remarks are required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Remarks must be between 5 and 100 characters")]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; } = string.Empty;

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;

        // Navigation properties
        [NotMapped]
        public int MeetingCount { get; set; }
    }
}