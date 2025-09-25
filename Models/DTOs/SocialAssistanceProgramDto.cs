using System.ComponentModel.DataAnnotations;

namespace SAIS.Models.DTOs
{
    public class SocialAssistanceProgramDto
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public int ApplicationCount { get; set; }
    }

    public class SocialAssistanceProgramCreateDto
    {
        [Required(ErrorMessage = "Program Name is required")]
        [MaxLength(100, ErrorMessage = "Program Name cannot exceed 100 characters")]
        public string ProgramName { get; set; } = string.Empty;
    }

    public class SocialAssistanceProgramEditDto
    {
        public int ProgramId { get; set; }

        [Required(ErrorMessage = "Program Name is required")]
        [MaxLength(100, ErrorMessage = "Program Name cannot exceed 100 characters")]
        public string ProgramName { get; set; } = string.Empty;
    }

    public class SocialAssistanceProgramListDto
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public int ApplicationCount { get; set; }
    }
}
