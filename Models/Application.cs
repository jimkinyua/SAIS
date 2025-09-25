using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class Application
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required] [ForeignKey("Applicant")]
        public int ApplicantId { get; set; }

        [Required][ForeignKey("Officer")]
        public int OfficerId { get; set; }

        [DataType(DataType.Date)]
        public DateTime ApplicationDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ApplicantSignedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime OfficerSignedDate { get; set; }

        public Applicant Applicant { get; set; } = null!;
        public Officer Officer { get; set; } = null!;
        public ICollection<AppliedProgram> AppliedPrograms { get; set; } = new List<AppliedProgram>();
    }
}
