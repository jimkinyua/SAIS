using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class PhoneNumber
    {
        [Key]
        public int PhoneId { get; set; }

        [Required] [ForeignKey("Applicant")]
        public int ApplicantId { get; set; }

        [Required, MaxLength(20)] [Phone]
        public string Number { get; set; } = string.Empty;
        public Applicant Applicant { get; set; } = null!;
    }
}
