using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAIS.Models
{
    public class Applicant
    {
        [Key]
        public int ApplicantId { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string IdNumber { get; set; } = string.Empty;

        [Range(0, 150)]
        public int Age { get; set; }

        [Required]
        [ForeignKey("GenderCategory")]
        public int GenderId { get; set; }

        [Required]
        [ForeignKey("MaritalStatus")]
        public int MaritalStatusId { get; set; }

        [Required]
        [ForeignKey("Village")]
        public int VillageId { get; set; }

        [MaxLength(200)]
        public string PostalAddress { get; set; } = string.Empty;

        [MaxLength(200)]
        public string PhysicalAddress { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // Navigation
        public virtual GenderCategory GenderCategory { get; set; } = null!;
        public virtual MaritalStatus MaritalStatus { get; set; } = null!;
        public virtual Village Village { get; set; } = null!;
        public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    }
}
