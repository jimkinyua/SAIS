using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required, MaxLength(100)]
        public string LocationName { get; set; } = string.Empty;

        [ForeignKey("SubCounty")]
        public int SubCountyId { get; set; }

        public SubCounty SubCounty { get; set; } = null!;
        public ICollection<SubLocation> SubLocations { get; set; } = new List<SubLocation>();
    }
}
