using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingVenueModel
    {
        [Key]
        public int MeetingVenueID { get; set; }

        [Required(ErrorMessage = "Meeting venue name is required.")]
        [StringLength(100, ErrorMessage = "Venue name cannot exceed 100 characters.")]
        [Display(Name = "Meeting Venue Name")]
        public string MeetingVenueName { get; set; }

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Modified date is required.")]
        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; }
    }
}
