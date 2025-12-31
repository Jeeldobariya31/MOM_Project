using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingModel
    {
        [Key]
        public int MeetingID { get; set; }

        [Required(ErrorMessage = "Meeting date is required.")]
        [Display(Name = "Meeting Date")]
        public DateTime MeetingDate { get; set; }

        [Required(ErrorMessage = "Meeting venue is required.")]
        [Display(Name = "Venue")]
        public int MeetingVenueID { get; set; }

        [Required(ErrorMessage = "Meeting type is required.")]
        [Display(Name = "Meeting Type")]
        public int MeetingTypeID { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department")]
        public int DepartmentID { get; set; }

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        [Display(Name = "Meeting Description")]
        public string? MeetingDescription { get; set; }

        [StringLength(250, ErrorMessage = "Document path cannot exceed 250 characters.")]
        [Display(Name = "Document Path")]
        public string? DocumentPath { get; set; }

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Modified date is required.")]
        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; }

        [Display(Name = "Is Cancelled")]
        public bool? IsCancelled { get; set; }

        [Display(Name = "Cancellation Date & Time")]
        public DateTime? CancellationDateTime { get; set; }

        [StringLength(250, ErrorMessage = "Cancellation reason cannot exceed 250 characters.")]
        [Display(Name = "Cancellation Reason")]
        public string? CancellationReason { get; set; }
    }
}
