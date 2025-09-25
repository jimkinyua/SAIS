using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class MaritalStatus
    {
        [Key]
        public int MaritalStatusId { get; set; }

        [Required, MaxLength(30)]
        public string StatusName { get; set; } = string.Empty;
        public ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();
    }
}
