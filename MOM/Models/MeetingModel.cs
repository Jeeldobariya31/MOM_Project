using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models
{
    public class MeetingModel
    {
        [Key]
        public int MeetingID { get; set; }

        [Required(ErrorMessage = "Meeting date is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Meeting Date & Time")]
        [FutureDate(ErrorMessage = "Meeting date must be in the future")]
        public DateTime MeetingDate { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        [Display(Name = "Meeting Venue")]
        public int MeetingVenueID { get; set; }

        [Required(ErrorMessage = "Meeting type is required")]
        [Display(Name = "Meeting Type")]
        public int MeetingTypeID { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentID { get; set; }

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,'-]*$", ErrorMessage = "Invalid characters in description")]
        [Display(Name = "Meeting Description")]
        public string MeetingDescription { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Document path cannot exceed 250 characters")]
        [RegularExpression(@"^.*\.(pdf|docx|xlsx)$", ErrorMessage = "Only PDF, DOCX, XLSX files are allowed")]
        [Display(Name = "Document Path")]
        public string DocumentPath { get; set; } = string.Empty;

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;

        [Display(Name = "Is Cancelled")]
        public bool IsCancelled { get; set; }

        [Display(Name = "Cancellation Date & Time")]
        public DateTime? CancellationDateTime { get; set; }

        [StringLength(250, ErrorMessage = "Cancellation reason cannot exceed 250 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,'-]*$", ErrorMessage = "Invalid characters in cancellation reason")]
        [Display(Name = "Cancellation Reason")]
        [RequiredIf("IsCancelled", true, ErrorMessage = "Cancellation reason is required when meeting is cancelled")]
        public string CancellationReason { get; set; } = string.Empty;

        // Navigation properties
        [NotMapped]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; } = string.Empty;

        [NotMapped]
        [Display(Name = "Meeting Type Name")]
        public string MeetingTypeName { get; set; } = string.Empty;

        [NotMapped]
        [Display(Name = "Meeting Venue Name")]
        public string MeetingVenueName { get; set; } = string.Empty;

        [NotMapped]
        public int MemberCount { get; set; }

        [NotMapped]
        public int PresentCount { get; set; }
    }

    // Custom validation attributes
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.Now;
            }
            return true; // Let Required attribute handle null values
        }
    }

    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly object _desiredValue;

        public RequiredIfAttribute(string propertyName, object desiredValue)
        {
            _propertyName = propertyName;
            _desiredValue = desiredValue;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_propertyName);
            if (property == null)
                return new ValidationResult($"Unknown property: {_propertyName}");

            var propertyValue = property.GetValue(validationContext.ObjectInstance);
            
            if (Equals(propertyValue, _desiredValue))
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required");
                }
            }

            return ValidationResult.Success;
        }
    }
}
