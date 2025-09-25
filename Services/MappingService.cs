using SAIS.Models;
using SAIS.Models.DTOs;

namespace SAIS.Services
{
    public class MappingService
    {
        // Applicant mappings
        public static ApplicantDto ToApplicantDto(Applicant applicant)
        {
            return new ApplicantDto
            {
                ApplicantId = applicant.ApplicantId,
                FirstName = applicant.FirstName,
                MiddleName = applicant.MiddleName,
                LastName = applicant.LastName,
                IdNumber = applicant.IdNumber,
                Age = applicant.Age,
                GenderId = applicant.GenderId,
                GenderName = applicant.GenderCategory?.GenderCategoryName,
                MaritalStatusId = applicant.MaritalStatusId,
                MaritalStatusName = applicant.MaritalStatus?.StatusName,
                VillageId = applicant.VillageId,
                VillageName = applicant.Village?.VillageName,
                PostalAddress = applicant.PostalAddress,
                PhysicalAddress = applicant.PhysicalAddress,
                DateOfBirth = applicant.DateOfBirth,
                PhoneNumbers = applicant.PhoneNumbers?.Select(p => p.Number).ToList() ?? new List<string>(),
                CountyName = applicant.Village?.SubLocation?.Location?.SubCounty?.County?.CountyName,
                SubCountyName = applicant.Village?.SubLocation?.Location?.SubCounty?.SubCountyName,
                LocationName = applicant.Village?.SubLocation?.Location?.LocationName,
                SubLocationName = applicant.Village?.SubLocation?.SubLocationName
            };
        }

        public static ApplicantListDto ToApplicantListDto(Applicant applicant)
        {
            return new ApplicantListDto
            {
                ApplicantId = applicant.ApplicantId,
                FullName = $"{applicant.FirstName} {applicant.MiddleName} {applicant.LastName}".Trim(),
                IdNumber = applicant.IdNumber,
                Age = applicant.Age,
                GenderName = applicant.GenderCategory?.GenderCategoryName ?? "",
                MaritalStatusName = applicant.MaritalStatus?.StatusName ?? "",
                VillageName = applicant.Village?.VillageName ?? "",
                CountyName = applicant.Village?.SubLocation?.Location?.SubCounty?.County?.CountyName ?? "",
                PhoneNumbers = string.Join(", ", applicant.PhoneNumbers?.Select(p => p.Number) ?? new List<string>()),
                DateOfBirth = applicant.DateOfBirth,
                ApplicationCount = applicant.Applications?.Count ?? 0
            };
        }

        public static Applicant ToApplicant(ApplicantCreateDto dto)
        {
            return new Applicant
            {
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                IdNumber = dto.IdNumber,
                Age = dto.Age,
                GenderId = dto.GenderId,
                MaritalStatusId = dto.MaritalStatusId,
                VillageId = dto.VillageId,
                PostalAddress = dto.PostalAddress,
                PhysicalAddress = dto.PhysicalAddress,
                DateOfBirth = dto.DateOfBirth
            };
        }

        public static void UpdateApplicant(Applicant applicant, ApplicantEditDto dto)
        {
            applicant.FirstName = dto.FirstName;
            applicant.MiddleName = dto.MiddleName;
            applicant.LastName = dto.LastName;
            applicant.IdNumber = dto.IdNumber;
            applicant.Age = dto.Age;
            applicant.GenderId = dto.GenderId;
            applicant.MaritalStatusId = dto.MaritalStatusId;
            applicant.VillageId = dto.VillageId;
            applicant.PostalAddress = dto.PostalAddress;
            applicant.PhysicalAddress = dto.PhysicalAddress;
            applicant.DateOfBirth = dto.DateOfBirth;
        }

        // Application mappings
        public static ApplicationDto ToApplicationDto(Application application)
        {
            return new ApplicationDto
            {
                ApplicationId = application.ApplicationId,
                ApplicationDate = application.ApplicationDate,
                ApplicantSignedDate = application.ApplicantSignedDate,
                OfficerSignedDate = application.OfficerSignedDate,
                ApplicantId = application.ApplicantId,
                ApplicantName = application.Applicant != null ?
                    $"{application.Applicant.FirstName} {application.Applicant.MiddleName} {application.Applicant.LastName}".Trim() : "",
                ApplicantIdNumber = application.Applicant?.IdNumber,
                OfficerId = application.OfficerId,
                OfficerName = application.Officer?.OfficerName ?? "",
                ProgramIds = application.AppliedPrograms?.Select(ap => ap.ProgramId).ToList() ?? new List<int>(),
                ProgramNames = application.AppliedPrograms?.Select(ap => ap.SocialAssistanceProgram.ProgramName).ToList() ?? new List<string>(),
                Status = DetermineApplicationStatus(application)
            };
        }

        public static ApplicationListDto ToApplicationListDto(Application application)
        {
            return new ApplicationListDto
            {
                ApplicationId = application.ApplicationId,
                ApplicationDate = application.ApplicationDate,
                ApplicantSignedDate = application.ApplicantSignedDate,
                OfficerSignedDate = application.OfficerSignedDate,
                ApplicantName = application.Applicant != null ?
                    $"{application.Applicant.FirstName} {application.Applicant.MiddleName} {application.Applicant.LastName}".Trim() : "",
                ApplicantIdNumber = application.Applicant?.IdNumber ?? "",
                OfficerName = application.Officer?.OfficerName ?? "",
                ProgramNames = string.Join(", ", application.AppliedPrograms?.Select(ap => ap.SocialAssistanceProgram.ProgramName) ?? new List<string>()),
                Status = DetermineApplicationStatus(application)
            };
        }

        public static ApplicationDetailsDto ToApplicationDetailsDto(Application application)
        {
            return new ApplicationDetailsDto
            {
                ApplicationId = application.ApplicationId,
                ApplicationDate = application.ApplicationDate,
                ApplicantSignedDate = application.ApplicantSignedDate,
                OfficerSignedDate = application.OfficerSignedDate,
                ApplicantId = application.ApplicantId,
                ApplicantName = application.Applicant != null ?
                    $"{application.Applicant.FirstName} {application.Applicant.MiddleName} {application.Applicant.LastName}".Trim() : "",
                ApplicantIdNumber = application.Applicant?.IdNumber ?? "",
                ApplicantPhone = string.Join(", ", application.Applicant?.PhoneNumbers?.Select(p => p.Number) ?? new List<string>()),
                ApplicantAddress = application.Applicant?.PhysicalAddress ?? "",
                OfficerId = application.OfficerId,
                OfficerName = application.Officer?.OfficerName ?? "",
                OfficerTitle = application.Officer?.Designation ?? "",
                Programs = application.AppliedPrograms?.Select(ap => new ProgramSelectionDto
                {
                    ProgramId = ap.SocialAssistanceProgram.ProgramId,
                    ProgramName = ap.SocialAssistanceProgram.ProgramName,
                    Description = "",
                    IsSelected = true
                }).ToList() ?? new List<ProgramSelectionDto>(),
                Status = DetermineApplicationStatus(application)
            };
        }

        public static Application ToApplication(ApplicationCreateDto dto)
        {
            return new Application
            {
                ApplicationDate = dto.ApplicationDate,
                ApplicantSignedDate = dto.ApplicantSignedDate ?? DateTime.MinValue,
                OfficerSignedDate = dto.OfficerSignedDate ?? DateTime.MinValue,
                ApplicantId = dto.ApplicantId ?? 0,
                OfficerId = dto.OfficerId ?? 0
            };
        }

        public static void UpdateApplication(Application application, ApplicationEditDto dto)
        {
            application.ApplicationDate = dto.ApplicationDate;
            application.ApplicantSignedDate = dto.ApplicantSignedDate ?? DateTime.MinValue;
            application.OfficerSignedDate = dto.OfficerSignedDate ?? DateTime.MinValue;
            application.OfficerId = dto.OfficerId;
        }

        // Officer mappings
        public static OfficerDto ToOfficerDto(Officer officer)
        {
            return new OfficerDto
            {
                OfficerId = officer.OfficerId,
                OfficerName = officer.OfficerName,
                Designation = officer.Designation,
                ApplicationCount = officer.Applications?.Count ?? 0
            };
        }

        public static OfficerListDto ToOfficerListDto(Officer officer)
        {
            return new OfficerListDto
            {
                OfficerId = officer.OfficerId,
                OfficerName = officer.OfficerName,
                Designation = officer.Designation,
                ApplicationCount = officer.Applications?.Count ?? 0
            };
        }

        public static Officer ToOfficer(OfficerCreateDto dto)
        {
            return new Officer
            {
                OfficerName = dto.OfficerName,
                Designation = dto.Designation
            };
        }

        public static void UpdateOfficer(Officer officer, OfficerEditDto dto)
        {
            officer.OfficerName = dto.OfficerName;
            officer.Designation = dto.Designation;
        }

        // SocialAssistanceProgram mappings
        public static SocialAssistanceProgramDto ToSocialAssistanceProgramDto(SocialAssistanceProgram program)
        {
            return new SocialAssistanceProgramDto
            {
                ProgramId = program.ProgramId,
                ProgramName = program.ProgramName,
                ApplicationCount = program.AppliedPrograms?.Count ?? 0
            };
        }

        public static SocialAssistanceProgramListDto ToSocialAssistanceProgramListDto(SocialAssistanceProgram program)
        {
            return new SocialAssistanceProgramListDto
            {
                ProgramId = program.ProgramId,
                ProgramName = program.ProgramName,
                ApplicationCount = program.AppliedPrograms?.Count ?? 0
            };
        }

        public static SocialAssistanceProgram ToSocialAssistanceProgram(SocialAssistanceProgramCreateDto dto)
        {
            return new SocialAssistanceProgram
            {
                ProgramName = dto.ProgramName
            };
        }

        public static void UpdateSocialAssistanceProgram(SocialAssistanceProgram program, SocialAssistanceProgramEditDto dto)
        {
            program.ProgramName = dto.ProgramName;
        }

        // Helper methods
        private static string DetermineApplicationStatus(Application application)
        {
            if (application.OfficerSignedDate != DateTime.MinValue)
                return "Approved";
            if (application.ApplicantSignedDate != DateTime.MinValue)
                return "Pending Officer Approval";
            return "Draft";
        }
    }
}
