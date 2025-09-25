using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class SubCounty
    {
        [Key]
        public int SubCountyId { get; set; }

        [Required, MaxLength(100)]
        public string SubCountyName { get; set; } = string.Empty;

        [ForeignKey("County")]
        public int CountyId { get; set; }

        public County County { get; set; } = null!;
        public ICollection<Location> Locations { get; set; } = new List<Location>();
    }
}
