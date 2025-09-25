using System.ComponentModel.DataAnnotations;

namespace SAIS.Models.DTOs
{
    public class OfficerDto
    {
        public int OfficerId { get; set; }
        public string OfficerName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int ApplicationCount { get; set; }
    }

    public class OfficerCreateDto
    {
        [Required(ErrorMessage = "Officer Name is required")]
        [MaxLength(100, ErrorMessage = "Officer Name cannot exceed 100 characters")]
        public string OfficerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required")]
        [MaxLength(100, ErrorMessage = "Designation cannot exceed 100 characters")]
        public string Designation { get; set; } = string.Empty;
    }

    public class OfficerEditDto
    {
        public int OfficerId { get; set; }

        [Required(ErrorMessage = "Officer Name is required")]
        [MaxLength(100, ErrorMessage = "Officer Name cannot exceed 100 characters")]
        public string OfficerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required")]
        [MaxLength(100, ErrorMessage = "Designation cannot exceed 100 characters")]
        public string Designation { get; set; } = string.Empty;
    }

    public class OfficerListDto
    {
        public int OfficerId { get; set; }
        public string OfficerName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int ApplicationCount { get; set; }
    }
}
