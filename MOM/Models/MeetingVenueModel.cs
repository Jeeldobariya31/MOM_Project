using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models
{
    public class MeetingVenueModel
    {
        [Key]
        public int MeetingVenueID { get; set; }

        [Required(ErrorMessage = "Meeting venue name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Meeting venue name must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s&.-]+$", ErrorMessage = "Meeting venue name contains invalid characters")]
        [Display(Name = "Meeting Venue Name")]
        public string MeetingVenueName { get; set; } = string.Empty;

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;

        // Navigation properties
        [NotMapped]
        public int MeetingCount { get; set; }
    }
}
