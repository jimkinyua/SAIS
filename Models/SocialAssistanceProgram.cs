using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class SocialAssistanceProgram
    {
        [Key]
        public int ProgramId { get; set; }

        [Required, MaxLength(100)]
        public string ProgramName { get; set; } = string.Empty;
        public ICollection<AppliedProgram> AppliedPrograms { get; set; } = new List<AppliedProgram>();
    }
}
