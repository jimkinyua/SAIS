using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class County
    {
        [Key]
        public int CountyId { get; set; }

        [Required, MaxLength(100)]
        public string CountyName { get; set; } = string.Empty;
        public ICollection<SubCounty> SubCounties { get; set; } = new List<SubCounty>();
    }
}
