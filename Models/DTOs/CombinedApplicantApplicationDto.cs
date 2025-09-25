using System.ComponentModel.DataAnnotations;

namespace SAIS.Models.DTOs
{
    public class CombinedApplicantApplicationDto
    {
        // Applicant Details
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters.")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID number is required.")]
        [StringLength(20, ErrorMessage = "ID number cannot exceed 20 characters.")]
        public string IdNumber { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120.")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public int GenderId { get; set; }

        [Required(ErrorMessage = "Marital status is required.")]
        public int MaritalStatusId { get; set; }

        [Required(ErrorMessage = "Village is required.")]
        public int VillageId { get; set; }

        [StringLength(200, ErrorMessage = "Postal address cannot exceed 200 characters.")]
        public string? PostalAddress { get; set; }

        [StringLength(200, ErrorMessage = "Physical address cannot exceed 200 characters.")]
        public string? PhysicalAddress { get; set; }

        [Required(ErrorMessage = "At least one phone number is required.")]
        public List<string> PhoneNumbers { get; set; } = new List<string>();

        // Application Details
        [Required(ErrorMessage = "Application date is required.")]
        [DataType(DataType.Date)]
        public DateTime ApplicationDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Applicant signed date is required.")]
        [DataType(DataType.Date)]
        public DateTime ApplicantSignedDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Officer signed date is required.")]
        [DataType(DataType.Date)]
        public DateTime OfficerSignedDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Officer is required.")]
        public int OfficerId { get; set; }

        [Required(ErrorMessage = "At least one program must be selected.")]
        public List<int> ProgramIds { get; set; } = new List<int>();

        // Display properties for dropdowns
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GenderCategories { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> MaritalStatuses { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Counties { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Officers { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        public List<SocialAssistanceProgram> Programs { get; set; } = new List<SocialAssistanceProgram>();

        // Geographic hierarchy for cascading dropdowns
        public int? SelectedCountyId { get; set; }
        public int? SelectedSubCountyId { get; set; }
        public int? SelectedLocationId { get; set; }
        public int? SelectedSubLocationId { get; set; }
    }
}
