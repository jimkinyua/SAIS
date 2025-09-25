using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class GenderCategory
    {
        [Key]
        public int GenderCategoryId { get; set; }

        [Required, MaxLength(20)]
        public string GenderCategoryName { get; set; } = string.Empty;

        public ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();
    }
}
