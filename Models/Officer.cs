using System.ComponentModel.DataAnnotations;

namespace SAIS.Models
{
    public class Officer
    {
        [Key]
        public int OfficerId { get; set; }

        [Required, MaxLength(100)]
        public string OfficerName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Designation { get; set; } = string.Empty;

        //One officer can sign many applications
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
