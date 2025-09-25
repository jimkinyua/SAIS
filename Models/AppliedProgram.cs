using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class AppliedProgram
    {
        [Key]
        public int AppliedProgramId { get; set; }

        [ForeignKey("Application")]
        public int ApplicationId { get; set; }

        [ForeignKey("SocialAssistanceProgram")]
        public int ProgramId { get; set; }

        public Application Application { get; set; } = null!;
        public SocialAssistanceProgram SocialAssistanceProgram { get; set; } = null!;


    }
}
