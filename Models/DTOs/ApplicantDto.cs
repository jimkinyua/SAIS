using System.ComponentModel.DataAnnotations;

namespace SAIS.Models.DTOs
{
    public class ApplicantDto
    {
        public int ApplicantId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Middle Name cannot exceed 50 characters")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID Number is required")]
        [MaxLength(20, ErrorMessage = "ID Number cannot exceed 20 characters")]
        public string IdNumber { get; set; } = string.Empty;

        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public int GenderId { get; set; }
        public string? GenderName { get; set; }

        [Required(ErrorMessage = "Marital Status is required")]
        public int MaritalStatusId { get; set; }
        public string? MaritalStatusName { get; set; }

        [Required(ErrorMessage = "Village is required")]
        public int VillageId { get; set; }
        public string? VillageName { get; set; }

        [MaxLength(200, ErrorMessage = "Postal Address cannot exceed 200 characters")]
        public string PostalAddress { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Physical Address cannot exceed 200 characters")]
        public string PhysicalAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public List<string> PhoneNumbers { get; set; } = new List<string>();
        public List<ApplicationListDto> Applications { get; set; } = new List<ApplicationListDto>();

        // Geographic hierarchy for display
        public string? CountyName { get; set; }
        public string? SubCountyName { get; set; }
        public string? LocationName { get; set; }
        public string? SubLocationName { get; set; }
    }

    public class ApplicantCreateDto
    {
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Middle Name cannot exceed 50 characters")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID Number is required")]
        [MaxLength(20, ErrorMessage = "ID Number cannot exceed 20 characters")]
        public string IdNumber { get; set; } = string.Empty;

        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public int GenderId { get; set; }

        [Required(ErrorMessage = "Marital Status is required")]
        public int MaritalStatusId { get; set; }

        [Required(ErrorMessage = "Village is required")]
        public int VillageId { get; set; }

        [MaxLength(200, ErrorMessage = "Postal Address cannot exceed 200 characters")]
        public string PostalAddress { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Physical Address cannot exceed 200 characters")]
        public string PhysicalAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public List<string> PhoneNumbers { get; set; } = new List<string>();
    }

    public class ApplicantEditDto
    {
        public int ApplicantId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Middle Name cannot exceed 50 characters")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID Number is required")]
        [MaxLength(20, ErrorMessage = "ID Number cannot exceed 20 characters")]
        public string IdNumber { get; set; } = string.Empty;

        [Range(0, 150, ErrorMessage = "Age must be between 0 and 150")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public int GenderId { get; set; }

        [Required(ErrorMessage = "Marital Status is required")]
        public int MaritalStatusId { get; set; }

        [Required(ErrorMessage = "Village is required")]
        public int VillageId { get; set; }

        [MaxLength(200, ErrorMessage = "Postal Address cannot exceed 200 characters")]
        public string PostalAddress { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Physical Address cannot exceed 200 characters")]
        public string PhysicalAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public List<string> PhoneNumbers { get; set; } = new List<string>();
    }

    public class ApplicantListDto
    {
        public int ApplicantId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public int Age { get; set; }
        public string GenderName { get; set; } = string.Empty;
        public string MaritalStatusName { get; set; } = string.Empty;
        public string VillageName { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;
        public string PhoneNumbers { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public int ApplicationCount { get; set; }
    }
}
