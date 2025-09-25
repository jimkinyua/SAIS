using Microsoft.EntityFrameworkCore;
using SAIS.Models;

namespace SAIS.Models.Data
{
    public class SAISDbContext : DbContext
    {
        public SAISDbContext(DbContextOptions<SAISDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique constraint for IdNumber
            modelBuilder.Entity<Applicant>()
                .HasIndex(a => a.IdNumber)
                .IsUnique();

            // Configure many-to-many relationship for AppliedProgram
            modelBuilder.Entity<AppliedProgram>()
                .HasKey(ap => new { ap.ApplicationId, ap.ProgramId });

            modelBuilder.Entity<AppliedProgram>()
                .HasOne(ap => ap.Application)
                .WithMany(a => a.AppliedPrograms)
                .HasForeignKey(ap => ap.ApplicationId);

            modelBuilder.Entity<AppliedProgram>()
                .HasOne(ap => ap.SocialAssistanceProgram)
                .WithMany(sap => sap.AppliedPrograms)
                .HasForeignKey(ap => ap.ProgramId);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed GenderCategory
            modelBuilder.Entity<GenderCategory>().HasData(
                new GenderCategory { GenderCategoryId = 1, GenderCategoryName = "Male" },
                new GenderCategory { GenderCategoryId = 2, GenderCategoryName = "Female" }
            );

            // Seed MaritalStatus
            modelBuilder.Entity<MaritalStatus>().HasData(
                new MaritalStatus { MaritalStatusId = 1, StatusName = "Single" },
                new MaritalStatus { MaritalStatusId = 2, StatusName = "Married" },
                new MaritalStatus { MaritalStatusId = 3, StatusName = "Divorced" },
                new MaritalStatus { MaritalStatusId = 4, StatusName = "Widowed" }
            );

            // Seed SocialAssistanceProgram
            modelBuilder.Entity<SocialAssistanceProgram>().HasData(
                new SocialAssistanceProgram { ProgramId = 1, ProgramName = "Orphans & Vulnerable" },
                new SocialAssistanceProgram { ProgramId = 2, ProgramName = "Elderly Persons" },
                new SocialAssistanceProgram { ProgramId = 3, ProgramName = "Disability" },
                new SocialAssistanceProgram { ProgramId = 4, ProgramName = "Extreme Poverty" },
                new SocialAssistanceProgram { ProgramId = 5, ProgramName = "Other" }
            );

            // Seed geographic hierarchy
            modelBuilder.Entity<County>().HasData(
                new County { CountyId = 1, CountyName = "Nairobi" }
            );

            modelBuilder.Entity<SubCounty>().HasData(
                new SubCounty { SubCountyId = 1, SubCountyName = "Westlands", CountyId = 1 }
            );

            modelBuilder.Entity<Location>().HasData(
                new Location { LocationId = 1, LocationName = "Kitisuru", SubCountyId = 1 }
            );

            modelBuilder.Entity<SubLocation>().HasData(
                new SubLocation { SubLocationId = 1, SubLocationName = "Kitisuru Estate", LocationId = 1 }
            );

            modelBuilder.Entity<Village>().HasData(
                new Village { VillageId = 1, VillageName = "Kitisuru Village", SubLocationId = 1 }
            );

            // Seed sample officer
            modelBuilder.Entity<Officer>().HasData(
                new Officer { OfficerId = 1, OfficerName = "John Mwangi", Designation = "Social Worker" }
            );
        }

        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Officer> Officers { get; set; }
        public DbSet<SocialAssistanceProgram> SocialAssistancePrograms { get; set; }
        public DbSet<AppliedProgram> AppliedPrograms { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<SubCounty> SubCounties { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SubLocation> SubLocations { get; set; }
        public DbSet<Village> Villages { get; set; }
        public DbSet<GenderCategory> GenderCategories { get; set; }
        public DbSet<MaritalStatus> MaritalStatuses { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
    }
}
