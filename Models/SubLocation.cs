using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class SubLocation
    {
        [Key]
        public int SubLocationId { get; set; }

        [Required, MaxLength(100)]
        public string SubLocationName { get; set; } = string.Empty;

        [ForeignKey("Location")]
        public int LocationId { get; set; }

        public Location Location { get; set; } = null!;
        public ICollection<Village> Villages { get; set; } = new List<Village>();
    }
}
