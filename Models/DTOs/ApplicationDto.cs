using System.ComponentModel.DataAnnotations;

namespace SAIS.Models.DTOs
{
    public class ApplicationDto
    {
        public int ApplicationId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? ApplicantSignedDate { get; set; }
        public DateTime? OfficerSignedDate { get; set; }
        public int ApplicantId { get; set; }
        public string? ApplicantName { get; set; }
        public string? ApplicantIdNumber { get; set; }
        public int OfficerId { get; set; }
        public string? OfficerName { get; set; }
        public List<int> ProgramIds { get; set; } = new List<int>();
        public List<string> ProgramNames { get; set; } = new List<string>();
        public string Status { get; set; } = "Draft";
    }

    public class ApplicationCreateDto
    {
        [Required(ErrorMessage = "Application Date is required")]
        [DataType(DataType.Date)]
        public DateTime ApplicationDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime? ApplicantSignedDate { get; set; }

        [Required(ErrorMessage = "Officer is required")]
        public int? OfficerId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? OfficerSignedDate { get; set; }

        [Required(ErrorMessage = "At least one program must be selected")]
        public List<int> ProgramIds { get; set; } = new List<int>();

        // For quick application entry
        public string? ApplicantIdNumber { get; set; }
        public int? ApplicantId { get; set; }
    }

    public class ApplicationEditDto
    {
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Application Date is required")]
        [DataType(DataType.Date)]
        public DateTime ApplicationDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ApplicantSignedDate { get; set; }

        [Required(ErrorMessage = "Officer is required")]
        public int OfficerId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? OfficerSignedDate { get; set; }

        [Required(ErrorMessage = "At least one program must be selected")]
        public List<int> ProgramIds { get; set; } = new List<int>();
    }

    public class ApplicationListDto
    {
        public int ApplicationId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? ApplicantSignedDate { get; set; }
        public DateTime? OfficerSignedDate { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string ApplicantIdNumber { get; set; } = string.Empty;
        public string OfficerName { get; set; } = string.Empty;
        public string ProgramNames { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class ApplicationDetailsDto
    {
        public int ApplicationId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? ApplicantSignedDate { get; set; }
        public DateTime? OfficerSignedDate { get; set; }
        public int ApplicantId { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string ApplicantIdNumber { get; set; } = string.Empty;
        public string ApplicantPhone { get; set; } = string.Empty;
        public string ApplicantAddress { get; set; } = string.Empty;
        public int OfficerId { get; set; }
        public string OfficerName { get; set; } = string.Empty;
        public string OfficerTitle { get; set; } = string.Empty;
        public List<ProgramSelectionDto> Programs { get; set; } = new List<ProgramSelectionDto>();
        public string Status { get; set; } = string.Empty;
    }

    public class ProgramSelectionDto
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
