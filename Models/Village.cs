using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class Village
    {
        [Key]
        public int VillageId { get; set; }

        [Required, MaxLength(100)]
        public string VillageName { get; set; } = string.Empty;

        [ForeignKey("SubLocation")]
        public int SubLocationId { get; set; }
        public SubLocation SubLocation { get; set; } = null!;
        public ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();

    }
}
