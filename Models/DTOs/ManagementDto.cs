using System.ComponentModel.DataAnnotations;

namespace SAIS.Models.DTOs
{
    // County DTOs
    public class CountyDto
    {
        public int CountyId { get; set; }
        public string CountyName { get; set; } = string.Empty;
        public int SubCountiesCount { get; set; }
    }

    // SubCounty DTOs
    public class SubCountyDto
    {
        public int SubCountyId { get; set; }
        public string SubCountyName { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;
        public int LocationsCount { get; set; }
    }

    public class SubCountyCreateDto
    {
        [Required(ErrorMessage = "Sub-county name is required.")]
        [StringLength(100, ErrorMessage = "Sub-county name cannot exceed 100 characters.")]
        public string SubCountyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "County is required.")]
        public int CountyId { get; set; }
    }

    public class SubCountyEditDto
    {
        public int SubCountyId { get; set; }

        [Required(ErrorMessage = "Sub-county name is required.")]
        [StringLength(100, ErrorMessage = "Sub-county name cannot exceed 100 characters.")]
        public string SubCountyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "County is required.")]
        public int CountyId { get; set; }
    }

    // Location DTOs
    public class LocationDto
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string SubCountyName { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;
        public int SubLocationsCount { get; set; }
    }

    public class LocationCreateDto
    {
        [Required(ErrorMessage = "Location name is required.")]
        [StringLength(100, ErrorMessage = "Location name cannot exceed 100 characters.")]
        public string LocationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sub-county is required.")]
        public int SubCountyId { get; set; }
    }

    public class LocationEditDto
    {
        public int LocationId { get; set; }

        [Required(ErrorMessage = "Location name is required.")]
        [StringLength(100, ErrorMessage = "Location name cannot exceed 100 characters.")]
        public string LocationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sub-county is required.")]
        public int SubCountyId { get; set; }
    }

    // SubLocation DTOs
    public class SubLocationDto
    {
        public int SubLocationId { get; set; }
        public string SubLocationName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string SubCountyName { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;
        public int VillagesCount { get; set; }
    }

    public class SubLocationCreateDto
    {
        [Required(ErrorMessage = "Sub-location name is required.")]
        [StringLength(100, ErrorMessage = "Sub-location name cannot exceed 100 characters.")]
        public string SubLocationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        public int LocationId { get; set; }
    }

    public class SubLocationEditDto
    {
        public int SubLocationId { get; set; }

        [Required(ErrorMessage = "Sub-location name is required.")]
        [StringLength(100, ErrorMessage = "Sub-location name cannot exceed 100 characters.")]
        public string SubLocationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        public int LocationId { get; set; }
    }

    public class CountyCreateDto
    {
        [Required(ErrorMessage = "County name is required.")]
        [StringLength(100, ErrorMessage = "County name cannot exceed 100 characters.")]
        public string CountyName { get; set; } = string.Empty;
    }

    public class CountyEditDto
    {
        public int CountyId { get; set; }

        [Required(ErrorMessage = "County name is required.")]
        [StringLength(100, ErrorMessage = "County name cannot exceed 100 characters.")]
        public string CountyName { get; set; } = string.Empty;
    }

    // Village DTOs
    public class VillageDto
    {
        public int VillageId { get; set; }
        public string VillageName { get; set; } = string.Empty;
        public string SubCountyName { get; set; } = string.Empty;
        public string CountyName { get; set; } = string.Empty;
    }

    public class VillageCreateDto
    {
        [Required(ErrorMessage = "Village name is required.")]
        [StringLength(100, ErrorMessage = "Village name cannot exceed 100 characters.")]
        public string VillageName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sub-county is required.")]
        public int SubCountyId { get; set; }
    }

    public class VillageEditDto
    {
        public int VillageId { get; set; }

        [Required(ErrorMessage = "Village name is required.")]
        [StringLength(100, ErrorMessage = "Village name cannot exceed 100 characters.")]
        public string VillageName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sub-county is required.")]
        public int SubCountyId { get; set; }
    }

    // Gender Category DTOs
    public class GenderCategoryDto
    {
        public int GenderCategoryId { get; set; }
        public string GenderName { get; set; } = string.Empty;
    }

    public class GenderCategoryCreateDto
    {
        [Required(ErrorMessage = "Gender name is required.")]
        [StringLength(50, ErrorMessage = "Gender name cannot exceed 50 characters.")]
        public string GenderName { get; set; } = string.Empty;
    }

    public class GenderCategoryEditDto
    {
        public int GenderCategoryId { get; set; }

        [Required(ErrorMessage = "Gender name is required.")]
        [StringLength(50, ErrorMessage = "Gender name cannot exceed 50 characters.")]
        public string GenderName { get; set; } = string.Empty;
    }

    // Marital Status DTOs
    public class MaritalStatusDto
    {
        public int MaritalStatusId { get; set; }
        public string MaritalStatusName { get; set; } = string.Empty;
    }

    public class MaritalStatusCreateDto
    {
        [Required(ErrorMessage = "Marital status name is required.")]
        [StringLength(50, ErrorMessage = "Marital status name cannot exceed 50 characters.")]
        public string MaritalStatusName { get; set; } = string.Empty;
    }

    public class MaritalStatusEditDto
    {
        public int MaritalStatusId { get; set; }

        [Required(ErrorMessage = "Marital status name is required.")]
        [StringLength(50, ErrorMessage = "Marital status name cannot exceed 50 characters.")]
        public string MaritalStatusName { get; set; } = string.Empty;
    }
}
